//const uri = "https://localhost:44354/api/itemsapi";
const uri = "https://mitchellmeasures.azurewebsites.net/api/itemsapi";
let todos = null;

let count = 0

let cost = 0;

let items = [];




function getCount(data) {
	const el = $("#counter");
	let name = "to-do";
	if (data) {
		if (data > 1) {
			name = "to-dos";
		}
		el.text(data + " " + name);
	} else {
		el.text("No " + name);
	}
}


$(document).ready(function () {
	$("#addItem").click(function () { });
	$.ajax({
		type: "GET",
		accepts: "application/json",
		url: uri,
		contentType: "application/json",
		error: function (jqXHR, textStatus, errorThrown) {
			alert("Something went wrong!");
		},
		success: function (result) {
			var data = result;
			if (data != null) {
				$.each(data, function (index, obj) {
					obj.identifier = obj.position;
					items.push(obj);
					displayData(obj);
				})
				recalculateCost();
			}
		}
	})
});


function deleteData(id) {

	let identifier = id.slice(6);
	console.log(identifier);
	var $tr = $(this).closest('tr');

	var it;

	for (var i = 0; i < items.length; i++) {
		if (items[i].identifier == identifier) {
			it = items[i];
		}
	}

	items.splice(items.indexOf(it), 1);


	var price = $("#cost" + identifier).text();


	cost -= price;

	$("#totalCost").text(cost);

	var deposit = cost * 0.2;

	$("#deposit").text(deposit);

	recalculateCost();

	const item = {
		address: $("#address" + identifier).text(),
		sqft: $("#sqft" + identifier).text(),
		cost: $("#cost" + identifier).text(),
		isComplete: false
	};

	console.log(item.address + ", " + item.sqft);

	$.ajax({
		type: "DELETE",
		accepts: "application/json",
		url: uri,
		contentType: "application/json",
		data: JSON.stringify(item),
		error: function (jqXHR, textStatus, errorThrown) {
			alert("Something went wrong!");
		},
		success: function (result) {
			console.log("Item Deleted")
			$("#address" + identifier).remove();
			$("#sqft" + identifier).remove();
			$("#delete" + identifier).remove();
			$("#row" + identifier).remove();
			$("#" + id).html();
			count--;
			console.log(items);

		}
	});



}

function recalculateCost() {
	var totalCost = 0
	for (var i = 0; i < items.length; i++) {
		let pos = items[i].identifier;
		items[i].position = i + 1;
		calculateCost(items[i]);
		console.log(pos + ": " + items[i].address + " -- price: " + items[i].price + " -- position: " + items[i].position);
		$('#cost' + pos).text(items[i].price);
		totalCost += items[i].price;
	}

	$("#totalCost").text(totalCost);
	$("#deposit").text(totalCost * 0.2);

}

function calculateCost(item) {
	var sqftrate = 0;
	switch (item.position) {
		case 1:
			sqftrate = 0.25;
			break;
		case 2:
			sqftrate = 0.20;
			break;
		default:
			sqftrate = 0.15;
			break;
	}
	var baseCost = 200;
	var multiplier = item.sqft - 1000;
	var extraCost = 0;
	if (multiplier > 0) {
		extraCost = multiplier * sqftrate;
	}
	item.price = baseCost + extraCost;
	return item.price;
}

function displayData(item) {
	const tBody = $("#item");

	cost += item.estimateCost;

	$("#totalCost").text(cost);

	var deposit = cost * 0.2;

	$("#deposit").text(deposit);

	console.log("displaying item");
	const tr = $("<tr id=\"row" + item.position + "\">");
	tr.append($("<td id=\"address" + item.position + "\"></td>").text(item.address));
	tr.append($("<td id=\"sqft" + item.position + "\"></td>").text(item.sqft));
	tr.append($("<td id=\"cost" + item.position + "\"></td>").text(item.estimateCost));
	tr.append("<td><button class=\"btn btn-secondary\" id=\"delete" + item.position + "\" onClick=\"deleteData(this.id)\">Delete</button></td>");
	tr.append("</tr>")
	tr.appendTo(tBody);
}

function addItem() {
	var address = String(document.getElementById("add-address").value)
	var sqft = Number(document.getElementById("add-sqft").value)
	if (Number.isInteger(sqft) && address !== "" && address.length < 40) {

		const item = {
			address: $("#add-address").val(),
			sqft: $("#add-sqft").val(),
			position: items.length + 1,
			identifier: items.length + 1,
			isComplete: false
		};

		items.push(item);

		console.log(items);

		$.ajax({
			type: "POST",
			accepts: "application/json",
			url: uri,
			contentType: "application/json",
			data: JSON.stringify(item),
			error: function (jqXHR, textStatus, errorThrown) {
				alert("Something went wrong!");
			},
			success: function (result) {
				//getData();
				displayData(result);
				$("#add-address").val("");
				$("#add-sqft").val("");
				count++;

			}

		});
	} else {
		alert("Invalid Value; SqFt must be a number and Address must be less then 20 Characters")
		return false;
	}
}
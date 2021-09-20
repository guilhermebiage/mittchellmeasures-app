//const url = "https://localhost:44354/api/itemsapi/"
const url = "https://mitchellmeasures.azurewebsites.net/api/itemsapi/"
$('#orderSelect').change(function () {

    var id = $('#orderSelect option:selected').val();
    
    $.ajax({
        type: "GET",
        url: url + id,
        accepts: "application/json",
        contentType: "application/json",
        success: function (result) {
            $("#itemSelect").html("");
            for (var i = 0; i < result.length; i++) {
                $("#itemSelect").append("<option label=\"" + result[i].address+ "\" value=\"" + result[i].itemId + "\">"
                    + result[i].address + "</option>");
            }
        }

    })
})
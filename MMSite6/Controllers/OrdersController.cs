using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MMSite6.Areas.Identity.Data;
using MMSite6.Models;
using static Microsoft.AspNetCore.Identity.UI.V3.Pages.Account.Internal.ExternalLoginModel;

namespace MMSite6.Controllers
{
    public class OrdersController : Controller
    {
        private readonly MMSite6Context _context;
        private readonly UserManager<MMSite6User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private List<Item> _items = new List<Item>();
        private ItemsAPIController _itemsAPIController;
        private OrdersAPIController _ordersAPIController;
        private readonly IEmailSender _emailSender;
        public readonly InputModel _inputModel;

        public OrdersController(MMSite6Context context, UserManager<MMSite6User> userManager, RoleManager<IdentityRole> roleManager, IEmailSender emailSender)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _emailSender = emailSender;
            _itemsAPIController = new ItemsAPIController(_context);
            _ordersAPIController = new OrdersAPIController(_context);
        }

        // GET: Orders

        [Authorize(Roles = "User, Admin")]
        public async Task<IActionResult> Index(string sortOrder, string searchString, string statusString, string currentFilter, string currentStatus, int? pageNumber)
        {
            var userId = _userManager.GetUserId(HttpContext.User);
            var orders = from o in _context.order
								select o;
			List<Order> usersOrders = new List<Order>();
            var user = await _userManager.FindByIdAsync(userId);

			ViewData["CurrentSort"] = sortOrder;
			ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
			ViewBag.StatusSortParm = String.IsNullOrEmpty(sortOrder) ? "status_desc" : "";
			ViewBag.CustSortParm = String.IsNullOrEmpty(sortOrder) ? "customer_desc" : "";
			ViewBag.DateSortParm = sortOrder == "Date" ? "date_desc" : "Date";

			if (searchString != null)
			{
				pageNumber = 1;
			}
			else
			{
				searchString = currentFilter;
			}

			if (statusString != null)
			{
				pageNumber = 1;
			}
			else
			{
				statusString = currentStatus;
			}

			ViewData["CurrentFilter"] = searchString;
			ViewData["CurrentStatus"] = statusString;

			if (!String.IsNullOrEmpty(searchString))
			{
				orders = orders.Where(s => s.summary.Contains(searchString) ||
				 s.user.firstName.Contains(searchString) || s.user.lastName.Contains(searchString));
			}

			if (!String.IsNullOrEmpty(statusString))
			{
				orders = orders.Where(s => s.status.ToString().Contains(statusString));
			}

			//ViewData["orderStatus"] = new SelectList(_context.order, "orderId", "status");
			//SelectList statusList = new SelectList(_context.order, "orderId", "status");

			ViewBag.statuses = new SelectList(new[]
				{
					new { id = "", name = "All" },
					new { id = "Opened", name = "Opened" },
					new { id = "InProgress", name = "InProgress" },
					new { id = "Completed", name = "Completed" },
					new { id = "Cancelled", name = "Cancelled" },
					new { id = "Paid", name = "Paid" },
				},
				"id", "name", statusString);

			switch (sortOrder)
			{
				case "name_desc":
					orders = orders.OrderBy(m => m.summary);
					break;
				case "Date":
					orders = orders.OrderBy(s => s.orderDate);
					break;
				case "status_desc":
					orders = orders.OrderBy(s => s.status);
					break;
				case "customer_desc":
					orders = orders.OrderBy(s => s.user.firstName);
					break;
				default:
					orders = orders.OrderBy(s => s.summary);
					break;
			}

			orders = orders.Include(m => m.user);

			if (!User.IsInRole("Admin"))
            {
                foreach (var order in orders)
                {
                    if (user == order.user)
                    {
                        usersOrders.Add(order);
                    }
                }
            } else
            {
                foreach (var order in orders)
                {
                    usersOrders.Add(order);
                }
            }

			int pageSize = 5;
			return View(PaginatedList<Order>.Create(usersOrders.AsQueryable().AsNoTracking(), pageNumber ?? 1, pageSize));
		}

		[Authorize(Roles = "Admin")]
		public ActionResult UnpaidOrders()
        {
            var orders = from o in _context.order
                         select o;
            orders = orders.Where(order => order.status == OrderStatus.Completed);
            
            orders = orders.Include(order => order.user);

            return View(orders);
            
        }

		[Authorize(Roles = "Admin")]
		public ActionResult IncompleteOrders()
        {
            var orders = from o in _context.order
                         select o;
            orders = orders.Where(order => order.status == OrderStatus.InProgress || order.status == OrderStatus.Opened);

            orders = orders.Include(order => order.user);
            orders = orders.Include(order => order.item);

            return View(orders);

        }

        // GET: Orders/Details/5
        [Authorize(Roles = "User, Admin")]
        public async Task<IActionResult> Details(int id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ordera = await _context.order
                .Include(m => m.user)
                .Include(m => m.item)
                .FirstOrDefaultAsync(m => m.orderId == id);
            if (ordera == null)
            {
                return NotFound();
            }

            double total = 0;

            foreach (var item in ordera.item)
            {
                total += (double)item.estimateCost;
            }

            var items = _itemsAPIController.GetItemAndDocument(ordera.orderId).Result.Value;

            OrderDetailsViewModel orderDetailsViewModel = new OrderDetailsViewModel()
            {
                order = ordera,

                //await itemsAPIController.GetItem(id)

                item = items

            };

            ViewBag.totalCost = total;

            return View(orderDetailsViewModel);
        }

		// GET: Orders/Create
		[Authorize(Roles = "User, Admin")]
		public IActionResult Create()
        {
            var userId = _userManager.GetUserId(HttpContext.User);
            if (String.IsNullOrEmpty(userId))
            {
                return Redirect("https://mitchellmeasures.azurewebsites.net/"); //CHANGE THIS ON LIVE SERVER
            }

            return View();
        }

		// POST: Orders/Create
		// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
		// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
		[Authorize(Roles = "User, Admin")]
		[HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("orderId,summary,desc")] Order order)
        {
            var userId = _userManager.GetUserId(HttpContext.User);
            var u = await _userManager.FindByIdAsync(userId);
            var itemList = HttpContext.Session.GetComplexData<List<Item>>("itemList");
            
            if (ModelState.IsValid)
            {
                if (itemList != null)
                {
                    if(itemList.Count > 0){ 
                        //order.item = itemList;
                        order.status = OrderStatus.Opened;
                        order.orderDate = DateTime.Now;

                        order.user = u;

                        HttpContext.Session.SetComplexData("itemList", null);
						var total = 0.0;
                        var email = u.Email;
						var mitchellEmail = "mitchell.measures@gmail.com"; //Replace with Kevin's Email when Deployed
						var custName = u.firstName + " " + u.lastName;
                        System.Diagnostics.Debug.WriteLine(email);
						string itemList2 = "";
						for (int i = 0; i < itemList.Count; i++)
						{
							itemList[i].position = i + 1;
							itemList[i].CalculateCost(itemList[i].position);
							itemList2 += "<tr><td style = 'padding: 0px 5px 2px 5px;'> " + itemList[i].address + " </td><td style = 'padding: 0px 5px 2px 5px;'> " + itemList[i].sqft + " sqft</td> <td style = 'padding: 0px 5px 2px 5px;'> $ "+ itemList[i].estimateCost +" </td> </tr>";
							total += itemList[i].estimateCost;
						}
						order.item = itemList;
						order.totalEstimateCost = total;
                        order.deposit = total * 0.2;

                        _context.Add(order);
                        await _context.SaveChangesAsync();

                        await _emailSender.SendEmailAsync(
                                email,
                                "Order Created - Mitchell Measures",
                                $"Your Order has been created!" +
								$"<br><br>" +
								$"Full Name: {custName}" +
								$"<br>" +
								$"Order Id: {order.orderId}" +
								$"<br>" +
                                $"Order Summary: {order.summary}" +
                                $"<br><br>" +
                                $"Order Description: {order.desc}" +
								$"<br>" +
								$"<b>Please make an E-Transfer payment with the desposit amount below to mitchell.measures@gmail.com with the Order Id or Summary in the notes for payment. </b><br/>" +
								$"<b>Once paid, you will receive an email within the next few days to schedule an appointment for measurements.</b> <br/>" +
								$"<br><table border='1' style = 'border-collapse: collapse;'>" +
								$"<tr><th style = 'padding: 0px 5px 2px 5px;'>Address</th><th style = 'padding: 0px 5px 2px 5px;'>Approx. Size</th><th>Price</th></tr>" +
								$"{itemList2}" +
								$"</table><br>" +
								$"<h3>Desposit Amount: ${order.deposit}</h3> <h3>Estimated Remaining Amount: ${total}</h3> <br/><br/>" +
								$"<img src='https://mitchellmeasures.blob.core.windows.net/logo/logo.png' style='width: 186px; height: 95px;' cursor: 'pointer';></img>");

						await _emailSender.SendEmailAsync(
								mitchellEmail,
                                "Order Created - Mitchell Measures",
								$"An Order has been created! - Awaiting Payment from Customer" +
								$"<br><br>" +
								$"Customer Name: {custName}" +
								$"<br><br>" +
								$"Order Id: {order.orderId}" +
                                $"<br><br>" +
                                $"Order Summary: {order.summary}" +
                                $"<br><br>" +
                                $"Order Description: {order.desc}" +
                                $"<br><br><table border='1' style = 'border-collapse: collapse;'>" +
								$"<tr><th style = 'padding: 0px 5px 2px 5px;'>Address</th><th style = 'padding: 0px 5px 2px 5px;'>Approx. Size</th><th>Price</th></tr>" +
								$"{itemList2}" +
								$"</table><br>" +
								$"<h3>Desposit Amount: ${order.deposit}</h3><h3>Estimated Order Total: ${total}</h3> <br/><br/>" +
								$"<img src='https://mitchellmeasures.blob.core.windows.net/logo/logo.png' style='width: 186px; height: 95px;' cursor: 'pointer';></img>");

                        return RedirectToAction(nameof(Index));
                    }
                }
                ViewBag.EmptyItems = "Please add a Location to the order";
            }
        
        return View(order);

        }

        [HttpPost]
        public ActionResult AddItem([Bind("address, sqft")] Item item)
        {
            if (ModelState.IsValid)
            {
                _items.Add(item);
                System.Diagnostics.Debug.WriteLine(_items.Count);
            }
            return PartialView("/Views/Shared/_ItemPartial.cshtml");
        }
		// GET: Orders/Edit/5
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> Edit(int? id)
        {
            var userId = _userManager.GetUserId(HttpContext.User);
            var roles = await _roleManager.FindByIdAsync(userId);
            var user = await _userManager.FindByIdAsync(userId);

            if (String.IsNullOrEmpty(userId) || !await _userManager.IsInRoleAsync(user, "Admin"))
            { 
                return Redirect("https://mitchellmeasures.azurewebsites.net/");
            }

            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.order.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            return View(order);
        }

        // POST: Orders/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("orderId,summary,desc,status")] Order order)
        {
            if (id != order.orderId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var order2 = _ordersAPIController.GetOrder(id).Result.Value;

					order2.desc = order.desc;
                    order2.summary = order.summary;
                    order2.status = order.status;
					var custEmail = order2.user.Email;
					var currentStatus = order2.status;
					var currentDesc = order2.desc;
					var currentSummary = order2.summary;
					var finalTotal = order2.totalEstimateCost;

                    _context.Update(order2);
                    await _context.SaveChangesAsync();

					if (order2.status == OrderStatus.Paid)
					{
						await _emailSender.SendEmailAsync(
						custEmail,
						"Your Floorplans are Ready! - Mitchell Measures",
						$"Your Floorplans are available to download - Please visit http://mitchellmeasures.com/ and access the Order Details" +
						$"<br><br>" +
						$"Current Order Status: {currentStatus}" +
						$"<br><br>" +
						$"Order Summary: {currentSummary}" +
						$"<br><br>" +
						$"Order Description: {currentDesc}" +
						$"<br><br>" +
						$"<img src='https://mitchellmeasures.blob.core.windows.net/logo/logo.png' style='width: 186px; height: 95px;' cursor: 'pointer';></img>");
					}
					if (order2.status == OrderStatus.Completed)
					{
						await _emailSender.SendEmailAsync(
						custEmail,
						"Your Floorplans have been created! - Mitchell Measures",
						$"Your Floorplans have been completed - They will be available upon receiving payment of the remaining cost." +
						$"<br><br>" +
						$"Remaining Balance: ${finalTotal}" +
						$"<br><br>" +
						$"Current Order Status: {currentStatus}" +
						$"<br><br>" +
						$"Order Summary: {currentSummary}" +
						$"<br><br>" +
						$"Order Description: {currentDesc}" +
						$"<br><br>" +
						$"<img src='https://mitchellmeasures.blob.core.windows.net/logo/logo.png' style='width: 186px; height: 95px;' cursor: 'pointer';></img>");
					}
					if (order2.status == OrderStatus.InProgress)
					{
						await _emailSender.SendEmailAsync(
						custEmail,
						"Order In Progress - Mitchell Measures",
						$"Your Order is currently in progress - http://mitchellmeasures.com/" +
						$"<br><br>" +
						$"Current Order Status: {currentStatus}" +
						$"<br><br>" +
						$"Order Summary: {currentSummary}" +
						$"<br><br>" +
						$"Order Description: {currentDesc}" +
						$"<br><br>" +
						$"<img src='https://mitchellmeasures.blob.core.windows.net/logo/logo.png' style='width: 186px; height: 95px;' cursor: 'pointer';></img>");
					}
					if (order2.status == OrderStatus.Cancelled)
					{
						await _emailSender.SendEmailAsync(
						custEmail,
						"Order Cancelled - Mitchell Measures",
						$"Order has been cancelled - Please contact mitchell.measures@gmail.com for more information" +
						$"<br><br>" +
						$"Current Order Status: {currentStatus}" +
						$"<br><br>" +
						$"Order Summary: {currentSummary}" +
						$"<br><br>" +
						$"Order Description: {currentDesc}" +
						$"<br><br>" +
						$"<img src='https://mitchellmeasures.blob.core.windows.net/logo/logo.png' style='width: 186px; height: 95px;' cursor: 'pointer';></img>");
					}

				}
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(order.orderId))
                    {
                        return NotFound();
                     }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(order);
        }

		// GET: Orders/Delete/5
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.order
                .FirstOrDefaultAsync(m => m.orderId == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

		// POST: Orders/Delete/5
		[Authorize(Roles = "Admin")]
		[HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var order = await _context.order.FindAsync(id);
            _context.order.Remove(order);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrderExists(int id)
        {
            return _context.order.Any(e => e.orderId == id);
        }
    }
}

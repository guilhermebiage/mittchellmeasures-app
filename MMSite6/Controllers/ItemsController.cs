using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MMSite6.Models;

namespace MMSite6.Controllers
{
	public class ItemsController : Controller
	{
		private readonly MMSite6Context _context;
        private ItemsAPIController _itemsAPIController;
		private OrdersAPIController _ordersAPIController;

		public ItemsController(MMSite6Context context)
		{
			_context = context;
            _itemsAPIController = new ItemsAPIController(_context);
			_ordersAPIController = new OrdersAPIController(_context);
		}
		[Authorize(Roles = "Admin")]
		// GET: Items
		public async Task<IActionResult> Index()
		{
			var mMSite6Context = _context.item.Include(i => i.order);
			return View(await mMSite6Context.ToListAsync());
		}
		[Authorize(Roles = "Admin")]
		// GET: Items/Details/5
		public async Task<IActionResult> Details(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var item = await _context.item
				.Include(i => i.order)
				.FirstOrDefaultAsync(m => m.itemId == id);
			if (item == null)
			{
				return NotFound();
			}

			return View(item);
		}
		[Authorize(Roles = "Admin")]
		// GET: Items/Create
		public IActionResult Create()
		{
			ViewData["orderId"] = new SelectList(_context.order, "orderId", "summary");
			return View();
		}
		[Authorize(Roles = "Admin")]
		// POST: Items/Create
		// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
		// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create([Bind("itemId,address,sqft,orderId")] Item item)
		{
			if (ModelState.IsValid)
			{
				_context.Add(item);
				await _context.SaveChangesAsync();
				return RedirectToAction(nameof(Index));
			}
			ViewData["orderId"] = new SelectList(_context.order, "orderId", "summary", item.orderId);
			return View(item);
		}
		[Authorize(Roles = "Admin")]
		// GET: Items/Edit/5
		public async Task<IActionResult> Edit(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var item = await _context.item.FindAsync(id);
			if (item == null)
			{
				return NotFound();
			}
			ViewData["orderId"] = new SelectList(_context.order, "orderId", "desc", item.orderId);
			return View(item);
		}
		[Authorize(Roles = "Admin")]
		// POST: Items/Edit/5
		// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
		// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(int id, [Bind("itemId,address,sqft,orderId, position")] Item item)
		{
			if (id != item.itemId)
			{
				return NotFound();
			}

			if (ModelState.IsValid)
			{
				try
				{
                    item.CalculateCost(item.position);
					_context.Update(item);

					var order = _ordersAPIController.GetOrder(item.orderId).Result.Value;
					order.totalEstimateCost = 0;
					foreach (var i in order.item)
					{
						order.totalEstimateCost += i.estimateCost;
					}
					_context.Update(order);

					await _context.SaveChangesAsync();
				}
				catch (DbUpdateConcurrencyException)
				{
					if (!ItemExists(item.itemId))
					{
						return NotFound();
					}
					else
					{
						throw;
					}
				}
				return Redirect("https://mitchellmeasures.azurewebsites.net/Orders");
			}
			ViewData["orderId"] = new SelectList(_context.order, "orderId", "desc", item.orderId);
			return View(item);
		}
		[Authorize(Roles = "Admin")]
		// GET: Items/Delete/5
		public async Task<IActionResult> Delete(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var item = await _context.item
				.Include(i => i.order)
				.FirstOrDefaultAsync(m => m.itemId == id);
			if (item == null)
			{
				return NotFound();
			}

			return View(item);
		}
		[Authorize(Roles = "Admin")]
		// POST: Items/Delete/5
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirmed(int id)
		{
			var item = await _context.item.FindAsync(id);
			_context.item.Remove(item);
			await _context.SaveChangesAsync();
			return RedirectToAction(nameof(Index));
		}

		private bool ItemExists(int id)
		{
			return _context.item.Any(e => e.itemId == id);
		}
	}
}
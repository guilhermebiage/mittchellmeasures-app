using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MMSite6.Models;
using Microsoft.AspNetCore.Session;
using Microsoft.AspNetCore.Authorization;

namespace MMSite6.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ItemsAPIController : ControllerBase
	{
		private readonly MMSite6Context _context;
		private IItemRepository _itemRepository;

		public ItemsAPIController(MMSite6Context context)
		{
			_context = context;
			_itemRepository = new ItemRepository(_context);
		}

		// GET: api/ItemsAPI
		//[Authorize(Roles = "User, Admin")]
		//[HttpGet]
		public async Task<ActionResult<IEnumerable<Item>>> Getitem()
		{
			return await _context.item.ToListAsync();
		}
		//[Authorize(Roles = "User, Admin")]
		[HttpGet]
		public async Task<ActionResult<IEnumerable<Item>>> GetItemCart()
		{
			return HttpContext.Session.GetComplexData<List<Item>>("itemList"); ;
		}
		//[Authorize(Roles = "User, Admin")]
		// GET: api/ItemsAPI/5
		//[HttpGet]
		public async Task<ActionResult<Item>> GetItembyId(int? id)
		{

			var item = await _context.item.FindAsync(id);

			if (item == null)
			{
				return NotFound();
			}


			return item;
		}


		//[Authorize(Roles = "User, Admin")]
		[HttpGet("{id}")]
		public async Task<ActionResult<List<Item>>> GetItem(int? id)
		{
			var items = await _context.item.Where(m => m.orderId == id).ToListAsync();
			return items;
		}
		//[Authorize(Roles = "User, Admin")]
		public async Task<ActionResult<List<Item>>> GetItemAndDocument(int? id)
		{
			var items = await _context.item.Where(m => m.orderId == id).Include(m => m.document).ToListAsync();
			return items;
		}
		[Authorize(Roles = "User, Admin")]
		// PUT: api/ItemsAPI/5
		[HttpPut("{id}")]
		public async Task<IActionResult> PutItem(int id, Item item)
		{
			if (id != item.itemId)
			{
				return BadRequest();
			}

			_context.Entry(item).State = EntityState.Modified;

			try
			{
				await _context.SaveChangesAsync();
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!ItemExists(id))
				{
					return NotFound();
				}
				else
				{
					throw;
				}
			}

			return NoContent();
		}
		[Authorize(Roles = "User, Admin")]
		// POST: api/ItemsAPI
		[HttpPost]
		public async Task<ActionResult<Item>> PostItem(Item item)
		{

			var itemList = HttpContext.Session.GetComplexData<List<Item>>("itemList");
			if (itemList == null)
			{
				itemList = new List<Item>();
			}
			int count = itemList.Count + 1;
			item.CalculateCost(count);
			itemList.Add(item);
			HttpContext.Session.SetComplexData("itemList", itemList);

			System.Diagnostics.Debug.WriteLine(itemList.Count);
			for (int i = 0; i < itemList.Count; i++)
			{
				System.Diagnostics.Debug.WriteLine(itemList[i].address + ", " + itemList[i].sqft);
			}


			return CreatedAtAction("GetItem", item);
		}
		[Authorize(Roles = "User, Admin")]
		// DELETE: api/ItemsAPI/5
		[HttpDelete]
		public async Task<ActionResult<Item>> DeleteItem(Item item)
		{
			var itemList = HttpContext.Session.GetComplexData<List<Item>>("itemList");
			System.Diagnostics.Debug.WriteLine(itemList.Count);
			for (int j = itemList.Count - 1; j >= 0; j--)
			{
				var i = itemList[j];
				var bool1 = item.address.Equals(i.address);
				var bool2 = item.sqft.ToString().Equals(i.sqft.ToString());

				System.Diagnostics.Debug.WriteLine(item.address + " " + i.address);
				System.Diagnostics.Debug.WriteLine(item.sqft.ToString() + " " + i.sqft.ToString());

				if (bool1 && bool2)
				{
					var i2 = i;
					itemList.Remove(i2);

				}
			}
			HttpContext.Session.SetComplexData("itemList", itemList);
			System.Diagnostics.Debug.WriteLine(itemList.Count);
			for (int i = 0; i < itemList.Count; i++)
			{
				System.Diagnostics.Debug.WriteLine(itemList[i].address + ", " + itemList[i].sqft);
			}

			return item;
		}

		private bool ItemExists(int id)
		{
			return _context.item.Any(e => e.itemId == id);
		}
	}
}

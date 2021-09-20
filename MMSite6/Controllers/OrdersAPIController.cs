using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MMSite6.Models;

namespace MMSite6.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersAPIController : ControllerBase
    {
        private readonly MMSite6Context _context;

        public OrdersAPIController(MMSite6Context context)
        {
            _context = context;
        }
		[Authorize(Roles = "User, Admin")]
		// GET: api/OrdersAPI
		[HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrder()
        {
            return await _context.order.Include(m=>m.user).ToListAsync();
        }
		[Authorize(Roles = "User, Admin")]
		// GET: api/OrdersAPI/5
		[HttpGet("{id}")]
        public async Task<ActionResult<Order>> GetOrder(int id)
        {
            var order = await _context.order.Include(m=>m.user).Where(m=>m.orderId == id).FirstOrDefaultAsync();
			order = await _context.order.Include(i => i.item).Where(i => i.orderId == id).FirstOrDefaultAsync();

			if (order == null)
            {
                return NotFound();
            }

            return order;
        }
		[Authorize(Roles = "User, Admin")]
		// PUT: api/OrdersAPI/5
		[HttpPut("{id}")]
        public async Task<IActionResult> PutOrder(int id, Order order)
        {
            if (id != order.orderId)
            {
                return BadRequest();
            }

            _context.Entry(order).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderExists(id))
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
		[HttpPut("{id}")]
        public async Task<IActionResult> UpdateStatus(int id, OrderStatus status)
        {

            var order = await _context.order.FindAsync(id);
            if (order != null)
            {
                order.status = status;
            }
            _context.Entry(order).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderExists(id))
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
		// POST: api/OrdersAPI
		[HttpPost]
        public async Task<ActionResult<Order>> PostOrder(Order order)
        {
            _context.order.Add(order);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetOrder", new { id = order.orderId }, order);
        }
		[Authorize(Roles = "Admin")]
		// DELETE: api/OrdersAPI/5
		[HttpDelete("{id}")]
        public async Task<ActionResult<Order>> DeleteOrder(int id)
        {
            var order = await _context.order.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            _context.order.Remove(order);
            await _context.SaveChangesAsync();

            return order;
        }

        private bool OrderExists(int id)
        {
            return _context.order.Any(e => e.orderId == id);
        }


        public async Task<ActionResult<Dictionary<String, int>>> GetGeneralStatistics() {

            //Time Frame
            DateTime pastWeek = DateTime.Now.AddDays(-7);

            var orders = await _context.order.ToListAsync();

            Dictionary<String, int> statistics = new Dictionary<string, int>();
            statistics.Add("All", orders.Count);

            int openCount = 0 , inProgressCount = 0, completedCount = 0;

            foreach (var order in orders)
            {
                if (order.status == OrderStatus.Opened)
                {
                    openCount++;
                }
                else if (order.status == OrderStatus.InProgress)
                {
                    inProgressCount++;
                }
                else if (order.status == OrderStatus.Completed)
                {
                    completedCount++;
                }
            }

            statistics.Add("open", openCount);
            statistics.Add("inprogress", inProgressCount);
            statistics.Add("complete", completedCount);

            return statistics;            
        }
    }
}

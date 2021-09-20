using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace MMSite6.Models
{
	public class Item
	{
		[DisplayName("Item ID")]
		public int itemId { get; set; }
		[DisplayName("Address")]
		public string address { get; set; }
		[DisplayName("SqFt")]
		public int sqft { get; set; }
		[DisplayName("Order ID")]
		public int orderId { get; set; }
		[DisplayName("Order")]
		public Order order { get; set; }
		public ICollection<Document> document { get; set; }
		[DisplayName("Estimated Cost")]
		public double estimateCost { get; set; }
		public int position { get; set; }

		public void CalculateCost(int extra)
		{
			this.position = extra;
			double sqftrate = 0;
			switch (extra)
			{
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

			double baseCost = 200;
			double multiplier = sqft - 1000;
			double extraCost = 0;
			if (multiplier > 0)
			{
				multiplier = multiplier * sqftrate;
				extraCost = multiplier;
			}

			estimateCost = baseCost + extraCost;
		}
	}
}

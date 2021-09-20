using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using MMSite6.Areas.Identity.Data;

namespace MMSite6.Models
{
    public class Order
    {
        [DisplayName("Order ID")]
        [Required]
        public int orderId { get; set; }
        [DisplayName("Status")]
        [Required]
        public OrderStatus status { get; set; }
        [DisplayName("Order Date")]
        [Required]
        public DateTime orderDate { get; set; }
        [StringLength(450)]
        [DisplayName("Summary")]
        [Required]
        public string summary { get; set; }
        [StringLength(450)]
        [DisplayName("Description")]
        [Required]
        public string desc { get; set; }
        [StringLength(450)]
        [DisplayName("User")]
        public MMSite6User user { get; set; }
        public ICollection <Item> item { get; set; }
		public double totalEstimateCost { get; set; }
		public double deposit { get; set; }
	}   
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MMSite6.Models
{
    public class OrderDetailsViewModel
    {
        public Order order { get; set; }
        public List<Item> item { get; set; }
    }
}

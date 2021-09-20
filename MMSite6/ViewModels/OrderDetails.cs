using MMSite6.Areas.Identity.Data;
using MMSite6.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MMSite6.ViewModels
{
    public class OrderDetails
    {
        public Order Order { get; set; }
        public ICollection<Item> Item { get; set; }

    }
}

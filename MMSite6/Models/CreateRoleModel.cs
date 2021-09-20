using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace MMSite6.Models
{
    public class CreateRoleModel
    {
        [Display(Name = "Role")]
        public string RoleName { get; set; }
    }
}

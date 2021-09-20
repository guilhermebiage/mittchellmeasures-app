using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MMSite6.Models
{
    public class EditRoleModel
    {
        public EditRoleModel()
        {
            Users = new List<string>();
        }

        public string Id { get; set; }

        public string RoleName { get; set; }

        public List<string> Users { get; set; }
    }
}


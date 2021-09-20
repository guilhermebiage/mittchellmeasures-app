using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MMSite6.Models
{
    public class EditUserModel
    {
        public EditUserModel()
        {
            Claims = new List<string>();
            Roles = new List<string>();
        }

        public string Id { get; set; }

        [DisplayName("Username")]
        public string UserName { get; set; }

        [DisplayName("Email")]
        [EmailAddress]
        public string Email { get; set; }

        [DisplayName("First Name")]
        [PersonalData]
        public string firstName { get; set; }

        [DisplayName("Last Name")]
        [PersonalData]
        public string lastName { get; set; }

        [DisplayName("Business Address")]
        [PersonalData]
        public string businessAddress { get; set; }

        [DisplayName("Business City")]
        [PersonalData]
        public string businessCity { get; set; }

        [DisplayName("Business Postal Code")]
        [PersonalData]
        public string businessPostal { get; set; }

        [DisplayName("Company Name")]
        [PersonalData]
        public string companyName { get; set; }

        public List<string> Claims { get; set; }

        public IList<string> Roles { get; set; }
    }
}

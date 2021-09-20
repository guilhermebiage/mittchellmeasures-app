using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace MMSite6.Areas.Identity.Data
{
    // Add profile data for application users by adding properties to the MMSite6User class
    public class MMSite6User : IdentityUser
    {
        [DisplayName("First Name")]
        [StringLength(30)]
        [PersonalData]
        public string firstName { get; set; }
        [DisplayName("Last Name")]
        [StringLength(30)]
        [PersonalData]
        public string lastName { get; set; }
        [DisplayName("Date Created")]
        [PersonalData]
        public DateTime dateCreated { get; set; }
        [DisplayName("Business Address")]
        [PersonalData]
        [StringLength(450)]
        public string businessAddress { get; set; }
        [DisplayName("Business City")]
        [StringLength(30)]
        [PersonalData]
        public string businessCity { get; set; }
        [DisplayName("Business Postal Code")]
        [PersonalData]
        [RegularExpression(@"[ABCEGHJKLMNPRSTVXY][0-9][ABCEGHJKLMNPRSTVWXYZ] ?[0-9][ABCEGHJKLMNPRSTVWXYZ][0-9]$", ErrorMessage = "Not a accepted postal code format")]
        public string businessPostal { get; set; }
        [DisplayName("Company Name")]
        [PersonalData]
        public string companyName { get; set; }
    }
}

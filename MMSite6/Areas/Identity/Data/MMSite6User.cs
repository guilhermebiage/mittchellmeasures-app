using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace MMSite6.Areas.Identity.Data
{
    // Add profile data for application users by adding properties to the MMSite6User class
    public class MMSite6User : IdentityUser
    {
        [PersonalData]
        public string firstName { get; set; }
        [PersonalData]
        public string lastName { get; set; }
        [PersonalData]
        public string businessAddress { get; set; }
        [PersonalData]
        public string businessCity { get; set; }
        [PersonalData]
        public string businessPostal { get; set; }
        [PersonalData]
        public string companyName { get; set; }
    }
}

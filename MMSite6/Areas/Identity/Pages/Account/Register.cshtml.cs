using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using MMSite6.Areas.Identity.Data;

namespace MMSite6.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<MMSite6User> _signInManager;
        private readonly UserManager<MMSite6User> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;

        public RegisterModel(
            UserManager<MMSite6User> userManager,
            SignInManager<MMSite6User> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            [RegularExpression(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$", ErrorMessage = "Not a valid email")]
            public string Email { get; set; }

            [Required]
            [DataType(DataType.Text)]
            [StringLength(30)]
            [Display(Name = "First Name")]
            public string firstName { get; set; }

            [Required]
            [DataType(DataType.Text)]
            [StringLength(30)]
            [Display(Name = "Last Name")]
            public string lastName { get; set; }

            [Required]
            [DataType(DataType.Text)]
            [StringLength(50)]
            [RegularExpression(@"^[A-Za-z0-9]+(?:\s[A-Za-z0-9'_-]+)+$", ErrorMessage = "Not a valid address")]
            [Display(Name = "Business Address")]
            public string businessAddress { get; set; }

            [Required]
            [DataType(DataType.Text)]
            [StringLength(30)]
            [Display(Name = "Business City")]
            public string businessCity { get; set; }

            [Required]
            [DataType(DataType.Text)]
            [RegularExpression(@"[ABCEGHJKLMNPRSTVXY][0-9][ABCEGHJKLMNPRSTVWXYZ] ?[0-9][ABCEGHJKLMNPRSTVWXYZ][0-9]", ErrorMessage = "Not a valid postal code")]
            [Display(Name = "Business Postal Code")]
            public string businessPostal { get; set; }

            [Required]
            [DataType(DataType.Text)]
            [StringLength(50)]
            [Display(Name = "Company Name")]
            public string companyName { get; set; }

            [Phone]
            [Display(Name = "Phone Number")]
            [DataType(DataType.Text)]
            public string PhoneNumber { get; set; }

            [Display(Name = "Date Created")]
            public DateTime dateCreated { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }
        }

        public void OnGet(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            if (ModelState.IsValid)
            {
                var user = new MMSite6User {
                    firstName = Input.firstName,
                    lastName = Input.lastName,
                    businessAddress = Input.businessAddress,
                    businessCity = Input.businessCity,
                    businessPostal = Input.businessPostal,
                    companyName = Input.companyName,
                    UserName = Input.Email,
                    Email = Input.Email,
                    PhoneNumber = Input.PhoneNumber,
                    dateCreated = System.DateTime.Now



                };
                var result = await _userManager.CreateAsync(user, Input.Password);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");
                    var currentUser = _userManager.FindByNameAsync(user.UserName).Result;
                    await _userManager.AddToRoleAsync(currentUser, "User");

                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { userId = user.Id, code = code },
                        protocol: Request.Scheme);

					await _emailSender.SendEmailAsync(Input.Email, "Account Verification",
	$"Thank you for signing up to the Mitchell Measures website!</a>.<br>" +
	$"Please verify your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>." +
	$"<img src='https://mitchellmeasuresstorage.blob.core.windows.net/uploadblob92b2ac74-4577-4360-8891-4d4da8aaa50f/logo.png' style='width: 186px; height: 95px;' cursor: 'pointer';></img>");

					await _signInManager.SignInAsync(user, isPersistent: false);
                    return LocalRedirect(returnUrl);
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MMSite6.Areas.Identity.Data;
using MMSite6.Models;

namespace MMSite6.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly UserManager<MMSite6User> userManager;
        private OrdersAPIController _ordersAPIController;
        private readonly MMSite6Context _context;

        public AdminController(MMSite6Context context, RoleManager<IdentityRole> roleManager, UserManager<MMSite6User> userManager)
        {
            this.roleManager = roleManager;
            this.userManager = userManager;
            _context = context;
            _ordersAPIController = new OrdersAPIController(_context);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult CreateRole()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateRole(CreateRoleModel model)
        {
            if (ModelState.IsValid)
            {
                IdentityRole identityRole = new IdentityRole
                {
                    Name = model.RoleName
                };

                IdentityResult result = await roleManager.CreateAsync(identityRole);

                if (result.Succeeded)
                {
                    return RedirectToAction("index", "Admin");
                }

                foreach (IdentityError error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

            }
            return View(model);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Index()
        {
            var stats = _ordersAPIController.GetGeneralStatistics().Result.Value;
            return View(stats);
        }
		[Authorize(Roles = "Admin")]
		[HttpGet]
        public IActionResult ListRoles()
        {
            var roles = roleManager.Roles;
            return View(roles);
        }
		[Authorize(Roles = "Admin")]
		[HttpGet]
        public async Task<IActionResult> EditRole(string id)
        {
            // Find the role by Role ID
            var role = await roleManager.FindByIdAsync(id);

            if (role == null)
            {
                return View("Index");
            }

            var model = new EditRoleModel
            {
                Id = role.Id,
                RoleName = role.Name
            };

            // Retrieve all the Users
            foreach (var user in userManager.Users)
            {
                if (await userManager.IsInRoleAsync(user, role.Name))
                {
                    model.Users.Add(user.UserName);
                }
            }

            return View(model);
        }
		[Authorize(Roles = "Admin")]
		// This action responds to HttpPost and receives EditRoleViewModel
		[HttpPost]
        public async Task<IActionResult> EditRole(EditRoleModel model)
        {
            var role = await roleManager.FindByIdAsync(model.Id);

            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id = {model.Id} cannot be found";
                return View("NotFound");
            }
            else
            {
                role.Name = model.RoleName;

                var result = await roleManager.UpdateAsync(role);

                if (result.Succeeded)
                {
                    return RedirectToAction("ListRoles");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

                return View(model);
            }
        }
		[Authorize(Roles = "Admin")]
		[HttpGet]
        public IActionResult ListUsers()
        {
            var users = userManager.Users;
            return View(users);
        }
		[Authorize(Roles = "Admin")]
		[HttpGet]
        public async Task<IActionResult> EditUser(string id)
        {
            var user = await userManager.FindByIdAsync(id);

            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {id} cannot be found";
                return View("NotFound");
            }

            var userClaims = await userManager.GetClaimsAsync(user);
            var userRoles = await userManager.GetRolesAsync(user);

            var model = new EditUserModel
            {
                Id = user.Id,
                Email = user.Email,
                UserName = user.UserName,
                firstName = user.firstName,
                lastName = user.lastName,
                businessAddress = user.businessAddress,
                businessCity = user.businessCity,
                businessPostal = user.businessPostal,
                companyName = user.companyName,
                Claims = userClaims.Select(c => c.Value).ToList(),
                Roles = userRoles
            };

            return View(model);
        }
		[Authorize(Roles = "Admin")]
		[HttpPost]
        public async Task<IActionResult> EditUser(EditUserModel model)
        {
            var user = await userManager.FindByIdAsync(model.Id);

            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {model.Id} cannot be found";
                return View("NotFound");
            }
            else
            {
                user.Email = model.Email;
                user.UserName = model.UserName;
                user.firstName = model.firstName;
                user.lastName = model.lastName;
                user.businessAddress = model.businessAddress;
                user.businessCity = model.businessCity;
                user.businessPostal = model.businessPostal;
                user.companyName = model.companyName;

                var result = await userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    return RedirectToAction("ListUsers");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

                return View(model);
            }
        }
		[Authorize(Roles = "Admin")]
		[HttpPost]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await userManager.FindByIdAsync(id);

            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {id} cannot be found";
                return View("NotFound");
            }
            else
            {
                var result = await userManager.DeleteAsync(user);

                if (result.Succeeded)
                {
                    return RedirectToAction("ListUsers");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

                return View("ListUsers");
            }
        }
		[Authorize(Roles = "Admin")]
		[HttpPost]
		public async Task<IActionResult> UnlockAccount(string id)
		{
			var user = await userManager.FindByIdAsync(id);

			var result = await userManager.SetLockoutEndDateAsync(user, null);

			if (result.Succeeded)
			{
				return RedirectToAction("ListUsers");
			}

			foreach (var error in result.Errors)
			{
				ModelState.AddModelError("", error.Description);
			}

			return View("ListUsers");
		}
		[Authorize(Roles = "Admin")]
		[HttpPost]
		public async Task<IActionResult> LockAccount(string id)
		{
			var user = await userManager.FindByIdAsync(id);

			var result = await userManager.SetLockoutEndDateAsync(user, new DateTimeOffset(new DateTime(2099, 1, 1)));

			if (result.Succeeded)
			{
				return RedirectToAction("ListUsers");
			}

			foreach (var error in result.Errors)
			{
				ModelState.AddModelError("", error.Description);
			}

			return View("ListUsers");

		}

		[Authorize(Roles = "Admin")]
		[HttpGet]
        public async Task<IActionResult> EditUsersInRole(string roleId)
        {
            ViewBag.roleId = roleId;

            var role = await roleManager.FindByIdAsync(roleId);

            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id = {roleId} cannot be found";
                return View("NotFound");
            }

            var model = new List<UserRoleViewModel>();

            foreach (var user in userManager.Users)
            {
                var userRoleViewModel = new UserRoleViewModel
                {
                    UserId = user.Id,
                    UserName = user.UserName
                };

                if (await userManager.IsInRoleAsync(user, role.Name))
                {
                    userRoleViewModel.IsSelected = true;
                }
                else
                {
                    userRoleViewModel.IsSelected = false;
                }

                model.Add(userRoleViewModel);
            }

            return View(model);
        }
		[Authorize(Roles = "Admin")]
		[HttpPost]
        public async Task<IActionResult> EditUsersInRole(List<UserRoleViewModel> model, string roleId)
        {
            var role = await roleManager.FindByIdAsync(roleId);

            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id = {roleId} cannot be found";
                return View("NotFound");
            }

            for (int i = 0; i < model.Count; i++)
            {
                var user = await userManager.FindByIdAsync(model[i].UserId);

                IdentityResult result = null;

                if (model[i].IsSelected && !(await userManager.IsInRoleAsync(user, role.Name)))
                {
                    result = await userManager.AddToRoleAsync(user, role.Name);
                }
                else if (!model[i].IsSelected && await userManager.IsInRoleAsync(user, role.Name))
                {
                    result = await userManager.RemoveFromRoleAsync(user, role.Name);
                }
                else
                {
                    continue;
                }

                if (result.Succeeded)
                {
                    if (i < (model.Count - 1))
                        continue;
                    else
                        return RedirectToAction("EditRole", new { Id = roleId });
                }
            }

            return RedirectToAction("EditRole", new { Id = roleId });
        }



    }
}
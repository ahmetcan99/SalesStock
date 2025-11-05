using System.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SalesStock.Infrastructure.Identity;
using SalesStock.Web.ViewModels.UserManagement;



namespace SalesStock.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UserManagementController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public UserManagementController(UserManager<ApplicationUser> userManager, 
            RoleManager<IdentityRole> roleManager, 
            SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
        }

        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users.ToListAsync();
            var userViewModels = new List<UserViewModel>();
            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userViewModels.Add(new UserViewModel
                {
                    Id = user.Id,
                    UserName = user.UserName ?? string.Empty,
                    Email = user.Email ?? string.Empty,
                    IsActive = user.IsActive,
                    Roles = roles
                });
            }
            return View(userViewModels.OrderBy(u => u.UserName).ToList());
        }
        [HttpGet]
        public async Task<IActionResult> CreateUser()
        {
            var roles = await _roleManager.Roles.OrderBy(r => r.Name).Select(r => new SelectListItem { Text = r.Name, Value = r.Name }).ToListAsync();
            var model = new CreateUserViewModel
            {
                RoleList = roles
            };
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateUser(CreateUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.UserName,
                    Email = model.Email,
                    EmailConfirmed = true,
                    IsActive = true
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, model.SelectedRole);
                    TempData["SuccessMessage"] = $"'{user.UserName} is created.";
                    return RedirectToAction(nameof(Index));
                }
                TempData["ErrorMessage"] = "There was an error while creating the user.";
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            model.RoleList = await _roleManager.Roles.OrderBy(r => r.Name).Select(r => new SelectListItem { Text = r.Name, Value = r.Name }).ToListAsync();

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var loggedInUserId = _userManager.GetUserId(User);

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                TempData["ErrorMessage"] = "User is not found.";
                return NotFound();
            }
            var userCurrentRole = (await _userManager.GetRolesAsync(user)).FirstOrDefault();
            var allRoles = await _roleManager.Roles.OrderBy(r => r.Name)
                                 .Select(r => new SelectListItem { Text = r.Name, Value = r.Name })
                                 .ToListAsync();
            
            var model = new EditUserViewModel
            {
                Id = user.Id,
                UserName = user.UserName ?? string.Empty,
                Email = user.Email ?? string.Empty,
                Status = user.IsActive ? "Active" : "Passive",
                RoleList = allRoles,
                SelectedRole = userCurrentRole!,
                IsEditingSelf = (id == loggedInUserId),
            };
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditUserViewModel model)
        {
            var user = await _userManager.FindByIdAsync(model.Id);
            if (user == null)
            {
                TempData["ErrorMessage"] = "User is not found.";
                return NotFound();
            }
            var loggedInUserId = _userManager.GetUserId(User);
            bool isEditingSelf = (model.Id == loggedInUserId);
            model.IsEditingSelf = isEditingSelf;

            if (isEditingSelf)
            {
                ModelState.Remove(nameof(model.Status));
                ModelState.Remove(nameof(model.SelectedRole));
            }

            async Task PopulateRoleListAndReturnView()
            {
                model.RoleList = await _roleManager.Roles.OrderBy(r => r.Name)
                                               .Select(r => new SelectListItem { Text = r.Name, Value = r.Name })
                                               .ToListAsync();
            }

            if (!ModelState.IsValid)
            {
                await PopulateRoleListAndReturnView();
                return View(model);
            }

            user.UserName = model.UserName;
            user.Email = model.Email;

            if (!isEditingSelf)
            {
                user.IsActive = (model.Status == "Active");

                var currentRoles = await _userManager.GetRolesAsync(user);
                await _userManager.RemoveFromRolesAsync(user, currentRoles);
                if (!string.IsNullOrEmpty(model.SelectedRole))
                {
                    await _userManager.AddToRoleAsync(user, model.SelectedRole);
                }
            }

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                if (isEditingSelf)
                {
                    await _signInManager.RefreshSignInAsync(user);
                }

                TempData["SuccessMessage"] = $"'{user.UserName} has been updated successfully.";
                return RedirectToAction(nameof(Index));
            }

            TempData["ErrorMessage"] = "There was an error while updating the user.";
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            await PopulateRoleListAndReturnView();
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> ResetPassword(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                TempData["ErrorMessage"] = "User not found.";
                return NotFound();
            }
            var model = new ResetPasswordViewModel
            {
                UserId = user.Id,
                UserName = user.UserName ?? string.Empty
            };
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var userForName = await _userManager.FindByIdAsync(model.UserId);
                if (userForName != null) model.UserName = userForName.UserName ?? string.Empty;
                return View(model);
            }
            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null)
            {
                TempData["ErrorMessage"] = "User not found.";
                return RedirectToAction(nameof(Index));
            }
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, model.NewPassword);

            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = $"'{user.UserName}'s password was changed succesfully.";
                return RedirectToAction(nameof(Index));
            }
            TempData["ErrorMessage"] = "There was an error while changing the password.";
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            model.UserName = user.UserName?? string.Empty;
            return View(model);
        }

    }
}

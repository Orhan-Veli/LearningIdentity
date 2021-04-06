using LearningIdentity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LearningIdentity.Controllers
{
    public class AdministrationController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdministrationController(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public IActionResult CreateRole()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateRole(CreateRoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                var identityRole = new IdentityRole
                {
                    Name = model.RoleName
                };
                var result = await _roleManager.CreateAsync(identityRole);
                if (result.Succeeded)
                {
                    return RedirectToAction("ListRoles", "home");
                }
                foreach (var item in result.Errors)
                {
                    ModelState.AddModelError("", item.Description);
                }
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult ListRoles()
        {
            var roles = _roleManager.Roles;
            return View(roles);
        }

        [HttpGet]
        public async Task<IActionResult> EditRole(string id)
        {
            var result = await _roleManager.FindByIdAsync(id);
            if (result == null)
            {
                ViewBag.ErrorMessage = $"Role with Id = {id} cannot be found";
                return View("Not Found");
            }
            var model = new EditViewModel
            {
                Id = result.Id,
                RoleName = result.Name

            };
            foreach (var item in _userManager.Users)
            {
                if (await _userManager.IsInRoleAsync(item, result.Name))
                {
                    model.Users.Add(item.UserName);
                }
            }
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> EditRole(EditViewModel editViewModel)
        {
            var result = await _roleManager.FindByIdAsync(editViewModel.Id);
            if (result == null)
            {
                ViewBag.ErrorMessage = $"Role with Id = {editViewModel.Id} cannot be found";
                return View("Error");
            }
            else
            {
                result.Name = editViewModel.RoleName;
                var updateResult = await _roleManager.UpdateAsync(result);
                if (updateResult.Succeeded)
                {
                    return RedirectToAction("ListRoles");

                }
                foreach (var item in updateResult.Errors)
                {
                    ModelState.AddModelError("", item.Description);
                }
                return View(updateResult);
            }

        }
    }
}

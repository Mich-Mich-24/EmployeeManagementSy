using EmployeeManagementSy.Data;
using EmployeeManagementSy.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace EmployeeManagementSy.Controllers
{
    public class RolesController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ApplicationDbContext _context;

        public RolesController(ApplicationDbContext context, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, SignInManager<IdentityUser> signInManager)
        {
            _roleManager = roleManager;
            _signInManager = signInManager;
            _userManager = userManager;
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var roles = await _context.Roles.ToListAsync();
            return View(roles);
        }
        [HttpGet]
        public async Task<IActionResult> Create()
        {

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(RolesViewModel model)
        {
            IdentityRole role = new IdentityRole();
            role.Name = model.RoleName;
            
            var result = await _roleManager.CreateAsync(role);
            if (result.Succeeded)
            {
                return RedirectToAction("Index");
            }
            else
            {
                return View(model);
            }

        }

        [HttpGet]
        public async Task<IActionResult> Edit( string id)
        {
            var role = new RolesViewModel();
            var result = await _roleManager.FindByIdAsync(id);
            role.RoleName = result.Name;
            role.Id = result.Id;
            return View(role);
        }

        [HttpPost]
        public async Task<IActionResult> Edit( string id, RolesViewModel model)
        {
            var result = await _roleManager.FindByIdAsync(id);
            result.Name = model.RoleName;

            var finalresult = await _roleManager.UpdateAsync(result);
            if (finalresult.Succeeded)
            {
                return RedirectToAction("Index");
            }
            else
            {
                return View(model);
            }

        }
    }
}

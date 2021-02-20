using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using University.MVC.ViewModels;

namespace University.MVC.Controllers
{
    [Authorize]
    public class SecurityController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;

        private readonly SignInManager<IdentityUser> _signInManager;

        private readonly RoleManager<IdentityRole> _roleManager;

        public SecurityController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> Login(string returnUrl = null)
        {
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                await _signInManager.SignOutAsync();
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    return RedirectToAction("Courses", "Course");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return View(model);
                }
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Courses", "Course");
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateUser(CreateUserViewModel createUserViewModel)
        {
            if (ModelState.IsValid)
            {
                var result = await _userManager.CreateAsync(new IdentityUser(createUserViewModel.Email) { Email = createUserViewModel.Email }, createUserViewModel.Password);
                if (result.Succeeded)
                {
                    return RedirectToAction("Users");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid create User.");
                    return View(createUserViewModel);
                }
            }

            return View(createUserViewModel);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<ActionResult> CreateUser()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<ActionResult> CreateRole()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateRole(CreateRoleViewModel createRoleViewModel)
        {
            if (ModelState.IsValid)
            {
                var result = await _roleManager.CreateAsync(new IdentityRole(createRoleViewModel.Role));
                if (result.Succeeded)
                {
                    return RedirectToAction("Courses", "Course");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid create role.");
                    return View(createRoleViewModel);
                }
            }

            return View(createRoleViewModel);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<ActionResult> Users()
        {

            UsersViewModel model = new UsersViewModel() { UserNames = _userManager.Users.Select(p => p.Email).ToList() };
            return View(model);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<ActionResult> AssignUserRole(string userName)
        {
            AssignUserRoleViewModel model = new AssignUserRoleViewModel();
            var user = _userManager.Users.SingleOrDefault(p => p.Email == userName);
            if (user == null)
            {
                return NotFound();
            }

            foreach (var role in _roleManager.Roles)
            {
                model.UserRoles.Add(new UserRoleViewModel()
                {
                    RoleName = role.Name,
                    IsAssigned = await _userManager.IsInRoleAsync(user, role.Name)
                });
            }

            return View(model);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult> AssignUserRole(AssignUserRoleViewModel model)
        {
            var user = _userManager.Users.SingleOrDefault(p => p.Email == model.UserName);
            foreach (var userRoleViewModel in model.UserRoles)
            {
                if (await _userManager.IsInRoleAsync(user, userRoleViewModel.RoleName))
                {
                    await _userManager.RemoveFromRoleAsync(user, userRoleViewModel.RoleName);
                }

                if (userRoleViewModel.IsAssigned)
                {
                    var result = await _userManager.AddToRoleAsync(user, userRoleViewModel.RoleName);
                }
            }

            return RedirectToAction("Users");
        }

        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using QuestRoom.Areas.Identity.Data;
using QuestRoom.ViewModel;
using System.Security.Claims;
using System.Threading.Tasks;

namespace QuestRoom.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<QuestRoomUser> _userManager;
        private readonly SignInManager<QuestRoomUser> _signInManager;
        private readonly ILogger _logger;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountController(
            UserManager<QuestRoomUser> userManager,
            SignInManager<QuestRoomUser> signInManager,
            ILoggerFactory loggerFactory, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = loggerFactory.CreateLogger<AccountController>();
            _roleManager = roleManager;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel login)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(login.Email, login.Password, login.RememberMe, false);
                if (result.Succeeded)
                {
                    _logger.LogInformation(1, "User logged in.");
                    return Redirect("/");
                }

                if (result.IsLockedOut)
                {
                    _logger.LogInformation(1, "User logged in.");
                    return View("Lockout");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return View(login);
                }
            }
            return View(login);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult RegisterAdmin()
        {
            return View();
        }


        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterAdmin(RegisterViewModel register)
        {
            if (ModelState.IsValid)
            {
                var userExists = await _userManager.FindByNameAsync(register.Email);
                if (userExists != null)
                    ModelState.AddModelError("Error", "Such user already exists");

                var user = new QuestRoomUser()
                {
                    UserName = register.Email,
                    Email = register.Email
                };
                var result = await _userManager.CreateAsync(user, register.Password);

                if (result.Succeeded)
                {
                    if (!await _roleManager.RoleExistsAsync("Admin"))
                        await _roleManager.CreateAsync(new IdentityRole("Admin"));
                    
                    if(await _roleManager.RoleExistsAsync("Admin"))
                    {
                        await _userManager.AddToRoleAsync(user, "Admin");
                    }
                    await _signInManager.SignInAsync(user, false);
                    _logger.LogInformation(3, "Admin created a new account with password.");
                    return RedirectToAction(nameof(HomeController.Index), "Home");
                }
            }
            return View(register);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel register)
        {
            if (ModelState.IsValid)
            {
                var userExists = await _userManager.FindByNameAsync(register.Email);
                if (userExists != null)
                    ModelState.AddModelError("Error", "Such user already exists");

                var user = new QuestRoomUser()
                {
                    UserName = register.Email,
                    Email = register.Email
                };
                var res = await _userManager.CreateAsync(user, register.Password);

                if (res.Succeeded)
                {
                    await _signInManager.SignInAsync(user, false);
                    _logger.LogInformation(3, "User created a new account with password.");
                    return RedirectToAction(nameof(HomeController.Index), "Home");
                }
                foreach (var error in res.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View(register);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation(4, "User logged out.");
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }
    }
}
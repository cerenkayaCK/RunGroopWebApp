using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RunGroopWebApp.Data;
using RunGroopWebApp.Models;
using RunGroopWebApp.ViewModels;

namespace RunGroopWebApp.Controllers
{
    public class AcountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ApplicationDbContext _context;

        public AcountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, ApplicationDbContext context)
        {
            _context = context;
            _signInManager = signInManager;
            _userManager = userManager;

        }
        public IActionResult Login()
        {

            var response = new LoginViewModel();
            return View(response);
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel loginviewModel)
        {
            if(!ModelState.IsValid)
            {
                return View(loginviewModel);

            }

            var user = await _userManager.FindByEmailAsync(loginviewModel.Email);

            if(user != null)
            {
                //user is found and checked password
                var passwordCheck = await _userManager.CheckPasswordAsync(user, loginviewModel.Password);
                if (passwordCheck)
                {
                    //password correct, sign in
                    var result = await _signInManager.PasswordSignInAsync(user, loginviewModel.Password, false, false);
                    if(result.Succeeded)
                    {
                        return RedirectToAction("Index", "Race");
                    }
                    //password is incorrect
                    TempData["Error"] = "Wrong credentials. Please , try again.";
                    return View(loginviewModel);
                }
            }
            TempData["Error"] = "Wrong credentials. Please try again";
            return View(loginviewModel);
        }
    }
}

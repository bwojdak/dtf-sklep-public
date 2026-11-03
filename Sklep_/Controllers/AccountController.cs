using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Sklep_.Data;
using Sklep_.ViewModel;

namespace Sklep_.Controllers;
[AllowAnonymous]
public class AccountController : Controller
{
    private readonly AppDbContext _context;
    private readonly SignInManager<IdentityUser> _signInManager;
    public AccountController(AppDbContext context, SignInManager<IdentityUser> signInManager)
    {
        _signInManager = signInManager;
        _context = context;
    }
    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if(!ModelState.IsValid)return View(model);
        var result = await _signInManager
            .PasswordSignInAsync(model.UserName, model.Password, false, false);
        if (result.Succeeded) return RedirectToAction("Index", "Home");
        ModelState.AddModelError("","Nieprawidłowy login lub hasło");
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Login");
    }
}
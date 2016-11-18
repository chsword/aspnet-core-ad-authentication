using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Demo.Controllers
{
    public class AccountController : Controller
    {
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string u, string p)
        {
            if (LDAPUtil.Validate(u, p))
            {
                var identity = new ClaimsIdentity(new MyIdentity(u));
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.Authentication.SignInAsync(LDAPUtil.CookieName, principal);
                return RedirectToAction("Index", "Home");
            }
            return View();
        }
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.Authentication.SignOutAsync(LDAPUtil.CookieName);
            return RedirectToAction("Index", "Home");
        }
    }
}

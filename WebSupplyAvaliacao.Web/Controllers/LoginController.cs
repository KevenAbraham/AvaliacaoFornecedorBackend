using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace WebSupplyAvaliacao.Web.Controllers
{
    public class LoginController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Sair()
        {
            return Redirect("https://login.microsoftonline.com/f42acaec-b2b8-4cf1-aa36-d6ffdd442618/oauth2/logout?post_logout_redirect_uri=https://localhost:7298");
        }

        //public async Task<IActionResult> Logout()
        //{
        //    await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        //    return RedirectToAction("Index", "Login");
        //}
    }
}

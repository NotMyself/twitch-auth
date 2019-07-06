using System.Threading.Tasks;
using System.Linq;
using System.Security.Claims;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;

using App.Models;

namespace App.Controllers
{
  public class AccountController : Controller
  {
    public async Task Login(string returnUrl = "/")
    {
      await HttpContext.ChallengeAsync("Auth0", new AuthenticationProperties() { RedirectUri = returnUrl });
    }

    [Authorize]
    public async Task Logout()
    {
      await HttpContext.SignOutAsync("Auth0", new AuthenticationProperties
      {
        // Indicate here where Auth0 should redirect the user after a logout.
        // Note that the resulting absolute Uri must be whitelisted in the
        // **Allowed Logout URLs** settings for the app.
        RedirectUri = Url.Action("Index", "Home")
      });
      await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    }

    [Authorize]
    public async Task<IActionResult> Profile()
    {
      var email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
      var connections = User.Claims.FirstOrDefault(c => c.Type == "https://iamnotmyself.com/connections").Value;
      var idToken = await HttpContext.GetTokenAsync("id_token");
      var accessToken = await HttpContext.GetTokenAsync("access_token");

      return View(new UserProfileViewModel()
      {
        Name = User.Identity.Name,
        EmailAddress = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value,
        ProfileImage = User.Claims.FirstOrDefault(c => c.Type == "picture")?.Value
      });
    }

    [Authorize("Admin")]
    public async Task<IActionResult> Profile2()
    {
      var email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
      var connections = User.Claims.FirstOrDefault(c => c.Type == "https://iamnotmyself.com/connections").Value;
      var idToken = await HttpContext.GetTokenAsync("id_token");
      var accessToken = await HttpContext.GetTokenAsync("access_token");

      return View("Profile", new UserProfileViewModel()
      {
        Name = User.Identity.Name,
        EmailAddress = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value,
        ProfileImage = User.Claims.FirstOrDefault(c => c.Type == "picture")?.Value
      });
    }
  }
}

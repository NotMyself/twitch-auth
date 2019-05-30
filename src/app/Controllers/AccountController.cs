using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.Cookies;
using App.Models;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Auth0.ManagementApi;

namespace App.Controllers
{
  public class AccountController : Controller
  {
    private readonly ManagementApiClient api;

    public AccountController(ManagementApiClient api)
    {
      this.api = api ?? throw new System.ArgumentNullException(nameof(api));
    }
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
      var profile = await api.Users.GetUsersByEmailAsync(email);
      return View(new UserProfileViewModel()
      {
        Name = User.Identity.Name,
        EmailAddress = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value,
        ProfileImage = User.Claims.FirstOrDefault(c => c.Type == "picture")?.Value
      });
    }
  }
}

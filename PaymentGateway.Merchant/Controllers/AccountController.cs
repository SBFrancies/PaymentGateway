using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PaymentGateway.Merchant.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class AccountController : ControllerBase
    {
        [HttpGet("SignIn")]
        public IActionResult SignIn()
        {
            return Challenge(
                new AuthenticationProperties { RedirectUri = Url.Content("~/" )},
                OpenIdConnectDefaults.AuthenticationScheme);
        }

        [HttpGet("SignOut")]
        public IActionResult SignOut()
        {
            return SignOut(
                 new AuthenticationProperties
                 {
                     RedirectUri = Url.Page("/Index", null, null, Request.Scheme),
                 },
                 CookieAuthenticationDefaults.AuthenticationScheme,
                 OpenIdConnectDefaults.AuthenticationScheme);
        }
    }
}

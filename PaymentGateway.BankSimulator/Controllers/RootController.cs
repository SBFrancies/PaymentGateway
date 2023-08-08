using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PaymentGateway.BankSimulator.Controllers
{
    [Route("/")]
    [AllowAnonymous]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class RootController : ControllerBase
    {        
        [HttpGet]
        public IActionResult Index()
        {
            return Ok("PaymentGateway Bank Simulator");
        }
    }
}

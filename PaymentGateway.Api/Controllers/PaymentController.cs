using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PaymentGateway.Api.Models.Request;
using System;
using System.Threading.Tasks;

namespace PaymentGateway.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class PaymentController : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> PostAsync([FromBody]CreatePaymentRequest request)
        {
            await Task.Delay(1000);

            return Ok();
        }

        [HttpGet]
        [Route("{id:guid}")]
        [Authorize]
        public async Task<IActionResult> GetAsync(Guid id)
        {
            await Task.Delay(1000);

            return Ok(id);
        }
    }
}

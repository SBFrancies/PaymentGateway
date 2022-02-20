using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PaymentGateway.BankSimulator.Interface;
using PaymentGateway.BankSimulator.Models.Request;
using System;
using System.Net;
using System.Threading.Tasks;

namespace PaymentGateway.BankSimulator.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize(Policy = "ApiKey")]
    public class CardPaymentController : ControllerBase
    {
        private IPaymentProcessor PaymentProcessor { get; }

        public CardPaymentController(IPaymentProcessor paymentProcessor)
        {
            PaymentProcessor = paymentProcessor ?? throw new ArgumentNullException(nameof(paymentProcessor));
        }

        [HttpPost]
        public async Task<IActionResult> PostAsync([FromBody]CardPaymentRequest request)
        {
            try
            {
                var result = await PaymentProcessor.ProcessPaymentAsync(request);

                return StatusCode((int)result.code, result.response);
            }
            catch (Exception exception)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, exception.Message);
            }
        }
    }
}

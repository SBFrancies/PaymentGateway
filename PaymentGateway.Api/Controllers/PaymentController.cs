using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PaymentGateway.Api.Interface;
using PaymentGateway.Api.Models.Request;
using PaymentGateway.Api.Models.Response;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace PaymentGateway.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class PaymentController : ControllerBase
    {
        private IPaymentService PaymentService { get; }

        public PaymentController(IPaymentService paymentService)
        {
            PaymentService = paymentService ?? throw new ArgumentNullException(nameof(paymentService));
        }

        [HttpPost]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType(typeof(CreatePaymentResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(CreatePaymentResponse), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.UnprocessableEntity)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> PostAsync([FromBody]CreatePaymentRequest request)
        {
            try
            {
                var response = await PaymentService.CreatePaymentAsync(request);

                if(response.Success)
                {
                    return Ok(response);
                }

                else
                {
                    return BadRequest(response);
                }
            }

            catch (ValidationException validationException)
            {
                return StatusCode((int)HttpStatusCode.UnprocessableEntity, new ErrorResponse
                {
                    ErrorId = "Validation failed",
                    ErrorDescription = string.Join(", ", validationException.Errors),
                });
            }

            catch (Exception exception)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new ErrorResponse
                {
                    ErrorId = "Unhandled exception",
                    ErrorDescription = string.Join(", ", new [] {exception.Message, exception.InnerException?.Message}),
                });
            }
        }

        [HttpGet]
        [Route("{id:guid}")]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType(typeof(PaymentResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetAsync([FromRoute]Guid id)
        {
            try
            {
                var response = await PaymentService.FetchPaymentAsync(id);

                if (response == null)
                {
                    return NotFound(new ErrorResponse
                    {
                        ErrorId = "Payment not found",
                        ErrorDescription = $"Payment with ID {id} not found",
                    });
                }

                else
                {
                    return Ok(response);
                }
            }

            catch (Exception exception)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new ErrorResponse
                {
                    ErrorId = "Unhandled exception",
                    ErrorDescription = string.Join(", ", new[] { exception.Message, exception.InnerException?.Message }),
                });
            }
        }

        [HttpGet]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType(typeof(FetchPaymentsResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetAsync([FromQuery]FetchPaymentsRequest request)
        {
            try
            {
                var response = await PaymentService.FetchPaymentsAsync(request);

                if (response == null || !response.Payments.Any())
                {
                    return NotFound(new ErrorResponse
                    {
                        ErrorId = "Payments not found",
                        ErrorDescription = "No payments matching the given criteria were found",
                    });
                }

                else
                {
                    return Ok(response);
                }
            }

            catch (Exception exception)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new ErrorResponse
                {
                    ErrorId = "Unhandled exception",
                    ErrorDescription = string.Join(", ", new[] { exception.Message, exception.InnerException?.Message }),
                });
            }
        }
    }
}

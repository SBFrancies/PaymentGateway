using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Identity.Web;
using PaymentGateway.Merchant.Interface;
using PaymentGateway.Merchant.Models.ApiModels;
using System;
using System.Threading.Tasks;

namespace PaymentGateway.Merchant.Pages
{
    [AuthorizeForScopes(ScopeKeySection = "https://PaymentGatewayAD.onmicrosoft.com/f5fd2651-c11f-490b-9e81-f3933aa7a0ac/Payments.ReadWrite")]
    public class PaymentModel : PageModel
    {
        private IGatewayApi GatewayApi {get;}

        [BindProperty]
        public PaymentResponse Payment { get; set; }

        public PaymentModel(IGatewayApi gatewayApi)
        {
            GatewayApi = gatewayApi ?? throw new ArgumentNullException(nameof(gatewayApi));
        }

        public async Task<IActionResult> OnGetAsync([FromRoute] Guid id)
        {
            Payment = await GatewayApi.GetAsync(id);

            if(Payment == null)
            {
                return RedirectToPage("ViewPayments");
            }

            return Page();
        }
    }
}

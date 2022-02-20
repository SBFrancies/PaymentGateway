using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PaymentGateway.Merchant.Interface;
using PaymentGateway.Merchant.Models.ApiModels;
using System;
using System.Threading.Tasks;

namespace PaymentGateway.Merchant.Pages
{
    public class PaymentModel : PageModel
    {
        private IGatewayApi GatewayApi {get;}

        [BindProperty]
        public Payment Payment { get; set; }

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

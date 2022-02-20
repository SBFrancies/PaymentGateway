using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PaymentGateway.Merchant.Interface;
using PaymentGateway.Merchant.Models.ApiModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PaymentGateway.Merchant.Pages
{
    public class ViewPaymentsModel : PageModel
    {
        private IGatewayApi GatewayApi { get; }

        [BindProperty]
        public IEnumerable<Payment> Payments { get; set; }

        public ViewPaymentsModel(IGatewayApi gatewayApi)
        {
            GatewayApi = gatewayApi ?? throw new ArgumentNullException(nameof(gatewayApi));
        }


        public async Task<IActionResult> OnGetAsync([FromQuery]string cardNumber, [FromQuery] string reference)
        {
            Payments = await GatewayApi.GetAsync(cardNumber, reference);

            return Page();
        }
    }
}

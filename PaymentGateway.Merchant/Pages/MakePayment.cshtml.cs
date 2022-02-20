using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PaymentGateway.Merchant.Interface;
using PaymentGateway.Merchant.Models.ApiModels;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace PaymentGateway.Merchant.Pages
{
    public class MakePaymentModel : PageModel
    {
        private IGatewayApi GatewayApi {get;}

        public MakePaymentModel(IGatewayApi gatewayApi)
        {
            GatewayApi = gatewayApi ?? throw new ArgumentNullException(nameof(gatewayApi));
        }


        [BindProperty]
        [Required]
        [Range(0.01, int.MaxValue)]
        public decimal Amount { get; set; }

        [BindProperty]
        [CreditCard]
        [Required]
        [MaxLength(40)]
        public string CardNumber { get; set; }
        
        [BindProperty]
        [Required]
        [MaxLength(100)]
        public string CardName { get; set; }
        
        [BindProperty]
        [Range(1, 12)]
        [Required]
        public int ExpiryMonth { get; set; }

        [BindProperty]
        [Required]
        [Range(2022, 2040)]
        public int ExpiryYear { get; set; } = DateTime.UtcNow.Year;
       
        [BindProperty]
        [MaxLength(4)]
        [RegularExpression("^[0-9]{3,4}$")]
        [Required]
        public string Cvv { get; set; }

        [BindProperty]
        [Required]
        [MaxLength(3)]
        [RegularExpression("^[a-zA-Z]{3}$")]
        public string CurrencyCode { get; set; }

        [BindProperty]
        [MaxLength(50)]
        [RegularExpression("^[a-zA-Z0-9]{0,50}$")]
        public string Reference { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }


            var createRequest = new CreatePayment
            {
                Amount = Amount,
                CardName = CardName,
                ExpiryMonth = ExpiryMonth,
                ExpiryYear = ExpiryYear,
                CardNumber = CardNumber,
                CurrencyCode = CurrencyCode,
                Reference = Reference,
                Cvv = Cvv,          
            };

            var result = await GatewayApi.PostAsync(createRequest);

            return RedirectToPage("Payment", new {id = result.Id});
        }
    }
}

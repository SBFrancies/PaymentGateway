using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Identity.Web;
using PaymentGateway.Merchant.Interface;
using PaymentGateway.Merchant.Models.ApiModels;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace PaymentGateway.Merchant.Pages
{
    [AuthorizeForScopes(ScopeKeySection = "https://PaymentGatewayAD.onmicrosoft.com/f5fd2651-c11f-490b-9e81-f3933aa7a0ac/Payments.ReadWrite")]
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
        [DataType(DataType.Currency)]
        public decimal Amount { get; set; }

        [BindProperty]
        [CreditCard]
        [Required]
        [MaxLength(40)]
        [DataType(DataType.CreditCard)]
        public string CardNumber { get; set; }
        
        [BindProperty]
        [Required]
        [MaxLength(100)]
        [RegularExpression("^[a-zA-Z ]{1,100}$")]
        public string CardName { get; set; }

        [BindProperty]
        [Range(1, 12)]
        [Required]
        public int ExpiryMonth { get; set; } = DateTime.UtcNow.Month;

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
        [RegularExpression("^[a-zA-Z0-9 ]{0,50}$")]
        public string Reference { get; set; }

        [BindProperty]        
        public string ErrorMessage { get; set; }

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

            try
            {
                ErrorMessage = null;

                var result = await GatewayApi.PostAsync(createRequest);

                return RedirectToPage("Payment", new { id = result.PaymentId });
            }

            catch(Exception exception) when (exception is not MicrosoftIdentityWebChallengeUserException)
            {
                ErrorMessage = exception.Message;
                return Page();
            }
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using PaymentGateway.Merchant.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PaymentGateway.Merchant.Pages
{
    public class PrivacyModel : PageModel
    {
        private readonly ILogger<PrivacyModel> _logger;
        private IGatewayApi Api { get; }

        public PrivacyModel(ILogger<PrivacyModel> logger, IGatewayApi api)
        {
            _logger = logger;
            Api = api;
        }

        public async Task OnGetAsync()
        {
            await Api.GetAsync(Guid.NewGuid());
        }
    }
}

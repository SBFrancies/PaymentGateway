using PaymentGateway.Api.Interface;
using System;

namespace PaymentGateway.Api.Services
{
    public class IdentifierGenerator : IIdentifierGenerator
    {
        public Guid GenerateIdentifier()
        {
            return Guid.NewGuid();  
        }
    }
}

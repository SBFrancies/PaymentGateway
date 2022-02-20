using System;

namespace PaymentGateway.Api.Interface
{
    public interface IIdentifierGenerator
    {
        public Guid GenerateIdentifier();
    }
}

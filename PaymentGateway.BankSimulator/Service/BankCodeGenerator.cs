using PaymentGateway.BankSimulator.Interface;
using System;

namespace PaymentGateway.BankSimulator.Service
{
    public class BankCodeGenerator : IBankCodeGenerator
    {
        public string GenerateBankCode()
        {
            return Guid.NewGuid().ToString();
        }
    }
}

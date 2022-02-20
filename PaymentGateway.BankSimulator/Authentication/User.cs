using System;
using System.Security.Principal;

namespace PaymentGateway.BankSimulator.Authentication
{
    public class User : IIdentity
    {
        public User(string authenticationType, bool isAuthenticated, string name)
        {
            AuthenticationType = authenticationType ?? throw new ArgumentNullException(nameof(authenticationType));
            IsAuthenticated = isAuthenticated;
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }

        public string AuthenticationType { get; }

        public bool IsAuthenticated { get; }

        public string Name { get; }
    }
}

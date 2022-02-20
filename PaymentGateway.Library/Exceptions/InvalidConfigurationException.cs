using System;

namespace PaymentGateway.Library.Exceptions
{
    public class InvalidConfigurationException : Exception
    {
        public InvalidConfigurationException(string fieldName, string message) : base(message)
        {
            FieldName = fieldName;
        }

        public string FieldName { get; }
    }
}

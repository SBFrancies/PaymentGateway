namespace PaymentGateway.Api.Enums
{
    public enum PaymentStatus
    {
        Uploaded,
        FailedValidation,
        Processing,
        FailedAtBank,
        Success,
    }
}

using PaymentGateway.Domain;

namespace PaymentGateway.Application.Utilities
{
    public static class PaymentStatusMapper
    {
        public static PaymentStatus MapFromString(string status)
        {
            if (string.IsNullOrWhiteSpace(status)) return PaymentStatus.Rejected;
            
            return status.ToLower() switch
            {
                "authorized" => PaymentStatus.Authorized,
                "declined" => PaymentStatus.Declined,
                "rejected" => PaymentStatus.Rejected,
                _ => PaymentStatus.Rejected 
            };
        }
    }
}
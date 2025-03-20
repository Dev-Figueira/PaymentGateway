using PaymentGateway.Application.Models.Requests;
using PaymentGateway.Application.Models.Responses;

namespace PaymentGateway.Application.Interfaces
{
    public interface IBankService
    {
        Task<BankResponse> ProcessPaymentAsync(BankRequest request, CancellationToken cancellationToken);
    }
}
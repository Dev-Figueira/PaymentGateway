using PaymentGateway.Application.Models.Responses;

namespace PaymentGateway.Application.Interfaces
{
    public interface IPaymentRepository
    {
        Task AddPaymentAsync(PostPaymentResponse paymentResponse, CancellationToken cancellationToken);
        public Task<PostPaymentResponse> GetPaymentAsync(Guid id, CancellationToken cancellationToken);
    }
}
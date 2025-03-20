
using System.Threading;

using PaymentGateway.Application.Interfaces;
using PaymentGateway.Application.Models.Responses;

namespace PaymentGateway.Infrastructure.Repositories;

public class PaymentRepository : IPaymentRepository
{
    public List<PostPaymentResponse> Payments = [];

    public Task AddPaymentAsync(PostPaymentResponse payment, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        Payments.Add(payment);
        return Task.CompletedTask;
    }

    public Task<PostPaymentResponse?> GetPaymentAsync(Guid id, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var payment = Payments.FirstOrDefault(p => p.Id == id);
        return Task.FromResult(payment);
    }
}
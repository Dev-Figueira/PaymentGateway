using MediatR;

using PaymentGateway.Application.Models.Responses;

namespace PaymentGateway.Application.Queries
{
    public class GetPaymentQuery : IRequest<GetPaymentResponse>
    {
        public Guid MerchantId { get; set; }
    }
}

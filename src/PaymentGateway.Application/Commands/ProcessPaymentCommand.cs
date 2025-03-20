using MediatR;

using PaymentGateway.Application.Models.Requests;
using PaymentGateway.Application.Models.Responses;

namespace PaymentGateway.Application.Commands
{
    public class ProcessPaymentCommand : IRequest<PostPaymentResponse>
    {
        public PostPaymentRequest PaymentRequest { get; set; }
    }
}

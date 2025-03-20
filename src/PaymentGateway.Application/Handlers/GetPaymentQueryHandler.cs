using MediatR;

using Microsoft.Extensions.Logging;

using PaymentGateway.Application.Interfaces;
using PaymentGateway.Application.Models.Responses;
using PaymentGateway.Application.Queries;
using PaymentGateway.Application.Utilities;
using PaymentGateway.Domain.Exceptions;

namespace PaymentGateway.Application.Handlers
{
    public class GetPaymentQueryHandler : IRequestHandler<GetPaymentQuery, GetPaymentResponse?>
    {
        private readonly ILogger<GetPaymentQueryHandler> _logger;
        private readonly IPaymentRepository _paymentRepository;

        public GetPaymentQueryHandler(IPaymentRepository paymentRepository)
        {
            _paymentRepository = paymentRepository;
        }

        public async Task<GetPaymentResponse?> Handle(GetPaymentQuery request, CancellationToken cancellationToken)
        {
            var payment = await _paymentRepository.GetPaymentAsync(request.MerchantId, cancellationToken);

            if (payment == null)
                throw new NotFoundException($"Payment not found for merchantId: {request.MerchantId}");
            
            try
            {
                var status = PaymentStatusMapper.MapFromString(payment.Status);

                return new GetPaymentResponse
                {
                    Id = payment.Id,
                    Status = status,
                    CardNumberLastFour = payment.CardNumberLastFour,
                    ExpiryMonth = payment.ExpiryMonth,
                    ExpiryYear = payment.ExpiryYear,
                    Currency = payment.Currency,
                    Amount = payment.Amount
                };
            }
            catch (Exception ex)
            {
                throw new BusinessException("Invalid payment status.", ex);
            }
        }
    }
}
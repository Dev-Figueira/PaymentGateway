using MediatR;

using Microsoft.Extensions.Logging;

using PaymentGateway.Application.Commands;
using PaymentGateway.Application.Interfaces;
using PaymentGateway.Application.Models.Requests;
using PaymentGateway.Application.Models.Responses;
using PaymentGateway.Domain;

namespace PaymentGateway.Application.Handlers
{
    public class ProcessPaymentCommandHandler : IRequestHandler<ProcessPaymentCommand, PostPaymentResponse>
    {
        private readonly ILogger<ProcessPaymentCommandHandler> _logger;
        private readonly IBankService _bankService;
        private readonly IPaymentRepository _paymentRepository;

        public ProcessPaymentCommandHandler(IBankService bankService, IPaymentRepository paymentRepository, ILogger<ProcessPaymentCommandHandler> logger)
        {
            _bankService = bankService;
            _paymentRepository = paymentRepository;
            _logger = logger;
        }

        public async Task<PostPaymentResponse> Handle(ProcessPaymentCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var bankRequest = new BankRequest
                {
                    CardNumber = request.PaymentRequest.CardNumber,
                    ExpiryDate = $"{request.PaymentRequest.ExpiryMonth:D2}/{request.PaymentRequest.ExpiryYear}",
                    Currency = request.PaymentRequest.Currency,
                    Amount = request.PaymentRequest.Amount,
                    CVV = request.PaymentRequest.CVV
                };

                var bankResponse = await _bankService.ProcessPaymentAsync(bankRequest, cancellationToken);

                var paymentResponse = new PostPaymentResponse
                {
                    Id = Guid.NewGuid(),
                    Status = bankResponse.Authorized ? PaymentStatus.Authorized.ToString() : PaymentStatus.Declined.ToString(),
                    CardNumberLastFour = request.PaymentRequest.CardNumber[^4..],
                    ExpiryMonth = request.PaymentRequest.ExpiryMonth,
                    ExpiryYear = request.PaymentRequest.ExpiryYear,
                    Currency = request.PaymentRequest.Currency,
                    Amount = request.PaymentRequest.Amount
                };

                await _paymentRepository.AddPaymentAsync(paymentResponse, cancellationToken);

                return paymentResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing payment for card ending with: {CardNumber}", request.PaymentRequest.CardNumber[^4..]);
                throw;
            }
        }
    }
}
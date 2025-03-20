using PaymentGateway.Application.Queries;
using PaymentGateway.Domain.Exceptions;

namespace PaymentGateway.Api.Tests.Handlers
{
    public class GetPaymentQueryHandlerTests
    {
        private readonly Mock<IPaymentRepository> _paymentRepositoryMock;
        private readonly GetPaymentQueryHandler _handler;

        public GetPaymentQueryHandlerTests()
        {
            _paymentRepositoryMock = new Mock<IPaymentRepository>();
            _handler = new GetPaymentQueryHandler(_paymentRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_PaymentFound_ReturnsPaymentResponse()
        {
            // Arrange
            var paymentResponse = PaymentTestDataGenerator.GeneratePostPaymentResponseValid();

            _paymentRepositoryMock.Setup(repo => repo.GetPaymentAsync(paymentResponse.Id, default))
                .ReturnsAsync(paymentResponse);

            var query = new GetPaymentQuery { MerchantId = paymentResponse.Id };

            // Act
            var result = await _handler.Handle(query, default);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(paymentResponse.Amount, result?.Amount);
            Assert.Equal(paymentResponse.Currency, result?.Currency);
            Assert.Equal(paymentResponse.Status, result?.Status.ToString());
        }

        [Fact]
        public async Task Handle_PaymentNotFound_ThrowsNotFoundException()
        {
            // Arrange
            var merchantId = Guid.NewGuid();

            _paymentRepositoryMock.Setup(repo => repo.GetPaymentAsync(merchantId, default))
                .ReturnsAsync((PostPaymentResponse?)null);

            var query = new GetPaymentQuery { MerchantId = merchantId };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(query, default));
            Assert.Equal($"Payment not found for merchantId: {merchantId}", exception.Message);
        }
    }
}

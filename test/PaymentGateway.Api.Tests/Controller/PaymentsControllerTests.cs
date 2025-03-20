using MediatR;

using Microsoft.AspNetCore.Mvc;

using PaymentGateway.Api.Controllers;
using PaymentGateway.Application.Queries;

namespace PaymentGateway.Api.Tests.Controller
{
    public class PaymentsControllerTests
    {
        private readonly Mock<IMediator> _mockMediator;
        private readonly Mock<ILogger<PaymentsController>> _mockLogger;
        private readonly PaymentsController _controller;

        public PaymentsControllerTests()
        {
            _mockMediator = new Mock<IMediator>();
            _mockLogger = new Mock<ILogger<PaymentsController>>();
            _controller = new PaymentsController(_mockMediator.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task GetPaymentAsync_ShouldReturnNotFound_WhenPaymentIsNotFound()
        {
            // Arrange
            var merchantId = Guid.NewGuid();

            _mockMediator.Setup(m => m.Send(It.IsAny<GetPaymentQuery>(), default))
             .ReturnsAsync((GetPaymentResponse?)null);

            // Act
            var result = await _controller.GetPaymentAsync(merchantId, default);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task GetPaymentAsync_ShouldReturnOk_WhenPaymentIsFound()
        {
            // Arrange
            var merchantId = Guid.NewGuid();
            var paymentResponse = PaymentTestDataGenerator.CreateGetPaymentResponseValid(); 
            _mockMediator.Setup(m => m.Send(It.IsAny<GetPaymentQuery>(), default))
                         .ReturnsAsync(paymentResponse);

            // Act
            var result = await _controller.GetPaymentAsync(merchantId, default);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<GetPaymentResponse>(okResult.Value);
            Assert.Equal(paymentResponse.Amount, returnValue.Amount);
        }

        [Fact]
        public async Task ProcessPayment_ShouldReturnOk_WhenPaymentIsProcessedSuccessfully()
        {
            // Arrange
            var paymentRequest = PaymentTestDataGenerator.CreatePaymentRequestValid(); 

            var paymentResponse = PaymentTestDataGenerator.GeneratePostPaymentResponseValid();

            _mockMediator.Setup(m => m.Send(It.IsAny<ProcessPaymentCommand>(), default))
                         .ReturnsAsync(paymentResponse);

            // Act
            var result = await _controller.ProcessPayment(paymentRequest, default);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<PostPaymentResponse>(okResult.Value);
            Assert.Equal(paymentResponse.Amount, returnValue.Amount);
        }
    }
}

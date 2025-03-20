public class ProcessPaymentCommandHandlerTests
{
    private readonly Mock<IBankService> _mockBankService;
    private readonly Mock<IPaymentRepository> _mockPaymentRepository;
    private readonly Mock<ILogger<ProcessPaymentCommandHandler>> _mockLogger;
    private readonly ProcessPaymentCommandHandler _handler;

    public ProcessPaymentCommandHandlerTests()
    {
        _mockBankService = new Mock<IBankService>();
        _mockPaymentRepository = new Mock<IPaymentRepository>();
        _mockLogger = new Mock<ILogger<ProcessPaymentCommandHandler>>();
        _handler = new ProcessPaymentCommandHandler(_mockBankService.Object, _mockPaymentRepository.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnAuthorizedResponse_WhenBankAuthorizesPayment()
    {
        // Arrange
        var paymentRequest = PaymentTestDataGenerator.CreatePaymentRequestValid(); 
        var command = new ProcessPaymentCommand
        {
            PaymentRequest = paymentRequest
        };

        var bankResponse = new BankResponse { Authorized = true };
        _mockBankService.Setup(x => x.ProcessPaymentAsync(It.IsAny<BankRequest>(), It.IsAny<CancellationToken>())).ReturnsAsync(bankResponse);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(PaymentStatus.Authorized.ToString(), result.Status);
        _mockPaymentRepository.Verify(x => x.AddPaymentAsync(It.IsAny<PostPaymentResponse>(), default), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnDeclinedResponse_WhenBankDeclinesPayment()
    {
        // Arrange
        var paymentRequest = PaymentTestDataGenerator.CreatePaymentRequestValid();
        var command = new ProcessPaymentCommand
        {
            PaymentRequest = paymentRequest
        };

        var bankResponse = new BankResponse { Authorized = false };
        _mockBankService.Setup(x => x.ProcessPaymentAsync(It.IsAny<BankRequest>(), default)).ReturnsAsync(bankResponse);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(PaymentStatus.Declined.ToString(), result.Status);
        _mockPaymentRepository.Verify(x => x.AddPaymentAsync(It.IsAny<PostPaymentResponse>(), default), Times.Once);
    }
}

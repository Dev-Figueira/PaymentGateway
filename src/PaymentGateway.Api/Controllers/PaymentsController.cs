using MediatR;

using Microsoft.AspNetCore.Mvc;

using PaymentGateway.Application.Commands;
using PaymentGateway.Application.Models.Requests;
using PaymentGateway.Application.Models.Responses;
using PaymentGateway.Application.Queries;

namespace PaymentGateway.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PaymentsController : Controller
{
    private readonly ILogger<PaymentsController> _logger;
    private readonly IMediator _mediator;
    public PaymentsController(IMediator mediator, ILogger<PaymentsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Get transaction for merchant id
    /// </summary>
    /// <param name="merchantId"></param>
    /// <returns></returns>
    [HttpGet("{merchantId:guid}")]
    public async Task<ActionResult<PostPaymentResponse?>> GetPaymentAsync(Guid merchantId, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching payment with merchantId: {MerchantId}", merchantId);

        var query = new GetPaymentQuery { MerchantId = merchantId };
        var payment = await _mediator.Send(query, cancellationToken);

        if (payment == null)
        {
            _logger.LogWarning("Payment not found for merchantId: {MerchantId}", merchantId);
            return NotFound();
        }

        _logger.LogInformation("Payment retrieved successfully for merchantId: {MerchantId}", merchantId);
        return Ok(payment);
    }


    /// <summary>
    /// Process a transaction
    /// </summary>
    /// <param name="transactionRepresenter"></param>
    /// <returns></returns>
    [HttpPost]
    [Route("process")]
    public async Task<IActionResult> ProcessPayment([FromBody] PostPaymentRequest paymentRequest, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Processing payment for card ending with: {CardNumber}", paymentRequest.CardNumber[^4..]);

        var command = new ProcessPaymentCommand { PaymentRequest = paymentRequest };
        var result = await _mediator.Send(command, cancellationToken);

        _logger.LogInformation("Payment processed successfully. Transaction ID: {TransactionId}", result.Id);
        return Ok(result);
    }
}
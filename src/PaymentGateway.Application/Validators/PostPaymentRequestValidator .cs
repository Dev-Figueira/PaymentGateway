using FluentValidation;

using PaymentGateway.Application.Models.Requests;

namespace PaymentGateway.Application.Validators
{
    public class PostPaymentRequestValidator : AbstractValidator<PostPaymentRequest>
    {
        private readonly List<string> _validCurrencies = ["USD", "EUR", "GBP"];

        public PostPaymentRequestValidator()
        {
            RuleFor(x => x.CardNumber)
                .NotEmpty().WithMessage("The card number is required.")
                .Length(14, 19).WithMessage("The card number must be between 14 and 19 characters.")
                .Matches(@"^\d+$").WithMessage("The card number must contain only digits.");

            RuleFor(x => x.ExpiryMonth)
                .NotEmpty().WithMessage("The expiration month is required.")
                .InclusiveBetween(1, 12).WithMessage("The expiration month must be between 1 and 12.");

            RuleFor(x => x.ExpiryYear)
                .NotEmpty().WithMessage("The expiration year is required.")
                .InclusiveBetween(1000, 9999).WithMessage("The expiryYear must be a 4-digit number.")
                .When(x => x.ExpiryMonth >= 1 && x.ExpiryMonth <= 12)
                .Must((request, year) => year >= 1000 && year <= 9999 && ValidExpirationDate(request.ExpiryMonth, year))
                .WithMessage("The expiration date must be in the future.")
                .When(x => x.ExpiryMonth >= 1 && x.ExpiryMonth <= 12);

            RuleFor(x => x.Currency)
                .NotEmpty().WithMessage("The currency is required.")
                .Length(3).WithMessage("The currency must be 3 characters.")
                .Must(ValidCurrency).WithMessage("The currency is not valid. Valid currencies are: USD, EUR, GBP.");

            RuleFor(x => x.Amount)
                .NotEmpty().WithMessage("The amount is required.")
                .GreaterThan(0).WithMessage("The amount must be greater than zero.");

            RuleFor(x => x.CVV)
                .NotEmpty().WithMessage("The CVV é required.")
                .Length(3, 4).WithMessage("The CVV must be between 3 and 4 characters.")
                .Matches(@"^\d+$").WithMessage("The CVV must contain only digits.");
        }

        private static bool ValidExpirationDate(int expiryMonth, int expiryYear)
        {
            var currentDate = DateTime.UtcNow;
            var expirationDate = new DateTime(expiryYear, expiryMonth, 1).AddMonths(1).AddDays(-1);
            return expirationDate > currentDate;
        }

        private bool ValidCurrency(string currency)
        {
            return _validCurrencies.Contains(currency);
        }
    }
}
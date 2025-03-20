using Bogus;

namespace PaymentGateway.Api.Tests.Features
{
    public static class PaymentTestDataGenerator
    {
        private static readonly string[] AvailableCurrencies = { "USD", "EUR", "GBP" };
        private static readonly Faker Faker = new();

        public static PostPaymentResponse GeneratePostPaymentResponseValid()
        {
            var paymentResponse = new PostPaymentResponse
            {
                Id = Guid.NewGuid(),
                Amount = Faker.Random.Int(1, 1000),  
                Currency = Faker.PickRandom(AvailableCurrencies),
                CardNumberLastFour = Faker.Finance.CreditCardNumber().Substring(12, 4),  
                ExpiryMonth = Faker.Date.Between(DateTime.Now, DateTime.Now.AddYears(1)).Month,  
                ExpiryYear = Faker.Date.Between(DateTime.Now, DateTime.Now.AddYears(1)).Year,  
                Status = PaymentStatus.Authorized.ToString()  
            };

            return paymentResponse;
        }

        public static PostPaymentRequest CreatePaymentRequestValid()
        {
            return new PostPaymentRequest
            {
                CardNumber = Faker.Random.Long(1000000000000000, 9999999999999999).ToString(),
                ExpiryMonth = Faker.Date.Between(DateTime.Now, DateTime.Now.AddYears(1)).Month,
                ExpiryYear = Faker.Date.Between(DateTime.Now, DateTime.Now.AddYears(1)).Year,
                Currency = Faker.PickRandom(AvailableCurrencies),
                Amount = Faker.Random.Int(1, 1000),
                CVV = Faker.Random.Int(100, 999).ToString()
            };
        }

        public static GetPaymentResponse CreateGetPaymentResponseValid()
        {
            return new GetPaymentResponse
            {
                Id = Guid.NewGuid(),
                Amount = Faker.Random.Int(1, 1000),
                Currency = Faker.PickRandom(AvailableCurrencies)
            };
        }
    }
}
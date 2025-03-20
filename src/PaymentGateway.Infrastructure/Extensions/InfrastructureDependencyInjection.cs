using System.Net.Http.Headers;

using Microsoft.Extensions.DependencyInjection;

using PaymentGateway.Application.Interfaces;
using PaymentGateway.Infrastructure.Repositories;
using PaymentGateway.Infrastructure.Services;

namespace PaymentGateway.Infrastructure.Extensions
{
    public static class InfrastructureDependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            services.AddHttpClient<IBankService, BankService>(client =>
            {
                client.BaseAddress = new Uri("http://localhost:8080/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            });

            services.AddSingleton<IPaymentRepository, PaymentRepository>();
            return services;
        }
    }
}
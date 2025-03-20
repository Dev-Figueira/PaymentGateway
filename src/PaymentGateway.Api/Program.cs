using System.Text.Json.Serialization;

using FluentValidation;
using FluentValidation.AspNetCore;

using PaymentGateway.Application.Extensions;
using PaymentGateway.Application.Validators;
using PaymentGateway.CrossCutting.Middleware;
using PaymentGateway.Infrastructure.Extensions;

using Serilog;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

builder.Services.AddApplication(); 
builder.Services.AddInfrastructure(); 

builder.Services.AddValidatorsFromAssemblyContaining(typeof(PostPaymentRequestValidator));
builder.Services.AddFluentValidationAutoValidation();

builder.Services.AddHttpClient();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()); 
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseMiddleware<InterceptorHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger(); 
    app.UseSwaggerUI();
}

app.UseHttpsRedirection(); 
app.UseAuthorization(); 

app.MapControllers(); 

app.Run(); 
# Project Documentation

## Overview

This project is designed to implement a **Payment Gateway** using **CQRS (Command Query Responsibility Segregation)** to separate the concerns of commands and queries. The solution is built with **.NET 8** and follows the **Clean Architecture** principles to ensure scalability, maintainability, and proper testing.

## **Applied Principles**

### **1. Clean Architecture**
- Ensures **independence** between the core business logic and external components like frameworks, databases, and UI.
- Follows **layered separation**, where:
  - **Application Layer** contains business rules, handlers, and models.
  - **Infrastructure Layer** includes database repositories and external service calls.
  - **API Layer** exposes HTTP endpoints.

### **2. CQRS (Command Query Responsibility Segregation)**
- **Commands** (modifying actions) and **Queries** (retrieving actions) are handled separately.
- **Justification**: Even though CQRS is often used with separate databases for reads and writes, in this implementation, it is applied only at the **application layer** to improve code clarity and maintainability.
- **Benefit**: This separation makes it easier to scale the system in the future without major refactoring.

### **3. Dependency Injection & Decoupling**
- **MediatR** is used to ensure communication between handlers and controllers without tight coupling.
- **Repositories and external services** are injected using **interfaces**, making the system testable and maintainable.

### **4. Resilience and Fault Tolerance**
- **Polly** is used to implement retry policies, making API calls to external systems more resilient.
- Error handling ensures that **failures are logged properly** without disrupting the entire system.

### **5. Logging and Observability**
- **ILogger** is integrated into the system to track:
  - **Success logs** for normal operations.
  - **Warnings** for cases like missing payments.
  - **Error logs** when an unexpected failure occurs.

---

## **Usage of Polly for Resilience**

To handle transient failures and improve API stability, **Polly** is used to implement a retry mechanism when calling the **bank service** for payment processing.

### **Implementation Details**
- **Retry Policy**: If the request to the bank fails, Polly retries **3 times** with an **exponential backoff** strategy.
- **Handled Exceptions**:
  - **HttpRequestException**
  - **Non-success HTTP responses** (e.g., 500 Internal Server Error)

#### **Example from `BankService.cs`**
```csharp
_retryPolicy = Policy
    .Handle<HttpRequestException>()
    .OrResult<HttpResponseMessage>(r => r.StatusCode == HttpStatusCode.RequestTimeout || !r.IsSuccessStatusCode)
    .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (exception, timeSpan, retryCount, context)
    => _logger.LogWarning("Retry {RetryCount} due to {ExceptionType}. Retrying in {TimeSpanSeconds} seconds.",
            retryCount, exception.GetType().Name, timeSpan.TotalSeconds));
```

## **Assumptions Made**

- **Simplified CQRS**: As mentioned, the separation between command and query responsibilities is conceptual, and there is no use of distinct databases for each. This decision was made to avoid unnecessary complexity while still achieving the main benefits of **CQRS** in terms of clear responsibility separation.
- **No external payment gateway integration**: In this version of the project, payment processing is simulated with a mock **bank service** rather than integrating with a real-world payment gateway.
- **Basic Payment Flow**: The system supports basic functionality for processing payments and retrieving transaction data for a merchant, with minimal validation checks for simplicity.
- **Limited Currency Support**: The system supports only the following currencies:
    - USD
    - EUR
    - GBP
- **No Authentication/Authorization**: For simplicity, security measures such as OAuth or JWT authentication are not implemented in this version.

## **Folder Structure**

```plaintext
.
├── Presentation
│   └── Controllers
├── Application
│   ├── Commands
│   ├── Handlers
│   ├── Models
│   ├── Queries
│   └── Interfaces
├── Domain
│   ├── Enum
│   ├── Exceptions
├── Infrastructure
│   ├── Repositories
│   ├── Services
│   ├── Extensions
├── Infrastructure.CrossCutting
│   ├── Middleware
│   ├── Models
└── Tests
    ├── Controller
    ├── Features
    └── Handlers

```

## **Endpoints Documentation**
1. Process Payment Endpoint: POST /payments
- Request Body:

```json
{
    "cardNumber": "2222405343248877",
    "expiryMonth": 12,
    "expiryYear": 2025,
    "currency": "USD",
    "amount": 100,
    "cvv": "123"
}
```
- Response:
```json
{
    "id": "550e8400-e29b-41d4-a716-446655440000",
    "status": "Authorized",
    "cardNumberLastFour": "3456",
    "expiryMonth": 12,
    "expiryYear": 2025,
    "currency": "USD",
    "amount": 100
}
```
- Request Body:
```json
{
    "cardNumber": "2222405343248876",
    "expiryMonth": 12,
    "expiryYear": 2025,
    "currency": "USD",
    "amount": 100,
    "cvv": "123"
}
```
- Response:
```json
{
    "id": "68032b17-3fab-4cec-a968-d8b3d6d58149",
    "status": "Declined",
    "cardNumberLastFour": "3456",
    "expiryMonth": 12,
    "expiryYear": 2025,
    "currency": "USD",
    "amount": 100
}
```
- Request Body:
```json
{
    "cardNumber": "2222405343248870",
    "expiryMonth": 12,
    "expiryYear": 2025,
    "currency": "USD",
    "amount": 100,
    "cvv": "123"
}
```
- Response:
```json
{
  "StatusCode": 503,
  "Message": "Bank returned status code: ServiceUnavailable"
}
```

2. Get Payment Details
Endpoint: GET /payments/{id}
- Request Body:

```json
{
    "id": "550e8400-e29b-41d4-a716-446655440000",
    "status": "Authorized",
    "cardNumberLastFour": "3456",
    "expiryMonth": 12,
    "expiryYear": 2025,
    "currency": "USD",
    "amount": 100
}
```

- Error Response (Not Found):
```json
{
    "error": "Payment not found for ID: {id}"
}
```
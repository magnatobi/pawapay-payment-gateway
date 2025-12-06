# PawaPay Payment Gateway

A comprehensive payment gateway integration service that provides a unified interface for multiple African payment gateways and banks. This .NET 7.0 application enables seamless integration with Flutterwave, Paystack, Opay, and other payment providers through a single, consistent API.

## 🚀 Features

- **Multi-Gateway Support**: Integrate with multiple payment providers (Flutterwave, Paystack, Opay) simultaneously
- **Unified API**: Single interface for all payment operations across different providers
- **Gateway Management**: Dynamic gateway registration and management with priority-based routing
- **Transaction Operations**:
  - Receive payments (payment links)
  - Send funds (bank transfers)
  - Query transaction status
  - Validate bank account numbers
  - Check gateway balance and availability
- **Intelligent Routing**: Automatic gateway selection based on priority and availability
- **Error Handling**: Comprehensive error handling with email notifications
- **Swagger Documentation**: Built-in API documentation for easy integration
- **Docker Support**: Container-ready for easy deployment

## 🏗️ Architecture

The project follows a clean architecture pattern with clear separation of concerns:

```
PawaPayGateway/
├── Domain/              # Domain entities, interfaces, and business logic
│   ├── Entities/       # Domain entities (Gateway, etc.)
│   ├── Interfaces/      # Core interfaces (ITransactionProvider, IBankTransactionProvider)
│   ├── Enums/          # Enumerations (GatewayType, TransactionStatus, etc.)
│   ├── Models/         # Request/Response models for each gateway
│   └── Responses/      # Common response models
├── Infrastructure/      # Infrastructure implementations
│   ├── Services/       # Gateway service implementations
│   ├── Implementations/# Core implementations (GatewayManager, RepositoryBase)
│   ├── Configs/        # Configuration classes
│   └── Data/           # Data context and database setup
├── Application/        # Application layer (behaviors, validations)
└── Controllers/        # API controllers
```

## 🛠️ Technology Stack

- **.NET 7.0**: Modern C# framework
- **ASP.NET Core Web API**: RESTful API framework
- **Entity Framework Core 7.0.20**: ORM for database operations
- **RestSharp 112.1.0**: HTTP client for external API calls
- **Swashbuckle.AspNetCore 6.5.0**: Swagger/OpenAPI documentation
- **Docker**: Containerization support

## 📋 Supported Payment Gateways

The following payment gateways are currently supported:

1. **Paystack** - Payment processing and bank transfers
2. **Flutterwave** - Payment processing, bank transfers, and airtime
3. **Opay** - Payment processing and transfers
4. **Squad** (Planned)
5. **Remitta** (Planned)

## 🚦 Getting Started

### Prerequisites

- .NET 7.0 SDK or later
- Visual Studio 2022 or VS Code (optional)
- SQL Server or compatible database (for Entity Framework)
- Docker (optional, for containerized deployment)

### Installation

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd pawapay-payment-gateway
   ```

2. **Restore dependencies**
   ```bash
   dotnet restore
   ```

3. **Configure the application**
   
   Update `appsettings.json` or `appsettings.Development.json` with your configuration:
   
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "your-connection-string"
     },
     "DOMAINAPIHOST": "https://your-api-host.com",
     "IRISAPIHOST": "https://your-iris-api-host.com",
     "paystack_yourdomain": {
       "SecretKey": "your-paystack-secret-key"
     },
     "opay_yourdomain": {
       "AuthToken": "your-opay-auth-token"
     },
     "paystack_pawapay": {
       "SecretKey": "your-flutterwave-secret-key"
     }
   }
   ```

4. **Set up the database**
   
   Configure your database connection string and run Entity Framework migrations (if applicable).

5. **Run the application**
   ```bash
   dotnet run --project PawaPayGateway/PawaPayGateway.csproj
   ```

6. **Access Swagger UI**
   
   Navigate to `https://localhost:<port>/swagger` in your browser to view the API documentation.

### Docker Deployment

The project includes Docker support. To run using Docker:

```bash
docker build -t pawapay-gateway .
docker run -p 443:443 pawapay-gateway
```

## ⚙️ Configuration

### Gateway Configuration

Gateways are configured dynamically from the database. Each gateway requires:

- **Name**: Display name of the gateway
- **GatewayType**: Type of gateway (Paystack, Flutterwave, Opay, etc.)
- **CredentialKey**: Key used to retrieve credentials from `appsettings.json`
- **Priority**: Priority level for gateway selection (lower numbers = higher priority)
- **Status**: Current status (Online, Offline, Error, Disabled, Checking)
- **GatewayUrl**: Base URL for the gateway API

### Environment Variables

- `DOMAINAPIHOST`: Base URL for your domain API (used for Paystack callbacks)
- `IRISAPIHOST`: Base URL for your IRIS API (used for Flutterwave callbacks)

### Gateway Credentials

Credentials are stored in `appsettings.json` using the `CredentialKey` as the section name:

```json
{
  "paystack_yourdomain": {
    "SecretKey": "sk_test_..."
  },
  "opay_yourdomain": {
    "AuthToken": "your-auth-token"
  },
  "paystack_pawapay": {
    "SecretKey": "FLWSECK_TEST-..."
  }
}
```

## 📚 API Overview

### Core Interfaces

#### `ITransactionProvider`
Base interface for all transaction providers with common operations:
- `CheckAvailability()`: Check if gateway is available
- `QueryGatewayBalance()`: Get gateway balance

#### `IBankTransactionProvider`
Extends `ITransactionProvider` with banking operations:
- `ReceiveFundsDetails()`: Get payment provider details
- `ReceiveFundsLink()`: Generate payment link
- `SendFunds()`: Transfer funds to bank account
- `QueryTransaction()`: Query transaction status
- `GetAccountName()`: Validate and get bank account name

### Gateway Manager

The `GatewayManager` provides:
- Dynamic gateway registration
- Priority-based gateway selection
- Gateway filtering by type and status
- Concurrent gateway management

## 🔧 Project Structure

### Domain Layer
- **Entities**: Core domain entities like `Gateway`
- **Interfaces**: Contracts for services and repositories
- **Enums**: `GatewayType`, `GatewayStatus`, `TransactionStatusEnum`, `GatewayUseCaseEnum`
- **Models**: Request/Response models for each payment gateway
- **Responses**: Common response models

### Infrastructure Layer
- **Services**: 
  - `PaystackTransferService`: Paystack integration
  - `FlutterWaveService`: Flutterwave integration
  - `OpayService`: Opay integration
- **Implementations**:
  - `GatewayManager`: Manages multiple gateway instances
  - `TransactionProvider`: Base implementation for transaction providers
- **Configs**: Configuration classes for each gateway
- **Data**: Entity Framework context and data seeding

### Application Layer
- **Behaviours**: MediatR behaviors for validation and exception handling

## 🔐 Security Considerations

- Store sensitive credentials in `appsettings.json` or use User Secrets for development
- Use environment variables for production configurations
- Implement proper authentication and authorization for API endpoints
- Use HTTPS in production environments
- Regularly rotate API keys and tokens

## 📝 Usage Examples

### Registering Gateways

Gateways are automatically registered on application startup based on database configuration:

```csharp
// In Program.cs
builder.Services.AddPaystack(builder.Configuration);
builder.Services.AddOpayPayment(builder.Configuration);
builder.Services.AddFlutterwave(builder.Configuration);
```

### Using Gateway Manager

```csharp
// Get all active gateways
var gateways = gatewayManager.GetGatewaysByType(GatewayType.Paystack);

// Get specific gateway
var gateway = gatewayManager.GetGatewayById(gatewayId);

// Use gateway for transaction
if (gateway is IBankTransactionProvider bankProvider)
{
    var result = await bankProvider.SendFunds(
        bankCode: "058",
        accountNumber: "1234567890",
        amount: 1000.00m,
        reference: "TXN-001"
    );
}
```

## 🧪 Testing

To test the application:

1. Ensure all required services are configured
2. Use Swagger UI to test API endpoints
3. Verify gateway configurations in the database
4. Test with sandbox/test credentials before production use

## 🤝 Contributing

Contributions are welcome! Please follow these steps:

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add some amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## 📄 License

This project is licensed under the MIT License - see the LICENSE file for details.

## 🆘 Support

For issues, questions, or contributions, please open an issue on the repository.

## 🔮 Roadmap

- [ ] Add Squad payment gateway integration
- [ ] Add Remitta payment gateway integration
- [ ] Implement comprehensive unit and integration tests
- [ ] Add webhook handling for payment callbacks
- [ ] Implement retry mechanisms for failed transactions
- [ ] Add transaction logging and audit trails
- [ ] Implement rate limiting
- [ ] Add monitoring and health checks

## 📞 Contact

For more information, please contact the development team.

---

**Note**: This is a payment gateway integration service. Ensure you comply with all relevant financial regulations and PCI-DSS requirements when handling payment data.

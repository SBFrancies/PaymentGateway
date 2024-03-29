# PaymentGateway

## Description
A solution containing a Payment Gateway API to make and retrieve payments. Teh solution also contains two other services, a mock Bank API and a simple Merhcant front end which can be used to test the Payment Gateway API.

## Requirements to run

1) .Net 7.0
2) SQL Server
3) An OpenIdConnect compliant identity provider (e.g. Azure AD B2C)
4) Cosmos DB
5) Azure key vault (optional)
6) Azure table storage (optional)

## Projects

### PaymentGateway.Api

The main project containing the Payment Gateway API.

### PaymentGateway.BankSimulator

Contains the mock Banking API.

### PaymentGateway.IntegrationTests

Contains the integration tests for the API project.

### PaymentGateway.Library

Common code to be shared between projects to avoid duplication.

### PaymentGateway.Merchant

A simple front end for testing the API.

### PaymentGateway.UnitTests

Contains the unit tests for the API project.

## Architecture

The API project consists of the following components:
1) A RESTful JSON over HTTP API for making and retreiving payments
2) A backing SQL server store for persisting payment details
3) Azure table storage for storing logs
4) Azure CosmosDB for storing payment event information
5) An OpenIDConnent (OIDC) compliant identity provider - in the test project Azure AD B2C is used
6) Azure KeyVault for storing secret configuration data

### Payments table schema

```
CREATE TABLE Payments
(
Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY NONCLUSTERED,
RowIndex INT NOT NULL IDENTITY,
DateCreated DATETIMEOFFSET NOT NULL DEFAULT GETUTCDATE(),
CreatedBy VARCHAR(200) NOT NULL,
Amount DECIMAL(20,8) NOT NULL,
CurrencyCode VARCHAR(10) NOT NULL,
CardNumber VARCHAR(30) NOT NULL,
CardName VARCHAR(100) NOT NULL,
ExpiryMonth TINYINT NOT NULL,
ExpiryYear SMALLINT NOT NULL,
Reference VARCHAR(50),
BankCode VARCHAR(100)
)

CREATE CLUSTERED INDEX IX_Payments ON Payments(RowIndex)
```

## Dev environment

Each of the three web projects are hosted in an Azure dev environment. The services are deployed as Azure App Services for containers using a CI/CD pipeline. These are located at:

1) Payment Gateway API: https://wa-payment-gateway-api.azurewebsites.net/swagger

2) Bank Simulator API: https://wa-payment-gateway-bank.azurewebsites.net/swagger

3) Merchant: https://wa-payment-gateway-merchant.azurewebsites.net/

The Payment Gateway API and mock Bank API can be tested via their Open API (swagger) UI pages. The Merchant UI can also be used for testing.

### Test credentials:

1) 
Username: TestUser1@paymentgatewayad.onmicrosoft.com
Password: WE45wSxY

2)
Username: TestUser2@paymentgatewayad.onmicrosoft.com
Password: 4q*v^%FxMnKUnfh&

## If I had more time / Future changes

1) Use something better than Azure Table Storage to be able to persist structured logs, maybe Seq or GreyLog

2) Use Terraform or Pulumi or some other infrastructure as code to to build and tear down the infrastructure

3) Add endpoints for other types of payment like Bank Transfers

4) Improve error handling logic - probably a middleware based solution

5) Better solution for local running

6) Write other event store providers

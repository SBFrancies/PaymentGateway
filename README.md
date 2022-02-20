# PaymentGateway

## Description
A solution containing a Payment Gateway API to make and retrieve payments. Teh solution also contains two other services, a mock Bank API and a simple Merhcant front end which can be used to test the Payment Gateway API.

## Requirements to run

1) .Net Core 3.1
2) SQL Server
3) An OpenIdConnect compliant identity provider (e.g. Azure AD B2C)
4) Redis cache
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
4) A Redis cache for storing payment event information
5) An OpenIDConnent (OIDC) compliant identity provider - in the test project Azure AD B2C is used
6) Azure KeyVault for storing secret configuration data

### Payments table schema

```
CREATE TABLE Payments
(
Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
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
```

## Dev environment

Each of the three web projects are hosted in an Azure dev environment. These are located at:

1) Payment Gateway API: https://paymentgatewaychallenge-api.azurewebsites.net/swagger

2) Bank Simulator API: https://paymentgatewaychallenge-bank.azurewebsites.net/swagger

3) Merchant: https://paymentgatewaychallenge-merchant.azurewebsites.net/

The Payment Gateway API and mock Bank API can be tested via their Open API (swagger) UI pages. The Merchant UI can also be used for testing.

### Test credentials:

Username: TestUser1@paymentgatewayad.onmicrosoft.com
Password: WE45wSxY

## If I had more time / Future changes

1) Add StyleCop or some other static code analysis tool

2) Add SAST and/or DAST scans to the deployment pipeline

3) Upgrade the projects to .Net 6

4) Use something better than Azure Table Storage to be able to persist structured logs, maybe Seq or GreyLog

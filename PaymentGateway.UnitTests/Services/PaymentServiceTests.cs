using FluentValidation;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Moq;
using PaymentGateway.Api.Data.Entities;
using PaymentGateway.Api.Interface;
using PaymentGateway.Api.Models.BankApi;
using PaymentGateway.Api.Models.Request;
using PaymentGateway.Api.Models.Response;
using PaymentGateway.Api.Services;
using PaymentGateway.Library.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace PaymentGateway.UnitTests.Services
{
    public class PaymentServiceTests
    {
        private readonly Mock<IEventStore> _mockEventStore = new Mock<IEventStore>();
        private readonly Mock<IPaymentDataAccess> _mockPaymentDataAccess = new Mock<IPaymentDataAccess>();
        private readonly Mock<IBankApi> _mockBankApi = new Mock<IBankApi>();
        private readonly Mock<ILogger<PaymentService>> _mockLogger = new Mock<ILogger<PaymentService>>();
        private readonly Mock<IValidator<CreatePaymentRequest>> _mockValidator = new Mock<IValidator<CreatePaymentRequest>>();
        private readonly Mock<IMapper<CreatePaymentRequest, CardPaymentRequest>> _mockToBankApiMapper = new Mock<IMapper<CreatePaymentRequest, CardPaymentRequest>>();
        private readonly Mock<IIdentifierGenerator> _mockIdGenerator = new Mock<IIdentifierGenerator>();
        private readonly Mock<IMapper<CreatePaymentRequest, PaymentTable>> _mockToDbMaper = new Mock<IMapper<CreatePaymentRequest, PaymentTable>>();
        private readonly Mock<IBankStatusHolder> _mockBankStatusHolder = new Mock<IBankStatusHolder>();
        private readonly Mock<IMapper<PaymentTable, PaymentDetails>> _mockToDetailsMapper = new Mock<IMapper<PaymentTable, PaymentDetails>>();
        private readonly Mock<ISystemClock> _mockClock = new Mock<ISystemClock>();


        private PaymentService GetSystemUnderTest()
        {
            return new PaymentService(
                _mockLogger.Object,
                _mockBankApi.Object,
                _mockPaymentDataAccess.Object,
                _mockEventStore.Object,
                _mockValidator.Object,
                _mockToBankApiMapper.Object,
                _mockIdGenerator.Object,
                _mockToDbMaper.Object,
                _mockBankStatusHolder.Object,
                _mockToDetailsMapper.Object,
                _mockClock.Object
                );
        }
    }
}

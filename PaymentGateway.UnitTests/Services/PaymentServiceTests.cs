using AutoFixture;
using FluentValidation;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Moq;
using PaymentGateway.Api.Data.Entities;
using PaymentGateway.Api.Interface;
using PaymentGateway.Api.Mapping;
using PaymentGateway.Api.Models.BankApi;
using PaymentGateway.Api.Models.Request;
using PaymentGateway.Api.Models.Response;
using PaymentGateway.Api.Services;
using PaymentGateway.Library.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

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
        private readonly Mock<ICardNumberHider> _mockCardNumberHider = new Mock<ICardNumberHider>();
        private readonly Fixture _fixture = new Fixture();
        
        [Fact]
        public async Task PaymentService_FetchPaymentAsync_NoPaymentReturnsNull()
        {
            var id = Guid.NewGuid();
            _mockPaymentDataAccess.Setup(a => a.GetPaymentAsync(id)).ReturnsAsync((PaymentTable)null);
            var sut = GetSystemUnderTest();
        
            var result = await sut.FetchPaymentAsync(id);

            Assert.Null(result);
            _mockPaymentDataAccess.Verify(a => a.GetPaymentAsync(id), Times.Once);
            _mockEventStore.Verify(a => a.GetPaymentEventsAsync(id), Times.Never);
            _mockToDetailsMapper.Verify(a => a.Map(It.IsAny<PaymentTable>()), Times.Never);
        }

        [Fact]
        public async Task PaymentService_FetchPaymentAsync_ReturnsPaymentIfExists()
        {
            var now = DateTimeOffset.UtcNow;
            _mockClock.SetupGet(a => a.UtcNow).Returns(now);
            var payment = _fixture.Create<PaymentTable>();
            var events = _fixture.CreateMany<PaymentEvent>(4);

            _mockCardNumberHider.Setup(a => a.ObscureCardNumber(payment.CardNumber)).Returns("XXX");
            _mockToDetailsMapper.Setup(a => a.Map(payment)).Returns(() => new PaymentTableToPaymentDetailsMapper(_mockCardNumberHider.Object).Map(payment));
            _mockPaymentDataAccess.Setup(a => a.GetPaymentAsync(payment.Id)).ReturnsAsync(payment);
            _mockEventStore.Setup(a => a.GetPaymentEventsAsync(payment.Id)).ReturnsAsync(events);
            var sut = GetSystemUnderTest();

            var result = await sut.FetchPaymentAsync(payment.Id);

            Assert.NotNull(result);

            Assert.Equal(payment.Reference, result.Payment.Reference);
            Assert.Equal(payment.Amount, result.Payment.Amount);
            Assert.Equal(payment.CurrencyCode, result.Payment.CurrencyCode);
            Assert.Equal(payment.Id, result.Payment.Id);
            Assert.Equal(payment.ExpiryYear, result.Payment.ExpiryYear);
            Assert.Equal(payment.ExpiryMonth, result.Payment.ExpiryMonth);
            Assert.Equal(payment.CardName, result.Payment.CardName);
            Assert.Equal("XXX", result.Payment.CardNumber);
            Assert.Equal(payment.BankCode, result.Payment.BankCode);
            Assert.Equal(now, result.ResponseTime);
            Assert.Equal(events.Count(), result.Events.Count());

            foreach(var e in result.Events)
            {
                Assert.True(events.SingleOrDefault(a => a.TimeStamp == e.TimeStamp && a.Status == e.Status) != null);
            }

            _mockPaymentDataAccess.Verify(a => a.GetPaymentAsync(payment.Id), Times.Once);
            _mockEventStore.Verify(a => a.GetPaymentEventsAsync(payment.Id), Times.Once);
            _mockToDetailsMapper.Verify(a => a.Map(payment), Times.Once);
            _mockCardNumberHider.Verify(a => a.ObscureCardNumber(payment.CardNumber), Times.Once);
        }


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

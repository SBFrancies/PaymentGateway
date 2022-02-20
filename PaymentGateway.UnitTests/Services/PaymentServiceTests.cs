using AutoFixture;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Moq;
using PaymentGateway.Api.Data.Entities;
using PaymentGateway.Api.Enums;
using PaymentGateway.Api.Interface;
using PaymentGateway.Api.Mapping;
using PaymentGateway.Api.Models.BankApi;
using PaymentGateway.Api.Models.Request;
using PaymentGateway.Api.Models.Response;
using PaymentGateway.Api.Services;
using PaymentGateway.Library.Interface;
using System;
using System.Linq;
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
        private readonly Mock<IMapper<CreatePaymentRequest, PaymentTable>> _mockToDbMapper = new Mock<IMapper<CreatePaymentRequest, PaymentTable>>();
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

        [Fact]
        public async Task PaymentService_FetchPaymentsAsync_ReturnsPayments()
        {
            var now = DateTimeOffset.UtcNow;
            _mockClock.SetupGet(a => a.UtcNow).Returns(now);
            var payments = _fixture.CreateMany<PaymentTable>(15);
            var request = _fixture.Create<FetchPaymentsRequest>();

            _mockCardNumberHider.Setup(a => a.ObscureCardNumber(It.IsAny<string>())).Returns("XXX");
            _mockToDetailsMapper.Setup(a => a.Map(payments)).Returns(() => new PaymentTableToPaymentDetailsMapper(_mockCardNumberHider.Object).Map(payments));
            _mockPaymentDataAccess.Setup(a => a.GetPaymentsAsync(request.CardNumber, request.Reference)).ReturnsAsync(payments);
            var sut = GetSystemUnderTest();

            var result = await sut.FetchPaymentsAsync(request);

            Assert.NotNull(result);
            Assert.Equal(now, result.ResponseTime);
            Assert.Equal(payments.Count(), result.Payments.Count());

            for (var i = 0; i < payments.Count(); i++)
            {
                var payment = payments.ElementAt(i);
                var resultPayment = result.Payments.ElementAt(i);

                Assert.Equal(payment.Reference, resultPayment.Reference);
                Assert.Equal(payment.Amount, resultPayment.Amount);
                Assert.Equal(payment.CurrencyCode, resultPayment.CurrencyCode);
                Assert.Equal(payment.Id, resultPayment.Id);
                Assert.Equal(payment.ExpiryYear, resultPayment.ExpiryYear);
                Assert.Equal(payment.ExpiryMonth, resultPayment.ExpiryMonth);
                Assert.Equal(payment.CardName, resultPayment.CardName);
                Assert.Equal("XXX", resultPayment.CardNumber);
                Assert.Equal(payment.BankCode, resultPayment.BankCode);
            }

            _mockPaymentDataAccess.Verify(a => a.GetPaymentsAsync(request.CardNumber, request.Reference), Times.Once);
            _mockToDetailsMapper.Verify(a => a.Map(payments), Times.Once);
            _mockCardNumberHider.Verify(a => a.ObscureCardNumber(It.IsAny<string>()), Times.AtLeast(15));
        }


        [Fact]
        public async Task PaymentService_CreatePaymentAsync_BankUnhealthyReturnsError()
        {
            var request = _fixture.Create<CreatePaymentRequest>();
            _mockBankStatusHolder.SetupGet(a => a.IsHealthy).Returns(false);

            var sut = GetSystemUnderTest();

            var result = await sut.CreatePaymentAsync(request);

            Assert.NotNull(result);
            Assert.Equal("Bank not available, please try again later", result.Message);
            Assert.False(result.Success);
            Assert.Equal(request, result.Request);

            _mockBankStatusHolder.Verify(a => a.IsHealthy, Times.Once);
            _mockIdGenerator.Verify(a => a.GenerateIdentifier(), Times.Never);
            _mockToDbMapper.Verify(a => a.Map(request), Times.Never);
            _mockPaymentDataAccess.Verify(a => a.SaveNewPaymentAsync(It.IsAny<PaymentTable>()), Times.Never);
            _mockEventStore.Verify(a => a.CreatePaymentEventAsync(It.IsAny<Guid>(), PaymentStatus.Uploaded), Times.Never);
            _mockValidator.Verify(a => a.Validate(request), Times.Never);
            _mockEventStore.Verify(a => a.CreatePaymentEventAsync(It.IsAny<Guid>(), PaymentStatus.FailedValidation), Times.Never);
            _mockToBankApiMapper.Verify(a => a.Map(request), Times.Never);
            _mockEventStore.Verify(a => a.CreatePaymentEventAsync(It.IsAny<Guid>(), PaymentStatus.Processing), Times.Never);
            _mockBankApi.Verify(a => a.MakeBankPaymentAsync(It.IsAny<CardPaymentRequest>()), Times.Never);
            _mockEventStore.Verify(a => a.CreatePaymentEventAsync(It.IsAny<Guid>(), PaymentStatus.Success), Times.Never);
            _mockPaymentDataAccess.Verify(a => a.UpdatePaymentAsync(It.IsAny<Guid>(), It.IsAny<string>()), Times.Never);
            _mockEventStore.Verify(a => a.CreatePaymentEventAsync(It.IsAny<Guid>(), PaymentStatus.FailedAtBank), Times.Never);
        }

        [Fact]
        public async Task PaymentService_CreatePaymentAsync_InvalidPaymentThrowsError()
        {
            var id = Guid.NewGuid();
            _mockIdGenerator.Setup(a => a.GenerateIdentifier()).Returns(id);

            var request = _fixture.Create<CreatePaymentRequest>();
            _mockBankStatusHolder.SetupGet(a => a.IsHealthy).Returns(true);

            var validationResult = new ValidationResult(_fixture.CreateMany<ValidationFailure>(10));
            _mockValidator.Setup(a => a.Validate(request)).Returns(validationResult);

            var entity = _fixture.Build<PaymentTable>().With(a => a.Id, id).Create();
            _mockToDbMapper.Setup(a => a.Map(request)).Returns(entity);

            var sut = GetSystemUnderTest();

            await Assert.ThrowsAsync<ValidationException>(async () => await sut.CreatePaymentAsync(request));

            _mockBankStatusHolder.Verify(a => a.IsHealthy, Times.Once);
            _mockIdGenerator.Verify(a => a.GenerateIdentifier(), Times.Once);
            _mockToDbMapper.Verify(a => a.Map(request), Times.Once);
            _mockPaymentDataAccess.Verify(a => a.SaveNewPaymentAsync(It.IsAny<PaymentTable>()), Times.Once);
            _mockEventStore.Verify(a => a.CreatePaymentEventAsync(id, PaymentStatus.Uploaded), Times.Once);
            _mockValidator.Verify(a => a.Validate(request), Times.Once);
            _mockEventStore.Verify(a => a.CreatePaymentEventAsync(id, PaymentStatus.FailedValidation), Times.Once);
            _mockToBankApiMapper.Verify(a => a.Map(request), Times.Never);
            _mockEventStore.Verify(a => a.CreatePaymentEventAsync(id, PaymentStatus.Processing), Times.Never);
            _mockBankApi.Verify(a => a.MakeBankPaymentAsync(It.IsAny<CardPaymentRequest>()), Times.Never);
            _mockEventStore.Verify(a => a.CreatePaymentEventAsync(id, PaymentStatus.Success), Times.Never);
            _mockPaymentDataAccess.Verify(a => a.UpdatePaymentAsync(id, It.IsAny<string>()), Times.Never);
            _mockEventStore.Verify(a => a.CreatePaymentEventAsync(id, PaymentStatus.FailedAtBank), Times.Never);
        }

        [Fact]
        public async Task PaymentService_CreatePaymentAsync_ValidRequestCallBankApiFails()
        {
            var id = Guid.NewGuid();
            _mockIdGenerator.Setup(a => a.GenerateIdentifier()).Returns(id);

            var request = _fixture.Create<CreatePaymentRequest>();
            _mockBankStatusHolder.SetupGet(a => a.IsHealthy).Returns(true);

            var validationResult = new ValidationResult();
            _mockValidator.Setup(a => a.Validate(request)).Returns(validationResult);

            var entity = _fixture.Build<PaymentTable>().With(a => a.Id, id).Create();
            _mockToDbMapper.Setup(a => a.Map(request)).Returns(entity);

            var apiRequest = _fixture.Create<CardPaymentRequest>();
            _mockToBankApiMapper.Setup(a => a.Map(request)).Returns(apiRequest);

            _mockBankApi.Setup(a => a.MakeBankPaymentAsync(apiRequest)).ReturnsAsync((CardPaymentResponse)null);

            var sut = GetSystemUnderTest();

            var result = await sut.CreatePaymentAsync(request);

            Assert.False(result.Success);
            Assert.Equal(DateTimeOffset.MinValue, result.DateProcessed);
            Assert.Equal("Bank payment failed, please try again later", result.Message);
            Assert.Equal(id, result.PaymentId);
            Assert.Equal(request, result.Request);

            _mockBankStatusHolder.Verify(a => a.IsHealthy, Times.Once);
            _mockIdGenerator.Verify(a => a.GenerateIdentifier(), Times.Once);
            _mockToDbMapper.Verify(a => a.Map(request), Times.Once);
            _mockPaymentDataAccess.Verify(a => a.SaveNewPaymentAsync(It.IsAny<PaymentTable>()), Times.Once);
            _mockEventStore.Verify(a => a.CreatePaymentEventAsync(id, PaymentStatus.Uploaded), Times.Once);
            _mockValidator.Verify(a => a.Validate(request), Times.Once);
            _mockEventStore.Verify(a => a.CreatePaymentEventAsync(id, PaymentStatus.FailedValidation), Times.Never);
            _mockToBankApiMapper.Verify(a => a.Map(request), Times.Once);
            _mockEventStore.Verify(a => a.CreatePaymentEventAsync(id, PaymentStatus.Processing), Times.Once);
            _mockBankApi.Verify(a => a.MakeBankPaymentAsync(It.IsAny<CardPaymentRequest>()), Times.Once);
            _mockEventStore.Verify(a => a.CreatePaymentEventAsync(id, PaymentStatus.Success), Times.Never);
            _mockPaymentDataAccess.Verify(a => a.UpdatePaymentAsync(id, It.IsAny<string>()), Times.Never);
            _mockEventStore.Verify(a => a.CreatePaymentEventAsync(id, PaymentStatus.FailedAtBank), Times.Once);
        }

        [Fact]
        public async Task PaymentService_CreatePaymentAsync_ValidRequestCallApiAndFailure()
        {
            var id = Guid.NewGuid();
            _mockIdGenerator.Setup(a => a.GenerateIdentifier()).Returns(id);

            var request = _fixture.Create<CreatePaymentRequest>();
            _mockBankStatusHolder.SetupGet(a => a.IsHealthy).Returns(true);

            var validationResult = new ValidationResult();
            _mockValidator.Setup(a => a.Validate(request)).Returns(validationResult);

            var entity = _fixture.Build<PaymentTable>().With(a => a.Id, id).Create();
            _mockToDbMapper.Setup(a => a.Map(request)).Returns(entity);

            var apiRequest = _fixture.Create<CardPaymentRequest>();
            _mockToBankApiMapper.Setup(a => a.Map(request)).Returns(apiRequest);

            var apiResult = _fixture.Build<CardPaymentResponse>().With(a => a.Success, false).Create();

            _mockBankApi.Setup(a => a.MakeBankPaymentAsync(apiRequest)).ReturnsAsync(apiResult);

            var sut = GetSystemUnderTest();

            var result = await sut.CreatePaymentAsync(request);

            Assert.Equal(apiResult.Success, result.Success); 
            Assert.Equal(apiResult.DateProcessed ?? DateTime.MinValue, result.DateProcessed);
            Assert.Equal(apiResult.Message, result.Message);
            Assert.Equal(id, result.PaymentId);
            Assert.Equal(request, result.Request);

            _mockBankStatusHolder.Verify(a => a.IsHealthy, Times.Once);
            _mockIdGenerator.Verify(a => a.GenerateIdentifier(), Times.Once);
            _mockToDbMapper.Verify(a => a.Map(request), Times.Once);
            _mockPaymentDataAccess.Verify(a => a.SaveNewPaymentAsync(It.IsAny<PaymentTable>()), Times.Once);
            _mockEventStore.Verify(a => a.CreatePaymentEventAsync(id, PaymentStatus.Uploaded), Times.Once);
            _mockValidator.Verify(a => a.Validate(request), Times.Once);
            _mockEventStore.Verify(a => a.CreatePaymentEventAsync(id, PaymentStatus.FailedValidation), Times.Never);
            _mockToBankApiMapper.Verify(a => a.Map(request), Times.Once);
            _mockEventStore.Verify(a => a.CreatePaymentEventAsync(id, PaymentStatus.Processing), Times.Once);
            _mockBankApi.Verify(a => a.MakeBankPaymentAsync(It.IsAny<CardPaymentRequest>()), Times.Once);
            _mockEventStore.Verify(a => a.CreatePaymentEventAsync(id, PaymentStatus.Success), Times.Never);
            _mockPaymentDataAccess.Verify(a => a.UpdatePaymentAsync(id, It.IsAny<string>()), Times.Never);
            _mockEventStore.Verify(a => a.CreatePaymentEventAsync(id, PaymentStatus.FailedAtBank), Times.Once);
        }


        [Fact]
        public async Task PaymentService_CreatePaymentAsync_ValidRequestCallApiAndSuccess()
        {
            var id = Guid.NewGuid();
            _mockIdGenerator.Setup(a => a.GenerateIdentifier()).Returns(id);

            var request = _fixture.Create<CreatePaymentRequest>();
            _mockBankStatusHolder.SetupGet(a => a.IsHealthy).Returns(true);

            var validationResult = new ValidationResult();
            _mockValidator.Setup(a => a.Validate(request)).Returns(validationResult);

            var entity = _fixture.Build<PaymentTable>().With(a => a.Id, id).Create();
            _mockToDbMapper.Setup(a => a.Map(request)).Returns(entity);

            var apiRequest = _fixture.Create<CardPaymentRequest>();
            _mockToBankApiMapper.Setup(a => a.Map(request)).Returns(apiRequest);

            var apiResult = _fixture.Build<CardPaymentResponse>().With(a => a.Success, true).Create();

            _mockBankApi.Setup(a => a.MakeBankPaymentAsync(apiRequest)).ReturnsAsync(apiResult);

            var sut = GetSystemUnderTest();

            var result = await sut.CreatePaymentAsync(request);

            Assert.Equal(apiResult.Success, result.Success);
            Assert.Equal(apiResult.DateProcessed ?? DateTime.MinValue, result.DateProcessed);
            Assert.Equal(apiResult.Message, result.Message);
            Assert.Equal(id, result.PaymentId);
            Assert.Equal(request, result.Request);

            _mockBankStatusHolder.Verify(a => a.IsHealthy, Times.Once);
            _mockIdGenerator.Verify(a => a.GenerateIdentifier(), Times.Once);
            _mockToDbMapper.Verify(a => a.Map(request), Times.Once);
            _mockPaymentDataAccess.Verify(a => a.SaveNewPaymentAsync(It.IsAny<PaymentTable>()), Times.Once);
            _mockEventStore.Verify(a => a.CreatePaymentEventAsync(id, PaymentStatus.Uploaded), Times.Once);
            _mockValidator.Verify(a => a.Validate(request), Times.Once);
            _mockEventStore.Verify(a => a.CreatePaymentEventAsync(id, PaymentStatus.FailedValidation), Times.Never);
            _mockToBankApiMapper.Verify(a => a.Map(request), Times.Once);
            _mockEventStore.Verify(a => a.CreatePaymentEventAsync(id, PaymentStatus.Processing), Times.Once);
            _mockBankApi.Verify(a => a.MakeBankPaymentAsync(It.IsAny<CardPaymentRequest>()), Times.Once);
            _mockEventStore.Verify(a => a.CreatePaymentEventAsync(id, PaymentStatus.Success), Times.Once);
            _mockPaymentDataAccess.Verify(a => a.UpdatePaymentAsync(id, apiResult.Message), Times.Once);
            _mockEventStore.Verify(a => a.CreatePaymentEventAsync(id, PaymentStatus.FailedAtBank), Times.Never);
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
                _mockToDbMapper.Object,
                _mockBankStatusHolder.Object,
                _mockToDetailsMapper.Object,
                _mockClock.Object
                );
        }
    }
}

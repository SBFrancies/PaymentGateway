using FluentValidation;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using PaymentGateway.Api.Data.Entities;
using PaymentGateway.Api.Enums;
using PaymentGateway.Api.Interface;
using PaymentGateway.Api.Models.BankApi;
using PaymentGateway.Api.Models.Request;
using PaymentGateway.Api.Models.Response;
using PaymentGateway.Library.Interface;
using System;
using System.Threading.Tasks;

namespace PaymentGateway.Api.Services
{
    public class PaymentService : IPaymentService
    {
        private IEventStore EventStore { get; }
        private IPaymentDataAccess PaymentDataAccess { get;  }
        private IBankApi BankApi { get; }
        private ILogger<PaymentService> Logger { get;  }
        private IValidator<CreatePaymentRequest> Validator { get; }
        private IMapper<CreatePaymentRequest, CardPaymentRequest> ToBankApiMapper { get; }
        private IIdentifierGenerator IdGenerator { get; }
        private IMapper<CreatePaymentRequest, PaymentTable> ToDbMaper { get; }
        private IBankStatusHolder BankStatusHolder { get; }
        private IMapper<PaymentTable, PaymentDetails> ToDetailsMapper { get; }
        private ISystemClock Clock { get; }

        public PaymentService(
            ILogger<PaymentService> logger,
            IBankApi bankApi,
            IPaymentDataAccess paymentDataAccess,
            IEventStore eventStore,
            IValidator<CreatePaymentRequest> validator,
            IMapper<CreatePaymentRequest, CardPaymentRequest> toBankApiMapper,
            IIdentifierGenerator idGenerator,
            IMapper<CreatePaymentRequest, PaymentTable> toDbMaper,
            IBankStatusHolder bankStatusHolder,
            IMapper<PaymentTable, PaymentDetails> toDetailsMapper,
            ISystemClock clock)
        {
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            BankApi = bankApi ?? throw new ArgumentNullException(nameof(bankApi));
            PaymentDataAccess = paymentDataAccess ?? throw new ArgumentNullException(nameof(paymentDataAccess));
            EventStore = eventStore ?? throw new ArgumentNullException(nameof(eventStore));
            Validator = validator ?? throw new ArgumentNullException(nameof(validator));
            ToBankApiMapper = toBankApiMapper ?? throw new ArgumentNullException(nameof(toBankApiMapper));
            IdGenerator = idGenerator ?? throw new ArgumentNullException(nameof(idGenerator));
            ToDbMaper = toDbMaper ?? throw new ArgumentNullException(nameof(toDbMaper));
            BankStatusHolder = bankStatusHolder ?? throw new ArgumentNullException(nameof(bankStatusHolder));
            ToDetailsMapper = toDetailsMapper ?? throw new ArgumentNullException(nameof(toDetailsMapper));
            Clock = clock ?? throw new ArgumentNullException(nameof(clock));
        }

        public async Task<CreatePaymentResponse> CreatePaymentAsync(CreatePaymentRequest request)
        {
            if(!BankStatusHolder.IsHealthy)
            {
                Logger.LogWarning("Bank API unhealthy, request could not be processed");

                return new CreatePaymentResponse
                {
                    Message = "Bank not available, please try again later",
                    Success = false,
                    Request = request,
                };
            }

            var id = IdGenerator.GenerateIdentifier();
            Logger.LogInformation($"Create payment request triggered with ID {id}");

            var entity = ToDbMaper.Map(request);
            entity.Id = id;

            _ = await PaymentDataAccess.SaveNewPaymentAsync(entity);
            await EventStore.CreatePaymentEventAsync(id, PaymentStatus.Uploaded);

            var valid = Validator.Validate(request);

            if(!valid.IsValid)
            {
                await EventStore.CreatePaymentEventAsync(id, PaymentStatus.FailedValidation);
                throw new ValidationException(valid.Errors);
            }

            var bankApiRequest = ToBankApiMapper.Map(request);

            await EventStore.CreatePaymentEventAsync(id, PaymentStatus.Processing);
            var apiResult = await BankApi.MakeBankPaymentAsync(bankApiRequest);

            if(apiResult == null)
            {
                await EventStore.CreatePaymentEventAsync(id, PaymentStatus.FailedAtBank);

                return new CreatePaymentResponse
                {
                    PaymentId = id,
                    Message = "Bank payment failed, please try again later",
                    Success = false,
                    Request = request,
                };
            }

            if(apiResult.Success)
            {
                await EventStore.CreatePaymentEventAsync(id, PaymentStatus.Success);
                await PaymentDataAccess.UpdatePaymentAsync(id, apiResult.Message);
            }
            else
            {
                Logger.LogWarning($"Bank payment failed for payment ID {id}");
                await EventStore.CreatePaymentEventAsync(id, PaymentStatus.FailedAtBank);
            }

            return new CreatePaymentResponse
            {
                DateProcessed = apiResult.DateProcessed ?? DateTimeOffset.MinValue,
                Message = apiResult.Message,
                Success = apiResult.Success,
                PaymentId = id,
                Request = request,
            };
        }

        public async Task<FetchPaymentsResponse> FetchPaymentsAsync(FetchPaymentsRequest request)
        {
            var payments = await PaymentDataAccess.GetPaymentsAsync(request.CardNumber, request.Reference);

            var details = ToDetailsMapper.Map(payments);

            return new FetchPaymentsResponse
            {
                Payments = details,
                ResponseTime = Clock.UtcNow,
            };
        }

        public async Task<PaymentResponse> FetchPaymentAsync(Guid id)
        {
            var payment = await PaymentDataAccess.GetPaymentAsync(id);

            if(payment == null)
            {
                return null;
            }

            var details = ToDetailsMapper.Map(payment);

            var events = await EventStore.GetPaymentEventsAsync(id);

            return new PaymentResponse
            {
                Events = events,
                Payment = details,
                ResponseTime = Clock.UtcNow,
            };
        }
    }
}

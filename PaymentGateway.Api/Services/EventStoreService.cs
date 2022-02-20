using Microsoft.AspNetCore.Authentication;
using PaymentGateway.Api.Enums;
using PaymentGateway.Api.Interface;
using PaymentGateway.Api.Models.Response;
using PaymentGateway.Api.Models.Settings;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PaymentGateway.Api.Services
{
    public class EventStoreService : IEventStore
    {
        private ConnectionMultiplexer RedisConnection { get; }
        private ISystemClock Clock { get; }

        public EventStoreService(PaymentGatewaySettings settings, ISystemClock clock)
        {
            RedisConnection = ConnectionMultiplexer.Connect(settings?.EventStoreConnectionString?? throw new ArgumentNullException(nameof(settings)));
            Clock = clock ?? throw new ArgumentNullException(nameof(clock));
        }

        public async Task CreatePaymentEventAsync(Guid id, PaymentStatus key)
        {
            var db = RedisConnection.GetDatabase();
            _ = await db.StreamAddAsync(id.ToString(), key.ToString(), Clock.UtcNow.ToString("yyyyMMdd HH:mm:ss"));
        }

        public async Task<IEnumerable<PaymentEvent>> GetPaymentEventsAsync(Guid id)
        {
            var db = RedisConnection.GetDatabase();

            var events = await db.StreamReadAsync(id.ToString(), 0);

            var e = events.SelectMany(a => a.Values).Where(a => a.Value.HasValue).Select(a => new PaymentEvent
            {
                Id = id,
                Status = Enum.Parse<PaymentStatus>(a.Name, true),
                TimeStamp = DateTimeOffset.ParseExact(a.Value, "yyyyMMdd HH:mm:ss", null),
            }).OrderBy(a => a.TimeStamp);

            return e;
        }
    }
}

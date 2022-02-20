using PaymentGateway.Api.Enums;
using PaymentGateway.Api.Interface;
using PaymentGateway.Api.Models.Response;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PaymentGateway.IntegrationTests.FakeServices
{
    public class FakeEventStoreService : IEventStore
    {
        public readonly IDictionary<Guid, ConcurrentBag<PaymentEvent>> _eventStore = new ConcurrentDictionary<Guid, ConcurrentBag<PaymentEvent>>();  

        public Task CreatePaymentEventAsync(Guid id, PaymentStatus key)
        {
            if(!_eventStore.ContainsKey(id))
            {
                _eventStore.Add(id, new ConcurrentBag<PaymentEvent>());
            }

            _eventStore[id].Add(new PaymentEvent
            {
                Id = id,
                Status = key,
                TimeStamp = DateTimeOffset.UtcNow,
            });

            return Task.CompletedTask;
        }

        public Task<IEnumerable<PaymentEvent>> GetPaymentEventsAsync(Guid id)
        {
            if (_eventStore.TryGetValue(id, out var events))
            {
                return Task.FromResult(events.AsEnumerable());
            }

            return null;
        }
    }
}

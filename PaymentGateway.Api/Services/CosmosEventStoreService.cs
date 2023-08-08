using Microsoft.AspNetCore.Authentication;
using Microsoft.Azure.Cosmos;
using PaymentGateway.Api.Enums;
using PaymentGateway.Api.Interface;
using PaymentGateway.Api.Models.Response;
using PaymentGateway.Api.Models.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PaymentGateway.Api.Services
{
    public class CosmosEventStoreService : IEventStore
    {
        private const string DatabaseName = "PaymentGatewayEventStore";
        private const string ContainerName = "Events";

        private string ConnectionString { get; }
        private ISystemClock Clock { get; }

        public CosmosEventStoreService(PaymentGatewaySettings settings, ISystemClock clock)
        {
            ConnectionString = settings?.EventStoreConnectionString ?? throw new ArgumentNullException(nameof(settings));
            Clock = clock ?? throw new ArgumentNullException(nameof(clock));
        }

        public async Task CreatePaymentEventAsync(Guid id, PaymentStatus status)
        {
            using var cosmosClient = new CosmosClient(ConnectionString);
            var database = await cosmosClient.CreateDatabaseIfNotExistsAsync(DatabaseName);
            var container = await database.Database.CreateContainerIfNotExistsAsync(new ContainerProperties(ContainerName, "/Id"));
            var paymentEvent = new PaymentEvent
            {
                Id = id,
                Status = status,
                TimeStamp = Clock.UtcNow,
            };

            _ = await container.Container.CreateItemAsync(paymentEvent, new PartitionKey(id.ToString()));
        }

        public async Task<IEnumerable<PaymentEvent>> GetPaymentEventsAsync(Guid id)
        {
            using var cosmosClient = new CosmosClient(ConnectionString);
            var database = await cosmosClient.CreateDatabaseIfNotExistsAsync(DatabaseName);
            var container = await database.Database.CreateContainerIfNotExistsAsync(new ContainerProperties(ContainerName, "/Id"));

            var query = new QueryDefinition($"SELECT * FROM {ContainerName} c WHERE c.Id = @ID")
                            .WithParameter("@ID", id);

            using var feed = container.Container.GetItemQueryIterator<PaymentEvent>(query);

            var events = new List<PaymentEvent>();

            while (feed.HasMoreResults)
            {
                var response = await feed.ReadNextAsync();
                
                foreach (var item in response)
                {
                    events.Add(item);
                }
            }

            return events.OrderBy(a => a.TimeStamp);
        }
    }
}

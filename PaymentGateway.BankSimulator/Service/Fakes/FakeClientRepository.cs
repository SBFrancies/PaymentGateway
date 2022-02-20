using PaymentGateway.BankSimulator.Interface;
using PaymentGateway.BankSimulator.Models;
using PaymentGateway.BankSimulator.Models.Settings;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace PaymentGateway.BankSimulator.Service.Fakes
{
    public class FakeClientRepository : IClientRepository
    {
        private IFileService FileService { get; }
        private string FilePath { get; }

        public FakeClientRepository(IFileService fileService, BankSimulatorSettings settings)
        {
            FileService = fileService ?? throw new ArgumentNullException(nameof(fileService));
            FilePath = settings?.ClientRepositoryFilePath ?? throw new ArgumentNullException(nameof(settings));
        }

        public async Task<BankClient> GetClientAsync(string apiKey)
        {
            var lines =  await FileService.ReadAllLinesAsync(FilePath);
            var match = lines.FirstOrDefault(a => a.Equals(apiKey));
            
            if(match == null)
            {
                return null;
            }

            return new BankClient { ApiKey = match };
        }
    }
}

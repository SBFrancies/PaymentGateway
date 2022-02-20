using Microsoft.Extensions.Hosting;
using PaymentGateway.Api.Interface;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace PaymentGateway.Api.Jobs
{
    public class BankHealthCheckJob : IHostedService
    {
        private IBankApi BankApi { get; }
        private IBankStatusHolder BankStatusHolder { get; }
        private System.Timers.Timer StandardTimer { get; }
        private System.Timers.Timer DownTimer { get; }

        public BankHealthCheckJob(IBankApi bankApi, IBankStatusHolder bankStatusHolder)
        {
            BankApi = bankApi ?? throw new ArgumentNullException(nameof(bankApi));
            BankStatusHolder = bankStatusHolder ?? throw new ArgumentNullException(nameof(bankStatusHolder));
            StandardTimer = new System.Timers.Timer(30000);
            DownTimer = new System.Timers.Timer(5000);
            StandardTimer.Elapsed += CheckBankHealth;
            DownTimer.Elapsed += CheckBankHealth;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            CheckBankHealth(null,null);
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            StandardTimer?.Dispose();
            DownTimer?.Dispose();

            return Task.CompletedTask;
        }

        private async void CheckBankHealth(object sender, ElapsedEventArgs e)
        {
            var currentStatus = BankStatusHolder.IsHealthy;
            var isHealthy = await BankApi.CheckBankHealthyAsync();

            if (isHealthy != currentStatus || sender == null)
            {
                if (!isHealthy)
                {
                    StandardTimer.Stop();
                    DownTimer.Start();
                }

                else
                {
                    DownTimer.Stop();
                    StandardTimer.Start();
                }

                BankStatusHolder.SetIsHealthy(isHealthy);
            }
        }
    }
}

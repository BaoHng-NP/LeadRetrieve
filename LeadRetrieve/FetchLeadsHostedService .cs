using LeadRetrieve.Controllers;
using Microsoft.Extensions.Hosting;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LeadRetrieve
{

    public class FetchLeadsHostedService : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;

        public FetchLeadsHostedService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var leadController = scope.ServiceProvider.GetRequiredService<LeadController>();

                // Gọi API FetchLeads
                await leadController.FetchLeads();
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}

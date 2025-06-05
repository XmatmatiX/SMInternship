using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SMInternship.Domain.Models;
using SMInternship.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMInternship.Application.Services
{
    public class ExpirationService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;

        public ExpirationService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        /// <summary>
        /// Loop that will check expiration of negotiations everyday at 2.00 AM
        /// </summary>
        /// <param name="stoppingToken"></param>
        /// <returns></returns>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var now = DateTime.Now;

                var nextRun = new DateTime(now.Year, now.Month, now.Day, 2, 0, 0);
                if (now > nextRun)
                    nextRun = nextRun.AddDays(1);
                var delay = nextRun - now;
                await Task.Delay(delay, stoppingToken);

                await ExpireOldNegotiationsAsync();
            }
        }

        /// <summary>
        /// Searching for expired negotiations and changing their status
        /// </summary>
        /// <returns></returns>
        private async Task ExpireOldNegotiationsAsync()
        {
            using var scope = _serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<Context>();

            var now = DateTime.UtcNow;
            var expiredNegotiations = dbContext.Negotiations
                .Where(n => n.Status == NegotiationStatus.Rejected&& n.LastAttemp.AddDays(7) < now)
                .ToList();

            foreach (var negotiation in expiredNegotiations)
            {
                negotiation.Status = NegotiationStatus.Canceled;
            }

            await dbContext.SaveChangesAsync();
        }
    }
}

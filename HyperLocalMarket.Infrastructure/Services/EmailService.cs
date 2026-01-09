using HyperLocalMarket.Application.Common.Interfaces;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace HyperLocalMarket.Infrastructure.Services
{
    public sealed class EmailService : IEmailService
    {
        private readonly ILogger<EmailService> _logger;

        public EmailService(ILogger<EmailService> logger)
        {
            _logger = logger;
        }

        public Task SendAsync(string to, string subject, string body, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Email sent. To={To}, Subject={Subject}, Body={Body}", to, subject, body);
            return Task.CompletedTask;
        }
    }
}

using Customer.Consumer.Messaging;
using MediatR;

namespace Customer.Consumer.Handlers
{
    public class CustomerUpdatedHandler : IRequestHandler<CustomerUpdated>
    {
        private readonly ILogger<CustomerUpdatedHandler> _logger;

        public CustomerUpdatedHandler(ILogger<CustomerUpdatedHandler> logger)
        {
            _logger = logger;
        }

        public Task Handle(CustomerUpdated request, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Customer Updated: {request.FullName}, Email: {request.Email}");
            return Task.CompletedTask;
        }
    }
}

using Customer.Consumer.Messaging;
using MediatR;

namespace Customer.Consumer.Handlers
{
    public class CustomerDeletedHandler : IRequestHandler<CustomerDeleted>
    {
        private readonly ILogger<CustomerDeletedHandler> _logger;

        public CustomerDeletedHandler(ILogger<CustomerDeletedHandler> logger)
        {
            _logger = logger;
        }

        public Task Handle(CustomerDeleted request, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Customer Delete: {request.Id}");
            return Task.CompletedTask;
        }
    }
}

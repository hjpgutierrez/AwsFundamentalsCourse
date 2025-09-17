using Customer.Consumer.Messaging;
using MediatR;

namespace Customer.Consumer.Handlers
{
    public class CustomerCreatedHandler : IRequestHandler<CustomerCreated>
    {
        private readonly ILogger<CustomerCreatedHandler> _logger;

        public CustomerCreatedHandler(ILogger<CustomerCreatedHandler> logger)
        {
            _logger = logger;
        }

        public Task Handle(CustomerCreated request, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Customer Created: {request.FullName}, Email: {request.Email}");
            return Task.CompletedTask;
        }
    }
}

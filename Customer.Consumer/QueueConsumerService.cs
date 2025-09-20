
using Amazon.SQS;
using Amazon.SQS.Model;
using Customer.Consumer.Messaging;
using Customers.Consumer;
using MediatR;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace Customer.Consumer;
public class QueueConsumerService : BackgroundService
{
    private readonly IAmazonSQS _sqs;
    private readonly IOptions<QueueSettings> _queueSettings;
    private readonly IMediator _mediator;
    private readonly ILogger<QueueConsumerService> _logger;

    public QueueConsumerService(
                            IAmazonSQS sqs,
                            IOptions<QueueSettings> queueSettings,
                            IMediator mediator,
                            ILogger<QueueConsumerService> logger)
    {
        _sqs = sqs;
        _queueSettings = queueSettings;
        _mediator = mediator;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var queueUrlResponse = await _sqs.GetQueueUrlAsync(_queueSettings.Value.Name);

        var receiveMessageRequest = new ReceiveMessageRequest
        {
            QueueUrl = queueUrlResponse.QueueUrl,
            MessageAttributeNames = new List<string> { "All" },
            MessageSystemAttributeNames = new List<string> { "All" },
            MaxNumberOfMessages = 1,
        };

        while (!stoppingToken.IsCancellationRequested)
        {
            var response = await _sqs.ReceiveMessageAsync(receiveMessageRequest, stoppingToken);
            if (response?.Messages is null || response?.Messages?.Count == 0)
            {
                await Task.Delay(1000, stoppingToken);
                continue;
            }

            foreach (var message in response.Messages)
            {
                var messageType = message.MessageAttributes["MessageType"].StringValue;
                var type = Type.GetType($"Customer.Consumer.Messaging.{messageType}");
                if (type == null)
                {
                    _logger.LogWarning("Unknown message type: {MessageType}", messageType);
                    continue;
                }

                var typedMessage = (ISqsMessage)JsonSerializer.Deserialize(message.Body, type)!;

                try
                {
                    await _mediator.Send(typedMessage, stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing message of type {MessageType}", messageType);
                    // Optionally, you might want to handle the error (e.g., move the message to a dead-letter queue)
                    continue; // Skip deleting the message so it can be retried
                }

                await _sqs.DeleteMessageAsync(queueUrlResponse.QueueUrl, message.ReceiptHandle, stoppingToken);
            }

            await Task.Delay(3000, stoppingToken);
        }
    }
}


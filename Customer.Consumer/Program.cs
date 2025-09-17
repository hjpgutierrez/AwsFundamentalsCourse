using Amazon;
using Amazon.SQS;
using Customers.Consumer;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Customer.Consumer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.Configure<QueueSettings>(builder.Configuration.GetSection(QueueSettings.Key));
            builder.Services.AddSingleton<IAmazonSQS>(_ => new AmazonSQSClient(RegionEndpoint.USEast2));
            builder.Services.AddHostedService<QueueConsumerService>();

            builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

            var app = builder.Build();

            app.Run();
        }
    }
}

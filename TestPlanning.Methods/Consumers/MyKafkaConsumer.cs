using System;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Methods.Application.IServices;
using Methods.Core.Messages.Commands;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Methods.API.Consumers
{
    public class MyKafkaConsumer : BackgroundService
    {
        ConsumerConfig _consumerConfig;
        private readonly IConfiguration Configuration;
        private readonly IMethodCommandHandlers _methodCommandHandler;
        private readonly string METHODS_TOPIC;


        public MyKafkaConsumer(ConsumerConfig consumerConfig, IMethodCommandHandlers methodCommandHandler, IConfiguration configuration)
        {
            _consumerConfig = new ConsumerConfig
            {
                GroupId = "methods-microservice-consumer",
                BootstrapServers = "localhost:9092",
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            _methodCommandHandler = methodCommandHandler;
            Configuration = configuration;
            METHODS_TOPIC = Configuration["MethodsTopic"];
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Task.Run(() => StartConsumer(stoppingToken));
            return Task.CompletedTask;
        }

        private void StartConsumer(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var c = new ConsumerBuilder<string, string>(_consumerConfig).Build())
                {
                    c.Subscribe(METHODS_TOPIC);

                    CancellationTokenSource cts = new CancellationTokenSource();
                    Console.CancelKeyPress += (_, e) => {
                        e.Cancel = true; // prevent the process from terminating.
                        cts.Cancel();
                    };

                    try
                    {
                        while (true)
                        {
                            Console.Write("Starting loop");
                            try
                            {
                                var cr = c.Consume(cts.Token);
                                var msg = cr.Message;
                                //var key = cr.Message.Key;
                                var val = cr.Message.Value;
                                Console.WriteLine($"Consumed message '{cr.Message.Value}' at: '{cr.TopicPartitionOffset}'.");

                                Command command = GetCommandType(val);
                                if (command != null)
                                    _methodCommandHandler.Handle(command);
                                //otherwise, it's not a command
                            }
                            catch (ConsumeException e)
                            {
                                Console.WriteLine($"Error occured: {e.Error.Reason}");
                            }
                        }
                    }
                    catch (OperationCanceledException)
                    {
                        // Ensure the consumer leaves the group cleanly and final offsets are committed.
                        c.Close();
                    }
                }
            }
        }

        private Command GetCommandType(string message)
        {
            JObject rss = JObject.Parse(message);
            string messageType = (string)rss["messageType"];
            string payload = ((JObject)rss["payload"]).ToString();

            return messageType switch
            {
                "CreateMethod" => JsonConvert.DeserializeObject<CreateMethod>(payload),
                "CreateMethods" => JsonConvert.DeserializeObject<CreateMethods>(payload),
                "DeleteMethod" => JsonConvert.DeserializeObject<DeleteMethod>(payload),
                "DeleteMethods" => JsonConvert.DeserializeObject<DeleteMethods>(payload),
                "UpdateMethod" => JsonConvert.DeserializeObject<UpdateMethod>(payload),
                "AddMethodsToExperiment" => JsonConvert.DeserializeObject<AddMethodsToExperiment>(payload),
                "RemoveMethodsFromExperiment" => JsonConvert.DeserializeObject<RemoveMethodsFromExperiment>(payload),
                _ => null,
            };
        }
    }

}

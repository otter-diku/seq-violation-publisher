using Confluent.Kafka;

namespace ViolationPublisher.Web;

public class ViolationPublisherBackgroundService : BackgroundService
{
    private readonly ILogger<ViolationPublisherBackgroundService> _logger;
    private readonly IConsumer<string,string> _kafkaConsumer;
    private readonly string _topic;

    public ViolationPublisherBackgroundService(IConfiguration configuration, ILogger<ViolationPublisherBackgroundService> logger)
    {
        try
        {
            _logger = logger;
            var consumerConfig = new ConsumerConfig();
            configuration.GetSection("Kafka:ConsumerSettings").Bind(consumerConfig);
            _kafkaConsumer = new ConsumerBuilder<string, string>(consumerConfig).Build();
            _topic = configuration.GetSection("Kafka:Topic").Value!;
        }
        catch (Exception e)
        {
            _logger.LogWarning(e, "Exception in bg service constructor");
            throw;
        }
    }
    
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogDebug("Starting to execute background service. Subscribing to topic: {Topic}", _topic);
        _kafkaConsumer.Subscribe(_topic);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var cr = _kafkaConsumer.Consume(stoppingToken);
                    _logger.LogInformation(
                        "[Local][{Topic}]:[{MessageKey}]:[{MessageContent}]", _topic, cr?.Message?.Key, cr?.Message?.Value);
                }
                catch (ConsumeException consumeException) when (consumeException.Error.IsFatal)
                {
                    _logger.LogCritical(consumeException, "Fatal consume exception");
                }
                catch (ConsumeException consumeException)
                {
                    _logger.LogInformation(consumeException, "Recoverable consume exception");
                }
                catch (OperationCanceledException)
                {
                    _logger.LogInformation("Cancelling the background service...");
                    break;
                }
                catch (Exception exception)
                {
                    _logger.LogWarning(exception, "Exception in the background service");
                    break;
                }
            }

            _logger.LogDebug("Closing the connection to kafka consumer");
            _kafkaConsumer.Close();
            return Task.CompletedTask;
    }
}
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

var builder = new ConfigurationBuilder();
builder.SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false);

IConfiguration config = builder.Build();
var connectionString = config["ServiceBusConnectionString"];

using var loggerFactory =
    LoggerFactory.Create(loggingBuilder =>
        loggingBuilder
            .AddSimpleConsole(options =>
        {
            options.IncludeScopes = true;
            options.SingleLine = true;
            options.TimestampFormat = "HH:mm:ss ";
        }));

var logger = loggerFactory.CreateLogger<Program>();

try
{
    logger.LogInformation("Starting...");
    
    var serviceBusClient = new ServiceBusClient(connectionString);
    logger.LogInformation("serviceBusClient created!");
    
    var sender = serviceBusClient.CreateSender("mandate-creation-events");
    logger.LogInformation("sender created!");
    
    var messages = 
        Directory
            .GetFiles("./MessagesToSend")
            .Select(filePath => new ServiceBusMessage(File.ReadAllBytes(filePath)));
    
    await sender.SendMessagesAsync(messages);
        
    logger.LogInformation("messages sent!");
}
catch (Exception ex)
{ 
    await File.WriteAllTextAsync("log.txt", ex.ToString());
    logger.LogError(ex, ex.ToString());
}

Console.WriteLine("Done!");
Console.ReadLine();
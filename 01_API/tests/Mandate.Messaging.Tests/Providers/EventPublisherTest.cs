// <copyright file="EventPublisherTest.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

using System.Text.Json;
using Azure.Messaging.ServiceBus;
using KPMG.Pulse.Back.Accounting.Mandate.Application;
using Mandate.Messaging.Models;
using Mandate.Messaging.Providers;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Options;

namespace Mandate.Messaging.Tests.Providers
{
    public class EventPublisherTest
    {
        [Fact]
        public async Task PublishToQueueAsync()
        {
            // Arrange
            var options = new ServiceBusOptions
            {
                ServiceBusCreateMandateQueueName = "b",
            };

            var @event = new MandateCreationMessage
            {
                Id = 2,
                SiretNumber = "121234",
            };
            var correlationId = "correlationId";

            var mockOptions = new Mock<IOptions<ServiceBusOptions>>();
            mockOptions.Setup(ap => ap.Value).Returns(options);

            var mockClient = new Mock<ServiceBusSender>();
            mockClient.Setup(x => x.SendMessageAsync(It.IsAny<ServiceBusMessage>(), It.IsAny<CancellationToken>()))
                .Callback<ServiceBusMessage, CancellationToken>((message, token) =>
                {
                    var msg = JsonSerializer.Deserialize<MandateCreationMessage>(message.Body);
                    msg.Should().BeEquivalentTo(@event);
                })
                .Returns(Task.CompletedTask);

            var mockClientFactory = new Mock<IAzureClientFactory<ServiceBusSender>>();
            mockClientFactory.Setup(x => x.CreateClient(options.ServiceBusCreateMandateQueueName))
                .Returns(mockClient.Object);

            var eventPublisher = new EventPublisher(mockOptions.Object, mockClientFactory.Object);

            // Act
            await eventPublisher.PublishToQueueAsync<MandateCreationMessage>(@event, correlationId);

            // Assert
            mockClientFactory.Verify(x => x.CreateClient(options.ServiceBusCreateMandateQueueName), Times.Once);
            mockClient.Verify(x => x.SendMessageAsync(It.IsAny<ServiceBusMessage>(), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}

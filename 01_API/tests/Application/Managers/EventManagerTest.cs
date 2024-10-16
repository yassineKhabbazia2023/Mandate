// <copyright file="EventManagerTest.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

using KPMG.Pulse.Back.Accounting.Mandate.Application.Interfaces;
using KPMG.Pulse.Back.Accounting.Mandate.Application.Managers;

namespace KPMG.Pulse.Back.Accounting.Mandate.Application.Tests.Managers
{
    public class EventManagerTest
    {
        [Fact]
        public async Task PublishCreateMandateAsync()
        {
            var @event = new MandateCreationMessage
            {
                Id = 2,
                SiretNumber = "121234",
            };
            var correlationId = "correlationId";

            var mockClient = new Mock<IEventPublisher>();
            mockClient.Setup(x => x.PublishToQueueAsync(@event, correlationId, It.IsAny<string?>()))
                .Returns(Task.CompletedTask);

            var eventManager = new EventManager(mockClient.Object);

            await eventManager.PublishCreateMandateAsync(@event, correlationId);

            mockClient.Verify(x => x.PublishToQueueAsync(@event, correlationId, It.IsAny<string?>()), Times.Once);
        }
    }
}

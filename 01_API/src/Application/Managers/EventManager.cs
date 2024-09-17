// <copyright file="EventManager.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

using KPMG.Pulse.Back.Accounting.Mandate.Application.Interfaces;

namespace KPMG.Pulse.Back.Accounting.Mandate.Application.Managers
{
    public class EventManager : IEventManager
    {
        private readonly IEventPublisher eventPublisher;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventManager"/> class.
        /// </summary>
        /// <param name="eventPublisher">eventPublisher.</param>
        public EventManager(IEventPublisher eventPublisher)
        {
            this.eventPublisher = eventPublisher;
        }

        /// <inheritdoc/>
        public async Task PublishCreateMandateAsync(MandateCreationMessage message, string? correlationId = null)
        {
            await this.eventPublisher.PublishToQueueAsync(message, correlationId);
        }
    }
}

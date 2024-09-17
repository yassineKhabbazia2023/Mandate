// <copyright file="IEventPublisher.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Application.Interfaces
{
    /// <summary>
    /// IEventPublisher.
    /// </summary>
    public interface IEventPublisher
    {
        /// <summary>
        /// Publishes a message to Azure Service Queue.
        /// </summary>
        /// <typeparam name="T">The type of the message to be published.</typeparam>
        /// <param name="message">The message to be published.</param>
        /// <param name="correlationId">The correlation ID for the message (optional).</param>
        /// <param name="queueName">The name of the queue to publish the message to (optional).</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException">Thrown if the <paramref name="baseEvent"/> is null.</exception>
        Task PublishToQueueAsync<T>(T message, string? correlationId = null, string? queueName = null);
    }
}
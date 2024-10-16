// <copyright file="ServiceBusOptions.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

using System.Diagnostics.CodeAnalysis;

namespace Mandate.Messaging.Models
{
    /// <summary>
    /// ServiceBusOptions.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class ServiceBusOptions
    {
        /// <summary>
        /// Gets or sets ServiceBusCreateMandateQueueName.
        /// </summary>
        public string ServiceBusCreateMandateQueueName { get; set; }
    }
}

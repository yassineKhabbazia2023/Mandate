// <copyright file="MandateCreationLogMessage.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate;

public class MandateCreationLogMessage
{
    public MandateCreationLogMessage(Guid collectionId, string message)
    {
        this.CollectionId = collectionId;
        this.MessageContent = message;
        this.CreatedDate = DateTime.Now;
    }

    public Guid Id { get; }

    public Guid CollectionId { get; }

    public string MessageContent { get; }

    public DateTime CreatedDate { get; }
}
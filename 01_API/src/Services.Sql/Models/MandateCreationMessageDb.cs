// <copyright file="MandateCreationLogMessageDb.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Sql;

public class MandateCreationLogMessageDb
{
    public Guid Id { get; set; }

    public Guid CollectionId { get; set; }

    public string MessageContent { get; set; }

    public DateTime CreatedDate { get; set; }
    
    public int? CreatedById { get; set; }
}
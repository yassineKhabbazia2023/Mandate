// <copyright file="MandateCreationOptions.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Application.BusinessHelpers;

/// <summary>
/// Configuration options for mandate creation.
/// </summary>
public class MandateCreationOptions
{
    /// <summary>
    /// List of destination tools available for mandate creation.
    /// </summary>
    public List<DestinationTool> ConfiguredDestinationTools { get; set; } = new List<DestinationTool>();
}

/// <summary>
/// Represents a destination tool.
/// </summary>
public class DestinationTool
{
    /// <summary>
    /// Name of the tool.
    /// </summary>
    public string ToolName { get; set; } = string.Empty;

    /// <summary>
    /// Identifier of the tool.
    /// </summary>
    public string ToolId { get; set; } = string.Empty;
}

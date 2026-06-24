// <copyright file="GetAcceptOptions.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.AspNetCore.GetAccept;

/// <summary>
/// Represents GetAccept API configuration.
/// </summary>
public sealed class GetAcceptOptions
{
    /// <summary>
    /// Gets or sets the GetAccept API base URL.
    /// </summary>
    public string BaseUrl { get; set; } = "https://api.getaccept.com/";

    /// <summary>
    /// Gets or sets the GetAccept account email.
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the GetAccept account password.
    /// </summary>
    public string Password { get; set; } = string.Empty;
}

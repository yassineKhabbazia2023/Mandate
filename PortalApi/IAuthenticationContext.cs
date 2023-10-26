// <copyright file="IAuthenticationContext.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.PortalApi
{
    public interface IAuthenticationContext
    {
        string BearerToken { get; }
    }
}
// <copyright file="IAuthenticationContext.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Portal
{
    public interface IAuthenticationContext
    {
        string BearerToken { get; }

        string? Email { get; }
    }
}

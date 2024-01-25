// <copyright file="IAuthenticationContext.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Notifications
{
    public interface IAuthenticationContext
    {
        string BearerToken { get; }
    }
}

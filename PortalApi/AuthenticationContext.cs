// <copyright file="AuthenticationContext.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.PortalApi
{
    using Kpmg.Constellation.IdentityService.Client;
    using Microsoft.AspNetCore.Http;

    public class AuthenticationContext : IAuthenticationContext
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly ISystemAccountAuthenticationProvider systemAccountAuthenticationProvider;

        public AuthenticationContext(IHttpContextAccessor httpContextAccessor, ISystemAccountAuthenticationProvider systemAccountAuthenticationProvider)
        {
            this.httpContextAccessor = httpContextAccessor;
            this.systemAccountAuthenticationProvider = systemAccountAuthenticationProvider;
        }

        public string BearerToken
        {
            get
            {
                if (this.httpContextAccessor?.HttpContext == null)
                {
                    // Generate Token systeme
                    return this.systemAccountAuthenticationProvider.GetTokenAsync().GetAwaiter().GetResult();
                }
                else
                {
                    var headers = this.httpContextAccessor?.HttpContext?.Request.Headers!;
                    if (!headers.ContainsKey("Authorization"))
                    {
                        throw new InvalidOperationException("An Authorization header is mandatory.");
                    }

                    var auths = headers["Authorization"];
                    foreach (var auth in auths)
                    {
                        if (auth.Contains("Bearer"))
                        {
                            return auth.Replace("Bearer", string.Empty).Trim();
                        }
                    }

                    throw new InvalidOperationException("A Bearer token is mandatory in the Authorization header.");
                }
            }
        }
    }
}

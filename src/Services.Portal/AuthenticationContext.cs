// <copyright file="AuthenticationContext.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Portal
{
    using Microsoft.AspNetCore.Http;

    public class AuthenticationContext : IAuthenticationContext
    {
        private readonly IHttpContextAccessor httpContextAccessor;

        public AuthenticationContext(IHttpContextAccessor httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;
        }

        public string BearerToken
        {
            get
            {
                if (this.httpContextAccessor?.HttpContext == null)
                {
                    throw new InvalidOperationException("no http context for the request.");
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
                        if (auth != null && auth.Contains("Bearer"))
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
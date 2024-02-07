// <copyright file="AuthenticationServices.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate
{
    using System.Security.Claims;
    using Microsoft.AspNetCore.Http;

    public class AuthenticationServices : IAuthenticationServices
    {
        private readonly IHttpContextAccessor httpContextAccessor;

        public AuthenticationServices(IHttpContextAccessor httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;
        }

        public string? Email
        {
            get
            {
                if (
                    this.httpContextAccessor.HttpContext != null &&
                    this.httpContextAccessor.HttpContext.User.Identity!.IsAuthenticated)
                {
                    var emailClaim = this.httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Email);
                    if (emailClaim != null)
                    {
                        return emailClaim.Value;
                    }
                }

                return null;
            }
        }
    }
}

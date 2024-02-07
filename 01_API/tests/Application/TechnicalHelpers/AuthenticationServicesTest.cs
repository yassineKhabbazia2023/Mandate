// <copyright file="AuthenticationServicesTest.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Application.Tests.TechnicalHelpers
{
    using System.Security.Claims;
    using Microsoft.AspNetCore.Http;

    public class AuthenticationServicesTest
    {
        [Fact]
        public void Email_ShouldReturnNull_WhenHttpContextIsNull()
        {
            HttpContext context = null!;
            var accessor = new Mock<IHttpContextAccessor>(MockBehavior.Strict);
            accessor.SetupGet(x => x.HttpContext).Returns(context).Verifiable();

            var authService = new AuthenticationServices(accessor.Object);

            var result = authService.Email;

            result.Should().BeNull();
        }

        [Fact]
        public void Email_ShouldReturnNull_When_IsNotAuthenticated()
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity());
            var authService = new AuthenticationServices(MockHttpContextAccessor(user));

            var result = authService.Email;

            result.Should().BeNull();
        }

        [Fact]
        public void Email_ShouldReturnNull_WhenEmailClaimNotFound()
        {
            var identity = new ClaimsIdentity() { };
            var user = new ClaimsPrincipal(identity);
            var authService = new AuthenticationServices(MockHttpContextAccessor(user));

            var result = authService.Email;

            result.Should().BeNull();
        }

        [Fact]
        public void Email_WhenUserAuthenticatedWithEmailClaim()
        {
            Claim[] claims = new[] { new Claim(ClaimTypes.Email, "user.demo@kpmg.fr") };
            var identity = new ClaimsIdentity(claims, "auth");
            var user = new ClaimsPrincipal(identity);

            var authService = new AuthenticationServices(MockHttpContextAccessor(user));

            var result = authService.Email;

            result.Should().Be("user.demo@kpmg.fr");
        }

        private static IHttpContextAccessor MockHttpContextAccessor(ClaimsPrincipal? user)
        {
            var httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            httpContextAccessorMock.SetupGet(x => x.HttpContext.User).Returns(user);
            return httpContextAccessorMock.Object;
        }
    }
}

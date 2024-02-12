// <copyright file="AuthenticationContextTest.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Portal.Tests
{
    using Kpmg.Constellation.IdentityService.Client;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Primitives;

    public class AuthenticationContextTest
    {
        [Fact]
        public void BearerToken_Get()
        {
            var dict = new HeaderDictionary
            {
                { "Authorization", new StringValues("Bearer abc") },
            };

            var request = new Mock<HttpRequest>();
            request.SetupGet(x => x.Headers)
                .Returns(dict)
                .Verifiable();

            var httpContext = new Mock<HttpContext>();
            httpContext.SetupGet(x => x.Request)
                .Returns(request.Object);

            var httpContextAccessor = new Mock<IHttpContextAccessor>(MockBehavior.Strict);
            httpContextAccessor.SetupGet(a => a.HttpContext).Returns(httpContext.Object);

            var authenticationContext = new AuthenticationContext(httpContextAccessor.Object);
            authenticationContext.BearerToken.Should().Be("abc");
        }

        [Fact]
        public void BearerToken_NoBearer()
        {
            var dict = new HeaderDictionary
            {
                { "Authorization", new StringValues("x abc") },
            };

            var request = new Mock<HttpRequest>();
            request.SetupGet(x => x.Headers)
                .Returns(dict)
                .Verifiable();

            var httpContext = new Mock<HttpContext>();
            httpContext.SetupGet(x => x.Request)
                .Returns(request.Object);

            var httpContextAccessor = new Mock<IHttpContextAccessor>(MockBehavior.Strict);
            httpContextAccessor.SetupGet(a => a.HttpContext).Returns(httpContext.Object);

            var authenticationContext = new AuthenticationContext(httpContextAccessor.Object);
            Func<string> act = () => authenticationContext.BearerToken;
            act.Should().ThrowExactly<InvalidOperationException>().WithMessage("A Bearer token is mandatory in the Authorization header.");
        }

        [Fact]
        public void BearerToken_NoRequest_ThrowExeption()
        {
            HttpContext httpContext = null!;

            var httpContextAccessor = new Mock<IHttpContextAccessor>(MockBehavior.Strict);
            httpContextAccessor.SetupGet(a => a.HttpContext).Returns(httpContext);

            var authenticationContext = new AuthenticationContext(httpContextAccessor.Object);

            Func<string> act = () => authenticationContext.BearerToken;
            act.Should().ThrowExactly<InvalidOperationException>().WithMessage("no http context for the request.");

            httpContextAccessor.Verify();
        }
    }
}

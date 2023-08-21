// <copyright file="ApiDocumentationControllerTest.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.AspNetCore.Tests
{
    using KPMG.Pulse.Back.Accounting.Mandate.AspNetCore;
    using Microsoft.AspNetCore.Mvc;

    public class ApiDocumentationControllerTest
    {
        [Fact]
        public void Get()
        {
            var controller = new ApiDocumentationController();

            var result = controller.Get() as FileResult;

            result!.ContentType.Should().Be("application/json");
        }
    }
}

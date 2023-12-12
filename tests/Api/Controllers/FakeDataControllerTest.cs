// <copyright file="FakeDataControllerTest.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.AspNetCore.Tests
{
    using KPMG.Pulse.Back.Accounting.Mandate.Adapters;
    using KPMG.Pulse.Back.Accounting.Mandate.Application;
    using KPMG.Pulse.Back.Accounting.Mandate.Sql;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Logging.Abstractions;

    public class FakeDataControllerTest
    {
        [Fact]
        public async Task CreateFakeRef()
        {
            var mandateRepository = new Mock<IMandateRepository>(MockBehavior.Strict);
            mandateRepository.Setup(r => r.CreateFakeRefAsync()).Returns(Task.CompletedTask).Verifiable();
            mandateRepository.Setup(r => r.DeleteFakeRefAsync()).Returns(Task.CompletedTask).Verifiable();
            mandateRepository.Setup(r => r.CreateFakeAuthAsync()).Returns(Task.CompletedTask).Verifiable();
            mandateRepository.Setup(r => r.DeleteFakeAuthAsync()).Returns(Task.CompletedTask).Verifiable();
            mandateRepository.Setup(r => r.AddFakeDataAsync()).Returns(Task.CompletedTask).Verifiable();
            mandateRepository.Setup(r => r.DeleteFakeDataAsync()).Returns(Task.CompletedTask).Verifiable();

            var sqlAdapter = new SqlAdapter(mandateRepository.Object);
            var fakeDataManager = new FakeDataManager(sqlAdapter);
            var fakeDataControler = new FakeDataController(new NullLoggerFactory().CreateLogger<FakeDataController>(), fakeDataManager);

            await fakeDataControler.CreateFakeRef();
            await fakeDataControler.DeleteFakeRef();
            await fakeDataControler.CreateFakeAuth();
            await fakeDataControler.DeleteFakeAuth();
            await fakeDataControler.AddFakeData();
            await fakeDataControler.DeleteFakeData();

            mandateRepository.VerifyAll();
        }
    }
}

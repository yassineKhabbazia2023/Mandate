using KPMG.Pulse.Back.Accounting.Mandate.Function.Interfaces;
using KPMG.Pulse.Back.Accounting.Mandate.Sql;
using Mandate.AzureFunctions;
using Mandate.AzureFunctions.Functions;
using Mandate.AzureFunctions.Interfaces;
using Mandate.AzureFunctions.Managers;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Pulse.Back.Events.IntegrationEvents;

namespace KPMG.Pulse.Back.Accounting.Mandate.AzureFunctions.Tests.Functions;

public class MonitoringJdcStatusFunctionTest
{
    [Theory]
    [InlineData("2", typeof(MandateStatusChangedEvent))]
    [InlineData("4", typeof(MandateStatusChangedEvent))]
    [InlineData("9", typeof(MandateStatusChangedEvent))]
    [InlineData("17", typeof(MandateStatusChangedEvent))]
    public async Task StatusesMonitoringHourlyRunScheduleStart_When_AMandateHasBeenUpdated_SendAppropriatedEvent(string newStatus, Type eventType)
    {
        var options = Options.Create(new UpdateStatusesConfiguration
        {
            DailyStatusCodes = [],
            HourlyStatusCodes = [10]
        });
        ILoggerFactory loggerFactory = new LoggerFactory();
        var logger = loggerFactory.CreateLogger<UpdateMandateStatusesHandler>();
        var mandateEventPublisherMock = new Mock<IMandateEventPublisher>();
        var repositoryMock = new Mock<IMandateRepository>();
        var jeDeclareServiceMock = new Mock<IJeDeclareService>();

        IUpdateMandateStatusesHandler mandateManager = new UpdateMandateStatusesHandler(
            logger,
            mandateEventPublisherMock.Object,
            repositoryMock.Object,
            jeDeclareServiceMock.Object
        );

        var collectionId = Guid.NewGuid();
        jeDeclareServiceMock.Setup(s => s.GetAllConfigurationFromFolderAsync(It.IsAny<string>()))
            .ReturnsAsync([
                new TechnicalCollection(collectionId, "FolderId", "RibId",
                    new BankDetails("bankCode", "branchCode", "accountNumber", "checkDigits"),
                    newStatus)
            ]);
        repositoryMock.Setup(r => r.GetCollectionByStatus(new List<int> { 10 })).ReturnsAsync(
            [new MandateIdsAndStatus(collectionId, "AccountNumber", "JdcDossierId", "RibId", 3, 10, 5)]);
        mandateEventPublisherMock.Setup(p => p.PublishEventAsync(It.Is<object>(t => t.GetType() == eventType)))
            .Returns(Task.CompletedTask);

        var sut = new MonitoringJdcStatusFunction(options, loggerFactory, mandateManager);
        await sut.StatusesMonitoringHourlyRunScheduleStart(new TimerInfo());
        mandateEventPublisherMock.Verify(p => p.PublishEventAsync(It.Is<object>(t => t.GetType() == eventType)), Times.Once);
    }
}
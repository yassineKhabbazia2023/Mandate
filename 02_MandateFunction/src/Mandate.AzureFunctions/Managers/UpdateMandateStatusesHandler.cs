// <copyright file="MandateFunctionManager.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

using KPMG.Pulse.Back.Accounting.Mandate;
using KPMG.Pulse.Back.Accounting.Mandate.Function.Interfaces;
using Microsoft.Extensions.Logging;
using Pulse.Back.Events.IntegrationEvents;
using Pulse.Back.Events.IntegrationEvents.EventsData;

namespace Mandate.AzureFunctions.Managers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Interfaces;
    using KPMG.Pulse.Back.Accounting.Mandate.Function.Managers;
    using KPMG.Pulse.Back.Accounting.Mandate.Sql;

    /// <inheritdoc/>
    public class UpdateMandateStatusesHandler : IUpdateMandateStatusesHandler
    {
        private readonly ILogger<UpdateMandateStatusesHandler> _logger;
        private readonly IMandateEventPublisher _mandateEventPublisher;
        private readonly IMandateRepository _repository;
        private readonly IJeDeclareService _jeDeclareService;

        public UpdateMandateStatusesHandler(
            ILogger<UpdateMandateStatusesHandler> logger,
            IMandateEventPublisher mandateEventPublisher,
            IMandateRepository repository,
            IJeDeclareService jeDeclareService)
        {
            _logger = logger;
            _mandateEventPublisher = mandateEventPublisher;
            _repository = repository;
            _jeDeclareService = jeDeclareService;
        }

        public async Task UpdateMandateStatusAsync(List<int> statusCodes)
        {
            var mandatesToPotentiallyUpdate = await _repository.GetCollectionByStatus(statusCodes);

            foreach (var group in mandatesToPotentiallyUpdate.GroupBy(mandate => mandate.JdcDossierId))
            {
                var jdcDossierId = group.Key;
                var ribIds = group.Select(i => i.JdcRibId).ToList();
                var mandates = await _jeDeclareService.GetAllConfigurationFromFolderAsync(jdcDossierId);
                var statusByRibId = mandates.Where(m => ribIds.Contains(m.RibId))
                    .ToDictionary(mandate => mandate.RibId, mandate => int.Parse(mandate.StatusCode));

                var mandatesToUpdate =
                    mandatesToPotentiallyUpdate
                        .Where(toUpdate =>
                            statusByRibId.ContainsKey(toUpdate.JdcRibId)
                            && statusByRibId[toUpdate.JdcRibId] !=
                        toUpdate.StatusCode)
                        .Select(item => new MandateToUpdate(item.Id, item.AccountNumber, statusByRibId[item.JdcRibId], item.CreatedBy, item.AccountId))
                        .ToList();

                await UpdateMandatesStatusAsync(mandatesToUpdate);
                await SendStatusUpdateEventAsync(mandatesToUpdate);
            }
        }

        private const int ActivatedStatusCode = 2;
        private const int TerminatedStatusCode = 4;

        private static int[] IncidentStatusCodes =
        [
            9, // Rib non utilisé comme un relevé
            17 // Activation demandée, mandat rejeté
        ];

        private async Task SendStatusUpdateEventAsync(List<MandateToUpdate> mandatesWithNewStatus)
        {
            var mandatesWithNewStatusList = mandatesWithNewStatus.Where(m => m.CreatedById.HasValue);
            if (mandatesWithNewStatusList != null && mandatesWithNewStatusList.Any())
            {
                foreach (var mandate in mandatesWithNewStatus)
                {
                    string statusString = mandate.NextStatus switch
                    {
                        2 => "Activated",
                        4 => "Terminated",
                        9 or 17 => "Incident",
                        _ => "Unknown"
                    };
                    if (mandate.CreatedById.HasValue)
                    {
                        var eventData = new MandateEventData
                        {
                            BankAccountNumber = mandate.AccountNumber,
                            MandateStatus = statusString,
                            AccounId = mandate.AccountId,
                            MandateCreatorId = mandate.CreatedById.Value,
                        };
                        await _mandateEventPublisher.PublishEventAsync(new MandateStatusChangedEvent(eventData));

                    }

                }

                _logger.LogInformation("Mandate status events sent: {Count}", mandatesWithNewStatus.Count);
            }
            else
            {
                _logger.LogInformation("No Mandate To send : Mandate Creator null");

            }
        }

        private async Task UpdateMandatesStatusAsync(List<MandateToUpdate> mandatesToUpdate)
        {
            var statuses = mandatesToUpdate.Select(
                mandateToUpdate =>
                    new StatusDb
                    {
                        CollectionId = mandateToUpdate.CollectionId,
                        IsCurrent = true,
                        StatusCode = mandateToUpdate.NextStatus,
                        StatusDate = DateTime.UtcNow,
                        CreatedBy = "azfUpdate",
                    });

            await _repository.UpdateMandatesStatusAsync(statuses);
        }
    }
}
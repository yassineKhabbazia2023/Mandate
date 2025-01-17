// <copyright file="MandateFunctionManager.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

using KPMG.Pulse.Back.Accounting.Mandate;
using Microsoft.Extensions.Logging;

namespace Mandate.AzureFunctions.Managers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Interfaces;
    using KPMG.Pulse.Back.Accounting.Mandate.Sql;

    /// <inheritdoc/>
    public class UpdateMandateStatusesHandler : IUpdateMandateStatusesHandler
    {
        private readonly ILogger<UpdateMandateStatusesHandler> _logger;
        private readonly IMandateRepository _repository;
        private readonly IJeDeclareService _jeDeclareService;

        public UpdateMandateStatusesHandler(ILogger<UpdateMandateStatusesHandler> logger, IMandateRepository repository,
            IJeDeclareService jeDeclareService)
        {
            _logger = logger;
            _repository = repository;
            _jeDeclareService = jeDeclareService;
        }

        public async Task UpdateMandateStatusAsync(List<int> statusCodes)
        {
            var mandatesToUpdate = await _repository.GetCollectionByStatus(statusCodes);

            foreach (var group in mandatesToUpdate.GroupBy(mandate => mandate.JdcDossierId))
            {
                var jdcDossierId = group.Key;
                var ribIds = group.Select(i => i.JdcRibId).ToList();
                var mandates = await _jeDeclareService.GetAllConfigurationFromFolderAsync(jdcDossierId);
                var statusByRibId = mandates.Where(m => ribIds.Contains(m.RibId))
                    .ToDictionary(mandate => mandate.RibId, mandate => int.Parse(mandate.StatusCode));

                await UpdateMandatesStatusAsync(mandatesToUpdate, statusByRibId);
            }
        }

        private async Task UpdateMandatesStatusAsync(List<MandateIdsAndStatus> mandatesToUpdate, Dictionary<string, int> statusByRibId)
        {
            var mandatesReallyToUpdate =
                mandatesToUpdate
                    .Where(toUpdate =>
                        statusByRibId.ContainsKey(toUpdate.JdcRibId)
                        && statusByRibId[toUpdate.JdcRibId] !=
                        toUpdate.StatusCode);
            var statuses = mandatesReallyToUpdate.Select(
                mandateToUpdate =>
                new StatusDb
                {
                    CollectionId = mandateToUpdate.Id,
                    IsCurrent = true,
                    StatusCode = statusByRibId[mandateToUpdate.JdcRibId],
                    StatusDate = DateTime.UtcNow,
                    CreatedBy = "azfUpdate",
                });
            
            await _repository.UpdateMandatesStatusAsync(statuses); 
        }
    }
}
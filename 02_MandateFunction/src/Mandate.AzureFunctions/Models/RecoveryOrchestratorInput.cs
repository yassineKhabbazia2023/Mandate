// <copyright file="RecoveryOrchestratorInput.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace Mandate.AzureFunctions
{
    public class RecoveryOrchestratorInput
    {
        public string LimitConfig { get; set; }

        public int Skip { get; set; }
    }
}

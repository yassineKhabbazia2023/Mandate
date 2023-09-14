// <copyright file="BankManager.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Application
{
    public class BankManager : IBankManager
    {
        private readonly IDatabaseService databaseService;

        public BankManager(IDatabaseService databaseService)
        {
            this.databaseService = databaseService;
        }

        public async Task<Bank> GetByCodeAsync(string bankCode)
        {
            return await this.databaseService.GetBankByCodeAsync(bankCode).ConfigureAwait(false);
        }
    }
}

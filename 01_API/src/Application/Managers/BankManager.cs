// <copyright file="BankManager.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

using Pulse.Back.Accounting.Mandate.Application;
using Pulse.Back.Accounting.Mandate.Application.Exceptions;
using Pulse.ExceptionMiddleware.Exceptions;

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

            if (Enum.TryParse<BankCodeNotAuthorized>(bankCode, out var bankCodeOut)
             && Enum.IsDefined(typeof(BankCodeNotAuthorized), bankCodeOut))
            {
                throw new BadRequestException(Errors.NotAuthorizedBankCode, string.Format(Errors.NotAuthorizedBankCodeMessage, bankCode));
            }
            return await this.databaseService.GetBankByCodeAsync(bankCode).ConfigureAwait(false);
        }
    }
}

// <copyright file="PagedRecoveryMandate.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Client
{
    public class PagedRecoveryMandate
    {
        public PagedRecoveryMandate(int imported, IReadOnlyList<CollectionSummary> failed)
        {
            this.Imported = imported;
            this.Failed = failed;
        }

        public int Imported { get; }

        public IReadOnlyList<CollectionSummary> Failed { get; }
    }
}

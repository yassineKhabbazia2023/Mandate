// <copyright file="PagedMandate.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate
{
    public class PagedMandate
    {
        public PagedMandate(Counters counters, List<Collection> data)
        {
            this.Counters = counters;
            this.Data = data;
        }

        public Counters Counters { get; }

        public IEnumerable<Collection> Data { get; }
    }
}
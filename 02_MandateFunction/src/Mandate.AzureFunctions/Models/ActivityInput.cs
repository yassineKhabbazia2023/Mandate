// <copyright file="ActivityInput.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Function.Models
{
    public class ActivityInput
    {
        public ActivityInput(int skip, int limit)
        {
            this.Skip = skip;
            this.Limit = limit;
        }

        public int Skip { get; }

        public int Limit { get; }
    }
}

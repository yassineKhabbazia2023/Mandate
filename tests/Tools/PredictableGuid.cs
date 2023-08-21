// <copyright file="PredictableGuid.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate
{
    public class PredictableGuid : IGuidGenerator
    {
        private readonly byte diff = 0;
        private int seed = 1;

        public PredictableGuid(byte diff = 0)
        {
            this.diff = diff;
        }

        public Guid NewGuid() => new (this.seed++, 0, 0, 0, 0, 0, 0, 0, 0, 0, this.diff);
    }
}

// <copyright file="Counters.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate
{
    public class Counters
    {
        public Counters(int all, int status10, int status20, int status30, int status40, int status50)
        {
            this.All = all;
            this.Status10 = status10;
            this.Status20 = status20;
            this.Status30 = status30;
            this.Status40 = status40;
            this.Status50 = status50;
        }

        public int All { get; }

        public int Status10 { get; }

        public int Status20 { get; }

        public int Status30 { get; }

        public int Status40 { get; }

        public int Status50 { get; }
    }
}
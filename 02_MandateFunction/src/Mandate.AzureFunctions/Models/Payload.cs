// <copyright file="Payload.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace Mandate.AzureFunctions
{
    using System.Collections.Generic;

    public class Payload
    {
        public Payload(int skip, int limit, List<int> statusCodes)
        {
            this.Skip = skip;
            this.Limit = limit;
            this.StatusCodes = statusCodes;
        }

        public int Skip { get; }

        public int Limit { get; }

        public List<int> StatusCodes { get; }
    }
}

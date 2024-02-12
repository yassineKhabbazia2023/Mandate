// <copyright file="Status.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate
{
    public class Status
    {
        public Status(CollectionStatus statusCode, string statusName)
        {
            this.StatusCode = statusCode;
            this.StatusName = statusName;
        }

        public CollectionStatus StatusCode { get; }

        public string StatusName { get; }
    }
}
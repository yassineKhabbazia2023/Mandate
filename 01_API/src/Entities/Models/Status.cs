// <copyright file="Status.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate
{
    public class Status
    {
        public Status(CollectionStatus statusCode, string statusName, JdcCollectionStatus statusCodeJdc)
        {
            this.StatusCode = statusCode;
            this.StatusName = statusName;
            this.StatusCodeJdc = statusCodeJdc;
        }

        public CollectionStatus StatusCode { get; }

        public JdcCollectionStatus StatusCodeJdc { get; }

        public string StatusName { get; }
    }
}
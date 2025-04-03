// <copyright file="Status.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate
{
    public class Status
    {
        public Status(CollectionStatus statusCode, string statusName, JdcCollectionStatus statusCodeJdc, string? errorMessage)
        {
            this.StatusCode = statusCode;
            this.StatusName = statusName;
            this.StatusCodeJdc = statusCodeJdc;
            this.ErrorMessage = errorMessage;
        }

        public CollectionStatus StatusCode { get; }

        public JdcCollectionStatus StatusCodeJdc { get; }

        public string StatusName { get; }

        public string? ErrorMessage { get; }
    }
}
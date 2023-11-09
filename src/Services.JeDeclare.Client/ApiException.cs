// <copyright file="ApiException.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.JeDeclare.Client
{
    using System.Net;
    using System.Runtime.Serialization;

    [Serializable]
    public class ApiException : Exception
    {
        public ApiException()
        {
        }

        public ApiException(string message)
            : base(message)
        {
        }

        public ApiException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public ApiException(string message, HttpStatusCode statusCode, Exception innerException)
            : base(message, innerException)
        {
            this.StatusCode = statusCode;
        }

        public ApiException(string message, HttpStatusCode statusCode, Error error)
            : base(message)
        {
            this.Error = error;
            this.StatusCode = statusCode;
        }

        protected ApiException(SerializationInfo info, StreamingContext context)
          : base(info, context)
        {
        }

        public HttpStatusCode StatusCode { get; }

        public Error Error { get; }
    }
}

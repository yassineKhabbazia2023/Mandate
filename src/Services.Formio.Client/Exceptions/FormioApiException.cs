// <copyright file="FormIoApiException.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Formio.Client
{
    using System.Runtime.Serialization;

    [Serializable]
    public class FormIoApiException : Exception
    {
        public FormIoApiException()
        {
        }

        public FormIoApiException(string message)
            : base(message)
        {
        }

        public FormIoApiException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected FormIoApiException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}

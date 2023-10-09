// <copyright file="FormioApiException.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Formio.Client
{
    using System.Runtime.Serialization;

    [Serializable]
    public class FormioApiException : Exception
    {
        public FormioApiException()
        {
        }

        public FormioApiException(string message)
            : base(message)
        {
        }

        public FormioApiException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public FormioApiException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}

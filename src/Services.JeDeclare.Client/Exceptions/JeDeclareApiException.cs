// <copyright file="JeDeclareApiException.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.JeDeclare.Client
{
    using System.Runtime.Serialization;

    [Serializable]
    public class JeDeclareApiException : Exception
    {
        public JeDeclareApiException()
        {
        }

        public JeDeclareApiException(string message)
            : base(message)
        {
        }

        public JeDeclareApiException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected JeDeclareApiException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}

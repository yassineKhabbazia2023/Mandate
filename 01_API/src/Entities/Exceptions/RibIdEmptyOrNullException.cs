// <copyright file="RibIdEmptyOrNullException.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate
{
    using System.Runtime.Serialization;

    [Serializable]
    public class RibIdEmptyOrNullException : Exception
    {
        public RibIdEmptyOrNullException()
        {
        }

        public RibIdEmptyOrNullException(string message)
            : base(message)
        {
        }

        public RibIdEmptyOrNullException(string message, Exception inner)
            : base(message, inner)
        {
        }

        protected RibIdEmptyOrNullException(SerializationInfo info, StreamingContext context)
              : base(info, context)
        {
        }
    }
}

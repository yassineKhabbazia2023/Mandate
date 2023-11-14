// <copyright file="CollectionNotFoundException.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate
{
    using System.Runtime.Serialization;

    [Serializable]
    public class CollectionNotFoundException : Exception
    {
        public CollectionNotFoundException()
        {
        }

        public CollectionNotFoundException(string message)
            : base(message)
        {
        }

        public CollectionNotFoundException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected CollectionNotFoundException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}

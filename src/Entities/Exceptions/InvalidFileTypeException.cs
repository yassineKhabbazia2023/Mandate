// <copyright file="InvalidFileTypeException.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate
{
    using System.Runtime.Serialization;

    [Serializable]
    public class InvalidFileTypeException : Exception
    {
        public InvalidFileTypeException()
        {
        }

        public InvalidFileTypeException(string message)
            : base(message)
        {
        }

        public InvalidFileTypeException(string message, Exception inner)
            : base(message, inner)
        {
        }

        protected InvalidFileTypeException(SerializationInfo info, StreamingContext context)
              : base(info, context)
        {
        }
    }
}

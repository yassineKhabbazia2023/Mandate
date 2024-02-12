// <copyright file="FolderIdEmptyOrNullException.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate
{
    using System.Runtime.Serialization;

    [Serializable]
    public class FolderIdEmptyOrNullException : Exception
    {
        public FolderIdEmptyOrNullException()
        {
        }

        public FolderIdEmptyOrNullException(string message)
            : base(message)
        {
        }

        public FolderIdEmptyOrNullException(string message, Exception inner)
            : base(message, inner)
        {
        }

        protected FolderIdEmptyOrNullException(SerializationInfo info, StreamingContext context)
              : base(info, context)
        {
        }
    }
}

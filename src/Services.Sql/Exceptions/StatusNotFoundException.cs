// <copyright file="StatusNotFoundException.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Sql
{
    using System.Runtime.Serialization;

    [Serializable]
    public class StatusNotFoundException : Exception
    {
        public StatusNotFoundException()
        {
        }

        public StatusNotFoundException(string message)
            : base(message)
        {
        }

        public StatusNotFoundException(string message, Exception inner)
            : base(message, inner)
        {
        }

        protected StatusNotFoundException(SerializationInfo info, StreamingContext context)
              : base(info, context)
        {
        }

        public static StatusNotFoundException FromId(Guid collectionId)
        {
            return new StatusNotFoundException($"La collection avec l\'id '{collectionId}' n'a pas de status en cours");
        }
    }
}

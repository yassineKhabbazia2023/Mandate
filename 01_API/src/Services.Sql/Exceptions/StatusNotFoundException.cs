// <copyright file="StatusNotFoundException.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Sql;

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

    public static StatusNotFoundException FromId(Guid collectionId)
    {
        return new StatusNotFoundException($"La collection avec l\'id '{collectionId}' n'a pas de status en cours");
    }

    public static StatusNotFoundException FromId(string jdcStatusCode)
    {
        return new StatusNotFoundException($"Le status code jdc '{jdcStatusCode}' n'a pas été trouvé dans la collection");
    }
}

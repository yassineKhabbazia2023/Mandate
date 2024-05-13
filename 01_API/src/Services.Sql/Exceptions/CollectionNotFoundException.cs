// <copyright file="CollectionNotFoundException.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Sql;

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

    public static CollectionNotFoundException FromId(string collectionId)
    {
        return new CollectionNotFoundException($"La collecton avec l'id '{collectionId}' n'a pas été trouvée");
    }
}

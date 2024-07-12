// <copyright file="FolderIdEmptyOrNullException.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate;

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
}

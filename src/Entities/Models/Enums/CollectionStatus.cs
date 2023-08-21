// <copyright file="CollectionStatus.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate
{
    public enum CollectionStatus : byte
    {
        ToDo = 10,

        Rejected = 20,

        InProgress = 30,

        Active = 40,

        Inactive = 50,
    }
}

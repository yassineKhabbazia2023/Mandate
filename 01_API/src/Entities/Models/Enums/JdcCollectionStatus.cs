// <copyright file="JdcCollectionStatus.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate
{
    public enum JdcCollectionStatus
    {
        Activation_Requested_Signed_Mandate_Uploaded = -2,
        Incident = -1,
        Creation_Failed = 98,
        Activation_Requested_Collection_Pending = 10,
        Creation_InProgress = 99,
    }
}
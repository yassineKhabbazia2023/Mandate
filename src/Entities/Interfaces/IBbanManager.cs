// <copyright file="IBbanManager.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate
{
    public interface IBbanManager
    {
        bool IsValid(string bban);
    }
}

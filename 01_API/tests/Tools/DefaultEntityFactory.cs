// <copyright file="DefaultEntityFactory.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate
{
    public static class DefaultEntityFactory
    {
        public static Address Address => new (default, default, default, default, default);

        public static Bank Bank => new (default!, default, default, default, default!);

        public static Bban Bban => new (default!, default!, default!, default!, default!, default!);

        public static Signatory Signatory => new (default!, default!, default!, default!);

        public static Company Company => new (default!, default!, default!, default!, default!, Signatory, Address);
    }
}

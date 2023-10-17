// <copyright file="JeDeclareCollection.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate
{
    public class JeDeclareCollection
    {
        public JeDeclareCollection(Guid id, string jdcReleveId, string jdcRibId, Collection collection)
        {
            this.Id = id;
            this.JdcRibId = jdcRibId;
            this.JdcReleveId = jdcReleveId;
            this.Collection = collection;
        }

        public Guid Id { get; }

        public Collection? Collection { get; }

        public string? JdcReleveId { get; }

        public string? JdcRibId { get; }
    }
}

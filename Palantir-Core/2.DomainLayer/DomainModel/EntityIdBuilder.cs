namespace Ix.Palantir.DomainModel
{
    using System.Diagnostics.Contracts;

    using Ix.Palantir.Logging;
    using Ix.Palantir.Utilities;

    public class EntityIdBuilder : IEntityIdBuilder
    {
        private readonly ILog log;

        public EntityIdBuilder(ILog log)
        {
            this.log = log;
        }

        public string CreateEntityId(string entityName, string majorId, params string[] minorIds)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(entityName));
            Contract.Requires(!string.IsNullOrWhiteSpace(majorId));
            Contract.Requires(minorIds != null);
            Contract.Requires(minorIds.Length > 0);

            SeparatedStringBuilder stringBuilder = new SeparatedStringBuilder("_", minorIds);
            var entityId = string.Concat(entityName.ToLower(), "_", majorId.ToLower(), "_", stringBuilder.ToString());
            this.log.DebugFormat("Generated caching key: {0}", entityId);
            return entityId;
        }
    }
}
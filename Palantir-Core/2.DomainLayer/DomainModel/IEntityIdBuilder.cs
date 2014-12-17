namespace Ix.Palantir.DomainModel
{
    public interface IEntityIdBuilder
    {
        string CreateEntityId(string entityName, string majorId, params string[] minorIds);
    }
}
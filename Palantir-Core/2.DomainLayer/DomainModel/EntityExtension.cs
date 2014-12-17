namespace Ix.Palantir.DomainModel
{
    public static class EntityExtension
    {
        public static bool IsTransient(this IEntity entity)
        {
            return entity != null && entity.Id <= 0;
        }
    }
}
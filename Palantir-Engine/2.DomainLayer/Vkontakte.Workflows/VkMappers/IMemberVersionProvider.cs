namespace Ix.Palantir.Vkontakte.Workflows.VkMappers
{
    public interface IMemberVersionProvider
    {
        int GetNextVersionNumber(int vkGroupId);
    }
}
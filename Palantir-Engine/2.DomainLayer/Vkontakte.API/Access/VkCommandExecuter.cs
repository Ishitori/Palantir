namespace Ix.Palantir.Vkontakte.API.Access
{
    using System.Collections.Generic;

    public class VkCommandExecuter : IVkCommandExecuter
    {
        private readonly IVkAccessor vkAccessor;

        public VkCommandExecuter(IVkAccessor vkAccessor)
        {
            this.vkAccessor = vkAccessor;
        }

        public bool JoinGroup(string groupId)
        {
            string callResult = this.vkAccessor.ExecuteCall("groups.join.xml", new Dictionary<string, string> { { "gid", groupId } });
            return callResult.IndexOf("<response>1</response>", System.StringComparison.Ordinal) != -1;
        }
    }
}
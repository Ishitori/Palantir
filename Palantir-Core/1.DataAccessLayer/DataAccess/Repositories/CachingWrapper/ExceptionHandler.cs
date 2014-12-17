namespace Ix.Palantir.DataAccess.Repositories.CachingWrapper
{
    using System;
    using Ix.Palantir.DomainModel;

    public static class ExceptionHandler
    {
        public static void HandleSaveException(Exception exc, IVkEntity vkEntity, string name)
        {
            ////ILog log = Factory.GetInstance<ILog>();
            exc.Data.Add("VkGroupId", vkEntity.VkGroupId);
            exc.Data.Add("VkId", vkEntity.VkId);
            exc.Data.Add("Name", name);
            throw exc;
            ////log.ErrorFormat("Exception while saving a {4} VkGroupId=\"{0}\" VkId=\"{1}\", PostedDate=\"{2}\" exc: {3}", vkEntity.VkGroupId, vkEntity.VkId, vkEntity.PostedDate, exc.ToString(), name);
        }
        public static void HandleSaveException(Exception exc, Member member)
        {
            ////ILog log = Factory.GetInstance<ILog>();
            ////log.ErrorFormat("Exception while saving a member VkGroupId=\"{0}\" VkId=\"{1}\", exc: {2}", member.VkGroupId, member.VkId, exc.ToString());
            exc.Data.Add("VkGroupId", member.VkGroupId);
            exc.Data.Add("VkId", member.VkId);
            exc.Data.Add("Name", member);
        }
    }
}
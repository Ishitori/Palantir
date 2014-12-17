namespace Ix.Palantir.Infrastructure.Process
{
    using System.Collections.Generic;
    using System.Linq;
    using Ix.Palantir.DataAccess.API.Repositories;
    using Ix.Palantir.DomainModel;
    using Ix.Palantir.Logging;
    using Ix.Palantir.Utilities;
    using Ix.Palantir.Vkontakte.API.Access;
    using MembershipProfile = Ix.Palantir.Vkontakte.API.Responses.MembershipProfile.response;

    public class JoinVkGroupProcess
    {
        private readonly IVkGroupRepository groupRepository;
        private readonly ILog log;
        private readonly IVkConnectionBuilder vkConnectionBuilder;

        public JoinVkGroupProcess(IVkConnectionBuilder vkConnectionBuilder, IVkGroupRepository groupRepository, ILog log)
        {
            this.log = log;
            this.vkConnectionBuilder = vkConnectionBuilder;
            this.groupRepository = groupRepository;
        }

        public void JoinAllGroups()
        {
            this.log.Debug("Groups participation checking started");
            IList<VkGroup> allExistingGroups = this.groupRepository.GetGroups();
            MembershipProfile groupMembershipStatus = this.vkConnectionBuilder.GetVkDataProvider().GetGroupMembershipStatus();

            ICollection<VkGroup> notSubscribedGroups = this.GetNotSubscribedGroups(allExistingGroups, groupMembershipStatus);
            this.SubscribeToGroups(notSubscribedGroups);
            this.log.Debug("Groups participation checking finished");
        }

        private void SubscribeToGroups(IEnumerable<VkGroup> notSubscribedGroups)
        {
            IVkCommandExecuter commandExecuter = this.vkConnectionBuilder.GetVkCommandExecuter();

            foreach (var group in notSubscribedGroups)
            {
                bool joinGroup = commandExecuter.JoinGroup(group.Id.ToString());

                if (!joinGroup)
                {
                    this.log.WarnFormat("Unable to join group \"{0}\"", group.Id);
                }
            }
        }

        private ICollection<VkGroup> GetNotSubscribedGroups(IList<VkGroup> allExistingGroups, MembershipProfile groupMembershipStatus)
        {
            IList<VkGroup> notSubscribedGroups = new List<VkGroup>();

            foreach (var existingGroup in allExistingGroups)
            {
                string groupId = existingGroup.Id.ToString();

                if (groupMembershipStatus.group != null && groupMembershipStatus.group.All(g => g.gid != groupId))
                {
                    notSubscribedGroups.Add(existingGroup);
                }
            }

            ICollection<string> groupIds = notSubscribedGroups.Select(g => g.Id.ToString()).ToList();

            if (groupIds.Count > 0)
            {
                this.log.InfoFormat("User is not in groups: {0}", new SeparatedStringBuilder(groupIds).ToString());
            }
            else
            {
                this.log.Info("User participates in all groups");
            }

            return notSubscribedGroups;
        }
    }
}
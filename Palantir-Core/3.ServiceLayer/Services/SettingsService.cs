namespace Ix.Palantir.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Ix.Palantir.DataAccess.API;
    using Ix.Palantir.DataAccess.API.Repositories;
    using Ix.Palantir.DomainModel;
    using Ix.Palantir.Localization.API;
    using Ix.Palantir.Security;
    using Ix.Palantir.Security.API;
    using Ix.Palantir.Services.API;

    public class SettingsService : ISettingsService
    {
        private readonly IUnitOfWorkProvider unitOfWorkProvider;
        private readonly IProjectRepository projectRepository;
        private readonly IVkGroupRepository vkGroupRepository;
        private readonly IDateTimeHelper dateTimeHelper;
        private readonly ICurrentUserProvider currentUserProvider;
        private readonly IProjectService projectService;

        public SettingsService(IUnitOfWorkProvider unitOfWorkProvider, IProjectRepository projectRepository, IVkGroupRepository vkGroupRepository, IDateTimeHelper dateTimeHelper, ICurrentUserProvider currentUserProvider, IProjectService projectService)
        {
            this.unitOfWorkProvider = unitOfWorkProvider;
            this.projectRepository = projectRepository;
            this.vkGroupRepository = vkGroupRepository;
            this.dateTimeHelper = dateTimeHelper;
            this.currentUserProvider = currentUserProvider;
            this.projectService = projectService;
        }

        public IList<GroupProcessingItem> GetProcessingHistory(int projectId)
        {
            using (this.unitOfWorkProvider.CreateUnitOfWork())
            {
                VkGroup group = this.projectRepository.GetVkGroup(projectId);
                var items = this.vkGroupRepository.GetLatestProcessingItems(@group.Id);

                IList<GroupProcessingItem> groupItems = items.Values
                    .Where(x => x.FeedType != DataFeedType.MembersCount)
                    .Select(x => new GroupProcessingItem() 
                    { 
                        ProcessingDate = this.dateTimeHelper.GetLocalUserDate(x.ProcessingDate), 
                        Item = this.GetItemTitle(x.FeedType) 
                    }).ToList();
                return groupItems;
            } 
        }

        public void DeleteProject(int projectId)
        {
            int accountId = this.currentUserProvider.GetCurrentUser().GetAccount().Id;
            var project = this.projectRepository.GetByAccountId(accountId).FirstOrDefault(x => x.Id == projectId);

            if (project != null)
            {
                this.projectService.DeleteProject(projectId, project.VkGroup.Id);
            }
        }

        private string GetItemTitle(DataFeedType feedType)
        {
            switch (feedType)
            {
                case DataFeedType.WallPosts:
                    return "Посты";
                
                case DataFeedType.Photo:
                    return "Фотографии";

                case DataFeedType.Video:
                    return "Видео";

                case DataFeedType.Administrators:
                    return "Администраторы";

                case DataFeedType.WallPostComments:
                    return "Комментарии к постам";

                case DataFeedType.Topic:
                    return "Темы";

                case DataFeedType.TopicComment:
                    return "Сообщения в темах";

                case DataFeedType.MembersInfo:
                    return "Участники";

                case DataFeedType.PhotoAlbumDetails:
                    return "Информация о фотографиях";

                case DataFeedType.VideoComments:
                    return "Комментарии к видео";

                case DataFeedType.VideoLikes:
                    return "Лайки к видео";

                case DataFeedType.MemberLikes:
                    return "Лайки";

                case DataFeedType.MemberShares:
                    return "Шеры";

                case DataFeedType.MemberSubscription:
                    return "Подписки";

                default:
                    throw new ArgumentOutOfRangeException("feedType");
            }
        }
    }
}
namespace Ix.Palantir.Vkontakte.Workflows.FeedProcessor
{
    using System;
    using API;
    using API.Responses.Videos;
    using DataAccess.API.Repositories;
    using DomainModel;
    using Logging;
    using Utilities;

    public class VideoFeedProcessor : IFeedProcessor
    {
        private readonly ILog log;
        private readonly IVkResponseMapper responseMapper;
        private readonly IVideoRepository videoRepository;
        private readonly IProcessingStrategy processingStrategy;

        public VideoFeedProcessor(IVkResponseMapper responseMapper, IVideoRepository videoRepository, IProcessingStrategy processingStrategy, ILog log)
        {
            this.log = log;
            this.responseMapper = responseMapper;
            this.videoRepository = videoRepository;
            this.processingStrategy = processingStrategy;
        }

        public void Process(DataFeed dataFeed, VkGroup group)
        {
            var feed = this.responseMapper.MapResponse<response>(dataFeed.Feed);

            foreach (var video in feed.video)
            {
                this.ProcessVideo(video, group);
            }
        }
        public void ProcessTerminator(int vkGroupId, int feedTypeVersion)
        {
        }

        private void ProcessVideo(responseVideo video, VkGroup group)
        {
            Video savedVideo = this.videoRepository.GetVideo(group.Id, video.vid);

            if (savedVideo != null)
            {
                this.log.DebugFormat("Video with VkId={0} is already in database", video.vid);
                return;
            }

            if (this.processingStrategy.IsLimitedProcessingEnabled(group.Id, DataFeedType.Video) &&
                video.date.FromUnixTimestamp().AddMonths(this.processingStrategy.GetMonthLimit()) < DateTime.UtcNow)
            {
                this.log.DebugFormat("Fetched video with VkId={0} is created more than {1} months ago. Skipping.", video.vid, this.processingStrategy.GetMonthLimit());
                return;
            }

            savedVideo = new Video
            {
                VkGroupId = group.Id,
                PostedDate = video.date.FromUnixTimestamp(),
                VkId = video.vid,
                Title = video.title,
                Description = video.description,
                Duration = int.Parse(video.duration)
            };

            this.videoRepository.Save(savedVideo);
            this.log.DebugFormat("Video with VkId={0} is not found in database. Saved with Id={1}", savedVideo.VkId, savedVideo.Id);
        }
    }
}
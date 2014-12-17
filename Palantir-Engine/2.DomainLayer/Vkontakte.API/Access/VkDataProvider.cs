namespace Ix.Palantir.Vkontakte.API.Access
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using Ix.Palantir.DomainModel;
    using Ix.Palantir.Utilities;
    using Ix.Palantir.Vkontakte.API;
    using Ix.Palantir.Vkontakte.API.Responses;
    using Ix.Palantir.Vkontakte.API.Responses.MemberInformation;
    using VkGroupType = Ix.Palantir.Vkontakte.API.VkGroupType;

    public class VkDataProvider : IVkDataProvider
    {
        private const string CONST_VkDomain = "http://vk.com";
        private const int CONST_CitiesPerQuery = 100;

        private readonly IVkAccessor vkAccessor;
        private readonly IHttpAccessor httpAccessor;
        private readonly IVkResponseMapper responseMapper;
        private readonly IVkDataLimits dataLimits;

        public VkDataProvider(IVkAccessor vkAccessor, IHttpAccessor httpAccessor, IVkResponseMapper responseMapper, IVkDataLimits dataLimits)
        {
            this.vkAccessor = vkAccessor;
            this.httpAccessor = httpAccessor;
            this.responseMapper = responseMapper;
            this.dataLimits = dataLimits;
        }

        public string GetUserProfile(long userId)
        {
            string profileXml = this.vkAccessor.ExecuteCall("getProfiles.xml", new Dictionary<string, string> { { "uid", userId.ToString() } });
            return profileXml;
        }
        public response GetUserProfiles(IEnumerable<string> userIds)
        {
            string groupMembers = this.vkAccessor.ExecuteCall("users.get.xml", new Dictionary<string, string> { { "uids", new SeparatedStringBuilder(",", userIds).ToString() }, { "fields", " uid, first_name, last_name, nickname, screen_name, sex, bdate, city, country, photo_rec, has_mobile, rate, education, universities, schools, relation, interests, movies, tv, books, games, about" }, { "name_case", "nom" } });
            response members = this.responseMapper.MapResponse<response>(groupMembers);
            members.Feed = groupMembers;

            return members;
        }
        public API.Responses.MembershipProfile.response GetGroupMembershipStatus(long? userId = null)
        {
            var parameters = new Dictionary<string, string> { { "extended", 1.ToString() } };

            if (userId.HasValue)
            {
                parameters.Add("uid", userId.ToString());
            }

            string profileXml = this.vkAccessor.ExecuteCall("getGroupsFull.xml", parameters);
            API.Responses.MembershipProfile.response profile = this.responseMapper.MapResponse<API.Responses.MembershipProfile.response>(profileXml);
            profile.Feed = profileXml;

            return profile;
        }
        public API.Responses.GroupDefinition.response GetGroup(string groupId)
        {
            API.Responses.GroupDefinition.response group = this.vkAccessor.ExecuteCall<API.Responses.GroupDefinition.response>("groups.getById.xml", new Dictionary<string, string> { { "gid", groupId } });
            return group;
        }
        public API.Responses.GroupMembers.response GetGroupMemberIds(string groupId, int offset)
        {
            string groupMembers = this.vkAccessor.ExecuteCall("groups.getMembers.xml", new Dictionary<string, string> { { "gid", groupId }, { "offset", offset.ToString() }, { "count", 1000.ToString() } });
            API.Responses.GroupMembers.response members = this.responseMapper.MapResponse<API.Responses.GroupMembers.response>(groupMembers);
            members.Feed = groupMembers;

            return members;
        }
        public API.Responses.GroupTopics.response GetTopics(string groupId, int offset)
        {
            var parameters = new Dictionary<string, string>
            {
                { "gid", groupId },
                { "order", 2.ToString() }, // Order by creation time
                { "offset", offset.ToString() },
                { "count", 100.ToString() },
            };

            string topicsXml = this.vkAccessor.ExecuteCall("board.getTopics.xml", parameters);
            API.Responses.GroupTopics.response topics = this.responseMapper.MapResponse<API.Responses.GroupTopics.response>(topicsXml);
            topics.Feed = topicsXml;

            return topics;
        }
        public API.Responses.GroupTopicComments.response GetTopicComments(string groupId, string topicId, int offset)
        {
            var parameters = new Dictionary<string, string>
            {
                { "gid", groupId },
                { "tid", topicId },
                { "extended", "1" },
                { "offset", offset.ToString() },
                { "count", 100.ToString() },
            };

            string topicsXml = this.vkAccessor.ExecuteCall("board.getComments.xml", parameters);
            API.Responses.GroupTopicComments.response topics = this.responseMapper.MapResponse<API.Responses.GroupTopicComments.response>(topicsXml);
            topics.Feed = topicsXml;

            return topics;
        }
        public API.Responses.NewsFeed.response GetNewsFeed(string groupName, DateTime? lowerBound = null, DateTime? upperBound = null)
        {
            string sourceId = "g" + groupName; // this.GetSourceId(groupNames);
            Dictionary<string, string> parameters = new Dictionary<string, string> { { "source_ids", sourceId }, { "filters", "post" } };

            if (lowerBound != null)
            {
                parameters.Add("start_time", lowerBound.Value.ToUnixTimestamp().ToString());
            }

            if (upperBound != null)
            {
                parameters.Add("end_time", upperBound.Value.ToUnixTimestamp().ToString());
            }

            parameters.Add("count", "100");

            API.Responses.NewsFeed.response newsFeed = this.vkAccessor.ExecuteCall<API.Responses.NewsFeed.response>("newsfeed.get.xml", parameters);
            return newsFeed;
        }

        public API.Responses.WallPosts.response GetWallPosts(string userId, int offset)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string> { { "owner_id", "-" + userId }, { "filter", "all" }, { "count", "100" }, { "offset", offset.ToString() } };
            string wallContent = this.vkAccessor.ExecuteCall("wall.get.xml", parameters);
            API.Responses.WallPosts.response wall = this.responseMapper.MapResponse<API.Responses.WallPosts.response>(wallContent);
            wall.Feed = wallContent;
            return wall;
        }

        public API.Responses.WallPostComments.response GetWallPostComments(string wallPostId, string userId, int offset)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string> { { "owner_id", "-" + userId }, { "post_id", wallPostId }, { "count", "100" }, { "offset", offset.ToString() } };
            string postCommentsContent = this.vkAccessor.ExecuteCall("wall.getComments.xml", parameters);
            API.Responses.WallPostComments.response wallPostComments = this.responseMapper.MapResponse<API.Responses.WallPostComments.response>(postCommentsContent);
            wallPostComments.Feed = postCommentsContent;
            return wallPostComments;
        }

        public API.Responses.Photos.response GetPhotos(string groupId, int offset)
        {
            // http://vk.com/developers.php?oid=-1&p=photos.get
            Dictionary<string, string> parameters = new Dictionary<string, string> { { "owner_id", "-" + groupId }, { "offset", offset.ToString() }, { "count", "100" } };
            string photosContent = this.vkAccessor.ExecuteCall("photos.getAll.xml", parameters);
            API.Responses.Photos.response photos = this.responseMapper.MapResponse<API.Responses.Photos.response>(photosContent);
            photos.Feed = photosContent;

            return photos;
        }

        public API.Responses.PhotoAlbumDetailsResponse.response GetPhotoAlbumDetails(string groupId, string vkAlbumId, int offset)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string> { { "gid", groupId }, { "aid", vkAlbumId }, { "extended", 1.ToString() }, { "offset", offset.ToString() } };
            string photosContent = this.vkAccessor.ExecuteCall("photos.get.xml", parameters);

            var photos = this.responseMapper.MapResponse<API.Responses.PhotoAlbumDetailsResponse.response>(photosContent, true);
            photos.Feed = photosContent;
            return photos;
        }

        public API.Responses.Videos.response GetVideos(string groupId, int offset)
        {
            // http://vk.com/developers.php?oid=-1&p=video.getComments
            // http://vk.com/developers.php?oid=-1&p=likes.getList
            Dictionary<string, string> parameters = new Dictionary<string, string> { { "gid", groupId }, { "offset", offset.ToString() }, { "count", "200" } };
            string videosContent = this.vkAccessor.ExecuteCall("video.get.xml", parameters);
            API.Responses.Videos.response videos = this.responseMapper.MapResponse<API.Responses.Videos.response>(videosContent);
            videos.Feed = videosContent;

            return videos;
        }

        public API.Responses.VideoComments.response GetVideoComments(string groupId, string videoId, int offset)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string> { { "owner_id", "-" + groupId }, { "vid", videoId }, { "offset", offset.ToString() }, { "allow_group_comments", "1" }, { "count", "100" } };
            string videoCommentsContent = this.vkAccessor.ExecuteCall("video.getComments.xml", parameters);
            API.Responses.VideoComments.response videoComments = this.responseMapper.MapResponse<API.Responses.VideoComments.response>(videoCommentsContent);
            videoComments.Feed = videoCommentsContent;

            return videoComments;
        }

        public Responses.LikeShareFeed.response GetLikes(string groupId, string relatedObjectId, LikeShareType type, int offsetCounter)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string> { { "owner_id", "-" + groupId }, { "item_id", relatedObjectId }, { "type", type.ToString().ToLower() }, { "filter", "likes" }, { "friends_only", "0" }, { "offset", offsetCounter.ToString() }, { "count", "1000" } };
            string likesList = this.vkAccessor.ExecuteCall("likes.getList.xml", parameters);
            Ix.Palantir.Vkontakte.API.Responses.LikeShareFeed.response likes = this.responseMapper.MapResponse<Ix.Palantir.Vkontakte.API.Responses.LikeShareFeed.response>(likesList);
            likes.Feed = likesList;

            return likes;
        }

        public Responses.LikeShareFeed.response GetShares(string groupId, string relatedObjectId, LikeShareType type, int offsetCounter)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string> { { "owner_id", "-" + groupId }, { "item_id", relatedObjectId }, { "type", type.ToString().ToLower() }, { "filter", "copies" }, { "friends_only", "0" }, { "offset", offsetCounter.ToString() }, { "count", "1000" } };
            string sharesList = this.vkAccessor.ExecuteCall("likes.getList.xml", parameters);
            Ix.Palantir.Vkontakte.API.Responses.LikeShareFeed.response shares = this.responseMapper.MapResponse<Ix.Palantir.Vkontakte.API.Responses.LikeShareFeed.response>(sharesList);
            shares.Feed = sharesList;

            return shares;
        }

        public Ix.Palantir.Vkontakte.API.Responses.MemberSubscription.response GetMemberSubscriptions(string userId, int offsetCounter)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string> { { "uid", userId }, { "extended", "1" }, { "offset", offsetCounter.ToString() }, { "count", this.dataLimits.MemberSubscriptionLimits.ToString() } };
            string groupList = this.vkAccessor.ExecuteCall("users.getSubscriptions.xml", parameters);

            Ix.Palantir.Vkontakte.API.Responses.MemberSubscription.response subscriptions = this.responseMapper.MapResponse<Ix.Palantir.Vkontakte.API.Responses.MemberSubscription.response>(groupList);
            subscriptions.Feed = groupList;

            return subscriptions;
        }

        public string GetPostStats(string groupId, string postId, DateTime from, DateTime to)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string> { { "gid", groupId }, { "post_id", postId }, { "date_from", from.ToString("yyyy-MM-dd") }, { "date_to", to.ToString("yyyy-MM-dd") } };
            string postStats = this.vkAccessor.ExecuteCall("stats.getPostStats", parameters);
            
            return postStats;
        }

        public AdminsResponse GetGroupAministrators(string groupId, VkGroupType groupType)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string> { { "act", "a_get_contacts" }, { "al", "1" }, { "oid", groupId } };
            string page = "/al_page.php";

            string adminsPage = this.httpAccessor.GetPageByUriViaPost(CONST_VkDomain + page, parameters.ToUrlFormat());
            AdminsResponse response = new AdminsResponse(adminsPage);

            return response;
            }

        public API.Responses.Countries.response GetCountries()
        {
            Dictionary<string, string> parameters = new Dictionary<string, string> { { "need_full", string.Empty } };
            string countriesResponse = this.vkAccessor.ExecuteCall("places.getCountries.xml", parameters);
            API.Responses.Countries.response countries = this.responseMapper.MapResponse<API.Responses.Countries.response>(countriesResponse);
            countries.Feed = countriesResponse;

            return countries;
        }
        public API.Responses.Cities.response GetCities(int offset)
        {
            if (offset == int.MaxValue)
            {
                throw new ArgumentOutOfRangeException("offset", "offset must be lower than Int32.MaxValue");
            }

            IEnumerable<int> cityIdsInt = Enumerable.Range(offset + 1, CONST_CitiesPerQuery);
            var cityIds = new SeparatedStringBuilder(cityIdsInt.Select(i => i.ToString()));
            Dictionary<string, string> parameters = new Dictionary<string, string> { { "cids", cityIds.ToString() } };
            string citiesResponse = this.vkAccessor.ExecuteCall("places.getCityById.xml", parameters);
            API.Responses.Cities.response cities = this.responseMapper.MapResponse<API.Responses.Cities.response>(citiesResponse);
            cities.Feed = citiesResponse;

            return cities;
        }
    }
}
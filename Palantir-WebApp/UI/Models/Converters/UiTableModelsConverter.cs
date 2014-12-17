namespace Ix.Palantir.UI.Models.Converters
{
    using Metrics;
    using Services.API;
    using Shared;

    public class UiTableModelsConverter
    {
        public MostPopularPostModel CreatePostModel(PostEntityInfo post)
        {
            var postLink = new UiLink { Url = post.Url, Text = post.Title, Target = "_blank" };

            return new MostPopularPostModel
            {
                Id = post.Id,
                PostedDate = post.PostedDate,
                PostValue = postLink,
                LikesCount = post.LikesCount,
                CommentsCount = post.CommentsCount,
                ShareCount = post.ShareCount,
                TotalCount = post.LikesAndCommentsAndShareCount
            };
        }

        public MostPopularContentModel CreateContentModel(ContentEntityInfo content)
        {
            var postLink = new UiLink { Url = content.Url, Text = content.Title, Target = "_blank" };

            return new MostPopularContentModel
            {
                Id = content.Id,
                PostedDate = content.PostedDate,
                Value = postLink,
                Type = content.IsVideo ? "Видео" : "Фото",
                LikesCount = content.LikesCount,
                CommentsCount = content.CommentsCount,
                ShareCount = content.ShareCount,
                TotalCount = content.LikesAndCommentsAndShareCount
            };
        }
    }
}
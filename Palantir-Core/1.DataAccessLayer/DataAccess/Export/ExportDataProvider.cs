namespace Ix.Palantir.DataAccess.Export
{
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using Ix.Palantir.DataAccess.API.Export;
    using Ix.Palantir.DataAccess.API.StatisticsProviders;
    using Ix.Palantir.DomainModel;
    using Ix.Palantir.DomainModel.Extensions;
    using Ix.Palantir.Querying.Common;
    using Ix.Palantir.Utilities;
    using OfficeOpenXml;

    public class ExportDataProvider : IExportDataProvider
    {
        private const string CONST_DateTimePattern = "yyyy-MM-dd HH:mm";
        private readonly IRawDataProvider dataProvider;
        private readonly IStatisticsProvider statisticsProvider;
        private readonly IIrChartDataProvider irChartDataProvider;

        private int vkGroupId;
        private DateRange dateRange;

        public ExportDataProvider(IRawDataProvider dataProvider, IStatisticsProvider statisticsProvider, IIrChartDataProvider irChartDataProvider)
        {
            this.dataProvider = dataProvider;
            this.statisticsProvider = statisticsProvider;
            this.irChartDataProvider = irChartDataProvider;
        }

        public byte[] ExportToXlsx(int vkGroupId, DateRange dateRange)
        {
            Contract.Requires(dateRange != null);

            this.vkGroupId = vkGroupId;
            this.dateRange = dateRange;

            var package = new ExcelPackage();
            var membersSheet = package.Workbook.Worksheets.Add("Участники");
            var postsSheet = package.Workbook.Worksheets.Add("Посты");
            var videoSheet = package.Workbook.Worksheets.Add("Видео");
            var photoSheet = package.Workbook.Worksheets.Add("Фото");
            var irSheet = package.Workbook.Worksheets.Add("Вовлеченность участников");
            var interestSheet = package.Workbook.Worksheets.Add("Интересы участников");

            this.FillMembersSheet(membersSheet);
            this.FillPostsSheet(postsSheet);
            this.FillVideoSheet(videoSheet);
            this.FillPhotoSheet(photoSheet);
            this.FillIrSheet(irSheet);
            this.FillInterestsSheet(interestSheet);

            var byteArray = package.GetAsByteArray();
            return byteArray;
        }

        private void FillPostsSheet(ExcelWorksheet postsSheet)
        {
            int headerStartRow = 2;

            postsSheet.Cells[1, 1].Value = this.FormatPeriodString("Посты за период c {0} по {1}", this.dateRange);

            postsSheet.Cells[headerStartRow, 1].Value = "Id";
            postsSheet.Cells[headerStartRow, 2].Value = "Id автора";
            postsSheet.Cells[headerStartRow, 3].Value = "Дата/Время размещения";
            postsSheet.Cells[headerStartRow, 4].Value = "Текст";
            postsSheet.Cells[headerStartRow, 5].Value = "Количество комментариев";
            postsSheet.Cells[headerStartRow, 6].Value = "Количество лайков";
            postsSheet.Cells[headerStartRow, 7].Value = "Количество репостов";

            IList<Post> posts = this.dataProvider.GetPosts(this.vkGroupId, this.dateRange);

            if (posts.Count == 0)
            {
                return;
            }

            for (int i = headerStartRow + 1; i < posts.Count + 1; i++)
            {
                var post = posts[i - 2];

                postsSheet.Cells[i, 1].Value = int.Parse(post.VkId);
                postsSheet.Cells[i, 2].Value = post.CreatorId;
                postsSheet.Cells[i, 3].Value = post.PostedDate.ToString(CONST_DateTimePattern);
                postsSheet.Cells[i, 4].Value = post.Text;
                postsSheet.Cells[i, 5].Value = post.CommentsCount;
                postsSheet.Cells[i, 6].Value = post.LikesCount;
                postsSheet.Cells[i, 7].Value = post.SharesCount;
            }

            this.SetAutofitToColumn(postsSheet, 7);
            this.SetColumnWidth(postsSheet, 1);
        }
        private void FillMembersSheet(ExcelWorksheet membersSheet)
        {
            int headerStartRow = 1;
            membersSheet.Cells[headerStartRow, 1].Value = "Id";
            membersSheet.Cells[headerStartRow, 2].Value = "Имя";
            membersSheet.Cells[headerStartRow, 3].Value = "Пол";
            membersSheet.Cells[headerStartRow, 4].Value = "Семейное положение";
            membersSheet.Cells[headerStartRow, 5].Value = "Год рождения";
            membersSheet.Cells[headerStartRow, 6].Value = "Месяц рождения";
            membersSheet.Cells[headerStartRow, 7].Value = "День рождения";
            membersSheet.Cells[headerStartRow, 8].Value = "Страна";
            membersSheet.Cells[headerStartRow, 9].Value = "Город";
            membersSheet.Cells[headerStartRow, 10].Value = "Образование";
            membersSheet.Cells[headerStartRow, 11].Value = "Статус";
            membersSheet.Cells[headerStartRow, 12].Value = "Интересы";
            membersSheet.Cells[headerStartRow, 13].Value = "Количество постов";
            membersSheet.Cells[headerStartRow, 14].Value = "Количество комментариев";
            membersSheet.Cells[headerStartRow, 15].Value = "Количество лайков";
            membersSheet.Cells[headerStartRow, 16].Value = "Количество репостов";

            IList<Member> members = this.dataProvider.GetMembers(this.vkGroupId);
            var membersLikes = this.dataProvider.GetMemberStatInfo(this.vkGroupId, "memberlike", "vkmemberid").ToDictionary(m => m.CreatorId);
            var membersPosts = this.dataProvider.GetMemberStatInfo(this.vkGroupId, "post", "creatorid").ToDictionary(m => m.CreatorId);
            var membersShares = this.dataProvider.GetMemberStatInfo(this.vkGroupId, "membershare", "vkmemberid").ToDictionary(m => m.CreatorId);
            var membersComments = this.dataProvider.GetMemberStatInfo(this.vkGroupId, "postcomment", "creatorid").ToDictionary(m => m.CreatorId);

            if (members.Count == 0)
            {
                return;
            }

            IDictionary<int, Country> countries = this.dataProvider.GetCountries();
            IDictionary<int, City> cities = this.dataProvider.GetCities();

            for (int i = headerStartRow + 1; i < members.Count + 1; i++)
            {
                var member = members[i - 2];

                membersSheet.Cells[i, 1].Value = int.Parse(member.VkId);
                membersSheet.Cells[i, 2].Value = member.Name;
                membersSheet.Cells[i, 3].Value = member.Gender.GetLabel();
                membersSheet.Cells[i, 4].Value = member.MaritalStatus.GetLabel();
                membersSheet.Cells[i, 5].Value = member.BirthDate.BirthYear;
                membersSheet.Cells[i, 6].Value = member.BirthDate.BirthMonth;
                membersSheet.Cells[i, 7].Value = member.BirthDate.BirthDay;
                membersSheet.Cells[i, 8].Value = countries.ContainsKey(member.CountryId) ? countries[member.CountryId].Title : "Неизвестно";
                membersSheet.Cells[i, 9].Value = cities.ContainsKey(member.CityId) ? cities[member.CityId].Title : "Неизвестно";
                membersSheet.Cells[i, 10].Value = member.Education.GetLabel();
                membersSheet.Cells[i, 11].Value = member.Status.GetLabel();
                membersSheet.Cells[i, 12].Value = new SeparatedStringBuilder(", ", member.Interests.Select(mi => mi.Title));
                membersSheet.Cells[i, 13].Value = membersPosts.ContainsKey(member.VkMemberId) ? membersPosts[member.VkMemberId].EntityCount : 0;
                membersSheet.Cells[i, 14].Value = membersComments.ContainsKey(member.VkMemberId) ? membersComments[member.VkMemberId].EntityCount : 0;
                membersSheet.Cells[i, 15].Value = membersLikes.ContainsKey(member.VkMemberId) ? membersLikes[member.VkMemberId].EntityCount : 0;
                membersSheet.Cells[i, 16].Value = membersShares.ContainsKey(member.VkMemberId) ? membersShares[member.VkMemberId].EntityCount : 0;
            }

            this.SetAutofitToColumn(membersSheet, 16);
        }

        private void FillVideoSheet(ExcelWorksheet videoSheet)
        {
            int headerStartRow = 2;
            videoSheet.Cells[1, 1].Value = this.FormatPeriodString("c {0} по {1}", this.dateRange);

            videoSheet.Cells[headerStartRow, 1].Value = "Id";
            videoSheet.Cells[headerStartRow, 2].Value = "Дата/Время размещения";
            videoSheet.Cells[headerStartRow, 3].Value = "Название";
            videoSheet.Cells[headerStartRow, 4].Value = "Описание";
            videoSheet.Cells[headerStartRow, 5].Value = "Продолжительность (сек)";
            videoSheet.Cells[headerStartRow, 6].Value = "Количество лайков";
            videoSheet.Cells[headerStartRow, 7].Value = "Количество комментариев";
            videoSheet.Cells[headerStartRow, 8].Value = "Количество репостов";

            IList<Video> videos = this.dataProvider.GetVideos(this.vkGroupId, this.dateRange);

            if (videos.Count == 0)
            {
                return;
            }

            for (int i = headerStartRow + 1; i < videos.Count + 1; i++)
            {
                var video = videos[i - 2];

                videoSheet.Cells[i, 1].Value = int.Parse(video.VkId);
                videoSheet.Cells[i, 2].Value = video.PostedDate.ToString(CONST_DateTimePattern);
                videoSheet.Cells[i, 3].Value = video.Title;
                videoSheet.Cells[i, 4].Value = video.Description;
                videoSheet.Cells[i, 5].Value = video.Duration;
                videoSheet.Cells[i, 6].Value = video.LikesCount;
                videoSheet.Cells[i, 7].Value = video.CommentsCount;
                videoSheet.Cells[i, 8].Value = video.ShareCount;
            }

            this.SetAutofitToColumn(videoSheet, 8);
        }
        private void FillPhotoSheet(ExcelWorksheet photoSheet)
        {
            int headerStartRow = 2;
            photoSheet.Cells[1, 1].Value = this.FormatPeriodString("c {0} по {1}", this.dateRange);

            photoSheet.Cells[headerStartRow, 1].Value = "Id";
            photoSheet.Cells[headerStartRow, 2].Value = "Id альбома";
            photoSheet.Cells[headerStartRow, 3].Value = "Дата/Время размещения";
            photoSheet.Cells[headerStartRow, 4].Value = "Количество лайков";
            photoSheet.Cells[headerStartRow, 5].Value = "Количество комментариев";
            photoSheet.Cells[headerStartRow, 6].Value = "Количество репостов";

            IList<Photo> photos = this.dataProvider.GetPhotos(this.vkGroupId, this.dateRange);

            if (photos.Count == 0)
            {
                return;
            }

            for (int i = headerStartRow + 1; i < photos.Count + 1; i++)
            {
                var photo = photos[i - 2];

                photoSheet.Cells[i, 1].Value = int.Parse(photo.VkId);
                photoSheet.Cells[i, 2].Value = int.Parse(photo.AlbumId);
                photoSheet.Cells[i, 3].Value = photo.PostedDate.ToString(CONST_DateTimePattern);
                photoSheet.Cells[i, 4].Value = photo.LikesCount;
                photoSheet.Cells[i, 5].Value = photo.CommentsCount;
                photoSheet.Cells[i, 6].Value = photo.ShareCount;
            }

            this.SetAutofitToColumn(photoSheet, 6);
        }

        private void FillInterestsSheet(ExcelWorksheet interestSheet)
        {
            int headerStartRow = 1;
            interestSheet.Cells[headerStartRow, 1].Value = "Интерес";
            interestSheet.Cells[headerStartRow, 2].Value = "Количество";

            IList<MemberInterestsGroupedObject> interests = this.statisticsProvider.GetMemberInterest(this.vkGroupId);

            if (interests.Count == 0)
            {
                return;
            }

            for (int i = headerStartRow + 1; i < interests.Count + 1; i++)
            {
                var interest = interests[i - 2];

                interestSheet.Cells[i, 1].Value = interest.Title;
                interestSheet.Cells[i, 2].Value = interest.Count;
            }

            this.SetAutofitToColumn(interestSheet, 2);
        }

        private void FillIrSheet(ExcelWorksheet irSheet)
        {
            int headerStartRow = 2;
            irSheet.Cells[1, 1].Value = this.FormatPeriodString("Interaction Rate за период c {0} по {1}", this.dateRange);
            irSheet.Cells[1, 8].Value = this.FormatPeriodString("Количество реакций по типам постов за период с {0} по {1}", this.dateRange);

            irSheet.Cells[headerStartRow, 1].Value = "Дата";
            irSheet.Cells[headerStartRow, 2].Value = "Лайков";
            irSheet.Cells[headerStartRow, 3].Value = "Комментариев";
            irSheet.Cells[headerStartRow, 4].Value = "Репостов";
            irSheet.Cells[headerStartRow, 5].Value = "Постов";
            irSheet.Cells[headerStartRow, 6].Value = "Пользователей";

            irSheet.Cells[headerStartRow, 9].Value = "Лайков";
            irSheet.Cells[headerStartRow, 10].Value = "Комментариев";
            irSheet.Cells[headerStartRow, 11].Value = "Репостов";
            irSheet.Cells[headerStartRow, 12].Value = "Количество постов";

            irSheet.Cells[3, 8].Value = "Тексты";
            irSheet.Cells[4, 8].Value = "Фото";
            irSheet.Cells[5, 8].Value = "Видео";

            int[] reactionsPostData = this.statisticsProvider.GetPostReactionCount(this.vkGroupId, this.dateRange);
            irSheet.Cells[3, 9].Value = reactionsPostData[0];
            irSheet.Cells[3, 10].Value = reactionsPostData[1];
            irSheet.Cells[3, 11].Value = reactionsPostData[2];

            int[] reactionsPhotoData = this.statisticsProvider.GetPhotoReactionCount(this.vkGroupId, this.dateRange);
            irSheet.Cells[4, 9].Value = reactionsPhotoData[0];
            irSheet.Cells[4, 10].Value = reactionsPhotoData[1];
            irSheet.Cells[4, 11].Value = reactionsPhotoData[2];

            int[] reactionsVideoData = this.statisticsProvider.GetVideoReactionCount(this.vkGroupId, this.dateRange);
            irSheet.Cells[5, 9].Value = reactionsVideoData[0];
            irSheet.Cells[5, 10].Value = reactionsVideoData[1];
            irSheet.Cells[5, 11].Value = reactionsVideoData[2];

            var irData = this.irChartDataProvider.GetInteractionRate(this.vkGroupId, this.dateRange, Periodicity.ByDay).ToList();

            if (!irData.Any())
            {
                return;
            }

            var likes = irData[2].ToList();
            var comments = irData[1].ToList();
            var shares = irData[3].ToList();

            this.DisplayDatesListInColumn(irSheet, likes, 1, 3);
            this.DisplayListInColumn(irSheet, likes, 2, 3);
            this.DisplayListInColumn(irSheet, comments, 3, 3);
            this.DisplayListInColumn(irSheet, shares, 4, 3);

            this.SetAutofitToColumn(irSheet, 11);
            this.SetColumnWidth(irSheet, 1);
        }

        private void SetAutofitToColumn(ExcelWorksheet membersSheet, int columnNumber)
        {
            for (int i = 1; i < columnNumber + 1; i++)
            {
                membersSheet.Column(i).AutoFit();
            }
        }

        private void SetColumnWidth(ExcelWorksheet membersSheet, int columnNumber, int width = 150)
        {
            membersSheet.Column(columnNumber).Width = width;
        }

        private string FormatPeriodString(string part, DateRange dr)
        {
            return string.Format(part, dr.From.ToShortDateString(), dr.To.ToShortDateString());
        }

        private void DisplayListInColumn(ExcelWorksheet irSheet, IList<PointInTime> points, int columnNum, int start_rowNum)
        {
            for (int i = 0; i < points.Count; i++)
            {
                irSheet.Cells[start_rowNum + i, columnNum].Value = points[i].Value;
            }
        }

        private void DisplayDatesListInColumn(ExcelWorksheet irSheet, IList<PointInTime> points, int columnNum, int start_rowNum)
        {
            for (int i = 0; i < points.Count; i++)
            {
                irSheet.Cells[start_rowNum + i, columnNum].Value = points[i].Date.ToShortDateString();
            }
        }
    }
}
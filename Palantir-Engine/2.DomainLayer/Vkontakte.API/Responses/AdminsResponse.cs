namespace Ix.Palantir.Vkontakte.API.Responses
{
    using System.Collections.Generic;
    using System.Text.RegularExpressions;

    public class AdminsResponse
    {
        private static readonly Regex adminsRegex = new Regex("href=\"/write(\\d+)\"", RegexOptions.Singleline | RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Compiled);

        private readonly string page;

        public AdminsResponse(string page)
        {
            this.page = page;
        }

        public string Page
        {
            get
            {
                return this.page;
            }
        }
        public IList<long> AdminIds
        {
            get
            {
                var adminIds = new List<long>();

                Match matchResult = adminsRegex.Match(this.page);

                while (matchResult.Success)
                {
                    adminIds.Add(long.Parse(matchResult.Groups[1].Value));
                    matchResult = matchResult.NextMatch();
                } 

                return adminIds;
            }
        }
    }
}
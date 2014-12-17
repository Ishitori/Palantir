namespace Ix.Palantir.UI.ModelBuilders
{
    public class ActiveUserJsonModelBuilderOptions
    {
        public ActiveUserJsonModelBuilderOptions(int userTableCount, int reportCount, int interestCount)
        {
            this.UserTableCount = userTableCount;
            this.ReportCount = reportCount;
            this.InterestCount = interestCount;
        }

        public static ActiveUserJsonModelBuilderOptions Default
        {
            get
            {
                return new ActiveUserJsonModelBuilderOptions(50, 50, 45);
            }
        }

        public int UserTableCount { get; set; }
        public int ReportCount { get; set; }
        public int InterestCount { get; set; }
    }
}
namespace Ix.Palantir.DataAccess.API.Repositories.Changes
{
    public class MemberChangeContext
    {
        public static MemberChangeContext Default
        {
            get
            {
                return new MemberChangeContext
                {
                    IsMemberChanged = true,
                    IsMemberInterestsChanged = true
                };
            }
        }

        public bool IsMemberChanged { get; set; }
        public bool IsMemberInterestsChanged { get; set; }
    }
}
namespace Ix.Palantir.DataAccess.API.StatisticsProviders.DTO
{
    using System;
    using System.Collections.Generic;

    [Serializable]
    public class AudienceFilteringResult
    {
        public AudienceFilteringResult()
        {
        }
        public AudienceFilteringResult(long code)
        {
            this.Code = code;
        }
        public AudienceFilteringResult(IList<MemberMainInfo> infos)
        {
            this.Members = infos;
        }

        public virtual long Code { get; set; }
        public virtual bool IsAllMembers { get; set; }
        public virtual IList<MemberMainInfo> Members { get; set; }
    }
}
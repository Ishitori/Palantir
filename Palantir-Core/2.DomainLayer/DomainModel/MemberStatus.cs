namespace Ix.Palantir.DomainModel
{
    using System;

    [Serializable]
    public enum MemberStatus
    {
        Active = 0,
        Blocked = 1,
        Deleted = 2,
        Bot = 3
    }
}
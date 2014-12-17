namespace Ix.Palantir.DomainModel
{
    using System;

    [Serializable]
    public enum MemberInterestType : int
    {
        General = 0,
        Movie = 1,
        TVShow = 2,
        Book = 3,
        VideoGame = 4
    }
}
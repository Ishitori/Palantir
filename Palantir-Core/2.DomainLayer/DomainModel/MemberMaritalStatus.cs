namespace Ix.Palantir.DomainModel
{
    using System;

    [Serializable]
    public enum MemberMaritalStatus
    {
        Unknown = 0,
        Single = 1, // 1 - не женат/не замужем 2 - есть друг/есть подруга, 3 - помолвлен/помолвлена,   7 - влюблён/влюблена  5 - всё сложно, 6 - в активном поиске 
        Married = 2 // 4 - женат/замужем 
    }
}
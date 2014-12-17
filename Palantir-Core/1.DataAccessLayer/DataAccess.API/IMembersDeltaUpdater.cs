namespace Ix.Palantir.DataAccess
{
    using System;

    public interface IMembersDeltaUpdater
    {
        void CalculateMembersDelta(int vkGroupId, DateTime date);
    }
}
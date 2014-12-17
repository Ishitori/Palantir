namespace Ix.Palantir.LockServer.API
{
    using System;

    public interface ILockServer
    {
        T LockSection<T>(string initKey, Func<T> section);
        void LockSection(string initKey, Action section);
    }
}
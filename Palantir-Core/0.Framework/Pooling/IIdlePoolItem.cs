namespace Ix.Palantir.Pooling
{
    using System;

    public interface IIdlePoolItem
    {
        DateTime? LastUsedTime { get; set; }
        void DisposeItem();
    }
}
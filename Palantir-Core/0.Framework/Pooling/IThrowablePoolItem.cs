namespace Ix.Palantir.Pooling
{
    public interface IThrowablePoolItem
    {
        bool CanReuseItem { get; set; }
    }
}
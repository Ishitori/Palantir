namespace Ix.Palantir.Pooling.Storage
{
    public interface IItemStore<T>
    {
        int Count { get; }

        T Fetch();
        T Peek();
        void Store(T item);
    }
}
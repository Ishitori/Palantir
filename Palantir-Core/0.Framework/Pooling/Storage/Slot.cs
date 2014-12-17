namespace Ix.Palantir.Pooling.Storage
{
    public class Slot<T>
    {
        public Slot(T item)
        {
            this.Item = item;
        }

        public T Item { get; private set; }
        public bool IsInUse { get; set; }
    }
}
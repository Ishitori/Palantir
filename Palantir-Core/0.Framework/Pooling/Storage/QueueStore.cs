namespace Ix.Palantir.Pooling.Storage
{
    using System.Collections.Generic;

    public class QueueStore<T> : Queue<T>, IItemStore<T>
    {
        public QueueStore(int capacity) : base(capacity)
        {
        }

        public T Fetch()
        {
            return this.Dequeue();
        }
        public void Store(T item)
        {
            this.Enqueue(item);
        }
    }
}
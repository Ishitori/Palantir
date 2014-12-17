namespace Ix.Palantir.Pooling.Storage
{
    using System.Collections.Generic;

    public class StackStore<T> : Stack<T>, IItemStore<T>
    {
        public StackStore(int capacity) : base(capacity)
        {
        }

        public T Fetch()
        {
            return this.Pop();
        }
        public void Store(T item)
        {
            this.Push(item);
        }
    }
}
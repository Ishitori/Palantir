namespace Ix.Palantir.Pooling
{
    using System;
    using System.Diagnostics;
    using System.Threading;
    using Ix.Palantir.Exceptions;
    using Ix.Palantir.Pooling.Storage;

    public class Pool<T> : IDisposable
    {
        private const int CONST_MaxWaitTime = 10000;
        private readonly int idleTimeoutInSec;

        private readonly Func<Pool<T>, T> factory;
        private readonly IItemStore<T> itemStore;
        private readonly LoadingMode loadingMode;
        private readonly int size;
        private readonly Semaphore sync;
        private int count;
        private bool isDisposed;

        public Pool(int size, Func<Pool<T>, T> factory) : this(size, factory, LoadingMode.Lazy, AccessMode.FIFO, idleTimeoutInSec: 600)
        {
        }
        public Pool(int size, Func<Pool<T>, T> factory, LoadingMode loadingMode, AccessMode accessMode, int idleTimeoutInSec)
        {
            if (size <= 0)
            {
                throw new ArgumentOutOfRangeException("size", size, "Argument 'size' must be greater than zero.");
            }
            if (factory == null)
            {
                throw new ArgumentNullException("factory");
            }

            this.size = size;
            this.factory = factory;
            this.sync = new Semaphore(size, size);
            this.loadingMode = loadingMode;
            this.idleTimeoutInSec = idleTimeoutInSec;
            this.itemStore = this.CreateItemStore(accessMode, size);

            if (loadingMode == LoadingMode.Eager)
            {
                this.PreloadItems();
            }
        }

        public bool IsDisposed
        {
            get { return this.isDisposed; }
        }

        public T Acquire()
        {
            if (!this.sync.WaitOne(CONST_MaxWaitTime))
            {
                throw new PalantirException(string.Format("Cannot acquire item from pool for maximum allowed time {0}ms. That means that pool limit of {1} items is reached and every item is in use", CONST_MaxWaitTime, this.size));
            }

            switch (this.loadingMode)
            {
                case LoadingMode.Eager:
                    return this.AcquireEager();

                case LoadingMode.Lazy:
                    return this.AcquireLazy();

                default:
                    Debug.Assert(this.loadingMode == LoadingMode.LazyExpanding, "Unknown LoadingMode encountered in Acquire method.");
                    return this.AcquireLazyExpanding();
            }
        }
        public void Release(T item)
        {
            this.SetLastUsedTime(item);
            
            if (this.CanReuseItem(item))
            {
                lock (this.itemStore)
                {
                    this.itemStore.Store(item);
                }
            }

            this.ExpireIdleItems();
            this.sync.Release();
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool isDisposing)
        {
            if (this.isDisposed)
            {
                return;
            }

            this.isDisposed = true;

            if (typeof(IDisposable).IsAssignableFrom(typeof(T)))
            {
                lock (this.itemStore)
                {
                    while (this.itemStore.Count > 0)
                    {
                        var disposable = (IDisposable)this.itemStore.Fetch();
                        disposable.Dispose();
                    }
                }
            }

            this.sync.Close();
        }

        private T AcquireEager()
        {
            lock (this.itemStore)
            {
                return this.itemStore.Fetch();
            }
        }
        private T AcquireLazy()
        {
            lock (this.itemStore)
            {
                if (this.itemStore.Count > 0)
                {
                    return this.itemStore.Fetch();
                }
            }
            Interlocked.Increment(ref this.count);
            return this.factory(this);
        }
        private T AcquireLazyExpanding()
        {
            bool shouldExpand = false;
            if (this.count < this.size)
            {
                int newCount = Interlocked.Increment(ref this.count);
                if (newCount <= this.size)
                {
                    shouldExpand = true;
                }
                else
                {
                    // Another thread took the last spot - use the store instead
                    Interlocked.Decrement(ref this.count);
                }
            }
            if (shouldExpand)
            {
                return this.factory(this);
            }

            lock (this.itemStore)
            {
                return this.itemStore.Fetch();
            }
        }

        private void PreloadItems()
        {
            for (int i = 0; i < this.size; i++)
            {
                T item = this.factory(this);
                this.itemStore.Store(item);
            }
            this.count = this.size;
        }
        private IItemStore<T> CreateItemStore(AccessMode mode, int capacity)
        {
            switch (mode)
            {
                case AccessMode.FIFO:
                    return new QueueStore<T>(capacity);

                default:
                    throw new ArgumentException("Access mode is not supported", "mode");

/*
                case AccessMode.LIFO:
                    return new StackStore<T>(capacity);

                default:
                    Debug.Assert(mode == AccessMode.Circular, "Invalid AccessMode in CreateItemStore");
                    return new CircularStore<T>(capacity);
*/
            }
        }
        private void SetLastUsedTime(T item)
        {
            IIdlePoolItem poolItem = item as IIdlePoolItem;

            if (poolItem != null)
            {
                poolItem.LastUsedTime = DateTime.UtcNow;
            }
        }
        private void ExpireIdleItems()
        {
            lock (this.itemStore)
            {
                int repeatCount = this.itemStore.Count;

                for (int i = 0; i < repeatCount; i++)
                {
                    IIdlePoolItem item = this.itemStore.Peek() as IIdlePoolItem;

                    if (item == null)
                    {
                        break;
                    }

                    if (!item.LastUsedTime.HasValue || this.idleTimeoutInSec <= 0 || (item.LastUsedTime.Value - DateTime.UtcNow).Seconds < this.idleTimeoutInSec)
                    {
                        // hack to advance pointer to the next item stored in slot
                        T ignoredItem = this.itemStore.Fetch();
                        this.itemStore.Store(ignoredItem);
                        continue;
                    }

                    Debug.Assert(item.Equals(this.itemStore.Fetch()), "Peeked item and fetched item are not the same");
                    item.DisposeItem();
                }
            }
        }
        private bool CanReuseItem(T item)
        {
            IThrowablePoolItem throwableItem = item as IThrowablePoolItem;

            if (throwableItem == null)
            {
                return true;
            }

            return throwableItem.CanReuseItem;
        }
    }
}
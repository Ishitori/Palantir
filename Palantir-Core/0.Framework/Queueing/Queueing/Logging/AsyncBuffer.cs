namespace Ix.Palantir.Queueing.Logging
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Ix.Palantir.Exceptions;

    public sealed class AsyncBuffer<T> : IDisposable
    {
        private readonly T[] buffer;
        private readonly int bufferSize;
        private readonly Action<T> readMethod;
        private readonly object writeLock = new object();

        private uint head;
        private bool isDisposed;
        private bool readStarted;
        private uint tail;

        public AsyncBuffer(int bufferSize, Action<T> readMethod)
        {
            this.head = 0;
            this.bufferSize = bufferSize;
            this.readMethod = readMethod;

            this.buffer = new T[this.bufferSize];
            this.EventWaitHandle = new EventWaitHandle(false, EventResetMode.AutoReset);
            this.NoSubscribersHandle = new EventWaitHandle(false, EventResetMode.AutoReset);
        }

        public bool ReadStarted
        {
            get { return this.readStarted; }
        }

        private EventWaitHandle EventWaitHandle { get; set; }
        private EventWaitHandle NoSubscribersHandle { get; set; }

        public void Dispose()
        {
            this.Close();
            this.isDisposed = true;

            this.NoSubscribersHandle.WaitOne();
            this.EventWaitHandle.Dispose();
            this.NoSubscribersHandle.Dispose();
        }

        public void StartRead()
        {
            if (this.isDisposed)
            {
                throw new PalantirException("AsyncBuffer instance was already disposed. Create new instance to use.");
            }

            if (this.readStarted)
            {
                return;
            }

            this.readStarted = true;
            Task.Factory.StartNew(this.Read);
        }

        public void WriteAsync(T item)
        {
            if (this.isDisposed)
            {
                return;
            }

            Task.Factory.StartNew(() => this.Write(item));
        }

        private void Write(T item)
        {
            lock (this.writeLock)
            {
                this.buffer[this.head % this.bufferSize] = item;

                unchecked
                {
                    this.head += 1;
                }

                this.EventWaitHandle.Set();
            }
        }

        private void Read()
        {
            while (this.readStarted && !this.isDisposed)
            {
                this.NoSubscribersHandle.Reset();

                if (this.head == this.tail)
                {
                    this.EventWaitHandle.Reset();
                }

                this.EventWaitHandle.WaitOne();
                this.NoSubscribersHandle.Set();

                if (!this.readStarted || this.isDisposed)
                {
                    break;
                }

                this.readMethod(this.buffer[this.tail % this.bufferSize]);

                unchecked
                {
                    this.tail += 1;
                }
            }
        }

        private void Close()
        {
            this.readStarted = false;
            this.EventWaitHandle.Set();
        }
    }
}
namespace Ix.Palantir.Queueing
{
    using System;
    using Ix.Palantir.Queueing.API;
    using Pooling;

    public sealed class PooledMessageSender : IMessageSender, IIdlePoolItem, IThrowablePoolItem
    {
        private readonly Pool<IMessageSender> pool;
        private readonly IMessageSender realSender;

        public PooledMessageSender(Pool<IMessageSender> pool, IMessageSender realSender)
        {
            this.pool = pool;
            this.realSender = realSender;
            this.CanReuseItem = true;
        }

        public DateTime? LastUsedTime
        {
            get;
            set;
        }
        public bool CanReuseItem
        {
            get; 
            set;
        }

        public void Send(IMessage message)
        {
            try
            {
                this.realSender.Send(message);
            }
            catch
            {
                this.realSender.Dispose();
                this.CanReuseItem = false;
                throw;
            }
        }

        public void Dispose()
        {
            if (this.pool.IsDisposed)
            {
                this.realSender.Dispose();
            }
            else
            {
                this.pool.Release(this);
            }
        }
        void IIdlePoolItem.DisposeItem()
        {
            this.realSender.Dispose();
        }
    }
}
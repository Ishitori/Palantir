namespace Ix.Palantir.Queueing.Factories
{
    using System;
    using System.Collections.Generic;
    using Ix.Palantir.Configuration.Queueing;
    using Ix.Palantir.Pooling;
    using Ix.Palantir.Queueing.API;
    using StructureMap;
    using IMessageSender = Ix.Palantir.Queueing.API.IMessageSender;

    public class QueueingFactory : IQueueingFactory, IDisposable
    {
        private readonly object senderLockObject = new object();
     
        private bool isDisposed;
        private IDictionary<string, Pool<IMessageSender>> poolDictionary;

        public QueueingFactory()
        {
            this.poolDictionary = new Dictionary<string, Pool<IMessageSender>>();
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        public IMessageReceiver GetReceiver(Queue queue, string selector)
        {
            var messageReceiver = ObjectFactory.With("queue").EqualTo(queue).GetInstance<IMessageReceiver>();
            messageReceiver.Selector = selector;
            return messageReceiver;
        }

        public IMessageSender GetSender(Queue queue)
        {
            if (!queue.EnableSendersPooling)
            {
                return this.CreateSender(queue);
            }

            if (!this.poolDictionary.ContainsKey(queue.Id))
            {
                lock (this.senderLockObject)
                {
                    if (!this.poolDictionary.ContainsKey(queue.Id))
                    {
                        this.poolDictionary.Add(queue.Id, new Pool<IMessageSender>(queue.MaxPoolSize, p => new PooledMessageSender(p, this.CreateSender(queue)), LoadingMode.Lazy, AccessMode.FIFO, queue.IdleTimeout));
                    }
                }
            }

            Pool<IMessageSender> poolForQueue = this.poolDictionary[queue.Id];
            return poolForQueue.Acquire();
        }

        protected virtual void Dispose(bool isDisposing)
        {
            if (this.isDisposed)
            {
                return;
            }

            if (isDisposing)
            {
                foreach (KeyValuePair<string, Pool<IMessageSender>> keyValuePair in this.poolDictionary)
                {
                    keyValuePair.Value.Dispose();
                }

                this.poolDictionary.Clear();
                this.poolDictionary = null;
            }

            this.isDisposed = true;
        }

        private IMessageSender CreateSender(Queue queue)
        {
            return ObjectFactory.With("queue").EqualTo(queue).GetInstance<IMessageSender>();
        }
    }
}
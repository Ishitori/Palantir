namespace Ix.Palantir.LockServer
{
    using System;
    using System.Threading;
    using Ix.Palantir.Caching;
    using Ix.Palantir.LockServer.API;
    using Ix.Palantir.Logging;

    public class LockServer : ILockServer
    {
        private const int CONST_RetryPeriodMs = 300;

        private readonly ICacheStorage cacheStorage;
        private readonly ILog log;

        public LockServer(ICacheStorage cacheStorage)
        {
            this.log = LogManager.GetLogger();
            this.cacheStorage = cacheStorage;
        }

        public T LockSection<T>(string initKey, Func<T> section)
        {
            this.log.DebugFormat("LockSection: Entering. InitKey: {0}", initKey);

            for (int tryIndex = 0;; tryIndex++)
            {
                this.log.DebugFormat("LockSection: Trying to aquire lock. InitKey: {0}, Zero-based try: {1}", initKey, tryIndex);
                bool putSuccess = this.cacheStorage.PutItemIfNotExists(initKey, new Lock());

                if (!putSuccess)
                {
                    this.log.DebugFormat("LockSection: Lock aquiring is failed. InitKey: {0}, Zero-based try: {1}, Timeout till next try: {2}ms", initKey, tryIndex, CONST_RetryPeriodMs);
                    Thread.Sleep(CONST_RetryPeriodMs);
                    continue;
                }

                try
                {
                    this.log.DebugFormat("LockSection: Lock is aquired. InitKey: {0}, On try: {1}. Executing critical section...", initKey, tryIndex);
                    T result = section();
                    this.log.DebugFormat("LockSection: Executing critical section is finished. InitKey: {0}", initKey);

                    return result;
                }
                catch (Exception)
                {
                    this.log.DebugFormat("LockSection: Error while processing the section. InitKey: {0}", initKey);
                    throw;
                }
                finally
                {
                    this.cacheStorage.RemoveItem(initKey);
                    this.log.DebugFormat("LockSection: Lock is removed. InitKey: {0}", initKey);
                }
            }
        }

        public void LockSection(string initKey, Action section)
        {
            this.log.DebugFormat("LockSection: Entering. InitKey: {0}", initKey);

            for (int tryIndex = 0;; tryIndex++)
            {
                this.log.DebugFormat("LockSection: Trying to aquire lock. InitKey: {0}, Zero-based try: {1}", initKey, tryIndex);
                bool putSuccess = this.cacheStorage.PutItemIfNotExists(initKey, new Lock());

                if (!putSuccess)
                {
                    this.log.DebugFormat("LockSection: Lock aquiring is failed. InitKey: {0}, Zero-based try: {1}, Timeout till next try: {2}ms", initKey, tryIndex, CONST_RetryPeriodMs);
                    Thread.Sleep(CONST_RetryPeriodMs);
                    continue;
                }

                try
                {
                    this.log.DebugFormat("LockSection: Lock is aquired. InitKey: {0}, On try: {1}. Executing critical section...", initKey, tryIndex);
                    section();
                    this.log.DebugFormat("LockSection: Executing critical section is finished. InitKey: {0}", initKey);
                    return;
                }
                catch (Exception)
                {
                    this.log.DebugFormat("LockSection: Error while processing the section. InitKey: {0}", initKey);
                    throw;
                }
                finally
                {
                    this.cacheStorage.RemoveItem(initKey);
                    this.log.DebugFormat("LockSection: Lock is removed. InitKey: {0}", initKey);
                }
            }
        }
    }
}
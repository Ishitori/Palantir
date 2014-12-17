namespace Ix.Palantir.Vkontakte.API
{
    using System;
    using System.Threading;
    using Ix.Palantir.Logging;

    public class VkTimeQuantinizer : IDisposable
    {
        private const int CONST_BeatsPerSecond = 3;
        private const int CONST_Epsilon = 50;
        private const int CONST_MaxTimeForVkRequest = 60000;

        private readonly AutoResetEvent quantum;
        private readonly ManualResetEvent vkRequestFinishedEvent;

        public VkTimeQuantinizer()
        {
            this.quantum = new AutoResetEvent(false);
            this.vkRequestFinishedEvent = new ManualResetEvent(true);
        }

        public void StartHeartBeat()
        {
            Thread heartBeatThread = new Thread(this.VkHeartBeat);
            heartBeatThread.Start();
        }

        public bool StartVkRequest(int maxHeartBeatTimeout)
        {
            bool canDoVkRequest = this.quantum.WaitOne(maxHeartBeatTimeout);
            this.vkRequestFinishedEvent.Reset();
            return canDoVkRequest;
        }

        public void FinishVkRequest()
        {
            this.vkRequestFinishedEvent.Set();
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void VkHeartBeat()
        {
            int msBetweenBeats = (1000 / CONST_BeatsPerSecond) + CONST_Epsilon;

            while (true)
            {
                if (!this.vkRequestFinishedEvent.WaitOne(CONST_MaxTimeForVkRequest))
                {
                    LogManager.GetLogger("VkAccessor").WarnFormat("Vk request hasn't been completed even in {0} ms. Something is wrong with VK", CONST_MaxTimeForVkRequest);
                }

                Thread.Sleep(msBetweenBeats);
                this.quantum.Set();
            }
        }

        private void Dispose(bool isDisposing)
        {
            if (isDisposing)
            {
                this.quantum.Dispose();
                this.vkRequestFinishedEvent.Dispose();
            }
        }
    }
}
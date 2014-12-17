namespace Ix.Palantir.Scheduler
{
    using System;
    using Exceptions;
    using Ix.Palantir.Logging;
    using Runner;

    public sealed class ScheduleRunner : IDisposable
    {
        private bool disposed;
        private readonly ILog log;
        private readonly SchedulerServer schedulerServer;
        private static ScheduleRunner instance;
        private static readonly object lockObj = new object();
        private bool isSchedulerRunning;

        public ScheduleRunner(string scheduleConfigPath)
        {
            this.log = LogManager.GetLogger("Application");

            try
            {
                this.log.InfoFormat("Loading scheduler configuration file '{0}'...", scheduleConfigPath);
                this.schedulerServer = new SchedulerServer();
                this.schedulerServer.LoadFromXmlFile(scheduleConfigPath);
                this.log.Info("Scheduler configuration loaded");
            }
            catch (Exception ex)
            {
                this.log.ErrorFormat("Scheduler configuration failed to loaded: {0}", ex.ToString());

                if (ExceptionHelper.IsFatalException(ex))
                {
                    throw;
                }
            }
        }

        public static string ConfigurationFilePath { get; set; }
        public static ScheduleRunner Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (lockObj)
                    {
                        if (instance == null)
                        {
                            string path = ConfigurationFilePath;

                            if (string.IsNullOrEmpty(path))
                            {
                                throw new ArgumentException("ConfigurationFilePath was not properly initialized before using scheduler", "ConfigurationFilePath");
                            }

                            instance = new ScheduleRunner(path);
                        }
                    }
                }

                return instance;
            }
        }

        public bool IsSchedulerRunning
        {
            get
            {
                return this.isSchedulerRunning;
            }
        }

        public static void StartNewScheduler()
        {
            if (instance != null)
            {
                lock (lockObj)
                {
                    instance = null;
                }
            }

            Instance.Start();
        }

        public void Start()
        {
            try
            {
                this.log.Info("Starting scheduler...");
                this.schedulerServer.Run();
                this.isSchedulerRunning = true;
                this.log.Info("Scheduler started");
            }
            catch (Exception ex)
            {
                this.log.ErrorFormat("Scheduler failed to start: {0}", ex.ToString());

                if (ExceptionHelper.IsFatalException(ex))
                {
                    throw;
                }

                this.isSchedulerRunning = false;
            }
        }
        public void Stop()
        {
            if (this.schedulerServer != null)
            {
                this.isSchedulerRunning = false;
                this.log.Info("Stopping Scheduler...");
                this.schedulerServer.Stop();
                this.log.Info("Scheduler stopped.");
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }
         
        private void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    if (this.schedulerServer != null)
                    {
                        this.schedulerServer.Dispose();
                    }
                }

                this.disposed = true;
            }
        }
    }
}
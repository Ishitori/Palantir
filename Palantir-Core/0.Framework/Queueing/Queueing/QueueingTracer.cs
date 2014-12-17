namespace Ix.Palantir.Queueing
{
    using Palantir.Logging;

    public class QueueingTracer : Apache.NMS.ITrace
    {
        private readonly ILog logger;

        public QueueingTracer()
        {
            this.logger = LogManager.GetLogger(ActivityType.Queueing.ToString());
        }

        public bool IsDebugEnabled
        {
            get 
            {
                return this.logger.IsDebugEnabled;
            }
        }
        public bool IsInfoEnabled
        {
            get
            {
                return this.logger.IsInfoEnabled;
            }
        }
        public bool IsWarnEnabled
        {
            get
            {
                return this.logger.IsWarnEnabled;
            }
        }
        public bool IsErrorEnabled
        {
            get
            {
                return this.logger.IsErrorEnabled;
            }
        }
        public bool IsFatalEnabled
        {
            get
            {
                return this.logger.IsFatalEnabled;
            }
        }

        public void Debug(string message)
        {
            this.logger.Debug(message);
        }
        public void Info(string message)
        {
            this.logger.Info(message);
        }
        public void Warn(string message)
        {
            this.logger.Warn(message);
        }
        public void Error(string message)
        {
            this.logger.Error(message);
        }
        public void Fatal(string message)
        {
            this.logger.Fatal(message);
        }
    }
}

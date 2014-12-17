namespace Ix.Palantir.Logging
{
    using System;
    using NLog;

    public class NLogWrapper : ILog
    {
        private readonly Logger logger;

        public NLogWrapper(Logger logger)
        {
            this.logger = logger;
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
        public void Debug(string message, Exception exception)
        {
            this.logger.DebugException(message, exception);
        }
        public void DebugFormat(string format, params object[] args)
        {
            this.logger.Debug(string.Format(format, args));
        }
        public void Info(string message)
        {
            this.logger.Info(message);
        }
        public void Info(string message, Exception exception)
        {
            this.logger.InfoException(message, exception);
        }
        public void InfoFormat(string format, params object[] args)
        {
            this.logger.Info(string.Format(format, args));
        }
        public void Warn(string message)
        {
            this.logger.Warn(message);
        }
        public void Warn(string message, Exception exception)
        {
            this.logger.WarnException(message, exception);
        }
        public void WarnFormat(string format, params object[] args)
        {
            this.logger.Warn(string.Format(format, args));
        }
        public void Error(string message)
        {
            this.logger.Error(message);
        }
        public void Error(string message, Exception exception)
        {
            this.logger.ErrorException(message, exception);
        }
        public void ErrorFormat(string format, params object[] args)
        {
            this.logger.Error(string.Format(format, args));
        }
        public void Fatal(string message)
        {
            this.logger.Fatal(message);
        }
        public void Fatal(string message, Exception exception)
        {
            this.logger.FatalException(message, exception);
        }
        public void FatalFormat(string format, params object[] args)
        {
            this.logger.Fatal(string.Format(format, args));
        }
    }
}
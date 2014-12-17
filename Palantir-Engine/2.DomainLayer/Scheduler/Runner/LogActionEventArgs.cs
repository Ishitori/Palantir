namespace Ix.Palantir.Scheduler.Runner
{
    using System;

    public class LogActionEventArgs : EventArgs
    {
        public LogActionEventArgs(string msg)
        {
            this.Message = msg;
        }

        public string Message
        {
            get
            {
                return this.mMessage;
            }

            set
            {
                this.mMessage = value;
            }
        }

        private string mMessage;
    }
}
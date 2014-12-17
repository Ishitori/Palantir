namespace Ix.Palantir.Logging
{
    public static class LogManager
    {
        public static ILog GetLogger()
        {
            return new NLogWrapper(NLog.LogManager.GetCurrentClassLogger());
        }
        public static ILog GetLogger(string activityType)
        {
            return new NLogWrapper(NLog.LogManager.GetLogger(activityType));
        }
        public static void Flush()
        {
            NLog.LogManager.Flush();
        }
    }
}
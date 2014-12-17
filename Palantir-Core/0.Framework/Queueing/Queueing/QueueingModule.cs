namespace Ix.Palantir.Queueing
{
    using Configuration.Queueing;
    using Framework.ObjectFactory;

    using Ix.Palantir.Queueing.API;

    public class QueueingModule : IQueueingModule
    {
        private readonly QueueingConfig configuration;

        public QueueingModule(QueueingConfig configuration)
        {
            this.configuration = configuration;
        }

        public void InitializeQueueingSystem()
        {
            Apache.NMS.ITrace trace = this.configuration.EnableTracing
                ? Factory.GetInstance<Apache.NMS.ITrace>()
                : null;

            Apache.NMS.Tracer.Trace = trace;
        }
    }
}

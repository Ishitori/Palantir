namespace Ix.Palantir.Queueing.Bootstrapper
{
    using System;
    using Command;
    using Configuration.Queueing;
    using Framework.ObjectFactory;

    using Ix.Palantir.Configuration.API;
    using Ix.Palantir.Queueing.API;
    using Ix.Palantir.Queueing.API.Command;
    using Ix.Palantir.Queueing.Factories;

    using Logging;
    using ObjectFactory.StructureMapExtension;
    using Queueing;
    using Queueing.Redelivery;
    using StatServer.Queueing.Commands;
    using StructureMap;
    using StructureMap.Pipeline;

    using IMessageSender = Ix.Palantir.Queueing.API.IMessageSender;

    public class QueueingRegistry : IRegistry
    {
        public void InstantiateIn(IObjectResolver objectResolver)
        {
            if (objectResolver is StructureMapObjectResolver)
            {
                this.InstantiateInStructureMap();
            }
            else
            {
                throw new ArgumentException("Unsupported type of object resolver is provided", "objectResolver");
            }
        }
        private void InstantiateInStructureMap()
        {
            StructureMap.ObjectFactory.Configure(x =>
            {
                x.For<IQueueingTransportLogger>().Use<NullQueueingTransportLogger>();

                x.For<IMessage>().Use<Message>();
                x.For<IMessageReceiver>().Use<MessageReceiver>();
                x.For<IMessageSender>().Use<Ix.Palantir.Queueing.MessageSender>();
                x.For<ISession>().Use<Session>();
                x.For<ISessionProvider>().Use<SessionProvider>();
                x.For<IConnectionNameProvider>().Use<ConnectionNameProvider>();
                x.For<QueueingConfig>().Use(y => y.GetInstance<IConfigurationProvider>().GetConfigurationSection<QueueingConfig>());
                x.For<Apache.NMS.ITrace>().Use<QueueingTracer>();
                x.For<IQueueingModule>().Use<QueueingModule>();
                x.For<IQueueingFactory>().LifecycleIs(Lifecycles.GetLifecycle(InstanceScope.Singleton)).Use<QueueingFactory>();
                x.For<IRedeliveryStrategyFactory>().LifecycleIs(Lifecycles.GetLifecycle(InstanceScope.Singleton)).Use<RedeliveryStrategyFactory>();
                
                x.For<ICommandSender>().Use<CommandSender>();
                x.For<ICommandReceiver>().Use<CommandReceiver>();
                x.For<ICommandMessageMapper>().Use<CommandMessageMapper>();
                x.For<ICommandRepository>().Use<CommandRepository>();
            });
        }  
    }
}

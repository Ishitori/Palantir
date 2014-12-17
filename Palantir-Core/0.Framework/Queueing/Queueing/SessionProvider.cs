namespace Ix.Palantir.Queueing
{
    using Framework.ObjectFactory;

    using Ix.Palantir.Queueing.API;

    public class SessionProvider : ISessionProvider
    {
        public ISession GetSession(bool transactionMode)
        {
            ISession session = Factory.GetInstance<ISession>();
            session.Initialize(transactionMode);

            return session;
        }
    }
}

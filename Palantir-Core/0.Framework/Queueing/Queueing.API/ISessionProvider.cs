namespace Ix.Palantir.Queueing.API
{
    public interface ISessionProvider
    {
        ISession GetSession(bool transactionalMode);
    }
}

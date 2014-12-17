namespace Ix.Palantir.DataAccess.NHibernateImpl
{
    using System.Reflection;
    using Ix.Palantir.DataAccess.API;
    using NHibernate;
    using NHibernate.Tool.hbm2ddl;

    public class SessionFactory
    {
        private static readonly ISessionFactory NHSessionFactory;
        private static readonly NHibernate.Cfg.Configuration NHConfiguration;

        static SessionFactory()
        {
            NHConfiguration = new NHibernate.Cfg.Configuration();
            NHConfiguration.AddAssembly(Assembly.GetExecutingAssembly().FullName);
            SchemaMetadataUpdater.QuoteTableAndColumns(NHConfiguration);
            NHSessionFactory = NHConfiguration.BuildSessionFactory();
        }

        public IDataGateway CreateSession()
        {
            ISession session = NHSessionFactory.OpenSession();
            session.FlushMode = FlushMode.Commit;
            return new DataGateway(session);
        }
        public IDurableDataGateway CreateDurableSession()
        {
            ISession session = NHSessionFactory.OpenSession();
            session.FlushMode = FlushMode.Commit;
            var durableDataGateway = new DurableDataGateway(session);
            durableDataGateway.IsFresh = true;
            return durableDataGateway;
        }
    }
}
namespace Ix.Palantir.DataAccess
{
    using Framework.ObjectFactory;

    using Ix.Palantir.DataAccess.API;

    public class UnitOfWorkProvider : IUnitOfWorkProvider
    {
        public ITransactionScope CreateTransaction()
        {
            return Factory.GetInstance<ITransactionScope>();
        }
        public IUnitOfWork CreateUnitOfWork()
        {
            return Factory.GetInstance<IUnitOfWork>();
        }
    }
}
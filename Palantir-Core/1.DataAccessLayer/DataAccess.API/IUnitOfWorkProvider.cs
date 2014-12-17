namespace Ix.Palantir.DataAccess.API
{
    public interface IUnitOfWorkProvider
    {
        ITransactionScope CreateTransaction();
        IUnitOfWork CreateUnitOfWork();
    }
}
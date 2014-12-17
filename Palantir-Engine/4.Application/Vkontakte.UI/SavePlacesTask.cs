namespace Ix.Palantir.Vkontakte.UI
{
    using DataAccess.API;
    using Framework.ObjectFactory;
    using Infrastructure.Process;

    public class SavePlacesTask
    {
        private readonly IUnitOfWorkProvider unitOfWorkProvider;

        public SavePlacesTask(IUnitOfWorkProvider unitOfWorkProvider)
        {
            this.unitOfWorkProvider = unitOfWorkProvider;
        }

        public void Execute()
        {
            using (this.unitOfWorkProvider.CreateUnitOfWork())
            using (ITransactionScope transaction = this.unitOfWorkProvider.CreateTransaction().Begin())
            {
                Factory.GetInstance<SavePlacesFromVkProcess>().SaveCountriesAndCities();
                transaction.Commit();
            }
        }         
    }
}
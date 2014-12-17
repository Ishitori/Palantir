namespace Ix.Framework.ObjectFactory
{
    public interface IRegistry
    {
        void InstantiateIn(IObjectResolver objectResolver);
    }
}
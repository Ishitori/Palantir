namespace Ix.Palantir.Services.API
{
    using System.Collections.Generic;

    public interface INavigationService
    {
        IEnumerable<MenuItemLink> GetMenuItems();
        IEnumerable<MenuItemLink> GetMetricsMenuItems();
    }
}

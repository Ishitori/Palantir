namespace Ix.Palantir.Services.API
{
    using System.Collections.Generic;
    using System.Web.Routing;

    public class MenuItemLink
    {
        public MenuItemLink()
        {
            this.Children = new List<MenuItemLink>();
        }

        public string Title { get; set; }
        public IList<MenuItemLink> Children { get; set; }
        public RouteValueDictionary RouteValues { get; set; }
        public string ImgSrc { get; set; }
        public bool IsSelected { get; set; }
    }
}
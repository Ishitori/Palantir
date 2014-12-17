namespace Ix.Palantir.UI.Binders
{
    using System.Collections.Generic;
    using System.Web.Mvc;

    public class IntListBinder : IModelBinder
    {
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            IList<int> result = new List<int>();
            var serializedString = bindingContext.ValueProvider.GetValue("items").AttemptedValue;

            if (string.IsNullOrWhiteSpace(serializedString))
            {
                return new List<int>();
            }

            var parts = serializedString.Split(',');

            foreach (var part in parts)
            {
                int value;

                if (int.TryParse(part, out value))
                {
                    result.Add(value);
                }
            }

            return result;
        }
    }
}
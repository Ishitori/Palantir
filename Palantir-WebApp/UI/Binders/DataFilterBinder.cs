namespace Ix.Palantir.UI.Binders
{
    using System;
    using System.Web.Mvc;
    using Ix.Palantir.Querying.Common;
    using Ix.Palantir.Querying.Common.DataFilters;

    public class DataFilterBinder : IModelBinder
    {
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var filter = new DataFilter();
            FilteringPeriod period;
            var periodParameter = bindingContext.ValueProvider.GetValue("period").AttemptedValue;
            Enum.TryParse(periodParameter, true, out period);
            filter.Period = period;
            
            if (filter.Period == FilteringPeriod.Other)
            {
                var dateFromParameter = bindingContext.ValueProvider.GetValue("dateFrom").AttemptedValue;
                var dateToParameter = bindingContext.ValueProvider.GetValue("dateTo").AttemptedValue;
                
                if (!string.IsNullOrEmpty(dateFromParameter) && !string.IsNullOrEmpty(dateToParameter))
                {
                    var dateFrom = DateTime.ParseExact(dateFromParameter, "d.MM.yyyy", null);
                    var dateTo = DateTime.ParseExact(dateToParameter, "d.MM.yyyy", null);
                    filter.DateRange = new DateRange(dateFrom, dateTo);
                }
            }

            return filter;
        }
    }
}
namespace Ix.Palantir.UI.Formatters
{
    using System.Globalization;
    using Ix.Palantir.Services.API;
    using Ix.Palantir.Utilities;

    public class UiPostDensityFormatter
    {
        private readonly PostDensity density;

        public UiPostDensityFormatter(PostDensity density)
        {
            this.density = density;
        }

        public override string ToString()
        {
            if (this.density.AbsoluteValue == 0)
            {
                return "-";
            }

            return string.Format(
                "<span class=\"crowd-time\">{0}, {1}:00 - {2}:00</span><span class=\"crowd-volume\">({3}% всех сообщений)</span>",
                CultureInfo.CurrentCulture.DateTimeFormat.AbbreviatedDayNames[(int)this.density.DayOfWeek].ToUpperFirstLetter(),
                this.density.BeginHour,
                this.density.EndHour,
                this.density.RelativeValue);
        }
    }
}
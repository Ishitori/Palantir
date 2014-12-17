namespace Ix.Palantir.UI.Models.Chart
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using Ix.Framework.ObjectFactory;
    using Ix.Palantir.Localization.API;
    using Querying.Common;

    [DataContract]
    [KnownType(typeof(ChartPoint_WithPercents))]
    public class TrendChartData
    {
        public TrendChartData()
        {
            this.Values = new List<ChartPoint>();
            this.YaxisOrder = 1;
            this.ShowBars = true;
            this.ShowLines = false;
            this.FillLines = false;
            this.ShowPoints = false;
            this.BarWidth = 0.2;
            this.Stacking = false;
            this.Limited = false;
            this.ShowLabelInTip = false;
        }

        public TrendChartData(IEnumerable<PointInTime> points, FilteringPeriod period)
        {
            string pattern = "{0:dd MMM yyyy}";

            switch (period)
            {
                case FilteringPeriod.Day:
                    // час:минуты
                    pattern = "{0:HH}:{0:mm}";
                    break;

                case FilteringPeriod.Month:
                    // день мес€ц
                    pattern = "{0:dd} {0:MMM}";
                    break;

                case FilteringPeriod.Week:
                    // день мес€ц (день недели)
                    pattern = "{0:dd} {0:MMM} ({0:ddd})";
                    break;

                case FilteringPeriod.Year:
                    // мес€ц год
                    pattern = "{0:MMM} {0:yyyy}";
                    break;
            }

            var dateTimeHelper = Factory.GetInstance<IDateTimeHelper>();

            if (points != null)
            {
                this.Values = points.Select(point => this.CreateChartPoint(point, dateTimeHelper, pattern)).ToList();
            }
            this.ShowPoints = true;
            this.ShowLines = true;
            this.ShowBars = false;
            this.BarWidth = 0.3;
            this.FillLines = true;
            this.YaxisOrder = 1;
            this.Stacking = false;
            this.Limited = false;
            this.ShowLabelInTip = false;
        }

        [DataMember(Name = "values")]
        public IList<ChartPoint> Values { get; set; }

        [DataMember]
        public bool ShowPoints { get; set; }

        [DataMember]
        public bool ShowLines { get; set; }

        [DataMember]
        public bool FillLines { get; set; }

        [DataMember]
        public bool ShowBars { get; set; }

        [DataMember]
        public double BarWidth { get; set; }

        [IgnoreDataMember]
        public BarAlign BarAlign { get; set; }

        [DataMember]
        public int BarOrder { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public int YaxisOrder { get; set; }

        [DataMember]
        public bool Stacking { get; set; }

        [DataMember]
        public bool Limited { get; set; }

        [DataMember]
        public string MaxTimeLimit { get; set; }

        [DataMember]
        public string MinTimeLimit { get; set; }

        [DataMember]
        public bool ShowLabelInTip { get; set; }

        [DataMember]
        protected string BarAlignString
        {
            get { return this.BarAlign.ToString().ToLower(); }
            set { this.BarAlign = (BarAlign)Enum.Parse(typeof(BarAlign), value); }
        }

        private ChartPoint CreateChartPoint(PointInTime point, IDateTimeHelper dateTimeHelper, string pattern)
        {
            // TODO: Conversion to local date time should be moved to services
            string stringDate = string.Format(pattern, dateTimeHelper.GetLocalUserDate(point.Date));
            return new ChartPoint(stringDate, point.Value);
        }
    }
}
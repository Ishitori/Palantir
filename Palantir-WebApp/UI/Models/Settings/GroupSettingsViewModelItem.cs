namespace Ix.Palantir.UI.Models.Settings
{
    using Ix.Palantir.Services.API;

    public class GroupSettingsViewModelItem
    {
        private readonly GroupProcessingItem item;

        public GroupSettingsViewModelItem(GroupProcessingItem item)
        {
            this.item = item;
        }

        public string ProcessingType
        {
            get { return this.item.Item; }
        }

        public string Date
        {
            get { return this.item.ProcessingDate.ToString("dd.MM.yyyy"); }
        }

        public string Time
        {
            get { return this.item.ProcessingDate.ToString("HH:mm"); }
        }
    }
}
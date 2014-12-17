namespace Ix.Palantir.Querying.Common
{
    public class GroupedObjectWithPercents<T> : GroupedObject<T>
    {
        public GroupedObjectWithPercents(T item, int value, int usersAll, int usersActive)
        {
            this.Item = item;
            this.Value = value;
            this.PercentsFromAllUsers = string.Format("{0:0.0}%", ((float)value / (float)usersAll) * 100);
            this.PerecentsFromActiveUsers = string.Format("{0:0.0}%", ((float)value / (float)usersActive) * 100);
        }

        public string PercentsFromAllUsers { get; set; }
        public string PerecentsFromActiveUsers { get; set; }
    }
}

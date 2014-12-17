namespace Ix.Palantir.DomainModel
{
    public class VkGroupReference : IVkEntity
    {
        public virtual int Id
        {
            get
            {
                return this.VkGroupId;
            }
        }
        public virtual int VkGroupId { get; set; }
        public virtual string VkId
        {
            get
            {
                return this.Id.ToString();
            }
        }
        public virtual string NameGroup { get; set; }
        public virtual string Photo { get; set; }
        public virtual string ScreenName { get; set; }
    }
}
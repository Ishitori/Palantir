namespace Ix.Palantir.Services.API.DTO
{
    public class BirthDate
    {
        public BirthDate(int birthYear, int birthMonth, int birthDay)
        {
            this.BirthYear = birthYear;
            this.BirthMonth = birthMonth;
            this.BirthDay = birthDay;
        }
        public virtual int BirthDay { get; set; }
        public virtual int BirthMonth { get; set; }
        public virtual int BirthYear { get; set; }
    }
}
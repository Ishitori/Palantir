namespace Ix.Palantir.UI.Formatters
{
    public class UIResponseTimeFromatter
    {
        public UIResponseTimeFromatter(string rT)
        {
            this.RT = rT;
        }

        public string RT { get; set; }

        public override string ToString()
        {
            return string.Format("<span class=\"crowd-time\">{0}</span>", this.RT);
        }
    }
}
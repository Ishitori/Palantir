namespace Ix.Palantir.Utilities
{
    using System.Text.RegularExpressions;

    public class HtmlUtils
    {
         private static readonly Regex HtmlCleanerRegex = new Regex("<.*?>", RegexOptions.Compiled | RegexOptions.IgnoreCase);

         public string RemoveHtml(string htmlString)
         {
             if (string.IsNullOrWhiteSpace(htmlString))
             {
                 return string.Empty;
             }

             return HtmlCleanerRegex.Replace(htmlString, string.Empty);
         }
    }
}
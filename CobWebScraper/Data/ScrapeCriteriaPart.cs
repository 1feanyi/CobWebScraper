using System.Text.RegularExpressions;

namespace CobWebScraper.Data
{
    internal class ScrapeCriteriaPart
    {
        public string Regex { get; set; }
        public RegexOptions RegexOption { get; set; }
    }
}
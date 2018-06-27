using CobWebScraper.Builders;
using CobWebScraper.Data;
using CobWebScraper.Workers;
using System;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
[assembly: InternalsVisibleTo("CobWebScraper.Test.Unit")]

namespace CobWebScraper
{
    internal class Program
    {
        private const string Method = "search";

        private static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("Please enter which city you would like to scrap info from:");
                var craigslistCity = Console.ReadLine() ?? string.Empty;

                Console.WriteLine("Please enter the Craigslist category you would like to scrape:");
                var craigslistCategoryName = Console.ReadLine() ?? string.Empty;

                using (WebClient client = new WebClient())
                {
                    string content = client.DownloadString($"http://{craigslistCity.Replace(" ", string.Empty)}.craigslist.org/{Method}/{craigslistCategoryName}");

                    // using regex and builder pattern to scrape downloaded content
                    ScrapeCriteria scrapeCriteria = new ScrapeCriteriaBuilder()
                        .WithData(content)
                        .WithRegex(@"<a href=\""(.*?)\"" data-id=\""(.*?)\"" class=\""result-title hdrlnk\"">(.*?)</a>")
                        .WithRegexOption(RegexOptions.ExplicitCapture)
                        .WithPart(new ScrapeCriteriaPartBuilder()
                            .WithRegex(@">(.*?)</a>")
                            .WithRegexOption(RegexOptions.Singleline)
                            .Build())
                        .WithPart(new ScrapeCriteriaPartBuilder()
                            .WithRegex(@"href=\""(.*?)\""")
                            .WithRegexOption(RegexOptions.Singleline)
                            .Build())
                        .Build();

                    Scraper scraper = new Scraper();

                    var scrapedElements = scraper.Scrape(scrapeCriteria);

                    if (scrapedElements.Any())
                    {
                        foreach (var scrapedElement in scrapedElements)
                        {
                            Console.WriteLine(scrapedElement);
                        }
                    }
                    else
                    {
                        Console.WriteLine("There were no mathces for the specified criteria.");
                    }
                }

                Console.ReadLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
using CobWebScraper.Builders;
using CobWebScraper.Data;
using CobWebScraper.Workers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text.RegularExpressions;

namespace CobWebScraper.Test.Unit
{
    [TestClass]
    public class ScraperTest
    {
        private readonly Scraper _scraper = new Scraper();

        [TestMethod]
        public void FindCollectionWithNoSegments()
        {
            var content = "Some fluff data <a href=\"http://domain.com\" data-id=\"someId\" class=\"result-title hdrlnk\">some text</a> more fluff data";

            ScrapeCriteria scrapeCriteria = new ScrapeCriteriaBuilder()
                .WithData(content)
                .WithRegex(@"<a href=\""(.*?)\"" data-id=\""(.*?)\"" class=\""result-title hdrlnk\"">(.*?)</a>")
                .WithRegexOption(RegexOptions.ExplicitCapture)
                .Build();

            var foundElements = _scraper.Scrape(scrapeCriteria);

            Assert.IsTrue(foundElements.Count == 1);
            Assert.IsTrue(foundElements[0] == "<a href=\"http://domain.com\" data-id=\"someId\" class=\"result-title hdrlnk\">some text</a>");
        }

        [TestMethod]
        public void FindCollectionWithTwoParts()
        {
            var content = "Some fluff data <a href=\"http://domain.com\" data-id=\"someId\" class=\"result-title hdrlnk\">some text</a> more fluff data";

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

            var foundElements = _scraper.Scrape(scrapeCriteria);

            Assert.IsTrue(foundElements.Count == 2);
            Assert.IsTrue(foundElements[0] == "some text");
            Assert.IsTrue(foundElements[1] == "http://domain.com");
        }
    }
}
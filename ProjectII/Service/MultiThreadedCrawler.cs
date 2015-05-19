using Neo4jClient;
using ProjectII.Domain;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Abot.Crawler;
using Abot.Poco;

namespace ProjectII.Service
{
    class MultiThreadedCrawler
    {

        private GraphClient client;
        private int crawledPages;

        public MultiThreadedCrawler(GraphClient client) {
            this.client = client;
            this.crawledPages = 0;
        }

        public void crawl(string url,  int nrOfThreads, int pagesToCrawl, int crawlTimeout, int crawlDelay) {
            CrawlConfiguration crawlConfig = new CrawlConfiguration();
            crawlConfig.CrawlTimeoutSeconds = crawlTimeout;
            crawlConfig.MaxConcurrentThreads = nrOfThreads;
            crawlConfig.MaxPagesToCrawl = pagesToCrawl;
            crawlConfig.UserAgentString = "abot v1.0 https://github.com/sjdirect/abot/";
            crawlConfig.IsExternalPageCrawlingEnabled = true;
            crawlConfig.IsExternalPageLinksCrawlingEnabled = true;
            crawlConfig.MinCrawlDelayPerDomainMilliSeconds = crawlDelay;


            PoliteWebCrawler crawler = new PoliteWebCrawler(crawlConfig, null, null, null, null, null, null, null, null);

            crawler.PageCrawlStarting += crawler_ProcessPageCrawlStarting;
            //crawler.PageCrawlCompleted += crawler_ProcessPageCrawlCompleted;
            //crawler.PageCrawlDisallowed += crawler_PageCrawlDisallowed;
            //crawler.PageLinksCrawlDisallowed += crawler_PageLinksCrawlDisallowed;

            CrawlResult result = crawler.Crawl(new Uri(url)); //This is synchronous, it will not go to the next line until the crawl has completed

            if (result.ErrorOccurred)
                Console.WriteLine("Crawl of {0} completed with error: {1}", result.RootUri.AbsoluteUri, result.ErrorException.Message);
            else
                Console.WriteLine("Crawl of {0} completed without error.", result.RootUri.AbsoluteUri);

        }

        void crawler_ProcessPageCrawlStarting(object sender, PageCrawlStartingArgs e)
        {
            PageToCrawl pageToCrawl = e.PageToCrawl;
            //Console.WriteLine("About to crawl link {0} which was found on page {1}", pageToCrawl.Uri.AbsoluteUri, pageToCrawl.ParentUri.AbsoluteUri);
            saveURL(pageToCrawl.ParentUri.AbsoluteUri, pageToCrawl.Uri.AbsoluteUri);
            crawledPages++;
            Console.WriteLine(crawledPages);
        }

        void crawler_ProcessPageCrawlCompleted(object sender, PageCrawlCompletedArgs e)
        {
            crawledPages++;
            Console.WriteLine(crawledPages);
        }

        void crawler_PageLinksCrawlDisallowed(object sender, PageLinksCrawlDisallowedArgs e)
        {
            CrawledPage crawledPage = e.CrawledPage;
            Console.WriteLine("Did not crawl the links on page {0} due to {1}", crawledPage.Uri.AbsoluteUri, e.DisallowedReason);
        }

        void crawler_PageCrawlDisallowed(object sender, PageCrawlDisallowedArgs e)
        {
            PageToCrawl pageToCrawl = e.PageToCrawl;
            Console.WriteLine("Did not crawl page {0} due to {1}", pageToCrawl.Uri.AbsoluteUri, e.DisallowedReason);
        }

        private void saveURL(string startUrl, string url)
        {
            try
            {
                var newLink = new Link { Id = 456, Name = url };
                client.Cypher
                    .Merge("(link:Link { Name: {name} })")
                    .OnCreate()
                    .Set("link = {newLink}")
                    .WithParams(new
                    {
                        name = newLink.Name,
                        newLink
                    })
                    .ExecuteWithoutResults();

                client.Cypher
                .Match("(link1:Link)", "(link2:Link)")
                .Where((Link link1) => link1.Name == startUrl)
                .AndWhere((Link link2) => link2.Name == url)
                .CreateUnique("link1-[:KNOWS]->link2")
                .ExecuteWithoutResults();
            }
            catch (Exception e) {
                Console.WriteLine(e.Data);
            }
        }
    }
}

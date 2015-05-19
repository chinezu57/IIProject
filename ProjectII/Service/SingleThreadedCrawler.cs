using Neo4jClient;
using ProjectII.Domain;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ProjectII.Service
{
    class SingleThreadedCrawler
    {
        private Int64 links { get; set; }
        private string Rstring { get; set; }
        public GraphClient client;
        private Queue<string> queue;
        private Int64 crawledPages { get; set; }
        private Int64 numberOfPagesToCrawl { get; set; }
        private int maxLinks { get; set; }

        public SingleThreadedCrawler(GraphClient client, Int64 numberOfPagesToCrawl, int queueSize, int maxLinks) {
            this.client = client;
            this.numberOfPagesToCrawl = numberOfPagesToCrawl;
            this.crawledPages = 0;
            this.links = 0;
            this.queue = new Queue<string>(queueSize);
            this.maxLinks = maxLinks;
        }

        public void Crawl(string startingUrl){
            queue.Enqueue(startingUrl);
            while (queue.Peek() != null && crawledPages < numberOfPagesToCrawl) {
                CrawlLink(queue.Dequeue());
                crawledPages++;
                Console.WriteLine(crawledPages+" ---> "+links);
            }
        }

        private void CrawlLink(string url)
        {
            WebRequest myWebRequest;
            WebResponse myWebResponse;

            try
            {
                myWebRequest = WebRequest.Create(url);
                myWebResponse = myWebRequest.GetResponse();

                Stream streamResponse = myWebResponse.GetResponseStream();

                StreamReader sreader = new StreamReader(streamResponse);
                Rstring = sreader.ReadToEnd();
                GetContent(url, Rstring);
                streamResponse.Close();
                sreader.Close();
                myWebResponse.Close();
            }
            catch (Exception e) 
            { 
            
            }
        }

        private void GetContent(string startUrl, string content)
        {
            Regex regexLink = new Regex("(?<=<a\\s*?href=(?:'|\"))[^'\"]*?(?=(?:'|\"))");
            foreach (var match in regexLink.Matches(content))
            {
                if (!queue.Contains(match.ToString()))
                {
                    Uri uriResult;
                    if (Uri.TryCreate(match.ToString(), UriKind.Absolute, out uriResult) && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps))
                    {
                        saveURL(startUrl, match.ToString());
                        queue.Enqueue(match.ToString());
                        links++;
                    }
                }
            }

        }

        private void saveURL(string startUrl, string url)
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
    }
}

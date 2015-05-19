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

namespace ProjectII.Service
{
    class MultiThreadedCrawler
    {
        private static Int64 links { get; set; }
        private static string Rstring { get; set; }
        public static GraphClient client;
        private static Queue<string> queue = new Queue<string>();
        private Int64 crawledPages { get; set; }
        private Int64 numberOfPagesToCrawl { get; set; }

        public MultiThreadedCrawler(GraphClient client, Int64 numberOfPagesToCrawl)
        {
            //this.client = client;
            this.numberOfPagesToCrawl = numberOfPagesToCrawl;
            this.crawledPages = 0;
            //this.links = 0;
        }

        public void Crawl(string startingUrl)
        {
            queue.Enqueue(startingUrl);
            while (crawledPages < numberOfPagesToCrawl)
            {
                if (queue.Count == 0)
                {
                    System.Threading.Thread.Sleep(500);
                }
                else
                {
                    ThreadPool.QueueUserWorkItem(CrawlLink, queue.Dequeue());
                    //CrawlLink(queue.Dequeue());
                    crawledPages++;
                    Console.WriteLine(crawledPages + " ---> " + links);
                }
            }
        }

        private static void CrawlLink(object input)
        {
            WebRequest myWebRequest;
            WebResponse myWebResponse;

            string startUrl = (string)input;

            myWebRequest = WebRequest.Create(startUrl);
            myWebResponse = myWebRequest.GetResponse();

            Stream streamResponse = myWebResponse.GetResponseStream();

            StreamReader sreader = new StreamReader(streamResponse);
            Rstring = sreader.ReadToEnd();

            Regex regexLink = new Regex("(?<=<a\\s*?href=(?:'|\"))[^'\"]*?(?=(?:'|\"))");
            foreach (var match in regexLink.Matches(Rstring))
            {
                if (!queue.Contains(match.ToString()))
                {
                    Uri uriResult;
                    if (Uri.TryCreate(match.ToString(), UriKind.Absolute, out uriResult) && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps))
                    {
                        var newUser = new Link { Id = 456, Name = match.ToString() };
                        client.Cypher
                            .Merge("(user:User { Name: {name} })")
                            .OnCreate()
                            .Set("user = {newUser}")
                            .WithParams(new
                            {
                                name = newUser.Name,
                                newUser
                            })
                            .ExecuteWithoutResults();

                        //client.Cypher
                        //.Match("(user1:User)", "(user2:User)")
                        //.Where((User user1) => user1.Name == startUrl)
                        //.AndWhere((User user2) => user2.Name == match.ToString())
                        //.CreateUnique("user1-[:FRIENDS_WITH]->user2")
                        //.ExecuteWithoutResults();

                        queue.Enqueue(match.ToString());
                        links++;
                    }
                }
            }

            streamResponse.Close();
            sreader.Close();
            myWebResponse.Close();
        }

    }
}

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
using System.Threading;

namespace ProjectII.Service
{
    class Crawler
    {
        private static Int64 links { get; set; }
        public static string Rstring { get; set; }
        public static GraphClient client;
        private static Queue<string> queue = new Queue<string>();
        private Int64 crawledPages { get; set; }
        private Int64 numberOfPagesToCrawl { get; set; }

        public Crawler(GraphClient client, String url) {
            //this.client = client;
            this.crawledPages = 0;
            //this.links = 0;
            //this.queue = new Queue<string>();
            this.numberOfPagesToCrawl = 10000;

            queue.Enqueue(url);
            var newUser = new Link { Id = 456, Name = url };
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
            this.crawl();
        }

        public static void CallToChildThread()
        {
            string url = queue.Peek();
            WebRequest myWebRequest;
            WebResponse myWebResponse;
            try
            {
                myWebRequest = WebRequest.Create(url);
                myWebResponse = myWebRequest.GetResponse();//Returns a response from an Internet resource
                Stream streamResponse = myWebResponse.GetResponseStream();//return the data stream from the internet
                //and save it in the stream
                StreamReader sreader = new StreamReader(streamResponse);//reads the data stream
                Rstring = sreader.ReadToEnd();//reads it to the end
                GetContent(url, Rstring);
                streamResponse.Close();
                sreader.Close();
                myWebResponse.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Data);
            }
            queue.Dequeue();
        }

        public void crawl() {
            while (crawledPages < numberOfPagesToCrawl)
            {
                //this.WebRequestGetExample(queue.Dequeue());
                if (queue.Count > 0)
                {
                    new Thread(new ThreadStart(CallToChildThread)).Start();
                    crawledPages++;
                    Console.WriteLine(crawledPages+" ----> "+links);
                }
            }
        }

        public void WebRequestGetExample(string url)
        {
            WebRequest myWebRequest;
            WebResponse myWebResponse;

            myWebRequest = WebRequest.Create(url);
            myWebResponse = myWebRequest.GetResponse();//Returns a response from an Internet resource

            Stream streamResponse = myWebResponse.GetResponseStream();//return the data stream from the internet
            //and save it in the stream

            StreamReader sreader = new StreamReader(streamResponse);//reads the data stream
            Rstring = sreader.ReadToEnd();//reads it to the end
            GetContent(url, Rstring);
            streamResponse.Close();
            sreader.Close();
            myWebResponse.Close();
        }

        private static void GetContent(string startUrl, string content)
        {
            Regex regexLink = new Regex("(?<=<a\\s*?href=(?:'|\"))[^'\"]*?(?=(?:'|\"))");
            foreach (var match in regexLink.Matches(content))
            {
                if (!queue.Contains(match.ToString()))
                {
                    Uri uriResult;
                    if (Uri.TryCreate(match.ToString(), UriKind.Absolute, out uriResult)&& (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps))
                    {
                        //Console.WriteLine(match);
                        saveURL(startUrl, match.ToString());
                        queue.Enqueue(match.ToString());
                        links++;
                        //Console.WriteLine(links);
                    }
                }
            }

        }

        private static void saveURL(string startUrl, string url) {
            URL newUrl = new URL();
            newUrl.link = url;
            newUrl.visited = true;
            //IDictionary<string, object> list = new Dictionary<string, object>();
            //list.Add("newUrl", newUrl);
            //list.Add("link", newUrl.link);
            //var updateQuerry = client.Cypher.Merge("(url:URL {link: {link} })").OnCreate().Set("url = {newUrl}").WithParams(new { link = newUrl.link, newUrl });
            //updateQuerry.ExecuteWithoutResults();

            //var link = client.Cypher
            //.Match("(invitee:URL)")
            //.Where((URL invitee) => invitee.link == startUrl)
            //.Create("invitee-[:INVITED]->(invited:URL {newUser})")
            //.WithParam("newUser", newUrl);

            //link.ExecuteWithoutResults();

            //var results = client.Cypher
            //.Match("(url3:URL)")
            //.Return(url3 => url3.As<URL>())
            //.Results;
            //Console.WriteLine(results);
            //var newUser = new User { Id = 456, Name = url };
            //client.Cypher
            //    .Merge("(user:User { Name: {name} })")
            //    .OnCreate()
            //    .Set("user = {newUser}")
            //    .WithParams(new
            //    {
            //        name = newUser.Name,
            //        newUser
            //    })
            //    .ExecuteWithoutResults();

            var newUser = new Link { Id = 456, Name = url };
            client.Cypher
                .Merge("(user:Link { Name: {name} })")
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
            //.AndWhere((User user2) => user2.Name == url)
            //.Create("user1-[:KNOWS]->user2")
            //.ExecuteWithoutResults();

            //client.Cypher
            //.Match("(user1:User)", "(user2:User)")
            //.Where((User user1) => user1.Name == startUrl)
            //.AndWhere((User user2) => user2.Name == url)
            //.CreateUnique("user1-[:FRIENDS_WITH]->user2")
            //.ExecuteWithoutResults();

            //var newUser = new User { Id = 456, Name = url };
            //client.Cypher
            //    .Match("(invitee:User)")
            //    .Where((User invitee) => invitee.Name == startUrl)
            //    .Create("invitee-[:KNOWS]->(invited:User {newUser})")
            //    .WithParam("newUser", newUser)
            //    .ExecuteWithoutResults();
        }
    }
}

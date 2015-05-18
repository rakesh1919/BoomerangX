using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BoomerangX.DataFetcher
{
    public class Helper
    {
        string oauth_token;
        string oauth_token_secret;
        string oauth_consumer_key;
        string oauth_consumer_secret;

        public Helper()
        {
            oauth_token = "68413144-lhQNHkzAlAQFVYs02vaETaxOQYFCcEqEt7UPZviB7";
            oauth_token_secret = "qzFfjT9bJ5a62iefPYhC7hD8z01IZnGwE4heVNFKq4Vny";
            oauth_consumer_key = "iu9puupNVaHEvLGVKs5E59Pi3";
            oauth_consumer_secret = "Ex7hrEMcijEIKPD7apYUPJMaJyKAEExhh4jqAFX7TMLZKVfEU0";
        }

        public List<int> findTweetCount(string resource_url, string q)
        {

            // oauth implementation details
            var oauth_version = "1.0";
            var oauth_signature_method = "HMAC-SHA1";
            Logger.Log("Query String for Search is : " + q);
            // unique request details
            var oauth_nonce = Convert.ToBase64String(new ASCIIEncoding().GetBytes(DateTime.Now.Ticks.ToString()));
            var timeSpan = DateTime.UtcNow
                - new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            var oauth_timestamp = Convert.ToInt64(timeSpan.TotalSeconds).ToString();


            // create oauth signature
            var baseFormat = "oauth_consumer_key={0}&oauth_nonce={1}&oauth_signature_method={2}" +
                            "&oauth_timestamp={3}&oauth_token={4}&oauth_version={5}&q={6}";

            var baseString = string.Format(baseFormat,
                                        oauth_consumer_key,
                                        oauth_nonce,
                                        oauth_signature_method,
                                        oauth_timestamp,
                                        oauth_token,
                                        oauth_version,
                                        Uri.EscapeDataString(q)
                                        );

            baseString = string.Concat("GET&", Uri.EscapeDataString(resource_url), "&", Uri.EscapeDataString(baseString));

            var compositeKey = string.Concat(Uri.EscapeDataString(oauth_consumer_secret),
                                    "&", Uri.EscapeDataString(oauth_token_secret));

            string oauth_signature;
            using (HMACSHA1 hasher = new HMACSHA1(ASCIIEncoding.ASCII.GetBytes(compositeKey)))
            {
                oauth_signature = Convert.ToBase64String(
                    hasher.ComputeHash(ASCIIEncoding.ASCII.GetBytes(baseString)));
            }

            // create the request header
            var headerFormat = "OAuth oauth_nonce=\"{0}\", oauth_signature_method=\"{1}\", " +
                               "oauth_timestamp=\"{2}\", oauth_consumer_key=\"{3}\", " +
                               "oauth_token=\"{4}\", oauth_signature=\"{5}\", " +
                               "oauth_version=\"{6}\"";

            var authHeader = string.Format(headerFormat,
                                    Uri.EscapeDataString(oauth_nonce),
                                    Uri.EscapeDataString(oauth_signature_method),
                                    Uri.EscapeDataString(oauth_timestamp),
                                    Uri.EscapeDataString(oauth_consumer_key),
                                    Uri.EscapeDataString(oauth_token),
                                    Uri.EscapeDataString(oauth_signature),
                                    Uri.EscapeDataString(oauth_version)
                            );



            ServicePointManager.Expect100Continue = false;

            // make the request
            var postBody = "q=" + Uri.EscapeDataString(q);//
            resource_url += "?" + postBody;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(resource_url);
            request.Headers.Add("Authorization", authHeader);
            request.Method = "GET";
            request.ContentType = "application/x-www-form-urlencoded";
            var response = (HttpWebResponse)request.GetResponse();
            var reader = new StreamReader(response.GetResponseStream());
            var objText = reader.ReadToEnd();
            int retweetcount = 0;
            int tweetcount = 0;
           
            try
            {
                JArray jsonDat = JArray.Parse(objText);
                tweetcount = jsonDat.Count();
                for (int x = 0; x < jsonDat.Count(); x++)
                {
                    //html += jsonDat[x]["id"].ToString() + "<br/>";\
                    string temp = jsonDat[x].ToString();
                    string[] splitted = temp.Split(',');

                    int cnt = 0;
                    foreach (string s in splitted)
                    {
                        cnt++;
                        if (s.Contains("text\": ") && s.Length > 100)
                        {
                            Logger.Log(s);
                            System.Threading.Thread.Sleep(1000);
                        }
                        if (s.Contains("retweet_count"))
                        {
                            temp = s;
                            retweetcount += Convert.ToInt32(temp.Split(':').ElementAt(1));
                        }
                    }

                   
                }

            }
            catch (Exception twit_error)
            {
                Logger.Log("parsing error" + twit_error);
            }

            Logger.Log(string.Format("TweetCount  =  {0}  RetweetCount  = {1}", tweetcount, retweetcount));
            System.Threading.Thread.Sleep(1000);
            return new List<int> { tweetcount, retweetcount};
        }

         public List<int> findTweetTrend()
        {
            string resource_url = "https://api.twitter.com/1.1/trends/place.json";
            // oauth implementation details
            var oauth_version = "1.0";
            var oauth_signature_method = "HMAC-SHA1";
            
            // unique request details
            var oauth_nonce = Convert.ToBase64String(new ASCIIEncoding().GetBytes(DateTime.Now.Ticks.ToString()));
            var timeSpan = DateTime.UtcNow
                - new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            var oauth_timestamp = Convert.ToInt64(timeSpan.TotalSeconds).ToString();


            // create oauth signature
            var baseFormat = "id=1&oauth_consumer_key={0}&oauth_nonce={1}&oauth_signature_method={2}" +
                            "&oauth_timestamp={3}&oauth_token={4}&oauth_version={5}";

            var baseString = string.Format(baseFormat,
                                        oauth_consumer_key,
                                        oauth_nonce,
                                        oauth_signature_method,
                                        oauth_timestamp,
                                        oauth_token,
                                        oauth_version
                                        );

            baseString = string.Concat("GET&", Uri.EscapeDataString(resource_url), "&", Uri.EscapeDataString(baseString));

            var compositeKey = string.Concat(Uri.EscapeDataString(oauth_consumer_secret),
                                    "&", Uri.EscapeDataString(oauth_token_secret));

            string oauth_signature;
            using (HMACSHA1 hasher = new HMACSHA1(ASCIIEncoding.ASCII.GetBytes(compositeKey)))
            {
                oauth_signature = Convert.ToBase64String(
                    hasher.ComputeHash(ASCIIEncoding.ASCII.GetBytes(baseString)));
            }

            // create the request header
            var headerFormat = "OAuth oauth_nonce=\"{0}\", oauth_signature_method=\"{1}\", " +
                               "oauth_timestamp=\"{2}\", oauth_consumer_key=\"{3}\", " +
                               "oauth_token=\"{4}\", oauth_signature=\"{5}\", " +
                               "oauth_version=\"{6}\"";

            var authHeader = string.Format(headerFormat,
                                    Uri.EscapeDataString(oauth_nonce),
                                    Uri.EscapeDataString(oauth_signature_method),
                                    Uri.EscapeDataString(oauth_timestamp),
                                    Uri.EscapeDataString(oauth_consumer_key),
                                    Uri.EscapeDataString(oauth_token),
                                    Uri.EscapeDataString(oauth_signature),
                                    Uri.EscapeDataString(oauth_version)
                            );


            ServicePointManager.Expect100Continue = false;

            // make the request
           //  var postBody = "q=" + Uri.EscapeDataString(q);//
            resource_url = resource_url + "?id=1";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(resource_url);
            request.Headers.Add("Authorization", authHeader);
            request.Method = "GET";
            request.ContentType = "application/x-www-form-urlencoded";
            var response = (HttpWebResponse)request.GetResponse();
            var reader = new StreamReader(response.GetResponseStream());
            var objText = reader.ReadToEnd();
            int retweetcount = 0;
            int tweetcount = 0;
            try
            {
                JArray jsonDat = JArray.Parse(objText);
                tweetcount = jsonDat.Count();
                for (int x = 0; x < jsonDat.Count(); x++)
                {
                    //html += jsonDat[x]["id"].ToString() + "<br/>";\
                    string temp = jsonDat[x].ToString();
                    string[] splitted = temp.Split(',');
                    foreach (string s in splitted)
                    {
                        if (s.Contains("retweet"))
                        {
                            temp = s;
                            break;
                        }
                    }

                    retweetcount += Convert.ToInt32(temp.Split(':').ElementAt(1));
                }

            }
            catch (Exception twit_error)
            {
                Logger.Log("parsing error" + twit_error);
            }

            return new List<int> { tweetcount, retweetcount };
        }

    }
}

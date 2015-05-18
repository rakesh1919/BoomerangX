
namespace BoomerangX.DataFetcher
{
    using BoomerangX.Utils;
    using System;
    using System.Collections.Generic;
    using System.Data.SqlClient;
    public static class DataFetcher
    {
        static void Main(string[] args)
        {
            /*
            Helper hp = new Helper();
            string query = "#FIFA";
            string url = "https://api.twitter.com/1.1/users/search.json";
             */
            if(args.Length > 0 && args[0] == "true")
                 Logger.showlog = true;
            StartOperations();
        }


        public static void StartOperations()
        {
            Logger.Log("...connecting to Twitter...");
            System.Threading.Thread.Sleep(500);
            Logger.Log("Connected to Twitter..");
            Logger.Log("Searching Queries");
            SqlOperations op = new SqlOperations();
            SqlDataReader reader  = op.ExecuteQuery("select ProductName from Product");
            List<string> tags = new List<string>();
            while(reader.Read())
            {
                string name = reader[0].ToString();
                tags.Add(name);
            }
            
            for (int i  =0; i <tags.Count; i++)
            {
                Populate(tags[i]);
            }
            // get the last time stamp of the product 
        }

        public static void Populate(string productName)
        {
          
            List<string> query = CreateSearchquery(productName);
            foreach (string searchTag in query)
            {
                string searchQuery = "select Id from dbo.Product where [ProductName] = '{0}'";
                string insertQuery = "Insert Into ProductStaging(id, productid, RetweetCount, TweetCount) Values ( {0}, {1}, {2}, {3})";

                searchQuery = string.Format(searchQuery, productName);
                SqlOperations op = new SqlOperations();
                // get productid
                long pid = 0;
                SqlDataReader reader = op.ExecuteQuery(searchQuery);
                if (reader != null)
                {
                    reader.Read();
                    pid = Convert.ToInt32(reader[0].ToString());
                    reader.Close();
                }

                searchQuery = "select ISNULL(max(id), 0) +1 from ProductStaging";
                reader = op.ExecuteQuery(searchQuery);
                int pkey = 1;
                if (reader != null)
                {
                    reader.Read();
                    pkey = Convert.ToInt32(reader[0].ToString());
                    reader.Close();
                }

                Helper hp = new Helper();
                List<int> v = hp.findTweetCount("https://api.twitter.com/1.1/users/search.json", searchTag);
                insertQuery = string.Format(insertQuery, pkey, pid, v[1], v[0]);
                op.ExecuteQuery(insertQuery);
            }
        }

        public static List<string> CreateSearchquery(string productName)
        {
            string[] comb = { "#Soccer", "#Fifa" };
            List<string> searchTags = new List<string>();
            for (int i = 0; i < comb.Length; i++)
            {
                searchTags.Add(productName + " And " + comb[i]);
            }

            return searchTags;
        }
    }
}

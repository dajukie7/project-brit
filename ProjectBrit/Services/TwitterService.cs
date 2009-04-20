using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Xml.Linq;

namespace ProjectBrit.Services
{
    public class TwitterService
    {
        public XDocument GetUserStatuses(string twitterName)
        {
            return GetUserStatuses(twitterName, null, null);
        }

        public XDocument GetUserStatuses(string twitterName, string username, string password)
        {
            string url = "http://twitter.com/statuses/user_timeline/" + twitterName + ".xml";
            HttpWebRequest twitterRequest = (HttpWebRequest)HttpWebRequest.Create(url);
            twitterRequest.Method = "GET";

            if(!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
            {
                twitterRequest.Credentials = new NetworkCredential(username, password);
            }

            WebResponse twitterResponse = twitterRequest.GetResponse();
            StreamReader reader = new StreamReader(twitterResponse.GetResponseStream());
            string responsePostString = reader.ReadToEnd();
            reader.Close();
            XDocument document = XDocument.Parse(responsePostString);
            return document;
        }
    }
}
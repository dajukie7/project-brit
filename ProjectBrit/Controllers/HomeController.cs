using System;
using System.Drawing;
using System.IO;
using System.Net;
using System.Text;
using System.Xml.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProjectBrit.Data;
using ProjectBrit.Models;

namespace ProjectBrit.Controllers
{
    [HandleError]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewData["Message"] = "Welcome to ASP.NET MVC!";
            Image img = Image.FromFile("..\\background.jpg");
            byte[] imageBytes = ImageConvert(img);

            //UploadPhoto(imageBytes, String.Empty, img, "background.jpg");

            return View();
        }

        private byte[] ImageConvert(Image img)
        {
            MemoryStream ms = new MemoryStream();
            img.Save(ms,img.RawFormat);
            return ms.ToArray();
        }

        [OutputCache(Duration = 10, VaryByParam = "none")]
        public ActionResult About()
        {

            HttpWebRequest dajukie7Request = (HttpWebRequest)HttpWebRequest.Create("http://twitter.com/statuses/user_timeline.xml");
            dajukie7Request.Credentials = new NetworkCredential("dajukie7", "russ3l");
            dajukie7Request.Method = "GET";

            WebResponse dajukie7Response = dajukie7Request.GetResponse();
            StreamReader reader = new StreamReader(dajukie7Response.GetResponseStream());
            string responsePostString = reader.ReadToEnd();
            reader.Close();
            XDocument document = XDocument.Parse(responsePostString);
            string id = document.Element("statuses").Elements("status").Last().Element("id").Value;

            WebRequest request = WebRequest.Create("http://twitter.com/statuses/user_timeline/britneemichele.xml");
            WebResponse response = (WebResponse)request.GetResponse();
            Stream dataStream = response.GetResponseStream();
            // Open the stream using a StreamReader for easy access.
            reader = new StreamReader(dataStream);
            XDocument doc2 = XDocument.Parse(reader.ReadToEnd());
            // Read the content.
          
            string responseFromServer = reader.ReadToEnd();
            reader.Close();

            foreach(XElement status in document.Descendants("status"))
            {
                doc2.Element("statuses").Add(status);
            }

            var query = from s in doc2.Descendants("status")
                        orderby ConvertTwitterDate(s.Element("created_at").Value) descending
                        select new Status
                        {
                            CreatedOn = ConvertTwitterDate(s.Element("created_at").Value),
                            UserName = s.Element("user").Element("name").Value,
                            Id = s.Element("id").Value,
                            Message = s.Element("text").Value
                        };
            StatusesViewModel model = new StatusesViewModel(query.ToList());
            return View(model);
        }

        public DateTime ConvertTwitterDate(string val)
        {
            var datePieces = val.Split(' ');
            string date = datePieces[1] + " " + datePieces[2] + " " + datePieces[5] + " " + datePieces[3];
            return DateTime.Parse(date);

        }
            
        /// <summary>
        /// URL for the TwitPic API's upload method
        /// </summary>
        private const string TWITPIC_UPLADO_API_URL = "http://twitpic.com/api/upload";

        /// <summary>
        /// URL for the TwitPic API's upload and post method
        /// </summary>
        private const string TWITPIC_UPLOAD_AND_POST_API_URL = "http://twitpic.com/api/uploadAndPost";

        /// <summary>
        /// Uploads the photo and sends a new Tweet
        /// </summary>
        /// <param name="binaryImageData">The binary image data.</param>
        /// <param name="tweetMessage">The tweet message.</param>
        /// <param name="image">The Actual Image</param>
        /// <param name="filename">The filename.</param>
        /// <returns>Return true, if the operation was succeded.</returns>
        public bool UploadPhoto(byte[] binaryImageData, string tweetMessage, Image image, string filename)
        {
          // Documentation: http://www.twitpic.com/api.do
          string boundary = Guid.NewGuid().ToString();
          string requestUrl = String.IsNullOrEmpty(tweetMessage) ? TWITPIC_UPLADO_API_URL : TWITPIC_UPLOAD_AND_POST_API_URL;
          HttpWebRequest request = (HttpWebRequest)WebRequest.Create(requestUrl);
          string encoding = "iso-8859-1";

          request.PreAuthenticate = true;
          request.AllowWriteStreamBuffering = true;
          request.ContentType = string.Format("multipart/form-data; boundary={0}", boundary);
          request.Method = "POST";

          string header = string.Format("--{0}", boundary);
          string footer = string.Format("--{0}--", boundary);

          StringBuilder contents = new StringBuilder();
          contents.AppendLine(header);

          string fileContentType = image.RawFormat.ToString();
          string fileHeader = String.Format("Content-Disposition: file; name=\"{0}\"; filename=\"{1}\"", "media", filename);
          string fileData = Encoding.GetEncoding(encoding).GetString(binaryImageData);

          contents.AppendLine(fileHeader);
          contents.AppendLine(String.Format("Content-Type: {0}", fileContentType));
          contents.AppendLine();
          contents.AppendLine(fileData);

          contents.AppendLine(header);
          contents.AppendLine(String.Format("Content-Disposition: form-data; name=\"{0}\"", "username"));
          contents.AppendLine();
          contents.AppendLine("dajukie7");

          contents.AppendLine(header);
          contents.AppendLine(String.Format("Content-Disposition: form-data; name=\"{0}\"", "password"));
          contents.AppendLine();
          contents.AppendLine("russ3l");

          if (!String.IsNullOrEmpty(tweetMessage))
          {
            contents.AppendLine(header);
            contents.AppendLine(String.Format("Content-Disposition: form-data; name=\"{0}\"", "message"));
            contents.AppendLine();
            contents.AppendLine(tweetMessage);
          }

          contents.AppendLine(footer);

          byte[] bytes = Encoding.GetEncoding(encoding).GetBytes(contents.ToString());
          request.ContentLength = bytes.Length;

          using (Stream requestStream = request.GetRequestStream())
          {
            requestStream.Write(bytes, 0, bytes.Length);

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
              using (StreamReader reader = new StreamReader(response.GetResponseStream()))
              {
                string result = reader.ReadToEnd();

                XDocument doc = XDocument.Parse(result);

                XElement rsp = doc.Element("rsp");
                string status = rsp.Attribute(XName.Get("status")) != null ? rsp.Attribute(XName.Get("status")).Value : rsp.Attribute(XName.Get("stat")).Value;

                return status.ToUpperInvariant().Equals("OK");
              }
            }
          }
        }
    }
}

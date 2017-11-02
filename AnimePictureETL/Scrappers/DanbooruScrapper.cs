using AnimePictureETL.ExtractedData;
using AnimePictureETL.ExtractedData.Danbooru;
using AnimePictureETL.Models;
using AnimePictureETL.Repositories;
using HtmlAgilityPack;
using log4net;
using ScrapySharp.Network;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace AnimePictureETL.Scrappers
{
    public class DanbooruScrapper
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static readonly string WEB_ADDRESS = @"https://danbooru.donmai.us";
        private static readonly string POSTS_ADDRESS = @"/posts";

        private static readonly string TAG_LIST_ID = "tag-list";
        private static readonly string VALUE_NODE_CLASS = "search-tag";
        private static readonly string INFORMATION_ID = "post-information";

        private static readonly string SERIES_CLASS = "category-3";
        private static readonly string CHARACTER_CLASS = "category-4";
        private static readonly string ARTIST_CLASS = "category-1";
        private static readonly string TAG_CLASS = "category-0";

        public static readonly string DOWNLOAD_DEST = @"D:\AnimeWorks";

        private static ScrapingBrowser _browser = null;

        private static ScrapingBrowser BrowserInstance()
        {
            if (_browser == null)
                _browser = new ScrapingBrowser();
            return _browser;
        }

        private static string _downloadDest = null;

        public DanbooruScrapper()
        {
            _downloadDest = DOWNLOAD_DEST;
        }

        public DanbooruScrapper(string downloadDest)
        {
            _downloadDest = downloadDest;
        }

        public IList<Post> ExtractPicturesByTag(string tag1, string tag2 = "")
        {
            HtmlWeb web = new HtmlWeb();
            bool noPages = false;
            int index = 1;
            NameValueCollection args = new NameValueCollection()
            {
                { "utf8", "\\u2713" },
                { "tags", tag1 + ' ' + tag2}
            };
            List<string> links = new List<string>();
            while (!noPages)
            {
                args.Set("page", index.ToString());
                string argStr = ToQueryString(args);
                HtmlDocument doc = web.Load(WEB_ADDRESS + POSTS_ADDRESS + '?' + argStr);
                var articles = doc.DocumentNode.Descendants("article");
                if (articles.Count() == 0)
                    noPages = true;
                else
                {
                    string[] linksToPosts = GetPageLinks(articles);
                    links.AddRange(linksToPosts);
                    ++index;
                }
                if (index == 4)
                    break;
            }
            if (links.Count() == 0)
                return new List<Post>();
            var picturePosts = GetPostsDocuments(links);
            IList<Post> pictures = ScrapPicturesFromPosts(picturePosts);
            return pictures;
        }

        public Post ExtractPictureById(long id)
        {
            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc = web.Load(WEB_ADDRESS + POSTS_ADDRESS + '/' + id.ToString());
            var picturePost = ScrapPictureFromPost(doc);
            return picturePost;
        }

        private string[] GetPageLinks(IEnumerable<HtmlNode> articles)
        {
            string[] linksToPosts = new string[articles.Count()];
            int index = 0;
            foreach (var article in articles)
            {
                var aTag = article.FirstChild;
                string hrefValue = aTag.GetAttributeValue("href", string.Empty);
                if (hrefValue != string.Empty)
                {
                    int questionIndex = hrefValue.IndexOf('?');
                    string linkWithoutArgs = hrefValue;
                    if (questionIndex != -1)
                        linkWithoutArgs = hrefValue.Substring(0, hrefValue.IndexOf('?'));
                    linksToPosts[index++] = linkWithoutArgs;
                }
            }
            return linksToPosts;
        }

        private IList<HtmlDocument> GetPostsDocuments(IList<string> linksToPosts)
        {
            var postDocs = new List<HtmlDocument>();
            HtmlWeb web = new HtmlWeb();
            foreach (var link in linksToPosts)
            {
                string fullLink = WEB_ADDRESS + link;
                HtmlDocument post = web.Load(fullLink);
                postDocs.Add(post);
            }
            return postDocs;
        }

        private static string ToQueryString(NameValueCollection nvc)
        {
            var array = (from key in nvc.AllKeys
                         from value in nvc.GetValues(key)
                         select string.Format("{0}={1}", HttpUtility.UrlEncode(key), HttpUtility.UrlEncode(value)))
                         .ToArray();
            return string.Join("&", array);
        }

        private IList<Post> ScrapPicturesFromPosts(IList<HtmlDocument> posts)
        {
            IList<Post> pictures = new List<Post>();
            foreach (var post in posts)
            {
                Post picture = ScrapPictureFromPost(post);
                if (picture != null)
                    pictures.Add(picture);
            }
            return pictures;
        }

        private Post ScrapPictureFromPost(HtmlDocument post)
        {
            var informationNode = post.GetElementbyId(INFORMATION_ID);
            var idNode = informationNode.Descendants().Where(n => n.InnerText.Contains("ID: ") && n.NodeType == HtmlNodeType.Text).Single();
            string danbooruIdStr = idNode.InnerText.Substring(idNode.InnerText.IndexOf(' ') + 1);
            int danbooruId = Convert.ToInt32(danbooruIdStr);

            log.Info("Starting scrapping post of ID: " + danbooruIdStr);

            bool pictureExists = false;
            Post data = new Post { Exists = true };
            Picture picture = null;
            using (PictureRepository repository = new PictureRepository())
            {
                pictureExists = repository.DoPictureExists(danbooruId, PictureRepository.SourceType.Danbooru);
                if (pictureExists)
                {
                    log.Info("Picture of ID: " + danbooruIdStr + "already exists");
                    picture = repository.GetBySourceId(danbooruId, PictureRepository.SourceType.Danbooru);
                }
            }
            if (pictureExists)
                return null;
            var widthNode = informationNode.Descendants().Where(n => n.GetAttributeValue("itemprop", "") == "width").Single();
            string width = widthNode.InnerText;
            var heightNode = informationNode.Descendants().Where(n => n.GetAttributeValue("itemprop", "") == "height").Single();
            string height = heightNode.InnerText;
            string linkToBlob = widthNode.PreviousSibling.PreviousSibling.GetAttributeValue("href", "");

            var sourceNodes = informationNode.Descendants().Where(n => n.InnerText == "Source: " && n.Name == "#text");
            string source;
            if (sourceNodes.Count() != 0)
            {
                var sourceNode = sourceNodes.Single();
                var sourceText = sourceNode.NextSibling;
                if (sourceText != null)
                    source = sourceNode.NextSibling.GetAttributeValue("href", "");
                else
                    source = string.Empty;
            }
            else
                source = WEB_ADDRESS + POSTS_ADDRESS + '/' + danbooruIdStr;
            string link = WEB_ADDRESS + linkToBlob;

            int pointIndex = linkToBlob.LastIndexOf('.');
            string extension = linkToBlob.Substring(pointIndex + 1);
            string location = _downloadDest + @"\tmp." + extension;

            using (var client = new WebClient())
            {
                client.DownloadFile(link, location);
            }

            string checksum = GetMd5ChecksumForPicture(location);
            string fileName = checksum + '.' + extension;
            string newLocation = _downloadDest + @"\" + fileName;
            if (File.Exists(newLocation))
                log.Warn("Extract: Danbooru ID: " + danbooruIdStr + ", there already exists a picture with the MD5 checksum: " + checksum);
            else
                File.Move(location, newLocation);

            data.Exists = false;
            data.Width = Convert.ToInt16(width);
            data.Height = Convert.ToInt16(height);
            data.Source = source;
            data.DanbooruId = danbooruId;
            data.FileName = fileName;
            data.Checksum = checksum;
            data.Size = new FileInfo(newLocation).Length;

            var tagListNode = post.GetElementbyId(TAG_LIST_ID);
            string[] series = ExtractTags(tagListNode, SERIES_CLASS);
            string[] characters = ExtractTags(tagListNode, CHARACTER_CLASS);
            string[] artists = ExtractTags(tagListNode, ARTIST_CLASS);
            string[] tags = ExtractTags(tagListNode, TAG_CLASS);

            data.Series = series;
            data.Characters = characters;
            data.Artists = artists;
            data.Tags = tags;

            log.Info("Extracting data of a picture of ID: " + danbooruIdStr + "succeeded");

            return data;
        }

        private string[] ExtractTags(HtmlNode tagListNode, string tagType)
        {
            var tagNodes = tagListNode.Descendants().Where(n => n.GetAttributeValue("class", "") == tagType);
            string[] tags = new string[tagNodes.Count()];
            int index = 0;
            foreach (var tagNode in tagNodes)
            {
                var searchTag = tagNode.Descendants().Where(n => n.GetAttributeValue("class", "") == VALUE_NODE_CLASS).SingleOrDefault();
                tags[index++] = searchTag.InnerText;
            }
            return tags;
        }

        private string GetMd5ChecksumForPicture(string path)
        {
            byte[] bytes = File.ReadAllBytes(path);
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            byte[] hash = md5.ComputeHash(bytes);
            StringBuilder sb = new StringBuilder();
            foreach (byte b in hash)
            {
                sb.Append(b.ToString("x2").ToLower());
            }
            string hashString = sb.ToString();
            return hashString;
        }


        public Bitmap ResizeImage(Image image, short width, short height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }
    }
}
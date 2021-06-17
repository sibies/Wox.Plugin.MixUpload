using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;

namespace Wox.Plugin.MixUpload
{
    public class MixUploadClient
    {
        const string Address = "http://mixupload.com/search/quick";
        private PluginInitContext _context;

        public MixUploadClient(PluginInitContext context)
        {
            _context = context;
        }

        public List<Result> Search(string keyword)
        {
            List<Result> results = new List<Result>();

            using (WebClient client = new WebClient())
            {
                var reqparm = new NameValueCollection
                {
                    { "q", keyword }
                };
                byte[] responsebytes = client.UploadValues(Address, "POST", reqparm);
                string responsebody = Encoding.UTF8.GetString(responsebytes);
                var doc = new HtmlDocument();
                doc.LoadHtml(responsebody);
                var anchors = doc.DocumentNode.SelectNodes("//li/a");
                foreach (var anchor in anchors)
                {
                    var link = anchor.Attributes["href"].Value;

                    var result = new Result
                    {
                        Action = (ActionContext context) =>
                        {
                            Process.Start(string.Format("http://mixupload.com{0}", link));
                            return true;
                        }
                    };

                    var titleWrapper = anchor.SelectSingleNode(".//div/div");
                    if (titleWrapper.HasChildNodes && titleWrapper.ChildNodes.Count >= 2)
                    {
                        var t = titleWrapper.SelectNodes(".//p");
                        result.Title = t[0].InnerText.Trim();
                        result.SubTitle = t[1].InnerText.Trim();
                    }
                    else
                    {
                        result.Title = titleWrapper.InnerText.Trim();
                    }
                    var img = anchor.SelectSingleNode(".//div/img").Attributes["src"].Value;
                    if (img.EndsWith("default.png"))
                    {
                        result.IcoPath = "Images\\default.png";
                    }
                    else
                    {
                        result.IcoPath = GetCover(anchor.SelectSingleNode(".//div/img").Attributes["src"].Value, result.Title);
                    }

                    results.Add(result);
                }
            }

            return results;
        }

        public string GetCover(string href, string title)
        {
            if (!Directory.Exists(this.CacheFodler))
            {
                Directory.CreateDirectory(this.CacheFodler);
            }
            var ext = Path.GetExtension(href) ?? ".jpg";
            string text = string.Format("{0}\\{1}{2}", CacheFodler, title, ext);
            try
            {
                if (!File.Exists(text))
                {
                    new WebClient().DownloadFile(new Uri(href), text);
                }
            }
            catch (Exception e)
            {
                return _context.CurrentPluginMetadata.IcoPath;
            }
            return text;
        }

        private string CacheFodler
        {
            get
            {
#if DEBUG
                return Path.Combine("D:\\", "Cache");
#else
                return Path.Combine(_context.CurrentPluginMetadata.PluginDirectory, "Cache");
#endif
            }
        }

    }
}

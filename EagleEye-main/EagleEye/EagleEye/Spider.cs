using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Windows.Forms;

namespace EagleEye
{
    class Spider
    {
        public List<string> spiderUrl(string url)
        {
            HtmlWeb web;
            HtmlAgilityPack.HtmlDocument doc;
            HTTP http = new HTTP(); 

            if (url == null)
            {
                return null;
            }

            web = new HtmlWeb();
            doc = web.Load(url);

            List<string> scrapedUrls = new List<string>();

            foreach (HtmlNode link in doc.DocumentNode.SelectNodes("//a[@href]"))
            {
                string hrefValue = link.GetAttributeValue("href", string.Empty);
                scrapedUrls.Add(http.GetAbsoluteUrlString(url, hrefValue));
            }

            return scrapedUrls;
        }
    }
}

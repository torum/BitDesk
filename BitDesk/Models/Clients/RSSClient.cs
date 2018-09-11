using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Globalization;
using System.ServiceModel;
using System.Xml;
using System.ServiceModel.Syndication;


namespace BitDesk.Models.Clients
{
    public enum Langs
    {
        en, ja
    }

    public class Rss
    {
        private DateTime _timestamp;
        public DateTime TimeStamp
        {
            get
            {
                return _timestamp;
            }
            set
            {
                if (_timestamp == value)
                    return;

                _timestamp = value;

            }
        }

        private string _subject;
        public string Subject
        {
            get
            {
                return _subject;
            }
            set
            {
                if (_subject == value)
                    return;

                _subject = value;

            }
        }

        private string _domain;
        public string Domain
        {
            get
            {
                return _domain;
            }
            set
            {
                if (_domain == value)
                    return;

                _domain = value;

            }
        }


        private string _summary;
        public string Summary
        {
            get
            {
                return _summary;
            }
            set
            {
                if (_summary == value)
                    return;

                _summary = value;

            }
        }

        private string _link;
        public string Link
        {
            get
            {
                return _link;
            }
            set
            {
                if (_link == value)
                    return;

                _link = value;

            }
        }

        public Rss()
        {

        }

    }

    public class RssResult
    {
        public List<Rss> RssList = new List<Rss>();

    }

    public class RSSClient : BaseClient
    {
        // 日本語版
        protected Uri _endpoint1 = new Uri ("https://news.google.com/news/rss/search/section/q/%E3%83%93%E3%83%83%E3%83%88%E3%82%B3%E3%82%A4%E3%83%B3%7CBitcoin?ned=us&gl=US&hl=en");

        protected Uri _endpoint2 = new Uri("https://news.google.com/news/rss/search/section/q/%E3%83%93%E3%83%83%E3%83%88%E3%82%B3%E3%82%A4%E3%83%B3%7CBitcoin?ned=jp&gl=JP&hl=ja");
        // 英語版

        // RSS取得メソッド
        public async Task<RssResult> GetRSS(Langs lang)
        {
            RssResult rr = new RssResult();

            if (lang == Langs.en)
            {
                try
                {
                    XmlReader reader = XmlReader.Create(_endpoint1.ToString());
                    SyndicationFeed feed = SyndicationFeed.Load(reader);
                    reader.Close();

                    foreach (SyndicationItem item in feed.Items)
                    {
                        Rss r = new Rss();
                        r.Subject = item.Title.Text;
                        //r.Summary = item.Summary.Text;
                        if (item.Links.Count > 0)
                        {
                            r.Link = item.Links[0].Uri.AbsoluteUri;//.AbsolutePath;//.ToString();
                            r.Domain = item.Links[0].Uri.Host;
                        }
                        r.TimeStamp = item.PublishDate.LocalDateTime;
                        rr.RssList.Add(r);

                    }
                }
                catch { }
            }
            else if (lang == Langs.ja)
            {
                // 日本語版

                try
                {
                    XmlReader reader2 = XmlReader.Create(_endpoint2.ToString());
                    SyndicationFeed feed2 = SyndicationFeed.Load(reader2);
                    reader2.Close();

                    foreach (SyndicationItem item in feed2.Items)
                    {
                        Rss r = new Rss();
                        r.Subject = item.Title.Text;
                        //r.Summary = item.Summary.Text;
                        if (item.Links.Count > 0)
                        {
                            r.Link = item.Links[0].Uri.AbsoluteUri;//.ToString();
                            r.Domain = item.Links[0].Uri.Host;

                        }
                        r.TimeStamp = item.PublishDate.LocalDateTime;
                        rr.RssList.Add(r);

                    }
                }
                catch { }
            }

            return rr;

        }

    }

}

using Microsoft.Phone.BackgroundAudio;
using MusicPlayer.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.Xml;
using System.Xml.Linq;
using Windows.Storage;

namespace MusicPlayer.Data.YouTube
{
    public sealed class YouTubeProvider : IMusicProvider
    {
        private static XNamespace yt = "http://gdata.youtube.com/schemas/2007";
        private static XNamespace gd = "http://schemas.google.com/g/2005";
        private static XNamespace openSearch = "http://a9.com/-/spec/opensearch/1.1/";
        private static XNamespace media = "http://search.yahoo.com/mrss/";
        private static XNamespace xn = "http://www.w3.org/2005/Atom/";
        private static XName VideoIdXname = yt + "videoid";
        private static XName DurationXName = yt + "duration";
        private static XName TitleXName = media + "title";
        private static XName UploadedXName = yt + "uploaded";
        private static XName ThumbNailXName = media + "thumbnail";
        private static XName GroupXName = media + "group";
        private static string generalQuery = "http://gdata.youtube.com/feeds/api/videos?q=IN_QUERY&category=Music&safeSearch=moderate&orderby=viewCount&hl=en&v=2";

        private string GetQuery(QueryType type, string query)
        {
            if (type == QueryType.GENERAL)
            {
                return generalQuery.Replace("IN_QUERY", query);
            }

            return string.Empty;
        }

        public Uri GetThumbnailUri(string id, ThumbnailSize size = ThumbnailSize.MoviePoster)
        {
            switch (size)
            {
                case ThumbnailSize.Small:
                    return new Uri(string.Format(CultureInfo.InvariantCulture, "{0}{1}{2}", "http://img.youtube.com/vi/", id, "/default.jpg"), UriKind.Absolute);
                case ThumbnailSize.Medium:
                    return new Uri(string.Format(CultureInfo.InvariantCulture, "{0}{1}{2}", "http://img.youtube.com/vi/", id, "/hqdefault.jpg"), UriKind.Absolute);
                case ThumbnailSize.Large:
                    return new Uri(string.Format(CultureInfo.InvariantCulture, "{0}{1}{2}", "http://img.youtube.com/vi/", id, "/maxresdefault.jpg"), UriKind.Absolute);
                case ThumbnailSize.MoviePoster:
                    return new Uri(string.Format(CultureInfo.InvariantCulture, "{0}{1}{2}", "http://img.youtube.com/vi/", id, "/movieposter.jpg"), UriKind.Absolute);
                default:
                    return null;
            }
            throw new Exception();
        }

        public int GetQualityIdentifier(Quality quality)
        {
            switch (quality)
            {
                case Quality.Quality480P: return 18;
                case Quality.Quality720P: return 22;
                case Quality.Quality1080P: return 37;
            }

            throw new ArgumentException();
        }

        public void QueryMusic(QueryType type, string query, Action<List<IMusicItem>> response)
        {
            var videos = new List<IMusicItem>();
            var querySearch = GetQuery(type, query);
            HttpWebRequest webRequest = HttpWebRequest.CreateHttp(querySearch);
            webRequest.UserAgent = YouTubeMusicItem.__USERAGENT__;

            webRequest.BeginGetResponse(new AsyncCallback(obj =>
            {
                try
                {
                    var webResponse = (HttpWebResponse)webRequest.EndGetResponse(obj);
                    if (webResponse.StatusCode == HttpStatusCode.OK)
                    {
                        Stream responseStream = webResponse.GetResponseStream();
                        XDocument xdoc = XDocument.Load(responseStream, LoadOptions.None);
                        var properties = from node in xdoc.Descendants(GroupXName)
                                         select new
                                         {
                                             VideoId = node.Element(VideoIdXname).Value,
                                             DurationSecs = node.Element(DurationXName).FirstAttribute.Value,
                                             Title = node.Element(TitleXName).Value,
                                             UploadedTime = node.Element(UploadedXName).Value,
                                             UrlList = node.Descendants(ThumbNailXName).Select(x => x.FirstAttribute.Value).ToList(),
                                         };

                        foreach (var property in properties)
                        {
                            string thumbNailurl;
                            if (property.UrlList.Count == 7)
                                thumbNailurl = property.UrlList[6];
                            else
                                thumbNailurl = property.UrlList[2];
                            videos.Add(new YouTubeMusicItem(property.VideoId,
                                property.DurationSecs,
                                property.Title,
                                thumbNailurl,
                                null,
                                null,
                                property.UploadedTime));
                        }

                        response(videos);
                    }

                    response(null);
                }
                catch(Exception ex)
                {
                    Logger.LogException(ex);
                    response(null);
                }

            }), webRequest);
        }
    }
}

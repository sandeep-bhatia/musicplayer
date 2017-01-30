using Microsoft.Phone.BackgroundAudio;
using MusicPlayer.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace MusicPlayer.Data.YouTube
{
    public sealed class YouTubeMusicItem : IMusicItem
    {
        /// <summary>
        /// current track handle to the music item
        /// </summary>
        private AudioTrack track;

        /// <summary>
        /// Request Url for the current music item
        /// </summary>
        private string requestUrl;

        /// <summary>
        /// Is the current Music item downloaded locally
        /// </summary>
        private bool isDownloaded;

        /// <summary>
        /// Music item access Url
        /// </summary>
        private string accessUrl;

        /// <summary>
        /// Unique Id of this music item
        /// </summary>
        private string id;

        /// <summary>
        /// Duration in seconds for this music item
        /// </summary>
        private string durationSecs;

        /// <summary>
        /// Title for this music item
        /// </summary>
        private string title;

        /// <summary>
        /// Thumbnail Url for this music item
        /// </summary>
        private string thumbnailUrl;

        /// <summary>
        /// Description for this music item
        /// </summary>
        private string description;

        /// <summary>
        /// List of actors for this music item
        /// </summary>
        private List<string> actors;

        /// <summary>
        /// Last updated time for the music item
        /// </summary>
        private string lastUpdated;

        /// <summary>
        /// USER AGENT string for download
        /// </summary>
        internal const string __USERAGENT__ = "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; Win64; x64; Trident/6.0)";

        /// <summary>
        /// Stream Map constant
        /// </summary>
        private const string __STREAMMAP__ = "url_encoded_fmt_stream_map";

        /// <summary>
        /// Signature constant
        /// </summary>
        private const string __SIGNATUREQUERYPARAM__ = "&signature=";

        /// <summary>
        /// Fallback Host
        /// </summary>
        private const string __FALLBACKHOST__ = "&fallback_host=";

        /// <summary>
        /// Comma character constant
        /// </summary>
        private const char COMMA = ',';

        /// <summary>
        /// Equals character
        /// </summary>
        private const char EQUALS = '=';

        /// <summary>
        /// 3GP tag
        /// </summary>
        private const string __3GPTAG__ = "itag=17";

        /// <summary>
        /// Sig const
        /// </summary>
        private const string __SIG__ = "sig";

        /// <summary>
        /// URL string const
        /// </summary>
        private const string __URL__ = "url";

        /// <summary>
        /// fallback string const
        /// </summary>
        private const string __FALLBACK__ = "fallback_host";

        /// <summary>
        /// 3GP extension 
        /// </summary>
        private const string __3GPEXTENSION__ = ".3gp";

        private string ExtractDownloadUrls(string source)
        {
            var urls = new List<Uri>();
            string exUrl = null;
            var list = ParseFormDecoded(source);
            foreach (var kv in list)
            {
                if (kv[0] != __STREAMMAP__) continue;
                var list2 = ParseFormDecoded(kv[1], COMMA);
                foreach (var kv2 in list2)
                {
                    var list3 = ParseFormDecoded(kv2[1]);
                    string url = string.Empty;
                    string fallback_host = string.Empty;
                    string sig = string.Empty;
                    foreach (var kv3 in list3)
                    {
                        switch (kv3[0])
                        {
                            case __URL__:
                                url = kv3[1];
                                break;
                            case __FALLBACK__:
                                fallback_host = kv3[1];
                                break;
                            case __SIG__:
                                sig = kv3[1];
                                break;
                        }
                    }
                    if (url.IndexOf(__FALLBACKHOST__, StringComparison.Ordinal) < 0)
                        url += __FALLBACKHOST__ + WebUtility.UrlDecode(fallback_host);
                    if (url.IndexOf(__SIGNATUREQUERYPARAM__, StringComparison.Ordinal) < 0)
                        url += __SIGNATUREQUERYPARAM__ + WebUtility.UrlDecode(sig);
                    urls.Add(new Uri(url));
                    if (url.IndexOf(__3GPTAG__) > 0)
                    {
                        exUrl = url;
                    }
                }
            }
            return exUrl;
        }

        private List<string[]> ParseFormDecoded(string qs, char split = '&')
        {
            var arr = qs.Split(split);
            var list = new List<string[]>(arr.Length);
            foreach (var kv in arr)
            {
                if (split == COMMA)
                {
                    list.Add(new[] { string.Empty, kv });
                }
                else
                {
                    var akv = kv.Split(EQUALS);
                    var k = WebUtility.UrlDecode(akv[0]);
                    var v = WebUtility.UrlDecode(akv[1]);
                    list.Add(new[] { k, v });
                }
            }
            return list;
        }

        private async void DownloadFile(IAsyncResult obj, HttpWebRequest webRequest)
        {
            var webResponse = (HttpWebResponse)webRequest.EndGetResponse(obj);
            if (webResponse.StatusCode == HttpStatusCode.OK)
            {
                StorageFolder folder = ApplicationData.Current.LocalFolder;
                StorageFile file = await folder.CreateFileAsync(GetFileName(), CreationCollisionOption.ReplaceExisting);
                using (Stream s = await file.OpenStreamForWriteAsync())
                {
                    webResponse.GetResponseStream().CopyTo(s);
                    isDownloaded = true;
                }
            }
        }

        private string GetFileName()
        {
            return string.Format(CultureInfo.InvariantCulture, "{0}{1}", Id, __3GPEXTENSION__);
        }

        private async Task<string> ReadAppStorage()
        {
            try
            {
                StorageFolder folder = ApplicationData.Current.LocalFolder;
                StorageFile file = await folder.GetFileAsync(GetFileName());
                return file.Path;
            }
            catch (Exception ex)
            {
                Logger.LogException(ex); 
                return null;
            }
        }

        internal YouTubeMusicItem(string id, string durationSecs, string title, string thumbnailUrl, string description, List<string> actors, string lastUpdated)
        {
            this.id = id;
            this.durationSecs = durationSecs;
            this.title = title;
            this.thumbnailUrl = thumbnailUrl;
            this.description = description;
            this.actors = actors;
            this.lastUpdated = lastUpdated;
            this.requestUrl = String.Format("http://www.youtube.com/get_video_info?&video_id={0}&el=detailpage&ps=default&eurl=&gl=US&hl=en", id);
        }

        public void GetMusicItemUri(Action<Exception, IMusicItem> callback)
        {
            string file = null;
            var taskRead = ReadAppStorage();
            var actionTask = taskRead.AsAsyncAction();
            actionTask.Completed = new Windows.Foundation.AsyncActionCompletedHandler((action, status) =>
            {
                if (status == Windows.Foundation.AsyncStatus.Completed)
                {
                    file = taskRead.Result;
                    if (string.IsNullOrEmpty(file))
                    {
                        HttpWebRequest webRequest = HttpWebRequest.CreateHttp(RequestUrl);
                        webRequest.UserAgent = __USERAGENT__;
                        var result = webRequest.BeginGetResponse(new AsyncCallback(obj =>
                        {
                            try
                            {
                                var webResponse = (HttpWebResponse)webRequest.EndGetResponse(obj);
                                if (webResponse.StatusCode == HttpStatusCode.OK)
                                {
                                    Stream responseStream = webResponse.GetResponseStream();
                                    using (StreamReader reader = new StreamReader(responseStream))
                                    {
                                        string readText = reader.ReadToEnd();
                                        accessUrl = ExtractDownloadUrls(readText);
                                        if (accessUrl != null)
                                        {
                                            isDownloaded = false;
                                            track = new AudioTrack(new Uri(accessUrl, UriKind.Absolute), Title, null, null, new Uri(ThumbnailUrl));
                                            callback(null, this);
                                        }

                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                callback(ex, this);
                            }
                        }), webRequest);
                    }
                    else
                    {
                        accessUrl = file;
                        isDownloaded = true;
                        track = new AudioTrack(new Uri(accessUrl, UriKind.Absolute), Title, null, null, new Uri(ThumbnailUrl));
                        callback(null, this);
                    }
                }
            });
        }

        public AudioTrack Track
        {
            get
            {
                return track;
            }
        }

        public string Id
        {
            get
            {
                return Id;
            }
        }

        public string DurationSecs
        {
            get
            {
                return durationSecs;
            }
        }

        public string Title
        {
            get
            {
                return title;
            }
        }

        public string ThumbnailUrl
        {
            get
            {
                return thumbnailUrl;
            }
        }

        public string Description
        {
            get
            {
                return description;
            }
        }

        public List<string> Actors
        {
            get { return actors; }
        }

        public string LastUpdated
        {
            get { return lastUpdated; }
        }

        public bool IsDownloaded
        {
            get { return isDownloaded; }
        }

        public string AccessUrl
        {
            get { return accessUrl; }
        }

        public string RequestUrl
        {
            get { return requestUrl; }
        }

        public void Download()
        {
            //would ensure result url is set
            GetMusicItemUri((ex, item) =>
            {
                HttpWebRequest webRequest = HttpWebRequest.CreateHttp(AccessUrl);
                webRequest.UserAgent = __USERAGENT__;
                webRequest.BeginGetResponse(new AsyncCallback(obj =>
                {
                    try
                    {
                        DownloadFile(obj, webRequest);
                    }
                    catch (Exception e) { Logger.LogException(e);  }
                }), webRequest);
            });
        }
    }
}

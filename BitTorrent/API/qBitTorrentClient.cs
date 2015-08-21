using API;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
namespace BitTorrent.API
{
    public class qBitTorrentClient : IClient
    {
        #region RequestHandler

        private class qRequestHandler : RequestHandler
        {
            private readonly string username;
            private readonly string password;

            private string SID;
            private string path;

            public qRequestHandler(string server, ushort port, string username, string password)
                : base($"{ensureHttp(server)}:{port}")
            {
                this.username = HttpUtility.UrlEncode(username);
                this.password = HttpUtility.UrlEncode(password);

                this.DefaultContentType = ContentTypes.URL_Encoded;
            }

            private static string ensureHttp(string server)
            {
                var m = Regex.Match(server, "^(?<protocol>[a-z]+)://");
                if (!m.Success)
                    return "http://" + server;
                else if (m.Groups["server"].Value.ToLower() == "http")
                    return server;
                else
                    throw new ArgumentException("Malformed server url.", nameof(server));
            }

            protected override async Task SignIn()
            {
                var request = await CreateRequest("/login", RequestMethods.POST, $"username={username}&password={password}", ContentTypes.URL_Encoded);
                var response = await request.GetResponseAsync() as HttpWebResponse;
                var res = await GetResponse<string>(response);

                if (res != "Ok.")
                    throw new Exception($"qBitTorrent reponded to authentication with: \"{res}\"");

                var m = Regex.Match(response.Headers.Get("Set-Cookie"), "SID=(?<sid>[^;]+); path=(?<path>.*)");

                if (!m.Success)
                    throw new Exception("Unable to parse qBitTorrent Set-Cookie header.");

                SID = m.Groups["sid"].Value;
                path = m.Groups["path"].Value;
            }

            protected override void SetCredentials(HttpWebRequest request)
            {
                request.Headers.Add("Cookie", $"SID={SID}");
            }
        }

        #endregion

        private qRequestHandler req;

        public qBitTorrentClient(string server, ushort port, string username, string password)
        {
            req = new qRequestHandler(server, port, username, password);
        }

        public async Task<bool> AddFromTorrentFile(string filepath, string downloadPath = null)
        {
            string tempPath = null;
            if (downloadPath != null)
            {
                tempPath = await getPath();
                await setPath(downloadPath);
            }

            await uploadTorrentFile(filepath);

            if (downloadPath != null)
                await setPath(tempPath);

            InfoHash hash = InfoHash.FromFile(filepath);

            var files = await ListTorrents();
            for (int i = 0; i < files.Length; i++)
                if (files[i].Hash.Equals(hash))
                    return true;

            return false;
        }
        public async Task<bool> AddFromMagnet(string magneturl, string downloadPath = null)
        {
            string tempPath = null;
            if (downloadPath != null)
            {
                tempPath = await getPath();
                await setPath(downloadPath);
            }

            await req.Post<string>("/command/download", $"urls={HttpUtility.UrlEncode(magneturl)}", ContentTypes.URL_Encoded);

            if (downloadPath != null)
                await setPath(tempPath);

            InfoHash hash = InfoHash.FromMagnetLink(magneturl);

            var files = await ListTorrents();
            for (int i = 0; i < files.Length; i++)
                if (files[i].Hash.Equals(hash))
                    return true;

            return false;
        }

        private async Task uploadTorrentFile(string filepath)
        {
            string boundary = "----------------------------" + DateTime.Now.Ticks.ToString("x2");
            byte[] boundarybytes = Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");

            string name = "torrents";
            string filename = Path.GetFileName(filepath);
            string fileheader = $"Content-Disposition: form-data; name=\"{name}\"; filename=\"{filename}\"\r\n Content-Type: application/x-bittorrent\r\n\r\n";

            HttpWebRequest request = req.CreateRequest("/command/upload", RequestMethods.POST).Result;
            request.ContentType = "multipart/form-data; boundary=" + boundary;
            request.KeepAlive = true;

            using (var reqStream = await request.GetRequestStreamAsync())
            {
                reqStream.Write(boundarybytes, 0, boundarybytes.Length);
                reqStream.Write(Encoding.UTF8.GetBytes(fileheader), 0, Encoding.UTF8.GetByteCount(fileheader));

                using (FileStream fileStream = new FileStream(filepath, FileMode.Open, FileAccess.Read))
                    fileStream.CopyTo(reqStream);

                reqStream.Write(boundarybytes, 0, boundarybytes.Length);
            }
            (await request.GetResponseAsync()).Dispose();
        }

        private async Task<string> getPath()
        {
            return (await req.Get<JObject>("/query/preferences")).Value<string>("save_path");
        }
        private async Task setPath(string path)
        {
            var settings = new JObject(new JProperty("save_path", path));
            var s = "json=" + settings.ToString(Newtonsoft.Json.Formatting.None);

            await req.Post<string>("/command/setPreferences", s, ContentTypes.URL_Encoded);
        }

        public async Task<bool> RemoveTorrent(InfoHash hash, bool removeData)
        {
            string url = removeData ? "/command/deletePerm" : "/command/delete";

            var data = await req.Post<string>(url, "hashes=" + hash.Hash, ContentTypes.URL_Encoded);

            var files = await ListTorrents();
            for (int i = 0; i < files.Length; i++)
                if (files[i].Hash.Equals(hash))
                    return false;

            return true;
        }
        public async Task<TorrentInfo[]> ListTorrents()
        {
            JArray torrentInfoJsonArray = await req.Request<JArray>("/query/torrents/", RequestMethods.GET);


            int torrents = torrentInfoJsonArray.Count;
            TorrentInfo[] torrentInfoArray = new TorrentInfo[torrents];

            for (int i = 0; i < torrents; i++)
            {
                var torrent = torrentInfoJsonArray[i];

                ulong size = torrent.Value<ulong>("size");
                double ratio = torrent.Value<double>("ratio");
                double progress = torrent.Value<double>("progress");

                ulong downloaded = progress == 0 ? 0 : (progress == 1.0 ? size : (ulong)(size * progress));
                ulong remaining = size - downloaded;

                ulong uploaded = (ulong)(downloaded * ratio);

                torrentInfoArray[i] = new TorrentInfo(new InfoHash(torrent.Value<string>("hash")),
                  torrent.Value<string>("name"),
                  torrent.Value<int>("priority"),
                  getActiveState(torrent.Value<string>("state")),
                  getDownloadstate(torrent.Value<string>("state")),
                  new string[] { torrent.Value<string>("label") },
                  size,
                  remaining,
                  uploaded);
            }

            return torrentInfoArray;
        }

        private static ActiveStates getActiveState(string QBstate)
        {
            switch (QBstate)
            {
                case "error": return ActiveStates.Stopped;
                case "pausedUP": return ActiveStates.Stopped;
                case "pausedDL": return ActiveStates.Stopped;
                case "queuedUP": return ActiveStates.Started;
                case "queuedDL": return ActiveStates.Started;
                case "uploading": return ActiveStates.Started;
                case "stalledUP": return ActiveStates.Started;
                case "checkingUP": return ActiveStates.Started;
                case "checkingDL": return ActiveStates.Started;
                case "downloading": return ActiveStates.Started;
                case "stalledDL": return ActiveStates.Started;

                default:
                    throw new KeyNotFoundException($@"The qBitTorrent state ""{QBstate}"" was not recognized.");
            }
        }
        private static DownloadStates getDownloadstate(string QBstate)
        {
            switch (QBstate)
            {
                case "error": return DownloadStates.Error;
                case "pausedUP": return DownloadStates.Seeding;
                case "pausedDL": return DownloadStates.Downloading;
                case "queuedUP": return DownloadStates.Queued | DownloadStates.Seeding;
                case "queuedDL": return DownloadStates.Queued | DownloadStates.Downloading;
                case "uploading": return DownloadStates.Seeding;
                case "stalledUP": return DownloadStates.Seeding;
                case "checkingUP": return DownloadStates.Checking | DownloadStates.Seeding;
                case "checkingDL": return DownloadStates.Checking | DownloadStates.Downloading;
                case "downloading": return DownloadStates.Downloading;
                case "stalledDL": return DownloadStates.Seeding;

                default:
                    throw new KeyNotFoundException($@"The qBitTorrent state ""{QBstate}"" was not recognized.");
            }
        }

        public async Task<bool> SetLabels(IEnumerable<InfoHash> torrents, string[] labels)
        {
            throw new NotImplementedException();
        }
        public async Task<bool> SetLabelsAll(string[] labels)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> SetPriority(IEnumerable<InfoHash> torrents, int priority)
        {
            throw new NotImplementedException();
        }
        public async Task<bool> SetPriorityAll(int priority)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> SetState(IEnumerable<InfoHash> torrents, ActiveStates state)
        {
            throw new NotImplementedException();
        }
        public async Task<bool> SetStateAll(ActiveStates state)
        {
            throw new NotImplementedException();
        }
    }
}

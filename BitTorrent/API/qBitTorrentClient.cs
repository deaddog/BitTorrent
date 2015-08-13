using API;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace BitTorrent.API
{
    public class qBitTorrentClient : IClient
    {
        #region

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

            protected override void SignIn()
            {
                Dictionary<string, string[]> headers = null;

                var res = RequestString("/login", RequestMethods.POST, ContentTypes.URL_Encoded, $"username={username}&password={password}", out headers);

                if (res != "Ok.")
                    throw new Exception($"qBitTorrent reponded to authentication with: \"{res}\"");

                var m = Regex.Match(headers["Set-Cookie"][0], "SID=(?<sid>[^;]+); path=(?<path>.*)");

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
            throw new NotImplementedException();
        }
        public async Task<bool> AddFromURL(string url, string downloadPath = null)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> RemoveTorrent(InfoHash hash)
        {
            throw new NotImplementedException();
        }
        public async Task<TorrentInfo[]> ListTorrents()
        {
            throw new NotImplementedException();
        }

        public async Task<bool> Move(InfoHash hash, string newpath)
        {
            throw new NotImplementedException();
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

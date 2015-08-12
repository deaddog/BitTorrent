using API;
using System;
using System.Collections.Generic;
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

            public qRequestHandler(string server, int port, string username, string password)
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
        }

        #endregion

        public Task<bool> AddFromTorrentFile(string filepath, string downloadPath = null)
        {
            throw new NotImplementedException();
        }
        public Task<bool> AddFromURL(string url, string downloadPath = null)
        {
            throw new NotImplementedException();
        }

        public Task<bool> RemoveTorrent(InfoHash hash)
        {
            throw new NotImplementedException();
        }
        public Task<TorrentInfo[]> ListTorrents()
        {
            throw new NotImplementedException();
        }

        public Task<bool> Move(InfoHash hash, string newpath)
        {
            throw new NotImplementedException();
        }

        public Task<bool> SetLabels(IEnumerable<InfoHash> torrents, string[] labels)
        {
            throw new NotImplementedException();
        }
        public Task<bool> SetLabelsAll(string[] labels)
        {
            throw new NotImplementedException();
        }

        public Task<bool> SetPriority(IEnumerable<InfoHash> torrents, int priority)
        {
            throw new NotImplementedException();
        }
        public Task<bool> SetPriorityAll(int priority)
        {
            throw new NotImplementedException();
        }

        public Task<bool> SetState(IEnumerable<InfoHash> torrents, ActiveStates state)
        {
            throw new NotImplementedException();
        }
        public Task<bool> SetStateAll(ActiveStates state)
        {
            throw new NotImplementedException();
        }
    }
}

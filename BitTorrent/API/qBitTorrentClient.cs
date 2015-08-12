using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BitTorrent.API
{
    public class qBitTorrentClient : IClient
    {
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

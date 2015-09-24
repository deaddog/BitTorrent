using System.Collections.Generic;
using System.Threading.Tasks;

namespace BitTorrent.API
{
    public interface IClient
    {
        Task<bool> AddFromMagnet(string magneturl, string downloadPath = null);
        Task<bool> AddFromTorrentFile(string filepath, string downloadPath = null);

        Task<bool> RemoveTorrent(InfoHash hash, bool removeData);
        Task<TorrentInfo[]> ListTorrents();

        Task<bool> SetState(IEnumerable<InfoHash> torrents, ActiveStates state);
        Task<bool> SetStateAll(ActiveStates state);

        Task<bool> SetLabels(IEnumerable<InfoHash> torrents, string[] labels);
        Task<bool> SetLabelsAll(string[] labels);

        Task<bool> SetPriority(IEnumerable<InfoHash> torrents, Priorities priority);
    }
}

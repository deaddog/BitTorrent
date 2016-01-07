using System.Collections.Generic;
using System.Threading.Tasks;

namespace BitTorrent.API
{
    public interface IClient
    {
        Task AddFromMagnet(string magneturl, string downloadPath = null);
        Task AddFromTorrentFile(string filepath, string downloadPath = null);

        Task RemoveTorrent(InfoHash hash, bool removeData);
        Task<TorrentInfo[]> ListTorrents();

        Task SetState(IEnumerable<InfoHash> torrents, ActiveStates state);
        Task SetStateAll(ActiveStates state);

        Task SetLabels(IEnumerable<InfoHash> torrents, string[] labels);
        Task SetLabelsAll(string[] labels);

        Task SetPriority(IEnumerable<InfoHash> torrents, Priorities priority);
    }
}

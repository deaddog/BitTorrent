using BitTorrent.API;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace BitTorrent
{
    public class Torrent
    {
        private TorrentManager manager;

        private readonly InfoHash hash;
        private TorrentInfo info;

        internal Torrent(TorrentManager manager, TorrentInfo info)
        {
            if (manager == null)
                throw new ArgumentNullException(nameof(manager));

            if (info == null)
                throw new ArgumentNullException(nameof(info));

            this.manager = manager;
            this.info = info;

            this.hash = info.Hash.Clone();
        }

        public InfoHash Hash => hash;
        public string Name => info.Name;
        public int Priority => info.Priority;
        public ActiveStates ActiveState => info.ActiveState;
        public DownloadStates DownloadState => info.DownloadState;
        public ReadOnlyCollection<string> Labels => info.Labels;
        public ulong Size => info.Size;
        public ulong Remaining => info.Remaining;
        public ulong Uploaded => info.Uploaded;
    }
}

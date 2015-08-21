using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace BitTorrent
{
    public class Torrent
    {
        private TorrentManager manager;

        public InfoHash Hash { get; }
        public string Name { get; }
        public int Priority { get; }
        public ActiveStates ActiveState { get; }
        public DownloadStates DownloadState { get; }
        public ReadOnlyCollection<string> Labels { get; }
        public ulong Size { get; }
        public ulong Remaining { get; }
        public ulong Uploaded { get; }

        internal Torrent(TorrentManager manager, InfoHash hash, string name, int priority, ActiveStates activestate, DownloadStates downloadstate, IEnumerable<string> labels, ulong size, ulong remaining, ulong uploaded)
        {
            if (manager == null)
                throw new ArgumentNullException(nameof(manager));

            this.manager = manager;

            Hash = hash;
            Name = name;
            Priority = priority;
            ActiveState = activestate;
            DownloadState = downloadstate;
            Labels = new ReadOnlyCollection<string>(new List<string>(labels));
            Size = size;
            Remaining = remaining;
            Uploaded = uploaded;
        }
    }
}

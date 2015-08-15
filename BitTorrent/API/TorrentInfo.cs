using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace BitTorrent.API
{
    public class TorrentInfo
    {
        public InfoHash Hash { get; }
        public string Name { get; }
        public int Priority { get; }
        public ActiveStates ActiveState { get; }
        public DownloadStates DownloadState { get; }
        public string Label { get; }
        public ulong Size { get; }
        public ulong Remaining { get; }
        public ulong Uploaded { get; }

        public TorrentInfo(InfoHash hash, string name, int priority, ActiveStates activestate, DownloadStates downloadstate, string label, ulong size, ulong remaining, ulong uploaded)
        {
            Hash = hash;
            Name = name;
            Priority = priority;
            ActiveState = activestate;
            DownloadState = downloadstate;
            Label = label;
            Size = size;
            Remaining = remaining;
            Uploaded = uploaded;
        }
    }
}

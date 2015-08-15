using Newtonsoft.Json.Linq;
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

        public TorrentInfo(JToken torrent) :
            this(
                new InfoHash(torrent["hash"].Value<string>()),
                torrent["name"].Value<string>(),
                torrent["priority"].Value<int>(),
                getActiveState(torrent["state"].Value<string>()),
                getDownloadstate(torrent["state"].Value<string>()),
                torrent["label"].Value<string>(),
                torrent["size"].Value<ulong>(),
                0, //remaining - only exists in generic torrent properties
                0) //uploaded - only exists in generic torrent properties
        {

        }

        private static ActiveStates getActiveState(string QBstate)
        {
            if (QBstate == QBUPLOADING || QBstate == QBSTALLEDDL || QBstate == QBSTALLEDUP)
                return ActiveStates.Started;
            else if (QBstate == QBPAUSEDDL || QBstate == QBPAUSEDUP)
                return ActiveStates.Stopped;
            else if (QBstate == QBCHECKINGDL || QBstate == QBCHECKINGUP)
                return ActiveStates.Checking;
            else if (QBstate == QBERROR)
                return ActiveStates.Error;
            else if (QBstate == QBQUEUEDDL || QBstate == QBQUEUEDUP)
                return ActiveStates.Queued;
            else throw new KeyNotFoundException();

        }

        private static DownloadStates getDownloadstate(string QBstate)
        {
            if (QBstate == QBUPLOADING || QBstate == QBQUEUEDUP || QBstate == QBSTALLEDUP || QBstate == QBCHECKINGUP || QBstate == QBPAUSEDUP)
                return DownloadStates.Seeding;

            else if (QBstate == QBQUEUEDDL || QBstate == QBSTALLEDDL || QBstate == QBPAUSEDDL || QBstate == QBCHECKINGDL)
                return DownloadStates.Downloading;
            else if (QBstate == QBERROR)
                return DownloadStates.Error;
            else throw new KeyNotFoundException();

        }

        private static string QBERROR = "error";
        private static string QBPAUSEDUP = "pausedUP";
        private static string QBPAUSEDDL = "pausedDL";
        private static string QBQUEUEDUP = "queuedUP";
        private static string QBQUEUEDDL = "queuedDL";
        private static string QBUPLOADING = "uploading";
        private static string QBSTALLEDUP = "stalledUP";
        private static string QBCHECKINGUP = "checkingUP";
        private static string QBCHECKINGDL = "checkingDL";
        private static string QBDOWNLOADING = "downloading";
        private static string QBSTALLEDDL = "stalledDL";
    }
}



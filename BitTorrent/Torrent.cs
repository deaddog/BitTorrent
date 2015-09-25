using BitTorrent.API;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace BitTorrent
{
    public class Torrent
    {
        private TorrentManager manager;
        private bool deleted;

        private readonly InfoHash hash;
        private TorrentInfo info;

        internal Torrent(TorrentManager manager, TorrentInfo info)
        {
            if (manager == null)
                throw new ArgumentNullException(nameof(manager));

            if (info == null)
                throw new ArgumentNullException(nameof(info));

            this.deleted = false;
            this.manager = manager;
            this.info = info;

            this.hash = info.Hash.Clone();
        }

        /// <summary>
        /// Marks the torrent as deleted and clears its <see cref="TorrentManager"/> reference.
        /// NOTE: This method should only be invoked from the managing <see cref="TorrentManager"/>.
        /// </summary>
        internal void ManagerDelete()
        {
            this.manager = null;
            this.deleted = true;
        }
        /// <summary>
        /// Updates the torrent information describing this <see cref="Torrent"/>.
        /// NOTE: This method should only be invoked from the managing <see cref="TorrentManager"/>.
        /// </summary>
        /// <param name="info">The new torrent information.</param>
        internal void ManagerUpdate(TorrentInfo info)
        {
            if (!info.Hash.Equals(hash))
                throw new ArgumentException($"{nameof(Torrent)} updates can only be performed using the same hash value.", nameof(info));
            if (deleted)
                throw new InvalidOperationException($"A deleted {nameof(Torrent)} cannot be updated.");

            this.info = info;
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

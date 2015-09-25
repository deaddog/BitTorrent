using BitTorrent.API;
using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

namespace BitTorrent
{
    /// <summary>
    /// Represents a torrent that is managed by a <see cref="TorrentManager"/>.
    /// The specifics of a torrent can be updated by calling <see cref="TorrentManager.Update"/>, which will update all the <see cref="Torrent"/>s it manages.
    /// </summary>
    public class Torrent
    {
        private TorrentManager manager;
        private IClient client => manager.Client;
        private bool deleted;

        private readonly InfoHash hash;
        private TorrentInfo info;
        private LabelCollection labels;

        internal Torrent(TorrentManager manager, TorrentInfo info)
        {
            if (manager == null)
                throw new ArgumentNullException(nameof(manager));

            if (info == null)
                throw new ArgumentNullException(nameof(info));

            this.deleted = false;
            this.manager = manager;
            this.info = info;
            this.labels = new LabelCollection(this);

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

        /// <summary>
        /// Deletes this <see cref="Torrent"/> on the torrent server.
        /// </summary>
        /// <param name="deleteData">if set to <c>true</c> the data associated with this <see cref="Torrent"/> is deleted with the torrent; otherwise, it will remain on disc.</param>
        /// <returns><c>true</c>, if this <see cref="Torrent"/> was deleted succesfully.</returns>
        public bool Delete(bool deleteData)
        {
            if (!deleted)
            {
                client.RemoveTorrent(hash, deleteData).Wait();
                manager.Update();
            }

            return deleted;
        }

        public InfoHash Hash => hash;
        public string Name => info.Name;
        public int Priority => info.Priority;
        public ActiveStates ActiveState => info.ActiveState;
        public DownloadStates DownloadState => info.DownloadState;
        public LabelCollection Labels => labels;
        public ulong Size => info.Size;
        public ulong Remaining => info.Remaining;
        public ulong Uploaded => info.Uploaded;

        public class LabelCollection : IEnumerable<string>
        {
            private Torrent torrent;
            private IClient client => torrent.client;
            private string[] labels => torrent.info.Labels.ToArray();

            internal LabelCollection(Torrent torrent)
            {
                this.torrent = torrent;
            }

            public bool Set(IEnumerable<string> labels)
            {
                return Set(labels.ToArray());
            }
            public bool Set(params string[] labels)
            {
                client.SetLabels(new InfoHash[] { torrent.hash }, labels);
                torrent.manager.Update();

                return ArraysEqual(labels, this.labels);
            }

            private static bool ArraysEqual<T>(T[] a1, T[] a2)
            {
                if (ReferenceEquals(a1, a2))
                    return true;

                if (a1 == null || a2 == null)
                    return false;

                if (a1.Length != a2.Length)
                    return false;

                EqualityComparer<T> comparer = EqualityComparer<T>.Default;
                for (int i = 0; i < a1.Length; i++)
                {
                    if (!comparer.Equals(a1[i], a2[i])) return false;
                }
                return true;
            }

            public IEnumerator<string> GetEnumerator()
            {
                return ((IEnumerable<string>)labels).GetEnumerator();
            }
            IEnumerator IEnumerable.GetEnumerator()
            {
                return ((IEnumerable<string>)labels).GetEnumerator();
            }
        }
    }
}

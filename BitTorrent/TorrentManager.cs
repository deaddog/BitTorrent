using BitTorrent.API;
using System;
using System.Collections.Generic;
using System.IO;
using System.Collections;
using System.Linq;

namespace BitTorrent
{
    public class TorrentManager : IEnumerable<Torrent>
    {
        private IClient client;
        internal IClient Client => client;

        private List<Torrent> torrents;

        public TorrentManager(IClient client)
        {
            if (client == null)
                throw new ArgumentNullException(nameof(client));

            this.client = client;
            this.torrents = new List<Torrent>();
        }

        public Torrent[] GetTorrents()
        {
            Update();
            return torrents.ToArray();
        }

        public void Update()
        {
            var info = client.ListTorrents().Result;

            List<Torrent> notUpdated = new List<Torrent>(torrents);
            foreach (var t in info)
            {
                var old = torrents.Find(x => x.Hash.Equals(t.Hash));
                if (old != null)
                {
                    old.ManagerUpdate(t);
                    notUpdated.Remove(old);
                }
                else
                    torrents.Add(new Torrent(this, t));
            }

            foreach (var t in notUpdated)
            {
                t.ManagerDelete();
                torrents.Remove(t);
            }
        }

        private Torrent addFromFile(string filepath, string downloadpath)
        {
            client.AddFromTorrentFile(filepath, downloadpath).Wait();
            var hash = InfoHash.FromFile(filepath);

            Update();

            for (int i = 0; i < torrents.Count; i++)
                if (torrents[i].Hash.Equals(hash))
                    return torrents[i];

            return null;
        }
        public Torrent Add(TorrentLink link, string downloadpath = null)
        {
            switch (link.LinkType)
            {
                case LinkType.Magnet:
                    client.AddFromMagnet(link.Path, downloadpath).Wait();
                    var hash = InfoHash.FromMagnetLink(link.Path);

                    Update();

                    for (int i = 0; i < torrents.Count; i++)
                        if (torrents[i].Hash.Equals(hash))
                            return torrents[i];

                    return null;

                case LinkType.LocalFile:
                    return addFromFile(link.Path, downloadpath);

                case LinkType.OnlineFile:
                    var file = Path.GetTempFileName();

                    using (System.Net.WebClient client = new System.Net.WebClient())
                        client.DownloadFile(link.Path, file);

                    var torrent = addFromFile(file, downloadpath);

                    File.Delete(file);

                    return torrent;

                default:
                    throw new InvalidOperationException("Unknown link type: " + link.LinkType);
            }
        }

        public int Count => torrents.Count;

        public Torrent this[int index] => torrents[index];
        public Torrent this[InfoHash hash] => torrents.Find(x => x.Hash.Equals(hash));

        public IEnumerator<Torrent> GetEnumerator()
        {
            Update();
            return new Enumerator(torrents);
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            Update();
            return new Enumerator(torrents);
        }

        private class Enumerator : IEnumerator<Torrent>
        {
            private int index = -1;
            private readonly Torrent[] torrents;

            public Enumerator(IEnumerable<Torrent> torrents)
            {
                this.torrents = torrents.ToArray();
            }

            public Torrent Current => torrents[index];
            object IEnumerator.Current => torrents[index];

            public void Dispose() { }

            public bool MoveNext() => ++index < torrents.Length;
            public void Reset() => index = -1;
        }
    }
}

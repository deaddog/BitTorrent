using BitTorrent.API;
using System;
using System.Collections.Generic;

namespace BitTorrent
{
    public class TorrentManager
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
    }
}

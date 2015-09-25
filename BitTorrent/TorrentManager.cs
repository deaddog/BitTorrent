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
    }
}

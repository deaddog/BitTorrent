using BitTorrent.API;
using System;

namespace BitTorrent
{
    public class TorrentManager
    {
        private IClient client;
        internal IClient Client => client;

        public TorrentManager(IClient client)
        {
            if (client == null)
                throw new ArgumentNullException(nameof(client));

            this.client = client;
        }
    }
}

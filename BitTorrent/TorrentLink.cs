using System;

namespace BitTorrent
{
    public class TorrentLink
    {
        private string path;
        private LinkType linkType;

        private TorrentLink(string path, LinkType linkType)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));

            this.path = path;
            this.linkType = linkType;
        }

        public static TorrentLink FromURL(string url)
        {
            return new TorrentLink(
                url,
                url.StartsWith("magnet:?") ? LinkType.Magnet : LinkType.OnlineFile);
        }
        public static TorrentLink FromLocal(string filepath)
        {
            return new TorrentLink(filepath, LinkType.Magnet);
        }

        internal string Path => path;
        public LinkType LinkType => linkType;
    }
}

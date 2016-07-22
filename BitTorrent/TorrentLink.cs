using System;

namespace BitTorrent
{
    public class TorrentLink
    {
        private readonly string path;
        private readonly LinkType linkType;

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
            return new TorrentLink(filepath, LinkType.LocalFile);
        }

        public string Path => path;
        public LinkType LinkType => linkType;

        private static string typeToString(LinkType type)
        {
            switch (type)
            {
                case LinkType.Magnet:return "magnet";
                case LinkType.OnlineFile:return "online";
                case LinkType.LocalFile:return "local";

                default:
                    throw new ArgumentOutOfRangeException(nameof(type));
            }
        }

        public override string ToString() => path + "@" + typeToString(linkType);
    }
}

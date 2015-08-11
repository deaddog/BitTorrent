using System;
using System.Text.RegularExpressions;

namespace BitTorrent
{
    public class TorrentHash : IEquatable<TorrentHash>
    {
        private static readonly Regex hashRegex = new Regex("^[a-z0-9]{40}$");

        private readonly string hash;
        public string Hash
        {
            get { return hash; }
        }

        public TorrentHash(string hash)
        {
            hash = hash.ToLower();

            if (!hashRegex.IsMatch(hash))
                throw new ArgumentException("The supplied value is not a valid torrent hash.", nameof(hash));

            this.hash = hash;
        }

        public static bool operator ==(TorrentHash a, TorrentHash b)
        {
            return a.Equals(b);
        }
        public static bool operator !=(TorrentHash a, TorrentHash b)
        {
            return !a.Equals(b);
        }

        public override bool Equals(object obj)
        {
            if (obj is TorrentHash)
                return Equals(obj as TorrentHash);
            else
                return false;
        }
        public bool Equals(TorrentHash other)
        {
            return hash.Equals(other.hash);
        }

        public override int GetHashCode()
        {
            return hash.GetHashCode();
        }
        public override string ToString()
        {
            return hash;
        }
    }
}

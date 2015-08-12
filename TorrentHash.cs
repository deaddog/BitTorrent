using BitTorrent.Bencoding;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace BitTorrent
{
    public class TorrentHash : IEquatable<TorrentHash>
    {
        static Dictionary<char, byte> base32DecodeTable;

        static TorrentHash()
        {
            base32DecodeTable = new Dictionary<char, byte>();
            string table = "abcdefghijklmnopqrstuvwxyz234567";
            for (int i = 0; i < table.Length; i++)
                base32DecodeTable[table[i]] = (byte)i;
        }

        private static readonly Regex hashRegex = new Regex("^[a-z0-9]{40}$");

        private readonly string hash;
        public string Hash
        {
            get { return hash; }
        }

        public TorrentHash(byte[] hash)
        {
            if (hash.Length != 20)
                throw new ArgumentException("Torrent hash must be exactly 20 bytes.", nameof(hash));

            StringBuilder sb = new StringBuilder();

            foreach (var s in hash)
                sb.Append(s.ToString("x"));

            this.hash = sb.ToString();
        }
        public TorrentHash(string hash)
        {
            hash = hash.ToLower();

            if (!hashRegex.IsMatch(hash))
                throw new ArgumentException("The supplied value is not a valid torrent hash.", nameof(hash));

            this.hash = hash;
        }

        public static TorrentHash FromFile(string filepath)
        {
            var info = (BenObject.FromFile(filepath) as BenDictionary)?["info"];

            if (info == null) return null;

            using (MemoryStream ms = new MemoryStream())
            {
                info.Encode(ms);
                ms.Seek(0, SeekOrigin.Begin);
                return new TorrentHash(SHA1(ms));
            }

        }
        public static TorrentHash FromMagnetLink(string magnetLink)
        {
            if (!magnetLink.StartsWith("magnet:?"))
                throw new ArgumentException("Invalid magnet link format", nameof(magnetLink));
            magnetLink = magnetLink.Substring("magnet:?".Length);
            int hashStart = magnetLink.IndexOf("xt=urn:btih:");
            if (hashStart == -1)
                throw new ArgumentException("Magnet link does not contain an infohash", nameof(magnetLink));
            hashStart += "xt=urn:btih:".Length;

            int hashEnd = magnetLink.IndexOf('&', hashStart);
            if (hashEnd == -1)
                hashEnd = magnetLink.Length;

            switch (hashEnd - hashStart)
            {
                case 32:
                    return FromBase32(magnetLink.Substring(hashStart, 32));
                case 40:
                    return new TorrentHash(magnetLink.Substring(hashStart, 40));
                default:
                    throw new ArgumentException("Infohash must be base32 or hex encoded.");
            }
        }

        public static TorrentHash FromBase32(string infoHash)
        {
            if (infoHash.Length != 32)
                throw new ArgumentException("Infohash must be a base32 encoded 32 character string");

            infoHash = infoHash.ToLower();
            int infohashOffset = 0;
            byte[] hash = new byte[20];
            var temp = new byte[8];
            for (int i = 0; i < hash.Length;)
            {
                for (int j = 0; j < 8; j++)
                    if (!base32DecodeTable.TryGetValue(infoHash[infohashOffset++], out temp[j]))
                        throw new ArgumentException("infoHash", "Value is not a valid base32 encoded string");

                //8 * 5bits = 40 bits = 5 bytes
                hash[i++] = (byte)((temp[0] << 3) | (temp[1] >> 2));
                hash[i++] = (byte)((temp[1] << 6) | (temp[2] << 1) | (temp[3] >> 4));
                hash[i++] = (byte)((temp[3] << 4) | (temp[4] >> 1));
                hash[i++] = (byte)((temp[4] << 7) | (temp[5] << 2) | (temp[6] >> 3));
                hash[i++] = (byte)((temp[6] << 5) | temp[7]);
            }

            return new TorrentHash(hash);
        }

        private static byte[] SHA1(Stream stream)
        {
            using (var sha1 = System.Security.Cryptography.SHA1.Create())
                return sha1.ComputeHash(stream);
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

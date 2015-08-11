using System;
using System.IO;
using System.Text;

namespace BitTorrent.Bencoding
{
    public class BenString : BenObject, IEquatable<BenString>
    {
        private byte[] value;
        private string str;

        public BenString()
        {
            value = null;
        }

        public byte[] Bytes => value;
        public string String => str;

        internal override void Decode(PeekStream ps)
        {
            int len = 0;
            while (!ps.PeekIs(':'))
                len = len * 10 + (ps.ReadByte().Value - '0');

            value = new byte[len];
            ps.ReadBytes(value, 0, len);

            ps.ReadByte();
            str = Encoding.UTF8.GetString(value);
        }
        public override void Encode(Stream stream)
        {
            var len = Encoding.ASCII.GetBytes(value.Length.ToString());

            stream.Write(len, 0, len.Length);
            stream.WriteByte((byte)':');
            stream.Write(value, 0, value.Length);
        }

        public override bool Equals(BenObject other)
        {
            if (other is BenString)
                return Equals(other as BenString);
            else
                return false;
        }
        public bool Equals(BenString other)
        {
            return value.Equals(other.value);
        }

        public override string ToString() => $"{nameof(BenString)}: {{{value.Length}:{str}}}";
    }
}

using System;
using System.IO;
using System.Text;

namespace BitTorrent.Bencoding
{
    public class BenString : BenObject, IEquatable<BenString>
    {
        private string value;

        public BenString()
        {
            value = null;
        }

        public string Value => value;

        internal override void Decode(PeekStream ps)
        {
            int len = 0;
            while (!ps.PeekIs(':'))
                len = len * 10 + (ps.ReadByte().Value - '0');

            byte[] buffer = new byte[len];
            ps.ReadBytes(buffer, 0, len);

            ps.ReadByte();
            value = Encoding.UTF8.GetString(buffer);
        }
        public override void Encode(Stream stream)
        {
            var buffer = Encoding.UTF8.GetBytes(value);
            var len = Encoding.ASCII.GetBytes(buffer.Length.ToString());

            stream.Write(len, 0, len.Length);
            stream.WriteByte((byte)':');
            stream.Write(buffer, 0, buffer.Length);
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
    }
}

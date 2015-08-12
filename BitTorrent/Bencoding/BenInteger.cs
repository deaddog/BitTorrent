using System;
using System.IO;
using System.Text;

namespace BitTorrent.Bencoding
{
    public class BenInteger : BenObject, IEquatable<BenInteger>
    {
        private long value;

        public BenInteger()
        {
            value = 0;
        }

        public long Value => value;

        internal override void Decode(PeekStream ps)
        {
            ps.ReadByte();

            value = 0;
            while (!ps.PeekIs('e'))
                value = value * 10 + (ps.ReadByte().Value - '0');

            ps.ReadByte();
        }
        public override void Encode(Stream stream)
        {
            stream.WriteByte((byte)'i');

            var buffer = Encoding.ASCII.GetBytes(value.ToString());
            stream.Write(buffer, 0, buffer.Length);

            stream.WriteByte((byte)'e');
        }

        public override bool Equals(BenObject other)
        {
            if (other is BenInteger)
                return Equals(other as BenInteger);
            else
                return false;
        }
        public bool Equals(BenInteger other)
        {
            return value.Equals(other.value);
        }

        public override string ToString() => $"{nameof(BenInteger)}: {{{value}}}";
    }
}

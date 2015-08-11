using System;

namespace BitTorrent.Bencoding
{
    public class BenInteger : BenObject
    {
        private long value;

        public BenInteger()
        {
            value = 0;
        }

        internal override void Decode(PeekStream ps)
        {
            ps.ReadByte();

            value = 0;
            while (!ps.PeekIs('e'))
                value = value * 10 + (ps.ReadByte().Value - '0');

            ps.ReadByte();
        }
    }
}

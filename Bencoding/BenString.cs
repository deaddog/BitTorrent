using System;
using System.Text;

namespace BitTorrent.Bencoding
{
    public class BenString : BenObject
    {
        private string value;

        public BenString()
        {
            value = null;
        }

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
    }
}

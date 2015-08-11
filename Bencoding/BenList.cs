using System.Collections.Generic;
using System.IO;

namespace BitTorrent.Bencoding
{
    public class BenList : BenObject
    {
        private List<BenObject> list;

        public BenList()
        {
            this.list = new List<BenObject>();
        }

        internal override void Decode(PeekStream ps)
        {
            ps.ReadByte();

            while (!ps.PeekIs('e'))
                list.Add(getObject(ps));

            ps.ReadByte();
        }
        public override void Encode(Stream stream)
        {
            stream.WriteByte((byte)'l');
            foreach (var k in list)
                k.Encode(stream);
            stream.WriteByte((byte)'e');
        }
    }
}

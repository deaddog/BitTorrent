using System;
using System.Collections.Generic;

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
    }
}

using System;
using System.Collections.Generic;

namespace BitTorrent.Bencoding
{
    public class BenDictionary : BenObject
    {
        private List<Tuple<BenString, BenObject>> dict;

        public BenDictionary()
        {
            this.dict = new List<Tuple<BenString, BenObject>>();
        }

        internal override void Decode(PeekStream ps)
        {
            ps.ReadByte();

            while (!ps.PeekIs('e'))
                dict.Add(Tuple.Create(getObject(ps) as BenString, getObject(ps)));

            ps.ReadByte();
        }
    }
}

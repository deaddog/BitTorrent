using System;
using System.Collections.Generic;
using System.IO;

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
        public override void Encode(Stream stream)
        {
            stream.WriteByte((byte)'d');
            foreach (var k in dict)
            {
                k.Item1.Encode(stream);
                k.Item2.Encode(stream);
            }
            stream.WriteByte((byte)'e');
        }
    }
}

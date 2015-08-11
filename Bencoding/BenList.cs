using System;
using System.Collections.Generic;
using System.IO;

namespace BitTorrent.Bencoding
{
    public class BenList : BenObject, IEquatable<BenList>
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

        public override bool Equals(BenObject other)
        {
            if (other is BenList)
                return Equals(other as BenList);
            else
                return false;
        }
        public bool Equals(BenList other)
        {
            if (list.Count != other.list.Count)
                return false;

            for (int i = 0; i < list.Count; i++)
                if (!list[i].Equals(other.list[i]))
                    return false;

            return true;
        }
    }
}

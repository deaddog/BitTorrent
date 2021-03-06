﻿using System;
using System.Collections.Generic;
using System.IO;

namespace BitTorrent.Bencoding
{
    public class BenDictionary : BenObject, IEquatable<BenDictionary>
    {
        // The dictionary uses a List internally to ensure that the add-order is preserved.
        // This is required to generate InfoHash for most clients.
        private List<Tuple<BenString, BenObject>> dict;

        public BenDictionary()
        {
            this.dict = new List<Tuple<BenString, BenObject>>();
        }

        public BenObject this[string key]
        {
            get
            {
                for (int i = 0; i < dict.Count; i++)
                    if (dict[i].Item1.String == key)
                        return dict[i].Item2;

                throw new KeyNotFoundException($"The key \"{key}\" was not found in the {nameof(BenDictionary)}.");
            }
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

        public override bool Equals(BenObject other)
        {
            if (other is BenDictionary)
                return Equals(other as BenDictionary);
            else
                return false;
        }
        public bool Equals(BenDictionary other)
        {
            if (dict.Count != other.dict.Count)
                return false;

            for (int i = 0; i < dict.Count; i++)
                if (!dict[i].Item1.Equals(other.dict[i].Item1) || !dict[i].Item2.Equals(other.dict[i].Item2))
                    return false;

            return true;
        }

        public override string ToString() => $"{nameof(BenDictionary)}: {{Count: {dict.Count}}}";
    }
}

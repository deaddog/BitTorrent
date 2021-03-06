﻿using System;
using System.IO;

namespace BitTorrent.Bencoding
{
    public abstract class BenObject : IEquatable<BenObject>
    {
        public static BenObject FromFile(string filepath)
        {
            using (FileStream fs = new FileStream(filepath, FileMode.Open))
                return FromStream(fs);
        }
        public static BenObject FromStream(Stream stream)
        {
            return getObject(new PeekStream(stream));
        }

        internal static BenObject getObject(PeekStream ps)
        {
            BenObject obj = null;

            var peek = ps.Peek();
            if (!peek.HasValue)
                return null;
            else
                switch ((char)peek.Value)
                {
                    case 'd': obj = new BenDictionary(); break;
                    case 'l': obj = new BenList(); break;
                    case 'i': obj = new BenInteger(); break;

                    case '0':
                    case '1':
                    case '2':
                    case '3':
                    case '4':
                    case '5':
                    case '6':
                    case '7':
                    case '8':
                    case '9':
                        obj = new BenString(); break;
                }

            obj.Decode(ps);
            return obj;
        }

        internal abstract void Decode(PeekStream ps);
        public abstract void Encode(Stream stream);

        public override bool Equals(object obj)
        {
            if (obj is BenObject)
                return Equals(obj as BenObject);
            else
                return false;
        }
        public abstract bool Equals(BenObject other);
    }
}

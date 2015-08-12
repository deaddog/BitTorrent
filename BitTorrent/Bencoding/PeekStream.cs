using System.IO;

namespace BitTorrent.Bencoding
{
    internal class PeekStream
    {
        private byte? peek;
        private Stream stream;

        public PeekStream(Stream stream)
        {
            this.stream = stream;
            this.peek = null;
        }

        public byte? ReadByte()
        {
            if (peek.HasValue)
            {
                var temp = peek.Value;
                peek = null;
                return temp;
            }

            int next = stream.ReadByte();
            if (next == -1)
                return null;
            else
                return (byte)next;
        }
        public byte? Peek()
        {
            if (peek.HasValue)
                return peek.Value;
            else
            {
                int next = stream.ReadByte();
                if (next == -1)
                    return null;
                else
                {
                    peek = (byte)next;
                    return peek;
                }
            }
        }

        public bool PeekIs(byte value)
        {
            var p = Peek();
            return p.HasValue && p.Value == value;
        }
        public bool PeekIs(char value)
        {
            var p = Peek();
            return p.HasValue && p.Value == value;
        }

        public int ReadBytes(byte[] buffer, int offset, int count)
        {
            return stream.Read(buffer, offset, count);
        }
    }
}

using CrowdSourcing.DataContracts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CrowdSourcing.Utils
{
    public class MediaStream : Stream
    {
        Stream inner;


        private byte[] bytes { get; set; }


        public override bool CanRead => this.inner.CanRead;

        public override bool CanSeek => this.inner.CanSeek;

        public override bool CanWrite => this.inner.CanWrite;

        public override long Length { get; }

        public override long Position
        {
            get => this.inner.Position;
            set => this.inner.Position = value;
        }

        public MediaStream(IMedia media)
        {
            this.bytes = media.Bytes();
            this.Length = this.bytes.Length;
            this.inner = new MemoryStream(this.bytes);
        }

        public override void Flush()
        {
            this.inner.Flush();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return this.inner.Read(buffer, offset, count);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return this.inner.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            this.inner.SetLength(value);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            this.Write(buffer, offset, count);
        }
    }
}

﻿using System;
using System.IO;
using System.Threading;

namespace InfinityMQ.Channels.Framing.Writers
{
    internal class BufferedOutStream : Stream
    {
        private readonly MemoryStream bufferStream;
        private readonly BufferSize bufferSize;
        private readonly Object syncLock;
        private readonly Timer timer;
        private Stream stream;
        private IAsyncResult asyncResult;

        public BufferedOutStream(Stream stream, BufferSize bufferSize)
        {
            Verify.NotNull(stream, "stream");

            this.stream = stream;
            this.syncLock = new Object();
            this.bufferSize = bufferSize;
            this.bufferStream = new MemoryStream(bufferSize);
            this.timer = new Timer(state => Flush(), null, TimeSpan.Zero, TimeSpan.FromMilliseconds(25)); //TODO: Issue #18 - Option Configuration.
        }

        protected override void Dispose(Boolean disposing)
        {
            base.Dispose(disposing);

            if (!disposing)
                return;

            lock (this.syncLock)
            {
                WaitOnPendingFlush();

                this.timer.Change(Timeout.Infinite, Timeout.Infinite);
                this.bufferStream.Dispose();
                this.timer.Dispose();
            }
        }

        public override void Write(Byte[] buffer, Int32 offset, Int32 count)
        {
            lock (this.syncLock)
            {
                WaitOnPendingFlush();

                bufferStream.Write(buffer, offset, count);

                if (Length >= bufferSize)
                    Flush();
            }
        }

        public override void Flush()
        {
            lock (this.syncLock)
            {
                if (Length == 0)
                    return;

                WaitOnPendingFlush();

                this.asyncResult = this.stream.BeginWrite(bufferStream.GetBuffer(), 0, (Int32)Length, result => WaitOnPendingFlush(), null);

                bufferStream.Position = 0;
                bufferStream.SetLength(0);
            }
        }

        private void WaitOnPendingFlush()
        {
            lock (this.syncLock)
            {
                if (this.asyncResult == null)
                    return;

                this.stream.EndWrite(this.asyncResult);
                this.asyncResult = null;
            }
        }

        public override Boolean CanRead
        {
            get { return false; }
        }

        public override Boolean CanSeek
        {
            get { return false; }
        }

        public override Boolean CanWrite
        {
            get { return true; }
        }

        public override Int64 Length
        {
            get { return this.bufferStream.Length; }
        }

        public override Int64 Position
        {
            get { return this.bufferStream.Position; }
            set { throw new NotSupportedException(); }
        }

        public override Int64 Seek(Int64 offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        public override void SetLength(Int64 value)
        {
            throw new NotSupportedException();
        }

        public override Int32 Read(Byte[] buffer, Int32 offset, Int32 count)
        {
            throw new NotSupportedException();
        }
    }
}

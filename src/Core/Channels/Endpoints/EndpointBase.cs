using System;
using System.Collections.Generic;
using System.IO;
using InfinityMQ.Channels.Framing;
using InfinityMQ.Channels.Framing.Readers;
using InfinityMQ.Channels.Framing.Writers;

namespace InfinityMQ.Channels.Endpoints
{
    internal abstract class EndpointBase : IEndpoint
    {
        private readonly IWriteFrames frameWriter;
        private readonly IReadFrames frameReader;
        private readonly Boolean ownsFraming;

        protected abstract Stream Stream { get; }
        protected abstract Boolean Connected { get; }

        protected EndpointBase(IReadFrames frameReader, IWriteFrames frameWriter, Boolean ownsFraming)
        {
            Verify.NotNull(frameReader, "frameReader");
            Verify.NotNull(frameWriter, "frameWriter");

            this.frameReader = frameReader;
            this.frameWriter = frameWriter;
            this.ownsFraming = ownsFraming;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(Boolean disposing)
        {
            frameReader.EnsureDisposed();
            frameWriter.EnsureDisposed();
        }

        public abstract void Bind(Uri uri);

        public abstract void WaitForConnection();

        public abstract void Connect(Uri uri);
        
        public abstract void Disconnect();
        
        public void Send(params Frame[] frames)
        {
            EnsureConnected();

            this.frameWriter.Write(Stream, frames);
        }

        public IEnumerable<Frame> Receive() //TODO: Consider returning `Message` class that is IEnumerable<Frame> but exposes Type property that finds TypeFrame if exists?
        {
            EnsureConnected();

            IList<Frame> frames = new List<Frame>();
            Frame frame;
            
            do
            {
                frames.Add(frame = this.frameReader.Read(Stream));
            } while ((frame.Flags & FrameFlags.More) == FrameFlags.More);

            return frames;
        }

        protected void EnsureConnected()
        {
            if (!Connected)
                throw new InvalidOperationException(); //TODO: Issue #23 - Throw meaningful execptions.
        }

        protected void EnsureDisconnected()
        {
            if (Connected)
                throw new InvalidOperationException(); //TODO: Issue #23 - Throw meaningful execptions.
        }
    }
}

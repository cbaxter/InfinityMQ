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
        private readonly ICreateFrameReaders frameReaderFactory;
        private readonly ICreateFrameWriters frameWriterFactory;
        private IWriteFrames frameWriter;
        private IReadFrames frameReader;

        protected abstract Stream Stream { get; }
        protected abstract Boolean Connected { get; }

        protected EndpointBase(ICreateFrameReaders frameReaderFactory, ICreateFrameWriters frameWriterFactory)
        {
            Verify.NotNull(frameReaderFactory, "frameReaderFactory");
            Verify.NotNull(frameWriterFactory, "frameWriterFactory");

            this.frameReaderFactory = frameReaderFactory;
            this.frameWriterFactory = frameWriterFactory;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(Boolean disposing)
        {
            frameReader.DisposeIfSet();
            frameWriter.DisposeIfSet();
        }

        public abstract void Bind(Uri uri);

        public abstract void WaitForConnection();

        public abstract void Connect(Uri uri);
        
        public abstract void Disconnect();
        
        public void Send(params Frame[] frames)
        {
            EnsureConnected();

            this.frameWriter.Write(frames);
        }

        public IEnumerable<Frame> Receive() //TODO: Consider returning `Message` class that is IEnumerable<Frame> but exposes Type property that finds TypeFrame if exists?
        {
            EnsureConnected();

            IList<Frame> frames = new List<Frame>();
            Frame frame;
            
            do
            {
                frames.Add(frame = this.frameReader.ReadFrame());
            } while ((frame.Flags & FrameFlags.More) == FrameFlags.More);

            return frames;
        }

        protected void EnsureConnected()
        {
            if (!Connected)
                throw new InvalidOperationException(ExceptionMessages.EndpointDisconnected);
        }

        protected void EnsureDisconnected()
        {
            if (Connected)
                throw new InvalidOperationException(ExceptionMessages.EndpointConnected);
        }

        //HACK:  Issue #19 - Allow for multiple Bind/Connect calls on single channel.
        //       Need to support concept of EndpointConnection for TcpEndpoints; short-term
        //       solution implemented to allow for completion of buffer implementations.
        //       Likely should differentiate between Endpoint Acceptors and Endpoint Connections?
        protected void InitializeFraming(Stream stream)
        {
            frameReader = this.frameReaderFactory.CreateReader(stream);
            frameWriter = this.frameWriterFactory.CreateWriter(stream);
        }
    }
}

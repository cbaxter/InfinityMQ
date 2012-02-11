using System;
using System.IO;
using System.IO.Pipes;
using InfinityMQ.Channels.Framing.Readers;
using InfinityMQ.Channels.Framing.Writers;

namespace InfinityMQ.Channels.Endpoints
{
    internal class NamedPipeEndpoint : EndpointBase
    {
        private PipeStream PipeStream { get; set; }
        protected override Stream Stream { get { return PipeStream; } }
        protected override Boolean Connected { get { return PipeStream != null; } }

        public NamedPipeEndpoint(ICreateFrameReaders frameReaderFactory, ICreateFrameWriters frameWriterFactory)
            : base(frameReaderFactory, frameWriterFactory)
        { }

        protected override void Dispose(Boolean disposing)
        {
            base.Dispose(disposing);

            if (!disposing)
                return;

            if (PipeStream != null)
                Disconnect();
        }

        public override void Bind(Uri uri)
        {
            EnsureDisconnected();

            var serverPipeStream = new NamedPipeServerStream(uri.PathAndQuery, PipeDirection.InOut); //TODO: Issue #20 - Properly parse URI for Endpoint specification.

            PipeStream = serverPipeStream;
        }

        public override void WaitForConnection()
        {
            var serverPipeStream = PipeStream as NamedPipeServerStream;
            if (serverPipeStream == null)
                throw new NotSupportedException(ExceptionMessages.EndpointWaitForConnectionNotSupported);

            serverPipeStream.WaitForConnection();

            InitializeFraming(PipeStream);
        }

        public override void Connect(Uri uri)
        {
            EnsureDisconnected();

            var clientPipeStream = new NamedPipeClientStream(".", uri.PathAndQuery, PipeDirection.InOut); //TODO: Issue #20 - Properly parse URI for Endpoint specification.

            clientPipeStream.Connect();

            PipeStream = clientPipeStream;

            InitializeFraming(PipeStream);
        }

        public override void Disconnect()
        {
            EnsureConnected();

            PipeStream.WaitForPipeDrain();
            PipeStream.Dispose();
            PipeStream = null;
        }
    }
}

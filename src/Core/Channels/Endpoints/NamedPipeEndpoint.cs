using System;
using System.IO;
using System.IO.Pipes;
using InfinityMQ.Channels.Framing;
using InfinityMQ.Channels.Framing.Readers;
using InfinityMQ.Channels.Framing.Writers;

namespace InfinityMQ.Channels.Endpoints
{
    internal class NamedPipeEndpoint : EndpointBase
    {
        private PipeStream PipeStream { get; set; }
        protected override Stream Stream { get { return PipeStream; } }
        protected override Boolean Connected { get { return PipeStream != null; } }

        public NamedPipeEndpoint(IReadFrames frameReader, IWriteFrames frameWriter)
            : base(frameReader, frameWriter)
        { }

        protected override void Dispose(Boolean disposing)
        {
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
            if(serverPipeStream == null)
                throw new InvalidOperationException(); //TODO: Issue #23 - Throw meaningful execptions.

            serverPipeStream.WaitForConnection();
        }

        public override void Connect(Uri uri)
        {
            EnsureDisconnected();

            var clientPipeStream = new NamedPipeClientStream(".", uri.PathAndQuery, PipeDirection.InOut); //TODO: Issue #20 - Properly parse URI for Endpoint specification.

            clientPipeStream.Connect();

            PipeStream = clientPipeStream;
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

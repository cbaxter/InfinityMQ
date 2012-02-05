using System;
using System.IO;
using System.Linq;
using InfinityMQ.Channels.Endpoints;
using InfinityMQ.Channels.Framing;
using InfinityMQ.Serialization;

namespace InfinityMQ.Channels
{ 
    internal abstract class ChannelBase : IDisposable
    {
        private readonly ICreateEndpoints endpointFactory;
        private readonly ISerializeMessages messageSerializer;

        //TODO: Issue #XX. Support binding/connecting to multiple endpoints.
        private IEndpoint messageEndpoint;

        protected ChannelBase(ICreateEndpoints endpointFactory, ISerializeMessages messageSerializer)
        {
            Verify.NotNull(messageSerializer, "messageSerializer");

            this.endpointFactory = endpointFactory;
            this.messageSerializer = messageSerializer;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(Boolean disposing)
        {
            if (!disposing)
                return;

            messageEndpoint.DisposeIfSet();
        }

        public void Bind(Uri uri)
        {
            this.messageEndpoint = endpointFactory.CreateEndpoint(uri.Scheme.ToEnum<EndpointType>());
            this.messageEndpoint.Bind(uri);
        }

        public void WaitForConnection()
        {
            this.messageEndpoint.WaitForConnection();
        }

        public void Connect(Uri uri)
        {
            this.messageEndpoint = endpointFactory.CreateEndpoint(uri.Scheme.ToEnum<EndpointType>());
            this.messageEndpoint.Connect(uri);
        }

        public void Disconnect()
        {
            this.messageEndpoint.Disconnect();
            this.messageEndpoint = null;
        }

        protected void WriteMessage(Object message)
        {
            //TODO: just reuse existing stream?
            using (var memoryStream = new MemoryStream())
            {
                this.messageSerializer.Serialize(message, memoryStream);
                this.messageEndpoint.Send(new Frame(FrameFlags.None, new ArraySegment<Byte>(memoryStream.GetBuffer(), 0, (Int32)memoryStream.Length)));
            }
        }

        protected void WriteMessage(Object message, Type type)
        {
            using (var memoryStream = new MemoryStream())
            {
                this.messageSerializer.Serialize(message, memoryStream);
                this.messageEndpoint.Send(new Frame(new Byte[0]) /* TODO: Explicit Type Header */, new Frame(FrameFlags.None, new ArraySegment<Byte>(memoryStream.GetBuffer(), 0, (Int32)memoryStream.Length)));
            }
        }

        protected void WriteSegment(ArraySegment<Byte> segment)
        {
            this.messageEndpoint.Send(new Frame(FrameFlags.None, segment));
        }

        protected Object ReadMessage()
        {
            var frames = this.messageEndpoint.Receive();

            //TODO: Check for Type Header frame.

            var buffer = frames.Single().Body;
            using (var memoryStream = new MemoryStream(buffer.Array, buffer.Offset, buffer.Count))
                return this.messageSerializer.Deserialize(typeof(Object), memoryStream);
        }

        protected ArraySegment<Byte> ReadSegment()
        {
            var frames = this.messageEndpoint.Receive();

            //TODO: Combine Body Frames.

            return frames.Single().Body;
        }
    }
}

using System;
using InfinityMQ.Channels.Framing.Readers;
using InfinityMQ.Channels.Framing.Writers;

namespace InfinityMQ.Channels.Endpoints
{
    internal interface ICreateEndpoints
    {
        IEndpoint CreateEndpoint(EndpointType type);
    }

    internal class EndpointFactory : ICreateEndpoints
    {
        private readonly ICreateFrameReaders frameReaderFactory;
        private readonly ICreateFrameWriters frameWriterFactory;

        public EndpointFactory(ICreateFrameReaders frameReaderFactory, ICreateFrameWriters frameWriterFactory)
        {
            Verify.NotNull(frameReaderFactory, "frameReaderFactory");
            Verify.NotNull(frameWriterFactory, "frameWriterFactory");

            this.frameReaderFactory = frameReaderFactory;
            this.frameWriterFactory = frameWriterFactory;
        }

        public IEndpoint CreateEndpoint(EndpointType type)
        {
            switch (type)
            {
                case EndpointType.InProc:
                    throw new NotSupportedException();
                case EndpointType.Ipc:
                    return new NamedPipeEndpoint(this.frameReaderFactory.CreateReader(), this.frameWriterFactory.CreateWriter());
                case EndpointType.Tcp:
                    return new TcpEndpoint(this.frameReaderFactory.CreateReader(), this.frameWriterFactory.CreateWriter());
                default:
                    throw new InvalidOperationException(); //TODO: Set messsage;
            }
        }
    }
}

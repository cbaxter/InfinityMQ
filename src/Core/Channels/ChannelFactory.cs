using InfinityMQ.Channels.Endpoints;
using InfinityMQ.Channels.Framing.Readers;
using InfinityMQ.Channels.Framing.Writers;
using InfinityMQ.Serialization;

namespace InfinityMQ.Channels
{
    public interface ICreateChannels
    {
        ISendMessages CreateSender(ISerializeMessages messageSerializer);
        IReceiveMessages CreateReceiver(ISerializeMessages messageSerializer);
        IPublishMessages CreatePublisher(ISerializeMessages messageSerializer);
        ISubscribeToMessages CreateSubscriber(ISerializeMessages messageSerializer);
    }
    
    public class ChannelFactory : ICreateChannels
    {
        public static readonly ICreateChannels Instance = new ChannelFactory();
        
        public ISendMessages CreateSender(ISerializeMessages messageSerializer)
        {
            return new DuplexChannel(new EndpointFactory(DefaultFrameReaderFactory.Instance, DefaultFrameWriterFactory.Instance), messageSerializer);
        }

        public IReceiveMessages CreateReceiver(ISerializeMessages messageSerializer)
        {
            return new DuplexChannel(new EndpointFactory(DefaultFrameReaderFactory.Instance, DefaultFrameWriterFactory.Instance), messageSerializer);
        }

        public IPublishMessages CreatePublisher(ISerializeMessages messageSerializer)
        {
            return new PublisherChannel(new EndpointFactory(NullFrameReaderFactory.Instance, BufferedFrameWriterFactory.Instance), messageSerializer);
        }

        public ISubscribeToMessages CreateSubscriber(ISerializeMessages messageSerializer)
        {
            return new SubscriberChannel(new EndpointFactory(BufferedFrameReaderFactory.Instance, NullFrameWriterFactory.Instance), messageSerializer);
        }
    }

    public static class ChannelFactoryExtensions
    {
        public static ISendMessages CreateSender(this ICreateChannels channelFactory)
        {
            Verify.NotNull(channelFactory, "channelFactory");
            
            return channelFactory.CreateSender(JsonDataContractSerializer.Instance);
        }

        public static IReceiveMessages CreateReceiver(this ICreateChannels channelFactory)
        {
            Verify.NotNull(channelFactory, "channelFactory");
            
            return channelFactory.CreateReceiver(JsonDataContractSerializer.Instance);
        }
        public static IPublishMessages CreatePublisher(this ICreateChannels channelFactory)
        {
            Verify.NotNull(channelFactory, "channelFactory");

            return channelFactory.CreatePublisher(JsonDataContractSerializer.Instance);
        }

        public static ISubscribeToMessages CreateSubscriber(this ICreateChannels channelFactory)
        {
            Verify.NotNull(channelFactory, "channelFactory");

            return channelFactory.CreateSubscriber(JsonDataContractSerializer.Instance);
        }
    }
}

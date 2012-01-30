using System;
using InfinityMQ.Serialization;

namespace InfinityMQ.Messaging
{
    public class DuplexChannel
    {
        private readonly MessageReader messageReader;
        private readonly MessageWriter messageWriter;

        internal DuplexChannel(MessageReader messageReader, MessageWriter messageWriter)
        {
            Verify.NotNull(messageReader, "messageReader");
            Verify.NotNull(messageWriter, "messageWriter");
            
            this.messageReader = messageReader;
            this.messageWriter = messageWriter;
        }
        
        public void Send(Object message)
        {
            this.messageWriter.Write(message);
        }

        public Object Receive()
        {
            return this.messageReader.Read();
        }
    }
}

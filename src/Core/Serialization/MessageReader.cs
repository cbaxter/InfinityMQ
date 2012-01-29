using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using InfinityMQ.Serialization.Serializers;

namespace InfinityMQ.Serialization
{
    internal class MessageReader
    {
        private readonly FrameReader frameReader;
        private readonly ISerializeMessages serializer;

        public MessageReader(FrameReader frameReader)
            : this(frameReader, new JsonMessageSerializer())
        { }

        public MessageReader(FrameReader frameReader, ISerializeMessages serializer)
        {
            Verify.NotNull(frameReader, "frameReader");
            Verify.NotNull(serializer, "serializer");

            this.frameReader = frameReader;
            this.serializer = serializer;
        }

        public Object Read()
        {
            var frames = ReadMessageFrames();
            var type = GetMessageType(frames.First());

            return DeserializeMessage(type, frames.Skip(1));
        }

        private IList<Frame> ReadMessageFrames()
        {
            Frame frame;
            IList<Frame> frames = new List<Frame>();

            do
            {
                frames.Add(frame = this.frameReader.Read());
            } while (frame.Flags.HasFlag(FrameFlags.More));

            return frames;
        }

        private Type GetMessageType(Frame frame)
        {
            var body = frame.Body;

            return Type.GetType(Encoding.UTF8.GetString(body.Array, body.Offset, body.Count));
        }

        private Object DeserializeMessage(Type type, IEnumerable<Frame> frames)
        {
            using (var memoryStream = new MemoryStream())
            {
                foreach (var item in frames)
                    memoryStream.Write(item.Body);

                memoryStream.Position = 0;

                return this.serializer.Deserialize(type, memoryStream);
            }
        }
    }
}

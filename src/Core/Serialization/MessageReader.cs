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
        private readonly IDictionary<String, Type> knownTypes = new Dictionary<String, Type>();

        public MessageReader(FrameReader frameReader)
            : this(frameReader, new JsonDataContractSerializer())
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
            //TODO: Issue #10 -- MessageReader cannot assume frame structure.

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
            } while ((frame.Flags & FrameFlags.More) == FrameFlags.More);

            return frames;
        }

        private Type GetMessageType(Frame frame)
        {
            Type type;
            ArraySegment<Byte> encodedBytes = frame.Body;
            String typeName = Encoding.UTF8.GetString(encodedBytes.Array, encodedBytes.Offset, encodedBytes.Count);

            if (knownTypes.TryGetValue(typeName, out type))
                return type;

            knownTypes[typeName] = type = Type.GetType(typeName);

            return type;
        }

        private Object DeserializeMessage(Type type, IEnumerable<Frame> frames)
        {
            using (var memoryStream = new MemoryStream())
            {
                foreach (var item in frames)
                    memoryStream.Write(item.Body.Array, item.Body.Offset, item.Body.Count);

                memoryStream.Position = 0;

                return this.serializer.Deserialize(type, memoryStream);
            }
        }
    }
}

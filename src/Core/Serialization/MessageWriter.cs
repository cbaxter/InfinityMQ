using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using InfinityMQ.Serialization.Serializers;

namespace InfinityMQ.Serialization
{
    internal class MessageWriter
    {
        private readonly FrameWriter frameWriter;
        private readonly ISerializeMessages serializer;
        private readonly Dictionary<Type, Frame> typeHeaders = new Dictionary<Type, Frame>();

        public MessageWriter(FrameWriter frameWriter)
            : this(frameWriter, new JsonDataContractSerializer())
        { }

        public MessageWriter(FrameWriter frameWriter, ISerializeMessages serializer)
        {
            Verify.NotNull(frameWriter, "frameWriter");
            Verify.NotNull(serializer, "serializer");

            this.frameWriter = frameWriter;
            this.serializer = serializer;
        }

        public void Write(Object message)
        {
            if (message == null)
                return;

            //TODO: Issue #8 -- Support custom message headers.
            //TODO: Issue #9 -- Header frames should be 'Key/Value' pairs.

            this.frameWriter.Write(GetOrCreateTypeFrame(message), GetBodyFrame(message));
        }

        private Frame GetOrCreateTypeFrame(Object message)
        {
            Frame typeFrame;
            Type type = message.GetType();

            if (this.typeHeaders.TryGetValue(type, out typeFrame))
                return typeFrame;

            var assemblyQualifiedName = type.AssemblyQualifiedName ?? String.Empty;
            var versionlessAssemblyQualifiedName = Regex.Replace(assemblyQualifiedName, @", Version=\d+.\d+.\d+.\d+,", ",");
            var encodedBytes = Encoding.UTF8.GetBytes(versionlessAssemblyQualifiedName);
            var rawFrame = new Byte[encodedBytes.Length + sizeof (Byte)];

            rawFrame[0] = (Byte)(FrameFlags.Header | FrameFlags.More);
            Buffer.BlockCopy(encodedBytes, 0, rawFrame, 0, encodedBytes.Length);

            this.typeHeaders[type] = typeFrame = new Frame(rawFrame);

            return typeFrame;
        }

        private Frame GetBodyFrame(Object message)
        {
            using (var memoryStream = new MemoryStream())
            {
                this.serializer.Serialize(message, memoryStream);

                return new Frame(FrameFlags.None, new ArraySegment<Byte>(memoryStream.GetBuffer(), 0, (Int32)memoryStream.Length));
            }
        }
    }
}

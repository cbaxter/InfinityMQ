using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace InfinityMQ.Channels.Framing
{
    //TODO: Will be used by ChannelBase (currently not referenced)
    internal class TypeFrameConverter
    {
        private readonly Dictionary<Type, Frame> typeHeaders = new Dictionary<Type, Frame>();
        private readonly IDictionary<String, Type> knownTypes = new Dictionary<String, Type>();

        public Frame GetTypeFrameFor(Type type)
        {
            Frame typeFrame;

            if (this.typeHeaders.TryGetValue(type, out typeFrame))
                return typeFrame;

            var assemblyQualifiedName = type.AssemblyQualifiedName ?? String.Empty;
            var versionlessAssemblyQualifiedName = Regex.Replace(assemblyQualifiedName, @", Version=\d+.\d+.\d+.\d+,", ",");
            var encodedBytes = Encoding.UTF8.GetBytes(versionlessAssemblyQualifiedName);
            var rawFrame = new Byte[encodedBytes.Length + sizeof(Byte)];

            rawFrame[0] = (Byte)(FrameFlags.Header | FrameFlags.More);
            Buffer.BlockCopy(encodedBytes, 0, rawFrame, 0, encodedBytes.Length);

            this.typeHeaders[type] = typeFrame = new Frame(rawFrame);

            return typeFrame;
        }

        public Type GetTypeFrom(Frame frame)
        {
            Type type;
            ArraySegment<Byte> encodedBytes = frame.Body;
            String typeName = Encoding.UTF8.GetString(encodedBytes.Array, encodedBytes.Offset, encodedBytes.Count);

            if (knownTypes.TryGetValue(typeName, out type))
                return type;

            knownTypes[typeName] = type = Type.GetType(typeName);

            return type;
        }
    }
}

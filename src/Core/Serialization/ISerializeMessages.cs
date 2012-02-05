using System;
using System.IO;

namespace InfinityMQ.Serialization
{
    public interface ISerializeMessages
    {
        void Serialize(Object graph, Stream output);
        Object Deserialize(Type type, Stream input);
    }
}


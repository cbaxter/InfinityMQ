using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Json;

namespace InfinityMQ.Serialization.Serializers
{
    internal class JsonDataContractSerializer : ISerializeMessages
    {
        // Instances of DataContractJsonSerializer are thread safe except when the instance is
        // used with an implementation of the IDataContractSurrogate or DataContractResolver.
        private readonly IDictionary<Type, DataContractJsonSerializer> jsonSerializers = new Dictionary<Type, DataContractJsonSerializer>();

        public void Serialize(Object graph, Stream output)
        {
            Verify.NotNull(output, "output");
            Verify.NotNull(output, "output");
            
            var jsonSerializer = GetJsonSerializerFor(graph.GetType());

            jsonSerializer.WriteObject(output, graph);
        }

        public Object Deserialize(Type type, Stream input)
        {
            Verify.NotNull(type, "type");
            Verify.NotNull(input, "input");

            var jsonSerializer = GetJsonSerializerFor(type);

            return jsonSerializer.ReadObject(input);
        }

        private DataContractJsonSerializer GetJsonSerializerFor(Type messageType)
        {
            DataContractJsonSerializer jsonSerializer;

            if (!this.jsonSerializers.TryGetValue(messageType, out jsonSerializer))
                this.jsonSerializers[messageType] = jsonSerializer = new DataContractJsonSerializer(messageType);

            return jsonSerializer;
        }
    }
}

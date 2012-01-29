using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Json;
using InfinityMQ.Serialization;

namespace InfinityMQ.Performance
{
    // Cost of Serialization/Deserialization should not count in benchmark tests. 
    internal class CachedMessageSerializer : ISerializeMessages
    {
        private readonly IDictionary<Type, Object> deserializedJson = new Dictionary<Type, Object>();
        private readonly IDictionary<Type, Byte[]> serializedJson = new Dictionary<Type, Byte[]>();

        public void PrimeCache(Object message)
        {
            Verify.NotNull(message, "message");
            
            var type = message.GetType();
            var serializer = new DataContractJsonSerializer(type);

            deserializedJson.Add(type, message);

            using(var memoryStream = new MemoryStream())
            {
                serializer.WriteObject(memoryStream, message);
                serializedJson.Add(type, memoryStream.ToArray());
            }
        }
        
        public void Serialize(Object graph, Stream output)
        {
            Verify.NotNull(graph, "graph");
            Verify.NotNull(output, "output");

            output.Write(this.serializedJson[graph.GetType()]);
            output.Flush();
        }

        public Object Deserialize(Type type, Stream input)
        {
            Verify.NotNull(type, "type");
            Verify.NotNull(input, "input");

            return this.deserializedJson[type];
        }
    }
}

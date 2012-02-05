using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;

namespace InfinityMQ.Serialization
{
    public class XmlDataContractSerializer : ISerializeMessages
    {
        // Instances of DataContractJsonSerializer are thread safe except when the instance is
        // used with an implementation of the IDataContractSurrogate or DataContractResolver.
        private readonly IDictionary<Type, DataContractSerializer> xmlSerializers = new Dictionary<Type, DataContractSerializer>();
        public static readonly ISerializeMessages Instance = new XmlDataContractSerializer();

        public void Serialize(Object graph, Stream output)
        {
            Verify.NotNull(output, "output");
            Verify.NotNull(output, "output");
            
            var jsonSerializer = GetXmlSerializerFor(graph.GetType());

            jsonSerializer.WriteObject(output, graph);
        }

        public Object Deserialize(Type type, Stream input)
        {
            Verify.NotNull(type, "type");
            Verify.NotNull(input, "input");

            var jsonSerializer = GetXmlSerializerFor(type);

            return jsonSerializer.ReadObject(input);
        }

        private DataContractSerializer GetXmlSerializerFor(Type messageType)
        {
            DataContractSerializer jsonSerializer;

            if (!this.xmlSerializers.TryGetValue(messageType, out jsonSerializer))
                this.xmlSerializers[messageType] = jsonSerializer = new DataContractSerializer(messageType);

            return jsonSerializer;
        }
    }
}

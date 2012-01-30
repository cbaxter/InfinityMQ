using System;
using System.IO;

namespace InfinityMQ.Serialization.Serializers
{
    internal interface ISerializeMessages
    {
        void Serialize(Object graph, Stream output);
        Object Deserialize(Type type, Stream input);
    }

    //internal static class ExtendedISerializeMessages
    //{
    //    public static ArraySegment<Byte> Serialize(this ISerializeMessages serializer, Object graph)
    //    {
    //        using (var memoryStream = new MemoryStream())
    //        {
    //            serializer.Serialize(graph, memoryStream);

    //            return new ArraySegment<Byte>(memoryStream.GetBuffer(), 0, (Int32)memoryStream.Length);
    //        }
    //    }

    //    //public static T Deserialize<T>(this ISerialize serializer, Byte[] buffer)
    //    //{
    //    //    return (T)serializer.Deserialize(typeof(T), buffer);
    //    //}

    //    //public static T Deserialize<T>(this ISerialize serializer, Byte[] buffer, Int32 offset, Int32 count)
    //    //{
    //    //    return (T)serializer.Deserialize(typeof(T), buffer, offset, count);
    //    //}

    //    //public static Object Deserialize(this ISerialize serializer, Type type, Byte[] buffer)
    //    //{
    //    //    Verify.NotNull(buffer, "buffer");

    //    //    return serializer.Deserialize(type, buffer, 0, buffer.Length);
    //    //}

    //    //public static Object Deserialize(this ISerialize serializer, Type type, Byte[] buffer, Int32 offset, Int32 count)
    //    //{
    //    //    using (var memoryStream = new MemoryStream(buffer, offset, count))
    //    //        return serializer.Deserialize(type, memoryStream);
    //    //}
    //}
}


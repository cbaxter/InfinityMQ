using System;
using System.IO;

namespace InfinityMQ.Serialization
{
    public interface ISerializeMessages
    {
        void Serialize(Object graph, Stream output);
        Object Deserialize(Type type, Stream input);
    }

    //TODO: REMOVE...
    //public static class ExtendedISerializeMessages
    //{
    //    public static Byte[] Serialize(this ISerialize serializer, Object graph)
    //    {
    //        using (var memoryStream = new MemoryStream())
    //        {
    //            serializer.Serialize(graph, memoryStream);
    //            return memoryStream.ToArray();
    //        }
    //    }

    //    public static T Deserialize<T>(this ISerialize serializer, Byte[] buffer)
    //    {
    //        return (T)serializer.Deserialize(typeof(T), buffer);
    //    }

    //    public static T Deserialize<T>(this ISerialize serializer, Byte[] buffer, Int32 offset, Int32 count)
    //    {
    //        return (T)serializer.Deserialize(typeof(T), buffer, offset, count);
    //    }

    //    public static Object Deserialize(this ISerialize serializer, Type type, Byte[] buffer)
    //    {
    //        Verify.NotNull(buffer, "buffer");

    //        return serializer.Deserialize(type, buffer, 0, buffer.Length);
    //    }

    //    public static Object Deserialize(this ISerialize serializer, Type type, Byte[] buffer, Int32 offset, Int32 count)
    //    {
    //        using (var memoryStream = new MemoryStream(buffer, offset, count))
    //            return serializer.Deserialize(type, memoryStream);
    //    }
    //}
}


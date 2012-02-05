using System;

namespace InfinityMQ
{
    internal static class StringExtensions
    {
        public static T ToEnum<T>(this String value)
            where T : struct
        {
            Verify.True(typeof(T).IsEnum, "value", "The generic type parameter T must be an Enum type.");

            return (T)Enum.Parse(typeof(T), value, true);
        }
    }
}

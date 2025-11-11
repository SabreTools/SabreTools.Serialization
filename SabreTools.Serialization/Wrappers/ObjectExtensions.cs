#if NETCOREAPP

using System;

namespace SabreTools.Serialization.Wrappers
{
    /// <summary>
    /// Extensions for generic object types
    /// </summary>
    /// <see href="https://stackoverflow.com/a/72775719"/>
    internal static class ObjectExtensions
    {
        public static TClass ThrowOnNull<TClass>(this TClass? value) where TClass : class
            => value ?? throw new ArgumentNullException();
    }
}

#endif

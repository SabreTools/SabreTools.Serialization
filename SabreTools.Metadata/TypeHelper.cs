using System;
using System.Reflection;
using System.Xml.Serialization;
using SabreTools.Data.Models.Metadata;

namespace SabreTools.Metadata
{
    // TODO: Investigate ways of either caching or speeding up these methods
    public static class TypeHelper
    {
        /// <summary>
        /// Attempt to get the DatItem type from the name
        /// </summary>
        public static Type? GetDatItemType(string? itemType)
        {
            if (string.IsNullOrEmpty(itemType))
                return null;

            // Loop through all loaded assemblies
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                // If not all types can be loaded, use the ones that could be
                Type?[] assemblyTypes = [];
                try
                {
                    assemblyTypes = assembly.GetTypes();
                }
                catch (ReflectionTypeLoadException rtle)
                {
                    assemblyTypes = Array.FindAll(rtle.Types ?? [], t => t is not null);
                }

                // Loop through all types
                foreach (Type? type in assemblyTypes)
                {
                    // If the type is invalid
                    if (type is null)
                        continue;

                    // If the type isn't a class or doesn't implement the interface
                    if (!type.IsClass || !typeof(DatItem).IsAssignableFrom(type))
                        continue;

                    // Get the XML type name
                    string? elementName = GetXmlRootAttributeElementName(type);
                    if (elementName is null)
                        continue;

                    // If the name matches
                    if (string.Equals(elementName, itemType, StringComparison.OrdinalIgnoreCase))
                        return type;
                }
            }

            return null;
        }

        /// <summary>
        /// Attempt to get the XmlRootAttribute.ElementName value from a type
        /// </summary>
        public static string? GetXmlRootAttributeElementName(Type? type)
        {
            if (type is null)
                return null;

#if NET20 || NET35 || NET40
            return (Attribute.GetCustomAttribute(type, typeof(XmlRootAttribute)) as XmlRootAttribute)!.ElementName;
#else
            return type.GetCustomAttribute<XmlRootAttribute>()?.ElementName;
#endif
        }
    }
}

using System;
using System.Reflection;
using System.Xml.Serialization;
using SabreTools.Data.Models.Metadata;

namespace SabreTools.Metadata.Filter
{
    /// <summary>
    /// Represents a single filter key
    /// </summary>
    public class FilterKey
    {
        #region Properties

        /// <summary>
        /// Item name associated with the filter
        /// </summary>
        public readonly string ItemName;

        /// <summary>
        /// Field name associated with the filter
        /// </summary>
        public readonly string FieldName;

        #endregion

        #region Constants

        /// <summary>
        /// Cached item type names for filter selection
        /// </summary>
#if NET5_0_OR_GREATER
        private static readonly string[] _datItemTypeNames = Enum.GetNames<ItemType>();
#else
        private static readonly string[] _datItemTypeNames = Enum.GetNames(typeof(ItemType));
#endif

        /// <summary>
        /// Known keys for Adjuster
        /// </summary>
        private static readonly string[] _adjusterKeys =
        [
            "default",
            "name"
        ];

        /// <summary>
        /// Known keys for Analog
        /// </summary>
        private static readonly string[] _analogKeys =
        [
            "mask"
        ];

        /// <summary>
        /// Known keys for Archive
        /// </summary>
        private static readonly string[] _archiveKeys =
        [
            "additional",
            "adult",
            "alt",
            "bios",
            "categories",
            "clone",
            "clonetag",
            "complete",
            "dat",
            "datternote",
            "description",
            "devstatus",
            "gameid1",
            "gameid2",
            "langchecked",
            "languages",
            "licensed",
            "listed",
            "mergeof",
            "mergename",
            "name",
            "namealt",
            "number",
            "physical",
            "pirate",
            "private",
            "region",
            "regparent",
            "showlang",
            "special1",
            "special2",
            "stickynote",
            "version1",
            "version2",
        ];

        /// <summary>
        /// Known keys for BiosSet
        /// </summary>
        private static readonly string[] _biossetKeys =
        [
            "default",
            "description",
            "name",
        ];

        /// <summary>
        /// Known keys for Chip
        /// </summary>
        private static readonly string[] _chipKeys =
        [
            "chiptype",
            "clock",
            "flags",
            "name",
            "soundonly",
            "tag",
        ];

        /// <summary>
        /// Known keys for Condition
        /// </summary>
        private static readonly string[] _conditionKeys =
        [
            "mask",
            "relation",
            "tag",
            "value",
        ];

        /// <summary>
        /// Known keys for Configuration
        /// </summary>
        private static readonly string[] _configurationKeys =
        [
            "mask",
            "name",
            "tag",
        ];

        /// <summary>
        /// Known keys for ConfLocation
        /// </summary>
        private static readonly string[] _confLocationKeys =
        [
            "inverted",
            "name",
            "number",
        ];

        /// <summary>
        /// Known keys for ConfSetting
        /// </summary>
        private static readonly string[] _confSettingKeys =
        [
            "default",
            "name",
            "value",
        ];

        /// <summary>
        /// Known keys for Control
        /// </summary>
        private static readonly string[] _controlKeys =
        [
            "buttons",
            "controltype",
            "keydelta",
            "maximum",
            "minimum",
            "player",
            "reqbuttons",
            "reverse",
            "sensitivity",
            "ways",
            "ways2",
            "ways3",
        ];

        /// <summary>
        /// Known keys for DataArea
        /// </summary>
        private static readonly string[] _dataAreaKeys =
        [
            "endianness",
            "name",
            "size",
            "width",
        ];

        /// <summary>
        /// Known keys for Device
        /// </summary>
        private static readonly string[] _deviceKeys =
        [
            "devicetype",
            "fixedimage",
            "interface",
            "mandatory",
            "tag",
        ];

        /// <summary>
        /// Known keys for DeviceRef
        /// </summary>
        private static readonly string[] _deviceRefKeys =
        [
            "name",
        ];

        /// <summary>
        /// Known keys for DipLocation
        /// </summary>
        private static readonly string[] _dipLocationKeys =
        [
            "inverted",
            "name",
            "number",
        ];

         /// <summary>
        /// Known keys for DipSwitch
        /// </summary>
        private static readonly string[] _dipSwitchKeys =
        [
            "default",
            "mask",
            "name",
            "tag",
        ];

        #endregion

        /// <summary>
        /// Validating combined key constructor
        /// </summary>
        public FilterKey(string? key)
        {
            if (!ParseFilterId(key, out string itemName, out string fieldName))
                throw new ArgumentException($"{nameof(key)} could not be parsed", nameof(key));

            ItemName = itemName;
            FieldName = fieldName;
        }

        /// <summary>
        /// Validating discrete value constructor
        /// </summary>
        public FilterKey(string itemName, string fieldName)
        {
            if (!ParseFilterId(ref itemName, ref fieldName))
                throw new ArgumentException($"{nameof(itemName)} was not recognized", nameof(itemName));

            ItemName = itemName;
            FieldName = fieldName;
        }

        /// <inheritdoc/>
        public override string ToString() => $"{ItemName}.{FieldName}";

        /// <summary>
        /// Parse a filter ID string into the item name and field name, if possible
        /// </summary>
        private static bool ParseFilterId(string? itemFieldString, out string itemName, out string fieldName)
        {
            // Set default values
            itemName = string.Empty; fieldName = string.Empty;

            // If we don't have a filter ID, we can't do anything
            if (string.IsNullOrEmpty(itemFieldString))
                return false;

            // If we only have one part, we can't do anything
            string[] splitFilter = itemFieldString!.Split('.');
            if (splitFilter.Length != 2)
                return false;

            // Set and sanitize the filter ID
            itemName = splitFilter[0];
            fieldName = splitFilter[1];
            return ParseFilterId(ref itemName, ref fieldName);
        }

        /// <summary>
        /// Parse a filter ID string into the item name and field name, if possible
        /// </summary>
        private static bool ParseFilterId(ref string itemName, ref string fieldName)
        {
            // If we don't have a filter ID, we can't do anything
            if (string.IsNullOrEmpty(itemName) || string.IsNullOrEmpty(fieldName))
                return false;

            // Return santized values based on the split ID
            return itemName.ToLowerInvariant() switch
            {
                // Header
                "header" => ParseHeaderFilterId(ref itemName, ref fieldName),

                // Machine
                "game" => ParseMachineFilterId(ref itemName, ref fieldName),
                "machine" => ParseMachineFilterId(ref itemName, ref fieldName),
                "resource" => ParseMachineFilterId(ref itemName, ref fieldName),
                "set" => ParseMachineFilterId(ref itemName, ref fieldName),

                // DatItem
                "datitem" => ParseDatItemFilterId(ref itemName, ref fieldName),
                "item" => ParseDatItemFilterId(ref itemName, ref fieldName),
                _ => ParseDatItemFilterId(ref itemName, ref fieldName),
            };
        }

        /// <summary>
        /// Parse and validate header fields
        /// </summary>
        private static bool ParseHeaderFilterId(ref string itemName, ref string fieldName)
        {
            // Get the set of properties
            var properties = GetProperties(typeof(Header));
            if (properties is null)
                return false;

            // Get if there's a match to a property
            string localFieldName = fieldName;
            string? propertyMatch = Array.Find(properties, c => string.Equals(c, localFieldName, StringComparison.OrdinalIgnoreCase));
            if (propertyMatch is null)
                return false;

            // Return the sanitized ID
            itemName = "header";
            fieldName = propertyMatch.ToLowerInvariant();
            return true;
        }

        /// <summary>
        /// Parse and validate machine/game fields
        /// </summary>
        private static bool ParseMachineFilterId(ref string itemName, ref string fieldName)
        {
            // Get the set of properties
            var properties = GetProperties(typeof(Machine));
            if (properties is null)
                return false;

            // Get if there's a match to a property
            string localFieldName = fieldName;
            string? propertyMatch = Array.Find(properties, c => string.Equals(c, localFieldName, StringComparison.OrdinalIgnoreCase));
            if (propertyMatch is null)
                return false;

            // Return the sanitized ID
            itemName = "machine";
            fieldName = propertyMatch.ToLowerInvariant();
            return true;
        }

        /// <summary>
        /// Parse and validate item fields
        /// </summary>
        private static bool ParseDatItemFilterId(ref string itemName, ref string fieldName)
        {
            // Special case if the item name is reserved
            if (string.Equals(itemName, "datitem", StringComparison.OrdinalIgnoreCase)
                || string.Equals(itemName, "item", StringComparison.OrdinalIgnoreCase))
            {
                // Handle item type
                if (string.Equals(fieldName, "type", StringComparison.OrdinalIgnoreCase))
                {
                    itemName = "item";
                    fieldName = "type";
                    return true;
                }

                // If we get any matches
                string localFieldName = fieldName;
                string? matchedType = Array.Find(_datItemTypeNames, t => DatItemContainsField(t, localFieldName));
                if (matchedType is not null)
                {
                    // Check for a matching field
                    string? matchedField = GetMatchingField(matchedType, fieldName);
                    if (matchedField is null)
                        return false;

                    itemName = "item";
                    fieldName = matchedField;
                    return true;
                }
            }
            else
            {
                // Check for a matching field
                string? matchedField = GetMatchingField(itemName, fieldName);
                if (matchedField is null)
                    return false;

                itemName = itemName.ToLowerInvariant();
                fieldName = matchedField;
                return true;
            }

            // Nothing was found
            return false;
        }

        /// <summary>
        /// Determine if an item type contains a field
        /// </summary>
        private static bool DatItemContainsField(string itemName, string fieldName)
            => GetMatchingField(itemName, fieldName) is not null;

        /// <summary>
        /// Determine if an item type contains a field
        /// </summary>
        private static string? GetMatchingField(string itemName, string fieldName)
        {
            // Get the set of properties
            string[]? properties = itemName.ToLowerInvariant() switch
            {
                "adjuster" => _adjusterKeys,
                "analog" => _analogKeys,
                "archive" => _archiveKeys,
                "biosset" => _biossetKeys,
                "chip" => _chipKeys,
                "condition" => _conditionKeys,
                "configuration" => _configurationKeys,
                "conflocation" => _confLocationKeys,
                "confsetting" => _confSettingKeys,
                "control" => _controlKeys,
                "dataarea" => _dataAreaKeys,
                "device" => _deviceKeys,
                "deviceref" => _deviceRefKeys,
                "diplocation" => _dipLocationKeys,
                "dipswitch" => _dipSwitchKeys,
                _ => null,
            };

            // TODO: Remove this fallback path
            if (properties is null)
            {
                // Get the correct item type
                var itemType = GetDatItemType(itemName.ToLowerInvariant());
                if (itemType is null)
                    return null;

                properties = GetProperties(itemType);

                // Special cases for mismatched names
                if (properties is not null && itemType == typeof(Rom))
                    properties = [.. properties, "crc"];
            }

            if (properties is null)
                return null;

            // Get if there's a match to a property
            string? propertyMatch = Array.Find(properties, c => string.Equals(c, fieldName, StringComparison.OrdinalIgnoreCase));
            return propertyMatch?.ToLowerInvariant();
        }

        #region Reflection-based Helpers

        /// <summary>
        /// Attempt to get the DatItem type from the name
        /// </summary>
        private static Type? GetDatItemType(string? itemType)
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
#if NET20 || NET35 || NET40
                    string? elementName = (Attribute.GetCustomAttribute(type, typeof(XmlRootAttribute)) as XmlRootAttribute)!.ElementName;
#else
                    string? elementName = type.GetCustomAttribute<XmlRootAttribute>()?.ElementName;
#endif
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
        /// Get property names for the given type, if possible
        /// </summary>
        private static string[]? GetProperties(Type? type)
        {
            if (type is null)
                return null;

            var properties = type.GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public);
            if (properties is null)
                return null;

            string[] propertyNames = Array.ConvertAll(properties, f => f.Name);
            return Array.FindAll(propertyNames, s => s.Length > 0);
        }

        #endregion
    }
}

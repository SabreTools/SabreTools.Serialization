using System;
using SabreTools.Data.Models.Metadata;
using static SabreTools.Metadata.Filter.Constants;

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
            if (splitFilter.Length < 2)
                return false;

            // Set and sanitize the filter ID
            itemName = splitFilter[0];
            fieldName = string.Join(".", splitFilter, 1, splitFilter.Length - 1);
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
            // Get if there's a match to a property
            string localFieldName = fieldName;
            string? propertyMatch = Array.Find(HeaderKeys, c => string.Equals(c, localFieldName, StringComparison.OrdinalIgnoreCase));
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
            // Get if there's a match to a property
            string localFieldName = fieldName;
            string? propertyMatch = Array.Find(MachineKeys, c => string.Equals(c, localFieldName, StringComparison.OrdinalIgnoreCase));
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
                "adjuster" => AdjusterKeys,
                "archive" => ArchiveKeys,
                "biosset" => BiossetKeys,
                "chip" => ChipKeys,
                "configuration" => ConfigurationKeys,
                "conflocation" => ConfLocationKeys,
                "confsetting" => ConfSettingKeys,
                "control" => ControlKeys,
                "dataarea" => DataAreaKeys,
                "device" => DeviceKeys,
                "deviceref" => DeviceRefKeys,
                "diplocation" => DipLocationKeys,
                "dipswitch" => DipSwitchKeys,
                "dipvalue" => DipValueKeys,
                "disk" => DiskKeys,
                "diskarea" => DiskAreaKeys,
                "display" => DisplayKeys,
                "driver" => DriverKeys,
                "feature" or "partfeature" => FeatureKeys,
                "game" or "machine" or "resource" or "set" => MachineKeys,
                "header" => HeaderKeys,
                "info" => InfoKeys,
                "input" => InputKeys,
                "media" => MediaKeys,
                "original" => OriginalKeys,
                "part" => PartKeys,
                "port" => PortKeys,
                "ramoption" => RamOptionKeys,
                "release" => ReleaseKeys,
                "releasedetails" => ReleaseDetailsKeys,
                "rom" => RomKeys,
                "sample" => SampleKeys,
                "serials" => SerialsKeys,
                "sharedfeat" => SharedFeatKeys,
                "slot" => SlotKeys,
                "slotoption" => SlotOptionKeys,
                "softwarelist" => SoftwareListKeys,
                "sound" => SoundKeys,
                "sourcedetails" => SourceDetailsKeys,
                "video" => VideoKeys,
                _ => null,
            };
            if (properties is null)
                return null;

            // Get if there's a match to a property
            string? propertyMatch = Array.Find(properties, c => string.Equals(c, fieldName, StringComparison.OrdinalIgnoreCase));
            return propertyMatch?.ToLowerInvariant();
        }
    }
}

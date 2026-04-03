using System;
using System.Text.RegularExpressions;
using SabreTools.Data.Extensions;
using SabreTools.Data.Models.Metadata;
using SabreTools.Text.Extensions;

namespace SabreTools.Metadata.Filter
{
    /// <summary>
    /// Represents a single filtering object
    /// </summary>
    /// <remarks>TODO: Add ability to have a set of values that are accepted</remarks>
    public class FilterObject
    {
        /// <summary>
        /// Item key associated with the filter
        /// </summary>
        public readonly FilterKey Key;

        /// <summary>
        /// Value to match in the filter
        /// </summary>
        public readonly string? Value;

        /// <summary>
        /// Operation on how to match the filter
        /// </summary>
        public readonly Operation Operation;

        public FilterObject(string? filterString)
        {
            if (!SplitFilterString(filterString, out var keyItem, out Operation operation, out var value))
                throw new ArgumentException($"{nameof(filterString)} could not be parsed", nameof(filterString));

            Key = new FilterKey(keyItem);
            Value = value;
            Operation = operation;
        }

        public FilterObject(string itemField, string? value, string? operation)
        {
            Key = new FilterKey(itemField);
            Value = value;
            Operation = GetOperation(operation);
        }

        public FilterObject(string itemField, string? value, Operation operation)
        {
            Key = new FilterKey(itemField);
            Value = value;
            Operation = operation;
        }

        #region Matching

        /// <summary>
        /// Determine if a DictionaryBase object matches the key and value
        /// </summary>
        public bool Matches(DictionaryBase dictionaryBase)
        {
            return Operation switch
            {
                Operation.Equals => MatchesEqual(dictionaryBase),
                Operation.NotEquals => MatchesNotEqual(dictionaryBase),
                Operation.GreaterThan => MatchesGreaterThan(dictionaryBase),
                Operation.GreaterThanOrEqual => MatchesGreaterThanOrEqual(dictionaryBase),
                Operation.LessThan => MatchesLessThan(dictionaryBase),
                Operation.LessThanOrEqual => MatchesLessThanOrEqual(dictionaryBase),

                Operation.NONE => false,
                _ => false,
            };
        }

        /// <summary>
        /// Determines if a value matches exactly
        /// </summary>
        private bool MatchesEqual(DictionaryBase dictionaryBase)
        {
            // Process the check value
            if (!GetCheckValue(dictionaryBase, Key.FieldName, out string? checkValue))
                return string.IsNullOrEmpty(Value);

            // If a null value is expected
            if (checkValue is null)
                return string.IsNullOrEmpty(Value);

            // If we have both a potentally boolean check and value
            bool? checkValueBool = checkValue.AsYesNo();
            bool? matchValueBool = Value.AsYesNo();
            if (checkValueBool is not null && matchValueBool is not null)
                return checkValueBool == matchValueBool;

            // If we have both a potentially numeric check and value
            if (NumberHelper.IsNumeric(checkValue) && NumberHelper.IsNumeric(Value))
            {
                // Check Int64 values
                long? checkValueLong = NumberHelper.ConvertToInt64(checkValue);
                long? matchValueLong = NumberHelper.ConvertToInt64(Value);
                if (checkValueLong is not null && matchValueLong is not null)
                    return checkValueLong == matchValueLong;

                // Check Double values
                double? checkValueDouble = NumberHelper.ConvertToDouble(checkValue);
                double? matchValueDouble = NumberHelper.ConvertToDouble(Value);
                if (checkValueDouble is not null && matchValueDouble is not null)
                    return checkValueDouble == matchValueDouble;
            }

            // If the value might contain valid Regex
            if (Value is not null && ContainsRegex(Value))
                return Regex.IsMatch(checkValue, Value);

            return string.Equals(checkValue, Value, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Determines if a value does not match exactly
        /// </summary>
        private bool MatchesNotEqual(DictionaryBase dictionaryBase)
        {
            // Process the check value
            if (!GetCheckValue(dictionaryBase, Key.FieldName, out string? checkValue))
                return string.IsNullOrEmpty(Value);

            // If a null value is expected
            if (checkValue is null)
                return !string.IsNullOrEmpty(Value);

            // If we have both a potentally boolean check and value
            bool? checkValueBool = checkValue.AsYesNo();
            bool? matchValueBool = Value.AsYesNo();
            if (checkValueBool is not null && matchValueBool is not null)
                return checkValueBool != matchValueBool;

            // If we have both a potentially numeric check and value
            if (NumberHelper.IsNumeric(checkValue) && NumberHelper.IsNumeric(Value))
            {
                // Check Int64 values
                long? checkValueLong = NumberHelper.ConvertToInt64(checkValue);
                long? matchValueLong = NumberHelper.ConvertToInt64(Value);
                if (checkValueLong is not null && matchValueLong is not null)
                    return checkValueLong != matchValueLong;

                // Check Double values
                double? checkValueDouble = NumberHelper.ConvertToDouble(checkValue);
                double? matchValueDouble = NumberHelper.ConvertToDouble(Value);
                if (checkValueDouble is not null && matchValueDouble is not null)
                    return checkValueDouble != matchValueDouble;
            }

            // If the value might contain valid Regex
            if (Value is not null && ContainsRegex(Value))
                return !Regex.IsMatch(checkValue, Value);

            return !string.Equals(checkValue, Value, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Determines if a value is strictly greater than
        /// </summary>
        private bool MatchesGreaterThan(DictionaryBase dictionaryBase)
        {
            // Process the check value
            if (!GetCheckValue(dictionaryBase, Key.FieldName, out string? checkValue))
                return string.IsNullOrEmpty(Value);

            // Null is always failure
            if (checkValue is null)
                return false;

            // If we have both a potentially numeric check and value
            if (NumberHelper.IsNumeric(checkValue) && NumberHelper.IsNumeric(Value))
            {
                // Check Int64 values
                long? checkValueLong = NumberHelper.ConvertToInt64(checkValue);
                long? matchValueLong = NumberHelper.ConvertToInt64(Value);
                if (checkValueLong is not null && matchValueLong is not null)
                    return checkValueLong > matchValueLong;

                // Check Double values
                double? checkValueDouble = NumberHelper.ConvertToDouble(checkValue);
                double? matchValueDouble = NumberHelper.ConvertToDouble(Value);
                if (checkValueDouble is not null && matchValueDouble is not null)
                    return checkValueDouble > matchValueDouble;
            }

            return false;
        }

        /// <summary>
        /// Determines if a value is greater than or equal
        /// </summary>
        private bool MatchesGreaterThanOrEqual(DictionaryBase dictionaryBase)
        {
            // Process the check value
            if (!GetCheckValue(dictionaryBase, Key.FieldName, out string? checkValue))
                return string.IsNullOrEmpty(Value);

            // Null is always failure
            if (checkValue is null)
                return false;

            // If we have both a potentially numeric check and value
            if (NumberHelper.IsNumeric(checkValue) && NumberHelper.IsNumeric(Value))
            {
                // Check Int64 values
                long? checkValueLong = NumberHelper.ConvertToInt64(checkValue);
                long? matchValueLong = NumberHelper.ConvertToInt64(Value);
                if (checkValueLong is not null && matchValueLong is not null)
                    return checkValueLong >= matchValueLong;

                // Check Double values
                double? checkValueDouble = NumberHelper.ConvertToDouble(checkValue);
                double? matchValueDouble = NumberHelper.ConvertToDouble(Value);
                if (checkValueDouble is not null && matchValueDouble is not null)
                    return checkValueDouble >= matchValueDouble;
            }

            return false;
        }

        /// <summary>
        /// Determines if a value is strictly less than
        /// </summary>
        private bool MatchesLessThan(DictionaryBase dictionaryBase)
        {
            // Process the check value
            if (!GetCheckValue(dictionaryBase, Key.FieldName, out string? checkValue))
                return string.IsNullOrEmpty(Value);

            // Null is always failure
            if (checkValue is null)
                return false;

            // If we have both a potentially numeric check and value
            if (NumberHelper.IsNumeric(checkValue) && NumberHelper.IsNumeric(Value))
            {
                // Check Int64 values
                long? checkValueLong = NumberHelper.ConvertToInt64(checkValue);
                long? matchValueLong = NumberHelper.ConvertToInt64(Value);
                if (checkValueLong is not null && matchValueLong is not null)
                    return checkValueLong < matchValueLong;

                // Check Double values
                double? checkValueDouble = NumberHelper.ConvertToDouble(checkValue);
                double? matchValueDouble = NumberHelper.ConvertToDouble(Value);
                if (checkValueDouble is not null && matchValueDouble is not null)
                    return checkValueDouble < matchValueDouble;
            }

            return false;
        }

        /// <summary>
        /// Determines if a value is less than or equal
        /// </summary>
        private bool MatchesLessThanOrEqual(DictionaryBase dictionaryBase)
        {
            // Process the check value
            if (!GetCheckValue(dictionaryBase, Key.FieldName, out string? checkValue))
                return string.IsNullOrEmpty(Value);

            // Null is always failure
            if (checkValue is null)
                return false;

            // If we have both a potentially numeric check and value
            if (NumberHelper.IsNumeric(checkValue) && NumberHelper.IsNumeric(Value))
            {
                // Check Int64 values
                long? checkValueLong = NumberHelper.ConvertToInt64(checkValue);
                long? matchValueLong = NumberHelper.ConvertToInt64(Value);
                if (checkValueLong is not null && matchValueLong is not null)
                    return checkValueLong <= matchValueLong;

                // Check Double values
                double? checkValueDouble = NumberHelper.ConvertToDouble(checkValue);
                double? matchValueDouble = NumberHelper.ConvertToDouble(Value);
                if (checkValueDouble is not null && matchValueDouble is not null)
                    return checkValueDouble <= matchValueDouble;
            }

            return false;
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Determine if a value may contain regex for matching
        /// </summary>
        /// <remarks>
        /// If a value contains one of the following characters:
        ///     ^ $ * ? +
        /// Then it will attempt to check if the value is regex or not.
        /// If none of those characters exist, then value will assumed
        /// not to be regex.
        /// </remarks>
        private static bool ContainsRegex(string? value)
        {
            // If the value is missing, it can't be regex
            if (value is null)
                return false;

            // If we find a special character, try parsing as regex
#if NETCOREAPP || NETSTANDARD2_1_OR_GREATER
            if (value.Contains('^')
                || value.Contains('$')
                || value.Contains('*')
                || value.Contains('?')
                || value.Contains('+'))
#else
            if (value.Contains("^")
                || value.Contains("$")
                || value.Contains("*")
                || value.Contains("?")
                || value.Contains("+"))
#endif
            {
                try
                {
                    _ = new Regex(value);
                    return true;
                }
                catch
                {
                    return false;
                }
            }

            return false;
        }

        /// <summary>
        /// Get the check value for a field from a DictionaryBase
        /// </summary>
        private static bool GetCheckValue(DictionaryBase dictionaryBase, string fieldName, out string? checkValue)
        {
            // Handle the common name field
            if (fieldName == "name")
            {
                checkValue = dictionaryBase.GetName();
                return true;
            }

            // Handle type-specific properties
            switch (dictionaryBase)
            {
                case Adjuster item when fieldName == "default":
                    checkValue = item.Default.FromYesNo();
                    return true;

                case Analog item when fieldName == "mask":
                    checkValue = item.Mask;
                    return true;

                case Archive item when fieldName == "description":
                    checkValue = item.Description;
                    return true;

                case BiosSet item when fieldName == "default":
                    checkValue = item.Default.FromYesNo();
                    return true;
                case BiosSet item when fieldName == "description":
                    checkValue = item.Description;
                    return true;

                case Chip item when fieldName == "type":
                    checkValue = item.ChipType?.AsStringValue();
                    return true;
                case Chip item when fieldName == "soundonly":
                    checkValue = item.SoundOnly.FromYesNo();
                    return true;
                case Chip item when fieldName == "tag":
                    checkValue = item.Tag;
                    return true;

                case Condition item when fieldName == "mask":
                    checkValue = item.Mask;
                    return true;
                case Condition item when fieldName == "relation":
                    checkValue = item.Relation?.AsStringValue();
                    return true;
                case Condition item when fieldName == "tag":
                    checkValue = item.Tag;
                    return true;
                case Condition item when fieldName == "value":
                    checkValue = item.Value;
                    return true;

                case Configuration item when fieldName == "mask":
                    checkValue = item.Mask;
                    return true;
                case Configuration item when fieldName == "tag":
                    checkValue = item.Tag;
                    return true;

                case ConfLocation item when fieldName == "inverted":
                    checkValue = item.Inverted.FromYesNo();
                    return true;

                case ConfSetting item when fieldName == "default":
                    checkValue = item.Default.FromYesNo();
                    return true;
                case ConfSetting item when fieldName == "value":
                    checkValue = item.Value;
                    return true;

                case Control item when fieldName == "type":
                    checkValue = item.ControlType?.AsStringValue();
                    return true;
                case Control item when fieldName == "reverse":
                    checkValue = item.Reverse.FromYesNo();
                    return true;

                case DataArea item when fieldName == "endianness":
                    checkValue = item.Endianness?.AsStringValue();
                    return true;
                case DataArea item when fieldName == "size":
                    checkValue = item.Size?.ToString();
                    return true;

                case Device item when fieldName == "type":
                    checkValue = item.DeviceType?.AsStringValue();
                    return true;
                case Device item when fieldName == "fixedimage":
                    checkValue = item.FixedImage;
                    return true;
                case Device item when fieldName == "interface":
                    checkValue = item.Interface;
                    return true;
                case Device item when fieldName == "mandatory":
                    checkValue = item.Mandatory.FromYesNo();
                    return true;
                case Device item when fieldName == "tag":
                    checkValue = item.Tag;
                    return true;

                case DipLocation item when fieldName == "inverted":
                    checkValue = item.Inverted.FromYesNo();
                    return true;

                case DipLocation item when fieldName == "invertwed":
                    checkValue = item.Inverted.FromYesNo();
                    return true;

                case DipSwitch item when fieldName == "default":
                    checkValue = item.Default.FromYesNo();
                    return true;
                case DipSwitch item when fieldName == "mask":
                    checkValue = item.Mask;
                    return true;
                case DipSwitch item when fieldName == "tag":
                    checkValue = item.Tag;
                    return true;

                case DipValue item when fieldName == "default":
                    checkValue = item.Default.FromYesNo();
                    return true;
                case DipValue item when fieldName == "value":
                    checkValue = item.Value;
                    return true;

                case Disk item when fieldName == "optional":
                    checkValue = item.Optional.FromYesNo();
                    return true;
                case Disk item when fieldName == "status":
                    checkValue = item.Status?.AsStringValue();
                    return true;
                case Disk item when fieldName == "writable":
                    checkValue = item.Writable.FromYesNo();
                    return true;

                case Display item when fieldName == "flipx":
                    checkValue = item.FlipX.FromYesNo();
                    return true;
                case Display item when fieldName == "refresh":
                    checkValue = item.Refresh?.ToString();
                    return true;
                case Display item when fieldName == "type":
                    checkValue = item.DisplayType?.AsStringValue();
                    return true;
                case Display item when fieldName == "tag":
                    checkValue = item.Tag;
                    return true;

                case Driver item when fieldName == "blit":
                    checkValue = item.Blit?.AsStringValue();
                    return true;
                case Driver item when fieldName == "cocktail":
                    checkValue = item.Cocktail?.AsStringValue();
                    return true;
                case Driver item when fieldName == "color":
                    checkValue = item.Color?.AsStringValue();
                    return true;
                case Driver item when fieldName == "emulation":
                    checkValue = item.Emulation?.AsStringValue();
                    return true;
                case Driver item when fieldName == "incomplete":
                    checkValue = item.Incomplete.FromYesNo();
                    return true;
                case Driver item when fieldName == "nosoundhardware":
                    checkValue = item.NoSoundHardware.FromYesNo();
                    return true;
                case Driver item when fieldName == "requiresartwork":
                    checkValue = item.RequiresArtwork.FromYesNo();
                    return true;
                case Driver item when fieldName == "savestate":
                    checkValue = item.SaveState?.AsStringValue();
                    return true;
                case Driver item when fieldName == "sound":
                    checkValue = item.Sound?.AsStringValue();
                    return true;
                case Driver item when fieldName == "status":
                    checkValue = item.Status?.AsStringValue();
                    return true;
                case Driver item when fieldName == "unofficial":
                    checkValue = item.Unofficial.FromYesNo();
                    return true;

                case Feature item when fieldName == "overall":
                    checkValue = item.Overall?.AsStringValue();
                    return true;
                case Feature item when fieldName == "status":
                    checkValue = item.Status?.AsStringValue();
                    return true;
                case Feature item when fieldName == "type":
                    checkValue = item.FeatureType?.AsStringValue();
                    return true;
                case Feature item when fieldName == "value":
                    checkValue = item.Value;
                    return true;

                case Header item when fieldName == "author":
                    checkValue = item.Author;
                    return true;
                case Header item when fieldName == "biosmode":
                    checkValue = item.BiosMode.AsStringValue();
                    return true;
                case Header item when fieldName == "build":
                    checkValue = item.Build;
                    return true;
                case Header item when fieldName == "category":
                    checkValue = item.Category;
                    return true;
                case Header item when fieldName == "comment":
                    checkValue = item.Comment;
                    return true;
                case Header item when fieldName == "date":
                    checkValue = item.Date;
                    return true;
                case Header item when fieldName == "datversion":
                    checkValue = item.DatVersion;
                    return true;
                case Header item when fieldName == "debug":
                    checkValue = item.Debug.FromYesNo();
                    return true;
                case Header item when fieldName == "description":
                    checkValue = item.Description;
                    return true;
                case Header item when fieldName == "email":
                    checkValue = item.Email;
                    return true;
                case Header item when fieldName == "emulatorversion":
                    checkValue = item.EmulatorVersion;
                    return true;
                case Header item when fieldName == "forcemerging":
                    checkValue = item.ForceMerging.AsStringValue();
                    return true;
                case Header item when fieldName == "forcenodump":
                    checkValue = item.ForceNodump.AsStringValue();
                    return true;
                case Header item when fieldName == "forcepacking":
                    checkValue = item.ForcePacking.AsStringValue();
                    return true;
                case Header item when fieldName == "forcezipping":
                    checkValue = item.ForceZipping.FromYesNo();
                    return true;
                case Header item when fieldName == "homepage":
                    checkValue = item.Homepage;
                    return true;
                case Header item when fieldName == "id":
                    checkValue = item.Id;
                    return true;
                case Header item when fieldName == "lockbiosmode":
                    checkValue = item.LockBiosMode.FromYesNo();
                    return true;
                case Header item when fieldName == "lockrommode":
                    checkValue = item.LockRomMode.FromYesNo();
                    return true;
                case Header item when fieldName == "locksamplemode":
                    checkValue = item.LockSampleMode.FromYesNo();
                    return true;
                case Header item when fieldName == "mameconfig":
                    checkValue = item.MameConfig;
                    return true;
                case Header item when fieldName == "notes":
                    checkValue = item.Notes;
                    return true;
                case Header item when fieldName == "plugin":
                    checkValue = item.Plugin;
                    return true;
                case Header item when fieldName == "refname":
                    checkValue = item.RefName;
                    return true;
                case Header item when fieldName == "rommode":
                    checkValue = item.RomMode.AsStringValue();
                    return true;
                case Header item when fieldName == "romtitle":
                    checkValue = item.RomTitle;
                    return true;
                case Header item when fieldName == "rootdir":
                    checkValue = item.RootDir;
                    return true;
                case Header item when fieldName == "samplemode":
                    checkValue = item.SampleMode.AsStringValue();
                    return true;
                case Header item when fieldName == "system":
                    checkValue = item.System;
                    return true;
                case Header item when fieldName == "timestamp":
                    checkValue = item.Timestamp;
                    return true;
                case Header item when fieldName == "type":
                    checkValue = item.Timestamp;
                    return true;
                case Header item when fieldName == "url":
                    checkValue = item.Url;
                    return true;
                case Header item when fieldName == "version":
                    checkValue = item.Version;
                    return true;

                case Info item when fieldName == "value":
                    checkValue = item.Value;
                    return true;

                case Input item when fieldName == "service":
                    checkValue = item.Service.FromYesNo();
                    return true;
                case Input item when fieldName == "tilt":
                    checkValue = item.Tilt.FromYesNo();
                    return true;

                case Machine item when fieldName == "description":
                    checkValue = item.Description;
                    return true;
                case Machine item when fieldName == "isbios":
                    checkValue = item.IsBios.FromYesNo();
                    return true;
                case Machine item when fieldName == "isdevice":
                    checkValue = item.IsDevice.FromYesNo();
                    return true;
                case Machine item when fieldName == "ismechanical":
                    checkValue = item.IsMechanical.FromYesNo();
                    return true;
                case Machine item when fieldName == "runnable":
                    checkValue = item.Runnable?.AsStringValue();
                    return true;
                case Machine item when fieldName == "supported":
                    checkValue = item.Supported?.AsStringValue();
                    return true;

                case Original item when fieldName == "content":
                    checkValue = item.Content;
                    return true;

                case Part item when fieldName == "interface":
                    checkValue = item.Interface;
                    return true;

                case Port item when fieldName == "tag":
                    checkValue = item.Tag;
                    return true;

                case RamOption item when fieldName == "content":
                    checkValue = item.Content;
                    return true;
                case RamOption item when fieldName == "default":
                    checkValue = item.Default.FromYesNo();
                    return true;

                case Release item when fieldName == "default":
                    checkValue = item.Default.FromYesNo();
                    return true;

                case Rom item when fieldName == "dispose":
                    checkValue = item.Dispose.FromYesNo();
                    return true;
                case Rom item when fieldName == "fileisavailable":
                    checkValue = item.FileIsAvailable.FromYesNo();
                    return true;
                case Rom item when fieldName == "inverted":
                    checkValue = item.Inverted.FromYesNo();
                    return true;
                case Rom item when fieldName == "loadflag":
                    checkValue = item.LoadFlag?.AsStringValue();
                    return true;
                case Rom item when fieldName == "mediatype":
                    checkValue = item.OpenMSXMediaType?.AsStringValue();
                    return true;
                case Rom item when fieldName == "mia":
                    checkValue = item.MIA.FromYesNo();
                    return true;
                case Rom item when fieldName == "optional":
                    checkValue = item.Optional.FromYesNo();
                    return true;
                case Rom item when fieldName == "size":
                    checkValue = item.Size?.ToString();
                    return true;
                case Rom item when fieldName == "soundonly":
                    checkValue = item.SoundOnly.FromYesNo();
                    return true;
                case Rom item when fieldName == "status":
                    checkValue = item.Status?.AsStringValue();
                    return true;
                case Rom item when fieldName == "value":
                    checkValue = item.Value;
                    return true;

                case SharedFeat item when fieldName == "value":
                    checkValue = item.Value;
                    return true;

                case SlotOption item when fieldName == "default":
                    checkValue = item.Default.FromYesNo();
                    return true;
                case SlotOption item when fieldName == "devname":
                    checkValue = item.DevName;
                    return true;

                case Software item when fieldName == "description":
                    checkValue = item.Description;
                    return true;
                case Software item when fieldName == "supported":
                    checkValue = item.Supported?.AsStringValue();
                    return true;

                case SoftwareList item when fieldName == "description":
                    checkValue = item.Description;
                    return true;
                case SoftwareList item when fieldName == "status":
                    checkValue = item.Status?.AsStringValue();
                    return true;
                case SoftwareList item when fieldName == "tag":
                    checkValue = item.Tag;
                    return true;

                case Sound item when fieldName == "channels":
                    checkValue = item.Channels?.ToString();
                    return true;

                case Video item when fieldName == "refresh":
                    checkValue = item.Refresh?.ToString();
                    return true;
                case Video item when fieldName == "screen":
                    checkValue = item.Screen?.AsStringValue();
                    return true;

                // Fallthrough to Dictionary-based checking
                default: break;
            }

            // If the key doesn't exist, we count it as null
            if (!dictionaryBase.ContainsKey(fieldName))
            {
                checkValue = null;
                return false;
            }

            // If the value in the dictionary is null
            checkValue = dictionaryBase.ReadString(fieldName);
            return true;
        }

        /// <summary>
        /// Derive an operation from the input string, if possible
        /// </summary>
        private static Operation GetOperation(string? operation)
        {
            return operation?.ToLowerInvariant() switch
            {
                "=" => Operation.Equals,
                "=:" => Operation.Equals,
                "==" => Operation.Equals,
                ":" => Operation.Equals,
                "::" => Operation.Equals,
                ":=" => Operation.Equals,

                "!" => Operation.NotEquals,
                "!=" => Operation.NotEquals,
                "!:" => Operation.NotEquals,

                ">" => Operation.GreaterThan,
                ">:" => Operation.GreaterThanOrEqual,
                ">=" => Operation.GreaterThanOrEqual,
                "!<" => Operation.GreaterThanOrEqual,

                "<" => Operation.LessThan,
                "<:" => Operation.LessThanOrEqual,
                "<=" => Operation.LessThanOrEqual,
                "!>" => Operation.LessThanOrEqual,

                _ => Operation.NONE,
            };
        }

        /// <summary>
        /// Derive a key, operation, and value from the input string, if possible
        /// </summary>
        private static bool SplitFilterString(string? filterString, out string? key, out Operation operation, out string? value)
        {
            // Set default values
            key = null; operation = Operation.NONE; value = null;

            if (string.IsNullOrEmpty(filterString))
                return false;

            // Trim quotations, if necessary
#if NETCOREAPP || NETSTANDARD2_1_OR_GREATER
            if (filterString!.StartsWith('\"'))
                filterString = filterString[1..^1];
#else
            if (filterString!.StartsWith("\""))
                filterString = filterString.Substring(1, filterString.Length - 2);
#endif

            // Split the string using regex
            var match = Regex.Match(filterString, @"^(?<itemField>[a-zA-Z._]+)(?<operation>[=!:><]{1,2})(?<value>.*)$", RegexOptions.Compiled);
            if (!match.Success)
                return false;

            key = match.Groups["itemField"].Value;
            operation = GetOperation(match.Groups["operation"].Value);

            // Only non-zero length values are counted as non-null
            if (match.Groups["value"]?.Value?.Length > 0)
                value = match.Groups["value"].Value;

            return true;
        }

        #endregion
    }
}

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
        /// Determine if a object matches the key and value
        /// </summary>
        public bool Matches(object obj)
        {
            return Operation switch
            {
                Operation.Equals => MatchesEqual(obj),
                Operation.NotEquals => MatchesNotEqual(obj),
                Operation.GreaterThan => MatchesGreaterThan(obj),
                Operation.GreaterThanOrEqual => MatchesGreaterThanOrEqual(obj),
                Operation.LessThan => MatchesLessThan(obj),
                Operation.LessThanOrEqual => MatchesLessThanOrEqual(obj),

                Operation.NONE => false,
                _ => false,
            };
        }

        /// <summary>
        /// Determines if a value matches exactly
        /// </summary>
        private bool MatchesEqual(object obj)
        {
            // Process the check value
            if (!GetCheckValue(obj, Key.FieldName, out string? checkValue))
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
        private bool MatchesNotEqual(object obj)
        {
            // Process the check value
            if (!GetCheckValue(obj, Key.FieldName, out string? checkValue))
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
        private bool MatchesGreaterThan(object obj)
        {
            // Process the check value
            if (!GetCheckValue(obj, Key.FieldName, out string? checkValue))
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
        private bool MatchesGreaterThanOrEqual(object obj)
        {
            // Process the check value
            if (!GetCheckValue(obj, Key.FieldName, out string? checkValue))
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
        private bool MatchesLessThan(object obj)
        {
            // Process the check value
            if (!GetCheckValue(obj, Key.FieldName, out string? checkValue))
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
        private bool MatchesLessThanOrEqual(object obj)
        {
            // Process the check value
            if (!GetCheckValue(obj, Key.FieldName, out string? checkValue))
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
        /// Get the check value for a field
        /// </summary>
        /// TODO: Figure out how to not have this hardcoded
        private static bool GetCheckValue(object obj, string fieldName, out string? checkValue)
        {
            switch (obj)
            {
                case Adjuster item: return GetCheckValue(item, fieldName, out checkValue);
                case Analog item: return GetCheckValue(item, fieldName, out checkValue);
                case Archive item: return GetCheckValue(item, fieldName, out checkValue);
                case BiosSet item: return GetCheckValue(item, fieldName, out checkValue);
                case Chip item: return GetCheckValue(item, fieldName, out checkValue);
                case Condition item: return GetCheckValue(item, fieldName, out checkValue);
                case Configuration item: return GetCheckValue(item, fieldName, out checkValue);
                case ConfLocation item: return GetCheckValue(item, fieldName, out checkValue);
                case ConfSetting item: return GetCheckValue(item, fieldName, out checkValue);
                case Control item: return GetCheckValue(item, fieldName, out checkValue);
                case DataArea item: return GetCheckValue(item, fieldName, out checkValue);
                case Device item: return GetCheckValue(item, fieldName, out checkValue);
                case DeviceRef item: return GetCheckValue(item, fieldName, out checkValue);
                case DipLocation item: return GetCheckValue(item, fieldName, out checkValue);
                case DipSwitch item: return GetCheckValue(item, fieldName, out checkValue);
                case DipValue item: return GetCheckValue(item, fieldName, out checkValue);
                case Disk item: return GetCheckValue(item, fieldName, out checkValue);
                case DiskArea item: return GetCheckValue(item, fieldName, out checkValue);
                case Display item: return GetCheckValue(item, fieldName, out checkValue);
                case Driver item: return GetCheckValue(item, fieldName, out checkValue);
                case Extension item: return GetCheckValue(item, fieldName, out checkValue);
                case Feature item: return GetCheckValue(item, fieldName, out checkValue);
                case Header item: return GetCheckValue(item, fieldName, out checkValue);
                case Info item: return GetCheckValue(item, fieldName, out checkValue);
                case Input item: return GetCheckValue(item, fieldName, out checkValue);
                case Instance item: return GetCheckValue(item, fieldName, out checkValue);
                case Machine item: return GetCheckValue(item, fieldName, out checkValue);
                case Media item: return GetCheckValue(item, fieldName, out checkValue);
                case Original item: return GetCheckValue(item, fieldName, out checkValue);
                case Part item: return GetCheckValue(item, fieldName, out checkValue);
                case Port item: return GetCheckValue(item, fieldName, out checkValue);
                case RamOption item: return GetCheckValue(item, fieldName, out checkValue);
                case Release item: return GetCheckValue(item, fieldName, out checkValue);
                case ReleaseDetails item: return GetCheckValue(item, fieldName, out checkValue);
                case Rom item: return GetCheckValue(item, fieldName, out checkValue);
                case Sample item: return GetCheckValue(item, fieldName, out checkValue);
                case Serials item: return GetCheckValue(item, fieldName, out checkValue);
                case SharedFeat item: return GetCheckValue(item, fieldName, out checkValue);
                case Slot item: return GetCheckValue(item, fieldName, out checkValue);
                case SlotOption item: return GetCheckValue(item, fieldName, out checkValue);
                case SoftwareList item: return GetCheckValue(item, fieldName, out checkValue);
                case Sound item: return GetCheckValue(item, fieldName, out checkValue);
                case SourceDetails item: return GetCheckValue(item, fieldName, out checkValue);
                case Video item: return GetCheckValue(item, fieldName, out checkValue);

                // If the key doesn't exist, we count it as null
                default:
                    checkValue = null;
                    return false;
            }
        }

        /// <summary>
        /// Get the check value for a field
        /// </summary>
        private static bool GetCheckValue(Adjuster obj, string fieldName, out string? checkValue)
        {
            switch (fieldName)
            {
                case "default":
                    checkValue = obj.Default.FromYesNo();
                    return true;
                case "name":
                    checkValue = obj.Name;
                    return true;

                // If the key doesn't exist, we count it as null
                default:
                    checkValue = null;
                    return false;
            }
        }

        /// <summary>
        /// Get the check value for a field
        /// </summary>
        private static bool GetCheckValue(Analog obj, string fieldName, out string? checkValue)
        {
            switch (fieldName)
            {
                case "mask":
                    checkValue = obj.Mask;
                    return true;

                // If the key doesn't exist, we count it as null
                default:
                    checkValue = null;
                    return false;
            }
        }

        /// <summary>
        /// Get the check value for a field
        /// </summary>
        private static bool GetCheckValue(Archive obj, string fieldName, out string? checkValue)
        {
            switch (fieldName)
            {
                case "additional":
                    checkValue = obj.Additional;
                    return true;
                case "adult":
                    checkValue = obj.Adult is null ? null : (obj.Adult == true ? "1" : "0");
                    return true;
                case "alt":
                    checkValue = obj.Alt is null ? null : (obj.Alt == true ? "1" : "0");
                    return true;
                case "bios":
                    checkValue = obj.Bios is null ? null : (obj.Bios == true ? "1" : "0");
                    return true;
                case "categories":
                    checkValue = obj.Categories;
                    return true;
                case "clone":
                    checkValue = obj.CloneTag;
                    return true;
                case "complete":
                    checkValue = obj.Complete is null ? null : (obj.Complete == true ? "1" : "0");
                    return true;
                case "dat":
                    checkValue = obj.Dat is null ? null : (obj.Dat == true ? "1" : "0");
                    return true;
                case "datternote":
                    checkValue = obj.DatterNote;
                    return true;
                case "description":
                    checkValue = obj.Description;
                    return true;
                case "devstatus":
                    checkValue = obj.DevStatus;
                    return true;
                case "gameid1":
                    checkValue = obj.GameId1;
                    return true;
                case "gameid2":
                    checkValue = obj.GameId2;
                    return true;
                case "langchecked":
                    checkValue = obj.LangChecked;
                    return true;
                case "languages":
                    checkValue = obj.Languages;
                    return true;
                case "licensed":
                    checkValue = obj.Licensed is null ? null : (obj.Licensed == true ? "1" : "0");
                    return true;
                case "listed":
                    checkValue = obj.Listed is null ? null : (obj.Listed == true ? "1" : "0");
                    return true;
                case "mergeof":
                    checkValue = obj.MergeOf;
                    return true;
                case "mergename":
                    checkValue = obj.MergeName;
                    return true;
                case "name":
                    checkValue = obj.Name;
                    return true;
                case "namealt":
                    checkValue = obj.NameAlt;
                    return true;
                case "number":
                    checkValue = obj.Number;
                    return true;
                case "physical":
                    checkValue = obj.Physical is null ? null : (obj.Physical == true ? "1" : "0");
                    return true;
                case "pirate":
                    checkValue = obj.Pirate is null ? null : (obj.Pirate == true ? "1" : "0");
                    return true;
                case "private":
                    checkValue = obj.Private is null ? null : (obj.Private == true ? "1" : "0");
                    return true;
                case "region":
                    checkValue = obj.Region;
                    return true;
                case "regparent":
                    checkValue = obj.RegParent;
                    return true;
                case "showlang":
                    checkValue = obj.ShowLang is null ? null : (obj.ShowLang == true ? "1" : "0");
                    return true;
                case "special1":
                    checkValue = obj.Special1;
                    return true;
                case "special2":
                    checkValue = obj.Special2;
                    return true;
                case "stickynote":
                    checkValue = obj.StickyNote;
                    return true;
                case "version1":
                    checkValue = obj.Version1;
                    return true;
                case "version2":
                    checkValue = obj.Version2;
                    return true;

                // If the key doesn't exist, we count it as null
                default:
                    checkValue = null;
                    return false;
            }
        }

        /// <summary>
        /// Get the check value for a field
        /// </summary>
        private static bool GetCheckValue(BiosSet obj, string fieldName, out string? checkValue)
        {
            switch (fieldName)
            {
                case "default":
                    checkValue = obj.Default.FromYesNo();
                    return true;
                case "description":
                    checkValue = obj.Description;
                    return true;
                case "name":
                    checkValue = obj.Name;
                    return true;

                // If the key doesn't exist, we count it as null
                default:
                    checkValue = null;
                    return false;
            }
        }

        /// <summary>
        /// Get the check value for a field
        /// </summary>
        private static bool GetCheckValue(Chip obj, string fieldName, out string? checkValue)
        {
            switch (fieldName)
            {
                case "type":
                    checkValue = obj.ChipType?.AsStringValue();
                    return true;
                case "clock":
                    checkValue = obj.Clock?.ToString();
                    return true;
                case "flags":
                    checkValue = obj.Flags;
                    return true;
                case "name":
                    checkValue = obj.Name;
                    return true;
                case "soundonly":
                    checkValue = obj.SoundOnly.FromYesNo();
                    return true;
                case "tag":
                    checkValue = obj.Tag;
                    return true;

                // If the key doesn't exist, we count it as null
                default:
                    checkValue = null;
                    return false;
            }
        }

        /// <summary>
        /// Get the check value for a field
        /// </summary>
        private static bool GetCheckValue(Condition obj, string fieldName, out string? checkValue)
        {
            switch (fieldName)
            {
                case "mask":
                    checkValue = obj.Mask;
                    return true;
                case "relation":
                    checkValue = obj.Relation?.AsStringValue();
                    return true;
                case "tag":
                    checkValue = obj.Tag;
                    return true;
                case "value":
                    checkValue = obj.Value;
                    return true;

                // If the key doesn't exist, we count it as null
                default:
                    checkValue = null;
                    return false;
            }
        }

        /// <summary>
        /// Get the check value for a field
        /// </summary>
        private static bool GetCheckValue(Configuration obj, string fieldName, out string? checkValue)
        {
            switch (fieldName)
            {
                case "mask":
                    checkValue = obj.Mask;
                    return true;
                case "name":
                    checkValue = obj.Name;
                    return true;
                case "tag":
                    checkValue = obj.Tag;
                    return true;

                // If the key doesn't exist, we count it as null
                default:
                    checkValue = null;
                    return false;
            }
        }

        /// <summary>
        /// Get the check value for a field
        /// </summary>
        private static bool GetCheckValue(ConfLocation obj, string fieldName, out string? checkValue)
        {
            switch (fieldName)
            {
                case "inverted":
                    checkValue = obj.Inverted.FromYesNo();
                    return true;
                case "name":
                    checkValue = obj.Name;
                    return true;
                case "number":
                    checkValue = obj.Number?.ToString();
                    return true;

                // If the key doesn't exist, we count it as null
                default:
                    checkValue = null;
                    return false;
            }
        }

        /// <summary>
        /// Get the check value for a field
        /// </summary>
        private static bool GetCheckValue(ConfSetting obj, string fieldName, out string? checkValue)
        {
            switch (fieldName)
            {
                case "default":
                    checkValue = obj.Default.FromYesNo();
                    return true;
                case "name":
                    checkValue = obj.Name;
                    return true;
                case "value":
                    checkValue = obj.Value;
                    return true;

                // If the key doesn't exist, we count it as null
                default:
                    checkValue = null;
                    return false;
            }
        }

        /// <summary>
        /// Get the check value for a field
        /// </summary>
        private static bool GetCheckValue(Control obj, string fieldName, out string? checkValue)
        {
            switch (fieldName)
            {
                case "buttons":
                    checkValue = obj.Buttons?.ToString();
                    return true;
                case "type":
                    checkValue = obj.ControlType?.AsStringValue();
                    return true;
                case "keydelta":
                    checkValue = obj.KeyDelta?.ToString();
                    return true;
                case "maximum":
                    checkValue = obj.Maximum?.ToString();
                    return true;
                case "minimum":
                    checkValue = obj.Minimum?.ToString();
                    return true;
                case "player":
                    checkValue = obj.Player?.ToString();
                    return true;
                case "reqbuttons":
                    checkValue = obj.ReqButtons?.ToString();
                    return true;
                case "reverse":
                    checkValue = obj.Reverse.FromYesNo();
                    return true;
                case "sensitivity":
                    checkValue = obj.Sensitivity?.ToString();
                    return true;
                case "ways":
                    checkValue = obj.Ways;
                    return true;
                case "ways2":
                    checkValue = obj.Ways2;
                    return true;
                case "ways3":
                    checkValue = obj.Ways3;
                    return true;

                // If the key doesn't exist, we count it as null
                default:
                    checkValue = null;
                    return false;
            }
        }

        /// <summary>
        /// Get the check value for a field
        /// </summary>
        private static bool GetCheckValue(DataArea obj, string fieldName, out string? checkValue)
        {
            switch (fieldName)
            {
                case "endianness":
                    checkValue = obj.Endianness?.AsStringValue();
                    return true;
                case "name":
                    checkValue = obj.Name;
                    return true;
                case "size":
                    checkValue = obj.Size?.ToString();
                    return true;
                case "width":
                    checkValue = obj.Width?.AsStringValue();
                    return true;

                // If the key doesn't exist, we count it as null
                default:
                    checkValue = null;
                    return false;
            }
        }

        /// <summary>
        /// Get the check value for a field
        /// </summary>
        private static bool GetCheckValue(Device obj, string fieldName, out string? checkValue)
        {
            switch (fieldName)
            {
                case "type":
                    checkValue = obj.DeviceType?.AsStringValue();
                    return true;
                case "fixedimage":
                    checkValue = obj.FixedImage;
                    return true;
                case "interface":
                    checkValue = obj.Interface;
                    return true;
                case "mandatory":
                    checkValue = obj.Mandatory.FromYesNo();
                    return true;
                case "tag":
                    checkValue = obj.Tag;
                    return true;

                // If the key doesn't exist, we count it as null
                default:
                    checkValue = null;
                    return false;
            }
        }

        /// <summary>
        /// Get the check value for a field
        /// </summary>
        private static bool GetCheckValue(DeviceRef obj, string fieldName, out string? checkValue)
        {
            switch (fieldName)
            {
                case "name":
                    checkValue = obj.Name;
                    return true;

                // If the key doesn't exist, we count it as null
                default:
                    checkValue = null;
                    return false;
            }
        }

        /// <summary>
        /// Get the check value for a field
        /// </summary>
        private static bool GetCheckValue(DipLocation obj, string fieldName, out string? checkValue)
        {
            switch (fieldName)
            {
                case "inverted":
                    checkValue = obj.Inverted.FromYesNo();
                    return true;
                case "name":
                    checkValue = obj.Name;
                    return true;
                case "number":
                    checkValue = obj.Number?.ToString();
                    return true;

                // If the key doesn't exist, we count it as null
                default:
                    checkValue = null;
                    return false;
            }
        }

        /// <summary>
        /// Get the check value for a field
        /// </summary>
        private static bool GetCheckValue(DipSwitch obj, string fieldName, out string? checkValue)
        {
            switch (fieldName)
            {
                case "default":
                    checkValue = obj.Default.FromYesNo();
                    return true;
                case "mask":
                    checkValue = obj.Mask;
                    return true;
                case "name":
                    checkValue = obj.Name;
                    return true;
                case "tag":
                    checkValue = obj.Tag;
                    return true;

                // If the key doesn't exist, we count it as null
                default:
                    checkValue = null;
                    return false;
            }
        }

        /// <summary>
        /// Get the check value for a field
        /// </summary>
        private static bool GetCheckValue(DipValue obj, string fieldName, out string? checkValue)
        {
            switch (fieldName)
            {
                case "default":
                    checkValue = obj.Default.FromYesNo();
                    return true;
                case "name":
                    checkValue = obj.Name;
                    return true;
                case "value":
                    checkValue = obj.Value;
                    return true;

                // If the key doesn't exist, we count it as null
                default:
                    checkValue = null;
                    return false;
            }
        }

        /// <summary>
        /// Get the check value for a field
        /// </summary>
        private static bool GetCheckValue(Disk obj, string fieldName, out string? checkValue)
        {
            switch (fieldName)
            {
                case "flags":
                    checkValue = obj.Flags;
                    return true;
                case "index":
                    checkValue = obj.Index?.ToString();
                    return true;
                case "md5":
                    checkValue = obj.MD5;
                    return true;
                case "merge":
                    checkValue = obj.Merge;
                    return true;
                case "name":
                    checkValue = obj.Name;
                    return true;
                case "optional":
                    checkValue = obj.Optional.FromYesNo();
                    return true;
                case "region":
                    checkValue = obj.Region;
                    return true;
                case "sha1":
                    checkValue = obj.SHA1;
                    return true;
                case "status":
                    checkValue = obj.Status?.AsStringValue();
                    return true;
                case "writable":
                    checkValue = obj.Writable.FromYesNo();
                    return true;

                // If the key doesn't exist, we count it as null
                default:
                    checkValue = null;
                    return false;
            }
        }

        /// <summary>
        /// Get the check value for a field
        /// </summary>
        private static bool GetCheckValue(DiskArea obj, string fieldName, out string? checkValue)
        {
            switch (fieldName)
            {
                case "name":
                    checkValue = obj.Name;
                    return true;

                // If the key doesn't exist, we count it as null
                default:
                    checkValue = null;
                    return false;
            }
        }

        /// <summary>
        /// Get the check value for a field
        /// </summary>
        private static bool GetCheckValue(Display obj, string fieldName, out string? checkValue)
        {
            switch (fieldName)
            {
                case "aspectx":
                    checkValue = obj.AspectX?.ToString();
                    return true;
                case "aspecty":
                    checkValue = obj.AspectY?.ToString();
                    return true;
                case "flipx":
                    checkValue = obj.FlipX.FromYesNo();
                    return true;
                case "hbend":
                    checkValue = obj.HBEnd?.ToString();
                    return true;
                case "hbstart":
                    checkValue = obj.HBStart?.ToString();
                    return true;
                case "height":
                case "y":
                    checkValue = obj.Height?.ToString();
                    return true;
                case "htotal":
                    checkValue = obj.HTotal?.ToString();
                    return true;
                case "pixclock":
                    checkValue = obj.PixClock?.ToString();
                    return true;
                case "refresh":
                case "freq":
                    checkValue = obj.Refresh?.ToString();
                    return true;
                case "rotate":
                case "orientation":
                    checkValue = obj.Rotate?.AsStringValue();
                    return true;
                case "type":
                case "screen":
                    checkValue = obj.DisplayType?.AsStringValue();
                    return true;
                case "tag":
                    checkValue = obj.Tag;
                    return true;
                case "vbend":
                    checkValue = obj.VBEnd?.ToString();
                    return true;
                case "vbstart":
                    checkValue = obj.VBStart?.ToString();
                    return true;
                case "vtotal":
                    checkValue = obj.VTotal?.ToString();
                    return true;
                case "width":
                case "x":
                    checkValue = obj.Width?.ToString();
                    return true;

                // If the key doesn't exist, we count it as null
                default:
                    checkValue = null;
                    return false;
            }
        }

        /// <summary>
        /// Get the check value for a field
        /// </summary>
        private static bool GetCheckValue(Driver obj, string fieldName, out string? checkValue)
        {
            switch (fieldName)
            {
                case "blit":
                    checkValue = obj.Blit?.AsStringValue();
                    return true;
                case "cocktail":
                    checkValue = obj.Cocktail?.AsStringValue();
                    return true;
                case "color":
                    checkValue = obj.Color?.AsStringValue();
                    return true;
                case "emulation":
                    checkValue = obj.Emulation?.AsStringValue();
                    return true;
                case "incomplete":
                    checkValue = obj.Incomplete.FromYesNo();
                    return true;
                case "nosoundhardware":
                    checkValue = obj.NoSoundHardware.FromYesNo();
                    return true;
                case "palettesize":
                    checkValue = obj.PaletteSize;
                    return true;
                case "requiresartwork":
                    checkValue = obj.RequiresArtwork.FromYesNo();
                    return true;
                case "savestate":
                    checkValue = obj.SaveState?.AsStringValue();
                    return true;
                case "sound":
                    checkValue = obj.Sound?.AsStringValue();
                    return true;
                case "status":
                    checkValue = obj.Status?.AsStringValue();
                    return true;
                case "unofficial":
                    checkValue = obj.Unofficial.FromYesNo();
                    return true;

                // If the key doesn't exist, we count it as null
                default:
                    checkValue = null;
                    return false;
            }
        }

        /// <summary>
        /// Get the check value for a field
        /// </summary>
        private static bool GetCheckValue(Extension obj, string fieldName, out string? checkValue)
        {
            switch (fieldName)
            {
                case "name":
                    checkValue = obj.Name;
                    return true;

                // If the key doesn't exist, we count it as null
                default:
                    checkValue = null;
                    return false;
            }
        }

        /// <summary>
        /// Get the check value for a field
        /// </summary>
        private static bool GetCheckValue(Feature obj, string fieldName, out string? checkValue)
        {
            switch (fieldName)
            {
                case "name":
                    checkValue = obj.Name;
                    return true;
                case "overall":
                    checkValue = obj.Overall?.AsStringValue();
                    return true;
                case "status":
                    checkValue = obj.Status?.AsStringValue();
                    return true;
                case "type":
                    checkValue = obj.FeatureType?.AsStringValue();
                    return true;
                case "value":
                    checkValue = obj.Value;
                    return true;

                // If the key doesn't exist, we count it as null
                default:
                    checkValue = null;
                    return false;
            }
        }

        /// <summary>
        /// Get the check value for a field
        /// </summary>
        private static bool GetCheckValue(Header obj, string fieldName, out string? checkValue)
        {
            switch (fieldName)
            {
                case "author":
                    checkValue = obj.Author;
                    return true;
                case "biosmode":
                    checkValue = obj.BiosMode.AsStringValue();
                    return true;
                case "build":
                    checkValue = obj.Build;
                    return true;
                // Header.CanOpen is intentionally skipped
                case "category":
                    checkValue = obj.Category;
                    return true;
                case "comment":
                    checkValue = obj.Comment;
                    return true;
                case "date":
                    checkValue = obj.Date;
                    return true;
                case "datversion":
                    checkValue = obj.DatVersion;
                    return true;
                case "debug":
                    checkValue = obj.Debug.FromYesNo();
                    return true;
                case "description":
                    checkValue = obj.Description;
                    return true;
                case "email":
                    checkValue = obj.Email;
                    return true;
                case "emulatorversion":
                    checkValue = obj.EmulatorVersion;
                    return true;
                case "filename":
                    checkValue = obj.FileName;
                    return true;
                case "forcemerging":
                    checkValue = obj.ForceMerging.AsStringValue();
                    return true;
                case "forcenodump":
                    checkValue = obj.ForceNodump.AsStringValue();
                    return true;
                case "forcepacking":
                    checkValue = obj.ForcePacking.AsStringValue();
                    return true;
                case "forcezipping":
                    checkValue = obj.ForceZipping.FromYesNo();
                    return true;
                // Header.HeaderRow is intentionally skipped
                case "header":
                case "headerskipper":
                case "skipper":
                    checkValue = obj.HeaderSkipper;
                    return true;
                case "homepage":
                    checkValue = obj.Homepage;
                    return true;
                case "id":
                    checkValue = obj.Id;
                    return true;
                // Header.Images is intentionally skipped
                case "imfolder":
                    checkValue = obj.ImFolder;
                    return true;
                // Header.Infos is intentionally skipped
                case "lockbiosmode":
                    checkValue = obj.LockBiosMode.FromYesNo();
                    return true;
                case "lockrommode":
                    checkValue = obj.LockRomMode.FromYesNo();
                    return true;
                case "locksamplemode":
                    checkValue = obj.LockSampleMode.FromYesNo();
                    return true;
                case "mameconfig":
                    checkValue = obj.MameConfig;
                    return true;
                case "name":
                    checkValue = obj.Name;
                    return true;
                // Header.NewDat is intentionally skipped
                case "notes":
                    checkValue = obj.Notes;
                    return true;
                case "plugin":
                    checkValue = obj.Plugin;
                    return true;
                case "refname":
                    checkValue = obj.RefName;
                    return true;
                case "rommode":
                    checkValue = obj.RomMode.AsStringValue();
                    return true;
                case "romtitle":
                    checkValue = obj.RomTitle;
                    return true;
                case "rootdir":
                    checkValue = obj.RootDir;
                    return true;
                case "samplemode":
                    checkValue = obj.SampleMode.AsStringValue();
                    return true;
                case "schemalocation":
                    checkValue = obj.SchemaLocation;
                    return true;
                case "screenshotsheight":
                    checkValue = obj.ScreenshotsHeight;
                    return true;
                case "screenshotswidth":
                    checkValue = obj.ScreenshotsWidth;
                    return true;
                // Header.Search is intentionally skipped
                case "system":
                    checkValue = obj.System;
                    return true;
                case "timestamp":
                    checkValue = obj.Timestamp;
                    return true;
                case "type":
                    checkValue = obj.Timestamp;
                    return true;
                case "url":
                    checkValue = obj.Url;
                    return true;
                case "version":
                    checkValue = obj.Version;
                    return true;

                // If the key doesn't exist, we count it as null
                default:
                    checkValue = null;
                    return false;
            }
        }

        /// <summary>
        /// Get the check value for a field
        /// </summary>
        private static bool GetCheckValue(Info obj, string fieldName, out string? checkValue)
        {
            switch (fieldName)
            {
                case "name":
                    checkValue = obj.Name;
                    return true;
                case "value":
                    checkValue = obj.Value;
                    return true;

                // If the key doesn't exist, we count it as null
                default:
                    checkValue = null;
                    return false;
            }
        }

        /// <summary>
        /// Get the check value for a field
        /// </summary>
        private static bool GetCheckValue(Input obj, string fieldName, out string? checkValue)
        {
            switch (fieldName)
            {
                case "buttons":
                    checkValue = obj.Buttons?.ToString();
                    return true;
                case "coins":
                    checkValue = obj.Coins?.ToString();
                    return true;
                case "controlattr":
                    checkValue = obj.ControlAttr;
                    return true;
                case "players":
                    checkValue = obj.Players?.ToString();
                    return true;
                case "service":
                    checkValue = obj.Service.FromYesNo();
                    return true;
                case "tilt":
                    checkValue = obj.Tilt.FromYesNo();
                    return true;

                // If the key doesn't exist, we count it as null
                default:
                    checkValue = null;
                    return false;
            }
        }

        /// <summary>
        /// Get the check value for a field
        /// </summary>
        private static bool GetCheckValue(Instance obj, string fieldName, out string? checkValue)
        {
            switch (fieldName)
            {
                case "name":
                    checkValue = obj.Name;
                    return true;

                // If the key doesn't exist, we count it as null
                default:
                    checkValue = null;
                    return false;
            }
        }

        /// <summary>
        /// Get the check value for a field
        /// </summary>
        private static bool GetCheckValue(Machine obj, string fieldName, out string? checkValue)
        {
            switch (fieldName)
            {
                case "board":
                    checkValue = obj.Board;
                    return true;
                case "buttons":
                    checkValue = obj.Buttons;
                    return true;
                case "category":
                    checkValue = obj.Category is null ? null : string.Join(", ", obj.Category);
                    return true;
                case "cloneof":
                    checkValue = obj.CloneOf;
                    return true;
                case "cloneofid":
                    checkValue = obj.CloneOfId;
                    return true;
                case "comment":
                    checkValue = obj.Comment is null ? null : string.Join(", ", obj.Comment);
                    return true;
                case "company":
                    checkValue = obj.Company;
                    return true;
                case "control":
                    checkValue = obj.Control;
                    return true;
                case "crc":
                    checkValue = obj.CRC;
                    return true;
                case "country":
                    checkValue = obj.Country;
                    return true;
                case "description":
                    checkValue = obj.Description;
                    return true;
                case "developer":
                    checkValue = obj.Developer;
                    return true;
                case "dirname":
                    checkValue = obj.DirName;
                    return true;
                case "displaycount":
                    checkValue = obj.DisplayCount;
                    return true;
                case "displaytype":
                    checkValue = obj.DisplayType;
                    return true;
                case "duplicateid":
                    checkValue = obj.DuplicateID;
                    return true;
                case "emulator":
                    checkValue = obj.Emulator;
                    return true;
                case "enabled":
                    checkValue = obj.Enabled;
                    return true;
                case "extra":
                    checkValue = obj.Extra;
                    return true;
                case "favorite":
                    checkValue = obj.Favorite;
                    return true;
                case "genmsxid":
                    checkValue = obj.GenMSXID;
                    return true;
                case "genre":
                    checkValue = obj.Genre;
                    return true;
                case "hash":
                    checkValue = obj.Hash;
                    return true;
                case "history":
                    checkValue = obj.History;
                    return true;
                case "id":
                    checkValue = obj.Id;
                    return true;
                case "im1crc":
                    checkValue = obj.Im1CRC;
                    return true;
                case "im2crc":
                    checkValue = obj.Im2CRC;
                    return true;
                case "imagenumber":
                    checkValue = obj.ImageNumber;
                    return true;
                case "isbios":
                    checkValue = obj.IsBios.FromYesNo();
                    return true;
                case "isdevice":
                    checkValue = obj.IsDevice.FromYesNo();
                    return true;
                case "ismechanical":
                    checkValue = obj.IsMechanical.FromYesNo();
                    return true;
                case "language":
                    checkValue = obj.Language;
                    return true;
                case "location":
                    checkValue = obj.Location;
                    return true;
                case "manufacturer":
                    checkValue = obj.Manufacturer;
                    return true;
                case "name":
                    checkValue = obj.Name;
                    return true;
                case "notes":
                    checkValue = obj.Notes;
                    return true;
                case "playedcount":
                    checkValue = obj.PlayedCount;
                    return true;
                case "playedtime":
                    checkValue = obj.PlayedTime;
                    return true;
                case "players":
                    checkValue = obj.Players;
                    return true;
                case "publisher":
                    checkValue = obj.Publisher;
                    return true;
                case "ratings":
                    checkValue = obj.Ratings;
                    return true;
                case "rebuildto":
                    checkValue = obj.RebuildTo;
                    return true;
                case "relatedto":
                    checkValue = obj.RelatedTo;
                    return true;
                case "releasenumber":
                    checkValue = obj.ReleaseNumber;
                    return true;
                case "romof":
                    checkValue = obj.RomOf;
                    return true;
                case "rotation":
                    checkValue = obj.Rotation;
                    return true;
                case "runnable":
                    checkValue = obj.Runnable?.AsStringValue();
                    return true;
                case "sampleof":
                    checkValue = obj.SampleOf;
                    return true;
                case "savetype":
                    checkValue = obj.SaveType;
                    return true;
                case "score":
                    checkValue = obj.Score;
                    return true;
                case "source":
                    checkValue = obj.Source;
                    return true;
                case "sourcefile":
                    checkValue = obj.SourceFile;
                    return true;
                case "sourcerom":
                    checkValue = obj.SourceRom;
                    return true;
                case "status":
                    checkValue = obj.Status;
                    return true;
                case "subgenre":
                    checkValue = obj.Subgenre;
                    return true;
                case "supported":
                    checkValue = obj.Supported?.AsStringValue();
                    return true;
                case "system":
                    checkValue = obj.System;
                    return true;
                case "tags":
                    checkValue = obj.Tags;
                    return true;
                case "titleid":
                    checkValue = obj.TitleID;
                    return true;
                case "url":
                    checkValue = obj.Url;
                    return true;
                case "year":
                    checkValue = obj.Year;
                    return true;

                // If the key doesn't exist, we count it as null
                default:
                    checkValue = null;
                    return false;
            }
        }

        /// <summary>
        /// Get the check value for a field
        /// </summary>
        private static bool GetCheckValue(Media obj, string fieldName, out string? checkValue)
        {
            switch (fieldName)
            {
                case "md5":
                    checkValue = obj.MD5;
                    return true;
                case "name":
                    checkValue = obj.Name;
                    return true;
                case "sha1":
                    checkValue = obj.SHA1;
                    return true;
                case "sha256":
                    checkValue = obj.SHA256;
                    return true;
                case "spamsum":
                    checkValue = obj.SpamSum;
                    return true;

                // If the key doesn't exist, we count it as null
                default:
                    checkValue = null;
                    return false;
            }
        }

        /// <summary>
        /// Get the check value for a field
        /// </summary>
        private static bool GetCheckValue(Original obj, string fieldName, out string? checkValue)
        {
            switch (fieldName)
            {
                case "content":
                    checkValue = obj.Content;
                    return true;
                case "value":
                    checkValue = obj.Value.FromYesNo();
                    return true;

                // If the key doesn't exist, we count it as null
                default:
                    checkValue = null;
                    return false;
            }
        }

        /// <summary>
        /// Get the check value for a field
        /// </summary>
        private static bool GetCheckValue(Part obj, string fieldName, out string? checkValue)
        {
            switch (fieldName)
            {
                case "interface":
                    checkValue = obj.Interface;
                    return true;
                case "name":
                    checkValue = obj.Name;
                    return true;

                // If the key doesn't exist, we count it as null
                default:
                    checkValue = null;
                    return false;
            }
        }

        /// <summary>
        /// Get the check value for a field
        /// </summary>
        private static bool GetCheckValue(Port obj, string fieldName, out string? checkValue)
        {
            switch (fieldName)
            {
                case "tag":
                    checkValue = obj.Tag;
                    return true;

                // If the key doesn't exist, we count it as null
                default:
                    checkValue = null;
                    return false;
            }
        }

        /// <summary>
        /// Get the check value for a field
        /// </summary>
        private static bool GetCheckValue(RamOption obj, string fieldName, out string? checkValue)
        {
            switch (fieldName)
            {
                case "content":
                    checkValue = obj.Content;
                    return true;
                case "default":
                    checkValue = obj.Default.FromYesNo();
                    return true;
                case "name":
                    checkValue = obj.Name;
                    return true;

                // If the key doesn't exist, we count it as null
                default:
                    checkValue = null;
                    return false;
            }
        }

        /// <summary>
        /// Get the check value for a field
        /// </summary>
        private static bool GetCheckValue(Release obj, string fieldName, out string? checkValue)
        {
            switch (fieldName)
            {
                case "date":
                    checkValue = obj.Date;
                    return true;
                case "default":
                    checkValue = obj.Default.FromYesNo();
                    return true;
                case "language":
                    checkValue = obj.Language;
                    return true;
                case "name":
                    checkValue = obj.Name;
                    return true;
                case "region":
                    checkValue = obj.Region;
                    return true;

                // If the key doesn't exist, we count it as null
                default:
                    checkValue = null;
                    return false;
            }
        }

        /// <summary>
        /// Get the check value for a field
        /// </summary>
        private static bool GetCheckValue(ReleaseDetails obj, string fieldName, out string? checkValue)
        {
            switch (fieldName)
            {
                case "appendtonumber":
                    checkValue = obj.AppendToNumber;
                    return true;
                case "archivename":
                    checkValue = obj.ArchiveName;
                    return true;
                case "category":
                    checkValue = obj.Category;
                    return true;
                case "comment":
                    checkValue = obj.Comment;
                    return true;
                case "date":
                    checkValue = obj.Date;
                    return true;
                case "dirname":
                    checkValue = obj.DirName;
                    return true;
                case "group":
                    checkValue = obj.Group;
                    return true;
                case "id":
                    checkValue = obj.Id;
                    return true;
                case "nfocrc":
                    checkValue = obj.NfoCRC;
                    return true;
                case "nfoname":
                    checkValue = obj.NfoName;
                    return true;
                case "nfosize":
                    checkValue = obj.NfoSize;
                    return true;
                case "origin":
                    checkValue = obj.Origin;
                    return true;
                case "originalformat":
                    checkValue = obj.OriginalFormat;
                    return true;
                case "region":
                    checkValue = obj.Region;
                    return true;
                case "rominfo":
                    checkValue = obj.RomInfo;
                    return true;
                case "tool":
                    checkValue = obj.Tool;
                    return true;

                // If the key doesn't exist, we count it as null
                default:
                    checkValue = null;
                    return false;
            }
        }

        /// <summary>
        /// Get the check value for a field
        /// </summary>
        private static bool GetCheckValue(Rom obj, string fieldName, out string? checkValue)
        {
            switch (fieldName)
            {
                case "album":
                    checkValue = obj.Album;
                    return true;
                case "altromname":
                    checkValue = obj.AltRomname;
                    return true;
                case "alttitle":
                    checkValue = obj.AltTitle;
                    return true;
                case "artist":
                    checkValue = obj.Artist;
                    return true;
                case "asrdetectedlang":
                    checkValue = obj.ASRDetectedLang;
                    return true;
                case "asrdetectedlangconf":
                    checkValue = obj.ASRDetectedLangConf;
                    return true;
                case "asrtranscribedlang":
                    checkValue = obj.ASRTranscribedLang;
                    return true;
                case "bios":
                    checkValue = obj.Bios;
                    return true;
                case "bitrate":
                    checkValue = obj.Bitrate;
                    return true;
                case "bittorrentmagnethash":
                    checkValue = obj.BitTorrentMagnetHash;
                    return true;
                case "clothcoverdetectionmoduleversion":
                    checkValue = obj.ClothCoverDetectionModuleVersion;
                    return true;
                case "collectioncatalognumber":
                    checkValue = obj.CollectionCatalogNumber;
                    return true;
                case "comment":
                    checkValue = obj.Comment;
                    return true;
                case "crc16":
                    checkValue = obj.CRC16;
                    return true;
                case "crc":
                case "crc32":
                    checkValue = obj.CRC32;
                    return true;
                case "crc64":
                    checkValue = obj.CRC64;
                    return true;
                case "creator":
                    checkValue = obj.Creator;
                    return true;
                case "date":
                    checkValue = obj.Date;
                    return true;
                case "dispose":
                    checkValue = obj.Dispose.FromYesNo();
                    return true;
                case "extension":
                    checkValue = obj.Extension;
                    return true;
                case "filecount":
                    checkValue = obj.FileCount?.ToString();
                    return true;
                case "fileisavailable":
                    checkValue = obj.FileIsAvailable.FromYesNo();
                    return true;
                case "flags":
                    checkValue = obj.Flags;
                    return true;
                case "format":
                    checkValue = obj.Format;
                    return true;
                case "header":
                    checkValue = obj.Header;
                    return true;
                case "height":
                    checkValue = obj.Height;
                    return true;
                case "hocrchartowordhocrversion":
                    checkValue = obj.hOCRCharToWordhOCRVersion;
                    return true;
                case "hocrchartowordmoduleversion":
                    checkValue = obj.hOCRCharToWordModuleVersion;
                    return true;
                case "hocrftstexthocrversion":
                    checkValue = obj.hOCRFtsTexthOCRVersion;
                    return true;
                case "hocrftstextmoduleversion":
                    checkValue = obj.hOCRFtsTextModuleVersion;
                    return true;
                case "hocrpageindexhocrversion":
                    checkValue = obj.hOCRPageIndexhOCRVersion;
                    return true;
                case "hocrpageindexmoduleversion":
                    checkValue = obj.hOCRPageIndexModuleVersion;
                    return true;
                case "inverted":
                    checkValue = obj.Inverted.FromYesNo();
                    return true;
                case "lastmodifiedtime":
                    checkValue = obj.LastModifiedTime;
                    return true;
                case "length":
                    checkValue = obj.Length;
                    return true;
                case "loadflag":
                    checkValue = obj.LoadFlag?.AsStringValue();
                    return true;
                case "matrixnumber":
                    checkValue = obj.MatrixNumber;
                    return true;
                case "md2":
                    checkValue = obj.MD2;
                    return true;
                case "md4":
                    checkValue = obj.MD4;
                    return true;
                case "md5":
                    checkValue = obj.MD5;
                    return true;
                case "mediatype":
                    checkValue = obj.OpenMSXMediaType?.AsStringValue();
                    return true;
                case "merge":
                    checkValue = obj.Merge;
                    return true;
                case "mia":
                    checkValue = obj.MIA.FromYesNo();
                    return true;
                case "name":
                    checkValue = obj.Name;
                    return true;
                case "offset":
                    checkValue = obj.Offset;
                    return true;
                case "optional":
                    checkValue = obj.Optional.FromYesNo();
                    return true;
                case "original":
                    checkValue = obj.Original;
                    return true;
                case "pdfmoduleversion":
                    checkValue = obj.PDFModuleVersion;
                    return true;
                case "previewimage":
                    checkValue = obj.PreviewImage;
                    return true;
                case "publisher":
                    checkValue = obj.Publisher;
                    return true;
                case "region":
                    checkValue = obj.Region;
                    return true;
                case "remark":
                    checkValue = obj.Remark;
                    return true;
                case "ripemd128":
                    checkValue = obj.RIPEMD128;
                    return true;
                case "ripemd160":
                    checkValue = obj.RIPEMD160;
                    return true;
                case "rotation":
                    checkValue = obj.Rotation;
                    return true;
                case "serial":
                    checkValue = obj.Serial;
                    return true;
                case "sha1":
                    checkValue = obj.SHA1;
                    return true;
                case "sha256":
                    checkValue = obj.SHA256;
                    return true;
                case "sha384":
                    checkValue = obj.SHA384;
                    return true;
                case "sha512":
                    checkValue = obj.SHA512;
                    return true;
                case "size":
                    checkValue = obj.Size?.ToString();
                    return true;
                case "soundonly":
                    checkValue = obj.SoundOnly.FromYesNo();
                    return true;
                case "source":
                    checkValue = obj.Source;
                    return true;
                case "spamsum":
                    checkValue = obj.SpamSum;
                    return true;
                case "start":
                    checkValue = obj.Start;
                    return true;
                case "status":
                    checkValue = obj.Status?.AsStringValue();
                    return true;
                case "summation":
                    checkValue = obj.Summation;
                    return true;
                case "tesseractocr":
                    checkValue = obj.TesseractOCR;
                    return true;
                case "tesseractocrconverted":
                    checkValue = obj.TesseractOCRConverted;
                    return true;
                case "tesseractocrdetectedlang":
                    checkValue = obj.TesseractOCRDetectedLang;
                    return true;
                case "tesseractocrdetectedlangconf":
                    checkValue = obj.TesseractOCRDetectedLangConf;
                    return true;
                case "tesseractocrdetectedscript":
                    checkValue = obj.TesseractOCRDetectedScript;
                    return true;
                case "tesseractocrdetectedscriptconf":
                    checkValue = obj.TesseractOCRDetectedScriptConf;
                    return true;
                case "tesseractocrmoduleversion":
                    checkValue = obj.TesseractOCRModuleVersion;
                    return true;
                case "tesseractocrparameters":
                    checkValue = obj.TesseractOCRParameters;
                    return true;
                case "title":
                    checkValue = obj.Title;
                    return true;
                case "track":
                    checkValue = obj.Track;
                    return true;
                case "openmsxtype":
                    checkValue = obj.OpenMSXType;
                    return true;
                case "value":
                    checkValue = obj.Value;
                    return true;
                case "whisperasrmoduleversion":
                    checkValue = obj.WhisperASRModuleVersion;
                    return true;
                case "whispermodelhash":
                    checkValue = obj.WhisperModelHash;
                    return true;
                case "whispermodelname":
                    checkValue = obj.WhisperModelName;
                    return true;
                case "whisperversion":
                    checkValue = obj.WhisperVersion;
                    return true;
                case "width":
                    checkValue = obj.Width;
                    return true;
                case "wordconfidenceinterval0to10":
                    checkValue = obj.WordConfidenceInterval0To10;
                    return true;
                case "wordconfidenceinterval11to20":
                    checkValue = obj.WordConfidenceInterval11To20;
                    return true;
                case "wordconfidenceinterval21to30":
                    checkValue = obj.WordConfidenceInterval21To30;
                    return true;
                case "wordconfidenceinterval31to40":
                    checkValue = obj.WordConfidenceInterval31To40;
                    return true;
                case "wordconfidenceinterval41to50":
                    checkValue = obj.WordConfidenceInterval41To50;
                    return true;
                case "wordconfidenceinterval51to60":
                    checkValue = obj.WordConfidenceInterval51To60;
                    return true;
                case "wordconfidenceinterval61to70":
                    checkValue = obj.WordConfidenceInterval61To70;
                    return true;
                case "wordconfidenceinterval71to80":
                    checkValue = obj.WordConfidenceInterval71To80;
                    return true;
                case "wordconfidenceinterval81to90":
                    checkValue = obj.WordConfidenceInterval81To90;
                    return true;
                case "wordconfidenceinterval91to100":
                    checkValue = obj.WordConfidenceInterval91To100;
                    return true;
                case "xxhash364":
                    checkValue = obj.xxHash364;
                    return true;
                case "xxhash3128":
                    checkValue = obj.xxHash3128;
                    return true;

                // If the key doesn't exist, we count it as null
                default:
                    checkValue = null;
                    return false;
            }
        }

        /// <summary>
        /// Get the check value for a field
        /// </summary>
        private static bool GetCheckValue(Sample obj, string fieldName, out string? checkValue)
        {
            switch (fieldName)
            {
                case "name":
                    checkValue = obj.Name;
                    return true;

                // If the key doesn't exist, we count it as null
                default:
                    checkValue = null;
                    return false;
            }
        }

        /// <summary>
        /// Get the check value for a field
        /// </summary>
        private static bool GetCheckValue(Serials obj, string fieldName, out string? checkValue)
        {
            switch (fieldName)
            {
                case "boxbarcode":
                    checkValue = obj.BoxBarcode;
                    return true;
                case "boxserial":
                    checkValue = obj.BoxSerial;
                    return true;
                case "chipserial":
                    checkValue = obj.ChipSerial;
                    return true;
                case "digitalserial1":
                    checkValue = obj.DigitalSerial1;
                    return true;
                case "digitalserial2":
                    checkValue = obj.DigitalSerial2;
                    return true;
                case "lockoutserial":
                    checkValue = obj.LockoutSerial;
                    return true;
                case "mediaserial1":
                    checkValue = obj.MediaSerial1;
                    return true;
                case "mediaserial2":
                    checkValue = obj.MediaSerial2;
                    return true;
                case "mediaserial3":
                    checkValue = obj.MediaSerial3;
                    return true;
                case "mediastamp":
                    checkValue = obj.MediaStamp;
                    return true;
                case "pcbserial":
                    checkValue = obj.PCBSerial;
                    return true;
                case "romchipserial1":
                    checkValue = obj.RomChipSerial1;
                    return true;
                case "romchipserial2":
                    checkValue = obj.RomChipSerial2;
                    return true;
                case "savechipserial":
                    checkValue = obj.SaveChipSerial;
                    return true;

                // If the key doesn't exist, we count it as null
                default:
                    checkValue = null;
                    return false;
            }
        }

        /// <summary>
        /// Get the check value for a field
        /// </summary>
        private static bool GetCheckValue(SharedFeat obj, string fieldName, out string? checkValue)
        {
            switch (fieldName)
            {
                case "name":
                    checkValue = obj.Name;
                    return true;
                case "value":
                    checkValue = obj.Value;
                    return true;

                // If the key doesn't exist, we count it as null
                default:
                    checkValue = null;
                    return false;
            }
        }

        /// <summary>
        /// Get the check value for a field
        /// </summary>
        private static bool GetCheckValue(Slot obj, string fieldName, out string? checkValue)
        {
            switch (fieldName)
            {
                case "name":
                    checkValue = obj.Name;
                    return true;

                // If the key doesn't exist, we count it as null
                default:
                    checkValue = null;
                    return false;
            }
        }

        /// <summary>
        /// Get the check value for a field
        /// </summary>
        private static bool GetCheckValue(SlotOption obj, string fieldName, out string? checkValue)
        {
            switch (fieldName)
            {
                case "default":
                    checkValue = obj.Default.FromYesNo();
                    return true;
                case "devname":
                    checkValue = obj.DevName;
                    return true;
                case "name":
                    checkValue = obj.Name;
                    return true;

                // If the key doesn't exist, we count it as null
                default:
                    checkValue = null;
                    return false;
            }
        }

        /// <summary>
        /// Get the check value for a field
        /// </summary>
        private static bool GetCheckValue(SoftwareList obj, string fieldName, out string? checkValue)
        {
            switch (fieldName)
            {
                case "filter":
                    checkValue = obj.Filter;
                    return true;
                case "name":
                    checkValue = obj.Name;
                    return true;
                case "status":
                    checkValue = obj.Status?.AsStringValue();
                    return true;
                case "tag":
                    checkValue = obj.Tag;
                    return true;

                // If the key doesn't exist, we count it as null
                default:
                    checkValue = null;
                    return false;
            }
        }

        /// <summary>
        /// Get the check value for a field
        /// </summary>
        private static bool GetCheckValue(Sound obj, string fieldName, out string? checkValue)
        {
            switch (fieldName)
            {
                case "channels":
                    checkValue = obj.Channels?.ToString();
                    return true;

                // If the key doesn't exist, we count it as null
                default:
                    checkValue = null;
                    return false;
            }
        }

        /// <summary>
        /// Get the check value for a field
        /// </summary>
        private static bool GetCheckValue(SourceDetails obj, string fieldName, out string? checkValue)
        {
            switch (fieldName)
            {
                case "appendtonumber":
                    checkValue = obj.AppendToNumber;
                    return true;
                case "comment1":
                    checkValue = obj.Comment1;
                    return true;
                case "comment2":
                    checkValue = obj.Comment2;
                    return true;
                case "dumpdate":
                    checkValue = obj.DumpDate;
                    return true;
                case "dumpdateinfo":
                    checkValue = obj.DumpDateInfo is null ? null : (obj.DumpDateInfo == true ? "1" : "0");
                    return true;
                case "dumper":
                    checkValue = obj.Dumper;
                    return true;
                case "id":
                    checkValue = obj.Id;
                    return true;
                case "link1":
                    checkValue = obj.Link1;
                    return true;
                case "link1public":
                    checkValue = obj.Link1Public is null ? null : (obj.Link1Public == true ? "1" : "0");
                    return true;
                case "link2":
                    checkValue = obj.Link2;
                    return true;
                case "link2public":
                    checkValue = obj.Link2Public is null ? null : (obj.Link2Public == true ? "1" : "0");
                    return true;
                case "link3":
                    checkValue = obj.Link3;
                    return true;
                case "link3public":
                    checkValue = obj.Link3Public is null ? null : (obj.Link3Public == true ? "1" : "0");
                    return true;
                case "mediatitle":
                    checkValue = obj.MediaTitle;
                    return true;
                case "nodump":
                    checkValue = obj.Nodump is null ? null : (obj.Nodump == true ? "1" : "0");
                    return true;
                case "origin":
                    checkValue = obj.Origin;
                    return true;
                case "originalformat":
                    checkValue = obj.OriginalFormat;
                    return true;
                case "project":
                    checkValue = obj.Project;
                    return true;
                case "region":
                    checkValue = obj.Region;
                    return true;
                case "releasedate":
                    checkValue = obj.ReleaseDate;
                    return true;
                case "releasedateinfo":
                    checkValue = obj.ReleaseDateInfo is null ? null : (obj.ReleaseDateInfo == true ? "1" : "0");
                    return true;
                case "rominfo":
                    checkValue = obj.RomInfo;
                    return true;
                case "section":
                    checkValue = obj.Section;
                    return true;
                case "tool":
                    checkValue = obj.Tool;
                    return true;

                // If the key doesn't exist, we count it as null
                default:
                    checkValue = null;
                    return false;
            }
        }

        /// <summary>
        /// Get the check value for a field
        /// </summary>
        private static bool GetCheckValue(Video obj, string fieldName, out string? checkValue)
        {
            switch (fieldName)
            {
                case "aspectx":
                    checkValue = obj.AspectX?.ToString();
                    return true;
                case "aspecty":
                    checkValue = obj.AspectY?.ToString();
                    return true;
                case "height":
                case "y":
                    checkValue = obj.Height?.ToString();
                    return true;
                case "orientation":
                case "rotate":
                    checkValue = obj.Orientation?.AsStringValue();
                    return true;
                case "refresh":
                case "freq":
                    checkValue = obj.Refresh?.ToString();
                    return true;
                case "screen":
                case "type":
                    checkValue = obj.Screen?.AsStringValue();
                    return true;
                case "width":
                case "x":
                    checkValue = obj.Width?.ToString();
                    return true;

                // If the key doesn't exist, we count it as null
                default:
                    checkValue = null;
                    return false;
            }
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

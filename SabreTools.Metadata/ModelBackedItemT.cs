using System;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace SabreTools.Metadata
{
    /// <summary>
    /// Represents an item that's backed by a DictionaryBase item
    /// </summary>
    public abstract class ModelBackedItem<T> : ModelBackedItem, IEquatable<ModelBackedItem<T>> where T : Data.Models.Metadata.DictionaryBase
    {
        /// <summary>
        /// Internal model wrapped by this DatItem
        /// </summary>
        [JsonIgnore, XmlIgnore]
        protected T _internal;

        #region Constructors

        public ModelBackedItem()
        {
            _internal = Activator.CreateInstance<T>();
        }

        #endregion

        #region Accessors

        /// <summary>
        /// Get the value from a field based on the type provided
        /// </summary>
        /// <typeparam name="U">Type of the value to get from the internal model</typeparam>
        /// <param name="fieldName">Field to retrieve</param>
        /// <returns>Value from the field, if possible</returns>
        public U? Read<U>(string fieldName)
            => _internal.Read<U>(fieldName);

        /// <summary>
        /// Get the value from a field based on the type provided
        /// </summary>
        /// <param name="fieldName">Field to retrieve</param>
        /// <returns>Value from the field, if possible</returns>
        public bool? ReadBool(string fieldName)
            => _internal.ReadBool(fieldName);

        /// <summary>
        /// Get the value from a field based on the type provided
        /// </summary>
        /// <param name="fieldName">Field to retrieve</param>
        /// <returns>Value from the field, if possible</returns>
        public double? ReadDouble(string fieldName)
            => _internal.ReadDouble(fieldName);

        /// <summary>
        /// Get the value from a field based on the type provided
        /// </summary>
        /// <param name="fieldName">Field to retrieve</param>
        /// <returns>Value from the field, if possible</returns>
        public long? ReadLong(string fieldName)
            => _internal.ReadLong(fieldName);

        /// <summary>
        /// Get the value from a field based on the type provided
        /// </summary>
        /// <param name="fieldName">Field to retrieve</param>
        /// <returns>Value from the field, if possible</returns>
        public string? ReadString(string fieldName)
            => _internal.ReadString(fieldName);

        /// <summary>
        /// Get the value from a field based on the type provided
        /// </summary>
        /// <param name="fieldName">Field to retrieve</param>
        /// <returns>Value from the field, if possible</returns>
        public string[]? ReadStringArray(string fieldName)
            => _internal.ReadStringArray(fieldName);

        /// <summary>
        /// Set the value from a field based on the type provided
        /// </summary>
        /// <typeparam name="U">Type of the value to set in the internal model</typeparam>
        /// <param name="fieldName">Field to set</param>
        /// <param name="value">Value to set</param>
        /// <returns>True if the value was set, false otherwise</returns>
        public bool Write<U>(string fieldName, U? value)
            => _internal.Write(fieldName, value);

        /// <summary>
        /// Set a field from the backing item validating the field is expected
        /// </summary>
        /// TODO: Figure out how to add this to DictionaryBase
        public bool WriteWithValidation(string fieldName, object value)
        {
            // If the item or field name are missing, we can't do anything
            if (string.IsNullOrEmpty(fieldName))
                return false;

            // Retrieve the list of valid fields for the item
            var constants = TypeHelper.GetConstants(_internal.GetType());
            if (constants is null)
                return false;

            // Get the value that matches the field name provided
            string? realField = Array.Find(constants, c => string.Equals(c, fieldName, StringComparison.OrdinalIgnoreCase));
            if (realField is null)
                return false;

            // Set the field with the new value
            return _internal.Write(realField, value);
        }

        #endregion

        #region Comparision Methods

        /// <inheritdoc/>
        public abstract bool Equals(ModelBackedItem<T>? other);

        #endregion

        #region Manipulation

        /// <summary>
        /// Remove a field from the backing item
        /// </summary>
        public bool Remove(string fieldName)
            => _internal.Remove(fieldName);

        /// <summary>
        /// Replace a field from another ModelBackedItem
        /// </summary>
        public bool Replace(ModelBackedItem<T>? from, string fieldName)
            => _internal.Replace(from?._internal, fieldName);

        #endregion
    }
}

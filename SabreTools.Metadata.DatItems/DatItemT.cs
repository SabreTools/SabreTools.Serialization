using System;
using SabreTools.Data.Extensions;

namespace SabreTools.Metadata.DatItems
{
    /// <summary>
    /// Base class for all items included in a set that are backed by an internal model
    /// </summary>
    public abstract class DatItem<T> : DatItem, IEquatable<DatItem<T>>, IComparable<DatItem<T>>, ICloneable where T : Data.Models.Metadata.DatItem, new()
    {
        #region Fields

        /// <inheritdoc>/>
        public override Data.Models.Metadata.ItemType ItemType => _internal.ItemType;

        #endregion

        #region Constructors

        /// <summary>
        /// Create a default, empty object
        /// </summary>
        public DatItem()
        {
            _internal = new T();

            SetName(string.Empty);
            Machine = new Machine();
        }

        /// <summary>
        /// Create an object from the internal model
        /// </summary>
        public DatItem(T item)
        {
            _internal = item;

            Machine = new Machine();
        }

        #endregion

        #region Cloning Methods

        /// <summary>
        /// Get a clone of the current internal model
        /// </summary>
        public virtual T GetInternalClone() => (_internal.DeepClone() as T)!;

        #endregion

        #region Comparision Methods

        /// <inheritdoc/>
        public int CompareTo(DatItem<T>? other)
        {
            // If the other item doesn't exist
            if (other is null)
                return 1;

            // Get the names to avoid changing values
            string? selfName = GetName();
            string? otherName = other.GetName();

            // If the names are equal
            if (selfName == otherName)
                return Equals(other) ? 0 : 1;

            // If `otherName` is null, Compare will return > 0
            // If `selfName` is null, Compare will return < 0
            return string.Compare(selfName, otherName, StringComparison.Ordinal);
        }

        /// <summary>
        /// Determine if an item is a duplicate using partial matching logic
        /// </summary>
        /// <param name="other">DatItem to use as a baseline</param>
        /// <returns>True if the items are duplicates, false otherwise</returns>
        public virtual bool Equals(DatItem<T>? other)
        {
            // If the other value is null
            if (other is null)
                return false;

            // Get the types for comparison
            Data.Models.Metadata.ItemType selfType = ItemType;
            Data.Models.Metadata.ItemType otherType = other.ItemType;

            // If we don't have a matched type, return false
            if (selfType != otherType)
                return false;

            // Compare the internal models
            return _internal.EqualTo(other._internal);
        }

        #endregion
    }
}

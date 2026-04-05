using System;
using SabreTools.Metadata.Filter;

namespace SabreTools.Metadata.DatItems
{
    /// <summary>
    /// Base class for all items included in a set that are backed by an internal model
    /// </summary>
    public abstract class DatItem<T> : DatItem, ICloneable, IEquatable<DatItem<T>>
        where T : Data.Models.Metadata.DatItem, new()
    {
        #region Private Fields

        /// <summary>
        /// Internal model
        /// </summary>
        protected T _internal;

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
        public abstract T GetInternalClone();

        #endregion

        #region Comparision Methods

        /// <summary>
        /// Determine if an item is a duplicate using partial matching logic
        /// </summary>
        /// <param name="other">DatItem to use as a baseline</param>
        /// <returns>True if the items are duplicates, false otherwise</returns>
        public abstract bool Equals(DatItem<T>? other);

        #endregion

        #region Manipulation

        /// <inheritdoc/>
        public override bool PassesFilter(FilterRunner filterRunner)
        {
            if (Machine is not null && !Machine.PassesFilter(filterRunner))
                return false;

            return filterRunner.Run(_internal);
        }

        /// <inheritdoc/>
        public override bool PassesFilterDB(FilterRunner filterRunner)
            => filterRunner.Run(_internal);

        #endregion
    }
}

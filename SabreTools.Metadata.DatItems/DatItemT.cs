using System;
using SabreTools.Metadata.Filter;

namespace SabreTools.Metadata.DatItems
{
    /// <summary>
    /// Base class for all items included in a set that are backed by an internal model
    /// </summary>
    public abstract class DatItem<T> : DatItem, ICloneable
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

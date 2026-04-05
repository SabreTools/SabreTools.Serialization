using System;

namespace SabreTools.Metadata
{
    /// <summary>
    /// Represents an item that's backed by a constructable item
    /// </summary>
    public abstract class ModelBackedItem : IEquatable<ModelBackedItem>
    {
        #region Comparision Methods

        /// <inheritdoc/>
        public abstract bool Equals(ModelBackedItem? other);

        #endregion
    }
}

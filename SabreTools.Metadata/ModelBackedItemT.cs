using System;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace SabreTools.Metadata
{
    /// <summary>
    /// Represents an item that's backed by a constructable item
    /// </summary>
    public abstract class ModelBackedItem<T> : ModelBackedItem, IEquatable<ModelBackedItem<T>> where T : new()
    {
        /// <summary>
        /// Internal model wrapped by this DatItem
        /// </summary>
        [JsonIgnore, XmlIgnore]
        protected T _internal;

        #region Constructors

        public ModelBackedItem()
        {
            _internal = new T();
        }

        #endregion

        #region Comparision Methods

        /// <inheritdoc/>
        public abstract bool Equals(ModelBackedItem<T>? other);

        #endregion
    }
}

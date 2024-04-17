using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Wrappers
{
    public abstract class WrapperBase : IWrapper
    {
        #region Descriptive Properties

        /// <inheritdoc/>
        public string Description() => DescriptionString;

        /// <summary>
        /// Description of the object
        /// </summary>
        public abstract string DescriptionString { get; }

        #endregion

        #region JSON Export

#if !NETFRAMEWORK
        /// <summary>
        /// Export the item information as JSON
        /// </summary>
        public abstract string ExportJSON();
#endif

        #endregion
    }
}
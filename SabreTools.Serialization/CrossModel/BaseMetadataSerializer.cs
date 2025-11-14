using SabreTools.Data.Models.Metadata;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.CrossModel
{
    /// <summary>
    /// Base class for <see cref="MetadataFile"/> cross-model serializers
    /// </summary>
    /// <typeparam name="TModel">Model convertable to <see cref="MetadataFile"/></typeparam>
    public abstract class BaseMetadataSerializer<TModel> : ICrossModel<TModel, MetadataFile>
    {
        /// <inheritdoc/>
        public abstract TModel? Deserialize(MetadataFile? obj);

        /// <inheritdoc/>
        public abstract MetadataFile? Serialize(TModel? obj);
    }
}

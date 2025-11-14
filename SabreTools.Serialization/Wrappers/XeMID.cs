using System.IO;
using System.Text;
using static SabreTools.Data.Models.Xbox.Constants;

namespace SabreTools.Serialization.Wrappers
{
    public partial class XeMID : WrapperBase<Data.Models.Xbox.XeMID>
    {
        #region Descriptive Properties

        /// <inheritdoc/>
        public override string DescriptionString => "Xbox 360 Media Identifier";

        #endregion

        #region Extension Properties

        /// <summary>
        /// Get the human-readable media subtype string
        /// </summary>
        public string MediaSubtype
        {
            get
            {
                // Use the cached data if possible
                if (field != null)
                    return field;

                if (MediaSubtypes.TryGetValue(Model.MediaSubtypeIdentifier, out var mediaSubtype))
                {
                    field = mediaSubtype;
                    return field;
                }

                field = $"Unknown ({Model.MediaSubtypeIdentifier})";
                return field;
            }
        } = null;

        /// <summary>
        /// Get the human-readable publisher string
        /// </summary>
        public string Publisher
        {
            get
            {
                // Use the cached data if possible
                if (field != null)
                    return field;

                if (Publishers.TryGetValue(Model.PublisherIdentifier, out var publisher))
                {
                    field = publisher;
                    return field;
                }

                field = $"Unknown ({Model.PublisherIdentifier})";
                return field;
            }
        } = null;

        /// <summary>
        /// Get the human-readable region string
        /// </summary>
        public string Region
        {
            get
            {
                // Use the cached data if possible
                if (field != null)
                    return field;

                if (Regions.TryGetValue(Model.RegionIdentifier, out var region))
                {
                    field = region;
                    return field;
                }

                field = $"Unknown ({Model.RegionIdentifier})";
                return field;
            }
        } = null;

        /// <summary>
        /// Get the human-readable serial string
        /// </summary>
        public string Serial => $"{Model.PublisherIdentifier}-{Model.PlatformIdentifier}{Model.GameID}";

        /// <summary>
        /// Get the human-readable version string
        /// </summary>
        public string Version => $"1.{Model.SKU}";

        #endregion

        #region Constructors

        /// <inheritdoc/>
        public XeMID(Data.Models.Xbox.XeMID model, byte[] data) : base(model, data) { }

        /// <inheritdoc/>
        public XeMID(Data.Models.Xbox.XeMID model, byte[] data, int offset) : base(model, data, offset) { }

        /// <inheritdoc/>
        public XeMID(Data.Models.Xbox.XeMID model, byte[] data, int offset, int length) : base(model, data, offset, length) { }

        /// <inheritdoc/>
        public XeMID(Data.Models.Xbox.XeMID model, Stream data) : base(model, data) { }

        /// <inheritdoc/>
        public XeMID(Data.Models.Xbox.XeMID model, Stream data, long offset) : base(model, data, offset) { }

        /// <inheritdoc/>
        public XeMID(Data.Models.Xbox.XeMID model, Stream data, long offset, long length) : base(model, data, offset, length) { }

        #endregion

        #region Static Constructors

        /// <summary>
        /// Create a XeMID from a string
        /// </summary>
        /// <param name="data">String representing the data</param>
        /// <returns>A XeMID wrapper on success, null on failure</returns>
        public static XeMID? Create(string? data)
        {
            // If the data is invalid
            if (data == null || data.Length == 0)
                return null;

            try
            {
                var model = new Readers.XeMID().Deserialize(data);
                if (model == null)
                    return null;

                var ms = new MemoryStream(Encoding.ASCII.GetBytes(data));
                return new XeMID(model, ms);
            }
            catch
            {
                return null;
            }
        }

        #endregion
    }
}

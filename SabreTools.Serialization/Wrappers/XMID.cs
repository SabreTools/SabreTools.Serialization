using System.IO;
using System.Text;
using static SabreTools.Serialization.Models.Xbox.Constants;

namespace SabreTools.Serialization.Wrappers
{
    public class XMID : WrapperBase<SabreTools.Serialization.Models.Xbox.XMID>
    {
        #region Descriptive Properties

        /// <inheritdoc/>
        public override string DescriptionString => "Xbox Media Identifier";

        #endregion

        #region Extension Properties

        /// <summary>
        /// Get the human-readable publisher string
        /// </summary>
        public string Publisher
        {
            get
            {
                var publisherIdentifier = Model.PublisherIdentifier;
                if (string.IsNullOrEmpty(publisherIdentifier))
                    return "Unknown";

                if (Publishers.ContainsKey(publisherIdentifier!))
                    return Publishers[publisherIdentifier!];

                return $"Unknown ({publisherIdentifier})";
            }
        }

        /// <summary>
        /// Get the human-readable region string
        /// </summary>
        public string Region
        {
            get
            {
                var regionIdentifier = Model.RegionIdentifier;
                if (Regions.ContainsKey(regionIdentifier))
                    return Regions[regionIdentifier];

                return $"Unknown ({regionIdentifier})";
            }
        }

        /// <summary>
        /// Get the human-readable serial string
        /// </summary>
        public string Serial => $"{Model.PublisherIdentifier}-{Model.GameID}";

        /// <summary>
        /// Get the human-readable version string
        /// </summary>
        public string Version => $"1.{Model.VersionNumber}";

        #endregion

        #region Constructors

        /// <inheritdoc/>
        public XMID(SabreTools.Serialization.Models.Xbox.XMID model, byte[] data) : base(model, data) { }

        /// <inheritdoc/>
        public XMID(SabreTools.Serialization.Models.Xbox.XMID model, byte[] data, int offset) : base(model, data, offset) { }

        /// <inheritdoc/>
        public XMID(SabreTools.Serialization.Models.Xbox.XMID model, byte[] data, int offset, int length) : base(model, data, offset, length) { }

        /// <inheritdoc/>
        public XMID(SabreTools.Serialization.Models.Xbox.XMID model, Stream data) : base(model, data) { }

        /// <inheritdoc/>
        public XMID(SabreTools.Serialization.Models.Xbox.XMID model, Stream data, long offset) : base(model, data, offset) { }

        /// <inheritdoc/>
        public XMID(SabreTools.Serialization.Models.Xbox.XMID model, Stream data, long offset, long length) : base(model, data, offset, length) { }

        #endregion

        #region Static Constructors

        /// <summary>
        /// Create a XMID from a string
        /// </summary>
        /// <param name="data">String representing the data</param>
        /// <returns>A XMID wrapper on success, null on failure</returns>
        public static XMID? Create(string? data)
        {
            // If the data is invalid
            if (data == null || data.Length == 0)
                return null;

            try
            {
                var model = new Deserializers.XMID().Deserialize(data);
                if (model == null)
                    return null;

                var ms = new MemoryStream(Encoding.ASCII.GetBytes(data));
                return new XMID(model, ms);
            }
            catch
            {
                return null;
            }
        }

        #endregion
    }
}

using System.IO;
using System.Text;
using static SabreTools.Models.Xbox.Constants;

namespace SabreTools.Serialization.Wrappers
{
    public class XeMID : WrapperBase<Models.Xbox.XeMID>
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
                char mediaSubtype = this.Model.MediaSubtypeIdentifier;
                if (MediaSubtypes.ContainsKey(mediaSubtype))
                    return MediaSubtypes[mediaSubtype];

                return $"Unknown ({mediaSubtype})";
            }
        }

        /// <summary>
        /// Get the human-readable publisher string
        /// </summary>
        public string Publisher
        {
            get
            {
                var publisherIdentifier = this.Model.PublisherIdentifier;
                if (string.IsNullOrWhiteSpace(publisherIdentifier))
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
                var regionIdentifier = this.Model.RegionIdentifier;
                if (Regions.ContainsKey(regionIdentifier))
                    return Regions[regionIdentifier];

                return $"Unknown ({regionIdentifier})";
            }
        }

        /// <summary>
        /// Get the human-readable serial string
        /// </summary>
        public string Serial => $"{this.Model.PublisherIdentifier}-{this.Model.PlatformIdentifier}{this.Model.GameID}";

        /// <summary>
        /// Get the human-readable version string
        /// </summary>
        public string Version => $"1.{this.Model.SKU}";

        #endregion

        #region Constructors

        /// <inheritdoc/>
        public XeMID(Models.Xbox.XeMID? model, byte[]? data, int offset)
            : base(model, data, offset)
        {
            // All logic is handled by the base class
        }

        /// <inheritdoc/>
        public XeMID(Models.Xbox.XeMID? model, Stream? data)
            : base(model, data)
        {
            // All logic is handled by the base class
        }

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

            var binary = new Strings.XeMID().Deserialize(data);
            if (binary == null)
                return null;

            try
            {
                var ms = new MemoryStream(Encoding.ASCII.GetBytes(data));
                return new XeMID(binary, ms);
            }
            catch
            {
                return null;
            }
        }

        #endregion
    }
}
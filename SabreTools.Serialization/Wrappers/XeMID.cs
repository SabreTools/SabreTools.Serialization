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
                char mediaSubtype = Model.MediaSubtypeIdentifier;
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
        public string Serial => $"{Model.PublisherIdentifier}-{Model.PlatformIdentifier}{Model.GameID}";

        /// <summary>
        /// Get the human-readable version string
        /// </summary>
        public string Version => $"1.{Model.SKU}";

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

            try
            {
                var xemid = Deserializers.XeMID.DeserializeString(data);
                if (xemid == null)
                    return null;

                var ms = new MemoryStream(Encoding.ASCII.GetBytes(data));
                return new XeMID(xemid, ms);
            }
            catch
            {
                return null;
            }
        }

        #endregion
    }
}

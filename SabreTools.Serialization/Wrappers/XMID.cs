using System.IO;
using System.Text;
using static SabreTools.Models.Xbox.Constants;

namespace SabreTools.Serialization.Wrappers
{
    public class XMID : WrapperBase<Models.Xbox.XMID>
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
        public XMID(Models.Xbox.XMID? model, byte[]? data, int offset)
            : base(model, data, offset)
        {
            // All logic is handled by the base class
        }

        /// <inheritdoc/>
        public XMID(Models.Xbox.XMID? model, Stream? data)
            : base(model, data)
        {
            // All logic is handled by the base class
        }

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
                var model = Deserializers.XMID.DeserializeString(data);
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

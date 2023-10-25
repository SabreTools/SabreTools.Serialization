using static SabreTools.Models.Xbox.Constants;

namespace SabreTools.Serialization.Wrappers
{
    /// <summary>
    /// Contains information specific to an XGD disc
    /// </summary>
    /// TODO: Convert this to a proper WrapperBase implementation
    public class XgdInfo
    {
        #region Properties

        /// <summary>
        /// Indicates whether the information in this object is fully instantiated or not
        /// </summary>
        public bool Initialized { get; private set; }

        /// <summary>
        /// Raw XMID/XeMID string that all other information is derived from
        /// </summary>
#if NET48
        public string RawXMID { get; private set; }
#else
        public string? RawXMID { get; private set; }
#endif

        /// <summary>
        /// XGD1 XMID
        /// </summary>
#if NET48
        public Models.Xbox.XMID XMID { get; private set; }
#else
        public Models.Xbox.XMID? XMID { get; private set; }
#endif

        /// <summary>
        /// XGD2/3 XeMID
        /// </summary>
#if NET48
        public Models.Xbox.XeMID XeMID { get; private set; }
#else
        public Models.Xbox.XeMID? XeMID { get; private set; }
#endif

        #endregion

        #region Extension Properties

        /// <summary>
        /// Get the human-readable media subtype string
        /// </summary>
#if NET48
        public string MediaSubtype
#else
        public string? MediaSubtype
#endif
        {
            get
            {
                if (!this.Initialized)
                    return null;

                // Media subtype is only valid for XGD2/3
                if (XeMID == null)
                    return null;

                char mediaSubtype = XeMID.MediaSubtypeIdentifier;
                if (MediaSubtypes.ContainsKey(mediaSubtype))
                    return MediaSubtypes[mediaSubtype];

                return $"Unknown ({mediaSubtype})";
            }
        }

        /// <summary>
        /// Get the human-readable publisher string
        /// </summary>
#if NET48
        public string Publisher
#else
        public string? Publisher
#endif
        {
            get
            {
                if (!this.Initialized)
                    return null;

#if NET48
                string publisherIdentifier = null;
#else
                string? publisherIdentifier = null;
#endif
                if (XMID != null)
                    publisherIdentifier = XMID.PublisherIdentifier;
                else if (XeMID != null)
                    publisherIdentifier = XeMID.PublisherIdentifier;

                if (string.IsNullOrWhiteSpace(publisherIdentifier))
                    return null;

                if (Publishers.ContainsKey(publisherIdentifier))
                    return Publishers[publisherIdentifier];

                return $"Unknown ({publisherIdentifier})";
            }
        }

        /// <summary>
        /// Get the human-readable region string
        /// </summary>
#if NET48
        public string Region
#else
        public string? Region
#endif
        {
            get
            {
                if (!this.Initialized)
                    return null;

                char? regionIdentifier = null;
                if (XMID != null)
                    regionIdentifier = XMID.RegionIdentifier;
                else if (XeMID != null)
                    regionIdentifier = XeMID.RegionIdentifier;

                if (regionIdentifier == null)
                    return null;

                if (Regions.ContainsKey((char)regionIdentifier))
                    return Regions[(char)regionIdentifier];

                return $"Unknown ({regionIdentifier})";
            }
        }

        /// <summary>
        /// Get the human-readable serial string
        /// </summary>
#if NET48
        public string Serial
#else
        public string? Serial
#endif
        {
            get
            {
                if (!this.Initialized)
                    return null;

                try
                {
                    // XGD1 doesn't use PlatformIdentifier
                    if (XMID != null)
                        return $"{XMID.PublisherIdentifier}-{XMID.GameID}";

                    // XGD2/3 uses a specific identifier
                    else if (XeMID?.PlatformIdentifier == '2')
                        return $"{XeMID.PublisherIdentifier}-{XeMID.PlatformIdentifier}{XeMID.GameID}";

                    return null;
                }
                catch
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Get the human-readable version string
        /// </summary>
#if NET48
        public string Version
#else
        public string? Version
#endif
        {
            get
            {
                if (!this.Initialized)
                    return null;

                try
                {
                    if (XMID != null)
                        return $"1.{XMID.VersionNumber}";
                    else if (XeMID?.PlatformIdentifier == '2')
                        return $"1.{XeMID.SKU}";

                    return null;
                }
                catch
                {
                    return null;
                }
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Populate a set of XGD information from a Master ID (XMID/XeMID) string
        /// </summary>
        /// <param name="xmid">XMID/XeMID string representing the DMI information</param>
        public XgdInfo(string xmid)
        {
            this.Initialized = false;
            if (string.IsNullOrWhiteSpace(xmid))
                return;

            this.RawXMID = xmid.TrimEnd('\0');
            if (string.IsNullOrWhiteSpace(this.RawXMID))
                return;

            // XGD1 information is 8 characters
            if (this.RawXMID.Length == 8)
                this.Initialized = ParseXGD1XMID(this.RawXMID);

            // XGD2/3 information is semi-variable length
            else if (this.RawXMID.Length == 13 || this.RawXMID.Length == 14 || this.RawXMID.Length == 21 || this.RawXMID.Length == 22)
                this.Initialized = ParseXGD23XeMID(this.RawXMID);
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Parse an XGD1 XMID string
        /// </summary>
        /// <param name="rawXmid">XMID string to attempt to parse</param>
        /// <returns>True if the XMID could be parsed, false otherwise</returns>
        private bool ParseXGD1XMID(string rawXmid)
        {
            try
            {
                var xmid = new Strings.XMID().Deserialize(rawXmid);
                if (xmid == null)
                    return false;

                this.XMID = xmid;
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Parse an XGD2/3 XeMID string
        /// </summary>
        /// <param name="rawXemid">XeMID string to attempt to parse</param>
        /// <returns>True if the XeMID could be parsed, false otherwise</returns>
        private bool ParseXGD23XeMID(string rawXemid)
        {
            try
            {
                var xemid = new Strings.XeMID().Deserialize(rawXemid);
                if (xemid == null)
                    return false;

                this.XeMID = xemid;
                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion
    }
}

namespace SabreTools.Data.Models.OLE
{
    /// <summary>
    /// The PropertySetStream packet specifies the stream format for simple property
    /// sets and the stream format for the CONTENTS stream in the Non-Simple Property
    /// Set Storage Format. A simple property set MUST be represented by a stream
    /// containing a PropertySetStream packet.
    /// 
    /// The PropertySetStream packet usually represents exactly one property set, but
    /// for historical reasons, the DocumentSummaryInfo and UserDefinedProperties
    /// property sets are represented in the same stream.In this special case, a
    /// PropertySetStream might represent two property sets.
    /// 
    /// An implementation SHOULD enforce a limit on the total size of a PropertySetStream
    /// packet. This limit MUST be at least 262,144 bytes, and for maximum interoperability
    /// SHOULD be 2,097,152 bytes.
    /// </summary>
    /// <see href="https://winprotocoldoc.z19.web.core.windows.net/MS-OLEPS/%5bMS-OLEPS%5d.pdf"/> 
    public class PropertySetStream
    {
        /// <summary>
        /// MUST be set to 0xFFFE
        /// </summary>
        public ushort ByteOrder { get; set; }

        /// <summary>
        /// An unsigned integer indicating the version number of the property set (or
        /// property sets). MUST be 0x0000 or 0x0001. An OLEPS implementation MUST
        /// accept version 0 property sets and SHOULD<5> also accept version 1 property
        /// sets. This field MUST be set to 0x0001 if the property set or property sets
        /// use any of the following features not supported by version 0 property sets:
        /// - Property types not supported for version 0 property sets, as specified in
        ///   the PropertyType enumeration.
        /// - The Behavior property.
        /// 
        /// If the property set does not use any of these features, this field SHOULD be
        /// set to 0x0000 for maximum interoperability.
        /// </summary>
        public ushort Version { get; set; }

        /// <summary>
        /// An implementation-specific value that SHOULD be ignored, except possibly to
        /// report this value to applications. It SHOULD NOT be interpreted by the
        /// OLEPS implementation.
        /// </summary>
        public uint SystemIdentifier { get; set; }

        /// <summary>
        /// MUST be a GUID (Packet Version) packet representing the associated CLSID of
        /// the property set (or property sets). If no CLSID is provided by the
        /// application, it SHOULD be set to GUID_NULL by default.
        /// </summary>
        public GUID? CLSID { get; set; }

        /// <summary>
        /// An unsigned integer indicating the number of property sets represented by
        /// this PropertySetStream structure. MUST be either 0x00000001 or 0x00000002.
        /// 
        /// - 0x00000001: This structure contains one property set
        /// - 0x00000002: This structure contains two property sets.
        ///               The optional fields for PropertySet 1 are present.
        /// </summary>
        public uint NumPropertySets { get; set; }

        /// <summary>
        /// A GUID that MUST be set to the FMTID of the property set represented by the
        /// field PropertySet 0. If NumPropertySets has the value 0x00000002, then this
        /// GUID MUST be set to FMTID_DocSummaryInformation
        /// ({D5CDD502-2E9C-101B-9397-08002B2CF9AE}).
        /// </summary>
        public GUID? FMTID0 { get; set; }

        /// <summary>
        /// An unsigned integer that MUST be set to the offset in bytes from the beginning
        /// of this PropertySetStream structure to the beginning of the field PropertySet 0.
        /// </summary>
        public uint Offset0 { get; set; }

        /// <summary>
        /// If NumPropertySets has the value 0x00000002, it MUST be set to FMTID_UserDefinedProperties
        /// ({D5CDD505-2E9C-101B-9397-08002B2CF9AE}). Otherwise, it MUST be absent.
        /// </summary>
        public GUID? FMTID1 { get; set; }

        /// <summary>
        /// If NumPropertySets has the value 0x00000002, it MUST be set to the offset in bytes
        /// from the beginning of this PropertySetStream structure to the beginning of the
        /// field PropertySet 1. Otherwise, it MUST be absent.
        /// </summary>
        public uint? Offset1 { get; set; }

        /// <summary>
        /// MUST be a PropertySet packet
        /// </summary>
        public PropertySet? PropertySet0 { get; set; }

        /// <summary>
        /// If NumPropertySets has the value 0x00000002, it MUST be a PropertySet packet.
        /// Otherwise, it MUST be absent.
        /// </summary>
        public PropertySet? PropertySet1 { get; set; }

        // Padding (variable): Contains additional padding added by the implementation.
        // If present, padding MUST be zeroes and MUST be ignored.
    }
}

namespace SabreTools.Serialization.Models.OLE
{
    /// <summary>
    /// The PropertyIdentifier data type represents the property identifier of a property in a property set
    /// </summary>
    /// <remarks>
    /// The SummaryInformation property set format, identified by FMTID_SummaryInformation
    /// ({F29F85E0-4FF9-1068-AB91-08002B27B3D9}), represents generic properties of a document. The properties
    /// specific to the SummaryInformation property set are specified in the following table. Except where
    /// otherwise stated, a SummaryInformation property set SHOULD have all of these properties, and SHOULD
    /// NOT have any other properties, except for the special properties specified in section 2.18.
    /// </remarks>
    /// <see href="https://winprotocoldoc.z19.web.core.windows.net/MS-OLEPS/%5bMS-OLEPS%5d.pdf"/> 
    public enum PropertyIdentifier : uint
    {
        /// <summary>
        /// Property identifier for the Dictionary property
        /// </summary>
        DICTIONARY_PROPERTY_IDENTIFIER = 0x00000000,

        /// <summary>
        /// Property identifier for the CodePage property
        /// </summary>
        CODEPAGE_PROPERTY_IDENTIFIER = 0x00000001,

        /// <summary>
        /// Property identifier for the Locale property
        /// </summary>
        LOCALE_PROPERTY_IDENTIFIER = 0x80000000,

        /// <summary>
        /// Property identifier for the Behavior property
        /// </summary>
        BEHAVIOR_PROPERTY_IDENTIFIER = 0x80000003,

        #region SummaryInformation

        /// <summary>
        /// The title of the document.
        /// </summary>
        /// <remarks>VT_LPSTR</remarks>
        PIDSI_TITLE = 0x00000002,

        /// <summary>
        /// The subject of the document.
        /// </summary>
        /// <remarks>VT_LPSTR</remarks>
        PIDSI_SUBJECT = 0x00000003,

        /// <summary>
        /// The author of the document.
        /// </summary>
        /// <remarks>VT_LPSTR</remarks>
        PIDSI_AUTHOR = 0x00000004,

        /// <summary>
        /// Keywords related to the document.
        /// </summary>
        /// <remarks>VT_LPSTR</remarks>
        PIDSI_KEYWORDS = 0x00000005,

        /// <summary>
        /// Comments related the document.
        /// </summary>
        /// <remarks>VT_LPSTR</remarks>
        PIDSI_COMMENTS = 0x00000006,

        /// <summary>
        /// The application-specific template from which the document was created.
        /// </summary>
        /// <remarks>VT_LPSTR</remarks>
        PIDSI_TEMPLATE = 0x00000007,

        /// <summary>
        /// The last author of the document.
        /// </summary>
        /// <remarks>VT_LPSTR</remarks>
        PIDSI_LASTAUTHOR = 0x00000008,

        /// <summary>
        /// An application-specific revision number for this version of the document.
        /// </summary>
        /// <remarks>VT_LPSTR</remarks>
        PIDSI_REVNUMBER = 0x00000009,

        /// <summary>
        /// A 64-bit unsigned integer indicating the total amount of time that
        /// has been spent editing the document in 100-nanosecond
        /// increments. MUST be encoded as a FILETIME by setting the
        /// dwLowDataTime field to the low 32-bits and the dwHighDateTime
        /// field to the high 32-bits.
        /// </summary>
        /// <remarks>VT_FILETIME</remarks>
        PIDSI_EDITTIME = 0x0000000A,

        /// <summary>
        /// The most recent time that the document was printed.
        /// </summary>
        /// <remarks>VT_FILETIME</remarks>
        PIDSI_LASTPRINTED = 0x0000000B,

        /// <summary>
        /// The time that the document was created.
        /// </summary>
        /// <remarks>VT_FILETIME</remarks>
        PIDSI_CREATE_DTM = 0x0000000C,

        /// <summary>
        /// The most recent time that the document was saved.
        /// </summary>
        /// <remarks>VT_FILETIME</remarks>
        PIDSI_LASTSAVE_DTM = 0x0000000D,

        /// <summary>
        /// The total number of pages in the document.
        /// </summary>
        /// <remarks>VT_I4</remarks>
        PIDSI_PAGECOUNT = 0x0000000E,

        /// <summary>
        /// The total number of words in the document.
        /// </summary>
        /// <remarks>VT_I4</remarks>
        PIDSI_WORDCOUNT = 0x0000000F,

        /// <summary>
        /// The total number of characters in the document.
        /// </summary>
        /// <remarks>VT_I4</remarks>
        PIDSI_CHARCOUNT = 0x00000010,

        /// <summary>
        /// Application-specific clipboard data containing a thumbnail
        /// representing the document's contents. MAY be absent.
        /// </summary>
        /// <remarks>VT_CF</remarks>
        PIDSI_THUMBNAIL = 0x00000011,

        /// <summary>
        /// The name of the application that was used to create the document.
        /// </summary>
        /// <remarks>VT_LPSTR</remarks>
        PIDSI_APPNAME = 0x00000012,

        /// <summary>
        /// A 32-bit signed integer representing a set of application-suggested
        /// access control flags with the following values:
        /// - 0x00000001: Password protected
        /// - 0x00000002: Read-only recommended
        /// - 0x00000004: Read-only enforced
        /// - 0x00000008: Locked for annotations
        /// </summary>
        /// <remarks>VT_I4</remarks>
        PIDSI_DOC_SECURITY = 0x00000013,

        #endregion
    }

    /// <summary>
    /// The PropertyType enumeration represents the type of a property in a property set. The set of types
    /// supported depends on the version of the property set, which is indicated by the Version field of the
    /// PropertySetStream packet. In addition, the property types not supported in simple property sets
    /// are specified as such. PropertyType is an enumeration, which MUST be one of the following values
    /// </summary>
    /// <see href="https://winprotocoldoc.z19.web.core.windows.net/MS-OLEPS/%5bMS-OLEPS%5d.pdf"/> 
    public enum PropertyType : ushort
    {
        /// <summary>
        /// Type is undefined, and the minimum property set version is 0
        /// </summary>
        VT_EMPTY = 0x0000,

        /// <summary>
        /// Type is null, and the minimum property set version is 0
        /// </summary>
        VT_NULL = 0x0001,

        /// <summary>
        /// Type is 16-bit signed integer, and the minimum property set version is 0
        /// </summary>
        VT_I2 = 0x0002,

        /// <summary>
        /// Type is 32-bit signed integer, and the minimum property set version is 0.
        /// </summary>
        VT_I4 = 0x0003,

        /// <summary>
        /// Type is 4-byte (single-precision) IEEE floating-point number, and the
        /// minimum property set version is 0.
        /// </summary>
        VT_R4 = 0x0004,

        /// <summary>
        /// Type is 8-byte (double-precision) IEEE floating-point number, and the
        /// minimum property set version is 0.
        /// </summary>
        VT_R8 = 0x0005,

        /// <summary>
        /// Type is CURRENCY, and the minimum property set version is 0.
        /// </summary>
        VT_CY = 0x0006,

        /// <summary>
        /// Type is DATE, and the minimum property set version is 0.
        /// </summary>
        VT_DATE = 0x0007,

        /// <summary>
        /// Type is CodePageString, and the minimum property set version is 0.
        /// </summary>
        VT_BSTR = 0x0008,

        /// <summary>
        /// Type is HRESULT, and the minimum property set version is 0.
        /// </summary>
        VT_ERROR = 0x000A,

        /// <summary>
        /// Type is VARIANT_BOOL, and the minimum property set version is 0.
        /// </summary>
        VT_BOOL = 0x000B,

        /// <summary>
        /// Type is DECIMAL, and the minimum property set version is 0.
        /// </summary>
        VT_DECIMAL = 0x000E,

        /// <summary>
        /// Type is 1-byte signed integer, and the minimum property set version
        /// is 1.
        /// </summary>
        VT_I1 = 0x0010,

        /// <summary>
        /// Type is 1-byte unsigned integer, and the minimum property set version
        /// is 0.
        /// </summary>
        VT_UI1 = 0x0011,

        /// <summary>
        /// Type is 2-byte unsigned integer, and the minimum property set version
        /// is 0.
        /// </summary>
        VT_UI2 = 0x0012,

        /// <summary>
        /// Type is 4-byte unsigned integer, and the minimum property set version
        /// is 0.
        /// </summary>
        VT_UI4 = 0x0013,

        /// <summary>
        ///  Type is 8-byte signed integer, and the minimum property set version
        /// is 0.
        /// </summary>
        VT_I8 = 0x0014,

        /// <summary>
        /// Type is 8-byte unsigned integer, and the minimum property set version
        /// is 0.
        /// </summary>
        VT_UI8 = 0x0015,

        /// <summary>
        /// Type is 4-byte signed integer, and the minimum property set version
        /// is 1.
        /// </summary>
        VT_INT = 0x0016,

        /// <summary>
        /// Type is 4-byte unsigned integer, and the minimum property set version
        /// is 1.
        /// </summary>
        VT_UINT = 0x0017,

        /// <summary>
        /// Type is CodePageString, and the minimum property set version is 0.
        /// </summary>
        VT_LPSTR = 0x001E,

        /// <summary>
        /// Type is UnicodeString, and the minimum property set version is 0.
        /// </summary>
        VT_LPWSTR = 0x001F,

        /// <summary>
        /// Type is FILETIME, and the minimum property set version is 0.
        /// </summary>
        VT_FILETIME = 0x0040,

        /// <summary>
        /// Type is binary large object (BLOB), and the minimum property set
        /// version is 0.
        /// </summary>
        VT_BLOB = 0x0041,

        /// <summary>
        /// Type is Stream, and the minimum property set version is 0. VT_STREAM
        /// is not allowed in a simple property set.
        /// </summary>
        VT_STREAM = 0x0042,

        /// <summary>
        /// Type is Storage, and the minimum property set version is 0. VT_STORAGE
        /// is not allowed in a simple property set.
        /// </summary>
        VT_STORAGE = 0x0043,

        /// <summary>
        /// Type is Stream representing an Object in an application-specific manner,
        /// and the minimum property set version is 0. VT_STREAMED_Object is not
        /// allowed in a simple property set.
        /// </summary>
        VT_STREAMED_Object = 0x0044,

        /// <summary>
        /// Type is Storage representing an Object in an application-specific manner,
        /// and the minimum property set version is 0. VT_STORED_Object is not
        /// allowed in a simple property set.
        /// </summary>
        VT_STORED_Object = 0x0045,

        /// <summary>
        /// Type is BLOB representing an object in an application-specific manner.
        /// The minimum property set version is 0.
        /// </summary>
        VT_BLOB_Object = 0x0046,

        /// <summary>
        /// Type is PropertyIdentifier, and the minimum property set version is 0.
        /// </summary>
        VT_CF = 0x0047,

        /// <summary>
        /// Type is CLSID, and the minimum property set version is 0.
        /// </summary>
        VT_CLSID = 0x0048,

        /// <summary>
        /// Type is Stream with application-specific version GUID (VersionedStream).
        /// The minimum property set version is 0. VT_VERSIONED_STREAM is not allowed
        /// in a simple property set.
        /// </summary>
        VT_VERSIONED_STREAM = 0x0049,

        /// <summary>
        /// Type is Vector, and the minimum property set version is 0
        /// </summary>
        VT_VECTOR = 0x1000,

        /// <summary>
        /// Type is Vector of 1-byte unsigned integers, and the minimum property set version
        /// is 0.
        /// </summary>
        VT_VECTOR_UI1 = 0x1011,

        /// <summary>
        /// Type is Vector of 2-byte unsigned integers, and the minimum property set version
        /// is 0.
        /// </summary>
        VT_VECTOR_UI2 = 0x1012,

        /// <summary>
        /// Type is Vector of 4-byte unsigned integers, and the minimum property set version
        /// is 0.
        /// </summary>
        VT_VECTOR_UI4 = 0x1013,

        /// <summary>
        /// Type is Vector of 8-byte signed integers, and the minimum property set version
        /// is 0.
        /// </summary>
        VT_VECTOR_I8 = 0x1014,

        /// <summary>
        /// Type is Vector of 8-byte unsigned integers and the minimum property set version
        /// is 0.
        /// </summary>
        VT_VECTOR_UI8 = 0x1015,

        /// <summary>
        /// Type is Vector of CodePageString, and the minimum property set version is 0.
        /// </summary>
        VT_VECTOR_LPSTR = 0x101E,

        /// <summary>
        /// Type is Vector of UnicodeString, and the minimum property set version is 0.
        /// </summary>
        VT_VECTOR_LPWSTR = 0x101F,

        /// <summary>
        /// Type is Vector of FILETIME, and the minimum property set version is 0.
        /// </summary>
        VT_VECTOR_FILETIME = 0x1040,

        /// <summary>
        /// Type is Vector of PropertyIdentifier, and the minimum property set version is 0.
        /// </summary>
        VT_VECTOR_CF = 0x1047,

        /// <summary>
        /// Type is Vector of CLSID, and the minimum property set version is 0.
        /// </summary>
        VT_VECTOR_CLSID = 0x1048,

        /// <summary>
        /// Type is Array of 16-bit signed integers, and the minimum property set version
        /// is 1.
        /// </summary>
        VT_ARRAY_I2 = 0x2002,

        /// <summary>
        /// Type is Array of 32-bit signed integers, and the minimum property set version
        /// is 1.
        /// </summary>
        VT_ARRAY_I4 = 0x2003,

        /// <summary>
        /// Type is Array of 4-byte (single-precision) IEEE floating-point numbers, and the
        /// minimum property set version is 1.
        /// </summary>
        VT_ARRAY_R4 = 0x2004,

        /// <summary>
        /// Type is IEEE floating-point numbers, and the minimum property set version is 1.
        /// </summary>
        VT_ARRAY_R8 = 0x2005,

        /// <summary>
        /// Type is Array of CURRENCY, and the minimum property set version is 1.
        /// </summary>
        VT_ARRAY_CY = 0x2006,

        /// <summary>
        /// Type is Array of DATE, and the minimum property set version is 1.
        /// </summary>
        VT_ARRAY_DATE = 0x2007,

        /// <summary>
        /// Type is Array of CodePageString, and the minimum property set version is 1.
        /// </summary>
        VT_ARRAY_BSTR = 0x2008,

        /// <summary>
        /// Type is Array of HRESULT, and the minimum property set version is 1.
        /// </summary>
        VT_ARRAY_ERROR = 0x200A,

        /// <summary>
        /// Type is Array of VARIANT_BOOL, and the minimum property set version is 1.
        /// </summary>
        VT_ARRAY_BOOL = 0x200B,

        /// <summary>
        /// Type is Array of variable-typed properties, and the minimum property set
        /// version is 1
        /// </summary>
        VT_ARRAY_VARIANT = 0x200C,

        /// <summary>
        /// Type is Array of DECIMAL, and the minimum property set version is 1
        /// </summary>
        VT_ARRAY_DECIMAL = 0x200E,

        /// <summary>
        /// Type is Array of 1-byte signed integers, and the minimum property set
        /// version is 1
        /// </summary>
        VT_ARRAY_I1 = 0x2010,

        /// <summary>
        /// Type is Array of 1-byte unsigned integers, and the minimum property set
        /// version is 1
        /// </summary>
        VT_ARRAY_UI1 = 0x2011,

        /// <summary>
        /// Type is Array of 2-byte unsigned integers, and the minimum property set
        /// version is 1.
        /// </summary>
        VT_ARRAY_UI2 = 0x2012,

        /// <summary>
        /// Type is Array of 4-byte unsigned integers, and the minimum property set
        /// version is 1.
        /// </summary>
        VT_ARRAY_UI4 = 0x2013,

        /// <summary>
        /// Type is Array of 4-byte signed integers, and the minimum property set version
        /// is 1.
        /// </summary>
        VT_ARRAY_INT = 0x2016,

        /// <summary>
        /// Type is Array of 4-byte unsigned integers, and the minimum property set version
        /// is 1.
        /// </summary>
        VT_ARRAY_UINT = 0x2017,
    }
}

using System.IO;
using System.Text;
using SabreTools.IO;
using SabreTools.Models.BDPlus;
using static SabreTools.Models.BDPlus.Constants;

namespace SabreTools.Serialization.Streams
{
    public partial class BDPlus : IStreamSerializer<SVM>
    {
        /// <inheritdoc/>
#if NET48
        public SVM Deserialize(Stream data)
#else
        public SVM? Deserialize(Stream? data)
#endif
        {
            // If the data is invalid
            if (data == null || data.Length == 0 || !data.CanSeek || !data.CanRead)
                return null;

            // If the offset is out of bounds
            if (data.Position < 0 || data.Position >= data.Length)
                return null;

            // Cache the current offset
            int initialOffset = (int)data.Position;

            // Try to parse the SVM
            return ParseSVMData(data);
        }

        /// <summary>
        /// Parse a Stream into an SVM
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled SVM on success, null on error</returns>
#if NET48
        private static SVM ParseSVMData(Stream data)
#else
        private static SVM? ParseSVMData(Stream data)
#endif
        {
            // TODO: Use marshalling here instead of building
            var svm = new SVM();

#if NET48
            byte[] signature = data.ReadBytes(8);
#else
            byte[]? signature = data.ReadBytes(8);
#endif
            if (signature == null)
                return null;

            svm.Signature = Encoding.ASCII.GetString(signature);
            if (svm.Signature != SignatureString)
                return null;

            svm.Unknown1 = data.ReadBytes(5);
            svm.Year = data.ReadUInt16BigEndian();
            svm.Month = data.ReadByteValue();
            if (svm.Month < 1 || svm.Month > 12)
                return null;

            svm.Day = data.ReadByteValue();
            if (svm.Day < 1 || svm.Day > 31)
                return null;

            svm.Unknown2 = data.ReadBytes(4);
            svm.Length = data.ReadUInt32();
            // if (svm.Length > 0)
            //     svm.Data = data.ReadBytes((int)svm.Length);

            return svm;
        }
    }
}
using System.IO;
using SabreTools.Numerics.Extensions;

namespace SabreTools.Serialization.Writers
{
    public class XRD : BaseBinaryWriter<Data.Models.XRD.File>
    {
        /// <inheritdoc/>
        public override Stream? SerializeStream(Data.Models.XRD.File? obj)
        {
            #region Validation

            // If the data is invalid
            if (obj?.Magic is null)
                return null;

            // If the magic doesn't match
            if (obj.Magic != Data.Models.XRD.Constants.MagicBytes)
                return null;

            // If the version is not supported
            if (obj.Version == 0 || obj.Version > 2)
                return null;

            // If the version specific fields are not set/unset
            if (obj.Version == 1 && (obj.WipedVideoISOSize is not null || obj.WipedVideoISOCRC is not null || obj.WipedVideoISOMD5 is not null || obj.WipedVideoISOSHA1 is not null))
                return null;
            if (obj.Version == 2 && (obj.WipedVideoISOSize is null || obj.WipedVideoISOCRC is null || obj.WipedVideoISOMD5 is null || obj.WipedVideoISOSHA1 is null))
                return null;
            if (obj.XGDType == 1 && obj.XboxCertificate is null)
                return null;
            if (obj.XGDType != 1 && obj.XboxCertificate is not null)
                return null;
            if (obj.Version == 1 && (obj.VideoISOFileCount is not null || obj.VideoISOFileInfo is not null))
                return null;
            if (obj.Version == 2 && (obj.VideoISOFileCount is null || obj.VideoISOFileInfo is null))
                return null;

            // If the XGD Type specific fields are not set/unset
            if ((obj.XGDType == 1 || obj.XGDType == 2 || obj.XGDType == 3) && obj.SecuritySectors is null)
                return null;
            if (obj.XGDType != 1 && obj.XGDType != 2 && obj.XGDType != 3 && obj.SecuritySectors is not null)
                return null;
            if ((obj.XGDType == 2 || obj.XGDType == 3) && obj.Xbox360Certificate is null)
                return null;
            if (obj.XGDType != 2 && obj.XGDType != 3 && obj.Xbox360Certificate is not null)
                return null;

            // If any static-length fields aren't the correct length
            if (obj.Ringcode.Length != 8)
                return null;
            if (obj.RedumpCRC.Length != 4)
                return null;
            if (obj.RedumpMD5.Length != 16)
                return null;
            if (obj.RedumpSHA1.Length != 20)
                return null;
            if (obj.RawXISOCRC.Length != 4)
                return null;
            if (obj.RawXISOMD5.Length != 16)
                return null;
            if (obj.RawXISOSHA1.Length != 20)
                return null;
            if (obj.CookedXISOCRC.Length != 4)
                return null;
            if (obj.CookedXISOMD5.Length != 16)
                return null;
            if (obj.CookedXISOSHA1.Length != 20)
                return null;
            if (obj.VideoISOCRC.Length != 4)
                return null;
            if (obj.VideoISOMD5.Length != 16)
                return null;
            if (obj.VideoISOSHA1.Length != 20)
                return null;
            if (obj.WipedVideoISOCRC is not null && obj.WipedVideoISOCRC.Length != 4)
                return null;
            if (obj.WipedVideoISOMD5 is not null && obj.WipedVideoISOMD5.Length != 16)
                return null;
            if (obj.WipedVideoISOSHA1 is not null && obj.WipedVideoISOSHA1.Length != 20)
                return null;
            if (obj.FillerCRC.Length != 4)
                return null;
            if (obj.FillerMD5.Length != 16)
                return null;
            if (obj.FillerSHA1.Length != 20)
                return null;
            if (obj.XGDType == 1 && obj.SecuritySectors?.Length != 16)
                return null;
            if (obj.XGDType == 2 && obj.SecuritySectors?.Length != 2)
                return null;
            if (obj.XGDType == 3 && obj.SecuritySectors?.Length != 2)
                return null;
            if (obj.XboxCertificate is not null)
            {
                if (obj.XboxCertificate.SizeOfCertificate + 16 != 492)
                    return null;
                if (obj.XboxCertificate.TitleName.Length != 0x50)
                    return null;
                if (obj.XboxCertificate.AlternativeTitleIDs.Length != 16)
                    return null;
                if (obj.XboxCertificate.LANKey.Length != 16)
                    return null;
                if (obj.XboxCertificate.SignatureKey.Length != 16)
                    return null;
                if (obj.XboxCertificate.AlternateSignatureKeys.Length != 16)
                    return null;
                for (int i = 0; i < obj.XboxCertificate.AlternateSignatureKeys.Length; i++)
                {
                    if (obj.XboxCertificate.AlternateSignatureKeys[i].Length != 16)
                        return null;
                }

                if (obj.XboxCertificate.CodeEncKey.Length != 16)
                    return null;
            }

            if (obj.Xbox360Certificate is not null)
            {
                var certificateLength = 388 + (24 * obj.Xbox360Certificate.Table.Length);
                if (obj.Xbox360Certificate.Length != certificateLength)
                    return null;
                if (obj.Xbox360Certificate.Signature.Length != 256)
                    return null;
                if (obj.Xbox360Certificate.UnknownHash1.Length != 20)
                    return null;
                if (obj.Xbox360Certificate.UnknownHash2.Length != 20)
                    return null;
                if (obj.Xbox360Certificate.MediaID.Length != 16)
                    return null;
                if (obj.Xbox360Certificate.XEXFileKey.Length != 16)
                    return null;
                if (obj.Xbox360Certificate.UnknownHash3.Length != 20)
                    return null;
                for (int i = 0; i < obj.Xbox360Certificate.Table.Length; i++)
                {
                    if (obj.Xbox360Certificate.Table[i].Data.Length != 20)
                        return null;
                }
            }

            if (obj.FileCount != obj.FileInfo.Length)
                return null;
            for (int i = 0; i < obj.FileInfo.Length; i++)
            {
                if (obj.FileInfo[i].SHA1.Length != 20)
                    return null;
            }

            if (obj.DirectoryCount != obj.DirectoryInfo.Length)
                return null;
            if (obj.VideoISOFileCount is not null && obj.VideoISOFileInfo is not null && obj.VideoISOFileCount != obj.VideoISOFileInfo.Length)
                return null;
            if (obj.XRDSHA1.Length != 20)
                return null;

            #endregion

            // Create the output stream
            var stream = new MemoryStream();

            stream.Write(obj.Magic);
            stream.Write(obj.Version);
            stream.Write(obj.XGDType);
            stream.Write(obj.XGDSubtype);
            stream.Write(obj.Ringcode);

            stream.WriteLittleEndian(obj.RedumpSize);
            stream.Write(obj.RedumpCRC);
            stream.Write(obj.RedumpMD5);
            stream.Write(obj.RedumpSHA1);

            stream.WriteLittleEndian(obj.RawXISOSize);
            stream.Write(obj.RawXISOCRC);
            stream.Write(obj.RawXISOMD5);
            stream.Write(obj.RawXISOSHA1);

            stream.WriteLittleEndian(obj.CookedXISOSize);
            stream.Write(obj.CookedXISOCRC);
            stream.Write(obj.CookedXISOMD5);
            stream.Write(obj.CookedXISOSHA1);

            stream.WriteLittleEndian(obj.VideoISOSize);
            stream.Write(obj.VideoISOCRC);
            stream.Write(obj.VideoISOMD5);
            stream.Write(obj.VideoISOSHA1);

            if (obj.WipedVideoISOSize is not null)
                stream.WriteLittleEndian(obj.WipedVideoISOSize.Value);
            if (obj.WipedVideoISOCRC is not null)
                stream.Write(obj.WipedVideoISOCRC);
            if (obj.WipedVideoISOMD5 is not null)
                stream.Write(obj.WipedVideoISOMD5);
            if (obj.WipedVideoISOSHA1 is not null)
                stream.Write(obj.WipedVideoISOSHA1);

            stream.WriteLittleEndian(obj.FillerSize);
            stream.Write(obj.FillerCRC);
            stream.Write(obj.FillerMD5);
            stream.Write(obj.FillerSHA1);

            if (obj.SecuritySectors is not null)
            {
                for (int i = 0; i < obj.SecuritySectors.Length; i++)
                {
                    stream.Write(obj.SecuritySectors[i]);
                }
            }

            if (obj.XboxCertificate is not null)
                SerializeXboxCertificate(stream, obj.XboxCertificate);
            if (obj.Xbox360Certificate is not null)
                SerializeXbox360Certificate(stream, obj.Xbox360Certificate);

            stream.WriteLittleEndian(obj.FileCount);
            for (int i = 0; i < obj.FileInfo.Length; i++)
            {
                stream.WriteLittleEndian(obj.FileInfo[i].Offset);
                stream.WriteLittleEndian(obj.FileInfo[i].Size);
                stream.Write(obj.FileInfo[i].SHA1);
            }

            XDVDFS.SerializeVolumeDescriptor(stream, obj.VolumeDescriptor);
            if (obj.LayoutDescriptor is not null)
                XDVDFS.SerializeLayoutDescriptor(stream, obj.LayoutDescriptor);

            stream.WriteLittleEndian(obj.DirectoryCount);
            for (int i = 0; i < obj.DirectoryInfo.Length; i++)
            {
                stream.Write(obj.DirectoryInfo[i].Offset);
                stream.Write(obj.DirectoryInfo[i].Size);
                XDVDFS.SerializeDirectoryDescriptor(stream, obj.DirectoryInfo[i].DirectoryDescriptor);
            }

            return stream;
        }

        public static void SerializeXboxCertificate(Stream stream, Data.Models.XboxExecutable.Certificate obj)
        {
            stream.WriteLittleEndian(obj.SizeOfCertificate);
            stream.WriteLittleEndian(obj.TimeDate);
            stream.WriteLittleEndian(obj.TitleID);
            stream.Write(obj.TitleName);
            for (int i = 0; i < obj.AlternativeTitleIDs.Length; i++)
            {
                stream.WriteLittleEndian(obj.AlternativeTitleIDs[i]);
            }

            stream.WriteLittleEndian((uint)obj.AllowedMediaTypes);
            stream.WriteLittleEndian((uint)obj.GameRegion);
            stream.WriteLittleEndian(obj.GameRatings);
            stream.WriteLittleEndian(obj.DiskNumber);
            stream.WriteLittleEndian(obj.Version);
            stream.Write(obj.LANKey);
            stream.Write(obj.SignatureKey);
            for (int i = 0; i < obj.AlternateSignatureKeys.Length; i++)
            {
                stream.Write(obj.AlternateSignatureKeys[i]);
            }

            stream.WriteLittleEndian(obj.OriginalCertificateSize);
            stream.WriteLittleEndian(obj.OnlineService);
            stream.WriteLittleEndian(obj.SecurityFlags);
            stream.Write(obj.CodeEncKey);
        }

        public static void SerializeXbox360Certificate(Stream stream, Data.Models.XenonExecutable.Certificate obj)
        {
            stream.WriteLittleEndian(obj.Length);
            stream.WriteLittleEndian(obj.ImageSize);
            stream.Write(obj.Signature);
            stream.WriteLittleEndian(obj.BaseFileLoadAddress);
            stream.WriteLittleEndian(obj.ImageFlags);
            stream.WriteLittleEndian(obj.ImageBaseAddress);
            stream.Write(obj.UnknownHash1);
            stream.WriteLittleEndian(obj.Unknown0128);
            stream.Write(obj.UnknownHash2);
            stream.Write(obj.MediaID);
            stream.Write(obj.XEXFileKey);
            stream.WriteLittleEndian(obj.Unknown0160);
            stream.Write(obj.UnknownHash3);
            stream.WriteLittleEndian(obj.RegionFlags);
            stream.WriteLittleEndian(obj.AllowedMediaTypeFlags);
            stream.WriteLittleEndian(obj.TableCount);

            for (int i = 0; i < obj.Table.Length; i++)
            {
                stream.WriteLittleEndian(obj.Table[i].ID);
                stream.Write(obj.Table[i].Data);
            }
        }
    }
}

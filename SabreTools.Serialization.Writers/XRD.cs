using System;
using System.IO;
using System.Text;

namespace SabreTools.Serialization.Writers
{
    public class XRD : BaseBinaryWriter<Data.Models.XRD.File>
    {
        /// <inheritdoc/>
        public override Stream? SerializeStream(Data.Models.XRD.File? obj)
        {
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
            if ((obj.XGDType != 1 && obj.XGDType != 2 && obj.XGDType != 3) && obj.SecuritySectors is not null)
                return null;
            if ((obj.XGDType == 2 || obj.XGDType == 3) && obj.Xbox360Certificate is null)
                return null;
            if ((obj.XGDType != 2 && obj.XGDType != 3) && obj.Xbox360Certificate is not null)
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
                var certificateLength = 388 + 24 * obj.Xbox360Certificate.Table.Length;
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

            // Create the output stream
            var stream = new MemoryStream();

            stream.Write(obj.Magic, 0, obj.Magic.Length);
            stream.WriteByte(obj.Version);
            stream.WriteByte(obj.XGDType);
            stream.WriteByte(obj.XGDSubtype);
            stream.Write(obj.Ringcode, 0, obj.Ringcode.Length);

            byte[] redumpSize = BitConverter.GetBytes(obj.RedumpSize);
            stream.Write(redumpSize, 0, redumpSize.Length);
            stream.Write(obj.RedumpCRC, 0, obj.RedumpCRC.Length);
            stream.Write(obj.RedumpMD5, 0, obj.RedumpMD5.Length);
            stream.Write(obj.RedumpSHA1, 0, obj.RedumpSHA1.Length);

            byte[] rawXISOSize = BitConverter.GetBytes(obj.RawXISOSize);
            stream.Write(rawXISOSize, 0, rawXISOSize.Length);
            stream.Write(obj.RawXISOCRC, 0, obj.RawXISOCRC.Length);
            stream.Write(obj.RawXISOMD5, 0, obj.RawXISOMD5.Length);
            stream.Write(obj.RawXISOSHA1, 0, obj.RawXISOSHA1.Length);

            byte[] cookedXISOSize = BitConverter.GetBytes(obj.CookedXISOSize);
            stream.Write(cookedXISOSize, 0, cookedXISOSize.Length);
            stream.Write(obj.CookedXISOCRC, 0, obj.CookedXISOCRC.Length);
            stream.Write(obj.CookedXISOMD5, 0, obj.CookedXISOMD5.Length);
            stream.Write(obj.CookedXISOSHA1, 0, obj.CookedXISOSHA1.Length);

            byte[] videoISOSize = BitConverter.GetBytes(obj.VideoISOSize);
            stream.Write(videoISOSize, 0, videoISOSize.Length);
            stream.Write(obj.VideoISOCRC, 0, obj.VideoISOCRC.Length);
            stream.Write(obj.VideoISOMD5, 0, obj.VideoISOMD5.Length);
            stream.Write(obj.VideoISOSHA1, 0, obj.VideoISOSHA1.Length);

            if (obj.WipedVideoISOSize is not null)
            {
                byte[] wipedVideoISOSize = BitConverter.GetBytes(obj.WipedVideoISOSize ?? 0);
                stream.Write(videoISOSize, 0, videoISOSize.Length);
            }
            if (obj.WipedVideoISOCRC is not null)
                stream.Write(obj.WipedVideoISOCRC, 0, obj.WipedVideoISOCRC.Length);
            if (obj.WipedVideoISOMD5 is not null)
                stream.Write(obj.WipedVideoISOMD5, 0, obj.WipedVideoISOMD5.Length);
            if (obj.WipedVideoISOSHA1 is not null)
                stream.Write(obj.WipedVideoISOSHA1, 0, obj.WipedVideoISOSHA1.Length);

            byte[] fillerSize = BitConverter.GetBytes(obj.FillerSize);
            stream.Write(fillerSize, 0, fillerSize.Length);
            stream.Write(obj.FillerCRC, 0, obj.FillerCRC.Length);
            stream.Write(obj.FillerMD5, 0, obj.FillerMD5.Length);
            stream.Write(obj.FillerSHA1, 0, obj.FillerSHA1.Length);

            if (obj.SecuritySectors is not null)
            {
                for (int i = 0; i < obj.SecuritySectors.Length; i++)
                {
                    byte[] securitySector = BitConverter.GetBytes(obj.SecuritySectors[i]);
                    stream.Write(securitySector, 0, securitySector.Length);
                }
            }

            if (obj.XboxCertificate is not null)
                SerializeXboxCertificate(stream, obj.XboxCertificate);
            if (obj.Xbox360Certificate is not null)
                SerializeXbox360Certificate(stream, obj.Xbox360Certificate);

            byte[] fileCount = BitConverter.GetBytes(obj.FileCount);
            stream.Write(fileCount, 0, fileCount.Length);
            for (int i = 0; i < obj.FileInfo.Length; i++)
            {
                byte[] offset = BitConverter.GetBytes(obj.FileInfo[i].Offset);
                stream.Write(offset, 0, offset.Length);
                byte[] size = BitConverter.GetBytes(obj.FileInfo[i].Size);
                stream.Write(size, 0, size.Length);
                stream.Write(obj.FileInfo[i].SHA1, 0, obj.FileInfo[i].SHA1.Length);
            }

            XDVDFS.SerializeVolumeDescriptor(stream, obj.VolumeDescriptor);
            if (obj.LayoutDescriptor is not null)
                XDVDFS.SerializeLayoutDescriptor(stream, obj.LayoutDescriptor);

            byte[] directoryCount = BitConverter.GetBytes(obj.DirectoryCount);
            stream.Write(directoryCount, 0, directoryCount.Length);
            for (int i = 0; i < obj.DirectoryInfo.Length; i++)
            {
                byte[] offset = BitConverter.GetBytes(obj.DirectoryInfo[i].Offset);
                stream.Write(offset, 0, offset.Length);
                byte[] size = BitConverter.GetBytes(obj.DirectoryInfo[i].Size);
                stream.Write(size, 0, size.Length);
                XDVDFS.SerializeDirectoryDescriptor(stream, obj.DirectoryInfo[i].DirectoryDescriptor);
            }

            return stream;
        }

        public static void SerializeXboxCertificate(Stream stream, Data.Models.XboxExecutable.Certificate obj)
        {
            byte[] sizeOfCertificate = BitConverter.GetBytes(obj.SizeOfCertificate);
            stream.Write(sizeOfCertificate, 0, sizeOfCertificate.Length);
            byte[] timeDate = BitConverter.GetBytes(obj.TimeDate);
            stream.Write(timeDate, 0, timeDate.Length);
            byte[] titleID = BitConverter.GetBytes(obj.TitleID);
            stream.Write(titleID, 0, titleID.Length);
            stream.Write(obj.TitleName, 0, obj.TitleName.Length);
            for (int i = 0; i < obj.AlternativeTitleIDs.Length; i++)
            {
                byte[] alternativeTitleID = BitConverter.GetBytes(obj.AlternativeTitleIDs[i]);
                stream.Write(alternativeTitleID, 0, alternativeTitleID.Length);
            }
            byte[] allowedMediaTypes = BitConverter.GetBytes((uint)obj.AllowedMediaTypes);
            stream.Write(allowedMediaTypes, 0, allowedMediaTypes.Length);
            byte[] gameRegion = BitConverter.GetBytes((uint)obj.GameRegion);
            stream.Write(gameRegion, 0, gameRegion.Length);
            byte[] gameRatings = BitConverter.GetBytes(obj.GameRatings);
            stream.Write(gameRatings, 0, gameRatings.Length);
            byte[] diskNumber = BitConverter.GetBytes(obj.DiskNumber);
            stream.Write(diskNumber, 0, diskNumber.Length);
            byte[] version = BitConverter.GetBytes(obj.Version);
            stream.Write(version, 0, version.Length);
            stream.Write(obj.LANKey, 0, obj.LANKey.Length);
            stream.Write(obj.SignatureKey, 0, obj.SignatureKey.Length);
            for (int i = 0; i < obj.AlternateSignatureKeys.Length; i++)
            {
                stream.Write(obj.AlternateSignatureKeys[i], 0, obj.AlternateSignatureKeys[i].Length);
            }
            byte[] originalCertificateSize = BitConverter.GetBytes(obj.OriginalCertificateSize);
            stream.Write(originalCertificateSize, 0, originalCertificateSize.Length);
            byte[] onlineService = BitConverter.GetBytes(obj.OnlineService);
            stream.Write(onlineService, 0, onlineService.Length);
            byte[] securityFlags = BitConverter.GetBytes(obj.SecurityFlags);
            stream.Write(securityFlags, 0, securityFlags.Length);
            stream.Write(obj.CodeEncKey, 0, obj.CodeEncKey.Length);
        }

        public static void SerializeXbox360Certificate(Stream stream, Data.Models.XenonExecutable.Certificate obj)
        {
            byte[] length = BitConverter.GetBytes(obj.Length);
            stream.Write(length, 0, length.Length);
            byte[] imageSize = BitConverter.GetBytes(obj.ImageSize);
            stream.Write(imageSize, 0, imageSize.Length);
            stream.Write(obj.Signature, 0, obj.Signature.Length);
            byte[] baseFileLoadAddress = BitConverter.GetBytes(obj.BaseFileLoadAddress);
            stream.Write(baseFileLoadAddress, 0, baseFileLoadAddress.Length);
            byte[] imageFlags = BitConverter.GetBytes(obj.ImageFlags);
            stream.Write(imageFlags, 0, imageFlags.Length);
            byte[] imageBaseAddress = BitConverter.GetBytes(obj.ImageBaseAddress);
            stream.Write(imageBaseAddress, 0, imageBaseAddress.Length);
            stream.Write(obj.UnknownHash1, 0, obj.UnknownHash1.Length);
            byte[] unknown0128 = BitConverter.GetBytes(obj.Unknown0128);
            stream.Write(unknown0128, 0, unknown0128.Length);
            stream.Write(obj.UnknownHash2, 0, obj.UnknownHash2.Length);
            stream.Write(obj.MediaID, 0, obj.MediaID.Length);
            stream.Write(obj.XEXFileKey, 0, obj.XEXFileKey.Length);
            byte[] unknown0160 = BitConverter.GetBytes(obj.Unknown0160);
            stream.Write(unknown0160, 0, unknown0160.Length);
            stream.Write(obj.UnknownHash3, 0, obj.UnknownHash3.Length);
            byte[] regionFlags = BitConverter.GetBytes(obj.RegionFlags);
            stream.Write(regionFlags, 0, regionFlags.Length);
            byte[] allowedMediaTypeFlags = BitConverter.GetBytes(obj.AllowedMediaTypeFlags);
            stream.Write(allowedMediaTypeFlags, 0, allowedMediaTypeFlags.Length);
            byte[] tableCount = BitConverter.GetBytes(obj.TableCount);
            stream.Write(tableCount, 0, tableCount.Length);

            for (int i = 0; i < obj.Table.Length; i++)
            {
                byte[] id = BitConverter.GetBytes(obj.Table[i].ID);
                stream.Write(id, 0, id.Length);
                stream.Write(obj.Table[i].Data, 0, obj.Table[i].Data.Length);
            }
        }
    }
}

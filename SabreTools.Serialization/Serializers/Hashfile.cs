using System;
using System.IO;
using System.Linq;
using System.Text;
using SabreTools.Hashing;
using SabreTools.IO.Writers;
using SabreTools.Models.Hashfile;

namespace SabreTools.Serialization.Serializers
{
    // TODO: Create variants for the implemented types
    public class Hashfile : BaseBinarySerializer<Models.Hashfile.Hashfile>
    {
        #region IByteSerializer

        /// <inheritdoc cref="Interfaces.IByteSerializer.SerializeArray(T?)"/>
        public static byte[]? SerializeBytes(Models.Hashfile.Hashfile? obj, HashType hash = HashType.CRC32)
        {
            var serializer = new Hashfile();
            return serializer.SerializeArray(obj, hash);
        }

        /// <inheritdoc/>
        public override byte[]? SerializeArray(Models.Hashfile.Hashfile? obj)
            => SerializeArray(obj, HashType.CRC32);

        /// <inheritdoc/>
        public byte[]? SerializeArray(Models.Hashfile.Hashfile? obj, HashType hash)
        {
            using var stream = SerializeStream(obj, hash);
            if (stream == null)
                return null;

            byte[] bytes = new byte[stream.Length];
            stream.Read(bytes, 0, bytes.Length);
            return bytes;
        }

        #endregion

        #region IFileSerializer

        /// <inheritdoc cref="IFileSerializer.Serialize(T?, string?)"/>
        public static bool SerializeFile(Models.Hashfile.Hashfile? obj, string? path, HashType hash = HashType.CRC32)
        {
            var serializer = new Hashfile();
            return serializer.Serialize(obj, path, hash);
        }
        
        /// <inheritdoc/>
        public override bool Serialize(Models.Hashfile.Hashfile? obj, string? path)
            => Serialize(obj, path, HashType.CRC32);

        /// <inheritdoc/>
        public bool Serialize(Models.Hashfile.Hashfile? obj, string? path, HashType hash)
        {
            if (string.IsNullOrEmpty(path))
                return false;

            using var stream = SerializeStream(obj, hash);
            if (stream == null)
                return false;

            using var fs = System.IO.File.OpenWrite(path);
            stream.CopyTo(fs);
            return true;
        }

        #endregion

        #region IStreamSerializer

        /// <inheritdoc cref="IStreamSerializer.Serialize(T?)"/>
        public static Stream? SerializeStream(Models.Hashfile.Hashfile? obj, HashType hash = HashType.CRC32)
        {
            var serializer = new Hashfile();
            return serializer.Serialize(obj, hash);
        }
        
        /// <inheritdoc/>
        public override Stream? Serialize(Models.Hashfile.Hashfile? obj)
            => Serialize(obj, HashType.CRC32);

        /// <inheritdoc/>
        public Stream? Serialize(Models.Hashfile.Hashfile? obj, HashType hash)
        {
            // If the metadata file is null
            if (obj == null)
                return null;

            // Setup the writer and output
            var stream = new MemoryStream();
            var writer = new SeparatedValueWriter(stream, Encoding.UTF8)
            {
                Separator = ' ',
                Quotes = false,
                VerifyFieldCount = false,
            };

            // Write out the items, if they exist
            switch (hash)
            {
                case HashType.CRC32:
                case HashType.CRC32_ISO:
                case HashType.CRC32_Naive:
                case HashType.CRC32_Optimized:
                case HashType.CRC32_Parallel:
                    WriteSFV(obj.SFV, writer);
                    break;
                case HashType.MD5:
                    WriteMD5(obj.MD5, writer);
                    break;
                case HashType.SHA1:
                    WriteSHA1(obj.SHA1, writer);
                    break;
                case HashType.SHA256:
                    WriteSHA256(obj.SHA256, writer);
                    break;
                case HashType.SHA384:
                    WriteSHA384(obj.SHA384, writer);
                    break;
                case HashType.SHA512:
                    WriteSHA512(obj.SHA512, writer);
                    break;
                case HashType.SpamSum:
                    WriteSpamSum(obj.SpamSum, writer);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(hash));
            }

            // Return the stream
            stream.Seek(0, SeekOrigin.Begin);
            return stream;
        }

        /// <summary>
        /// Write SFV information to the current writer
        /// </summary>
        /// <param name="sfvs">Array of SFV objects representing the files</param>
        /// <param name="writer">SeparatedValueWriter representing the output</param>
        private static void WriteSFV(SFV[]? sfvs, SeparatedValueWriter writer)
        {
            // If the item information is missing, we can't do anything
            if (sfvs == null || !sfvs.Any())
                return;

            // Loop through and write out the items
            foreach (var sfv in sfvs)
            {
                if (sfv == null)
                    continue;
                if (string.IsNullOrEmpty(sfv.File) || string.IsNullOrEmpty(sfv.Hash))
                    continue;

                writer.WriteValues(new string[] { sfv.File!, sfv.Hash! });
                writer.Flush();
            }
        }

        /// <summary>
        /// Write MD5 information to the current writer
        /// </summary>
        /// <param name="md5s">Array of MD5 objects representing the files</param>
        /// <param name="writer">SeparatedValueWriter representing the output</param>
        private static void WriteMD5(MD5[]? md5s, SeparatedValueWriter writer)
        {
            // If the item information is missing, we can't do anything
            if (md5s == null || !md5s.Any())
                return;

            // Loop through and write out the items
            foreach (var md5 in md5s)
            {
                if (md5 == null)
                    continue;
                if (string.IsNullOrEmpty(md5.Hash) || string.IsNullOrEmpty(md5.File))
                    continue;

                writer.WriteValues(new string[] { md5.Hash!, md5.File! });
                writer.Flush();
            }
        }

        /// <summary>
        /// Write SHA1 information to the current writer
        /// </summary>
        /// <param name="sha1s">Array of SHA1 objects representing the files</param>
        /// <param name="writer">SeparatedValueWriter representing the output</param>
        private static void WriteSHA1(SHA1[]? sha1s, SeparatedValueWriter writer)
        {
            // If the item information is missing, we can't do anything
            if (sha1s == null || !sha1s.Any())
                return;

            // Loop through and write out the items
            foreach (var sha1 in sha1s)
            {
                if (sha1 == null)
                    continue;
                if (string.IsNullOrEmpty(sha1.Hash) || string.IsNullOrEmpty(sha1.File))
                    continue;

                writer.WriteValues(new string[] { sha1.Hash!, sha1.File! });
                writer.Flush();
            }
        }

        /// <summary>
        /// Write SHA256 information to the current writer
        /// </summary>
        /// <param name="sha256s">Array of SHA256 objects representing the files</param>
        /// <param name="writer">SeparatedValueWriter representing the output</param>
        private static void WriteSHA256(SHA256[]? sha256s, SeparatedValueWriter writer)
        {
            // If the item information is missing, we can't do anything
            if (sha256s == null || !sha256s.Any())
                return;

            // Loop through and write out the items
            foreach (var sha256 in sha256s)
            {
                if (sha256 == null)
                    continue;
                if (string.IsNullOrEmpty(sha256.Hash) || string.IsNullOrEmpty(sha256.File))
                    continue;

                writer.WriteValues(new string[] { sha256.Hash!, sha256.File! });
                writer.Flush();
            }
        }

        /// <summary>
        /// Write SHA384 information to the current writer
        /// </summary>
        /// <param name="sha384s">Array of SHA384 objects representing the files</param>
        /// <param name="writer">SeparatedValueWriter representing the output</param>
        private static void WriteSHA384(SHA384[]? sha384s, SeparatedValueWriter writer)
        {
            // If the item information is missing, we can't do anything
            if (sha384s == null || !sha384s.Any())
                return;

            // Loop through and write out the items
            foreach (var sha384 in sha384s)
            {
                if (sha384 == null)
                    continue;
                if (string.IsNullOrEmpty(sha384.Hash) || string.IsNullOrEmpty(sha384.File))
                    continue;

                writer.WriteValues(new string[] { sha384.Hash!, sha384.File! });
                writer.Flush();
            }
        }

        /// <summary>
        /// Write SHA512 information to the current writer
        /// </summary>
        /// <param name="sha512s">Array of SHA512 objects representing the files</param>
        /// <param name="writer">SeparatedValueWriter representing the output</param>
        private static void WriteSHA512(SHA512[]? sha512s, SeparatedValueWriter writer)
        {
            // If the item information is missing, we can't do anything
            if (sha512s == null || !sha512s.Any())
                return;

            // Loop through and write out the items
            foreach (var sha512 in sha512s)
            {
                if (sha512 == null)
                    continue;
                if (string.IsNullOrEmpty(sha512.Hash) || string.IsNullOrEmpty(sha512.File))
                    continue;

                writer.WriteValues(new string[] { sha512.Hash!, sha512.File! });
                writer.Flush();
            }
        }

        /// <summary>
        /// Write SpamSum information to the current writer
        /// </summary>
        /// <param name="spamsums">Array of SpamSum objects representing the files</param>
        /// <param name="writer">SeparatedValueWriter representing the output</param>
        private static void WriteSpamSum(SpamSum[]? spamsums, SeparatedValueWriter writer)
        {
            // If the item information is missing, we can't do anything
            if (spamsums == null || !spamsums.Any())
                return;

            // Loop through and write out the items
            foreach (var spamsum in spamsums)
            {
                if (spamsum == null)
                    continue;
                if (string.IsNullOrEmpty(spamsum.Hash) || string.IsNullOrEmpty(spamsum.File))
                    continue;

                writer.WriteValues(new string[] { spamsum.Hash!, spamsum.File! });
                writer.Flush();
            }
        }

        #endregion
    }
}
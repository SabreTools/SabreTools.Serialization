using System;
using System.IO;
using System.Linq;
using System.Text;
using SabreTools.IO.Writers;
using SabreTools.Models.Hashfile;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Streams
{
    public partial class Hashfile : IStreamSerializer<Models.Hashfile.Hashfile>
    {
        /// <inheritdoc/>
        public Stream? Serialize(Models.Hashfile.Hashfile? obj) => Serialize(obj, Hash.CRC);

        /// <inheritdoc/>
        public Stream? Serialize(Models.Hashfile.Hashfile? obj, Hash hash)
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
                case Hash.CRC:
                    WriteSFV(obj.SFV, writer);
                    break;
                case Hash.MD5:
                    WriteMD5(obj.MD5, writer);
                    break;
                case Hash.SHA1:
                    WriteSHA1(obj.SHA1, writer);
                    break;
                case Hash.SHA256:
                    WriteSHA256(obj.SHA256, writer);
                    break;
                case Hash.SHA384:
                    WriteSHA384(obj.SHA384, writer);
                    break;
                case Hash.SHA512:
                    WriteSHA512(obj.SHA512, writer);
                    break;
                case Hash.SpamSum:
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
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using SabreTools.Hashing;
using SabreTools.Models.Hashfile;

namespace SabreTools.Serialization.Deserializers
{
    public class Hashfile : BaseBinaryDeserializer<Models.Hashfile.Hashfile>
    {
        #region IByteDeserializer

        /// <inheritdoc cref="IByteDeserializer.Deserialize(byte[]?, int)"/>
        public static Models.Hashfile.Hashfile? DeserializeBytes(byte[]? data, int offset, HashType hash = HashType.CRC32)
        {
            var deserializer = new Hashfile();
            return deserializer.Deserialize(data, offset, hash);
        }

        /// <inheritdoc/>
        public override Models.Hashfile.Hashfile? Deserialize(byte[]? data, int offset)
            => Deserialize(data, offset, HashType.CRC32);

        /// <inheritdoc/>
        public Models.Hashfile.Hashfile? Deserialize(byte[]? data, int offset, HashType hash)
        {
            // If the data is invalid
            if (data == null || data.Length == 0)
                return null;

            // If the offset is out of bounds
            if (offset < 0 || offset >= data.Length)
                return null;

            // Create a memory stream and parse that
            var dataStream = new MemoryStream(data, offset, data.Length - offset);
            return DeserializeStream(dataStream, hash);
        }

        #endregion

        #region IFileDeserializer

        /// <inheritdoc cref="IFileDeserializer.Deserialize(string?)"/>
        public static Models.Hashfile.Hashfile? DeserializeFile(string? path, HashType hash = HashType.CRC32)
        {
            var deserializer = new Hashfile();
            return deserializer.Deserialize(path, hash);
        }

        /// <inheritdoc/>
        public override Models.Hashfile.Hashfile? Deserialize(string? path)
            => Deserialize(path, HashType.CRC32);

        /// <inheritdoc/>
        public Models.Hashfile.Hashfile? Deserialize(string? path, HashType hash)
        {
            using var stream = PathProcessor.OpenStream(path);
            return DeserializeStream(stream, hash);
        }

        #endregion

        #region IStreamDeserializer

        /// <inheritdoc cref="IStreamDeserializer.Deserialize(Stream?)"/>
        public static Models.Hashfile.Hashfile? DeserializeStream(Stream? data, HashType hash = HashType.CRC32)
        {
            var deserializer = new Hashfile();
            return deserializer.Deserialize(data, hash);
        }

        /// <inheritdoc/>
        public override Models.Hashfile.Hashfile? Deserialize(Stream? data)
            => Deserialize(data, HashType.CRC32);

        /// <inheritdoc cref="Deserialize(Stream)"/>
        public Models.Hashfile.Hashfile? Deserialize(Stream? data, HashType hash)
        {
            return hash switch
            {
                HashType.CRC32 => DeserializeSFV(data),
                HashType.MD2 => DeserializeMD2(data),
                HashType.MD4 => DeserializeMD4(data),
                HashType.MD5 => DeserializeMD5(data),
                HashType.SHA1 => DeserializeSHA1(data),
                HashType.SHA256 => DeserializeSHA256(data),
                HashType.SHA384 => DeserializeSHA384(data),
                HashType.SHA512 => DeserializeSHA512(data),
                HashType.SpamSum => DeserializeSpamSum(data),

                _ => null,
            };
        }

        /// <inheritdoc cref="Deserialize(Stream)"/>
        public Models.Hashfile.Hashfile? DeserializeSFV(Stream? data)
        {
            // If tthe data is invalid
            if (data == null || !data.CanRead)
                return null;

            try
            {
                // Setup the reader and output
                var reader = new StreamReader(data);
                var sfvList = new List<SFV>();

                // Loop through the rows and parse out values
                while (!reader.EndOfStream)
                {
                    // Read and split the line
                    string? line = reader.ReadLine();
                    string[]? lineParts = line?.Split([' '], StringSplitOptions.RemoveEmptyEntries);
                    if (lineParts == null || lineParts.Length < 2)
                        continue;

                    var sfv = new SFV
                    {
                        File = string.Join(" ", lineParts, 0, lineParts.Length - 1),
                        Hash = lineParts[lineParts.Length - 1],
                    };
                    sfvList.Add(sfv);
                }

                // Assign the hashes to the hashfile and return
                if (sfvList.Count > 0)
                    return new Models.Hashfile.Hashfile { SFV = [.. sfvList] };

                return null;
            }
            catch
            {
                // Ignore the actual error
                return null;
            }
        }

        /// <inheritdoc cref="Deserialize(Stream)"/>
        public Models.Hashfile.Hashfile? DeserializeMD2(Stream? data)
        {
            // If tthe data is invalid
            if (data == null || !data.CanRead)
                return null;

            try
            {
                // Setup the reader and output
                var reader = new StreamReader(data);
                var md2List = new List<MD2>();

                // Loop through the rows and parse out values
                while (!reader.EndOfStream)
                {
                    // Read and split the line
                    string? line = reader.ReadLine();
                    string[]? lineParts = line?.Split([' '], StringSplitOptions.RemoveEmptyEntries);
                    if (lineParts == null || lineParts.Length < 2)
                        continue;

                    // Parse the line into a hash
                    var md2 = new MD2
                    {
                        Hash = lineParts[0],
                        File = string.Join(" ", lineParts, 1, lineParts.Length - 1),
                    };
                    md2List.Add(md2);
                }

                // Assign the hashes to the hashfile and return
                if (md2List.Count > 0)
                    return new Models.Hashfile.Hashfile { MD2 = [.. md2List] };

                return null;
            }
            catch
            {
                // Ignore the actual error
                return null;
            }
        }

        /// <inheritdoc cref="Deserialize(Stream)"/>
        public Models.Hashfile.Hashfile? DeserializeMD4(Stream? data)
        {
            // If tthe data is invalid
            if (data == null || !data.CanRead)
                return null;

            try
            {
                // Setup the reader and output
                var reader = new StreamReader(data);
                var md4List = new List<MD4>();

                // Loop through the rows and parse out values
                while (!reader.EndOfStream)
                {
                    // Read and split the line
                    string? line = reader.ReadLine();
                    string[]? lineParts = line?.Split([' '], StringSplitOptions.RemoveEmptyEntries);
                    if (lineParts == null || lineParts.Length < 2)
                        continue;

                    // Parse the line into a hash
                    var md4 = new MD4
                    {
                        Hash = lineParts[0],
                        File = string.Join(" ", lineParts, 1, lineParts.Length - 1),
                    };
                    md4List.Add(md4);
                }

                // Assign the hashes to the hashfile and return
                if (md4List.Count > 0)
                    return new Models.Hashfile.Hashfile { MD4 = [.. md4List] };

                return null;
            }
            catch
            {
                // Ignore the actual error
                return null;
            }
        }

        /// <inheritdoc cref="Deserialize(Stream)"/>
        public Models.Hashfile.Hashfile? DeserializeMD5(Stream? data)
        {
            // If tthe data is invalid
            if (data == null || !data.CanRead)
                return null;

            // Setup the reader and output
            var reader = new StreamReader(data);
            var md5List = new List<MD5>();

            // Loop through the rows and parse out values
            while (!reader.EndOfStream)
            {
                // Read and split the line
                string? line = reader.ReadLine();
                string[]? lineParts = line?.Split([' '], StringSplitOptions.RemoveEmptyEntries);
                if (lineParts == null || lineParts.Length < 2)
                    continue;

                // Parse the line into a hash
                var md5 = new MD5
                {
                    Hash = lineParts[0],
                    File = string.Join(" ", lineParts, 1, lineParts.Length - 1),
                };
                md5List.Add(md5);
            }

            // Assign the hashes to the hashfile and return
            if (md5List.Count > 0)
                return new Models.Hashfile.Hashfile { MD5 = [.. md5List] };

            return null;
        }

        /// <inheritdoc cref="Deserialize(Stream)"/>
        public Models.Hashfile.Hashfile? DeserializeSHA1(Stream? data)
        {
            // If tthe data is invalid
            if (data == null || !data.CanRead)
                return null;

            try
            {
                // Setup the reader and output
                var reader = new StreamReader(data);
                var sha1List = new List<SHA1>();

                // Loop through the rows and parse out values
                while (!reader.EndOfStream)
                {
                    // Read and split the line
                    string? line = reader.ReadLine();
                    string[]? lineParts = line?.Split([' '], StringSplitOptions.RemoveEmptyEntries);
                    if (lineParts == null || lineParts.Length < 2)
                        continue;

                    // Parse the line into a hash
                    var sha1 = new SHA1
                    {
                        Hash = lineParts[0],
                        File = string.Join(" ", lineParts, 1, lineParts.Length - 1),
                    };
                    sha1List.Add(sha1);
                }

                // Assign the hashes to the hashfile and return
                if (sha1List.Count > 0)
                    return new Models.Hashfile.Hashfile { SHA1 = [.. sha1List] };

                return null;
            }
            catch
            {
                // Ignore the actual error
                return null;
            }
        }

        /// <inheritdoc cref="Deserialize(Stream)"/>
        public Models.Hashfile.Hashfile? DeserializeSHA256(Stream? data)
        {
            // If tthe data is invalid
            if (data == null || !data.CanRead)
                return null;

            try
            {
                // Setup the reader and output
                var reader = new StreamReader(data);
                var sha256List = new List<SHA256>();

                // Loop through the rows and parse out values
                while (!reader.EndOfStream)
                {
                    // Read and split the line
                    string? line = reader.ReadLine();
                    string[]? lineParts = line?.Split([' '], StringSplitOptions.RemoveEmptyEntries);
                    if (lineParts == null || lineParts.Length < 2)
                        continue;

                    // Parse the line into a hash
                    var sha256 = new SHA256
                    {
                        Hash = lineParts[0],
                        File = string.Join(" ", lineParts, 1, lineParts.Length - 1),
                    };
                    sha256List.Add(sha256);
                }

                // Assign the hashes to the hashfile and return
                if (sha256List.Count > 0)
                    return new Models.Hashfile.Hashfile { SHA256 = [.. sha256List] };

                return null;
            }
            catch
            {
                // Ignore the actual error
                return null;
            }
        }

        /// <inheritdoc cref="Deserialize(Stream)"/>
        public Models.Hashfile.Hashfile? DeserializeSHA384(Stream? data)
        {
            // If tthe data is invalid
            if (data == null || !data.CanRead)
                return null;

            try
            {
                // Setup the reader and output
                var reader = new StreamReader(data);
                var sha384List = new List<SHA384>();

                // Loop through the rows and parse out values
                while (!reader.EndOfStream)
                {
                    // Read and split the line
                    string? line = reader.ReadLine();
                    string[]? lineParts = line?.Split([' '], StringSplitOptions.RemoveEmptyEntries);
                    if (lineParts == null || lineParts.Length < 2)
                        continue;

                    // Parse the line into a hash
                    var sha384 = new SHA384
                    {
                        Hash = lineParts[0],
                        File = string.Join(" ", lineParts, 1, lineParts.Length - 1),
                    };
                    sha384List.Add(sha384);
                }

                // Assign the hashes to the hashfile and return
                if (sha384List.Count > 0)
                    return new Models.Hashfile.Hashfile { SHA384 = [.. sha384List] };

                return null;
            }
            catch
            {
                // Ignore the actual error
                return null;
            }
        }

        /// <inheritdoc cref="Deserialize(Stream)"/>
        public Models.Hashfile.Hashfile? DeserializeSHA512(Stream? data)
        {
            // If tthe data is invalid
            if (data == null || !data.CanRead)
                return null;

            try
            {
                // Setup the reader and output
                var reader = new StreamReader(data);
                var sha512List = new List<SHA512>();

                // Loop through the rows and parse out values
                while (!reader.EndOfStream)
                {
                    // Read and split the line
                    string? line = reader.ReadLine();
                    string[]? lineParts = line?.Split([' '], StringSplitOptions.RemoveEmptyEntries);
                    if (lineParts == null || lineParts.Length < 2)
                        continue;

                    // Parse the line into a hash
                    var sha512 = new SHA512
                    {
                        Hash = lineParts[0],
                        File = string.Join(" ", lineParts, 1, lineParts.Length - 1),
                    };
                    sha512List.Add(sha512);
                }

                // Assign the hashes to the hashfile and return
                if (sha512List.Count > 0)
                    return new Models.Hashfile.Hashfile { SHA512 = [.. sha512List] };

                return null;
            }
            catch
            {
                // Ignore the actual error
                return null;
            }
        }

        /// <inheritdoc cref="Deserialize(Stream)"/>
        public Models.Hashfile.Hashfile? DeserializeSpamSum(Stream? data)
        {
            // If tthe data is invalid
            if (data == null || !data.CanRead)
                return default;

            try
            {
                // Setup the reader and output
                var reader = new StreamReader(data);
                var spamsumList = new List<SpamSum>();

                // Loop through the rows and parse out values
                while (!reader.EndOfStream)
                {
                    // Read and split the line
                    string? line = reader.ReadLine();
                    string[]? lineParts = line?.Split([' '], StringSplitOptions.RemoveEmptyEntries);
                    if (lineParts == null || lineParts.Length < 2)
                        continue;

                    // Parse the line into a hash
                    var spamSum = new SpamSum
                    {
                        Hash = lineParts[0],
                        File = string.Join(" ", lineParts, 1, lineParts.Length - 1),
                    };
                    spamsumList.Add(spamSum);
                }

                // Assign the hashes to the hashfile and return
                if (spamsumList.Count > 0)
                    return new Models.Hashfile.Hashfile { SpamSum = [.. spamsumList] };

                return null;
            }
            catch
            {
                // Ignore the actual error
                return null;
            }
        }

        #endregion
    }
}
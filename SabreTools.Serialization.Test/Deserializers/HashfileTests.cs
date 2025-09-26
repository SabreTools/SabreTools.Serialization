using System;
using System.IO;
using System.Linq;
using SabreTools.Hashing;
using SabreTools.Serialization.Deserializers;
using Xunit;

namespace SabreTools.Serialization.Test.Deserializers
{
    public class HashfileTests
    {
        [Fact]
        public void NullArray_Null()
        {
            byte[]? data = null;
            int offset = 0;
            var deserializer = new Hashfile();

            var actual = deserializer.Deserialize(data, offset);
            Assert.Null(actual);
        }

        [Fact]
        public void EmptyArray_Null()
        {
            byte[]? data = [];
            int offset = 0;
            var deserializer = new Hashfile();

            var actual = deserializer.Deserialize(data, offset);
            Assert.Null(actual);
        }

        [Fact]
        public void InvalidArray_Null()
        {
            byte[]? data = [.. Enumerable.Repeat<byte>(0xFF, 1024)];
            int offset = 0;
            var deserializer = new Hashfile();

            var actual = deserializer.Deserialize(data, offset);
            Assert.Null(actual);
        }

        [Fact]
        public void NullStream_Null()
        {
            Stream? data = null;
            var deserializer = new Hashfile();

            var actual = deserializer.Deserialize(data);
            Assert.Null(actual);
        }

        [Fact]
        public void EmptyStream_Null()
        {
            Stream? data = new MemoryStream([]);
            var deserializer = new Hashfile();

            var actual = deserializer.Deserialize(data);
            Assert.Null(actual);
        }

        [Fact]
        public void InvalidStream_Null()
        {
            Stream? data = new MemoryStream([.. Enumerable.Repeat<byte>(0xFF, 1024)]);
            var deserializer = new Hashfile();

            var actual = deserializer.Deserialize(data);
            Assert.Null(actual);
        }

        [Fact]
        public void RoundTripSFVTest()
        {
            // Get the serializer and deserializer
            var deserializer = new Hashfile();
            var serializer = new SabreTools.Serialization.Serializers.Hashfile();

            // Build the data
            Models.Hashfile.Hashfile hf = Build(HashType.CRC32);

            // Serialize to stream
            Stream? actual = serializer.SerializeStream(hf, HashType.CRC32);
            Assert.NotNull(actual);

            // Serialize back to original model
            Models.Hashfile.Hashfile? newHf = deserializer.Deserialize(actual, HashType.CRC32);

            // Validate the data
            Assert.NotNull(newHf);
            Assert.NotNull(newHf.SFV);
            var newSfv = Assert.Single(newHf.SFV);
            Validate(newSfv);
        }

        [Fact]
        public void RoundTripMD2Test()
        {
            // Get the serializer and deserializer
            var deserializer = new Hashfile();
            var serializer = new SabreTools.Serialization.Serializers.Hashfile();

            // Build the data
            Models.Hashfile.Hashfile hf = Build(HashType.MD2);

            // Serialize to stream
            Stream? actual = serializer.SerializeStream(hf, HashType.MD2);
            Assert.NotNull(actual);

            // Serialize back to original model
            Models.Hashfile.Hashfile? newHf = deserializer.Deserialize(actual, HashType.MD2);

            // Validate the data
            Assert.NotNull(newHf);
            Assert.NotNull(newHf.MD2);
            var newMd2 = Assert.Single(newHf.MD2);
            Validate(newMd2);
        }

        [Fact]
        public void RoundTripMD4Test()
        {
            // Get the serializer and deserializer
            var deserializer = new Hashfile();
            var serializer = new SabreTools.Serialization.Serializers.Hashfile();

            // Build the data
            Models.Hashfile.Hashfile hf = Build(HashType.MD4);

            // Serialize to stream
            Stream? actual = serializer.SerializeStream(hf, HashType.MD4);
            Assert.NotNull(actual);

            // Serialize back to original model
            Models.Hashfile.Hashfile? newHf = deserializer.Deserialize(actual, HashType.MD4);

            // Validate the data
            Assert.NotNull(newHf);
            Assert.NotNull(newHf.MD4);
            var newMd4 = Assert.Single(newHf.MD4);
            Validate(newMd4);
        }

        [Fact]
        public void RoundTripMD5Test()
        {
            // Get the serializer and deserializer
            var deserializer = new Hashfile();
            var serializer = new SabreTools.Serialization.Serializers.Hashfile();

            // Build the data
            Models.Hashfile.Hashfile hf = Build(HashType.MD5);

            // Serialize to stream
            Stream? actual = serializer.SerializeStream(hf, HashType.MD5);
            Assert.NotNull(actual);

            // Serialize back to original model
            Models.Hashfile.Hashfile? newHf = deserializer.Deserialize(actual, HashType.MD5);

            // Validate the data
            Assert.NotNull(newHf);
            Assert.NotNull(newHf.MD5);
            var newMd5 = Assert.Single(newHf.MD5);
            Validate(newMd5);
        }

        [Fact]
        public void RoundTripSHA1Test()
        {
            // Get the serializer and deserializer
            var deserializer = new Hashfile();
            var serializer = new SabreTools.Serialization.Serializers.Hashfile();

            // Build the data
            Models.Hashfile.Hashfile hf = Build(HashType.SHA1);

            // Serialize to stream
            Stream? actual = serializer.SerializeStream(hf, HashType.SHA1);
            Assert.NotNull(actual);

            // Serialize back to original model
            Models.Hashfile.Hashfile? newHf = deserializer.Deserialize(actual, HashType.SHA1);

            // Validate the data
            Assert.NotNull(newHf);
            Assert.NotNull(newHf.SHA1);
            var newSha1 = Assert.Single(newHf.SHA1);
            Validate(newSha1);
        }

        [Fact]
        public void RoundTripSHA256Test()
        {
            // Get the serializer and deserializer
            var deserializer = new Hashfile();
            var serializer = new SabreTools.Serialization.Serializers.Hashfile();

            // Build the data
            Models.Hashfile.Hashfile hf = Build(HashType.SHA256);

            // Serialize to stream
            Stream? actual = serializer.SerializeStream(hf, HashType.SHA256);
            Assert.NotNull(actual);

            // Serialize back to original model
            Models.Hashfile.Hashfile? newHf = deserializer.Deserialize(actual, HashType.SHA256);

            // Validate the data
            Assert.NotNull(newHf);
            Assert.NotNull(newHf.SHA256);
            var newSha256 = Assert.Single(newHf.SHA256);
            Validate(newSha256);
        }

        [Fact]
        public void RoundTripSHA384Test()
        {
            // Get the serializer and deserializer
            var deserializer = new Hashfile();
            var serializer = new SabreTools.Serialization.Serializers.Hashfile();

            // Build the data
            Models.Hashfile.Hashfile hf = Build(HashType.SHA384);

            // Serialize to stream
            Stream? actual = serializer.SerializeStream(hf, HashType.SHA384);
            Assert.NotNull(actual);

            // Serialize back to original model
            Models.Hashfile.Hashfile? newHf = deserializer.Deserialize(actual, HashType.SHA384);

            // Validate the data
            Assert.NotNull(newHf);
            Assert.NotNull(newHf.SHA384);
            var newSha384 = Assert.Single(newHf.SHA384);
            Validate(newSha384);
        }

        [Fact]
        public void RoundTripSHA512Test()
        {
            // Get the serializer and deserializer
            var deserializer = new Hashfile();
            var serializer = new SabreTools.Serialization.Serializers.Hashfile();

            // Build the data
            Models.Hashfile.Hashfile hf = Build(HashType.SHA512);

            // Serialize to stream
            Stream? actual = serializer.SerializeStream(hf, HashType.SHA512);
            Assert.NotNull(actual);

            // Serialize back to original model
            Models.Hashfile.Hashfile? newHf = deserializer.Deserialize(actual, HashType.SHA512);

            // Validate the data
            Assert.NotNull(newHf);
            Assert.NotNull(newHf.SHA512);
            var newSha512 = Assert.Single(newHf.SHA512);
            Validate(newSha512);
        }

        [Fact]
        public void RoundTripSpamSumTest()
        {
            // Get the serializer and deserializer
            var deserializer = new Hashfile();
            var serializer = new SabreTools.Serialization.Serializers.Hashfile();

            // Build the data
            Models.Hashfile.Hashfile hf = Build(HashType.SpamSum);

            // Serialize to stream
            Stream? actual = serializer.SerializeStream(hf, HashType.SpamSum);
            Assert.NotNull(actual);

            // Serialize back to original model
            Models.Hashfile.Hashfile? newHf = deserializer.Deserialize(actual, HashType.SpamSum);

            // Validate the data
            Assert.NotNull(newHf);
            Assert.NotNull(newHf.SpamSum);
            var newSpamsum = Assert.Single(newHf.SpamSum);
            Validate(newSpamsum);
        }

        /// <summary>
        /// Build model for serialization and deserialization
        /// </summary>
        private static Models.Hashfile.Hashfile Build(HashType hashType)
        {
            return hashType switch
            {
                HashType.CRC32 => new Models.Hashfile.Hashfile { SFV = [new Models.Hashfile.SFV { File = "XXXXXX", Hash = "XXXXXX" }] },
                HashType.MD2 => new Models.Hashfile.Hashfile { MD2 = [new Models.Hashfile.MD2 { Hash = "XXXXXX", File = "XXXXXX" }] },
                HashType.MD4 => new Models.Hashfile.Hashfile { MD4 = [new Models.Hashfile.MD4 { Hash = "XXXXXX", File = "XXXXXX" }] },
                HashType.MD5 => new Models.Hashfile.Hashfile { MD5 = [new Models.Hashfile.MD5 { Hash = "XXXXXX", File = "XXXXXX" }] },
                HashType.SHA1 => new Models.Hashfile.Hashfile { SHA1 = [new Models.Hashfile.SHA1 { Hash = "XXXXXX", File = "XXXXXX" }] },
                HashType.SHA256 => new Models.Hashfile.Hashfile { SHA256 = [new Models.Hashfile.SHA256 { Hash = "XXXXXX", File = "XXXXXX" }] },
                HashType.SHA384 => new Models.Hashfile.Hashfile { SHA384 = [new Models.Hashfile.SHA384 { Hash = "XXXXXX", File = "XXXXXX" }] },
                HashType.SHA512 => new Models.Hashfile.Hashfile { SHA512 = [new Models.Hashfile.SHA512 { Hash = "XXXXXX", File = "XXXXXX" }] },
                HashType.SpamSum => new Models.Hashfile.Hashfile { SpamSum = [new Models.Hashfile.SpamSum { Hash = "XXXXXX", File = "XXXXXX" }] },
                _ => throw new ArgumentOutOfRangeException(),
            };
        }

        /// <summary>
        /// Validate a SFV
        /// </summary>
        private static void Validate(Models.Hashfile.SFV? sfv)
        {
            Assert.NotNull(sfv);
            Assert.Equal("XXXXXX", sfv.File);
            Assert.Equal("XXXXXX", sfv.Hash);
        }

        /// <summary>
        /// Validate a MD2
        /// </summary>
        private static void Validate(Models.Hashfile.MD2? md2)
        {
            Assert.NotNull(md2);
            Assert.Equal("XXXXXX", md2.Hash);
            Assert.Equal("XXXXXX", md2.File);
        }

        /// <summary>
        /// Validate a MD4
        /// </summary>
        private static void Validate(Models.Hashfile.MD4? md4)
        {
            Assert.NotNull(md4);
            Assert.Equal("XXXXXX", md4.Hash);
            Assert.Equal("XXXXXX", md4.File);
        }

        /// <summary>
        /// Validate a MD5
        /// </summary>
        private static void Validate(Models.Hashfile.MD5? md5)
        {
            Assert.NotNull(md5);
            Assert.Equal("XXXXXX", md5.Hash);
            Assert.Equal("XXXXXX", md5.File);
        }

        /// <summary>
        /// Validate a SHA1
        /// </summary>
        private static void Validate(Models.Hashfile.SHA1? sha1)
        {
            Assert.NotNull(sha1);
            Assert.Equal("XXXXXX", sha1.Hash);
            Assert.Equal("XXXXXX", sha1.File);
        }

        /// <summary>
        /// Validate a SHA256
        /// </summary>
        private static void Validate(Models.Hashfile.SHA256? sha256)
        {
            Assert.NotNull(sha256);
            Assert.Equal("XXXXXX", sha256.Hash);
            Assert.Equal("XXXXXX", sha256.File);
        }

        /// <summary>
        /// Validate a SHA384
        /// </summary>
        private static void Validate(Models.Hashfile.SHA384? sha384)
        {
            Assert.NotNull(sha384);
            Assert.Equal("XXXXXX", sha384.Hash);
            Assert.Equal("XXXXXX", sha384.File);
        }

        /// <summary>
        /// Validate a SHA512
        /// </summary>
        private static void Validate(Models.Hashfile.SHA512? sha512)
        {
            Assert.NotNull(sha512);
            Assert.Equal("XXXXXX", sha512.Hash);
            Assert.Equal("XXXXXX", sha512.File);
        }

        /// <summary>
        /// Validate a SpamSum
        /// </summary>
        private static void Validate(Models.Hashfile.SpamSum? spamsum)
        {
            Assert.NotNull(spamsum);
            Assert.Equal("XXXXXX", spamsum.Hash);
            Assert.Equal("XXXXXX", spamsum.File);
        }
    }
}
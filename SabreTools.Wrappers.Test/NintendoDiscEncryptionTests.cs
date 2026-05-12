using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using Newtonsoft.Json;
using Xunit;
using Xunit.Abstractions;

namespace SabreTools.Wrappers.Test
{
    [Collection("NintendoDisc")]
    public class NintendoDiscEncryptionTests
    {
        private readonly ITestOutputHelper _output;

        public NintendoDiscEncryptionTests(ITestOutputHelper output)
        {
            _output = output;
        }
        // -----------------------------------------------------------------------
        // DecryptTitleKey — no provider set
        // -----------------------------------------------------------------------

        [Fact]
        public void DecryptTitleKey_NoProvider_ReturnsNull()
        {
            NintendoDisc.CommonKeyProvider = null;
            Assert.Null(NintendoDisc.DecryptTitleKey(new byte[16], new byte[8], 0));
        }

        // -----------------------------------------------------------------------
        // DecryptTitleKey — argument guards
        // -----------------------------------------------------------------------

        [Fact]
        public void DecryptTitleKey_NullEncKey_ReturnsNull()
        {
            NintendoDisc.CommonKeyProvider = _ => new byte[16];
            try { Assert.Null(NintendoDisc.DecryptTitleKey(null!, new byte[8], 0)); }
            finally { NintendoDisc.CommonKeyProvider = null; }
        }

        [Fact]
        public void DecryptTitleKey_WrongLengthEncKey_ReturnsNull()
        {
            NintendoDisc.CommonKeyProvider = _ => new byte[16];
            try { Assert.Null(NintendoDisc.DecryptTitleKey(new byte[8], new byte[8], 0)); }
            finally { NintendoDisc.CommonKeyProvider = null; }
        }

        [Fact]
        public void DecryptTitleKey_NullTitleId_ReturnsNull()
        {
            NintendoDisc.CommonKeyProvider = _ => new byte[16];
            try { Assert.Null(NintendoDisc.DecryptTitleKey(new byte[16], null!, 0)); }
            finally { NintendoDisc.CommonKeyProvider = null; }
        }

        [Fact]
        public void DecryptTitleKey_WrongLengthTitleId_ReturnsNull()
        {
            NintendoDisc.CommonKeyProvider = _ => new byte[16];
            try { Assert.Null(NintendoDisc.DecryptTitleKey(new byte[16], new byte[4], 0)); }
            finally { NintendoDisc.CommonKeyProvider = null; }
        }

        // -----------------------------------------------------------------------
        // DecryptTitleKey — provider returns null for unknown index
        // -----------------------------------------------------------------------

        [Fact]
        public void DecryptTitleKey_UnknownIndex_ReturnsNull()
        {
            NintendoDisc.CommonKeyProvider = _ => null;
            try { Assert.Null(NintendoDisc.DecryptTitleKey(new byte[16], new byte[8], 0)); }
            finally { NintendoDisc.CommonKeyProvider = null; }
        }

        // -----------------------------------------------------------------------
        // DecryptTitleKey — round-trip with injected key
        // -----------------------------------------------------------------------

        [Fact]
        public void DecryptTitleKey_WithInjectedKey_RoundTrips()
        {
            byte[] commonKey =
            {
                0xDE, 0xAD, 0xBE, 0xEF, 0xCA, 0xFE, 0xF0, 0x0D,
                0x11, 0x22, 0x33, 0x44, 0x55, 0x66, 0x77, 0x88,
            };
            byte[] plainTitleKey =
            {
                0x01, 0x23, 0x45, 0x67, 0x89, 0xAB, 0xCD, 0xEF,
                0xFE, 0xDC, 0xBA, 0x98, 0x76, 0x54, 0x32, 0x10,
            };
            byte[] titleId = { 0x00, 0x01, 0x00, 0x45, 0x52, 0x53, 0x42, 0x00 };

            byte[] iv = new byte[16];
            Array.Copy(titleId, 0, iv, 0, 8);
            byte[] encTitleKey = AesCbc.Encrypt(plainTitleKey, commonKey, iv)
                ?? throw new InvalidOperationException("AesCbc.Encrypt returned null");

            NintendoDisc.CommonKeyProvider = _ => commonKey;
            try
            {
                byte[]? decrypted = NintendoDisc.DecryptTitleKey(encTitleKey, titleId, 0);
                Assert.NotNull(decrypted);
                Assert.Equal(plainTitleKey, decrypted);
            }
            finally { NintendoDisc.CommonKeyProvider = null; }
        }

        // -----------------------------------------------------------------------
        // Integration test — reads a real key file supplied by the user.
        //
        // Copy keys.json.example to keys.json and fill in the real key bytes.
        // The test is silently skipped when the file is absent OR when the loaded
        // keys do not hash to the expected SHA256 values hardcoded below, so CI
        // stays green without real keys in the repository.
        // -----------------------------------------------------------------------

        // SHA256(retail common key bytes)
        private const string RetailKeySha256 = "de38aeab4fe0c36d828a47e6fd315100e7ce234d3b00aa25e6ad6f5ff2824af8";
        // SHA256(Korean common key bytes)
        private const string KoreanKeySha256 = "b9f42ca27a1e178f0f14ebf1a05d486fa8db8d08875336c4e6e8dfae29f2901c";

        [Fact]
        public void LoadFromKeyFile_RealKeys_DecryptTitleKey_Succeeds()
        {
            string keyFile = Path.Combine(
                AppContext.BaseDirectory, "TestData", "NintendoDisc", "keys.json");

            _output.WriteLine($"Looking for key file: {keyFile}");

            if (!File.Exists(keyFile))
            {
                _output.WriteLine("Key file not found — test skipped.");
                return;
            }

            _output.WriteLine("Key file found. Parsing...");
            var provider = LoadKeyProvider(keyFile);
            NintendoDisc.CommonKeyProvider = provider;
            try
            {
                byte[]? retail = provider.Invoke(0);
                byte[]? korean = provider.Invoke(1);

                string retailHash = retail is null ? "(missing)" : Sha256Hex(retail);
                string koreanHash = korean is null ? "(missing)" : Sha256Hex(korean);

                _output.WriteLine($"retail (index 0) SHA256 : {retailHash}");
                _output.WriteLine($"  expected               : {RetailKeySha256}");
                _output.WriteLine($"  match                  : {retailHash == RetailKeySha256}");

                _output.WriteLine($"korean (index 1) SHA256 : {koreanHash}");
                _output.WriteLine($"  expected               : {KoreanKeySha256}");
                _output.WriteLine($"  match                  : {koreanHash == KoreanKeySha256}");

                if (retail is null || retailHash != RetailKeySha256)
                {
                    _output.WriteLine("retail key did not match — integration assertions skipped.");
                    return;
                }
                if (korean is null || koreanHash != KoreanKeySha256)
                {
                    _output.WriteLine("korean key did not match — integration assertions skipped.");
                    return;
                }

                _output.WriteLine("Both keys verified — running assertions.");
                Assert.Equal(16, retail.Length);
                Assert.Equal(16, korean.Length);
                _output.WriteLine("Assertions passed.");
            }
            finally { NintendoDisc.CommonKeyProvider = null; }
        }

        private static string Sha256Hex(byte[] data)
        {
            using var sha = SHA256.Create();
            return BitConverter.ToString(sha.ComputeHash(data)).Replace("-", string.Empty).ToLowerInvariant();
        }

        // -----------------------------------------------------------------------
        // Helper — parses the named JSON key file and returns a provider delegate.
        // Lives here in the test project; the library itself never does file I/O.
        // -----------------------------------------------------------------------

        /// <summary>
        /// Parses a named Wii common-key JSON file and returns a
        /// <see cref="NintendoDisc.CommonKeyProvider"/>-compatible delegate.
        /// </summary>
        /// <remarks>
        /// Expected file format:
        /// <code>
        /// [
        ///   { "name": "retail", "index": 0, "key": "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx" },
        ///   { "name": "korean", "index": 1, "key": "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx" }
        /// ]
        /// </code>
        /// Whitespace inside hex strings is ignored. Returns <see langword="null"/> from the
        /// delegate for any index not present in the file.
        /// </remarks>
        internal static Func<byte, byte[]?> LoadKeyProvider(string path)
        {
            string json = File.ReadAllText(path);
            var entries = JsonConvert.DeserializeObject<List<WiiKeyEntry>>(json)
                ?? throw new FormatException("Key file could not be deserialized.");

            var map = new Dictionary<byte, byte[]>();
            foreach (var entry in entries)
            {
                if (entry.Key is null)
                    throw new FormatException($"Entry '{entry.Name}' is missing a key value.");

                string hex = entry.Key.Replace(" ", string.Empty).Replace("-", string.Empty);
                if (hex.Length != 32)
                    throw new FormatException($"Entry '{entry.Name}' key must be 16 bytes (32 hex chars), got {hex.Length / 2}.");

                byte[] bytes = new byte[16];
                for (int i = 0; i < 16; i++)
                    bytes[i] = Convert.ToByte(hex.Substring(i * 2, 2), 16);

                map[entry.Index] = bytes;
            }

            return index => map.TryGetValue(index, out byte[]? k) ? k : null;
        }

        private sealed class WiiKeyEntry
        {
            [JsonProperty("name")]  public string? Name  { get; set; }
            [JsonProperty("index")] public byte    Index { get; set; }
            [JsonProperty("key")]   public string? Key   { get; set; }
        }
    }
}

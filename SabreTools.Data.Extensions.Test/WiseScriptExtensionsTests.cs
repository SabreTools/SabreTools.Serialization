using Xunit;

namespace SabreTools.Data.Extensions.Test
{
    public class WiseScriptExtensionsTests
    {
        [Theory]
        // Defined functions
        [InlineData("f0", "Add Directory to PATH")]
        [InlineData("f1", "Add to AUTOEXEC.BAT")]
        [InlineData("f2", "Add to CONFIG.SYS")]
        [InlineData("f3", "Add to SYSTEM.INI")]
        [InlineData("f8", "Read INI Value")]
        [InlineData("f9", "Get Registry Key Value")]
        [InlineData("f10", "Register Font")]
        [InlineData("f11", "Win32 System Directory")]
        [InlineData("f12", "Check Configuration")]
        [InlineData("f13", "Search for File")]
        [InlineData("f15", "Read/Write Binary File")]
        [InlineData("f16", "Set Variable")]
        [InlineData("f17", "Get Environment Variable")]
        [InlineData("f19", "Check if File/Dir Exists")]
        [InlineData("f20", "Set File Attributes")]
        [InlineData("f21", "Set Files/Buffers")]
        [InlineData("f22", "Find File in Path")]
        [InlineData("f23", "Check Disk Space")]
        [InlineData("f25", "Insert Line Into Text File")]
        [InlineData("f27", "Parse String")]
        [InlineData("f28", "Exit Installation")]
        [InlineData("f29", "Self-Register OCXs/DLLs")]
        [InlineData("f30", "Install DirectX Components")]
        [InlineData("f31", "Wizard Block")]
        [InlineData("f33", "Read/Update Text File")]
        [InlineData("f34", "Post to HTTP Server")]
        [InlineData("f35", "Prompt for Filename")]
        [InlineData("f36", "Start/Stop Service")]
        [InlineData("f38", "Check HTTP Connection")]
        // Undefined functions
        [InlineData("f4", "UNDEFINED f4")]
        [InlineData("f5", "UNDEFINED f5")]
        [InlineData("f6", "UNDEFINED f6")]
        [InlineData("f7", "UNDEFINED f7")]
        [InlineData("f14", "UNDEFINED f14")]
        [InlineData("f18", "UNDEFINED f18")]
        [InlineData("f24", "UNDEFINED f24")]
        [InlineData("f26", "UNDEFINED f26")]
        [InlineData("f32", "UNDEFINED f32")]
        [InlineData("f37", "UNDEFINED f37")]
        // External DLL
        [InlineData(null, null)]
        [InlineData("f99", "UNDEFINED f99")]
        [InlineData("func", "External: func")]
        public void FromWiseFunctionIdTest(string? functionId, string? expected)
        {
            string? actual = functionId.FromWiseFunctionId();
            Assert.Equal(expected, actual);
        }
    }
}

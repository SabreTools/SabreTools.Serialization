using SabreTools.Serialization;
using SabreTools.Serialization.Deserializers;

namespace Test
{
    class Program
    {
        public static void Main(string[] args)
        {
            string path = "R:\\BurnOutSharp Testing Files\\FileType\\AACS Media Key Block\\1 (HD-DVD)\\MKBROM.AACS";
            var obj = AACS.DeserializeFile(path);
            // DO NOTHING FOR NOW
        }
    }
}
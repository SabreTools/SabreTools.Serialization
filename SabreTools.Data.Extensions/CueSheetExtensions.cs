using System.Text.RegularExpressions;
using SabreTools.Data.Models.CueSheets;

#pragma warning disable SYSLIB1045 // Convert to 'GeneratedRegexAttribute'
namespace SabreTools.Data.Extensions
{
    public static class CueSheetExtensions
    {
        /// <summary>
        /// Get the GD-ROM density area of a REM line
        /// </summary>
        /// <param name="remark">Text after the REM keyword</param>
        /// <returns>Density area of a REM SINGLE-DENSITY AREA or REM HIGH-DENSITY AREA line, otherwise None</returns>
        public static CueDensityArea GetDensityArea(string remark)
        {
            string[] parts = SplitRemark(remark);
            if (parts.Length < 2)
                return CueDensityArea.None;

            return parts[0] switch
            {
                "SINGLE-DENSITY" => CueDensityArea.SingleDensity,
                "HIGH-DENSITY" => CueDensityArea.HighDensity,
                _ => CueDensityArea.None,
            };
        }

        /// <summary>
        /// Get the session of a REM line
        /// </summary>
        /// <param name="remark">Text after the REM keyword</param>
        /// <returns>Session of a REM SESSION line, otherwise null</returns>
        public static int? GetSession(string remark)
        {
            string[] parts = SplitRemark(remark);
            if (parts.Length < 2 || parts[0] != "SESSION")
                return null;

            // A value that is not a number is ignored like any other comment
            if (!int.TryParse(parts[1], out int session))
                return null;

            return session;
        }

        /// <summary>
        /// Split the text of a REM line on whitespace, keeping quoted values together like the cue sheet reader
        /// </summary>
        private static string[] SplitRemark(string remark)
        {
            var matches = Regex.Matches(remark, @"[^\s""]+|""[^""]*""");
            var parts = new string[matches.Count];
            for (int i = 0; i < matches.Count; i++)
            {
                parts[i] = matches[i].Value;
            }

            return parts;
        }
    }
}

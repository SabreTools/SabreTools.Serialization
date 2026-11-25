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
            string[] parts = SplitLine(remark);
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
            string[] parts = SplitLine(remark);
            if (parts.Length < 2 || parts[0] != "SESSION")
                return null;

            // A value that is not a number is ignored like any other comment
            if (!int.TryParse(parts[1], out int session))
                return null;

            return session;
        }

        /// <summary>
        /// Read the session or density area from the text of a REM line, other REM lines do not change them
        /// </summary>
        /// <param name="remark">Text after the REM keyword</param>
        /// <param name="session">Session of the last REM SESSION line</param>
        /// <param name="densityArea">Density area of the last REM SINGLE-DENSITY AREA or REM HIGH-DENSITY AREA line</param>
        public static void ReadRemark(string remark, ref int session, ref CueDensityArea densityArea)
        {
            int? remarkSession = GetSession(remark);
            if (remarkSession is not null)
                session = remarkSession.Value;

            var remarkDensityArea = GetDensityArea(remark);
            if (remarkDensityArea != CueDensityArea.None)
                densityArea = remarkDensityArea;
        }

        /// <summary>
        /// Split a line of a cue sheet on whitespace, keeping quoted values together
        /// </summary>
        /// <param name="line">Line to split</param>
        /// <returns>Parts of the line, a quoted value with its quotes</returns>
        public static string[] SplitLine(string line)
        {
            // http://stackoverflow.com/questions/554013/regular-expression-to-split-on-spaces-unless-in-quotes
            var matches = Regex.Matches(line, @"[^\s""]+|""[^""]*""");
            var parts = new string[matches.Count];
            for (int i = 0; i < matches.Count; i++)
            {
                parts[i] = matches[i].Value;
            }

            return parts;
        }
    }
}

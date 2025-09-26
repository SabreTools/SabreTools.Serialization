namespace SabreTools.Data.Models.PlayStation4
{
    /// <see href="https://www.psdevwiki.com/ps4/PKG_files"/>
    public class Constants
    {
        /// <summary>
        /// Identifying bytes for app.pkg file, "\7FCNT"
        /// </summary>
        public const uint AppPkgMagic = 0x7F434E54;
    }
}

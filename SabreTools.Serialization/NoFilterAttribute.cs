namespace SabreTools.Serialization
{
    /// <summary>
    /// Marks a key as unable to be filtered on
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Field)]
    public class NoFilterAttribute : System.Attribute { }
}
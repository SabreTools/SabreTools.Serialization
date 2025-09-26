namespace SabreTools.Data.Attributes
{
    /// <summary>
    /// Marks a property as required on write
    /// </summary>
    /// <remarks>TODO: Use reflection to determine required fields on write</remarks>
    [System.AttributeUsage(System.AttributeTargets.Property)]
    public class RequiredAttribute : System.Attribute { }
}
using System.Xml.Serialization;

namespace SabreTools.Data.Models.PortableExecutable.Resource.Entries
{
    /// <see href="https://learn.microsoft.com/en-us/windows/win32/sbscs/manifest-file-schema"/>
    [XmlRoot(ElementName = "assembly", Namespace = "urn:schemas-microsoft-com:asm.v1")]
    public sealed class AssemblyManifest
    {
        [XmlAttribute("manifestVersion")]
        public string? ManifestVersion { get; set; }

        #region Group

        [XmlElement("assemblyIdentity")]
        public AssemblyIdentity[]? AssemblyIdentities { get; set; }

        [XmlElement("noInheritable")]
        public AssemblyNoInheritable[]? NoInheritables { get; set; }

        #endregion

        #region Group

        [XmlElement("description")]
        public AssemblyDescription? Description { get; set; }

        [XmlElement("noInherit")]
        public AssemblyNoInherit? NoInherit { get; set; }

        //[XmlElement("noInheritable")]
        //public AssemblyNoInheritable NoInheritable { get; set; }

        [XmlElement("comInterfaceExternalProxyStub")]
        public AssemblyCOMInterfaceExternalProxyStub[]? COMInterfaceExternalProxyStub { get; set; }

        [XmlElement("dependency")]
        public AssemblyDependency[]? Dependency { get; set; }

        [XmlElement("file")]
        public AssemblyFile[]? File { get; set; }

        [XmlElement("clrClass")]
        public AssemblyCommonLanguageRuntimeClass[]? CLRClass { get; set; }

        [XmlElement("clrSurrogate")]
        public AssemblyCommonLanguageSurrogateClass[]? CLRSurrogate { get; set; }

        #endregion

        [XmlAnyElement]
        public object[]? EverythingElse { get; set; }
    }

    /// <see href="https://learn.microsoft.com/en-us/windows/win32/sbscs/manifest-file-schema"/>
    public sealed class AssemblyActiveCodePage
    {
        [XmlText]
        public string? Value { get; set; }
    }

    /// <see href="https://learn.microsoft.com/en-us/windows/win32/sbscs/manifest-file-schema"/>
    public sealed class AssemblyAutoElevate
    {
        [XmlText]
        public string? Value { get; set; }
    }

    /// <see href="https://learn.microsoft.com/en-us/windows/win32/sbscs/manifest-file-schema"/>
    public sealed class AssemblyBindingRedirect
    {
        [XmlAttribute("oldVersion")]
        public string? OldVersion { get; set; }

        [XmlAttribute("newVersion")]
        public string? NewVersion { get; set; }
    }

    /// <see href="https://learn.microsoft.com/en-us/windows/win32/sbscs/manifest-file-schema"/>
    public sealed class AssemblyCOMClass
    {
        [XmlAttribute("clsid")]
        public string? CLSID { get; set; }

        [XmlAttribute("threadingModel")]
        public string? ThreadingModel { get; set; }

        [XmlAttribute("progid")]
        public string? ProgID { get; set; }

        [XmlAttribute("tlbid")]
        public string? TLBID { get; set; }

        [XmlAttribute("description")]
        public string? Description { get; set; }

        [XmlElement("progid")]
        public AssemblyProgID[]? ProgIDs { get; set; }
    }

    /// <see href="https://learn.microsoft.com/en-us/windows/win32/sbscs/manifest-file-schema"/>
    public sealed class AssemblyCOMInterfaceExternalProxyStub
    {
        [XmlAttribute("iid")]
        public string? IID { get; set; }

        [XmlAttribute("name")]
        public string? Name { get; set; }

        [XmlAttribute("tlbid")]
        public string? TLBID { get; set; }

        [XmlAttribute("numMethods")]
        public string? NumMethods { get; set; }

        [XmlAttribute("proxyStubClsid32")]
        public string? ProxyStubClsid32 { get; set; }

        [XmlAttribute("baseInterface")]
        public string? BaseInterface { get; set; }
    }

    /// <see href="https://learn.microsoft.com/en-us/windows/win32/sbscs/manifest-file-schema"/>
    public sealed class AssemblyCOMInterfaceProxyStub
    {
        [XmlAttribute("iid")]
        public string? IID { get; set; }

        [XmlAttribute("name")]
        public string? Name { get; set; }

        [XmlAttribute("tlbid")]
        public string? TLBID { get; set; }

        [XmlAttribute("numMethods")]
        public string? NumMethods { get; set; }

        [XmlAttribute("proxyStubClsid32")]
        public string? ProxyStubClsid32 { get; set; }

        [XmlAttribute("baseInterface")]
        public string? BaseInterface { get; set; }
    }

    /// <see href="https://learn.microsoft.com/en-us/windows/win32/sbscs/manifest-file-schema"/>
    public sealed class AssemblyCommonLanguageRuntimeClass
    {
        [XmlAttribute("name")]
        public string? Name { get; set; }

        [XmlAttribute("clsid")]
        public string? CLSID { get; set; }

        [XmlAttribute("progid")]
        public string? ProgID { get; set; }

        [XmlAttribute("tlbid")]
        public string? TLBID { get; set; }

        [XmlAttribute("description")]
        public string? Description { get; set; }

        [XmlAttribute("runtimeVersion")]
        public string? RuntimeVersion { get; set; }

        [XmlAttribute("threadingModel")]
        public string? ThreadingModel { get; set; }

        [XmlElement("progid")]
        public AssemblyProgID[]? ProgIDs { get; set; }
    }

    /// <see href="https://learn.microsoft.com/en-us/windows/win32/sbscs/manifest-file-schema"/>
    public sealed class AssemblyCommonLanguageSurrogateClass
    {
        [XmlAttribute("clsid")]
        public string? CLSID { get; set; }

        [XmlAttribute("name")]
        public string? Name { get; set; }

        [XmlAttribute("runtimeVersion")]
        public string? RuntimeVersion { get; set; }
    }

    /// <see href="https://learn.microsoft.com/en-us/windows/win32/sbscs/manifest-file-schema"/>
    public sealed class AssemblyDependency
    {
        [XmlElement("dependentAssembly")]
        public AssemblyDependentAssembly? DependentAssembly { get; set; }

        [XmlAttribute("optional")]
        public string? Optional { get; set; }
    }

    /// <see href="https://learn.microsoft.com/en-us/windows/win32/sbscs/manifest-file-schema"/>
    public sealed class AssemblyDependentAssembly
    {
        [XmlElement("assemblyIdentity")]
        public AssemblyIdentity? AssemblyIdentity { get; set; }

        [XmlElement("bindingRedirect")]
        public AssemblyBindingRedirect[]? BindingRedirect { get; set; }
    }

    /// <see href="https://learn.microsoft.com/en-us/windows/win32/sbscs/manifest-file-schema"/>
    public sealed class AssemblyDescription
    {
        [XmlText]
        public string? Value { get; set; }
    }

    /// <see href="https://learn.microsoft.com/en-us/windows/win32/sbscs/manifest-file-schema"/>
    public sealed class AssemblyDisableTheming
    {
        [XmlText]
        public string? Value { get; set; }
    }

    /// <see href="https://learn.microsoft.com/en-us/windows/win32/sbscs/manifest-file-schema"/>
    public sealed class AssemblyDisableWindowFiltering
    {
        [XmlText]
        public string? Value { get; set; }
    }

    /// <see href="https://learn.microsoft.com/en-us/windows/win32/sbscs/manifest-file-schema"/>
    public sealed class AssemblyDPIAware
    {
        [XmlText]
        public string? Value { get; set; }
    }

    /// <see href="https://learn.microsoft.com/en-us/windows/win32/sbscs/manifest-file-schema"/>
    public sealed class AssemblyDPIAwareness
    {
        [XmlText]
        public string? Value { get; set; }
    }

    /// <see href="https://learn.microsoft.com/en-us/windows/win32/sbscs/manifest-file-schema"/>
    public sealed class AssemblyFile
    {
        [XmlAttribute("name")]
        public string? Name { get; set; }

        [XmlAttribute("hash")]
        public string? Hash { get; set; }

        [XmlAttribute("hashalg")]
        public string? HashAlgorithm { get; set; }

        [XmlAttribute("size")]
        public string? Size { get; set; }

        #region Group

        [XmlElement("comClass")]
        public AssemblyCOMClass[]? COMClass { get; set; }

        [XmlElement("comInterfaceProxyStub")]
        public AssemblyCOMInterfaceProxyStub[]? COMInterfaceProxyStub { get; set; }

        [XmlElement("typelib")]
        public AssemblyTypeLib[]? Typelib { get; set; }

        [XmlElement("windowClass")]
        public AssemblyWindowClass[]? WindowClass { get; set; }

        #endregion
    }

    /// <see href="https://learn.microsoft.com/en-us/windows/win32/sbscs/manifest-file-schema"/>
    public sealed class AssemblyGDIScaling
    {
        [XmlText]
        public string? Value { get; set; }
    }

    /// <see href="https://learn.microsoft.com/en-us/windows/win32/sbscs/manifest-file-schema"/>
    public sealed class AssemblyHeapType
    {
        [XmlText]
        public string? Value { get; set; }
    }

    /// <see href="https://learn.microsoft.com/en-us/windows/win32/sbscs/manifest-file-schema"/>
    public sealed class AssemblyHighResolutionScrollingAware
    {
        [XmlText]
        public string? Value { get; set; }
    }

    /// <see href="https://learn.microsoft.com/en-us/windows/win32/sbscs/manifest-file-schema"/>
    public sealed class AssemblyIdentity
    {
        [XmlAttribute("name")]
        public string? Name { get; set; }

        [XmlAttribute("version")]
        public string? Version { get; set; }

        [XmlAttribute("type")]
        public string? Type { get; set; }

        [XmlAttribute("processorArchitecture")]
        public string? ProcessorArchitecture { get; set; }

        [XmlAttribute("publicKeyToken")]
        public string? PublicKeyToken { get; set; }

        [XmlAttribute("language")]
        public string? Language { get; set; }
    }

    /// <see href="https://learn.microsoft.com/en-us/windows/win32/sbscs/manifest-file-schema"/>
    public sealed class AssemblyLongPathAware
    {
        [XmlText]
        public string? Value { get; set; }
    }

    /// <see href="https://learn.microsoft.com/en-us/windows/win32/sbscs/manifest-file-schema"/>
    public sealed class AssemblyNoInherit
    {
    }

    /// <see href="https://learn.microsoft.com/en-us/windows/win32/sbscs/manifest-file-schema"/>
    public sealed class AssemblyNoInheritable
    {
    }

    /// <see href="https://learn.microsoft.com/en-us/windows/win32/sbscs/manifest-file-schema"/>
    public sealed class AssemblyPrinterDriverIsolation
    {
        [XmlText]
        public string? Value { get; set; }
    }

    /// <see href="https://learn.microsoft.com/en-us/windows/win32/sbscs/manifest-file-schema"/>
    public sealed class AssemblyProgID
    {
        [XmlText]
        public string? Value { get; set; }
    }

    /// <see href="https://learn.microsoft.com/en-us/windows/win32/sbscs/manifest-file-schema"/>
    public sealed class AssemblySupportedOS
    {
        [XmlAttribute("Id")]
        public string? Id { get; set; }
    }

    /// <see href="https://learn.microsoft.com/en-us/windows/win32/sbscs/manifest-file-schema"/>
    public sealed class AssemblyTypeLib
    {
        [XmlElement("tlbid")]
        public string? TLBID { get; set; }

        [XmlElement("version")]
        public string? Version { get; set; }

        [XmlElement("helpdir")]
        public string? HelpDir { get; set; }

        [XmlElement("resourceid")]
        public string? ResourceID { get; set; }

        [XmlElement("flags")]
        public string? Flags { get; set; }
    }

    /// <see href="https://learn.microsoft.com/en-us/windows/win32/sbscs/manifest-file-schema"/>
    public sealed class AssemblyUltraHighResolutionScrollingAware
    {
        [XmlText]
        public string? Value { get; set; }
    }

    /// <see href="https://learn.microsoft.com/en-us/windows/win32/sbscs/manifest-file-schema"/>
    public sealed class AssemblyWindowClass
    {
        [XmlAttribute("versioned")]
        public string? Versioned { get; set; }

        [XmlText]
        public string? Value { get; set; }
    }

    // TODO: Left off at <ElementType name="progid" />
}

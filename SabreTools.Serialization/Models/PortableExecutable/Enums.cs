using System;

namespace SabreTools.Serialization.Models.PortableExecutable
{
    [Flags]
    public enum AcceleratorTableFlags : ushort
    {
        /// <summary>
        /// The accelerator key is a virtual-key code. If this flag is not specified,
        /// the accelerator key is assumed to specify an ASCII character code. 
        /// </summary>
        FVIRTKEY = 0x01,

        /// <summary>
        /// A menu item on the menu bar is not highlighted when an accelerator is used.
        /// This attribute is obsolete and retained only for backward compatibility with
        /// resource files designed for 16-bit Windows.
        /// </summary>
        FNOINVERT = 0x02,

        /// <summary>
        /// The accelerator is activated only if the user presses the SHIFT key. This flag
        /// applies only to virtual keys. 
        /// </summary>
        FSHIFT = 0x04,

        /// <summary>
        /// The accelerator is activated only if the user presses the CTRL key. This flag
        /// applies only to virtual keys. 
        /// </summary>
        FCONTROL = 0x08,

        /// <summary>
        /// The accelerator is activated only if the user presses the ALT key. This flag
        /// applies only to virtual keys. 
        /// </summary>
        FALT = 0x10,

        /// <summary>
        /// The entry is last in an accelerator table.
        /// </summary>
        LastEntry = 0x80,
    }

    public enum BaseRelocationTypes : ushort
    {
        /// <summary>
        /// The base relocation is skipped. This type can be used to pad a block.
        /// </summary>
        IMAGE_REL_BASED_ABSOLUTE = 0,

        /// <summary>
        /// The base relocation adds the high 16 bits of the difference to the 16-bit
        /// field at offset. The 16-bit field represents the high value of a 32-bit word.
        /// </summary>
        IMAGE_REL_BASED_HIGH = 1,

        /// <summary>
        /// The base relocation adds the low 16 bits of the difference to the 16-bit
        /// field at offset. The 16-bit field represents the low half of a 32-bit word.
        /// </summary>
        IMAGE_REL_BASED_LOW = 2,

        /// <summary>
        /// The base relocation applies all 32 bits of the difference to the 32-bit
        /// field at offset.
        /// </summary>
        IMAGE_REL_BASED_HIGHLOW = 3,

        /// <summary>
        /// The base relocation adds the high 16 bits of the difference to the 16-bit
        /// field at offset. The 16-bit field represents the high value of a 32-bit word.
        /// The low 16 bits of the 32-bit value are stored in the 16-bit word that follows
        /// this base relocation. This means that this base relocation occupies two slots.
        /// </summary>
        IMAGE_REL_BASED_HIGHADJ = 4,

        /// <summary>
        /// The relocation interpretation is dependent on the machine type.
        /// When the machine type is MIPS, the base relocation applies to a MIPS jump
        /// instruction.
        /// </summary>
        IMAGE_REL_BASED_MIPS_JMPADDR = 5,

        /// <summary>
        /// This relocation is meaningful only when the machine type is ARM or Thumb.
        /// The base relocation applies the 32-bit address of a symbol across a consecutive
        /// MOVW/MOVT instruction pair.
        /// </summary>
        IMAGE_REL_BASED_ARM_MOV32 = 5,

        /// <summary>
        /// This relocation is only meaningful when the machine type is RISC-V. The base
        /// relocation applies to the high 20 bits of a 32-bit absolute address.
        /// </summary>
        IMAGE_REL_BASED_RISCV_HIGH20 = 5,

        /// <summary>
        /// Reserved, must be zero.
        /// </summary>
        RESERVED6 = 6,

        /// <summary>
        /// This relocation is meaningful only when the machine type is Thumb. The base
        /// relocation applies the 32-bit address of a symbol to a consecutive MOVW/MOVT
        /// instruction pair.
        /// </summary>
        IMAGE_REL_BASED_THUMB_MOV32 = 7,

        /// <summary>
        /// This relocation is only meaningful when the machine type is RISC-V. The base
        /// relocation applies to the low 12 bits of a 32-bit absolute address formed in
        /// RISC-V I-type instruction format.
        /// </summary>
        IMAGE_REL_BASED_RISCV_LOW12I = 7,

        /// <summary>
        /// This relocation is only meaningful when the machine type is RISC-V. The base
        /// relocation applies to the low 12 bits of a 32-bit absolute address formed in
        /// RISC-V S-type instruction format.
        /// </summary>
        IMAGE_REL_BASED_RISCV_LOW12S = 8,

        /// <summary>
        /// This relocation is only meaningful when the machine type is LoongArch 32-bit.
        /// The base relocation applies to a 32-bit absolute address formed in two
        /// consecutive instructions.
        /// </summary>
        IMAGE_REL_BASED_LOONGARCH32_MARK_LA = 8,

        /// <summary>
        /// This relocation is only meaningful when the machine type is LoongArch 64-bit.
        /// The base relocation applies to a 64-bit absolute address formed in four
        /// consecutive instructions.
        /// </summary>
        IMAGE_REL_BASED_LOONGARCH64_MARK_LA = 8,

        /// <summary>
        /// The relocation is only meaningful when the machine type is MIPS. The base
        /// relocation applies to a MIPS16 jump instruction.
        /// </summary>
        IMAGE_REL_BASED_MIPS_JMPADDR16 = 9,

        /// <summary>
        /// The base relocation applies the difference to the 64-bit field at offset. 
        /// </summary>
        IMAGE_REL_BASED_DIR64 = 10,
    }

    public enum CallbackReason : ushort
    {
        /// <summary>
        /// A new process has started, including the first thread. 
        /// </summary>
        DLL_PROCESS_ATTACH = 1,

        /// <summary>
        /// A new thread has been created. This notification sent for
        /// all but the first thread. 
        /// </summary>
        DLL_THREAD_ATTACH = 2,

        /// <summary>
        /// A thread is about to be terminated. This notification sent
        /// for all but the first thread. 
        /// </summary>
        DLL_THREAD_DETACH = 3,

        /// <summary>
        /// A process is about to terminate, including the original thread.
        /// </summary>
        DLL_PROCESS_DETACH = 0,
    }

    public enum COMDATSelect : byte
    {
        /// <summary>
        /// If this symbol is already defined, the linker issues a "multiply
        /// defined symbol" error.
        /// </summary>
        IMAGE_COMDAT_SELECT_NODUPLICATES = 0x01,

        /// <summary>
        /// Any section that defines the same COMDAT symbol can be linked;
        /// the rest are removed.
        /// </summary>
        IMAGE_COMDAT_SELECT_ANY = 0x02,

        /// <summary>
        /// The linker chooses an arbitrary section among the definitions
        /// for this symbol. If all definitions are not the same size, a
        /// "multiply defined symbol" error is issued.
        /// </summary>
        IMAGE_COMDAT_SELECT_SAME_SIZE = 0x03,

        /// <summary>
        /// The linker chooses an arbitrary section among the definitions
        /// for this symbol. If all definitions do not match exactly, a
        /// "multiply defined symbol" error is issued.
        /// </summary>
        IMAGE_COMDAT_SELECT_EXACT_MATCH = 0x04,

        /// <summary>
        /// The section is linked if a certain other COMDAT section is linked.
        /// This other section is indicated by the Number field of the
        /// auxiliary symbol record for the section definition. This setting
        /// is useful for definitions that have components in multiple sections
        /// (for example, code in one and data in another), but where all must
        /// be linked or discarded as a set. The other section this section is
        /// associated with must be a COMDAT section, which can be another
        /// associative COMDAT section. An associative COMDAT section's section
        /// association chain can't form a loop. The section association chain
        /// must eventually come to a COMDAT section that doesn't have
        /// IMAGE_COMDAT_SELECT_ASSOCIATIVE set.
        /// </summary>
        IMAGE_COMDAT_SELECT_ASSOCIATIVE = 0x05,

        /// <summary>
        /// The linker chooses the largest definition from among all of the
        /// definitions for this symbol. If multiple definitions have this size,
        /// the choice between them is arbitrary.
        /// </summary>
        IMAGE_COMDAT_SELECT_LARGEST = 0x06,
    }

    public enum DebugType : uint
    {
        /// <summary>
        /// An unknown value that is ignored by all tools.
        /// </summary>
        IMAGE_DEBUG_TYPE_UNKNOWN = 0,

        /// <summary>
        /// The COFF debug information (line numbers, symbol table, and string table).
        /// This type of debug information is also pointed to by fields in the file
        /// headers.
        /// </summary>
        IMAGE_DEBUG_TYPE_COFF = 1,

        /// <summary>
        /// The Visual C++ debug information.
        /// </summary>
        IMAGE_DEBUG_TYPE_CODEVIEW = 2,

        /// <summary>
        /// The frame pointer omission (FPO) information. This information tells the
        /// debugger how to interpret nonstandard stack frames, which use the EBP
        /// register for a purpose other than as a frame pointer.
        /// </summary>
        IMAGE_DEBUG_TYPE_FPO = 3,

        /// <summary>
        /// The location of DBG file.
        /// </summary>
        IMAGE_DEBUG_TYPE_MISC = 4,

        /// <summary>
        /// A copy of .pdata section.
        /// </summary>
        IMAGE_DEBUG_TYPE_EXCEPTION = 5,

        /// <summary>
        /// Reserved.
        /// </summary>
        IMAGE_DEBUG_TYPE_FIXUP = 6,

        /// <summary>
        /// The mapping from an RVA in image to an RVA in source image.
        /// </summary>
        IMAGE_DEBUG_TYPE_OMAP_TO_SRC = 7,

        /// <summary>
        /// The mapping from an RVA in source image to an RVA in image.
        /// </summary>
        IMAGE_DEBUG_TYPE_OMAP_FROM_SRC = 8,

        /// <summary>
        /// Reserved for Borland.
        /// </summary>
        IMAGE_DEBUG_TYPE_BORLAND = 9,

        /// <summary>
        /// Reserved.
        /// </summary>
        IMAGE_DEBUG_TYPE_RESERVED10 = 10,

        /// <summary>
        /// Reserved.
        /// </summary>
        IMAGE_DEBUG_TYPE_CLSID = 11,

        /// <summary>
        /// PE determinism or reproducibility.
        /// </summary>
        IMAGE_DEBUG_TYPE_REPRO = 16,

        /// <summary>
        /// Extended DLL characteristics bits.
        /// </summary>
        IMAGE_DEBUG_TYPE_EX_DLLCHARACTERISTICS = 20,
    }

    public enum DialogItemTemplateOrdinal : ushort
    {
        Button = 0x0080,
        Edit = 0x0081,
        Static = 0x0082,
        ListBox = 0x0083,
        ScrollBar = 0x0084,
        ComboBox = 0x0085,
    }

    [Flags]
    public enum DllCharacteristics : ushort
    {
        /// <summary>
        /// Reserved, must be zero.
        /// </summary>
        RESERVED0 = 0x0001,

        /// <summary>
        /// Reserved, must be zero.
        /// </summary>
        RESERVED1 = 0x0002,

        /// <summary>
        /// Reserved, must be zero.
        /// </summary>
        RESERVED2 = 0x0004,

        /// <summary>
        /// Reserved, must be zero.
        /// </summary>
        RESERVED3 = 0x0008,

        /// <summary>
        /// Image can handle a high entropy 64-bit virtual address space.
        /// </summary>
        IMAGE_DLLCHARACTERISTICS_HIGH_ENTROPY_VA = 0x0020,

        /// <summary>
        /// DLL can be relocated at load time.
        /// </summary>
        IMAGE_DLLCHARACTERISTICS_DYNAMIC_BASE = 0x0040,

        /// <summary>
        /// Code Integrity checks are enforced.
        /// </summary>
        IMAGE_DLLCHARACTERISTICS_FORCE_INTEGRITY = 0x0080,

        /// <summary>
        /// Image is NX compatible.
        /// </summary>
        IMAGE_DLLCHARACTERISTICS_NX_COMPAT = 0x0100,

        /// <summary>
        /// Isolation aware, but do not isolate the image.
        /// </summary>
        IMAGE_DLLCHARACTERISTICS_NO_ISOLATION = 0x0200,

        /// <summary>
        /// Does not use structured exception (SE) handling.
        /// No SE handler may be called in this image.
        /// </summary>
        IMAGE_DLLCHARACTERISTICS_NO_SEH = 0x0400,

        /// <summary>
        /// Do not bind the image.
        /// </summary>
        IMAGE_DLLCHARACTERISTICS_NO_BIND = 0x0800,

        /// <summary>
        /// Image must execute in an AppContainer.
        /// </summary>
        IMAGE_DLLCHARACTERISTICS_APPCONTAINER = 0x1000,

        /// <summary>
        /// A WDM driver.
        /// </summary>
        IMAGE_DLLCHARACTERISTICS_WDM_DRIVER = 0x2000,

        /// <summary>
        /// Image supports Control Flow Guard.
        /// </summary>
        IMAGE_DLLCHARACTERISTICS_GUARD_CF = 0x4000,

        /// <summary>
        /// Terminal Server aware.
        /// </summary>
        IMAGE_DLLCHARACTERISTICS_TERMINAL_SERVER_AWARE = 0x8000,
    }

    [Flags]
    public enum ExtendedDllCharacteristics : ushort
    {
        /// <summary>
        /// Image is Control-flow Enforcement Technology (CET) Shadow Stack compatible
        /// </summary>
        IMAGE_DLLCHARACTERISTICS_EX_CET_COMPAT = 0x0001,

        /// <summary>
        /// All branch targets in all image code sections are annotated with forward-edge
        /// control flow integrity guard instructions such as x86 CET-Indirect Branch
        /// Tracking (IBT) or ARM Branch Target Identification (BTI) instructions.
        /// This bit is not used by Windows.
        /// </summary>
        IMAGE_DLLCHARACTERISTICS_EX_FORWARD_CFI_COMPAT = 0x0040,
    }

    [Flags]
    public enum ExtendedWindowStyles : uint
    {
        /// <summary>
        /// The window has generic left-aligned properties. This is the default.
        /// </summary>
        WS_EX_LEFT = 0x00000000,

        /// <summary>
        /// The window text is displayed using left-to-right reading-order properties.
        /// This is the default.
        /// </summary>
        WS_EX_LTRREADING = 0x00000000,

        /// <summary>
        /// The vertical scroll bar (if present) is to the right of the client area.
        /// This is the default.
        /// </summary>
        WS_EX_RIGHTSCROLLBAR = 0x00000000,

        /// <summary>
        /// The window has a double border; the window can, optionally, be created with
        /// a title bar by specifying the WS_CAPTION style in the dwStyle parameter.
        /// </summary>
        WS_EX_DLGMODALFRAME = 0x00000001,

        /// <summary>
        /// The child window created with this style does not send the WM_PARENTNOTIFY
        /// message to its parent window when it is created or destroyed.
        /// </summary>
        WS_EX_NOPARENTNOTIFY = 0x00000004,

        /// <summary>
        /// The window should be placed above all non-topmost windows and should stay above them,
        /// even when the window is deactivated. To add or remove this style, use the
        /// SetWindowPos function.
        /// </summary>
        WS_EX_TOPMOST = 0x00000008,

        /// <summary>
        /// The window accepts drag-drop files.
        /// </summary>
        WS_EX_ACCEPTFILES = 0x00000010,

        /// <summary>
        /// The window should not be painted until siblings beneath the window (that were created
        /// by the same thread) have been painted. The window appears transparent because the bits
        /// of underlying sibling windows have already been painted.
        /// 
        /// To achieve transparency without these restrictions, use the SetWindowRgn function.
        /// </summary>
        WS_EX_TRANSPARENT = 0x00000020,

        /// <summary>
        /// The window is a MDI child window.
        /// </summary>
        WS_EX_MDICHILD = 0x00000040,

        /// <summary>
        /// The window is intended to be used as a floating toolbar. A tool window has a title
        /// bar that is shorter than a normal title bar, and the window title is drawn using a
        /// smaller font. A tool window does not appear in the taskbar or in the dialog that
        /// appears when the user presses ALT+TAB. If a tool window has a system menu, its icon
        /// is not displayed on the title bar. However, you can display the system menu by
        /// right-clicking or by typing ALT+SPACE. 
        /// </summary>
        WS_EX_TOOLWINDOW = 0x00000080,

        /// <summary>
        /// The window has a border with a raised edge.
        /// </summary>
        WS_EX_WINDOWEDGE = 0x00000100,

        /// <summary>
        /// The window has a border with a sunken edge.
        /// </summary>
        WS_EX_CLIENTEDGE = 0x00000200,

        /// <summary>
        /// The title bar of the window includes a question mark. When the user clicks
        /// the question mark, the cursor changes to a question mark with a pointer. If
        /// the user then clicks a child window, the child receives a WM_HELP message.
        /// The child window should pass the message to the parent window procedure,
        /// which should call the WinHelp function using the HELP_WM_HELP command. The
        /// Help application displays a pop-up window that typically contains help for
        /// the child window.
        /// 
        /// WS_EX_CONTEXTHELP cannot be used with the WS_MAXIMIZEBOX or WS_MINIMIZEBOX
        /// styles.
        /// </summary>
        WS_EX_CONTEXTHELP = 0x00000400,

        /// <summary>
        /// The window has generic "right-aligned" properties. This depends on the window class.
        /// This style has an effect only if the shell language is Hebrew, Arabic, or another
        /// language that supports reading-order alignment; otherwise, the style is ignored.
        /// 
        /// Using the WS_EX_RIGHT style for static or edit controls has the same effect as using
        /// the SS_RIGHT or ES_RIGHT style, respectively. Using this style with button controls
        /// has the same effect as using BS_RIGHT and BS_RIGHTBUTTON styles. 
        /// </summary>
        WS_EX_RIGHT = 0x00001000,

        /// <summary>
        /// If the shell language is Hebrew, Arabic, or another language that supports reading-order
        /// alignment, the window text is displayed using right-to-left reading-order properties.
        /// For other languages, the style is ignored.
        /// </summary>
        WS_EX_RTLREADING = 0x00002000,

        /// <summary>
        /// If the shell language is Hebrew, Arabic, or another language that supports
        /// reading order alignment, the vertical scroll bar (if present) is to the left
        /// of the client area. For other languages, the style is ignored.
        /// </summary>
        WS_EX_LEFTSCROLLBAR = 0x00004000,

        /// <summary>
        /// The window itself contains child windows that should take part in dialog box
        /// navigation. If this style is specified, the dialog manager recurses into
        /// children of this window when performing navigation operations such as handling
        /// the TAB key, an arrow key, or a keyboard mnemonic.
        /// </summary>
        WS_EX_CONTROLPARENT = 0x00010000,

        /// <summary>
        /// The window has a three-dimensional border style intended to be used for items that do
        /// not accept user input.
        /// </summary>
        WS_EX_STATICEDGE = 0x00020000,

        /// <summary>
        /// Forces a top-level window onto the taskbar when the window is visible.
        /// </summary>
        WS_EX_APPWINDOW = 0x00040000,

        /// <summary>
        /// The window is a layered window. This style cannot be used if the window has a
        /// class style of either CS_OWNDC or CS_CLASSDC.
        /// 
        /// Windows 8: The WS_EX_LAYERED style is supported for top-level windows and child
        /// windows. Previous Windows versions support WS_EX_LAYERED only for top-level windows.
        /// </summary>
        WS_EX_LAYERED = 0x00080000,

        /// <summary>
        /// The window does not pass its window layout to its child windows.
        /// </summary>
        WS_EX_NOINHERITLAYOUT = 0x00100000,

        /// <summary>
        /// The window does not render to a redirection surface. This is for windows that do not
        /// have visible content or that use mechanisms other than surfaces to provide their visual.
        /// </summary>
        WS_EX_NOREDIRECTIONBITMAP = 0x00200000,

        /// <summary>
        /// If the shell language is Hebrew, Arabic, or another language that supports reading
        /// order alignment, the horizontal origin of the window is on the right edge.
        /// Increasing horizontal values advance to the left.
        /// </summary>
        WS_EX_LAYOUTRTL = 0x00400000,

        /// <summary>
        /// Paints all descendants of a window in bottom-to-top painting order using
        /// double-buffering. Bottom-to-top painting order allows a descendent window
        /// to have translucency (alpha) and transparency (color-key) effects, but only
        /// if the descendent window also has the WS_EX_TRANSPARENT bit set.
        /// Double-buffering allows the window and its descendents to be painted without
        /// flicker. This cannot be used if the window has a class style of either
        /// CS_OWNDC or CS_CLASSDC.
        /// 
        /// Windows 2000: This style is not supported.
        /// </summary>
        WS_EX_COMPOSITED = 0x02000000,

        /// <summary>
        /// A top-level window created with this style does not become the foreground window when
        /// the user clicks it. The system does not bring this window to the foreground when the
        /// user minimizes or closes the foreground window.
        /// 
        /// The window should not be activated through programmatic access or via keyboard
        /// navigation by accessible technology, such as Narrator.
        /// 
        /// To activate the window, use the SetActiveWindow or SetForegroundWindow function.
        /// 
        /// The window does not appear on the taskbar by default. To force the window to appear on
        /// the taskbar, use the WS_EX_APPWINDOW style.
        /// </summary>
        WS_EX_NOACTIVATE = 0x08000000,

        /// <summary>
        /// The window is an overlapped window.
        /// </summary>
        WS_EX_OVERLAPPEDWINDOW = WS_EX_WINDOWEDGE | WS_EX_CLIENTEDGE,

        /// <summary>
        /// The window is palette window, which is a modeless dialog box that presents an array of
        /// commands.
        /// </summary>
        WS_EX_PALETTEWINDOW = WS_EX_WINDOWEDGE | WS_EX_TOOLWINDOW | WS_EX_TOPMOST,
    }

    public enum FixedFileInfoFileSubtype : uint
    {
        /// <summary>
        /// The driver type is unknown by the system.
        /// The font type is unknown by the system.
        /// </summary>
        VFT2_UNKNOWN = 0x00000000,

        #region VFT_DRV

        /// <summary>
        /// The file contains a printer driver.
        /// </summary>
        VFT2_DRV_PRINTER = 0x00000001,

        /// <summary>
        /// The file contains a keyboard driver.
        /// </summary>
        VFT2_DRV_KEYBOARD = 0x00000002,

        /// <summary>
        /// The file contains a language driver.
        /// </summary>
        VFT2_DRV_LANGUAGE = 0x00000003,

        /// <summary>
        /// The file contains a display driver.
        /// </summary>
        VFT2_DRV_DISPLAY = 0x00000004,

        /// <summary>
        /// The file contains a mouse driver.
        /// </summary>
        VFT2_DRV_MOUSE = 0x00000005,

        /// <summary>
        /// The file contains a network driver.
        /// </summary>
        VFT2_DRV_NETWORK = 0x00000006,

        /// <summary>
        /// The file contains a system driver.
        /// </summary>
        VFT2_DRV_SYSTEM = 0x00000007,

        /// <summary>
        /// The file contains an installable driver.
        /// </summary>
        VFT2_DRV_INSTALLABLE = 0x00000008,

        /// <summary>
        /// The file contains a sound driver.
        /// </summary>
        VFT2_DRV_SOUND = 0x00000009,

        /// <summary>
        /// The file contains a communications driver.
        /// </summary>
        VFT2_DRV_COMM = 0x0000000A,

        /// <summary>
        /// The file contains a versioned printer driver.
        /// </summary>
        VFT2_DRV_VERSIONED_PRINTER = 0x0000000C,

        #endregion

        #region VFT_FONT

        /// <summary>
        /// The file contains a raster font.
        /// </summary>
        VFT2_FONT_RASTER = 0x00000001,

        /// <summary>
        /// The file contains a vector font.
        /// </summary>
        VFT2_FONT_VECTOR = 0x00000002,

        /// <summary>
        /// The file contains a TrueType font.
        /// </summary>
        VFT2_FONT_TRUETYPE = 0x00000003,

        #endregion
    }

    public enum FixedFileInfoFileType : uint
    {
        /// <summary>
        /// The file type is unknown to the system.
        /// </summary>
        VFT_UNKNOWN = 0x00000000,

        /// <summary>
        /// The file contains an application.
        /// </summary>
        VFT_APP = 0x00000001,

        /// <summary>
        /// The file contains a DLL.
        /// </summary>
        VFT_DLL = 0x00000002,

        /// <summary>
        /// The file contains a device driver. If FileType is VFT_DRV, FileSubtype
        /// contains a more specific description of the driver.
        /// </summary>
        VFT_DRV = 0x00000003,

        /// <summary>
        /// The file contains a font. If FileType is VFT_FONT, FileSubtype contains
        /// a more specific description of the font file.
        /// </summary>
        VFT_FONT = 0x00000004,

        /// <summary>
        /// The file contains a virtual device.
        /// </summary>
        VFT_VXD = 0x00000005,

        /// <summary>
        /// The file contains a static-link library.
        /// </summary>
        VFT_STATIC_LIB = 0x00000007,
    }

    [Flags]
    public enum FixedFileInfoFlags : uint
    {
        /// <summary>
        /// The file contains debugging information or is compiled with debugging
        /// features enabled.
        /// </summary>
        VS_FF_DEBUG = 0x00000001,

        /// <summary>
        /// The file is a development version, not a commercially released product.
        /// </summary>
        VS_FF_PRERELEASE = 0x00000002,

        /// <summary>
        /// The file has been modified and is not identical to the original shipping
        /// file of the same version number.
        /// </summary>
        VS_FF_PATCHED = 0x00000004,

        /// <summary>
        /// The file was not built using standard release procedures. If this flag is
        /// set, the StringFileInfo structure should contain a PrivateBuild entry.
        /// </summary>
        VS_FF_PRIVATEBUILD = 0x00000008,

        /// <summary>
        /// The file's version structure was created dynamically; therefore, some
        /// of the members in this structure may be empty or incorrect. This flag
        /// should never be set in a file's VS_VERSIONINFO data. 
        /// </summary>
        VS_FF_INFOINFERRED = 0x00000010,

        /// <summary>
        /// The file was built by the original company using standard release
        /// procedures but is a variation of the normal file of the same version number.
        /// If this flag is set, the StringFileInfo structure should contain a SpecialBuild
        /// entry.
        /// </summary>
        VS_FF_SPECIALBUILD = 0x00000020,
    }

    [Flags]
    public enum FixedFileInfoOS : uint
    {
        /// <summary>
        /// The operating system for which the file was designed is
        /// unknown to the system.
        /// </summary>
        VOS_UNKNOWN = 0x00000000,

        /// <summary>
        /// The file was designed for 16-bit Windows.
        /// </summary>
        VOS__WINDOWS16 = 0x00000001,

        /// <summary>
        /// The file was designed for 16-bit Presentation Manager.
        /// </summary>
        VOS__PM16 = 0x00000002,

        /// <summary>
        /// The file was designed for 32-bit Presentation Manager.
        /// </summary>
        VOS__PM32 = 0x00000003,

        /// <summary>
        /// The file was designed for 32-bit Windows.
        /// </summary>
        VOS__WINDOWS32 = 0x00000004,

        /// <summary>
        /// The file was designed for MS-DOS.
        /// </summary>
        VOS_DOS = 0x00010000,

        /// <summary>
        /// The file was designed for 16-bit OS/2.
        /// </summary>
        VOS_OS216 = 0x00020000,

        /// <summary>
        /// The file was designed for 32-bit OS/2.
        /// </summary>
        VOS_OS232 = 0x00030000,

        /// <summary>
        /// The file was designed for Windows NT.
        /// </summary>
        VOS_NT = 0x00040000,
    }

    [Flags]
    public enum GuardFlags : uint
    {
        /// <summary>
        /// Module performs control flow integrity checks using
        /// system-supplied support.
        /// </summary>
        IMAGE_GUARD_CF_INSTRUMENTED = 0x00000100,

        /// <summary>
        /// Module performs control flow and write integrity checks.
        /// </summary>
        IMAGE_GUARD_CFW_INSTRUMENTED = 0x00000200,

        /// <summary>
        /// Module contains valid control flow target metadata.
        /// </summary>
        IMAGE_GUARD_CF_FUNCTION_TABLE_PRESENT = 0x00000400,

        /// <summary>
        /// Module does not make use of the /GS security cookie.
        /// </summary>
        IMAGE_GUARD_SECURITY_COOKIE_UNUSED = 0x00000800,

        /// <summary>
        /// Module supports read only delay load IAT.
        /// </summary>
        IMAGE_GUARD_PROTECT_DELAYLOAD_IAT = 0x00001000,

        /// <summary>
        /// Delayload import table in its own .didat section (with
        /// nothing else in it) that can be freely reprotected.
        /// </summary>
        IMAGE_GUARD_DELAYLOAD_IAT_IN_ITS_OWN_SECTION = 0x00002000,

        /// <summary>
        /// Module contains suppressed export information. This also
        /// infers that the address taken IAT table is also present
        /// in the load config.
        /// </summary>
        IMAGE_GUARD_CF_EXPORT_SUPPRESSION_INFO_PRESENT = 0x00004000,

        /// <summary>
        /// Module enables suppression of exports.
        /// </summary>
        IMAGE_GUARD_CF_ENABLE_EXPORT_SUPPRESSION = 0x00008000,

        /// <summary>
        /// Module contains longjmp target information.
        /// </summary>
        IMAGE_GUARD_CF_LONGJUMP_TABLE_PRESENT = 0x00010000,

        /// <summary>
        /// Mask for the subfield that contains the stride of Control
        /// Flow Guard function table entries (that is, the additional
        /// count of bytes per table entry).
        /// </summary>
        IMAGE_GUARD_CF_FUNCTION_TABLE_SIZE_MASK = 0xF0000000,

        /// <summary>
        /// Additionally, the Windows SDK winnt.h header defines this
        /// macro for the amount of bits to right-shift the GuardFlags
        /// value to right-justify the Control Flow Guard function table
        /// stride:
        /// </summary>
        IMAGE_GUARD_CF_FUNCTION_TABLE_SIZE_SHIFT = 28,
    }

    public enum ImportType : ushort
    {
        /// <summary>
        /// Executable code.
        /// </summary>
        IMPORT_CODE = 0,

        /// <summary>
        /// Data.
        /// </summary>
        IMPORT_DATA = 1,

        /// <summary>
        /// Specified as CONST in the .def file.
        /// </summary>
        IMPORT_CONST = 2,
    }

    // Actually 3 bits
    public enum ImportNameType : ushort
    {
        /// <summary>
        /// The import is by ordinal. This indicates that the value in the
        /// Ordinal/Hint field of the import header is the import's ordinal.
        /// If this constant is not specified, then the Ordinal/Hint field
        /// should always be interpreted as the import's hint.
        /// </summary>
        IMPORT_ORDINAL = 0,

        /// <summary>
        /// The import name is identical to the public symbol name.
        /// </summary>
        IMPORT_NAME = 1,

        /// <summary>
        /// The import name is the public symbol name, but skipping the leading
        /// ?, @, or optionally _.
        /// </summary>
        IMPORT_NAME_NOPREFIX = 2,

        /// <summary>
        /// The import name is the public symbol name, but skipping the leading
        /// ?, @, or optionally _, and truncating at the first @.
        /// </summary>
        IMPORT_NAME_UNDECORATE = 3,
    }

    [Flags]
    public enum MemoryFlags : ushort
    {
        // TODO: Validate the ~ statements
        MOVEABLE = 0x0010,
        FIXED = 0xFFEF, // ~MOVEABLE

        PURE = 0x0020,
        IMPURE = 0xFFDF, // ~PURE

        PRELOAD = 0x0040,
        LOADONCALL = 0xFFBF, // ~PRELOAD

        DISCARDABLE = 0x1000,
    }

    [Flags]
    public enum MenuFlags : uint
    {
        MF_INSERT = 0x00000000,
        MF_CHANGE = 0x00000080,
        MF_APPEND = 0x00000100,
        MF_DELETE = 0x00000200,
        MF_REMOVE = 0x00001000,

        MF_BYCOMMAND = 0x00000000,
        MF_BYPOSITION = 0x00000400,

        MF_SEPARATOR = 0x00000800,

        MF_ENABLED = 0x00000000,
        MF_GRAYED = 0x00000001,
        MF_DISABLED = 0x00000002,

        MF_UNCHECKED = 0x00000000,
        MF_CHECKED = 0x00000008,
        MF_USECHECKBITMAPS = 0x00000200,

        MF_STRING = 0x00000000,
        MF_BITMAP = 0x00000004,
        MF_OWNERDRAW = 0x00000100,

        MF_POPUP = 0x00000010,
        MF_MENUBARBREAK = 0x00000020,
        MF_MENUBREAK = 0x00000040,

        MF_UNHILITE = 0x00000000,
        MF_HILITE = 0x00000080,

        MF_DEFAULT = 0x00001000,
        MF_SYSMENU = 0x00002000,
        MF_HELP = 0x00004000,
        MF_RIGHTJUSTIFY = 0x00004000,

        MF_MOUSESELECT = 0x00008000,
        MF_END = 0x00000080,

        MFT_STRING = MF_STRING,
        MFT_BITMAP = MF_BITMAP,
        MFT_MENUBARBREAK = MF_MENUBARBREAK,
        MFT_MENUBREAK = MF_MENUBREAK,
        MFT_OWNERDRAW = MF_OWNERDRAW,
        MFT_RADIOCHECK = 0x00000200,
        MFT_SEPARATOR = MF_SEPARATOR,
        MFT_RIGHTORDER = 0x00002000,
        MFT_RIGHTJUSTIFY = MF_RIGHTJUSTIFY,

        MFS_GRAYED = 0x00000003,
        MFS_DISABLED = MFS_GRAYED,
        MFS_CHECKED = MF_CHECKED,
        MFS_HILITE = MF_HILITE,
        MFS_ENABLED = MF_ENABLED,
        MFS_UNCHECKED = MF_UNCHECKED,
        MFS_UNHILITE = MF_UNHILITE,
        MFS_DEFAULT = MF_DEFAULT,
    }

    public enum ResourceType : uint
    {
        RT_NEWRESOURCE = 0x2000,
        RT_ERROR = 0x7FFF,

        /// <summary>
        /// Hardware-dependent cursor resource.
        /// </summary>
        RT_CURSOR = 1,

        /// <summary>
        /// Bitmap resource.
        /// </summary>
        RT_BITMAP = 2,

        /// <summary>
        /// Hardware-dependent icon resource.
        /// </summary>
        RT_ICON = 3,

        /// <summary>
        /// Menu resource.
        /// </summary>
        RT_MENU = 4,

        /// <summary>
        /// Dialog box.
        /// </summary>
        RT_DIALOG = 5,

        /// <summary>
        /// String-table entry.
        /// </summary>
        RT_STRING = 6,

        /// <summary>
        /// Font directory resource.
        /// </summary>
        RT_FONTDIR = 7,

        /// <summary>
        /// Font resource.
        /// </summary>
        RT_FONT = 8,

        /// <summary>
        /// Accelerator table.
        /// </summary>
        RT_ACCELERATOR = 9,

        /// <summary>
        /// Application-defined resource (raw data).
        /// </summary>
        RT_RCDATA = 10,

        /// <summary>
        /// Message-table entry.
        /// </summary>
        RT_MESSAGETABLE = 11,

        /// <summary>
        /// Hardware-independent cursor resource.
        /// </summary>
        RT_GROUP_CURSOR = RT_CURSOR + 11,

        /// <summary>
        /// Hardware-independent icon resource.
        /// </summary>
        RT_GROUP_ICON = RT_ICON + 11,

        /// <summary>
        /// Version resource.
        /// </summary>
        RT_VERSION = 16,

        /// <summary>
        /// Allows a resource editing tool to associate a string with an .rc file.
        /// Typically, the string is the name of the header file that provides symbolic
        /// names. The resource compiler parses the string but otherwise ignores the
        /// value. For example, `1 DLGINCLUDE "MyFile.h"`
        /// </summary>
        RT_DLGINCLUDE = 17,

        /// <summary>
        /// Plug and Play resource.
        /// </summary>
        RT_PLUGPLAY = 19,

        /// <summary>
        /// VXD.
        /// </summary>
        RT_VXD = 20,

        /// <summary>
        /// Animated cursor.
        /// </summary>
        RT_ANICURSOR = 21,

        /// <summary>
        /// Animated icon.
        /// </summary>
        RT_ANIICON = 22,

        /// <summary>
        /// HTML resource.
        /// </summary>
        RT_HTML = 23,

        /// <summary>
        /// Side-by-Side Assembly Manifest.
        /// </summary>
        RT_MANIFEST = 24,

        RT_NEWBITMAP = (RT_BITMAP | RT_NEWRESOURCE),
        RT_NEWMENU = (RT_MENU | RT_NEWRESOURCE),
        RT_NEWDIALOG = (RT_DIALOG | RT_NEWRESOURCE),
    }

    public enum SymbolDerivedType : byte
    {
        /// <summary>
        /// No derived type; the symbol is a simple scalar variable.
        /// </summary>
        IMAGE_SYM_DTYPE_NULL = 0x00,

        /// <summary>
        /// The symbol is a pointer to base type.
        /// </summary>
        IMAGE_SYM_DTYPE_POINTER = 0x01,

        /// <summary>
        /// The symbol is a function that returns a base type.
        /// </summary>
        IMAGE_SYM_DTYPE_FUNCTION = 0x02,

        /// <summary>
        /// The symbol is an array of base type.
        /// </summary>
        IMAGE_SYM_DTYPE_ARRAY = 0x03,
    }

    public enum VersionResourceType : ushort
    {
        BinaryData = 0,
        TextData = 1,
    }

    [Flags]
    public enum WindowStyles : uint
    {
        #region Standard Styles

        /// <summary>
        /// The window is an overlapped window. An overlapped window has a title
        /// bar and a border. Same as the WS_TILED style.
        /// </summary>
        WS_OVERLAPPED = 0x00000000,

        /// <summary>
        /// The window is an overlapped window. An overlapped window has a title bar
        /// and a border. Same as the WS_OVERLAPPED style.
        /// </summary>
        WS_TILED = 0x00000000,

        /// <summary>
        /// The window has a maximize button. Cannot be combined with the
        /// WS_EX_CONTEXTHELP style. The WS_SYSMENU style must also be specified.
        /// </summary>
        WS_MAXIMIZEBOX = 0x00010000,

        /// <summary>
        /// The window is a control that can receive the keyboard focus when the user
        /// presses the TAB key. Pressing the TAB key changes the keyboard focus to
        /// the next control with the WS_TABSTOP style.
        /// 
        /// You can turn this style on and off to change dialog box navigation. To
        /// change this style after a window has been created, use the SetWindowLong
        /// function. For user-created windows and modeless dialogs to work with tab
        /// stops, alter the message loop to call the IsDialogMessage function.
        /// </summary>
        WS_TABSTOP = 0x00010000,

        /// <summary>
        /// The window has a minimize button. Cannot be combined with the
        /// WS_EX_CONTEXTHELP style. The WS_SYSMENU style must also be specified.
        /// </summary>
        WS_MINIMIZEBOX = 0x00020000,

        /// <summary>
        /// The window is the first control of a group of controls. The group consists
        /// of this first control and all controls defined after it, up to the next
        /// control with the WS_GROUP style. The first control in each group usually
        /// has the WS_TABSTOP style so that the user can move from group to group.
        /// The user can subsequently change the keyboard focus from one control in
        /// the group to the next control in the group by using the direction keys.
        /// 
        /// You can turn this style on and off to change dialog box navigation. To
        /// change this style after a window has been created, use the SetWindowLong
        /// function.
        /// </summary>
        WS_GROUP = 0x00020000,

        /// <summary>
        /// The window has a sizing border. Same as the WS_THICKFRAME style.
        /// </summary>
        WS_SIZEBOX = 0x00040000,

        /// <summary>
        /// The window has a sizing border. Same as the WS_SIZEBOX style.
        /// </summary>
        WS_THICKFRAME = 0x00040000,

        /// <summary>
        /// The window has a window menu on its title bar. The WS_CAPTION style must
        /// also be specified.
        /// </summary>
        WS_SYSMENU = 0x00080000,

        /// <summary>
        /// The window has a horizontal scroll bar.
        /// </summary>
        WS_HSCROLL = 0x00100000,

        /// <summary>
        /// The window has a vertical scroll bar.
        /// </summary>
        WS_VSCROLL = 0x00200000,

        /// <summary>
        /// The window has a border of a style typically used with dialog boxes. A
        /// window with this style cannot have a title bar.
        /// </summary>
        WS_DLGFRAME = 0x00400000,

        /// <summary>
        /// The window has a thin-line border
        /// </summary>
        WS_BORDER = 0x00800000,

        /// <summary>
        /// The window has a title bar
        /// </summary>
        WS_CAPTION = 0x00C00000,

        /// <summary>
        /// The window is initially maximized.
        /// </summary>
        WS_MAXIMIZE = 0x01000000,

        /// <summary>
        /// Excludes the area occupied by child windows when drawing occurs within the
        /// parent window. This style is used when creating the parent window.
        /// </summary>
        WS_CLIPCHILDREN = 0x02000000,

        /// <summary>
        /// Clips child windows relative to each other; that is, when a particular child
        /// window receives a WM_PAINT message, the WS_CLIPSIBLINGS style clips all other
        /// overlapping child windows out of the region of the child window to be updated.
        /// If WS_CLIPSIBLINGS is not specified and child windows overlap, it is possible,
        /// when drawing within the client area of a child window, to draw within the
        /// client area of a neighboring child window.
        /// </summary>
        WS_CLIPSIBLINGS = 0x04000000,

        /// <summary>
        /// The window is initially disabled. A disabled window cannot receive input from
        /// the user. To change this after a window has been created, use the EnableWindow
        /// function.
        /// </summary>
        WS_DISABLED = 0x08000000,

        /// <summary>
        /// The window is initially visible.
        /// This style can be turned on and off by using the ShowWindow or SetWindowPos
        /// function.
        /// </summary>
        WS_VISIBLE = 0x10000000,

        /// <summary>
        /// The window is initially minimized. Same as the WS_MINIMIZE style.
        /// </summary>
        WS_ICONIC = 0x20000000,

        /// <summary>
        /// The window is initially minimized. Same as the WS_ICONIC style.
        /// </summary>
        WS_MINIMIZE = 0x20000000,

        /// <summary>
        /// The window is a child window. A window with this style cannot have a menu
        /// bar. This style cannot be used with the WS_POPUP style.
        /// </summary>
        WS_CHILD = 0x40000000,

        /// <summary>
        /// Same as the WS_CHILD style.
        /// </summary>
        WS_CHILDWINDOW = 0x40000000,

        /// <summary>
        /// The window is a pop-up window. This style cannot be used with the WS_CHILD style.
        /// </summary>
        WS_POPUP = 0x80000000,

        /// <summary>
        /// The window is an overlapped window. Same as the WS_TILEDWINDOW style.
        /// </summary>
        WS_OVERLAPPEDWINDOW = WS_OVERLAPPED | WS_CAPTION | WS_SYSMENU | WS_THICKFRAME | WS_MINIMIZEBOX | WS_MAXIMIZEBOX,

        /// <summary>
        /// The window is a pop-up window. The WS_CAPTION and WS_POPUPWINDOW styles must be
        /// combined to make the window menu visible.
        /// </summary>
        WS_POPUPWINDOW = WS_POPUP | WS_BORDER | WS_SYSMENU,

        /// <summary>
        /// The window is an overlapped window. Same as the WS_OVERLAPPEDWINDOW style.
        /// </summary>
        WS_TILEDWINDOW = WS_OVERLAPPED | WS_CAPTION | WS_SYSMENU | WS_THICKFRAME | WS_MINIMIZEBOX | WS_MAXIMIZEBOX,

        #endregion

        #region Common Control Styles

        /// <summary>
        /// Causes the control to position itself at the top of the parent window's
        /// client area and sets the width to be the same as the parent window's width.
        /// Toolbars have this style by default. 
        /// </summary>
        CCS_TOP = 0x00000001,

        /// <summary>
        /// Causes the control to resize and move itself horizontally, but not vertically,
        /// in response to a WM_SIZE message. If CCS_NORESIZE is used, this style does not
        /// apply. Header windows have this style by default.
        /// </summary>
        CCS_NOMOVEY = 0x00000002,

        /// <summary>
        /// Causes the control to position itself at the bottom of the parent window's
        /// client area and sets the width to be the same as the parent window's width.
        /// Status windows have this style by default.
        /// </summary>
        CCS_BOTTOM = 0x00000003,

        /// <summary>
        /// Prevents the control from using the default width and height when setting its
        /// initial size or a new size. Instead, the control uses the width and height
        /// specified in the request for creation or sizing.
        /// </summary>
        CCS_NORESIZE = 0x00000004,

        /// <summary>
        /// Prevents the control from automatically moving to the top or bottom of the parent
        /// window. Instead, the control keeps its position within the parent window despite
        /// changes to the size of the parent. If CCS_TOP or CCS_BOTTOM is also used, the
        /// height is adjusted to the default, but the position and width remain unchanged. 
        /// </summary>
        CCS_NOPARENTALIGN = 0x00000008,

        /// <summary>
        /// Enables a toolbar's built-in customization features, which let the user to drag a
        /// button to a new position or to remove a button by dragging it off the toolbar.
        /// In addition, the user can double-click the toolbar to display the Customize Toolbar
        /// dialog box, which enables the user to add, delete, and rearrange toolbar buttons.
        /// </summary>
        CCS_ADJUSTABLE = 0x00000020,

        /// <summary>
        /// Prevents a two-pixel highlight from being drawn at the top of the control.
        /// </summary>
        CCS_NODIVIDER = 0x00000040,

        /// <summary>
        /// Version 4.70. Causes the control to be displayed vertically.
        /// </summary>
        CCS_VERT = 0x00000080,

        /// <summary>
        /// Version 4.70. Causes the control to be displayed vertically on the left side of the
        /// parent window.
        /// </summary>
        CCS_LEFT = CCS_VERT | CCS_TOP,

        /// <summary>
        /// Version 4.70. Causes the control to be displayed vertically on the right side of the
        /// parent window.
        /// </summary>
        CCS_RIGHT = CCS_VERT | CCS_BOTTOM,

        /// <summary>
        /// Version 4.70. Causes the control to resize and move itself vertically, but not
        /// horizontally, in response to a WM_SIZE message. If CCS_NORESIZE is used, this style
        /// does not apply.
        /// </summary>
        CCS_NOMOVEX = CCS_VERT | CCS_NOMOVEY,

        #endregion

        #region Dialog Box Styles

        /// <summary>
        /// Indicates that the coordinates of the dialog box are screen coordinates.
        /// If this style is not specified, the coordinates are client coordinates.
        /// </summary>
        DS_ABSALIGN = 0x00000001,

        /// <summary>
        /// This style is obsolete and is included for compatibility with 16-bit versions
        /// of Windows. If you specify this style, the system creates the dialog box with
        /// the WS_EX_TOPMOST style. This style does not prevent the user from accessing
        /// other windows on the desktop.
        /// 
        /// Do not combine this style with the DS_CONTROL style.
        /// </summary>
        DS_SYSMODAL = 0x00000002,

        /// <summary>
        /// Obsolete. The system automatically applies the three-dimensional look to dialog
        /// boxes created by applications.
        /// </summary>
        DS_3DLOOK = 0x00000004,

        /// <summary>
        /// Causes the dialog box to use the SYSTEM_FIXED_FONT instead of the default
        /// SYSTEM_FONT. This is a monospace font compatible with the System font in 16-bit
        /// versions of Windows earlier than 3.0.
        /// </summary>
        DS_FIXEDSYS = 0x00000008,

        /// <summary>
        /// Creates the dialog box even if errors occur for example, if a child window cannot
        /// be created or if the system cannot create a special data segment for an edit control.
        /// </summary>
        DS_NOFAILCREATE = 0x00000010,

        /// <summary>
        /// Applies to 16-bit applications only. This style directs edit controls in the
        /// dialog box to allocate memory from the application's data segment. Otherwise,
        /// edit controls allocate storage from a global memory object.
        /// </summary>
        DS_LOCALEDIT = 0x00000020,

        /// <summary>
        /// Indicates that the header of the dialog box template (either standard or extended)
        /// contains additional data specifying the font to use for text in the client area
        /// and controls of the dialog box. If possible, the system selects a font according
        /// to the specified font data. The system passes a handle to the font to the dialog
        /// box and to each control by sending them the WM_SETFONT message. For descriptions
        /// of the format of this font data, see DLGTEMPLATE and DLGTEMPLATEEX.
        /// 
        /// If neither DS_SETFONT nor DS_SHELLFONT is specified, the dialog box template does
        /// not include the font data.
        /// </summary>
        DS_SETFONT = 0x00000040,

        /// <summary>
        /// Creates a dialog box with a modal dialog-box frame that can be combined with a
        /// title bar and window menu by specifying the WS_CAPTION and WS_SYSMENU styles.
        /// </summary>
        DS_MODALFRAME = 0x00000080,

        /// <summary>
        /// Suppresses WM_ENTERIDLE messages that the system would otherwise send to the owner
        /// of the dialog box while the dialog box is displayed.
        /// </summary>
        DS_NOIDLEMSG = 0x00000100,

        /// <summary>
        /// Causes the system to use the SetForegroundWindow function to bring the dialog box
        /// to the foreground. This style is useful for modal dialog boxes that require immediate
        /// attention from the user regardless of whether the owner window is the foreground
        /// window.
        /// 
        /// The system restricts which processes can set the foreground window. For more
        /// information, see Foreground and Background Windows.
        /// </summary>
        DS_SETFOREGROUND = 0x00000200,

        /// <summary>
        /// Creates a dialog box that works well as a child window of another dialog box, much like
        /// a page in a property sheet. This style allows the user to tab among the control windows
        /// of a child dialog box, use its accelerator keys, and so on.
        /// </summary>
        DS_CONTROL = 0x00000400,

        /// <summary>
        /// Centers the dialog box in the working area of the monitor that contains the owner window.
        /// If no owner window is specified, the dialog box is centered in the working area of a
        /// monitor determined by the system. The working area is the area not obscured by the taskbar
        /// or any appbars.
        /// </summary>
        DS_CENTER = 0x00000800,

        /// <summary>
        /// Centers the dialog box on the mouse cursor.
        /// </summary>
        DS_CENTERMOUSE = 0x00001000,

        /// <summary>
        /// Includes a question mark in the title bar of the dialog box. When the user clicks the
        /// question mark, the cursor changes to a question mark with a pointer. If the user then clicks
        /// a control in the dialog box, the control receives a WM_HELP message. The control should pass
        /// the message to the dialog box procedure, which should call the function using the
        /// HELP_WM_HELP command. The help application displays a pop-up window that typically contains
        /// help for the control.
        /// 
        /// Note that DS_CONTEXTHELP is only a placeholder. When the dialog box is created, the system
        /// checks for DS_CONTEXTHELP and, if it is there, adds WS_EX_CONTEXTHELP to the extended style
        /// of the dialog box. WS_EX_CONTEXTHELP cannot be used with the WS_MAXIMIZEBOX or WS_MINIMIZEBOX
        /// styles.
        /// </summary>
        DS_CONTEXTHELP = 0x00002000,

        /// <remarks>
        /// Windows CE Version 5.0 and later
        /// </remarks>
        DS_USEPIXELS = 0x00008000,

        /// <summary>
        /// Indicates that the dialog box should use the system font. The typeface member of the extended
        /// dialog box template must be set to MS Shell Dlg. Otherwise, this style has no effect. It is
        /// also recommended that you use the DIALOGEX Resource, rather than the DIALOG Resource. For
        /// more information, see Dialog Box Fonts.
        /// 
        /// The system selects a font using the font data specified in the pointsize, weight, and italic
        /// members. The system passes a handle to the font to the dialog box and to each control by
        /// sending them the WM_SETFONT message. For descriptions of the format of this font data, see
        /// DLGTEMPLATEEX. 
        /// 
        /// If neither DS_SHELLFONT nor DS_SETFONT is specified, the extended dialog box template does
        /// not include the font data.
        /// </summary>
        DS_SHELLFONT = DS_SETFONT | DS_FIXEDSYS,

        #endregion
    }

    public enum WindowsCertificateRevision : ushort
    {
        /// <summary>
        /// Version 1, legacy version of the Win_Certificate structure. It is supported
        /// only for purposes of verifying legacy Authenticode signatures
        /// </summary>
        WIN_CERT_REVISION_1_0 = 0x0100,

        /// <summary>
        /// Version 2 is the current version of the Win_Certificate structure.
        /// </summary>
        WIN_CERT_REVISION_2_0 = 0x0200,
    }

    public enum WindowsCertificateType : ushort
    {
        /// <summary>
        /// bCertificate contains an X.509 Certificate
        /// </summary>
        /// <remarks>
        /// Not Supported
        /// </remarks>
        WIN_CERT_TYPE_X509 = 0x0001,

        /// <summary>
        /// bCertificate contains a PKCS#7 SignedData structure
        /// </summary>
        WIN_CERT_TYPE_PKCS_SIGNED_DATA = 0x0002,

        /// <summary>
        /// Reserved
        /// </summary>
        WIN_CERT_TYPE_RESERVED_1 = 0x0003,

        /// <summary>
        /// Terminal Server Protocol Stack Certificate signing
        /// </summary>
        /// <remarks>
        /// Not Supported
        /// </remarks>
        WIN_CERT_TYPE_TS_STACK_SIGNED = 0x0004,
    }

    public enum WindowsSubsystem : ushort
    {
        /// <summary>
        /// An unknown subsystem
        /// </summary>
        IMAGE_SUBSYSTEM_UNKNOWN = 0x0000,

        /// <summary>
        /// Device drivers and native Windows processes
        /// </summary>
        IMAGE_SUBSYSTEM_NATIVE = 0x0001,

        /// <summary>
        /// The Windows graphical user interface (GUI) subsystem
        /// </summary>
        IMAGE_SUBSYSTEM_WINDOWS_GUI = 0x0002,

        /// <summary>
        /// The Windows character subsystem
        /// </summary>
        IMAGE_SUBSYSTEM_WINDOWS_CUI = 0x0003,

        /// <summary>
        /// The OS/2 character subsystem
        /// </summary>
        IMAGE_SUBSYSTEM_OS2_CUI = 0x0005,

        /// <summary>
        /// The Posix character subsystem
        /// </summary>
        IMAGE_SUBSYSTEM_POSIX_CUI = 0x0007,

        /// <summary>
        /// Native Win9x driver
        /// </summary>
        IMAGE_SUBSYSTEM_NATIVE_WINDOWS = 0x0008,

        /// <summary>
        /// Windows CE
        /// </summary>
        IMAGE_SUBSYSTEM_WINDOWS_CE_GUI = 0x0009,

        /// <summary>
        /// An Extensible Firmware Interface (EFI) application
        /// </summary>
        IMAGE_SUBSYSTEM_EFI_APPLICATION = 0x000A,

        /// <summary>
        /// An EFI driver with boot services
        /// </summary>
        IMAGE_SUBSYSTEM_EFI_BOOT_SERVICE_DRIVER = 0x000B,

        /// <summary>
        /// An EFI driver with run-time services
        /// </summary>
        IMAGE_SUBSYSTEM_EFI_RUNTIME_DRIVER = 0x000C,

        /// <summary>
        /// An EFI ROM image
        /// </summary>
        IMAGE_SUBSYSTEM_EFI_ROM = 0x000D,

        /// <summary>
        /// XBOX
        /// </summary>
        IMAGE_SUBSYSTEM_XBOX = 0x000E,

        /// <summary>
        /// Windows boot application.
        /// </summary>
        IMAGE_SUBSYSTEM_WINDOWS_BOOT_APPLICATION = 0x0010,
    }
}

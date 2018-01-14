public static class DeviceMemoryMap {

    public const uint MemoryTotal = 0x800000;

    // Devices Memory Mapping
    public const uint VM_VideoMemoryStart = 0x780000;
    public const uint VM_VideoMemoryMax = 0x7F52FF; // for 800x600x256 mode
    public const uint HDD_SectorBufferStart = 0x7F5300;
    public const uint HDD_SectorBufferEnd = 0x7F54FF;
    public const uint HDD_CommandTrigger = 0x7F5500;
    public const uint HDD_Status = 0x7F5501;
    public const uint HDD_CommandArg = 0x7F5502; // 4 bytes
    // 40922 bytes gap
    public const uint KBD_MemoryStart = 0x7FF4E0;
    public const uint KBD_MemoryEnd = 0x7FF4EF;
    // 14 bytes gap
    public const uint VM_VideoModeTrigger = 0x7FF4FF;
    public const uint VM_TextModeCharsetStart = 0x7FF500;
    public const uint VM_TextModeCharsetEnd = 0x7FFCFF;
    public const uint VM_PaletteStart = 0x7FFD00;
    public const uint VM_PaletteEnd = 0x7FFFFF;


    // Helper constants
    public const uint HDD_SectorSize = HDD_SectorBufferEnd - HDD_SectorBufferStart + 1;
    public const uint KBD_MemorySize = KBD_MemoryEnd - KBD_MemoryStart + 1;
    public const uint VM_PaletteSize = VM_PaletteEnd - VM_PaletteStart + 1;
    public const uint VM_TextModeCharsetSize = VM_TextModeCharsetEnd - VM_TextModeCharsetStart + 1;
}

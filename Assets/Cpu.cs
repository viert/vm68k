using System;
using System.Runtime.InteropServices;
using System.Threading;

public static class Cpu
{
    enum RegisterT
    {
        D0,         /* Data registers */
        D1,
        D2,
        D3,
        D4,
        D5,
        D6,
        D7,
        A0,         /* Address registers */
        A1,
        A2,
        A3,
        A4,
        A5,
        A6,
        A7,
        PC,         /* Program Counter */
        SR,         /* Status Register */
        SP,         /* The current Stack Pointer (located in A7) */
        USP,        /* User Stack Pointer */
        ISP,        /* Interrupt Stack Pointer */
        MSP,        /* Master Stack Pointer */
        SFC,        /* Source Function Code */
        DFC,        /* Destination Function Code */
        VBR,        /* Vector Base Register */
        CACR,       /* Cache Control Register */
        CAAR,       /* Cache Address Register */
        PREF_ADDR,  /* Last prefetch address */
        PREF_DATA,  /* Last prefetch data */
        PPC,        /* Previous value in the program counter */
        IR,         /* Instruction register */
        CPU_TYPE    /* Type of CPU being run */
    }

    public enum CpuTypes
    {
        INVALID,
        M68000,
        M68010,
        M68EC020,
        M68020,
        M68030,     /* Supported by disassembler ONLY */
        M68040      /* Supported by disassembler ONLY */
    }

    [System.Serializable]
    public class RegisterSet
    {
        public uint D0;
        public uint D1;
        public uint D2;
        public uint D3;
        public uint D4;
        public uint D5;
        public uint D6;
        public uint D7;
        public uint A0;
        public uint A1;
        public uint A2;
        public uint A3;
        public uint A4;
        public uint A5;
        public uint A6;
        public uint A7;
        public uint PC;
        public uint SR;
        public uint SP;
        public uint USP;
        public uint ISP;
        public uint MSP;
        public uint SFC;
        public uint DFC;
        public uint VBR;
        public uint CACR;
        public uint CAAR;
        public uint PREF_ADDR;
        public uint PREF_DATA;
        public uint PPC;
        public uint IR;
        public uint CPU_TYPE;
    }

    [DllImport("Musashi")] static extern void set_read_8(MemoryReader handler);
    [DllImport("Musashi")] static extern void set_read_16(MemoryReader handler);
    [DllImport("Musashi")] static extern void set_read_32(MemoryReader handler);
    [DllImport("Musashi")] static extern void set_read_dasm_16(MemoryReader handler);
    [DllImport("Musashi")] static extern void set_read_dasm_32(MemoryReader handler);
    [DllImport("Musashi")] static extern void set_write_8(MemoryWriter handler);
    [DllImport("Musashi")] static extern void set_write_16(MemoryWriter handler);
    [DllImport("Musashi")] static extern void set_write_32(MemoryWriter handler);
    [DllImport("Musashi")] static extern void m68k_init();
    [DllImport("Musashi")] static extern void m68k_pulse_reset();
    [DllImport("Musashi")] static extern void m68k_pulse_halt();
    [DllImport("Musashi")] static extern void m68k_set_cpu_type(int cputype);
    [DllImport("Musashi")] static extern int m68k_execute(int cycles);
    [DllImport("Musashi")] static extern uint m68k_get_reg(IntPtr context, int reg_num);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    [return: MarshalAs(UnmanagedType.I4)]
    public delegate void MemoryWriter(uint addr, uint data);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    [return: MarshalAs(UnmanagedType.I4)]
    public delegate uint MemoryReader(uint addr);


    // cpu memory access functions
    static public Func<uint, byte> Read8;
    static public Func<uint, ushort> Read16;
    static public Func<uint, uint> Read32;
    static public Func<uint, ushort> ReadDasm16;
    static public Func<uint, uint> ReadDasm32;
    static public Action<uint, byte> Write8;
    static public Action<uint, ushort> Write16;
    static public Action<uint, uint> Write32;

    // cpu clock
    static private bool emulateClock = true;
    static private double clockFrequencyMhz = 8.0f;
    static private long nextExecutionTick;

    public static RegisterSet Registers;

    public static void EnableClockEmulation(bool state)
    {
        emulateClock = state;
    }

    public static void SetCpuFrequencyMhz(double freq)
    {
        clockFrequencyMhz = freq;
    }

    static void DumpRegisters()
    {
        RegisterSet rs = new RegisterSet();
        rs.D0 = m68k_get_reg(IntPtr.Zero, (int)RegisterT.D0);
        rs.D1 = m68k_get_reg(IntPtr.Zero, (int)RegisterT.D1);
        rs.D2 = m68k_get_reg(IntPtr.Zero, (int)RegisterT.D2);
        rs.D3 = m68k_get_reg(IntPtr.Zero, (int)RegisterT.D3);
        rs.D4 = m68k_get_reg(IntPtr.Zero, (int)RegisterT.D4);
        rs.D5 = m68k_get_reg(IntPtr.Zero, (int)RegisterT.D5);
        rs.D6 = m68k_get_reg(IntPtr.Zero, (int)RegisterT.D6);
        rs.D7 = m68k_get_reg(IntPtr.Zero, (int)RegisterT.D7);
        rs.A0 = m68k_get_reg(IntPtr.Zero, (int)RegisterT.A0);
        rs.A1 = m68k_get_reg(IntPtr.Zero, (int)RegisterT.A1);
        rs.A2 = m68k_get_reg(IntPtr.Zero, (int)RegisterT.A2);
        rs.A3 = m68k_get_reg(IntPtr.Zero, (int)RegisterT.A3);
        rs.A4 = m68k_get_reg(IntPtr.Zero, (int)RegisterT.A4);
        rs.A5 = m68k_get_reg(IntPtr.Zero, (int)RegisterT.A5);
        rs.A6 = m68k_get_reg(IntPtr.Zero, (int)RegisterT.A6);
        rs.A7 = m68k_get_reg(IntPtr.Zero, (int)RegisterT.A7);
        rs.PC = m68k_get_reg(IntPtr.Zero, (int)RegisterT.PC);
        rs.SR = m68k_get_reg(IntPtr.Zero, (int)RegisterT.SR);
        rs.SP = m68k_get_reg(IntPtr.Zero, (int)RegisterT.SP);
        rs.USP = m68k_get_reg(IntPtr.Zero, (int)RegisterT.USP);
        rs.ISP = m68k_get_reg(IntPtr.Zero, (int)RegisterT.ISP);
        rs.MSP = m68k_get_reg(IntPtr.Zero, (int)RegisterT.MSP);
        rs.SFC = m68k_get_reg(IntPtr.Zero, (int)RegisterT.SFC);
        rs.DFC = m68k_get_reg(IntPtr.Zero, (int)RegisterT.DFC);
        rs.VBR = m68k_get_reg(IntPtr.Zero, (int)RegisterT.VBR);
        rs.CACR = m68k_get_reg(IntPtr.Zero, (int)RegisterT.CACR);
        rs.CAAR = m68k_get_reg(IntPtr.Zero, (int)RegisterT.CAAR);
        rs.PREF_ADDR = m68k_get_reg(IntPtr.Zero, (int)RegisterT.PREF_ADDR);
        rs.PREF_DATA = m68k_get_reg(IntPtr.Zero, (int)RegisterT.PREF_DATA);
        rs.PPC = m68k_get_reg(IntPtr.Zero, (int)RegisterT.PPC);
        rs.IR = m68k_get_reg(IntPtr.Zero, (int)RegisterT.IR);
        rs.CPU_TYPE = m68k_get_reg(IntPtr.Zero, (int)RegisterT.CPU_TYPE);
        Registers = rs;
    }

    static public void Init()
    {
        m68k_init();
        set_read_8(innerRead8);
        set_read_16(innerRead16);
        set_read_32(innerRead32);
        set_read_dasm_16(innerReadDasm16);
        set_read_dasm_32(innerReadDasm32);
        set_write_8(innerWrite8);
        set_write_16(innerWrite16);
        set_write_32(innerWrite32);
    }

    static public void Reset()
    {
        m68k_pulse_reset();
    }

    static public void Halt()
    {
        m68k_pulse_halt();
    }

    static public void SetCPUType(CpuTypes type)
    {
        m68k_set_cpu_type((int)type);
    }

    static public int Execute(int cycles)
    {
        if (emulateClock && nextExecutionTick != 0)
        {
            while (DateTime.Now.Ticks < nextExecutionTick) { Thread.Sleep(1); }
        }
        double cycleLength = (1000 * 2 * 1.0f) / (clockFrequencyMhz * 1000000.0f); // Cycle length in milliseconds
        var eStarted = DateTime.Now.Ticks;
        var eCycles = m68k_execute(cycles);
        var timeShouldBeElapsed = cycleLength * eCycles;
        var ticksDelta = (long)(timeShouldBeElapsed * TimeSpan.TicksPerMillisecond);
        nextExecutionTick = eStarted + ticksDelta;
        DumpRegisters();
        return eCycles;
    }

    static uint innerRead8(uint addr)
    {
        return Read8 == null ? 0 : (uint)Read8.Invoke(addr);
    }
    static uint innerRead16(uint addr)
    {
        return Read16 == null ? 0 : (uint)Read16.Invoke(addr);
    }
    static uint innerRead32(uint addr)
    {
        return Read32 == null ? 0 : Read32.Invoke(addr);
    }
    static uint innerReadDasm16(uint addr)
    {
        return ReadDasm16 == null ? 0 : (uint)ReadDasm16.Invoke(addr);
    }
    static uint innerReadDasm32(uint addr)
    {
        return ReadDasm32 == null ? 0 : (uint)ReadDasm32.Invoke(addr);
    }
    static void innerWrite8(uint addr, uint data)
    {
        if (Write8 != null)
            Write8.Invoke(addr, (byte)data);
    }
    static void innerWrite16(uint addr, uint data)
    {
        if (Write16 != null)
            Write16.Invoke(addr, (ushort)data);
    }
    static void innerWrite32(uint addr, uint data)
    {
        if (Write32 != null)
            Write32.Invoke(addr, data);
    }
}

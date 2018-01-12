using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComputerMemory : MonoBehaviour {

    const int memorySize = 0x800000;
    static public byte[] memory = new byte[memorySize];
    bool isReady = false;

    public delegate void MemoryChangeHandler(MemoryRange range);
    public struct MemoryRange
    {
        public uint start;
        public uint end;
        public MemoryRange(uint start, uint end)
        {
            this.start = start;
            this.end = end;
        }
    }
    public struct Subscriber
    {
        public Guid id;
        public MemoryRange range;
        public MemoryChangeHandler callback;
        public string tag;
        public Subscriber(String tag, MemoryRange range, MemoryChangeHandler callback)
        {
            id = Guid.NewGuid();
            this.tag = tag;
            this.range = range;
            this.callback = callback;
        }
    }

    public ArrayList al = new ArrayList();
    List<Subscriber> subscribers = new List<Subscriber>();
    Dictionary<Guid, Subscriber> notifiers = new Dictionary<Guid, Subscriber>();

    public void Subscribe(string tag, MemoryRange range, MemoryChangeHandler callback)
    {
        Subscriber h;
        foreach (Subscriber handler in subscribers)
        {
            if (range.start == handler.range.start && range.end == handler.range.end)
            {
                h = handler;
                h.callback += callback;
                Debug.Log(string.Format("{0} subscribed to memory range {1:X8}-{2:X8} as secondary", tag, range.start, range.end));
                return;
            }
        }
        h = new Subscriber(tag, range, callback);
        subscribers.Add(h);
        Debug.Log(string.Format("{0} subscribed to memory range {1:X8}-{2:X8}", tag, range.start, range.end));
    }

    void NotifyRange(uint addr, int size)
    {
        foreach (Subscriber handler in subscribers)
        {
            if (handler.range.start < addr + size && handler.range.end >= addr)
            {
                notifiers[handler.id] = handler;
            }
        }
    }

    public void Write8(uint addr, byte data)
    {
        NotifyRange(addr, 1);

        memory[addr] = data;
    }

    uint nextAddr(uint addr)
    {
        addr++;
        if (addr >= memory.Length)
        {
            addr = 0;
        }
        return addr;
    }

    uint normalizeAddr(uint addr)
    {
        while (addr >= memory.Length)
        {
            addr -= (uint)memory.Length;
        }
        return addr;
    }

    public void Write16(uint addr, ushort data)
    {
        NotifyRange(addr, 2);
        byte low = (byte)data;
        byte high = (byte)(data >> 8);
        memory[addr] = high;
        addr = nextAddr(addr);
        memory[addr] = low;
    }

    public void Write32(uint addr, uint data)
    {
        NotifyRange(addr, 4);
        byte b0 = (byte)data;
        data = data >> 8;
        byte b1 = (byte)data;
        data = data >> 8;
        byte b2 = (byte)data;
        data = data >> 8;
        byte b3 = (byte)data;
        memory[addr] = b3;
        addr = nextAddr(addr);
        memory[addr] = b2;
        addr = nextAddr(addr);
        memory[addr] = b1;
        addr = nextAddr(addr);
        memory[addr] = b0;
    }

    public byte Read8(uint addr)
    {
        addr = normalizeAddr(addr);
        return memory[addr];
    }

    public ushort Read16(uint addr)
    {
        uint data = 0;
        addr = normalizeAddr(addr);
        data = data | memory[addr];
        data = data << 8;
        addr = nextAddr(addr);
        data = data | memory[addr];
        return (ushort)data;
    }

    public uint Read32(uint addr)
    {
        uint data = 0;
        addr = normalizeAddr(addr);
        data = data | memory[addr];
        data = data << 8;
        addr = nextAddr(addr);
        data = data | memory[addr];
        data = data << 8;
        addr = nextAddr(addr);
        data = data | memory[addr];
        data = data << 8;
        addr = nextAddr(addr);
        data = data | memory[addr];
        return data;
    }

    void Start()
    {
        Debug.Log(string.Format("Memory Started, {0} bytes of memory installed", memory.GetLongLength(0)));
        isReady = true;
    }

    void Update()
    {
        // Flash all notifiers
        foreach (var n in notifiers)
        {
            n.Value.callback.Invoke(n.Value.range);
        }
        notifiers.Clear();
    }

}

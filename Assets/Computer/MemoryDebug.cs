using System;
using System.Collections.Generic;
using UnityEngine;

public class MemoryDebug : MonoBehaviour {
    ComputerMemory mem;

    [Serializable]
    public struct MemoryPair {
        public string name;
        public string addr;
        public byte value;
    }

    public MemoryPair[] watches;
    bool dirty;

	// Use this for initialization
	void Start () {
        mem = GetComponent<ComputerMemory>();
        foreach (var watch in watches){
            var addr = Convert.ToUInt32(watch.addr, 16);
            mem.Subscribe("memdebug", new ComputerMemory.MemoryRange(addr, addr), OnMemoryChange);
        }
	}

    void OnMemoryChange(ComputerMemory.MemoryRange range) {
        dirty = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (dirty)
        {
            dirty = false;
            for (int i = 0; i < watches.Length; i++)
            {
                var watch = watches[i];
                var addr = Convert.ToUInt32(watch.addr, 16);
                watches[i].value = ComputerMemory.memory[addr];
            }
        }
    }
}

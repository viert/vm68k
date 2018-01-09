using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ComputerMemory))]
public class ComputerAssembly : MonoBehaviour {

    bool initiated = false;
    ComputerMemory mem;
    [SerializeField] int cyclesExecuted = 0;
    [SerializeField] Cpu.RegisterSet registers;


    public Cpu.CpuTypes cpuType = Cpu.CpuTypes.M68000;

    void Start()
    {
        mem = GetComponent<ComputerMemory>();
        Cpu.Init();
        initiated = true;
        Cpu.SetCPUType(cpuType);
        Cpu.Read8 += mem.Read8;
        Cpu.Read16 += mem.Read16;
        Cpu.Read32 += mem.Read32;
        Cpu.ReadDasm16 += mem.Read16;
        Cpu.ReadDasm32 += mem.Read32;
        Cpu.Write8 += mem.Write8;
        Cpu.Write16 += mem.Write16;
        Cpu.Write32 += mem.Write32;
        Cpu.Reset();
    }

    void Update () {
        if (initiated)
        {
            registers = Cpu.Registers;
        }
    }
}

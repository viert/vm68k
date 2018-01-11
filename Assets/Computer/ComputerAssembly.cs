using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

[RequireComponent(typeof(ComputerMemory))]
public class ComputerAssembly : MonoBehaviour {

    bool initiated = false;
    ThreadStart executionThreadStart;
    Thread executionThread;
    ComputerMemory mem;
    [SerializeField] int cyclesExecuted = 0;
    [SerializeField] Cpu.RegisterSet registers;

    public Cpu.CpuTypes cpuType = Cpu.CpuTypes.M68000;
    public int cyclesToAdjust = 100000;
    public double cpuFrequency = 8.0f;
    public bool stopped = false;

    void Start()
    {
        mem = GetComponent<ComputerMemory>();
        Cpu.Init();
        initiated = true;
        Cpu.SetCPUType(cpuType);
        Cpu.SetCpuFrequencyMhz(cpuFrequency);
        Cpu.Read8 += mem.Read8;
        Cpu.Read16 += mem.Read16;
        Cpu.Read32 += mem.Read32;
        Cpu.ReadDasm16 += mem.Read16;
        Cpu.ReadDasm32 += mem.Read32;
        Cpu.Write8 += mem.Write8;
        Cpu.Write16 += mem.Write16;
        Cpu.Write32 += mem.Write32;
        Cpu.Reset();
        executionThreadStart = new ThreadStart(CpuExecutionThread);
        executionThread = new Thread(executionThreadStart);
        Invoke("StartEmulation", 0.1f);
    }

    void StartEmulation()
    {
        executionThread.Start();
    }

    void OnApplicationQuit()
    {
        stopped = true;
    }

    void CpuExecutionThread()
    {
        while (!stopped)
        {
            Cpu.Execute(100000);
        }
    }

    void Update () {
        if (initiated)
        {
            registers = Cpu.Registers;
        }
    }
}

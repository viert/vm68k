using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ComputerMemory))]
[RequireComponent(typeof(ComputerTimerDevice))]
public class ComputerAssembly : MonoBehaviour {

    bool initiated = false;
    ThreadStart executionThreadStart;
    Thread executionThread;
    ComputerMemory mem;
    ComputerTimerDevice timer;
    [SerializeField] int cyclesExecuted = 0;
    [SerializeField] Cpu.RegisterSet registers;
    [SerializeField] ulong interruptsRequested;

    public Cpu.CpuTypes cpuType = Cpu.CpuTypes.M68000;
    public int cyclesToAdjust = 100000;
    public double cpuFrequency = 8.0f;
    public bool stopped = false;

    string[] registerNames =
    {
        "PC",
        "A0",
        "A1",
        "A2",
        "A3",
        "A4",
        "A5",
        "A6",
        "A7",
        "D0",
        "D1",
        "D2",
        "D3",
        "D4",
        "D5",
        "D6",
        "D7",
    };

    Dictionary<string, Text> uiRegisterMap;

    void Start()
    {
        BuildRegisterMap();
        mem = GetComponent<ComputerMemory>();
        timer = GetComponent<ComputerTimerDevice>();
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

    private void BuildRegisterMap()
    {
        uiRegisterMap = new Dictionary<string, Text>();
        Text[] objects = FindObjectsOfType<Text>();
        foreach (Text go in objects)
        {
            foreach (string regName in registerNames)
            {
                if (go.name == regName)
                {
                    uiRegisterMap[regName] = go;
                }
            }
        }
    }

    void StartEmulation()
    {
        executionThread.Start();
        timer.StartTimer();
    }

    void OnApplicationQuit()
    {
        stopped = true;
    }

    void CpuExecutionThread()
    {
        while (!stopped)
        {
            cyclesExecuted = Cpu.Execute(cyclesToAdjust);
            if (cyclesExecuted == 0)
            {
                // breakpoint probably
                stopped = true;
            }
        }
    }

    void Update () {
        if (initiated)
        {
            registers = Cpu.Registers;
            uiRegisterMap["PC"].text = string.Format("PC 0x{0:X8}", registers.PC);
            uiRegisterMap["A0"].text = string.Format("A0 0x{0:X8}", registers.A0);
            uiRegisterMap["A1"].text = string.Format("A1 0x{0:X8}", registers.A1);
            uiRegisterMap["A2"].text = string.Format("A2 0x{0:X8}", registers.A2);
            uiRegisterMap["A3"].text = string.Format("A3 0x{0:X8}", registers.A3);
            uiRegisterMap["A4"].text = string.Format("A4 0x{0:X8}", registers.A4);
            uiRegisterMap["A5"].text = string.Format("A5 0x{0:X8}", registers.A5);
            uiRegisterMap["A6"].text = string.Format("A6 0x{0:X8}", registers.A6);
            uiRegisterMap["A7"].text = string.Format("A7 0x{0:X8}", registers.A7);
            uiRegisterMap["D0"].text = string.Format("D0 0x{0:X8}", registers.D0);
            uiRegisterMap["D1"].text = string.Format("D1 0x{0:X8}", registers.D1);
            uiRegisterMap["D2"].text = string.Format("D2 0x{0:X8}", registers.D2);
            uiRegisterMap["D3"].text = string.Format("D3 0x{0:X8}", registers.D3);
            uiRegisterMap["D4"].text = string.Format("D4 0x{0:X8}", registers.D4);
            uiRegisterMap["D5"].text = string.Format("D5 0x{0:X8}", registers.D5);
            uiRegisterMap["D6"].text = string.Format("D6 0x{0:X8}", registers.D6);
            uiRegisterMap["D7"].text = string.Format("D7 0x{0:X8}", registers.D7);
            interruptsRequested = Cpu.interruptsRequested;
        }
    }
}

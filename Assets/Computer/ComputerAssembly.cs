using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ComputerMemory))]
[RequireComponent(typeof(ComputerTimerDevice))]
[RequireComponent(typeof(ComputerScreen))]
public class ComputerAssembly : MonoBehaviour {

    string dasm;
    bool initiated = false;
    bool nextStep = false;
    ThreadStart executionThreadStart;
    Thread executionThread;
    ComputerMemory mem;
    ComputerTimerDevice timer;
    ComputerScreen screen;
    Cpu.RegisterSet registers;

    int cyclesExecuted = 0;
    ulong interruptsRequested;

    public Cpu.CpuTypes cpuType = Cpu.CpuTypes.M68000;
    public int cyclesPerExec = 100000;
    public bool autoAdjustCycles = true;
    public double cpuFrequency = 8.0f;
    public bool stopped = false;
    public bool stepMode = false;
    public bool triggerReset = false;

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
    Text disassemble;

    void Start()
    {
        BuildRegisterMap();
        mem = GetComponent<ComputerMemory>();
        timer = GetComponent<ComputerTimerDevice>();
        screen = GetComponent<ComputerScreen>();
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
        executionThreadStart = new ThreadStart(CpuExecutionThread);
        Invoke("StartEmulation", 0.1f);
    }

    private void BuildRegisterMap()
    {
        uiRegisterMap = new Dictionary<string, Text>();
        Text[] objects = FindObjectsOfType<Text>();
        foreach (Text go in objects)
        {
            if (go.name == "Disassembler")
            {
                disassemble = go;
                continue;
            }

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
        executionThread = new Thread(executionThreadStart);
        executionThread.Start();
        timer.StartTimer();
    }

    void OnApplicationQuit()
    {
        stopped = true;
    }

    void CpuExecutionThread()
    {
        Debug.Log("CPU is waiting for hardware to become ready");

        while (!mem.isReady)
        {
            Thread.Sleep(10);
        }

        while (!screen.isReady)
        {
            Thread.Sleep(10);
        }

        Debug.Log("CPU started");
        Cpu.Reset();
        dasm = Cpu.Disassemble(Cpu.Registers.PC);

        Cpu.ExecutionInfo ei;
        int cycles;

        while (!stopped)
        {
            if (triggerReset)
            {
                triggerReset = false;
                Cpu.Reset();
            }

            if (stepMode)
            {
                if (!nextStep)
                {
                    Thread.Sleep(10);
                    continue;
                }
                else
                {
                    nextStep = false;
                    cycles = Cpu.ExecuteSingle();
                    cyclesExecuted += cycles;
                    dasm = Cpu.Disassemble(Cpu.Registers.PC);
                }
            }
            else
            {
                ei = Cpu.Execute(cyclesPerExec);
                if (ei.cyclesExecuted == 0)
                {
                    stopped = true; // breakpoint probably
                }

                if (autoAdjustCycles)
                {
                    if (ei.cyclesSlept > 5)
                    {
                        cyclesPerExec -= (ei.cyclesSlept - 5)*1000;
                    }
                    else if (ei.cyclesSlept < 1)
                    {
                        cyclesPerExec += 5000;
                    }

                }

                cyclesExecuted += ei.cyclesExecuted;
                dasm = Cpu.Disassemble(Cpu.Registers.PC);
            }
        }

        Debug.Log("CPU stopped");
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
            disassemble.text = dasm;
        }

        if (Input.GetKeyDown(KeyCode.Keypad5))
        {
            nextStep = true;
        }
    }
}

using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using UnityEngine;

[RequireComponent(typeof(ComputerMemory))]
public class DriveController : MonoBehaviour {

    ThreadStart ts;
    Thread th;
    FileStream fs;
    ComputerMemory mem;
    [SerializeField] Status currentStatus = Status.Initializing;
    [SerializeField] DriveInfo driveInfo;

    string deviceName = "Drive Controller";
    bool stopped = false;

    [HideInInspector] public bool isReady;
    public uint InterruptLevel = 5;
    public string driveFilename;


    bool driveAttached = false;

    public enum Command {
        None,
        Reset,
        ReadSector,
        WriteSector,
        Info
    };

    public enum Status
    {
        Ready,
        Initializing,
        Busy,
        Error
    }

    [Serializable]
    public struct DriveInfo
    {
        public uint sectors;
        public uint bytes;
    }

	void Start () {
        SetStatus(Status.Initializing);
        mem = GetComponent<ComputerMemory>();
        try
        {
            fs = File.Open(driveFilename, FileMode.Open, FileAccess.ReadWrite);
            driveInfo = new DriveInfo();
            driveInfo.bytes = (uint)fs.Length;
            driveInfo.sectors = (uint)Mathf.CeilToInt(fs.Length / (float)DeviceMemoryMap.HDD_SectorSize);
            driveAttached = true;
        }
        catch (Exception ex)
        {
            Debug.LogError(ex);
            driveAttached = false;
        }
        ts = new ThreadStart(ExecutionThread);
        th = new Thread(ts);
        th.Start();
    }

    void OnApplicationQuit()
    {
        stopped = true;
    }

    void SetStatus(Status newStatus)
    {
        if (currentStatus == newStatus)
        {
            return;
        }
        currentStatus = newStatus;
        UpdateMappedStatus();
        if (currentStatus == newStatus)
        {
            ComputerMemory.memory[DeviceMemoryMap.HDD_CommandTrigger] = (byte)Command.None;
        }
    }

    void UpdateMappedStatus()
    {
        ComputerMemory.memory[DeviceMemoryMap.HDD_Status] = (byte)currentStatus;
    }

    Command GetCurrentCommand()
    {
        return (Command)ComputerMemory.memory[DeviceMemoryMap.HDD_CommandTrigger];
    }

    void ExecutionThread()
    {
        Command cmd;
        Debug.Log(string.Format("{0} started", deviceName));
        if (!driveAttached)
        {
            SetStatus(Status.Error);
        }
        else
        {
            SetStatus(Status.Ready);
        }
        while (!stopped)
        {
            cmd = GetCurrentCommand();
            switch (cmd)
            {
                case Command.None:
                    Thread.Sleep(10);
                    break;
                case Command.Info:
                    cmdInfo();
                    break;
                case Command.ReadSector:
                    cmdReadSector();
                    break;
                case Command.WriteSector:
                    cmdWriteSector();
                    break;
                case Command.Reset:
                    cmdReset();
                    break;
            }
            Thread.Sleep(1);

        }
        fs.Close();
        Debug.Log(string.Format("{0} stopped", deviceName));
    }

    void requestInterrupt()
    {
        Cpu.SetIRQ(InterruptLevel);
    }

    void clearBuffer()
    {
        for (uint addr = DeviceMemoryMap.HDD_SectorBufferStart; addr <= DeviceMemoryMap.HDD_SectorBufferEnd; addr++)
        {
            ComputerMemory.memory[addr] = 0;
        }
    }

    void cmdInfo()
    {
        SetStatus(Status.Busy);
        if (!driveAttached)
        {
            SetStatus(Status.Error);
            requestInterrupt();
            return;
        }

        clearBuffer();
        uint addr = DeviceMemoryMap.HDD_SectorBufferStart;
        mem.Write32(addr, driveInfo.bytes);
        mem.Write32(addr + 4, driveInfo.sectors);
        SetStatus(Status.Ready);
        requestInterrupt();
    }

    uint getCmdArgument()
    {
        return mem.Read32(DeviceMemoryMap.HDD_CommandArg);
    }

    void cmdReadSector()
    {
        SetStatus(Status.Busy);
        if (!driveAttached)
        {
            SetStatus(Status.Error);
            requestInterrupt();
            return;
        }

        uint sectorNum = getCmdArgument();
        if (sectorNum >= driveInfo.sectors)
        {
            SetStatus(Status.Error);
            requestInterrupt();
            return;
        }

        uint position = sectorNum * DeviceMemoryMap.HDD_SectorSize;
        byte[] buffer = new byte[DeviceMemoryMap.HDD_SectorSize];
        fs.Seek(position, SeekOrigin.Begin);
        fs.Read(buffer, 0, (int)DeviceMemoryMap.HDD_SectorSize);

        uint addr = DeviceMemoryMap.HDD_SectorBufferStart;
        for (int i = 0; i < DeviceMemoryMap.HDD_SectorSize; i++)
        {
            ComputerMemory.memory[addr++] = buffer[i];
        }
        Debug.Log(string.Format("Read sector {0} from disk into buffer", sectorNum));

        SetStatus(Status.Ready);
        requestInterrupt();
    }

    void cmdWriteSector()
    {
        SetStatus(Status.Busy);
        if (!driveAttached)
        {
            SetStatus(Status.Error);
            requestInterrupt();
            return;
        }

        uint sectorNum = getCmdArgument();
        if (sectorNum >= driveInfo.sectors)
        {
            SetStatus(Status.Error);
            requestInterrupt();
            return;
        }

        uint position = sectorNum * DeviceMemoryMap.HDD_SectorSize;
        byte[] buffer = new byte[DeviceMemoryMap.HDD_SectorSize];
        fs.Seek(position, SeekOrigin.Begin);

        uint addr = DeviceMemoryMap.HDD_SectorBufferStart;
        for (int i = 0; i < DeviceMemoryMap.HDD_SectorSize; i++)
        {
            buffer[i] = ComputerMemory.memory[addr++];
        }
        fs.Write(buffer, 0, (int)DeviceMemoryMap.HDD_SectorSize);
        Debug.Log(string.Format("Wrote buffer to sector {0} of disk", sectorNum));

        SetStatus(Status.Ready);
        requestInterrupt();
    }

    void cmdReset()
    {
        if (!driveAttached)
        {
            SetStatus(Status.Error);
            requestInterrupt();
            return;
        }
        SetStatus(Status.Ready);
        requestInterrupt();
    }

}

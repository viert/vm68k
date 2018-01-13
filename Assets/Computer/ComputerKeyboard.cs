using UnityEngine;

public class ComputerKeyboard : MonoBehaviour {

    const uint keyboardMemoryStart = 0x7FF4E0;
    const int keyboardMemorySize = 16;
    public uint InterruptLevel = 2;

    public static KeyCode[] keyboardMap = {
        KeyCode.Escape,
        KeyCode.F1,
        KeyCode.F2,
        KeyCode.F3,
        KeyCode.F4,
        KeyCode.F5,
        KeyCode.F6,
        KeyCode.F7,
        KeyCode.F8,
        KeyCode.F9,
        KeyCode.F10,
        KeyCode.F11,
        KeyCode.F12,

        KeyCode.BackQuote,
        KeyCode.Alpha1,
        KeyCode.Alpha2,
        KeyCode.Alpha3,
        KeyCode.Alpha4,
        KeyCode.Alpha5,
        KeyCode.Alpha6,
        KeyCode.Alpha7,
        KeyCode.Alpha8,
        KeyCode.Alpha9,
        KeyCode.Alpha0,
        KeyCode.Minus,
        KeyCode.Equals,
        KeyCode.Backspace,

        KeyCode.Tab,
        KeyCode.Q,
        KeyCode.W,
        KeyCode.E,
        KeyCode.R,
        KeyCode.T,
        KeyCode.Y,
        KeyCode.U,
        KeyCode.I,
        KeyCode.O,
        KeyCode.P,
        KeyCode.LeftBracket,
        KeyCode.RightBracket,
        KeyCode.Return,
        
        KeyCode.CapsLock,
        KeyCode.A,
        KeyCode.S,
        KeyCode.D,
        KeyCode.F,
        KeyCode.G,
        KeyCode.H,
        KeyCode.J,
        KeyCode.K,
        KeyCode.L,
        KeyCode.Semicolon,
        KeyCode.Quote,
        KeyCode.Backslash,

        KeyCode.LeftShift,
        KeyCode.Z,
        KeyCode.X,
        KeyCode.C,
        KeyCode.V,
        KeyCode.B,
        KeyCode.N,
        KeyCode.M,
        KeyCode.Comma,
        KeyCode.Period,
        KeyCode.Slash,
        KeyCode.RightShift,

        KeyCode.LeftControl,
        KeyCode.LeftWindows,
        KeyCode.LeftAlt,
        KeyCode.Space,
        KeyCode.RightAlt,
        KeyCode.RightControl,

        KeyCode.Insert,
        KeyCode.Home,
        KeyCode.PageUp,
        KeyCode.Delete,
        KeyCode.End,
        KeyCode.PageDown,

        KeyCode.UpArrow,
        KeyCode.LeftArrow,
        KeyCode.DownArrow,
        KeyCode.RightArrow,
    };

    byte[] previousMap = new byte[keyboardMemorySize];
    byte[] currentMap = new byte[keyboardMemorySize];
    int i;

	void Update () {
        previousMap = currentMap;
        currentMap = new byte[keyboardMemorySize];
        for (i = 0; i < keyboardMap.Length; i++)
        {
            if (Input.GetKey(keyboardMap[i])) {
                SetBit(i);
            }
        }

        for (i = 0; i < keyboardMemorySize; i++)
        {
            if (currentMap[i] != previousMap[i])
            {
                CopyMem();
                Cpu.SetIRQ(InterruptLevel);
                return;
            }
        }
	}

    void CopyMem()
    {
        uint addr = keyboardMemoryStart;
        for (i = 0; i < keyboardMemorySize; i++)
        {
            ComputerMemory.memory[addr++] = currentMap[i];
        }
    }

    void SetBit(int index)
    {
        int byteNum = keyboardMemorySize - 1 - (index / 8);
        int bitNum = index % 8;
        byte setMask = (byte)(0x01 << bitNum);
        currentMap[byteNum] |= setMask;
    }
}

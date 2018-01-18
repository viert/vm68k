# vm68k - an imaginary m68k-based computer emulator

This is an imaginary computer emulator written in Unity3d/C# using [musashidll](https://github.com/viert/musashidll) as a CPU emulator

## Architecture

  * CPU M68000
  * Interrupt timer generates a low level interrupt 50 times a second
  * 8Mb of direct access memory
  * Simple graphics card memory-mapped into upper memory with 1 text and 3 graphic modes
  * Keyboard is memory-mapped into 16 bytes with every bit connected to a corresponding key. Every keypress generates a CPU interrupt.
  * Simple HDD controller is mapped into upper memory with a couple of bytes to communicate and a memory buffer to read from/write to. Every operation is async and generates a CPU interrupt when it's finished.
  
## Emulator details

  * The CPU is running in a background thread taking care of proper emulation speed.
  * Computer screen made with a whole-screen texture rerendering when needed
  * All computer components made as GameObject components.
  * ROM and HDD images are currently made as binary TextAssets
  * Built-in debugger may be turned on by pressing Pause key. Currently it doesn't switch the emulator to Step Mode.
  * In step mode the next step button is Num5. This will change in the nearest future

## How to use

  * Clone the repo and open it in Unity3d editor
  * Put Musashi.dll/dylib into the Assets folder (it's better to drag and drop it into unity project inspector to allow Unity to automatically assign project references)
  * Put the ROM file into the Assets folder (filename extension should be `.bytes` for Unity to consider it as a binary TextAsset) and connect it to ComputerMemory component of the Computer object in the scene. This will make the emulator load the ROM into memory on start
  * Put the HDD file into the Assets folder (filename extension should be `.bytes` as well) and connect it to DriveController component. There's only one disk in the emulator at the moment.

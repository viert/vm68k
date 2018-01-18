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

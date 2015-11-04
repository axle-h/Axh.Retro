# Axh.Retro
Emulation libraries.
Currently:

* Axh.Retro.CPU.Z80 - Z80 based CPU & MMU configurable for (untested)8080, Z80 and GameBoy.
  * Fully implemented DynaRec (targets .NET IL through expression API).
  * WIP interpreted core.
* Axh.Retro.CPU.Z80.Tests - Complete Z80 instruction set decode tests.
* Axh.Retro.GameBoy - WIP GameBoy hardware
* Axh.Retro.Z80Console - Simple implementation of Axh.Retro.CPU.Z80 in Z80, DynaRec mode.
 * Currently runs embedded code.bin file that is my [z80-hello](https://github.com/axle-h/z80-hello) compiled with [SDCC](http://sdcc.sourceforge.net/)
 
 
TODO:
* GameBoy GPU, controls & sound
* Debugger
* Interpreted core
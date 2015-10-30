# Axh.Retro
Emulation libraries.
Currently:

* Axh.Retro.CPU.X80 - 8080 based CPU & MMU configurable for 8080, Z80 and GameBoy.
  * Fully implemented DynaRec (targets .NET IL through expression API).
  * WIP interpreted core.
* Axh.Retro.CPU.X80.Tests - Complete Z80 instruction set decode tests.
* Axh.Retro.GameBoy - WIP GameBoy hardware
* Axh.Retro.Z80Console - Simple implementation of Axh.Retro.CPU.X80 in Z80, DynaRec mode.
 * Currently runs embedded code.bin file that is my [z80-hello](https://github.com/axle-h/z80-hello) compiled with [SDCC](http://sdcc.sourceforge.net/)
 
 
TODO:
* Write-aware DynaRec caching
* Interrupts
* Debugger
* Interpreted core
* GameBoy custom opcodes
* GameBoy hardware
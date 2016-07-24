# Axh.Retro
Emulation libraries.
Currently:

* Axh.Retro.CPU.Z80 - Z80 based CPU & MMU configurable for 8080, Z80 and GameBoy.
  * Simple interpreted core.
  * "Dynamically Re-compiling" core. Instead of executing each block of Z80 operations immedietely, it builds an expression tree representing the block, which can be cached for increased speed. It's about 2.5x faster than the simple interpreted core.
* Axh.Retro.CPU.Z80.Tests - Complete Z80 instruction set decode tests.
* Axh.Retro.GameBoy.BlarggTests - Integration tests for the Blargg test roms.
 * Currently only running the CPU instruction ROMs. Passes all except 02-interrupts: EI. Not sure why it fails.
* Axh.Retro.GameBoy - WIP GameBoy hardware. Just enough to play Tetris at full speed!
* Axh.Retro.GameBoy.Wpf - Barebones WPF view of gameboy GPU output.
* Axh.Retro.Z80Console - Simple implementation of Axh.Retro.CPU.Z80 in Z80, DynaRec mode.
 * Currently runs embedded code.bin file that is my [z80-hello](https://github.com/axle-h/z80-hello) compiled with [SDCC](http://sdcc.sourceforge.net/)
 
 
TODO:
* Gameboy sound.
* Gameboy GPU window rendering. I think that background and sprites are enough to play Tetris for now.
* WPF keyboard controls.
* Debugger.
* Be intersting to see which blocks are run most frequently and whether we can optimize their expression trees.
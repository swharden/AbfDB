# AbfDB

**AbfDB searches the filesystem for Axon Binary Format (ABF) files and collects their header information in a database.**

AbfDB is intended only to be used by the original authors. Source code is provided here for backup and educational purposes only.

### Requirements

AbfDB reads ABF header information using [AbfSharp](https://github.com/swharden/ABFsharp)'s ABFFIO wrapper. This means your that project must target `x86`, can only be run on Windows platforms, and your system must have all dependencies that ABFFIO.DLL requires installed. See [AbfSharp: DLL Requirements](https://github.com/swharden/ABFsharp#dll-requirements) for details.
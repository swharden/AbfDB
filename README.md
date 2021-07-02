# AbfDB

[![Build Status](https://dev.azure.com/swharden/swharden/_apis/build/status/swharden.AbfDB?branchName=main)](https://dev.azure.com/swharden/swharden/_build/latest?definitionId=22&branchName=main)

**AbfDB searches the filesystem for Axon Binary Format (ABF) files and collects their header information in a database.**

AbfDB is intended only to be used by the original authors. Source code is provided here for backup and educational purposes only.

AbfDB reads ABF header information using ABFFIO.DLL (a 32-bit Windows binary). This has some implications for how this project must be built. See the [AbfSharp page](https://github.com/swharden/ABFsharp#dll-requirements) for details.
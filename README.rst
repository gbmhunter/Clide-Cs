==============================================================
Clide (Command Line Interface Development Environment) Library
==============================================================

- Author: gbmhunter <gbmhunter@gmail.com> (http://www.cladlab.com)
- Created: 2012/03/19
- Last Modified: 2013/07/03
- Version: v1.0.0.3
- Company: CladLabs
- Project: Free Code Libraries
- Language: C#
- Compiler: .NET	
- uC Model: n/a
- Computer Architecture: .NET
- Operating System: Windows/Linux/MacOS (Linux and MacOS are supported with Mono)
- Documentation Format: Doxygen
- License: GPLv3

Description
===========

**This is the C# version of Clide, designed to be run on the .NET platform.
For the embedded version, see Cpp-Clide.**

A command-line style communication interface designed to make it easy to send messages and
data between two micro-controllers, a microcontroller and computer, or two computers, via
a serial interface that supports ASCII characters (e.g. UART, I2C, SPI).

Two functionally identical parts to Clide exist, one written in C++ for an embedded system,
and the other part written in C# for running on a PC.

Uses a both human readable and computer readable format (UNIX command-line style)
for easy implementation and debugging. Automatically generates UNIX-style help text
which is useful if system is being controlled by a human (see below).

Useful for working with and controlling embedded systems.

An example message takes the format of

::

	set-speed -rpm 2000\n

where set-speed is the command name, "-rpm" is an optional argument that specifies
that the value is in rpm, "2000" is a non-optional parameter which specifies the
speed, and "\n" is the new-line character which signifies the end of a message
(which is normally inserted automatically by command-lines when enter is pressed).

All text is case-sensitive. It is recommended to use lower-case only to
follow the UNIX command-line style.

Special support for the "help" command, and `-h`, `--help` flags for every registered
command. No special support other UNIX commands such as `man`, `whatis` or `info`.

Internal Dependencies
=====================

These are all included in the repository.

- Clide-Config.cs					-> Configuration file.
- Clide-Controller.cs				-> Controller object.
- Clide-Rx.cs						-> Receive object.
- Clide-Tx.cs						-> Transmit object.
- Clide-Cmd.cs						-> Command object.
- Clide-Param.cs					-> Parameter object.
- Clide-Option.cs					-> Option object.
- Clide-Port.cs 					-> Contains port-specific code. Change this to fit application.

External Dependencies
=====================
- None

Packet Decoding Process (RX)
============================

- Remove all non-alphanumeric characters from the start of the packet
- Split packet into separate arguments
- Make sure received command is registered
- Extract options and values (if present), execute option callback functions
- Check all parameters are present
- Execute parameter callback functions
- Execute command callback function

Issues
======

- See GitHub Issues

Limitations
===========

- None documented

Usage
=====

::

	coming soon...
	
Changelog
=========

======== ========== ===================================================================================================
Version  Date       Comment
======== ========== ===================================================================================================
v1.0.0.3 2013/07/03 Formatted some section titles in README that were all-caps.
v1.0.0.2 2013/07/03 Changelog in README is now in table format.
v1.0.0.1 2013/06/02 Removed incorrect external dependencies and programming language in README.
v1.0.0.0 2013/05/30 Initial version.
======== ========== ===================================================================================================

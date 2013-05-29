==============================================================
Clide (Command Line Interface Development Environment) Library
==============================================================

- Author: gbmhunter <gbmhunter@gmail.com> (http://www.cladlab.com)
- Created: 2012/03/19
- Last Modified: 2013/05/30
- Version: v1.0.0.0
- Company: CladLabs
- Project: Free Code Libraries	.
- Language: C++
- Compiler: GCC	
- uC Model: n/a
- Computer Architecture: n/a
- Operating System: n/a
- Documentation Format: Doxygen
- License: GPLv3

Description
===========

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

Uses dynamic memory allocation for creating commands/options/parameters e.t.c
Command data is stored in a contiguous block

All text is case-sensitive. It is recommended to use lower-case only to
follow the UNIX command-line style.

Special support for the "help" command, and `-h`, `--help` flags for every registered
command. No special support other UNIX commands such as `man`, `whatis` or `info`
'help' is a special command which can be implemented by calling
RegisterHelpCmd()

Internal Dependencies
=====================

These are all included in the repository.

- Clide-Config.cs					-> Configuration file.
- Clide-Rx.cs						-> Receive object.
- Clide-Tx.cs						-> Transmit object.
- Clide-Cmd.cs						-> Command object.
- Clide-Param.cs					-> Parameter object.
- Clide-Option.cs					-> Option object.
- Clide-Port.cs 					-> Contains port-specific code. Change this to fit application.

External Dependencies
=====================
- <stdio.h> 	-> snprintf()
- <stdlib.h> 	-> realloc(), malloc(), calloc(), free()
- <cctype>		-> isalnum()
- <getopt.h>	-> getopt()

PACKET DECODING PROCESS (RX)
=============================

- Remove all non-alphanumeric characters from the start of the packet
- Split packet into separate arguments
- Make sure received command is registered
- Extract options and values (if present), execute option callback functions
- Check all parameters are present
- Execute parameter callback functions
- Execute command callback function

ISSUES
====

- See GitHub Issues

LIMITATIONS
===========

- Maximum number of commands: 256
- Maximum number of parameters or options per command: 256
- Maximum string length of a command name, option name/value, parameter value: clideMAX_STRING_LENGTH

Usage
=====

::

	coming soon...
	
Changelog
=========

- v1.0.0.0 	-> (2013/05/30) Initial version.


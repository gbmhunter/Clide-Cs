using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections;
using System.Diagnostics;
using NDesk.Options;            // command-line parameter parser
using System.IO.Ports;              // For accessing the serial port

namespace Clide
{
  
    /// <summary>
    /// Flexible class for decoding serial data. 
    /// At a minimum, packets are expected to be ASCII-encoded, terminated with an end-character,
    /// and in description-value pairs with a common delimiter between pairs. 
    /// Start and header-end termination characters  are optional.
    /// Optional checksum. Removes non alpha-numeric characters from the start of the RX buffer.
    /// </summary>
    public class Rx
    {
        #region Enums

        /// <summary>
        /// Used as states for the decode state machine.
        /// </summary>
        enum DecodeStates
        {
            Idle,
            StartCharFound,
            EndCharFound,
            IdFound,
            Decoded,
            RunCallBackFunc,
            Finished
        }

        public enum StatusId_t
        {
            Ok,
            PacketDecodingPassed,
            DataIdDidNotMatch,
            RxBufferOverflow,
            PacketDecodingFailed,
            PacketUnexpectedlyTruncated,
            IncorrectNumParam
        }

        #endregion

        #region Parameters And Backing Fields

        //========== MESSAGE FORMAT VARIABLES ============//

        /// <summary>
        /// Backing field
        /// </summary>
        private bool _isStartChar = false;
        /// <summary>
        /// Set to true if packets have a start character. If set to true,
        /// remember to also set StartChar.
        /// </summary>
        public bool IsStartChar
        {
            get { return _isStartChar; }
            set { _isStartChar = value; }
        }

        /// <summary>
        /// Backing field
        /// </summary>
        private char _startChar;
        /// <summary>
        /// Sets the start indentification character.
        /// </summary>
        public char StartChar
        {
            get { return _startChar; }
            set { _startChar = value; }
        }

        /// <summary>
        /// Backing field.
        /// </summary>
        private bool _isHeader = false;
        /// <summary>
        /// Set to true if packets have a header. If true, remember to also set
        /// the header-data boundry character.
        /// </summary>
        public bool IsHeader
        {
            get { return _isHeader; }
            set { _isHeader = value; }
        }

        /// <summary>
        /// Backing field
        /// </summary>
        private char _headerDataSeperatorChar;
        /// <summary>
        /// The character that seperates the header from the data. 
        /// Only used if IsHeader is set to true.
        /// </summary>
        public char HeaderDataSeperatorChar
        {
            get { return _headerDataSeperatorChar; }
            set { _headerDataSeperatorChar = value; }
        }


        private char dataIdValueDivisorChar;
        private char dataSeperatorChar;
        
        /// <summary>
        /// Backing field.
        /// </summary>
        private char _endIdenChar;
        /// <summary>
        /// Sets the end indentification character. 
        /// Unlike the start character, this is non-optional, every packet must have a packet end character.
        /// </summary>
        public char EndIdenChar
        {
            get { return _endIdenChar; }
            set { _endIdenChar = value; }
        }

        

        /// <summary>
        /// Backing field
        /// </summary>
        private int _packetCount = 0;
        /// <summary>
        /// Stores the number of packets that have been decoded. Function exists to return this dataIList
        /// to the Form class so that it can be displayed to the user.
        /// </summary>
        public int PacketCount
        { get { return _packetCount; } set { _packetCount = value; } }

        public DataTable dataTable1;

        /// <summary>
        /// Stores a list of the parameters found after decoding the command line.
        /// </summary>
        public List<string> Parameters;

    

        /// <summary>
        /// Backing field. Default flush limit set to 10,000 bytes.
        /// </summary>
        private int _buffFlushLimit = 10000;
        /// <summary>
        /// The receiving buffer is flushed if it exceeds this amount. 
        /// Make sure this larger than the maximum rx packet size.
        /// </summary>
        public int BuffFlushLimit
        {
            get { return _buffFlushLimit; }
            set { _buffFlushLimit = value; }
        }

        /// <summary>
        /// A base type which holds the information regarding one status, the id,
        /// and the associated message. Assigned to in the constructor.
        /// </summary>
        public struct status_t
        {
            public StatusId_t id;
            public string message;
        }

        
        public status_t[] errorArray = new status_t[4];

        /// <summary>
        /// Contains all of the different statuses which could occur when decoding rx input.
        /// </summary>
        public struct statusArray_t
        {
            public status_t ok;
            public status_t dataIdDidNotMatch;
            public status_t packetDecodingPassed;
            public status_t rxBufferOverflow;
            public status_t unrecognisedCmd;
            public status_t packetUnexpectedlyTruncated;
            public status_t incorrectNumParameters;
        }

        public statusArray_t statusArray = new statusArray_t();
        
        /// <summary>
        /// Backing field.
        /// </summary>
        private int _lastCommandIndex;
        /// <summary>
        /// The index (of _registeredCmds) of the last decoded command.
        /// </summary>
        public int LastCommandIndex
        { get { return _lastCommandIndex; } set { _lastCommandIndex = value; } }


        #endregion

        #region Private Variables

        /// <summary>
        /// Inherits from collection so that methods such as .Add() can be used.
        /// </summary>
        private System.Collections.ObjectModel.Collection<Command> _registeredCmds;

        /// <summary>
        /// Container to remember the serial port object. Given to this class from Serial.Controller
        /// on creation.
        /// </summary>
        Clide.Port _port;

        /// <summary>
        /// Remembers what the current decode state is for the rx state machine.
        /// </summary>
        private DecodeStates _decodeState = DecodeStates.Idle;

        private int _startPos = 0;
        private int _endPos = 0;

        private string _dataIdString;
        private string _rxBuffer;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor. Initialises statuses and registered command array.
        /// </summary>
        /// <param name="port">The port which the RX engine is going to use to receive data from.</param>
        public Rx(Clide.Port port)
        {
            // Create new data table
            dataTable1 = new DataTable();

            statusArray.ok.id = StatusId_t.Ok;
            statusArray.ok.message = "O.K.";
            statusArray.dataIdDidNotMatch.id = StatusId_t.DataIdDidNotMatch;
            statusArray.dataIdDidNotMatch.message = "Data ID did not match.";
            statusArray.packetDecodingPassed.id = StatusId_t.PacketDecodingPassed;
            statusArray.packetDecodingPassed.message = "Packet successfully decoded.";
            statusArray.rxBufferOverflow.id = StatusId_t.RxBufferOverflow;
            statusArray.rxBufferOverflow.message = "RX buffer overflow.";
            statusArray.unrecognisedCmd.id = StatusId_t.PacketDecodingFailed;
            statusArray.unrecognisedCmd.message = "Received unrecognised command.";
            statusArray.packetUnexpectedlyTruncated.id = StatusId_t.PacketUnexpectedlyTruncated;
            statusArray.packetUnexpectedlyTruncated.message = "Packet unexpectedly truncated.";
            statusArray.incorrectNumParameters.id = StatusId_t.IncorrectNumParam;
            statusArray.incorrectNumParameters.message = "Incorrect number of parameters.";

            // Instantiate registered commands
            _registeredCmds = new System.Collections.ObjectModel.Collection<Command>();

            _port = port;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Searches for string identifiers and decodes if packet is found.
        /// Main part of RX functionality.
        /// </summary>
        /// <returns>The status after trying to decode any characters present in the RX buffer.</returns>
        public status_t Run()
        {

            status_t status = new status_t();

            // Read data from serial buffer into private buffer
            _rxBuffer += _port.serialPort.ReadExisting();

            // Check for empty buffer (no data yet received)
            if (_rxBuffer == "")
                return statusArray.ok;

            // Remove non-alpha numeric characters from start
            while (Char.IsLetterOrDigit(_rxBuffer, 0) == false)
            {
                // Remove first element
                _rxBuffer = _rxBuffer.Remove(0, 1);

                // Re-check if empty
                if (_rxBuffer == "")
                    return statusArray.ok;
            }

            // Check for too-large buffer
            if (_rxBuffer.Length >= _buffFlushLimit)
            {
                // Reset buffer if too large (including start/end counters)
                this.ResetBuffer(ref _startPos, ref _endPos);
                _decodeState = DecodeStates.Idle;
                // Return error
                return statusArray.rxBufferOverflow;
            }


            // Set finished flag to false on entry to state machine
            bool finishedFlag = false;


            while (!finishedFlag)
            {
                // State machine for search and decode algorithm
                switch (_decodeState)
                {
                    case DecodeStates.Idle:

                        if (_isStartChar)
                        {
                            // Try and find the first instance of the start packet identifier
                            _startPos = _rxBuffer.IndexOf(_startChar);

                            //Debug.WriteLine("IDL: Start Pos = " + startPos.ToString());

                            if (_startPos >= 0)
                            {
                                _decodeState = DecodeStates.StartCharFound;
                            }
                            else
                            {
                                // No start-character found, so clear buffer (only if not already 0 to avoid errors)
                                if (_rxBuffer.Length > 0)
                                {
                                    this.ResetBuffer(ref _startPos, ref _endPos);
                                }
                                // Remain in current searchAndDecodeState
                                // Return o.k.
                                status = statusArray.ok;
                                finishedFlag = true;
                            }
                            
                        }
                        else
                        {
                            // No start char, so jump straight into the next state
                            _startPos = 0;
                            _decodeState = DecodeStates.StartCharFound;
                        }
                        break;

                    case DecodeStates.StartCharFound:
                        // Search for end-character (starting for 1 after the start-character)
                        _endPos = _rxBuffer.IndexOf(_endIdenChar, _startPos + 1);

                        //Debug.WriteLine("SCF: Start Pos = " + startPos.ToString());

                        // Check to see if end character was found
                        if (_endPos > _startPos)
                        {
                            _decodeState = DecodeStates.EndCharFound;
                            break;
                        }
                        else
                        {

                            // Stay in same searchAndDecodeState (do nothing)
                            finishedFlag = true;
                            status = statusArray.ok;
                            break;
                        }

                    case DecodeStates.EndCharFound:

                        // Make sure there is not another start character before the end character
                        // (which would indicate a truncated packet for some reason)
                        // Only valid if there is a start character
                        if (_isStartChar)
                        {
                            int nextStartChar = _rxBuffer.IndexOf(_startChar, _startPos + 1);
                            if (nextStartChar > 0 && nextStartChar < _endPos)
                            {
                                // Unexpected truncation found
                                _decodeState = DecodeStates.Idle;
                                // Remove everything in buffer up to the next start char
                                _rxBuffer = _rxBuffer.Remove(0, nextStartChar);
                                status = statusArray.packetUnexpectedlyTruncated;
                                finishedFlag = true;
                                break;
                            }
                        }

                        if (_isHeader)
                        {
                            // If so, check for dataID straight after the start character (exact match returns 0)
                            if (String.Compare(_rxBuffer, _startPos + 1, _dataIdString, 0, _dataIdString.Length) == 0)
                            {
                                // If found, switch searchAndDecodeState
                                _decodeState = DecodeStates.IdFound;
                                break;
                            }
                            else
                            {
                                // Data ID not recognized, even though start and end characters found.
                                // Reset buffer, go back to idle searchAndDecodeState and return error

                                this.ResetBuffer(ref _startPos, ref _endPos);

                                // Reset back to idle state
                                _decodeState = DecodeStates.Idle;
                                // Set error status
                                status = statusArray.dataIdDidNotMatch;
                                // Exit loop
                                finishedFlag = true;
                                break;
                            }
                        }
                        {
                            // No header so directly go to the searchAndDecodeState
                            _decodeState = DecodeStates.IdFound;
                            break;
                        }


                    case DecodeStates.IdFound:
                        // Start, end characters and data id match, time to extract data
                        if (this.DecodeCmdString(_rxBuffer.Substring(_startPos, _endPos - _startPos + 1)) == true)
                        {
                            // Goto finished state
                            _decodeState = DecodeStates.Decoded;
                            break;
                        }
                        else
                        {
                            // IF FAIL
                            // Remove the packet and everything before it from the buffer
                            _rxBuffer = _rxBuffer.Remove(0, _endPos + 1);
                            // Go back to idle searchAndDecodeState
                            _decodeState = DecodeStates.Idle;
                            status = statusArray.unrecognisedCmd;
                            finishedFlag = true;
                            break;
                        }
                    // Packet has been decoded, now to check all parameters are
                    // present
                    case DecodeStates.Decoded:
                        // Check that all the required parameters were present,
                        // and if so, go to the next state.
                        if (this.CheckParameters())
                        {
                            _decodeState = DecodeStates.RunCallBackFunc;
                            break;
                        }
                        else
                        {
                            // IF FAIL
                            // Remove the packet and everything before it from the buffer
                            _rxBuffer = _rxBuffer.Remove(0, _endPos + 1);
                            // Go back to idle searchAndDecodeState
                            _decodeState = DecodeStates.Idle;
                            status = statusArray.incorrectNumParameters;
                            finishedFlag = true;
                            break;
                        }

                        break;
                    // Run callback function
                    case DecodeStates.RunCallBackFunc:
                        this.RunCallBackFunction();
                        _decodeState = DecodeStates.Finished;
                        break;
                    // Packet decoding finished successfully!
                    case DecodeStates.Finished:
                        // IF PASS
                        // Increment packet decoded variable
                        _packetCount++;
                        // Remove the packet and everything before it from the buffer
                        _rxBuffer = _rxBuffer.Remove(0, _endPos + 1);
                        // Go back to idle searchAndDecodeState
                        _decodeState = DecodeStates.Idle;
                        // Else return o.k.
                        status = statusArray.packetDecodingPassed;
                        finishedFlag = true;
                        break;
                       
                }

            }
            return status;
        }

        /// <summary>
        /// Creates schema of table by addind columns based on the name and type of the data
        /// provided to the function
        /// </summary>
        /// <param name="dataNames"></param>
        /// <param name="dataTypes"></param>
        public void SetupDataTable(string[] dataNames, Type[] dataTypes)
        {
            // Create table
            dataTable1 = new DataTable();

            // Create index (also primary key)
            DataColumn index = new DataColumn("Index", typeof(int));
            // Add index column to table
            dataTable1.Columns.Add(index);
            // Assign the primary key of the table
            DataColumn[] primaryKeys = new DataColumn[1];
            primaryKeys[0] = index;
            dataTable1.PrimaryKey = primaryKeys;

            // Create an array of column objects
            DataColumn[] dataColumns = new DataColumn[dataNames.Length];

            // Populate column array, adding names and datatypes. Also add columns to table.
            for (int i = 0; i < dataNames.Length; i++)
            {
                // Add column object to array
                dataColumns[i] = new DataColumn(dataNames[i], dataTypes[i]);
                // Add to table
                dataTable1.Columns.Add(dataColumns[i]);
            }

        }

        /// <summary>
        /// Registers a command with the RX decoding engine. Commands must be registered with the engine
        /// before they will be recognised in the receiving character buffer.
        /// </summary>
        /// <param name="cmd">The command to register.</param>
        public void RegisterCommand(Clide.Command cmd)
        {
            // Add command to object
            _registeredCmds.Add(cmd);
        }

        #endregion

        #region Private Functions

        /// <summary>
        /// Resets the rx buffer, as well as the start and end position variables.
        /// </summary>
        /// <param name="startPos">Start position variable</param>
        /// <param name="endPos">End position variable</param>
        private void ResetBuffer(ref int startPos, ref int endPos)
        {
            _rxBuffer = "";
            startPos = 0;
            endPos = 0;
        }

        /// <summary>
        /// Extracts data from the packet and trys to match it with one of the registered commands.
        /// </summary>
        /// <param name="dataString">String of data, beginning with the start character
        /// and terminated with the end character</param>
        /// <returns>False if data extraction was unsuccessful, due to the format of the string being
        /// incorrect or a data ID present that is not expected. Returns true if successful and
        /// data was added to the table.</returns>
        private bool DecodeCmdString(string dataString)
        {

            // Strip carriage return from end
            dataString = dataString.Remove(dataString.Length - 1);

            // Find the first space. The word before this space is the command name.
            int firstSpace = dataString.IndexOf(" ");

            string cmdName;

            // Check to make sure space was found and in valid position (not the first character) 
            // If no space, command must have no parameters or options
            if (firstSpace <= 0)
            {
                cmdName = dataString;
            }
            else
            {
                // Extract a string containing only the command name
                cmdName = dataString.Remove(firstSpace);
            }

            // Used below loop
            LastCommandIndex = -1;

            // Look for registered command with the same name, when found
            // remember which one
            for(int x = 0; x < _registeredCmds.Count; x++) // Serial.Command cmd in _registeredCmds)
            {
                if (_registeredCmds[x].Name == cmdName)
                {
                    LastCommandIndex = x;
                    break;
                }
            }

            if (LastCommandIndex == -1)
                // Received command wasn't found in registered command list, return false.
                return false;

            // Set all option.IsDetected to false
            foreach (Option option in _registeredCmds[LastCommandIndex].OptionList)
            {
                option.IsDetected = false;
            }

            string linearArgs;

            // Run parameter/option decoder if they exist
            if (firstSpace != -1)
            {
                // Strip off command name
                linearArgs = dataString.Remove(0, firstSpace + 1);
            }
            else
                linearArgs = "";

            // Split command-line string into the seperate arguments,
            // just like what is passed to the main() function
            string[] arguments = SplitArguments(linearArgs);

            // Parse the arguments. This is where an external library is used
            // A command-line parser library
            //! @todo Remove dependancy on 3-rd party program
            Parameters = _registeredCmds[LastCommandIndex].p.Parse(arguments);


            return true;

         
        }

        /// <summary>
        /// Checks that all the required parameters were sent with the command.
        /// Unlike options, parameters are not optional.
        /// </summary>
        /// <returns></returns>
        private bool CheckParameters()
        {
            if (_registeredCmds[LastCommandIndex].ParamList.Count != Parameters.Count)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Runs the callback function associated with the just-received command.
        /// </summary>
        private void RunCallBackFunction()
        {
            // Make sure command has been registered
            if(_registeredCmds[LastCommandIndex].CallBackFunc != null)
                _registeredCmds[LastCommandIndex].CallBackFunc(this.Parameters);
        }

        /// <summary>
        /// Splits arguments in the command-line from one linear string into
        /// a arrary of strings. White space is the indiciator used to split
        /// arguments.
        /// </summary>
        /// <param name="commandLine">The detected command-line string.</param>
        /// <returns>An array of the arguments.</returns>
        public static string[] SplitArguments(string commandLine)
        {
            var parmChars = commandLine.ToCharArray();
            var inSingleQuote = false;
            var inDoubleQuote = false;
            for (var index = 0; index < parmChars.Length; index++)
            {
                if (parmChars[index] == '"' && !inSingleQuote)
                {
                    inDoubleQuote = !inDoubleQuote;
                    parmChars[index] = '\n';
                }
                if (parmChars[index] == '\'' && !inDoubleQuote)
                {
                    inSingleQuote = !inSingleQuote;
                    parmChars[index] = '\n';
                }
                if (!inSingleQuote && !inDoubleQuote && parmChars[index] == ' ')
                    parmChars[index] = '\n';
            }
            return (new string(parmChars)).Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
        }

        #endregion
    }

    

}

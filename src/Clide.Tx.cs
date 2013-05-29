using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;              // For accessing the serial port

namespace Clide
{
    /// <summary>
    /// Class for handling the TX transmissions.
    /// </summary>
    public class Tx
    {

        #region Constructors

        /// <summary>
        /// Simplified constructor. DebugPrint left as null.
        /// </summary>
        /// <param name="port"></param>
        public Tx(Clide.Port port)
        {
            _port = port;
        }

        /// <summary>
        /// Base constructor.
        /// </summary>
        /// <param name="port"></param>
        /// <param name="debugPrint">The function to call to pass debug information to.</param>
        public Tx(Clide.Port port, Action<string> debugPrint)
        {
            Port = port;
            DebugPrint = debugPrint;
        }

        #endregion

        #region Parameters And Backing Fields

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

        /// <summary>
        /// Backing field
        /// </summary>
        private Clide.Port _port;
        /// <summary>
        /// Handle for the port used for communication. Usually set with constructor.
        /// </summary>
        public Clide.Port Port
        { get { return _port; } set { _port = value; } }

        /// <summary>
        /// Callback function that prints debug information. Can be set with constructor.
        /// </summary>
        public Action<string> DebugPrint { get; set; }

        #endregion


        #region Methods

        /// <summary>
        /// Sends a command over the serial port
        /// </summary>
        /// <param name="cmd">Command to send over serial port.</param>
        public void SendCommand(Clide.Command cmd)
        {
            string cmdString = BuildString(cmd) + "\r\n";
            _port.serialPort.Write(cmdString);

            if (DebugPrint != null)
            {
                // Need to do this because called from another thread
                DebugPrint("Sending message. Message: " + BuildString(cmd));
            }

            // Increment TX packet count
            _packetCount++;
        }

        /// <summary>
        /// Converts a command object into a string that can
        /// be then sent across the serial.
        /// </summary>
        /// <param name="cmd"></param>
        private static string BuildString(Clide.Command cmd)
        {
            string cmdLineString;

            // Add the command name to the start of the string
            cmdLineString = cmd.Name;
            
            // Add parameters
            for (int x = 0; x < cmd.ParamList.Count; x++)
            {
                cmdLineString += ' ' + cmd.ParamList[x].Value;
            }

            // Add options
            for(int x = 0; x < cmd.OptionList.Count; x++)
            {
                // Make sure it is wanted to be sent
                if (cmd.OptionList[x].ToSend)
                {
                    cmdLineString += " -" + cmd.OptionList[x].Name;

                    // Add value also, but only if one exists
                    if (cmd.OptionList[x].HasValue)
                    {
                        cmdLineString += ' ' + cmd.OptionList[x].Value;
                    }
                }
            }
            

            return cmdLineString;
        }

        #endregion
    }
}

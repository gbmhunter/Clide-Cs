using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;              // For accessing the serial port

namespace Clide
{
    public class Port
    {

        #region Enums

        /// <summary>
        /// Enumeration of common serial RS-232 baud rates. 
        /// Can be used to populate combo boxes.
        /// More options than the default System.IO.Ports.SerialPort enumeration.
        /// </summary>
        public enum BaudRates
        {
            baud9600 = 9600,
            baud19200 = 19200,
            baud38400 = 38400,
            baud57600 = 57600,
            baud115200 = 115200,
            baud230400 = 230400,
            baud460800 = 460800,
            baud921600 = 921600
        }

        #endregion

        /// <summary>
        /// Provides access to the serial port
        /// </summary>
        public SerialPort serialPort;

        /// <summary>
        /// Opens serial port
        /// </summary>
        /// <returns>True if serial port opening was successful, otherwise false.</returns>
        public bool OpenPort()
        {
            try
            {
                if (this.IsOpen())
                    this.CloseAndDisposePort();
                serialPort.Open();
            }
            catch (Exception e)
            {
                //MessageBox.Show(Convert.ToString(e));
                return false;
            }

            return true;

        }

        /// <summary>
        /// Checks whether serial port is open. 
        /// </summary>
        /// <returns>Returns true is serial port is open, returns false if serial port is closed or null.</returns>
        public bool IsOpen()
        {
            if (serialPort != null)
                return serialPort.IsOpen;
            else
                return false;
        }

        /// <summary>
        /// Call to set-up the COM port with the standard set of parameters.
        /// </summary>
        /// <param name="baudRate">You can use the enum BaudRates to make this part easy.</param>
        /// <param name="numDataBits">The number of databits. The standard is 8.</param>
        /// <param name="portName">The name of the port to connect to, as a string. e.g. "COM3" or "COM13".</param>
        /// <param name="stopBits">The number of stop bits. The standard is 1.</param>
        /// <param name="parity">The parity. The standard is none.</param>
        /// <param name="readBufferSize">The buffer size.</param>
        public void ComPortSetup(
            int baudRate, int numDataBits, string portName, StopBits stopBits, Parity parity, int readBufferSize)
        {
            this.serialPort = new SerialPort();
            // Set up serial comms
            this.serialPort.BaudRate = baudRate;
            this.serialPort.DataBits = numDataBits;
            this.serialPort.PortName = portName;
            this.serialPort.StopBits = stopBits;
            this.serialPort.Parity = parity;
            this.serialPort.ReadBufferSize = readBufferSize; //Default dataIList

        }

        /// <summary>
        /// Closes and disposes of serial port.
        /// </summary>
        public void CloseAndDisposePort()
        {
            serialPort.Close();
            serialPort.Dispose();
        }
    }
}

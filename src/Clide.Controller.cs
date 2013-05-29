using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clide
{
    /// <summary>
    /// It is recommended that the RX processing is done in a seperate thread to the main program.
    /// ASCII characters enclosed in double quotes are analysed as a single string.
    /// </summary>
    public class Controller
    {
        public Clide.Port Port{ get; set; }
        public Clide.Tx Tx{ get; set; }
        public Clide.Rx Rx { get; set; }

        #region Constructors

        /// <summary>
        /// Base constructor.
        /// </summary>
        /// <param name="debugPrint"></param>
        public Controller(Action<string> debugPrint)
        {
            Port = new Clide.Port();
            Tx = new Clide.Tx(Port, debugPrint);
            Rx = new Clide.Rx(Port);
        }

        /// <summary>
        /// Simplified constructor. Does not setup debug printing.
        /// </summary>
        public Controller()
        {
            Port = new Clide.Port();
            Tx = new Clide.Tx(Port);
            Rx = new Clide.Rx(Port);
        }

        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clide
{
    /// <summary>
    /// Object that contains the name and value of a option that can be part of a command
    /// </summary>
    public class Option
    {
        #region Constructors

        /// <summary>
        /// Most basic constructor, only a name and if it has a value has to be specified.
        /// </summary>
        /// <param name="name"></param>
        public Option(string name, bool hasValue)
        {
            Name = name;
            HasValue = hasValue;
        }

        /// <summary>
        /// The option name, if it has a value, and callback function are required.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="callBackFunc"></param>
        public Option(string name, bool hasValue, Action<string> callBackFunc)
        {
            Name = name;
            HasValue = hasValue;
            CallBackFunc = callBackFunc;
        }

        /// <summary>
        /// The option name, if it has a value, callback function and description are required.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="hasValue"></param>
        /// <param name="callBackFunc"></param>
        /// <param name="description"></param>
        public Option(string name, bool hasValue, Action<string> callBackFunc, string description)
        {
            Name = name;
            HasValue = hasValue;
            CallBackFunc = callBackFunc;
            Description = description;
        }

        #endregion

        #region Parameters And Backing Fields

        /// <summary>
        /// Backing field
        /// </summary>
        private string _name = "";
        /// <summary>
        /// The name of the option. This is searched for in the command line string.
        /// </summary>
        public string Name
        { get { return _name; } set { _name = value; } }

        /// <summary>
        /// Backing field.
        /// </summary>
        private bool _hasValue = false;
        /// <summary>
        /// Set to true if option has a value associated with it. If false, Value will
        /// be ignored. Default is false.
        /// </summary>
        public bool HasValue
        { get { return _hasValue; } set { _hasValue = value; } }

        /// <summary>
        /// Backing field
        /// </summary>
        private string _value = "";
        /// <summary>
        /// The value of the option. This is either searched for in the command line string
        /// once the Name has been found (receiving), or added to the commad-line string (transmitting)
        /// </summary>
        public string Value
        { get { return _value; } set { _value = value; } }

        /// <summary>
        /// Backing field
        /// </summary>
        private string _description = "";
        /// <summary>
        /// The description of this option, which will be displayed if the help command is used.
        /// Only used when transmitting.
        /// </summary>
        public string Description
        { get{ return _description; } set{ _description = value; }}

        /// <summary>
        /// Backing field
        /// </summary>
        private Action<string> _callBackFunc = null;
        /// <summary>
        /// The callback function to call if this option is detected in the command-line string. Only used
        /// when recieving.
        /// </summary>
        public Action<string> CallBackFunc
        { get { return _callBackFunc; } set { _callBackFunc = value; } }

        private bool _toSend = true;
        /// <summary>
        /// Set true if you want the option sent when calling Tx.SendCommand().
        /// </summary>
        public bool ToSend
        { get { return _toSend; } set { _toSend = value; } }

        /// <summary>
        /// Backing field.
        /// </summary>
        private bool _isDetected = false;
        /// <summary>
        /// Is set to true if option was detected in the command string received of the command
        /// it is registered to.
        /// </summary>
        public bool IsDetected
        { get { return _isDetected; } set { _isDetected = value; } }

        #endregion

        #region Public Methods

        
        #endregion
    }
}

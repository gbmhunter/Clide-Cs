using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clide
{
    public class Parameter
    {
        #region Constructors

        /// <summary>
        /// Most basic constructor, nothing has to be specified.
        /// </summary>
        public Parameter()
        {
           
        }

        /// <summary>
        /// Constructor that supplies a description.
        /// </summary>
        /// <param name="description">Used when the help command is called.</param>
        public Parameter(string description)
        {
            // Remember description
            Description = description;
        }

        #endregion



        #region Parameters And Backing Fields

        /// <summary>
        /// Backing field
        /// </summary>
        private string _name;
        /// <summary>
        /// The name of the parameter. This is purely for ease-of-use.
        /// </summary>
        public string Name
            { get { return _name; } set { _name = value; } }

        /// <summary>
        /// Backing field
        /// </summary>
        private string _value;
        /// <summary>
        /// The value of the parameter. This is either set when parameter has been found (receiving), or
        /// added to the commad-line string (transmitting)
        /// </summary>
        public string Value
            { get { return _value; } set { _value = value; } }

        /// <summary>
        /// Backing field
        /// </summary>
        private string _description;
        /// <summary>
        /// The description of this option, which will be displayed if the help command is used.
        /// Only used when transmitting.
        /// </summary>
        public string Description
            { get{ return _description; } set{ _description = value; }}

        /// <summary>
        /// Backing field
        /// </summary>
        private Action<string> _callBackFunc;
        /// <summary>
        /// The callback function to call if this option is detected in the command-line string. Only used
        /// when recieving.
        /// </summary>
        public Action<string> CallBackFunc
            { get { return _callBackFunc; } set { _callBackFunc = value; } }


        #endregion

        #region Public Methods

        
        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NDesk.Options;

namespace Clide
{
    /// <summary>
    /// Used by the SerialDecoder class
    /// </summary>
    public class Command
    {
        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">The identifier to look for in the command-line.</param>
        public Command(string name)
        {
            this.Name = name;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name">The identifier to look for in the command-line.</param>
        /// <param name="callBackFunc">The function to call if this command is detected.</param>
        public Command(string name, Action<List<string>> callBackFunc)
        {
            this.Name = name;
            this._callBackFunc = callBackFunc;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name">The identifier to look for in the command-line.</param>
        /// <param name="callBackFunc">The function to call if this command is detected.</param>
        /// <param name="description">A description of the command.</param>
        public Command(string name, Action<List<string>> callBackFunc, string description)
        {
            this.Name = name;
            this.CallBackFunc = callBackFunc;
            this.Description = description;
        }

        #endregion

        #region Public Parameters And Backing Feilds

        /// <summary>
        /// Backing field
        /// </summary>
        private string _name;
        /// <summary>
        /// Gets or sets the name of the command. The name of the command is the first word in the command string
        /// which distinguishes the type of command.
        /// </summary>
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }

        /// <summary>
        /// Backing field
        /// </summary>
        private Action<List<string>> _callBackFunc;
        /// <summary>
        /// Gets or sets the callback function. This function is called whenever the specific command
        /// is found in the commmand-line string. For receive only.
        /// </summary>
        public Action<List<string>> CallBackFunc
        { get { return _callBackFunc; } set { _callBackFunc = value; } }

        /// <summary>
        /// Backing field.
        /// </summary>
        private string _description;
        /// <summary>
        /// Holds a description of the command.
        /// </summary>
        public string Description
        { get { return _description; } set { _description = value; } }

        /// <summary>
        /// Stores a list of the registered parameters.
        /// </summary>
        private List<Clide.Parameter> _paramList = new List<Clide.Parameter>();
        public List<Clide.Parameter> ParamList
        {
            get { return _paramList; }
            set { _paramList = value; }
        }

        /// <summary>
        /// Stores a list of the registered options.
        /// </summary>
        private List<Clide.Option> _optionList = new List<Clide.Option>();
        public List<Clide.Option> OptionList
        {
            get { return _optionList; }
            set { _optionList = value; }
        }

        #endregion

        #region Private Variables

       

        /// <summary>
        /// Holds info on the parameters to look for. This is the third party library.
        /// </summary>
        public OptionSet p = new OptionSet();

        #endregion

       

        #region Public Methods

        /// <summary>
        /// Registers a parameter with this particular command.
        /// </summary>
        /// <param name="param">Parameter to register.</param>
        public void RegisterParam(Clide.Parameter param)
        {
            // Add to list of registered parameters
            _paramList.Add(param);
            // This is not registered with the plugin, and this does not support
            // parameter recognition (returns parameters as a string array)
        }

        /// <summary>
        /// Registers an option with this particular command.
        /// </summary>
        /// <param name="option">Option to register to command.</param>
        public void RegisterOption(Clide.Option option)
        {
            //Action<string> action = (x) => func(x);
            _optionList.Add(option);

            //Serial.Option option = new Option("h|help", v => { isHelp = (v != null); });
            
            // Register the parameter with the OptionSet
            //p.Add(option.Name, option.Description, option.CallBackFunc);
            p.Add(option.Name, option.Description, v => { option.IsDetected = true; option.Value = v; });
        }

        #endregion
    }
}

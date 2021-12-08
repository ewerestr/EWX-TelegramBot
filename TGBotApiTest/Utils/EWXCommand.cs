namespace me.ewerestr.ewxtelegrambot.Utils
{
    public class EWXCommand
    {
        private string _command;
        private string[] _arguments;

        public EWXCommand(string command)
        {
            if (command.Contains(" "))
            {
                string[] lArray = command.Split(' ');
                _command = lArray[0].ToLower();
                string[] llArray = new string[lArray.Length - 1];
                for (int i = 1; i < lArray.Length; i++) if (i > 0) llArray[i - 1] = lArray[i];
                _arguments = llArray;
            }
            else if (!string.IsNullOrEmpty(command)) _command = command;
            // else Throw exception
        }

        public string GetCommand()
        {
            return _command;
        }

        public string[] getArgumentsArray()
        {
            return _arguments;
        }

        public string GetArguments()
        {
            if (_arguments.Length > 0)
            {
                string line = _arguments[0];
                if (_arguments.Length >= 2) for (int i = 1; i < _arguments.Length; i++) line += " " + _arguments[i];
                return line;
            }
            else return null;
        }

        public int GetArgumentsCount()
        {
            return _arguments == null ? 0 : _arguments.Length;
        }

        public bool HasArguments()
        {
            return _arguments == null ? false : (_arguments.Length > 0 ? true : false);
        }
    }
}

using System.Collections.Generic;

namespace me.ewerestr.ewxtelegrambot.Utils
{
    class EWXRequestBuilder
    {
        // vars block
        private string _request;
        private string _method; // or page
        private Dictionary<string, string> _params = new Dictionary<string, string>();

        // entry block
        public EWXRequestBuilder(string url)
        {
            _request = url;
        }

        // methods block
        public EWXRequestBuilder AddParameter(string parameterName, string parameterValue)
        {
            _params.Add(parameterName, parameterValue);
            return this;
        }

        public EWXRequestBuilder SetMethod(string method)
        {
            _method = method;
            return this;
        }

        public bool RemoveParameter(string parameterName)
        {
            if (_params.ContainsKey(parameterName))
            {
                _params.Remove(parameterName);
                return true;
            }
            return false;
        }
        
        public string BuildRequest()
        {
            string lRequest = _request;
            if (lRequest[lRequest.Length - 1].Equals('/')) lRequest += _method + "?";
            else lRequest += "/" + _method + "?";
            if (_params.Count > 0)
            {
                int counter = 1;
                foreach (string key in _params.Keys)
                {
                    lRequest += key + "=" + _params[key];
                    if (counter++ != _params.Count) lRequest += "&";
                }
            }
            return lRequest;
        }
    }
}

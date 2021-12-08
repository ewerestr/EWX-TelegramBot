using System.Collections.Generic;

namespace me.ewerestr.ewxtelegrambot.Utils
{
    public class EWXComponentConfiguration
    {
        private Dictionary<string, string> _configuration;

        public EWXComponentConfiguration()
        {
            _configuration = new Dictionary<string, string>();
        }

        public void AddConfigurationUnit(string param, string value)
        {
            _configuration.Add(param, value);
        }
        
        public int GetCount()
        {
            return _configuration.Count;
        }

        public Dictionary<string, string> GetConfiguration()
        {
            return _configuration;
        }

        public void MergeConfig(Dictionary<string, string> config)
        {
            foreach (string key in config.Keys) _configuration.Add(key, config[key]);
        }
    }
}

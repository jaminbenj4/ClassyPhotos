using System.Collections.Generic;

namespace ClassyPhotos
{
    public class RegKey
    {
        public RegKey(string keyName)
        {
            KeyName = keyName;
            Values = new Dictionary<string, string>();
        }

        public string KeyName { get; set; }

        public Dictionary<string, string> Values { get; set; }
    }
}
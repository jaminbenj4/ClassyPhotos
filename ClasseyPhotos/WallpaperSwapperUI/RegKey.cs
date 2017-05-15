using System.Collections.Generic;

namespace WallpaperSwapperUI
{

    public class RegKey
    {
        public RegKey(string keyName)
        {
            KeyName = keyName;
            Values = new Dictionary<string, string>();
        }
        public string KeyName { get; set; }
        //public List<KeyValuePair<string, string>> Values { get; set; }
        public Dictionary<string, string> Values { get; set; }
    }
    
}

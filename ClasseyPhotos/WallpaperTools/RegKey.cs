using System.Collections.Generic;

namespace WallpaperTools
{
    public class RegKey
    {
        public RegKey()
        {
            Values = new Dictionary<string, string>();
        }

        public Dictionary<string, string> Values { get; }
    }
}
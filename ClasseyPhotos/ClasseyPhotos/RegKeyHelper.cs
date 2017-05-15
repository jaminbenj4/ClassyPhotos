using System.Collections.Generic;
using Microsoft.Win32;

namespace ClassyPhotos
{
    public static class RegKeyHelper
    {
        public const string WallpaperKeyPath = @"Control Panel\Desktop";

        public static List<RegKey> GetSubkeysValue(string path, RegistryHive hive)
        {
            var result = new List<RegKey>();
            using (var hiveKey = RegistryKey.OpenBaseKey(hive, RegistryView.Default))
            using (var key = hiveKey.OpenSubKey(path))
            {
                var subkeys = key.GetSubKeyNames();

                foreach (var subkey in subkeys)
                {
                    var values = GetKeyValue(hiveKey, subkey);
                    result.Add(values);
                }
            }
            return result;
        }

        public static List<RegKey> GetSubkeysValue(RegistryKey parentKey)
        {
            var result = new List<RegKey>();
            using (var key = parentKey)
            {
                var subkeys = key.GetSubKeyNames();

                foreach (var subkey in subkeys)
                {
                    var values = GetKeyValue(key, subkey);
                    result.Add(values);
                }
            }
            return result;
        }

        private static RegKey GetKeyValue(RegistryKey parentKey, string keyName)
        {
            var result = new RegKey(keyName);
            using (var key = parentKey.OpenSubKey(keyName))
            {
                foreach (var valueName in key.GetValueNames())
                {
                    var val = key.GetValue(valueName);
                    result.Values.Add(valueName, val.ToString());
                }
            }

            return result;
        }
    }
}
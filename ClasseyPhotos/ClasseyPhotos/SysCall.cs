using System;
using System.Runtime.InteropServices;

namespace ClassyPhotos
{
    internal static class SysCall
    {
        public enum ProcessDpiAwareness
        {
            ProcessDpiUnaware = 0,
            ProcessSystemDpiAware = 1,
            ProcessPerMonitorDpiAware = 2
        }

        private const int SPI_SETDESKWALLPAPER = 20;
        private const int SPIF_UPDATEINIFILE = 0x01;
        private const int SPIF_SENDWININICHANGE = 0x02;

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);

        [DllImport("shcore.dll")]
        private static extern int SetProcessDpiAwareness(ProcessDpiAwareness value);

        public static bool EnableDpiAwareness()
        {
            try
            {
                if (Environment.OSVersion.Version.Major < 6)
                {
                    return false;
                }
                SetProcessDpiAwareness(ProcessDpiAwareness.ProcessPerMonitorDpiAware);
                return true;
            }
            catch (Exception e1)
            {
                Console.WriteLine(e1);
                return false;
            }
        }

        public static bool SetSystemWallpaper(string wallpaperFilePath)
        {
            try
            {
                SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, wallpaperFilePath,
                    SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
using System.Runtime.InteropServices;

namespace WallpaperTools
{
    public static class SysCall
    {
/*
        private enum ProcessDpiAwareness
        {
            ProcessDpiUnaware = 0,
            ProcessSystemDpiAware = 1,
            ProcessPerMonitorDpiAware = 2
        }
*/

        private const int SpiSetDeskWallpaper = 20;
        private const int SpifUpdateIniFile = 0x01;
        private const int SpifSendWinIniChange = 0x02;

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);

/*
        [DllImport("shcore.dll")]
        private static extern int SetProcessDpiAwareness(ProcessDpiAwareness value);
*/

/*
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
*/

        public static void SetSystemWallpaper(string wallpaperFilePath)
        {
            SystemParametersInfo(SpiSetDeskWallpaper, 0, wallpaperFilePath,
                SpifUpdateIniFile | SpifSendWinIniChange);
        }
    }
}
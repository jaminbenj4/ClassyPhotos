using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Microsoft.Win32;
using NLog;

namespace WallpaperTools
{
    public static class Wallpaper
    {
        public enum Style
        {
            Fill,
            Fit,
            Span,
            Stretch,
            Tile,
            Center
        }


        private const string WindowsThemesPath = @"\Microsoft\Windows\Themes";
        private const string ClassyFilename = "classyWallpaper.bmp";

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private static readonly string ClassyPath = $"\\Roaming{WindowsThemesPath}\\{ClassyFilename}";


        /*
                public static bool PaintWall(string wallFilePath, Style style)
                {
                    Image img = null;
                    try
                    {
                        img = Image.FromFile(Path.GetFullPath(wallFilePath));
                    }
                    catch (Exception e1)
                    {
                        Console.WriteLine(e1);
                    }

                    return PaintWall(img, style);
                }
        */

        public static void PaintWall(Image image, Style style)
        {
            var primaryFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var wallFilePath = Path.Combine(primaryFolder + WindowsThemesPath, ClassyFilename);

            try
            {
                var imgTemp = new Bitmap(image);
                imgTemp.Save(wallFilePath, ImageFormat.Bmp);
                Logger.Info("Wallpaper saved to primary path: " + wallFilePath);
            }
            catch (Exception e1)
            {
                Logger.Error(e1);
                return;
            }

            try
            {
                var key = Registry.CurrentUser.OpenSubKey(KeyNames.WallpaperParentKey, true);
                if (key == null)
                {
                    throw new NullReferenceException(string.Format("Registry key [{0}] does not exist",
                        KeyNames.WallpaperParentKey));
                }
                if (style == Style.Fill)
                {
                    key.SetValue(KeyNames.WallpaperStyle, 10.ToString());
                    key.SetValue(KeyNames.TileWallpaper, 0.ToString());
                }
                if (style == Style.Fit)
                {
                    key.SetValue(KeyNames.WallpaperStyle, 6.ToString());
                    key.SetValue(KeyNames.TileWallpaper, 0.ToString());
                }
                if (style == Style.Span) // Windows 8 or newer only!
                {
                    key.SetValue(KeyNames.WallpaperStyle, 22.ToString());
                    key.SetValue(KeyNames.TileWallpaper, 0.ToString());
                }
                if (style == Style.Stretch)
                {
                    key.SetValue(KeyNames.WallpaperStyle, 2.ToString());
                    key.SetValue(KeyNames.TileWallpaper, 0.ToString());
                }
                if (style == Style.Tile)
                {
                    key.SetValue(KeyNames.WallpaperStyle, 0.ToString());
                    key.SetValue(KeyNames.TileWallpaper, 1.ToString());
                }
                if (style == Style.Center)
                {
                    key.SetValue(KeyNames.WallpaperStyle, 0.ToString());
                    key.SetValue(KeyNames.TileWallpaper, 0.ToString());
                }

                SysCall.SetSystemWallpaper(wallFilePath);

                Logger.Info("wallpaper set to " + wallFilePath);
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }

        public static RegKey GetWallKeys()
        {
            var key = new RegKey();
            using (var userWallpaper = Registry.CurrentUser.OpenSubKey(KeyNames.WallpaperParentKey, false))
            {
                if (userWallpaper == null)
                {
                    return null;
                }
                var path = userWallpaper.GetValue(KeyNames.WallpaperPath);
                var style = userWallpaper.GetValue(KeyNames.WallpaperStyle);
                var tile = userWallpaper.GetValue(KeyNames.TileWallpaper);

                //set to style:fit and tile:no by default if no keys are set
                if (style == null)
                {
                    style = 6;
                }
                if (tile == null)
                {
                    tile = 0;
                }

                key.Values.Add(KeyNames.WallpaperPath, path.ToString());
                key.Values.Add(KeyNames.WallpaperStyle, style.ToString());
                key.Values.Add(KeyNames.TileWallpaper, tile.ToString());
            }

            return key;
        }

        public static class KeyNames
        {
            public const string WallpaperParentKey = @"Control Panel\Desktop";
            public const string WallpaperPath = "Wallpaper";
            public const string WallpaperStyle = "WallpaperStyle";
            public const string TileWallpaper = "TileWallpaper";
        }
    }
}
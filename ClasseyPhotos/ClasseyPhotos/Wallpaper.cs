using System;
using System.Drawing;
using System.IO;
using Microsoft.Win32;

namespace ClassyPhotos
{
    public static class Wallpaper
    {

        public static class KeyNames
        {
            public const string WallpaperParentKey = @"Control Panel\Desktop";
            public const string WallpaperPath = @"Wallpaper";
            public const string WallpaperStyle = @"WallpaperStyle";
            public const string TileWallpaper = @"TileWallpaper";

        }
        public enum Style
        {
            Fill,
            Fit,
            Span,
            Stretch,
            Tile,
            Center
        }

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

        public static bool PaintWall(Image image, Style style)
        {
            var primaryFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string destWallFilePath = Path.Combine(primaryFolder + @"\Microsoft\Windows\Themes", "rollerWallpaper.bmp");

            Image img = null;
            Bitmap imgTemp = null;
            try
            {
                img = image;
                imgTemp = new Bitmap(img);
                imgTemp.Save(destWallFilePath, System.Drawing.Imaging.ImageFormat.Bmp);
                Console.WriteLine("Wallpaper saved to primary path: " + destWallFilePath);
            }
            catch (Exception e1)
            {
                Console.WriteLine(e1);
                try
                {
                    var secondaryFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
                    destWallFilePath = Path.Combine(secondaryFolder, "rollerWallpaper.bmp");
                    imgTemp.Save(destWallFilePath, System.Drawing.Imaging.ImageFormat.Bmp);
                    Console.WriteLine("Wallpaper saved to secondary path: " + destWallFilePath);
                }
                catch (Exception e2)
                {
                    Console.WriteLine(e2);
                    return false;
                }
            }

            try
            {
                RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop", true);

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

                SysCall.SetSystemWallpaper(destWallFilePath);

                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }

        public static void GetWall()
        {
            RegistryKey UserWallpaper = Registry.CurrentUser.OpenSubKey(KeyNames.WallpaperParentKey, false);
            var breakpoint = 0;
        }
        public static RegKey GetWallKeys()
        {
            var key = new RegKey("Wallpaper");
            using (var userWallpaper = Registry.CurrentUser.OpenSubKey(KeyNames.WallpaperParentKey, false))
            {
                if (userWallpaper == null)
                {
                    return null;
                }
                var path = userWallpaper.GetValue(KeyNames.WallpaperPath);
                var style = userWallpaper.GetValue(KeyNames.WallpaperStyle);
                var tile = userWallpaper.GetValue(KeyNames.TileWallpaper);

                key.Values.Add(KeyNames.WallpaperPath, path.ToString());
                key.Values.Add(KeyNames.WallpaperStyle, style.ToString());
                key.Values.Add(KeyNames.TileWallpaper, tile.ToString());
            }

            return key;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Security.Cryptography;
using System.Timers;
using Microsoft.Win32;
using NLog;

namespace WallpaperTools
{
    public class Swapper
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly int _checkInterval;
        private readonly List<string> _hashList;
        private readonly List<Image> _images;

        private readonly Random _rng;
        private readonly Timer _statusCheckTimer;
        private readonly int _swapMaxDuration;
        private readonly int _swapMinDuration;
        private readonly Timer _swapTimer;
        private RegKey _backupWallKey;
        private bool _desktopIsClassy;
        private int _prevImageIndex;
        private bool _swapped;

        /// <summary>
        ///     Swaps desktop wallpaper at random intervals
        /// </summary>
        /// <param name="images">images to set as wallpaper</param>
        /// <param name="swapMinDuration">minimum time (sec) to display normal wallpaper</param>
        /// <param name="swapMaxDuration">maximum time (sec) to display normal wallpaper</param>
        /// <param name="checkInterval">interval to (sec) to check desktop state</param>
        public Swapper(List<Image> images, int swapMinDuration, int swapMaxDuration, int checkInterval)
        {
            _swapMinDuration = swapMinDuration;
            _swapMaxDuration = swapMaxDuration;

            _checkInterval = checkInterval;
            _swapTimer = new Timer();
            _statusCheckTimer = new Timer();
            _desktopIsClassy = false;
            _swapped = false;
            _rng = new Random();

            _images = images;
            using (var hasher = new Hasher())
            {
                _hashList = hasher.ComputeHashes(_images);
            }

            _prevImageIndex = -1;

            Logger.Debug("Swapper init");
        }

        public void Start()
        {
            Logger.Debug("Swapper.Start() entered");

            _swapTimer.Interval = RandomInterval();
            _swapTimer.Elapsed += SwapTimerElapsed;
            Logger.Debug("swapper timer event setup");

            _statusCheckTimer.Interval = _checkInterval;
            _statusCheckTimer.Elapsed += StatusCheckTimerElapsed;
            Logger.Debug("status check timer event setup");

            _swapTimer.Start();
            Logger.Debug("swapper timer started");

            _statusCheckTimer.Start();
            Logger.Debug("status check timer started");

            Logger.Debug("swapper started");
        }

        /// <summary>
        ///     stop the swapper and restore the original wallpaper
        /// </summary>
        public void Stop()
        {
            _swapTimer.Stop();
            _statusCheckTimer.Stop();
            Restore();
        }

        private void SwapTimerElapsed(object sender, ElapsedEventArgs e)
        {
            Logger.Debug("swap timer ellapsed");

            _swapTimer.Stop();
            Swap();
        }

        private void StatusCheckTimerElapsed(object sender, ElapsedEventArgs e)
        {
            _desktopIsClassy = GetWallpaperStatus();

            //if user has changed wallpaper back, restart the swap timer
            if (_swapped && !_desktopIsClassy)
            {
                _swapped = false;

                _swapTimer.Stop();
                _swapTimer.Interval = RandomInterval();
                _swapTimer.Start();
            }
        }

        public void Swap()
        {
            var randomIndex = _rng.Next(0, _images.Count);
            //don't repeat images
            while (randomIndex == _prevImageIndex)
            {
                randomIndex = _rng.Next(0, _images.Count);
            }
            var image = _images[randomIndex];
            _prevImageIndex = randomIndex;
            Set(image);

            _swapped = true;
        }

        /// <summary>
        ///     Sets desktop wallpaper to specified image
        /// </summary>
        /// <param name="image">image to set wallpaper to</param>
        private void Set(Image image)
        {
            Backup();
            Wallpaper.PaintWall(image, Wallpaper.Style.Fill);
        }

        /// <summary>
        ///     backup the current wallpaper registry keys
        /// </summary>
        private void Backup()
        {
            _backupWallKey = Wallpaper.GetWallKeys();
            Logger.Info("wallpaper backup successful");
        }

        /// <summary>
        ///     Restore wallpaper back to its original image
        /// </summary>
        public void Restore()
        {
            try
            {
                using (var key = Registry.CurrentUser.OpenSubKey(Wallpaper.KeyNames.WallpaperParentKey, true))
                {
                    if (key == null)
                    {
                        return;
                    }

                    //restore reg keys for style and tiling
                    key.SetValue(Wallpaper.KeyNames.WallpaperStyle,
                        _backupWallKey.Values[Wallpaper.KeyNames.WallpaperStyle]);
                    key.SetValue(Wallpaper.KeyNames.TileWallpaper,
                        _backupWallKey.Values[Wallpaper.KeyNames.TileWallpaper]);

                    //restore wallpaper image
                    SysCall.SetSystemWallpaper(_backupWallKey.Values[Wallpaper.KeyNames.WallpaperPath]);
                }
            }
            catch (Exception exception)
            {
                Logger.Error(exception);
            }

            Logger.Info("wallpaper restored");
        }

        public bool GetWallpaperStatus()
        {
            //hash desktop to see if set by user or the swapper
            var wallpaper = Wallpaper.GetWallKeys();
            var path = wallpaper.Values[Wallpaper.KeyNames.WallpaperPath];
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(path))
                {
                    var imgTemp = new Bitmap(Image.FromFile(path));
                    using (var memoryStream = new MemoryStream())
                    {
                        imgTemp.Save(memoryStream, ImageFormat.Bmp);
                        //TODO: compare bytes from resx and wallpaper file
                        var bytes = memoryStream.ToArray();
                        var memHash = BitConverter.ToString(md5.ComputeHash(memoryStream)).Replace("-", "").ToLower();
                        var breakpoint = 0;
                    }

                    var hash = BitConverter.ToString(md5.ComputeHash(stream)).Replace("-", "").ToLower();
                    return _hashList.Contains(hash);
                }
            }
        }

        private int RandomInterval()
        {
            return _rng.Next(_swapMinDuration * 1000, _swapMaxDuration * 1000);
        }
    }
}
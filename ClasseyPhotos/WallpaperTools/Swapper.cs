using System;
using System.Collections.Generic;
using System.Drawing;
using System.Timers;
using Microsoft.Win32;
using NLog;

namespace WallpaperTools
{
    public class Swapper
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly int checkInterval;
        private readonly List<string> hashList;
        private readonly List<Image> images;

        private readonly Random rng;
        private readonly Timer statusCheckTimer;
        private readonly int swapMaxDuration;
        private readonly int swapMinDuration;
        private readonly Timer swapTimer;
        private RegKey backupWallKey;
        private bool desktopIsClassy;
        private int prevImageIndex;
        private bool swapped;

        /// <summary>
        ///     Swaps desktop wallpaper at random intervals
        /// </summary>
        /// <param name="images">images to set as wallpaper</param>
        /// <param name="hashList">hashes for the images</param>
        /// <param name="swapMinDuration">minimum time (sec) to display normal wallpaper</param>
        /// <param name="swapMaxDuration">maximum time (sec) to display normal wallpaper</param>
        /// <param name="checkInterval">interval to (sec) to check desktop state</param>
        public Swapper(List<Image> images, List<string> hashList, int swapMinDuration, int swapMaxDuration,
            int checkInterval)
        {
            this.swapMinDuration = swapMinDuration;
            this.swapMaxDuration = swapMaxDuration;

            this.checkInterval = checkInterval;
            swapTimer = new Timer();
            statusCheckTimer = new Timer();
            desktopIsClassy = false;
            swapped = false;
            rng = new Random();

            this.images = images;
            this.hashList = hashList;

            prevImageIndex = -1;

            Logger.Debug("Swapper init");
        }

        public void Start()
        {
            Logger.Debug("Swapper.Start() entered");

            swapTimer.Interval = RandomInterval();
            swapTimer.Elapsed += SwapTimerElapsed;
            Logger.Debug("swapper timer event setup");

            statusCheckTimer.Interval = checkInterval;
            statusCheckTimer.Elapsed += StatusCheckTimerElapsed;
            Logger.Debug("status check timer event setup");

            swapTimer.Start();
            Logger.Debug("swapper timer started");

            statusCheckTimer.Start();
            Logger.Debug("status check timer started");

            Logger.Debug("swapper started");
        }

        /// <summary>
        ///     stop the swapper and restore the original wallpaper
        /// </summary>
        public void Stop()
        {
            swapTimer.Stop();
            statusCheckTimer.Stop();
            Restore();
        }

        private void SwapTimerElapsed(object sender, ElapsedEventArgs e)
        {
            Logger.Debug("swap timer ellapsed");

            swapTimer.Stop();
            Swap();
        }

        private void StatusCheckTimerElapsed(object sender, ElapsedEventArgs e)
        {
            desktopIsClassy = GetWallpaperSwappedStatus();

            //if user has changed wallpaper back, restart the swap timer
            if (swapped && !desktopIsClassy)
            {
                swapped = false;

                swapTimer.Stop();
                swapTimer.Interval = RandomInterval();
                swapTimer.Start();
            }
        }

        public void Swap()
        {
            var randomIndex = rng.Next(0, images.Count);
            //don't repeat images
            while (randomIndex == prevImageIndex)
            {
                randomIndex = rng.Next(0, images.Count);
            }
            var image = images[randomIndex];
            prevImageIndex = randomIndex;
            Set(image);

            swapped = true;
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
            backupWallKey = Wallpaper.GetWallKeys();
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
                        backupWallKey.Values[Wallpaper.KeyNames.WallpaperStyle]);
                    key.SetValue(Wallpaper.KeyNames.TileWallpaper,
                        backupWallKey.Values[Wallpaper.KeyNames.TileWallpaper]);

                    //restore wallpaper image
                    SysCall.SetSystemWallpaper(backupWallKey.Values[Wallpaper.KeyNames.WallpaperPath]);
                }
            }
            catch (Exception exception)
            {
                Logger.Error(exception);
            }

            Logger.Info("wallpaper restored");
        }

        /// <summary>
        ///     check if wallpaper has been swapped or not
        /// </summary>
        /// <returns></returns>
        public bool GetWallpaperSwappedStatus()
        {
            using (var hasher = new Hasher())
            {
                //hash desktop to see if set by user or the swapper
                var wallpaper = Wallpaper.GetWallKeys();
                var path = wallpaper.Values[Wallpaper.KeyNames.WallpaperPath];
                var hash = hasher.ComputeImageHashFromPath(path);
                if (hashList.Contains(hash))
                {
                    return true;
                }
                return false;
            }
        }

        private int RandomInterval()
        {
            return rng.Next(swapMinDuration * 1000, swapMaxDuration * 1000);
        }
    }
}
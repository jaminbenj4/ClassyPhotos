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
        private const int CheckInterval = 60000; // check inactive and wallpaper status every minute (60*1000ms)
        private readonly int swapChance;    // 1/x chance of swap
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly List<string> hashList;
        private readonly int idleThreshold;

        private readonly List<Image> images;
        private readonly Random rng;
        private readonly Timer statusCheckTimer;
        private RegKey backupWallKey;
        private bool desktopIsClassy;
        private bool idleStatus;
        private bool prevIdleStatus;
        private int prevImageIndex;

        /// <summary>
        ///     Swaps desktop wallpaper at random intervals
        /// </summary>
        /// <param name="images">images to set as wallpaper</param>
        /// <param name="hashList">hashes for the images</param>
        /// <param name="idleThreshold">how long to wait (sec) before swap</param>
        /// <param name="swapChance">chance of swap (1/x)</param>
        public Swapper(List<Image> images, List<string> hashList, int idleThreshold, int swapChance)
        {
            this.images = images;
            this.hashList = hashList;

            this.idleThreshold = idleThreshold;
            this.swapChance = swapChance;

            statusCheckTimer = new Timer();
            rng = new Random();
            desktopIsClassy = false;
            idleStatus = false;
            prevIdleStatus = false;
            prevImageIndex = -1;

            Logger.Debug("Swapper init");
        }

        public void Start()
        {
            Logger.Debug("Swapper.Start() entered");

            statusCheckTimer.Interval = CheckInterval;
            statusCheckTimer.Elapsed += StatusCheckTimerElapsed;
            Logger.Debug("status check timer event setup");

            statusCheckTimer.Start();
            Logger.Debug("status check timer started");

            Logger.Debug("swapper started");
        }

        /// <summary>
        ///     stop the swapper and restore the original wallpaper
        /// </summary>
        public void Stop()
        {
            statusCheckTimer.Stop();
            Restore();
        }


        private void StatusCheckTimerElapsed(object sender, ElapsedEventArgs e)
        {
            desktopIsClassy = GetWallpaperSwappedStatus();

            // get idle status
            prevIdleStatus = idleStatus;
            var idleTime = TimeSpan.FromMilliseconds(IdleTimeFinder.GetIdleTime());
            idleStatus = idleTime > TimeSpan.FromSeconds(idleThreshold);

            // only swap on transition to idle, not every minute after

            if (!desktopIsClassy && idleStatus && !prevIdleStatus)
            {
                //  1/x chance to swap
                var rand = rng.Next(swapChance);
                if (rand == 1)
                {
                    Swap();
                }
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
                return hashList.Contains(hash);
            }
        }
    }
}
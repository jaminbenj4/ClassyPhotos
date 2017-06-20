using System;
using System.Collections.Generic;
using System.Drawing;
using System.Timers;
using Microsoft.Win32;
using NLog;
using WallpaperTools.Properties;

namespace WallpaperTools
{
    public class Swapper
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly int _classyMaxDuration;
        private readonly int _classyMinDuration;
        private readonly List<Image> _images;
        private readonly int _normalMaxDuration;
        private readonly int _normalMinDuration;
        private readonly Random _rng;
        private readonly Timer _timer;
        private RegKey _backupWallKey;
        private bool _desktopIsClassy;
        private int _prevImageIndex;

        /// <summary>
        ///     Swaps desktop wallpaper at random intervals
        /// </summary>
        /// <param name="normalMinDuration">minimum time (sec) to display normal wallpaper</param>
        /// <param name="normalMaxDuration">maximum time (sec) to display normal wallpaper</param>
        /// <param name="classyMinDuration">minimum time (sec) to display classy wallpaper</param>
        /// <param name="classyMaxDuration">maximum time (sec) to display classy wallpaper</param>
        public Swapper(int normalMinDuration, int normalMaxDuration, int classyMinDuration, int classyMaxDuration)
        {
            _normalMinDuration = normalMinDuration;
            _normalMaxDuration = normalMaxDuration;
            _classyMinDuration = classyMinDuration;
            _classyMaxDuration = classyMaxDuration;
            _timer = new Timer();
            _desktopIsClassy = false;
            _rng = new Random();

            //add the images you want from the resx here
            _images = new List<Image>
            {
                Resources.bieber01,
                Resources.bieber02,
                Resources.hoff01,
                Resources.hoff02,
                Resources.unicorn01
            };
            _prevImageIndex = -1;

            Logger.Debug("Swapper init");
        }

        public void Start()
        {
            Logger.Debug("Swapper.Start() entered");

            _timer.Interval = NormalRandomInterval();
            _timer.Elapsed += TimerElapsed;
            Logger.Debug("swapper timer event setup");
            _timer.Start();
            Logger.Debug("swapper timer started");

            Logger.Debug("swapper started");
        }

        /// <summary>
        ///     stop the swapper and restore the original wallpaper
        /// </summary>
        public void Stop()
        {
            _timer.Stop();
            Restore();
        }

        private void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            Logger.Debug("timer ellapsed");

            _timer.Stop();

            if (!_desktopIsClassy)
            {
                //get classy
                Swap();
                _timer.Interval = ClassyRandomInterval();
            }
            else
            {
                //back to normal and boring
                Restore();
                _timer.Interval = NormalRandomInterval();
            }
            _desktopIsClassy = !_desktopIsClassy;
            _timer.Start();

            Logger.Debug("timer started");
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
        ///     Sets desktop wallpaper to specified image
        /// </summary>
        /// <param name="image">image to set wallpaper to</param>
        private void Set(Image image)
        {
            Backup();
            Wallpaper.PaintWall(image, Wallpaper.Style.Fill);
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

        private int ClassyRandomInterval()
        {
            return _rng.Next(_classyMinDuration * 1000, _classyMaxDuration * 1000);
        }

        private int NormalRandomInterval()
        {
            return _rng.Next(_normalMinDuration * 1000, _normalMaxDuration * 1000);
        }
    }
}
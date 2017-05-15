using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Timers;
using System.Threading.Tasks;
using Microsoft.Win32;
using Microsoft.Win32.SafeHandles;
using WallpaperSwapperUI;
using static WallpaperSwapperUI.SysCall;

namespace ClassyPhotos
{
    class ClassyPhotosStart : IClassyPhotosStart, IDisposable
    {
        private Timer _timer;
        //private DateTime _lastRun = DateTime.Now.AddMinutes()
        SafeHandle handle = new SafeFileHandle(IntPtr.Zero, true);
        private bool disposed = false;
        private RegKey _backupWallKey;


        public ClassyPhotosStart()
        {
            
        }

        public void CheckForPhotos()
        {
            try
            {
                _timer = new Timer();
                _timer.Interval = 10000;
                _timer.Elapsed += new ElapsedEventHandler(timer_Elapsed);
                _timer.Start();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            

            // stop the timer while we are running the cleanup task
            _timer.Stop();
            //todo initialize if not initialized or if they found the photos
            //todo change photo to random photo

            _timer.Interval = 1000 * 60 * 1;  //minutes in milliseconds todo change to rng
            _timer.Start();

        }

        public void Run()
        {
            CheckForPhotos();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                handle.Dispose();
                StopProcessing();
            }

            disposed = true;
        }

        public void StopProcessing()
        {
            if (_timer.Enabled)
            {
                _timer.Stop();
            }
            _timer.Dispose();
            _timer = null;
        }

        private void BackupWallpaper()
        {
            _backupWallKey = Wallpaper.GetWallKeys();
        }

        private void SetWallpaper()
        {
            BackupWallpaper();
            Image image = Properties.Resources.lolFace;
            Wallpaper.PaintWall(image, Wallpaper.Style.Center);
        }

        private void RestoreWallpaper()
        {
            try
            {
                using (var key = Registry.CurrentUser.OpenSubKey(RegKeyHelper.WallpaperKeyPath, true))
                {
                    if (key == null)
                    {
                        return;
                    }

                    //restore reg keys for style and tiling
                    key.SetValue(Wallpaper.KeyNames.WallpaperStyle, _backupWallKey.Values[Wallpaper.KeyNames.WallpaperStyle]);
                    key.SetValue(Wallpaper.KeyNames.TileWallpaper, _backupWallKey.Values[Wallpaper.KeyNames.TileWallpaper]);

                    //restore wallapaper image
                    SetSystemWallpaper(_backupWallKey.Values[Wallpaper.KeyNames.WallpaperPath]);
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }
    }
}

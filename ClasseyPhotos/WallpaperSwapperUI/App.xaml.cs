﻿using System.Windows;

namespace WallpaperSwapperUI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {

        //private TaskbarIcon _notifyIcon;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            //make the window so the code runs, but don't show it
            MainWindow = new MainWindow();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            //_notifyIcon.Dispose(); //the icon would clean up automatically, but this is cleaner
            base.OnExit(e);
        }


    }
}

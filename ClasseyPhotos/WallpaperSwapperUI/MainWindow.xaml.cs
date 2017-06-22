using System.Collections.Generic;
using System.Drawing;
using System.Windows;
using WallpaperTools;

namespace WallpaperSwapperUI
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private const int SwapMinDuration = 5;
        private const int SwapMaxDuration = 10;
        private const int CheckInterval = 10;
        private readonly Swapper _swapper;
        private readonly List<Image> _images;

        public MainWindow()
        {
            InitializeComponent();

            //add the images you want from the .resx here
            _images = new List<Image>
            {
                Properties.Resources.bieber01,
                Properties.Resources.bieber01,
                Properties.Resources.bieber02,
                Properties.Resources.hoff01,
                Properties.Resources.hoff02,
                Properties.Resources.unicorn01
            };

            _swapper = new Swapper(_images, SwapMinDuration, SwapMaxDuration, CheckInterval);
            //_swapper.Start();
        }


        private void SetWallpaperButton_Click(object sender, RoutedEventArgs e)
        {
            _swapper.Swap();
        }


        private void RestoreButton_Click(object sender, RoutedEventArgs e)
        {
            _swapper.Restore();
        }

        private void RunSwapperButton_Click(object sender, RoutedEventArgs e)
        {
            _swapper.Start();
        }

        private void GetWallpaperButton_Click(object sender, RoutedEventArgs e)
        {
            var wallpaper = Wallpaper.GetWallKeys();
            var path = wallpaper.Values["Wallpaper"];
            WallpaperPathTextbox.Text = path;
        }

        private void ClassyCheckButton_Click(object sender, RoutedEventArgs e)
        {
            var isClassy = Wallpaper.IsClassy();
            IsClassyCheckbox.IsChecked = isClassy;
        }

        private void StopSwapperButton_Click(object sender, RoutedEventArgs e)
        {
            _swapper.Stop();
        }
    }
}
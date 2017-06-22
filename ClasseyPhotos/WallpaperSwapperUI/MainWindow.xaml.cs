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

        public MainWindow()
        {
            InitializeComponent();
            _swapper = new Swapper(SwapMinDuration, SwapMaxDuration, CheckInterval);
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
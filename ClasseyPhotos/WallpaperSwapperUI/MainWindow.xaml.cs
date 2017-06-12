using System.Windows;
using WallpaperTools;

namespace WallpaperSwapperUI
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private const int ClassyMinDuration = 5;
        private const int ClassyMaxDuration = 10;
        private const int NormalMinDuration = 5;
        private const int NormalMaxDuration = 10;
        private readonly Swapper _swapper;

        public MainWindow()
        {
            InitializeComponent();
            _swapper = new Swapper(NormalMinDuration, NormalMaxDuration, ClassyMinDuration, ClassyMaxDuration);
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
    }
}
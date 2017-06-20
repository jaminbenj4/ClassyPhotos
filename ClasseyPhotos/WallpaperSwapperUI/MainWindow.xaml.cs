using System.Windows;
using WallpaperTools;

namespace WallpaperSwapperUI
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private const int ClassyMinDuration = 60*5;         //5 minutes
        private const int ClassyMaxDuration = 60*60*1;      //1 hour
        private const int NormalMinDuration = 60*10;        //10 minutes 
        private const int NormalMaxDuration = 60*60*6;      //6 hours
        private readonly Swapper _swapper;

        public MainWindow()
        {
            InitializeComponent();
            _swapper = new Swapper(NormalMinDuration, NormalMaxDuration, ClassyMinDuration, ClassyMaxDuration);
            _swapper.Start();
        }


        private void SetWallpaperButton_Click(object sender, RoutedEventArgs e)
        {
            //_swapper.Swap();
        }


        private void RestoreButton_Click(object sender, RoutedEventArgs e)
        {
            //_swapper.Restore();
        }

        private void RunSwapperButton_Click(object sender, RoutedEventArgs e)
        {
            //_swapper.Start();
        }
    }
}
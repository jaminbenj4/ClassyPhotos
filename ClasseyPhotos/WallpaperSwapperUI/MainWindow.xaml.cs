using System.Collections.Generic;
using System.Drawing;
using System.IO;
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
        private readonly Swapper swapper;
        private readonly List<Image> images;


        public MainWindow()
        {
            InitializeComponent();

            //add the images you want from the .resx here
            images = new List<Image>
            {
                Properties.Resources.bieber01,
                Properties.Resources.bieber02,
                Properties.Resources.hoff01,
                Properties.Resources.hoff02,
                Properties.Resources.unicorn01,
                Properties.Resources.lolFace
            };

            // get this by running UI mode and clicking the "Gen Hashes" button
            var hashList = new List<string>
            {
                "8067680c232b51b1f14b829143e73d56",
                "2aa6ae2167918eda2180c7bf9d8d9814",
                "3c110776e4ea4cd70a32edb0f8a6973b",
                "92e70a1c0d862aebf63deea3aaa47508",
                "d63619af9b7a8ee700269cf66e0eb879",
                "fefccd7a39cb96649661af8d0a2771fe"
            };

            swapper = new Swapper(images, hashList, SwapMinDuration, SwapMaxDuration, CheckInterval);
            swapper.Start();
        }


        private void SetWallpaperButton_Click(object sender, RoutedEventArgs e)
        {
            swapper.Swap();
        }


        private void RestoreButton_Click(object sender, RoutedEventArgs e)
        {
            swapper.Restore();
        }

        private void RunSwapperButton_Click(object sender, RoutedEventArgs e)
        {
            swapper.Start();
        }

        private void GetWallpaperButton_Click(object sender, RoutedEventArgs e)
        {
            var wallpaper = Wallpaper.GetWallKeys();
            var path = wallpaper.Values["Wallpaper"];
            WallpaperPathTextbox.Text = path;
        }

        private void ClassyCheckButton_Click(object sender, RoutedEventArgs e)
        {
            var isClassy = swapper.GetWallpaperSwappedStatus();
            IsClassyCheckbox.IsChecked = isClassy;
        }

        private void StopSwapperButton_Click(object sender, RoutedEventArgs e)
        {
            swapper.Stop();
        }

        private void GenHashesButton_Click(object sender, RoutedEventArgs e)
        {
            using (var hasher = new Hasher())
            {
                var hashes = new List<string>();
                foreach (var image in images)
                {
                    Wallpaper.PaintWall(image, Wallpaper.Style.Fill);
                    var wallpaper = Wallpaper.GetWallKeys();
                    var path = wallpaper.Values[Wallpaper.KeyNames.WallpaperPath];
                    var hash = hasher.ComputeImageHashFromPath(path);

                    hashes.Add($"\"{hash}\",");
                }
                File.WriteAllLines(@"C:\Users\Public\ClassyHashes.txt", hashes);
            }
        }
    }
}
using System;
using System.Drawing;
using System.IO;
using System.Windows;
using Microsoft.Win32;

namespace WallpaperSwapperUI
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private RegKey _backupWallKey;
        private string _filename = string.Empty;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void PickFileButton_Click(object sender, RoutedEventArgs e)
        {
            // Create OpenFileDialog 
            var dlg = new OpenFileDialog
            {
                DefaultExt = ".jpg",
                Filter = "JPEG Files (*.jpeg)|*.jpeg|JPG Files (*.jpg)|*.jpg"
            };

            // Display OpenFileDialog by calling ShowDialog method 
            var result = dlg.ShowDialog();

            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                // Open document 
                _filename = dlg.FileName;
                FileNameTextBox.Text = _filename;
            }
        }

        private void EncodeButton_Click(object sender, RoutedEventArgs e)
        {
            using (var image = Image.FromFile(_filename))
            {
                using (var m = new MemoryStream())
                {
                    image.Save(m, image.RawFormat);
                    var imageBytes = m.ToArray();

                    //convert byte[] to Base64 string
                    var base64String = Convert.ToBase64String(imageBytes);
                    Base64StringTextBox.Text = base64String;
                }
            }
        }

        private void SetWallpaperButton_Click(object sender, RoutedEventArgs e)
        {
            BackupWallpaper();
            Image image = Properties.Resources.daveeeeen_face;
            Wallpaper.PaintWall(image, Wallpaper.Style.Center);
        }

        private void GetWallpaperButton_Click(object sender, RoutedEventArgs e)
        {
            BackupWallpaper();
        }

        private void BackupWallpaper()
        {
            _backupWallKey = Wallpaper.GetWallKeys();
        }

        private void RestoreButton_Click(object sender, RoutedEventArgs e)
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
                    SysCall.SetSystemWallpaper(_backupWallKey.Values[Wallpaper.KeyNames.WallpaperPath]);
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }
    }
}
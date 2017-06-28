using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Security.Cryptography;

namespace WallpaperTools
{
    public class Hasher : IDisposable
    {
        private readonly MD5 _md5;

        public Hasher()
        {
            _md5 = MD5.Create();
        }


        public void Dispose()
        {
            _md5?.Dispose();
        }

        public List<string> ComputeHashes(IEnumerable<Image> images)
        {
            var hashList = new List<string>();

            foreach (var image in images)
            {
                var hash = ComputeImageHash(image);
                hashList.Add(hash);
            }

            return hashList;
        }

        public string ComputeImageHash(Image image)
        {
            using (var memoryStream = new MemoryStream())
            {
                image.Save(memoryStream, ImageFormat.Bmp);
                var hash = BitConverter.ToString(_md5.ComputeHash(memoryStream)).Replace("-", "").ToLower();
                return hash;
            }
        }
    }
}
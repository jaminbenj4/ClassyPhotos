using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using NUnit.Framework.Internal;

namespace WallpaperTools.Tests
{
    [TestFixture()]
    class HasherTests
    {
        [Test]
        public void SameImageShouldHaveSameHash()
        {
            Image image = Properties.Resources.lolFace;

            //set wallpaper
            Wallpaper.PaintWall(image, Wallpaper.Style.Fill);
            //try different styles? see if that changes the actual wallpaper file

            //hash wallpaper

            //hash resource

            //save resource to tmp then hash temp

            //compare all hashes
            
            using (var hasher = new Hasher())
            {
                
            }
        }
    }
}

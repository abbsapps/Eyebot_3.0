using System;
using System.Drawing;
using Eyebot3;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestProject1
{
    [TestClass]
    public class UnitTest1
    {
        public Bitmap TestBitmap()
        {
            var bitmap = new Bitmap(5, 5);
            bitmap.SetPixel(0, 0, Color.FromArgb(1, 1, 1));
            bitmap.SetPixel(0, 1, Color.FromArgb(7, 7, 7));
            bitmap.SetPixel(0, 2, Color.FromArgb(6, 6, 6));
            bitmap.SetPixel(0, 3, Color.FromArgb(4, 4, 4));
            bitmap.SetPixel(0, 4, Color.FromArgb(8, 8, 8));
            bitmap.SetPixel(1, 0, Color.FromArgb(3, 3, 3));
            bitmap.SetPixel(1, 1, Color.FromArgb(4, 4, 4));
            bitmap.SetPixel(1, 2, Color.FromArgb(5, 5, 5));
            bitmap.SetPixel(1, 3, Color.FromArgb(0, 0, 0));
            bitmap.SetPixel(1, 4, Color.FromArgb(2, 2, 2));
            bitmap.SetPixel(2, 0, Color.FromArgb(4, 4, 4));
            bitmap.SetPixel(2, 1, Color.FromArgb(2, 2, 2));
            bitmap.SetPixel(2, 2, Color.FromArgb(5, 5, 5));
            bitmap.SetPixel(2, 3, Color.FromArgb(8, 8, 8));
            bitmap.SetPixel(2, 4, Color.FromArgb(1, 1, 1));
            bitmap.SetPixel(3, 0, Color.FromArgb(5, 5, 5));
            bitmap.SetPixel(3, 1, Color.FromArgb(6, 6, 6));
            bitmap.SetPixel(3, 2, Color.FromArgb(9, 9, 9));
            bitmap.SetPixel(3, 3, Color.FromArgb(2, 2, 2));
            bitmap.SetPixel(3, 4, Color.FromArgb(4, 4, 4));
            bitmap.SetPixel(4, 0, Color.FromArgb(8, 8, 8));
            bitmap.SetPixel(4, 1, Color.FromArgb(9, 9, 9));
            bitmap.SetPixel(4, 2, Color.FromArgb(1, 1, 1));
            bitmap.SetPixel(4, 3, Color.FromArgb(9, 9, 9));
            bitmap.SetPixel(4, 4, Color.FromArgb(9, 9, 9));

            /*
            |1 3 4 5 8|
            |7 4 2 6 9|
            |6 5 5 9 1|
            |4 0 8 2 9|
            |8 2 1 4 9|
            */
            return bitmap;
        }

        [TestMethod]
        public void BrightnessNoCenter()
        {
            var laplaceFilterer = new LaplaceFilter();
            var testImage = TestBitmap();

            var areaBrightnessOnePixelLaplaced = (int)laplaceFilterer.getLaplaceVal(0, 1, 1, 2, 2, testImage);
            var pixelBrightness = (int)(testImage.GetPixel(2, 2).GetBrightness() * 255);

            var areaBrightnessTwoPixelsLaplaced = (int)laplaceFilterer.getLaplaceVal(0, 2, 1, 2, 2, testImage);
            var areaBrightnessTwoPixels = 0.0;
            for(int i = 1; i < 4; i++)
            {
                for(int j = 1; j < 4; j++)
                {
                    areaBrightnessTwoPixels += testImage.GetPixel(i, j).GetBrightness() * 255;
                }
            }
            areaBrightnessTwoPixels = (int)(areaBrightnessTwoPixels / 9);

            Assert.AreEqual(areaBrightnessOnePixelLaplaced, pixelBrightness);
            Assert.AreEqual(areaBrightnessTwoPixelsLaplaced, areaBrightnessTwoPixels);
        }

        [TestMethod]
        public void laplaceWithCenter()
        {
            var laplaceFilterer = new LaplaceFilter();
            var testImage = TestBitmap();

            var laplacedSinglePixel = (int)laplaceFilterer.getLaplaceVal(1, 2, 1, 2, 2, testImage);
            var laplacedDoubleSurround = (int)laplaceFilterer.getLaplaceVal(1, 3, 1, 2, 2, testImage);
            var laplacedDoubleCenter = (int)laplaceFilterer.getLaplaceVal(2, 3, 1, 2, 2, testImage);

            Assert.AreEqual(2, laplacedSinglePixel);
            Assert.AreEqual(2, laplacedDoubleSurround);
            Assert.AreEqual(2, laplacedDoubleCenter);
        }

        [TestMethod]
        public void laplaceWithSharpener()
        {
            var laplaceFilterer = new LaplaceFilter();
            var testImage = TestBitmap();

            var laplacedSinglePixel = (int)laplaceFilterer.getLaplaceVal(1, 2, .5, 4, 4, testImage);
            var laplacedDoubleSurround = (int)laplaceFilterer.getLaplaceVal(1, 3, .5, 4, 4, testImage);
            var laplacedDoubleCenter = (int)laplaceFilterer.getLaplaceVal(2, 3, .5, 4, 4, testImage);

            Assert.AreEqual(31, laplacedSinglePixel);
            Assert.AreEqual(31, laplacedDoubleSurround);
            Assert.AreEqual(27, laplacedDoubleCenter);
        }
    }
}

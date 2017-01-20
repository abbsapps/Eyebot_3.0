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
            var bitmap = new Bitmap(10, 10);
            for(int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    bitmap.SetPixel(i, j, Color.FromArgb(255, i * 12 + j * 12, i * 12 + j * 12, i * 12 + j * 12));
                }
            }
            return bitmap;
        }

        [TestMethod]
        public void BrightnessNoCenter()
        {
            var laplaceFilterer = new LaplaceFilter();
            var testImage = TestBitmap();

            var areaBrightnessOnePixelLaplaced = (int)laplaceFilterer.getLaplaceVal(0, 1, 4, 4, testImage);
            var pixelBrightness = (int)(testImage.GetPixel(4, 4).GetBrightness() * 255);

            var areaBrightnessTwoPixelsLaplaced = (int)laplaceFilterer.getLaplaceVal(0, 2, 4, 4, testImage);
            var areaBrightnessTwoPixels = 0.0;
            for(int i = 3; i < 6; i++)
            {
                for(int j = 3; j < 6; j++)
                {
                    areaBrightnessTwoPixels += testImage.GetPixel(i, j).GetBrightness() * 255;
                }
            }
            areaBrightnessTwoPixels = (int)(areaBrightnessTwoPixels / 9);

            Assert.AreEqual(areaBrightnessOnePixelLaplaced, pixelBrightness);
            Assert.AreEqual(areaBrightnessTwoPixelsLaplaced, areaBrightnessTwoPixels);
        }
    }
}

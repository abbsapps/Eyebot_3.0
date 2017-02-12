using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Eyebot3
{
    public class CornerModule
    {
        private int xSize { get; set; }
        private int ySize { get; set; }
        private Random die { get; set; }
        private Bitmap perceivedImage { get; set; } //cornered one
        private Bitmap realImage { get; set; } //laplaced one
        private LaplaceFilter laplacer { get; set; }
        private Dictionary<int, Tuple<int, int>> knownPixels { get; set; } //Tuple is brightness, resolution
        private List<int> orderedBrightnesses { get; set; }
        private List<int> orderedPixels { get; set; }
        private int imageSize { get; set; }

        public int PixelSpread { get; set; }
        public int SurroundSpread { get; set; }
        public int ThresholdPower { get; set; }
        public int ChoicePower { get; set; }
        public int DeviationRange { get; set; }
        public int CounterThreshold { get; set; }




        private LaplaceCaller laplaceCaller { get; set; }
        public CornerModule(LaplaceCaller caller)
        {
            var image = caller.perceivedImage;
            xSize = image.Size.Width;
            ySize = image.Size.Height;
            die = new Random();
            perceivedImage = new Bitmap(xSize, ySize);
            realImage = image;
            laplacer = new LaplaceFilter();
            knownPixels = new Dictionary<int, Tuple<int, int>>();
            orderedBrightnesses = new List<int>() { 0 };
            orderedPixels = new List<int>() { 0 };
            imageSize = image.Width * image.Height;



            laplaceCaller = caller;
            for (int i = 6; i > 0; i--) {
                laplaceCaller.callLaplace(i, i + 1, 7, 3, 20, 10000);
            }

            callCorner(2, 3, 1, 1, 1, 1);
        }

        public void callCorner(int pixelSpread, int surroundSpread, int thresholdPower, int choicePower, int deviationRange, int counterThreshold)
        {
            for (int i = 0; i < 100000; i++) {
                var randomEntryChoice = new Tuple<int, int>((int)(die.NextDouble() * xSize), (int)(die.NextDouble() * ySize));

                var cornerMatch = getCornerVal(pixelSpread, surroundSpread, (int)(die.NextDouble() * 360), (int)(die.NextDouble() * 360), 1, randomEntryChoice.Item1, randomEntryChoice.Item2, realImage);

                var sharpened = (int)(Math.Pow((cornerMatch / 255.0), .3) * 255.0);
                perceivedImage.SetPixel(randomEntryChoice.Item1, randomEntryChoice.Item2, Color.FromArgb(255, sharpened, sharpened, sharpened));
            }
            var directory = System.IO.Directory.GetCurrentDirectory();
            perceivedImage.Save(directory + "/Images/corneredTriangle" + PixelSpread.ToString() + ".png");
        }

        private int getBrightness(Color pixel)
        {
            return (int)(pixel.GetBrightness() * 255);
        }

        public int getCornerVal(int centerResolution, int surroundResolution, int angle, int orientation, double sharpenValue, int xLoc, int yLoc, Bitmap image)
        {
            var cornerAreaMatch = getCornerMatchBrightness(centerResolution, xLoc, yLoc, image);
            var baseArmMatch = getArmMatchBrightness(centerResolution, 0, orientation, xLoc, yLoc, image);
            var otherArmMatch = getArmMatchBrightness(centerResolution, angle, orientation, xLoc, yLoc, image);
            //insert logic for getting surround contrast
            var surroundContrast = 0;
            //end empty logic
            return (int)((cornerAreaMatch + baseArmMatch + otherArmMatch)/3.0);
        }

        private int getCornerMatchBrightness(int getResolution, int xLoc, int yLoc, Bitmap image)
        {
            var sumGetBrightness = 0;
            var getCornerPixelCount = Math.Pow(getResolution * 2 - 1, 2);

            for (int i = -1 * getResolution + 1; i < getResolution; i++)
            {
                for (int j = -1 * getResolution + 1; j < getResolution; j++)
                {
                    if (xLoc + i >= 0 && yLoc + j >= 0 && xLoc + i < image.Width && yLoc + j < image.Height)
                    {
                        sumGetBrightness += getBrightness(image.GetPixel(xLoc + i, yLoc + j));
                    }
                    else
                    {
                        getCornerPixelCount -= 1;
                    }
                }
            }
            return (int)(sumGetBrightness / getCornerPixelCount);
        }



        private int getArmMatchBrightness(int getResolution, int angle, int orientation, int xLoc, int yLoc, Bitmap image)
        {
            var armExtentionMultiplier = 5;

            var sumGetBrightness = 0;
            var getArmPixelCount = Math.Pow(getResolution * 2 - 1, 2) * armExtentionMultiplier; //is this multiplication valid?  probably not - look into

            double orientationRadians = orientation * (Math.PI / 180);
            double rotationRadians = angle * (Math.PI / 180);

            double adjustedRadians = rotationRadians - orientationRadians > 0 ? 
                rotationRadians - orientationRadians : 360 + (rotationRadians - orientationRadians);

            double cosTheta = Math.Cos(adjustedRadians);
            double sinTheta = Math.Sin(adjustedRadians);

            for (int i = -1 * getResolution + 1; i < getResolution * armExtentionMultiplier; i++)
            {
                for (int j = -1 * getResolution + 1; j < getResolution; j++)
                {
                    var xLocation = xLoc + i;
                    var yLocation = yLoc + i;

                    var adjustedXLocation = (int)(cosTheta * (xLocation - xLoc) -
                        sinTheta * (yLocation - yLoc) + xLoc);

                    var adjustedYLocation = (int)(sinTheta * (xLocation - xLoc) +
                        cosTheta * (yLocation - yLoc) + yLoc);

                    if (adjustedXLocation >= 0 && adjustedYLocation >= 0 && adjustedXLocation < image.Width && adjustedYLocation < image.Height)
                    {
                        sumGetBrightness += getBrightness(image.GetPixel(adjustedXLocation, adjustedYLocation));
                    }
                    else
                    {
                        getArmPixelCount -= 1;
                    }
                }
            }
            return (int)(sumGetBrightness / getArmPixelCount);
        }
    }
}

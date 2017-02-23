﻿using System;
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
            callCorner(2, 2, 1, 1, 1, 1);
        }

        public void callCorner(int pixelSpread, int surroundSpread, int thresholdPower, int choicePower, int deviationRange, int counterThreshold)
        {
            for (int i = 0; i < 20000; i++) {
                var randomEntryChoice = new Tuple<int, int>((int)(die.NextDouble() * xSize), (int)(die.NextDouble() * ySize));
                var angle = (int)(die.NextDouble() * 360);
                var orientation = (int)(die.NextDouble() * 360);

                var cornerMatch = getCornerVal(pixelSpread, surroundSpread, 5, angle, orientation, 1, randomEntryChoice.Item1, randomEntryChoice.Item2, realImage);
                if(cornerMatch.Item2 > 0)
                {
                    var hit = "hit";
                }
                var sharpened = (int)(Math.Pow((cornerMatch.Item2 / 255.0), .3) * 255.0);
                for (int j = 0; j < cornerMatch.Item1.Count; j++)
                {
                    perceivedImage.SetPixel(cornerMatch.Item1[j].Item1, cornerMatch.Item1[j].Item2, Color.FromArgb(255, sharpened, sharpened, sharpened));
                }
            }
            var directory = System.IO.Directory.GetCurrentDirectory();
            perceivedImage.Save(directory + "/Images/corneredTriangle" + PixelSpread.ToString() + ".png");
        }

        private int getBrightness(Color pixel)
        {
            return (int)(pixel.GetBrightness() * 255);
        }

        public Tuple<List<Tuple<int, int>>, int> getCornerVal(int resolution, int surroundResolution, int armExtensionMultiplier, int angle, int orientation, double sharpenValue, int xLoc, int yLoc, Bitmap image)
        {
            var fillInPixels = new List<Tuple<int, int>>();

            int superResolution = armExtensionMultiplier * resolution;

            double orientationRadians = orientation * (Math.PI / 180);

            double orientationCosTheta = Math.Cos(orientationRadians);
            double orientationSinTheta = Math.Sin(orientationRadians);

            double rotationRadians = angle * (Math.PI / 180);

            double adjustedRadians = rotationRadians - orientationRadians > 0 ?
                rotationRadians - orientationRadians : 360 + (rotationRadians - orientationRadians);

            double rotationCosTheta = Math.Cos(adjustedRadians);
            double rotationSinTheta = Math.Sin(adjustedRadians);

            int matchBrightness = 0;
            int surroundBrightness = 0;
            int matchPixelCount = 0;
            int surroundPixelCount = 0;

            for (int i = -1 * superResolution; i < superResolution; i++)
            {
                for (int j = -1 * superResolution; j < superResolution; j++)
                {
                    var xLocation = xLoc + i;
                    var yLocation = yLoc + j;
                    if (xLocation > 0 && yLocation > 0 && xLocation < image.Width && yLocation < image.Height)
                    {
                        var noHit = true;

                        var adjustedBaseXLocation = (int)(orientationCosTheta * (xLocation - xLoc) -
                        orientationSinTheta * (yLocation - yLoc) + xLoc);

                        var adjustedBaseYLocation = (int)(orientationSinTheta * (xLocation - xLoc) +
                            orientationCosTheta * (yLocation - yLoc) + yLoc);

                        var adjustedRotationXLocation = (int)(rotationCosTheta * (xLocation - xLoc) -
                        rotationSinTheta * (yLocation - yLoc) + xLoc);

                        var adjustedRotationYLocation = (int)(rotationSinTheta * (xLocation - xLoc) +
                            rotationCosTheta * (yLocation - yLoc) + yLoc);

                        //note: need a way to detect unset pixels to skipe them (i.e. set noHit if xLocation, yLocation is not set in input image)
                        if (noHit && adjustedBaseXLocation >= xLoc - resolution && adjustedBaseYLocation >= yLoc - resolution && adjustedBaseYLocation < yLoc + resolution)
                        {
                            matchBrightness += (int)(realImage.GetPixel(xLocation, yLocation).GetBrightness() * 255.0);
                            if(matchBrightness > 0)
                            {
                                var hit = "hit";
                            }
                            matchPixelCount += 1;
                            fillInPixels.Add(new Tuple<int, int>(xLocation, yLocation));
                            noHit = false;
                        }
                        else if (noHit && adjustedRotationXLocation >= xLoc - resolution && adjustedRotationYLocation >= yLoc - resolution && adjustedRotationYLocation < yLoc + resolution)
                        {
                            matchBrightness += (int)(realImage.GetPixel(xLocation, yLocation).GetBrightness() * 255.0);
                            matchPixelCount += 1;
                            fillInPixels.Add(new Tuple<int, int>(xLocation, yLocation));
                            noHit = false;
                        }
                        else if (noHit && adjustedBaseXLocation >= xLoc - (resolution + surroundResolution) && adjustedBaseYLocation >= yLoc - (resolution + surroundResolution) && adjustedBaseYLocation < yLoc + (resolution + surroundResolution))
                        {
                            surroundBrightness += (int)(realImage.GetPixel(xLocation, yLocation).GetBrightness() * 255.0);
                            surroundPixelCount += 1;
                            noHit = false;
                        }
                        else if (noHit && adjustedRotationXLocation >= xLoc - (resolution + surroundResolution) && adjustedRotationYLocation >= yLoc - (resolution + surroundResolution) && adjustedRotationYLocation < yLoc + (resolution + surroundResolution))
                        {
                            surroundBrightness += (int)(realImage.GetPixel(xLocation, yLocation).GetBrightness() * 255.0);
                            surroundPixelCount += 1;
                            noHit = false;
                        }
                    }
                }
            }

            var matchAverage = (double)matchBrightness / matchPixelCount;
            var surroundAverage = (double)surroundBrightness / surroundPixelCount;
            var collatedValue = (int)(matchAverage - surroundAverage);
            collatedValue = collatedValue > 0 ? collatedValue : 0;
            return new Tuple<List<Tuple<int, int>>, int>(fillInPixels, collatedValue);
        }
    }
}

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
            callCorner(2, 2, 1, 1, 1, 1);
        }

        public void callCorner(int pixelSpread, int surroundSpread, int thresholdPower, int choicePower, int deviationRange, int counterThreshold)
        {
            for (int i = 0; i < 2000; i++) {
                var randomEntryChoice = new Tuple<int, int>((int)(die.NextDouble() * xSize), (int)(die.NextDouble() * ySize));
                var angle = (int)(die.NextDouble() * 360);
                var orientation = (int)(die.NextDouble() * 360);

                //for debugging
                orientation = 180;
                angle = 270;
                //end debugging

                var cornerMatch = getCornerVal(pixelSpread, surroundSpread, 5, angle, orientation, 1, randomEntryChoice.Item1, randomEntryChoice.Item2, realImage);
                if(cornerMatch.Item2 > 0)
                {
                    var hit = "hit";
                }
                var sharpened = (int)(Math.Pow((cornerMatch.Item2 / 255.0), .3) * 255.0);
                for (int j = 0; j < cornerMatch.Item1.Count; j++)
                {
                    //perceivedImage.SetPixel(cornerMatch.Item1[j].Item1, cornerMatch.Item1[j].Item2, Color.FromArgb(255, sharpened, sharpened, sharpened));
                }
                //for debugging
                var savePlace = System.IO.Directory.GetCurrentDirectory();
                perceivedImage.Save(savePlace + "/Images/corneredTriangle" + PixelSpread.ToString() + ".png");
                //end debugging
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

                        var rotatedBaseLocation = RotateLocation(0, orientation, xLoc, yLoc, i, j);
                        var rotatedRotationLocation = RotateLocation(orientation, angle, xLoc, yLoc, i, j);
                        //note: need a way to detect unset pixels to skip them (i.e. set noHit if xLocation, yLocation is not set in input image)
                        //check if rotated pixel location overlaps with base orientation line
                        if (rotatedBaseLocation.Item1 >= xLoc - resolution && rotatedBaseLocation.Item2 >= yLoc - resolution && rotatedBaseLocation.Item2 < yLoc + resolution)
                        {
                            matchBrightness += (int)(realImage.GetPixel(xLocation, yLocation).GetBrightness() * 255.0);
                            if(matchBrightness > 0)
                            {
                                var hit = "hit";
                            }
                            matchPixelCount += 1;
                            fillInPixels.Add(new Tuple<int, int>(xLocation, yLocation));
                            //for debugging
                            perceivedImage.SetPixel(xLocation, yLocation, Color.FromArgb(255, 125, 125, 125));
                            //end debugging
                        }
                        //check if rotated pixel location overlaps with angle (with respect to orientation) line
                        else if (rotatedRotationLocation.Item1 >= xLoc - resolution && rotatedRotationLocation.Item2 >= yLoc - resolution && rotatedRotationLocation.Item2 < yLoc + resolution)
                        {
                            matchBrightness += (int)(realImage.GetPixel(xLocation, yLocation).GetBrightness() * 255.0);
                            matchPixelCount += 1;
                            fillInPixels.Add(new Tuple<int, int>(xLocation, yLocation));
                            //for debugging
                            perceivedImage.SetPixel(xLocation, yLocation, Color.FromArgb(255, 125, 125, 125));
                            //end debugging
                        }
                        //check if rotated pixel location is just outside the edge of the base orientation line
                        else if (rotatedBaseLocation.Item1 >= xLoc - (resolution) && rotatedBaseLocation.Item2 < yLoc && rotatedBaseLocation.Item2 > yLoc - (resolution + surroundResolution))
                        {
                            surroundBrightness += (int)(realImage.GetPixel(xLocation, yLocation).GetBrightness() * 255.0);
                            surroundPixelCount += 1;
                            //for debugging
                            perceivedImage.SetPixel(xLocation, yLocation, Color.FromArgb(255, 0, 0, 0));
                            //end debugging
                        }
                        //check if rotated pixel location is just outside the edge of the angle line
                        else if (rotatedRotationLocation.Item1 >= xLoc - (resolution) && rotatedRotationLocation.Item2 > yLoc && rotatedRotationLocation.Item2 < yLoc + (resolution + surroundResolution))
                        {
                            surroundBrightness += (int)(realImage.GetPixel(xLocation, yLocation).GetBrightness() * 255.0);
                            surroundPixelCount += 1;
                            //for debugging
                            perceivedImage.SetPixel(xLocation, yLocation, Color.FromArgb(255, 0, 0, 0));
                            //end debugging
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


        private Tuple<int, int> RotateLocation(int orientationAngle, int rotationAngle, int xCenter, int yCenter, int xOffset, int yOffset)
        {
            double orientationRadians = orientationAngle * (Math.PI / 180);
            double rotationRadians = rotationAngle * (Math.PI / 180);

            rotationRadians = rotationRadians + orientationRadians <= (2*Math.PI) ?
                rotationRadians + orientationRadians : (rotationRadians + orientationRadians) - (2*Math.PI);

            double cosTheta = Math.Cos(rotationRadians);
            double sinTheta = Math.Sin(rotationRadians);

            var adjustedBaseXLocation = (int)(cosTheta * (xOffset) -
                        sinTheta * (yOffset) + xCenter);

            var adjustedBaseYLocation = (int)(sinTheta * (xOffset) +
                cosTheta * (yOffset) + yCenter);

            return new Tuple<int, int>(adjustedBaseXLocation, adjustedBaseYLocation);
        }
    }
}

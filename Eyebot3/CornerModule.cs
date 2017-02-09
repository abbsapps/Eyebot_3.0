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
        private LaplaceCaller laplaceCaller { get; set; }
        public CornerModule(LaplaceCaller caller)
        {
            laplaceCaller = caller;
            for (int i = 6; i > 0; i--) {
                laplaceCaller.callLaplace(i, i + 1, 7, 3, 20, 10000);
            }
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
            return cornerAreaMatch + baseArmMatch + otherArmMatch;
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
                        sumGetBrightness += getBrightness(image.GetPixel(xLoc + i, yLoc + j));
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

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eyebot3
{
    public class LaplaceFilter
    {
        public LaplaceFilter()
        {

        }

        private int getBrightness(Color pixel)
        {
            return (int)(pixel.GetBrightness() * 255);
        }

        public int getLaplaceVal(int centerResolution, int surroundResolution, double sharpenValue, int xLoc, int yLoc, Bitmap image)
        {
            var centerBrightness = getAreaBrightness(centerResolution, xLoc, yLoc, image);

            var rawContrast = getSurroundContrast(centerResolution, surroundResolution, xLoc, yLoc, centerBrightness, image);

            return (laplaceSharpener(rawContrast, sharpenValue));
        }

        private int laplaceSharpener(int inputBrightness, double sharpness)
        {
            return (int)(Math.Pow((inputBrightness / 255.0), sharpness) * 255.0);
        }

        private int getSurroundContrast(int skipResolution, int getResolution, int xLoc, int yLoc, int centerBrightness, Bitmap image)
        {
            var sumContrast = 0;
            var skipPixelCount = skipResolution == 0 ? 0 : Math.Pow(skipResolution * 2 - 1, 2);
            var getPixelCount = Math.Pow(getResolution * 2 - 1, 2) - skipPixelCount;

            for (int i = -1 * getResolution + 1; i < getResolution; i++)
            {
                for (int j = -1 * getResolution + 1; j < getResolution; j++)
                {
                    if (i > -1 * skipResolution && i < skipResolution && j > -1 * skipResolution && j < skipResolution)
                    {
                        sumContrast += 0;
                    }
                    else
                    {
                        if (xLoc+i >= 0 && yLoc+j >= 0 && xLoc+i < image.Width && yLoc+j < image.Height)
                        {
                            sumContrast += Math.Abs(getBrightness(image.GetPixel(xLoc + i, yLoc + j)) - centerBrightness);
                        }
                        else
                        {
                            getPixelCount -= 1;
                        }
                    }
                }
            }
            return (int)(sumContrast / getPixelCount);
        }

        private int getAreaBrightness(int getResolution, int xLoc, int yLoc, Bitmap image)
        {
            var sumGetBrightness = 0;
            var getPixelCount = Math.Pow(getResolution * 2 - 1, 2);
            for (int i = -1 * getResolution + 1; i < getResolution; i++)
            {
                for (int j = -1 * getResolution + 1; j < getResolution; j++)
                {
                    if (xLoc+i >= 0 && yLoc+j >= 0 && xLoc+i < image.Width && yLoc+j < image.Height)
                    {
                        sumGetBrightness += getBrightness(image.GetPixel(xLoc + i, yLoc + j));
                    }
                    else
                    {
                        getPixelCount -= 1;
                    }
                }
            }
            return (int)(sumGetBrightness / getPixelCount);
        }
    }
}

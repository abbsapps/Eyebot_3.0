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

        public int getLaplaceBrightness(Bitmap image, int xLoc, int yLoc)
        {
            return (getLaplaceVal(1, 2, xLoc, yLoc, image));
        }

        public int getBrightness(Color pixel)
        {
            return (int)(pixel.GetBrightness() * 255);
        }

        public int getLaplaceVal(int centerResolution, int surroundResolution, int xLoc, int yLoc, Bitmap image)
        {
            var centerBrightness = getAreaBrightness(0, centerResolution, xLoc, yLoc, image);
            var surroundBrightness = getAreaBrightness(centerResolution, surroundResolution, xLoc, yLoc, image);

            //sharpener experimentation
            var sumDifference = Math.Abs(centerBrightness - surroundBrightness);
            //end sharpener

            return Math.Abs(centerBrightness - surroundBrightness);


        }





        public int getAreaBrightness(int skipResolution, int getResolution, int xLoc, int yLoc, Bitmap image)
        {
            var sumGetBrightness = 0;
            var skipPixelCount = skipResolution == 0 ? 0 : Math.Pow(skipResolution * 2 - 1, 2);
            var getPixelCount = Math.Pow(getResolution * 2 - 1, 2) - skipPixelCount;
            for (int i = -1 * getResolution + 1; i < getResolution; i++)
            {
                for (int j = -1 * getResolution + 1; j < getResolution; j++)
                {
                    if (i > -1 * skipResolution && i < skipResolution && j > -1 * skipResolution && j < skipResolution)
                    {
                        sumGetBrightness += 0;
                    }
                    else
                    {
                        sumGetBrightness += getBrightness(image.GetPixel(xLoc + i, yLoc + j));
                    }
                }
            }
            return (int)(sumGetBrightness / getPixelCount);
        }
    }
}

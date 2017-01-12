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

            var sumDifference = 0;
            var pixelVal = getBrightness(image.GetPixel(xLoc, yLoc));
            for (int i = -1; i < 2; i++)
            {
                for (int j = -1; j < 2; j++)
                {
                    sumDifference += Math.Abs(pixelVal - getBrightness(image.GetPixel(xLoc + 1, yLoc + j)));
                }
            }
            return (int)(sumDifference / 9.0);
        }

        public int getBrightness(Color pixel)
        {
            return (int)(pixel.GetBrightness() * 255);
        }
    }
}

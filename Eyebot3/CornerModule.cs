using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;

namespace Eyebot3
{
    public class CornerModule
    {
        private int xSize { get; set; }
        private int ySize { get; set; }
        private Random die { get; set; }
        private Bitmap perceivedImage { get; set; }
        private Bitmap realImage { get; set; }
        private LaplaceFilter laplacer { get; set; }

        public CornerModule(Bitmap image)
        {
            xSize = image.Size.Width;
            ySize = image.Size.Height;
            die = new Random();
            perceivedImage = new Bitmap(xSize, ySize);
            realImage = image;
            laplacer = new LaplaceFilter();
        }

        //public Tuple<int, int> nextLocationStrategy()

        public void laplaceCaller()
        {
            var directory = System.IO.Directory.GetCurrentDirectory();
            //for (int i = 0; i < 1000; i++)
            var knownPixels = new Dictionary<Tuple<int, int>, int>();
            var counter = 0;
            while (true)
            {
                int xLocation = (int)(die.NextDouble() * xSize);
                int yLocation = (int)(die.NextDouble() * ySize);
                //if (xLocation > 5 && yLocation > 5 && xLocation < xSize - 5 && yLocation < ySize - 5)
                //{
                    var pixelBrightness = laplacer.getLaplaceVal(1, 2, .5, xLocation, yLocation, realImage);
                    perceivedImage.SetPixel(xLocation,
                        yLocation, Color.FromArgb(255, pixelBrightness, pixelBrightness, pixelBrightness));
                    //knownPixels[new Tuple<int, int>(xLocation, yLocation)] =  pixelBrightness;
                //}
                counter++;
                if(counter > 5000)
                {
                    counter = 0;
                    perceivedImage.Save(directory + "/Images/laplacedTriangle.png");
                }
            }
        }
    }
}

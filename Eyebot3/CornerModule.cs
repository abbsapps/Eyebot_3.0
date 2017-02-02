using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Specialized;

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
        private Dictionary<Tuple<int, int>, int> knownPixels { get; set; }
        private List<int> orderedBrightnesses { get; set; }
        private List<Tuple<int, int>> orderedPixels { get; set; }
        private int imageSize { get; set; }

        public CornerModule(Bitmap image)
        {
            xSize = image.Size.Width;
            ySize = image.Size.Height;
            die = new Random();
            perceivedImage = new Bitmap(xSize, ySize);
            realImage = image;
            laplacer = new LaplaceFilter();
            knownPixels = new Dictionary<Tuple<int, int>, int>();
            orderedBrightnesses = new List<int>() { 0 };
            orderedPixels = new List<Tuple<int, int>>() { new Tuple<int, int>(0,0)};
            imageSize = image.Width * image.Height;
        }

        public Tuple<int, int> nextLocationStrategy()
        {
            if(die.NextDouble() * imageSize > knownPixels.Count)
            {
                return new Tuple<int, int>((int)(die.NextDouble() * xSize), (int)(die.NextDouble() * ySize));
            }
            var entryChoice = (int)(Math.Pow(die.NextDouble(), 5) * orderedPixels.Count);
            var baseEntry = orderedPixels[orderedPixels.Count - entryChoice - 1];
            var chosenX = (int)(die.NextDouble() * 20 - 10);
            var chosenY = (int)(die.NextDouble() * 20 - 10);
            var chosenEntry = new Tuple<int, int>(baseEntry.Item1 + chosenX, baseEntry.Item2 + chosenY);
            return chosenEntry;
        }

        public void laplaceCaller()
        {
            var directory = System.IO.Directory.GetCurrentDirectory();
            var counter = 0;
            while (true)
            {
                //int xLocation = (int)(die.NextDouble() * xSize);
                //int yLocation = (int)(die.NextDouble() * ySize);
                var nextLocationTuple = nextLocationStrategy();
                int xLocation = nextLocationTuple.Item1;
                int yLocation = nextLocationTuple.Item2;

                var pixelBrightness = laplacer.getLaplaceVal(1, 2, .5, xLocation, yLocation, realImage);

                if (xLocation >= 0 && yLocation >= 0 && xLocation < xSize && yLocation < ySize)
                {
                    var pixelLocation = new Tuple<int, int>(xLocation, yLocation);
                    if (knownPixels.ContainsKey(pixelLocation))
                    {
                        var listIndex = orderedPixels.IndexOf(pixelLocation);
                        orderedPixels.RemoveAt(listIndex);
                        orderedBrightnesses.RemoveAt(listIndex);
                    }
                    knownPixels[pixelLocation] = pixelBrightness;
                    var listInsertLocation = orderedBrightnesses.BinarySearch(pixelBrightness);
                    if (listInsertLocation < 0)
                    {
                        listInsertLocation = ~listInsertLocation;
                    }
                    orderedPixels.Insert(listInsertLocation, pixelLocation);
                    orderedBrightnesses.Insert(listInsertLocation, pixelBrightness);


                    perceivedImage.SetPixel(xLocation, yLocation, Color.FromArgb(255, pixelBrightness, pixelBrightness, pixelBrightness));
                }

                counter++;
                if(counter > 50000)
                {
                    counter = 0;
                    perceivedImage.Save(directory + "/Images/laplacedTriangle.png");
                }
            }
        }
    }
}

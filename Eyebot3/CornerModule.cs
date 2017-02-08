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

        private Tuple<int, int> nextLocationStrategy(int pixelSpread)
        {
            var threshold = (Math.Pow(((float)(imageSize - orderedPixels.Count) / imageSize), 4));
            var dieRoll = die.NextDouble();
            if (dieRoll < threshold)
            {
                return new Tuple<int, int>((int)(die.NextDouble() * xSize), (int)(die.NextDouble() * ySize));
            }
            var entryChoice = (int)(Math.Pow(die.NextDouble(), 3) * orderedPixels.Count);
            var baseEntry = orderedPixels[orderedPixels.Count - entryChoice - 1];
            var chosenX = (int)(die.NextDouble() * 20 * pixelSpread - 10 * pixelSpread);
            var chosenY = (int)(die.NextDouble() * 20 * pixelSpread - 10 * pixelSpread);
            var chosenEntry = new Tuple<int, int>(baseEntry.Item1 + chosenX, baseEntry.Item2 + chosenY);
            return chosenEntry;
        }

        public void laplaceCaller()
        {
            var directory = System.IO.Directory.GetCurrentDirectory();
            var counter = 0;
            while (true)
            {
                var pixelSpread = 3;
                var surroundSpread = 5;

                var nextLocationTuple = nextLocationStrategy(pixelSpread);
                int xLocation = nextLocationTuple.Item1;
                int yLocation = nextLocationTuple.Item2;

                var pixelBrightness = laplacer.getLaplaceVal(pixelSpread, surroundSpread, .5, xLocation, yLocation, realImage);

                if (xLocation >= pixelSpread && yLocation >= pixelSpread && xLocation < xSize - pixelSpread && yLocation < ySize - pixelSpread)
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

                    for (int i = -1 * pixelSpread + 1; i < pixelSpread; i++)
                    {
                        for (int j = -1 * pixelSpread + 1; j < pixelSpread; j++)
                        {
                            perceivedImage.SetPixel(xLocation + i, yLocation + j, Color.FromArgb(255, pixelBrightness, pixelBrightness, pixelBrightness));
                        }
                    }
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

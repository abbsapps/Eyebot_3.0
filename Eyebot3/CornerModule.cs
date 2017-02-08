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
        private Dictionary<int, Tuple<int, int>> knownPixels { get; set; } //Tuple is brightness, resolution
        private List<int> orderedBrightnesses { get; set; }
        private List<int> orderedPixels { get; set; }
        private int imageSize { get; set; }

        public CornerModule(Bitmap image)
        {
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
        }

        private Tuple<int, int> nextLocationStrategy(int pixelSpread, int thresholdPower, int choicePower, int deviationRange)
        {
            var threshold = (Math.Pow(((float)(imageSize - orderedPixels.Count) / imageSize), thresholdPower));
            var dieRoll = die.NextDouble();
            if (dieRoll < threshold)
            {
                return new Tuple<int, int>((int)(die.NextDouble() * xSize), (int)(die.NextDouble() * ySize));
            }
            var entryChoice = (int)(Math.Pow(die.NextDouble(), choicePower) * orderedPixels.Count);
            var baseEntry = orderedPixels[orderedPixels.Count - entryChoice - 1];
            var chosenX = (int)(die.NextDouble() * deviationRange * pixelSpread - 10 * pixelSpread);
            var chosenY = (int)(die.NextDouble() * deviationRange * pixelSpread - 10 * pixelSpread);

            var chosenEntryInput = locationTranslatorFromInt(baseEntry);
            var chosenEntry = new Tuple<int, int>(chosenEntryInput.Item1 + chosenX, chosenEntryInput.Item2 + chosenY);
            return chosenEntry;
        }

        public int locationTranslatorToInt(int xLocation, int yLocation)
        {
            return yLocation * xSize + xLocation;
        }
        public Tuple<int, int> locationTranslatorFromInt(int intLocation)
        {
            var yLocation = (intLocation / xSize);
            var xLocation = intLocation - yLocation * xSize;
            return new Tuple<int, int>(xLocation, yLocation);
        }

        public void laplaceCaller()
        {
            var directory = System.IO.Directory.GetCurrentDirectory();
            var counter = 0;
            var pixelSpread = 6;
            var surroundSpread = pixelSpread + 1;
            var thresholdPower = 7;
            var choicePower = 3;
            var deviationRange = 20;
            var counterThreshold = 10000;

            while (true)
            {
                var nextLocationTuple = nextLocationStrategy(pixelSpread, thresholdPower, choicePower, deviationRange);
                int xLocation = nextLocationTuple.Item1;
                int yLocation = nextLocationTuple.Item2;

                var pixelBrightness = laplacer.getLaplaceVal(pixelSpread, surroundSpread, .5, xLocation, yLocation, realImage);

                if (xLocation >= pixelSpread && yLocation >= pixelSpread && xLocation < xSize - pixelSpread && yLocation < ySize - pixelSpread)
                {
                    var pixelLocation = locationTranslatorToInt(xLocation, yLocation);
                    //replace with unit test
                    var testReversal = locationTranslatorFromInt(pixelLocation);
                    //end replace
                    if (knownPixels.ContainsKey(pixelLocation))
                    {
                        var listIndex = orderedPixels.IndexOf(pixelLocation);
                        orderedPixels.RemoveAt(listIndex);
                        orderedBrightnesses.RemoveAt(listIndex);
                    }
                    knownPixels[pixelLocation] = new Tuple<int, int>(pixelBrightness, pixelSpread);
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
                if(counter > counterThreshold)
                {
                    counter = 0;

                    counterThreshold += 10000;
                    thresholdPower += 2;
                    pixelSpread -= 1;
                    surroundSpread -= 1;

                    perceivedImage.Save(directory + "/Images/laplacedTriangle.png");
                }
            }
        }
    }
}

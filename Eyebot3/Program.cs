using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eyebot3
{
    class Program
    {
         static void Main(string[] args)
        {
            LaplaceFilter laplaceFilter = new LaplaceFilter();
            Image environmentImage = Image.FromFile(System.IO.Directory.GetCurrentDirectory() + "/Images/triangles.png");
            Bitmap mappedImage = new Bitmap(environmentImage);
            Bitmap copiedImage = new Bitmap(environmentImage.Width, environmentImage.Height);
            Bitmap doubleCopiedImage = new Bitmap(environmentImage.Width, environmentImage.Height);
            //laplace

            var cornerModule = new CornerModule(mappedImage);
            cornerModule.laplaceCaller();

            /*
            for (int i = 5; i < mappedImage.Width - 5; i++)
            {
                for (int j = 5; j < mappedImage.Height - 5; j++)
                {
                    var brightness = laplaceFilter.getLaplaceVal(2, 3, 1, i, j, mappedImage);
                    copiedImage.SetPixel(i, j, Color.FromArgb(255, brightness, brightness, brightness));
                }
            }
            var directory = System.IO.Directory.GetCurrentDirectory();
            copiedImage.Save(directory + "/Images/laplacedTriangle.png");

            //double laplace
            for (int i = 5; i < copiedImage.Width - 5; i++)
            {
                for (int j = 5; j < copiedImage.Height - 5; j++)
                {
                    //var brightness = (int)(mappedImage.GetPixel(i, j).GetBrightness() * 255);
                    var brightness = laplaceFilter.getLaplaceVal(2, 3, 1, i, j, mappedImage);
                    doubleCopiedImage.SetPixel(i, j, Color.FromArgb(255, brightness, brightness, brightness));
                }
            }
            doubleCopiedImage.Save(System.IO.Directory.GetCurrentDirectory() + "/Images/doubleLaplacedTriangle.png");

            */
        }
    }
}

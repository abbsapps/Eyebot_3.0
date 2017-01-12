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
            //Console.WriteLine(System.IO.Directory.GetCurrentDirectory());
            //Console.ReadLine();
            Image environmentImage = Image.FromFile(System.IO.Directory.GetCurrentDirectory() + "/Images/eye.jpg");
            Bitmap mappedImage = new Bitmap(environmentImage);
            Bitmap copiedImage = new Bitmap(environmentImage.Width, environmentImage.Height);
            Bitmap doubleCopiedImage = new Bitmap(environmentImage.Width, environmentImage.Height);
            //laplace
            for (int i = 1; i < mappedImage.Width - 1; i++)
            {
                for (int j = 1; j < mappedImage.Height - 1; j++)
                {
                    //var brightness = (int)(mappedImage.GetPixel(i, j).GetBrightness() * 255);
                    var brightness = laplaceFilter.getLaplaceBrightness(mappedImage, i, j);
                    copiedImage.SetPixel(i, j, Color.FromArgb(255, brightness, brightness, brightness));
                }
            }
            copiedImage.Save(System.IO.Directory.GetCurrentDirectory() + "/Images/laplaced.png");

            //double laplace
            for (int i = 1; i < copiedImage.Width - 1; i++)
            {
                for (int j = 1; j < copiedImage.Height - 1; j++)
                {
                    //var brightness = (int)(mappedImage.GetPixel(i, j).GetBrightness() * 255);
                    var brightness = laplaceFilter.getLaplaceBrightness(copiedImage, i, j);
                    doubleCopiedImage.SetPixel(i, j, Color.FromArgb(255, brightness, brightness, brightness));
                }
            }
            doubleCopiedImage.Save(System.IO.Directory.GetCurrentDirectory() + "/Images/doubleLaplaced.png");
        }
    }
}

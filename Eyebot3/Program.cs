﻿using System;
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
            for (int i = 5; i < mappedImage.Width - 5; i++)
            {
                for (int j = 5; j < mappedImage.Height - 5; j++)
                {
                    //var brightness = (int)(mappedImage.GetPixel(i, j).GetBrightness() * 255);
                    var brightness = laplaceFilter.getLaplaceBrightness(mappedImage, i, j);
                    copiedImage.SetPixel(i, j, Color.FromArgb(255, brightness, brightness, brightness));
                }
            }
            var directory = System.IO.Directory.GetCurrentDirectory();
            copiedImage.Save(directory + "/Images/laplaced.png");

            //double laplace
            for (int i = 5; i < copiedImage.Width - 5; i++)
            {
                for (int j = 5; j < copiedImage.Height - 5; j++)
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

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

            var cornerModule = new CornerModule(mappedImage);
            cornerModule.laplaceCaller();
        }
    }
}

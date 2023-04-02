using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BMP_Console
{
    class Steganography : MyImage
    {

        //Adapt to variable number of bits stored in image
        public static MyImage Hide(MyImage parentImage, MyImage hiddenImage)
        {
            if(parentImage.Width < hiddenImage.Width || parentImage.Height < hiddenImage.Height)
            {
                throw new Exception("L'image cachée doit être inférieure ou égale en taille à l'image parente");
            }


            Pixel[,] newPixels = new Pixel[parentImage.Width, parentImage.Height]; 

            MyImage returnImage = new MyImage(parentImage.BuildHeader(parentImage.FileSize), 
                parentImage.BuildHeaderInfo(parentImage.Width, parentImage.Height, parentImage.ImageSize), newPixels);

            for(int i = 0; i < parentImage.Height; i++)
            {
                for(int j = 0; j < parentImage.Width; j++)
                {
                    newPixels[j, i] = new Pixel((byte)(parentImage.ImagePixels[j, i].red & 0xF0 | (i < hiddenImage.Height && j < hiddenImage.Width ? hiddenImage.ImagePixels[j, i].red >> 4 : 0x00)),
                        (byte)(parentImage.ImagePixels[j, i].green & 0xF0 | (i < hiddenImage.Height && j < hiddenImage.Width ? hiddenImage.ImagePixels[j, i].green >> 4 : 0x00)),
                        (byte)(parentImage.ImagePixels[j, i].blue & 0xF0 | (i < hiddenImage.Height && j < hiddenImage.Width ? hiddenImage.ImagePixels[j, i].blue >> 4 : 0x00)));
                }
            }

            return returnImage;
        }

        public static MyImage RestoreHidden(MyImage parentImage)
        {
            Pixel[,] newPixels = new Pixel[parentImage.Width, parentImage.Height];

            MyImage returnImage = new MyImage(parentImage.BuildHeader(parentImage.FileSize),
                parentImage.BuildHeaderInfo(parentImage.Width, parentImage.Height, parentImage.ImageSize), newPixels);

            for (int i = 0; i < parentImage.Height; i++)
            {
                for (int j = 0; j < parentImage.Width; j++)
                {

                }
            }
        }
    }
}

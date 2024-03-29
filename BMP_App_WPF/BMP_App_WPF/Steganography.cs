﻿using System;

namespace BMP_App_WPF
{
    class Steganography : MyImage
    {
        public static MyImage Hide(MyImage parentImage, MyImage hiddenImage, int byteShift)
        {
            if (parentImage.Width < hiddenImage.Width || parentImage.Height < hiddenImage.Height)
                throw new Exception("Hidden image must have a size inferior or equal to parent image");


            byte b = (byte)(Shift(byteShift) ^ 0xFF);
            byteShift = 8 - byteShift;

            PixelRGB[,] newPixels = new PixelRGB[parentImage.Width, parentImage.Height];
            MyImage returnImage = new MyImage(parentImage.BuildHeader(parentImage.FileSize),
                parentImage.BuildHeaderInfo(parentImage.Width, parentImage.Height, parentImage.ImageSize), newPixels);

            for (int i = 0; i < parentImage.Height; i++)
            {
                for (int j = 0; j < parentImage.Width; j++)
                {
                    newPixels[j, i] = new PixelRGB(
                        (byte)(parentImage.ImagePixels[j, i].red & b | (i < hiddenImage.Height && j < hiddenImage.Width ? hiddenImage.ImagePixels[j, i].red >> byteShift : 0x00)),
                        (byte)(parentImage.ImagePixels[j, i].green & b | (i < hiddenImage.Height && j < hiddenImage.Width ? hiddenImage.ImagePixels[j, i].green >> byteShift : 0x00)),
                        (byte)(parentImage.ImagePixels[j, i].blue & b | (i < hiddenImage.Height && j < hiddenImage.Width ? hiddenImage.ImagePixels[j, i].blue >> byteShift : 0x00))
                    );
                }
            }


            return returnImage;
        }

        public static MyImage RestoreHidden(MyImage parentImage, int byteShift)
        {
            byte b = Shift(byteShift);

            byteShift = 8 - byteShift;

            PixelRGB[,] newPixels = new PixelRGB[parentImage.Width, parentImage.Height];

            MyImage returnImage = new MyImage(parentImage.BuildHeader(parentImage.FileSize),
                parentImage.BuildHeaderInfo(parentImage.Width, parentImage.Height, parentImage.ImageSize), newPixels);

            for (int i = 0; i < parentImage.Height; i++)
            {
                for (int j = 0; j < parentImage.Width; j++)
                {
                    newPixels[j, i] = new PixelRGB(
                        (byte)((parentImage.ImagePixels[j, i].red) << byteShift),
                        (byte)((parentImage.ImagePixels[j, i].green) << byteShift),
                        (byte)((parentImage.ImagePixels[j, i].blue) << byteShift)
                    );
                }
            }

            return returnImage;
        }

        private static byte Shift(int byteShift)
        {
            if (byteShift > 7 | byteShift < 1)
                throw new Exception("You can only shift from 1 to 7 bytes!");
            return (byte)(Math.Pow(2, byteShift) - 1);
        }
    }
}

using System;
using System.IO;
using System.Linq;

namespace BMP_Console
{
    class MyImage
    {
        #region properties
        byte[] headerBytes;
        byte[] headerInfoBytes;

        Pixel[,] imagePixels;

        //The following are handeled by PopulateHeadersProperties method
        int height;
        int width;
        int bitsPerPixel;
        int bytesPerLine;
        string imageType;

        int imgOffset;

        #endregion properties

        #region access_control
        public Pixel[,] ImagePixels
        {
            get { return imagePixels; }
        }

        public int Width
        {
            get { return width; }
        }

        public int Height
        {
            get { return height; }
        }

        #endregion access_control

        #region constructors
        public MyImage(string path)
        {
            byte[] fileBytes = File.ReadAllBytes(path);

            headerBytes = new byte[14];

            //Populate headerBytes
            for (int i = 0; i < 14; i++)
                headerBytes[i] = fileBytes[i];

            //Check that the headers first bytes correspond to BMP File
            if (headerBytes[0] != 66 || headerBytes[1] != 77)
                throw new Exception("Invalid header");
            else
                imageType = "BM";

            //Get image offset from header
            imgOffset = Convertir_Endian_To_Int(new byte[] { headerBytes[10], headerBytes[11], headerBytes[12], headerBytes[13] });

            //Read the size of the DIM Header
            int DIMSize = Convertir_Endian_To_Int(new byte[] { fileBytes[14], fileBytes[15], fileBytes[16], fileBytes[17] });

            headerInfoBytes = new byte[DIMSize];

            //Populate headerInfoBytes
            for (int i = 14; i < 14 + DIMSize; i++)
                headerInfoBytes[i - 14] = fileBytes[i];

            PopulateHeaderProperties(headerBytes, headerInfoBytes);

            imagePixels = new Pixel[width, height];
            //Populate pixels
            imagePixels = new Pixel[width, height];

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    Pixel p = new Pixel(fileBytes[j * 3 + i * bytesPerLine + imgOffset], fileBytes[j * 3 + 1 + i * bytesPerLine + imgOffset], fileBytes[j * 3 + 2 + i * bytesPerLine + imgOffset]);

                    imagePixels[j, i] = p;
                }
            }
        }

        public MyImage(byte[] headerBytes, byte[] headerInfoBytes, Pixel[,] pixels)
        {
            this.headerBytes = headerBytes;
            this.headerInfoBytes = headerInfoBytes;
            this.imagePixels = pixels;

            PopulateHeaderProperties(headerBytes, headerInfoBytes);
        }

        #endregion constructors


        //Called after constructors 
        void PopulateHeaderProperties(byte[] headerBytes, byte[] headerInfoBytes)
        {
            width = Convertir_Endian_To_Int(new byte[] { headerInfoBytes[4], headerInfoBytes[5], headerInfoBytes[6], headerInfoBytes[7] });
            height = Convertir_Endian_To_Int(new byte[] { headerInfoBytes[8], headerInfoBytes[9], headerInfoBytes[10], headerInfoBytes[11] });
            bitsPerPixel = Convertir_Endian_To_Int(new byte[] { headerInfoBytes[14], headerInfoBytes[15] });
            bytesPerLine = (int)Math.Ceiling(bitsPerPixel * width / 32.0) * 4;
        }

        byte[] BuildHeaderInfoBytes(int newWidth, int newHeight)
        {
            byte[] newHeaderInfo = new byte[40];

            byte[] widthByte = Convertir_Int_To_Endian(newWidth);
            byte[] heightByte = Convertir_Int_To_Endian(newHeight);

            newHeaderInfo[4] = widthByte[0];
            newHeaderInfo[5] = widthByte[1];
            newHeaderInfo[6] = widthByte[2];
            newHeaderInfo[7] = widthByte[3];

            newHeaderInfo[8] = heightByte[0];
            newHeaderInfo[9] = heightByte[1];
            newHeaderInfo[10] = heightByte[2];
            newHeaderInfo[11] = heightByte[3];

            return newHeaderInfo;
        }



        public static int Convertir_Endian_To_Int(byte[] bytes)
        {
            int value = 0;

            for (int i = 0; i < bytes.Length; i++)
            {
                value += (int)Math.Pow(256, i) * bytes[i];
            }

            return value;
        }


        //Reminder change this
        static byte[] Convertir_Int_To_Endian(int val)
        {
            byte[] rep = null;
            if (val >= 0)
            {
                rep = new byte[4];

                if (val < 256)
                {
                    rep[0] = Convert.ToByte(val);
                    rep[1] = 0;
                    rep[2] = 0;
                    rep[3] = 0;
                }
                else
                {
                    rep[3] = Convert.ToByte(val / (256 * 256 * 256));
                    if (val >= 256 * 256 * 256)
                    {
                        int j = val / (256 * 256 * 256);
                        val -= j * 256 * 256 * 256;
                    }
                    rep[2] = Convert.ToByte(val / (256 * 256));
                    if (val >= 256 * 256)
                    {
                        int j = val / (256 * 256);
                        val -= j * 256 * 256;
                    }
                    rep[1] = Convert.ToByte(val / 256);
                    if (val >= 256)
                    {
                        int j = val / (256);
                        val -= j * 256;
                    }
                    rep[0] = Convert.ToByte(val);

                }
            }
            return rep;
        }


        public void From_Image_To_File(string path)
        {
            //Convert pixel matrix to bytes with padding
            byte[] newImageBytes = new byte[bytesPerLine * height];

            for(int i = 0; i < height; i++)
            {
                for(int j = 0; j < width; j++)
                {
                    newImageBytes[i * bytesPerLine + j * 3 ] = imagePixels[j, i].red;
                    newImageBytes[i * bytesPerLine + j * 3 + 1] = imagePixels[j, i].green;
                    newImageBytes[i * bytesPerLine + j * 3 + 2] = imagePixels[j, i].blue;
                }
            }

            //Rewrite without System.Linq
            File.WriteAllBytes(path, headerBytes.Concat(headerInfoBytes).Concat(newImageBytes).ToArray());
        }


        public MyImage ToGrayScale()
        {
            for(int i = 0; i < width; i++)
            {
                for(int j = 0; j < height; j++)
                {
                    Pixel p = imagePixels[i, j];
                    int grey = Convert.ToInt32(0.299 * p.red + 0.587 * p.green + 0.114 * p.blue);

                    p.red = Convert.ToByte(grey);
                    p.green = Convert.ToByte(grey);
                    p.blue = Convert.ToByte(grey);
                }
            }

            return new MyImage(headerBytes, headerInfoBytes, imagePixels);
        }


        public MyImage ToBlackAndWhite()
        {
            MyImage im = new MyImage(headerBytes, headerInfoBytes, imagePixels);
            
            for (int i = 0; i < im.width; i++)
            {
                for (int j = 0; j < im.height; j++)
                {

                    if (im.imagePixels[i, j].red < 64)
                    {
                        im.imagePixels[i, j].red = 0;
                        im.imagePixels[i, j].green = 0;
                        im.imagePixels[i, j].blue = 0;
                    }
                    else
                    {
                        im.imagePixels[i, j].red = 255;
                        im.imagePixels[i, j].green = 255;
                        im.imagePixels[i, j].blue = 255;
                    }
                }
            }

            return im;
        }


        public MyImage RescaleByFactor(double xFactor, double yFactor)
        {
            int newWidth = (int)(width * xFactor); 
            int newHeight = (int)(height * yFactor);

            return RescaleByWidth(newWidth, newHeight);
        }


        public MyImage RescaleByWidth(int newWidth, int newHeight)
        {
            Pixel[,] newPixels = new Pixel[newWidth, newHeight];

            Console.WriteLine(newWidth);
            Console.WriteLine(newHeight);

            //Only works if < 1
            int xPixelIndex = width / (newWidth - width);
            int yPixelIndex = width / (newHeight - height);


            int yIndex = 0;
            int yAddedPixels = 0;

            for (int i = 0; i < newHeight; i++)
            {
                int xIndex = 0;
                int xAddedPixels = 0;

                for (int j = 0; j < newWidth; j++)
                {
                    xIndex++;

                    if(xIndex > xPixelIndex)
                    {
                        xIndex = 0;
                        xAddedPixels++;
                    }
                    else
                    {
                        //Console.WriteLine(j - xAddedPixels);
                        //Console.WriteLine(i - yAddedPixels);
                        //newPixels[j, i] = ImagePixels[j - xAddedPixels, i - yAddedPixels];
                    }
                    
                }
            }

            return new MyImage(headerBytes, BuildHeaderInfoBytes(newWidth, newHeight), newPixels);
        }

        public MyImage Rotate(double rotationAngle) // Angle in degree
        {
            double rotationAngleRadian = (2 * Math.PI / 360); // Converting the angle in radiant.
            int newWidth = (int)(this.width * Math.Cos(rotationAngleRadian) + this.height * Math.Sin(rotationAngleRadian));
            int newHeight = (int)(this.height * Math.Cos(rotationAngleRadian) - this.width * Math.Sin(rotationAngleRadian));
            Pixel[,] newPixels = new Pixel[newWidth, newHeight];

            Pixel origin = ImagePixels[0, 0];
            //Pixel rightOrigin = ImagePixels[0, Get.];


            return new MyImage(headerBytes, headerInfoBytes, newPixels);

        }


    }
}

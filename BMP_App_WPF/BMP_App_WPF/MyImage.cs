﻿using System;
using System.IO;
using System.Linq;

namespace BMP_App_WPF
{
    public partial class MyImage
    {
        #region properties
        protected PixelRGB[,] imagePixels;

        //Header properties
        string imageType;
        int fileSize = 0;
        byte[] reservedBytes = new byte[4];
        int imgOffset = 54;

        //DIM Header properties
        int DIMsize = 40;
        int width;
        int height;
        int numberOfPlans = 1;
        int bitsPerPixel = 24;
        int compressionType = 0;
        int imageSize = 0;
        int horizontalResolution = 0;
        int verticalResolution = 0;
        int colorsInPalette = 0;
        int importantColor = 0;

        //The following are handeled by PopulateHeadersProperties method
        int bytesPerLine;


        #endregion properties

        #region access_control
        public PixelRGB[,] ImagePixels
        {
            get { return imagePixels; }
            set { imagePixels = value; }
        }

        public int Width
        {
            get { return width; }
            set { width = value; }
        }

        public int Height
        {
            get { return height; }
            set { height = value; }
        }
        public int FileSize
        {
            get { return fileSize; }
        }

        public int ImageSize
        {
            get { return imageSize; }
        }
        #endregion access_control

        #region constructors
        public MyImage(string path)
        {
            byte[] fileBytes = File.ReadAllBytes(path);

            byte[] headerBytes = new byte[14];

            Console.WriteLine(headerBytes[0]);

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

            byte[] headerInfoBytes = new byte[DIMSize];

            //Populate headerInfoBytes
            for (int i = 14; i < 14 + DIMSize; i++)
                headerInfoBytes[i - 14] = fileBytes[i];

            PopulateHeaderProperties(headerBytes, headerInfoBytes);

            imagePixels = new PixelRGB[width, height];
            //Populate pixels
            imagePixels = new PixelRGB[width, height];

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    PixelRGB p = new PixelRGB(fileBytes[j * 3 + i * bytesPerLine + imgOffset], fileBytes[j * 3 + 1 + i * bytesPerLine + imgOffset], fileBytes[j * 3 + 2 + i * bytesPerLine + imgOffset]);

                    imagePixels[j, i] = p;
                }
            }
        }

        public MyImage(byte[] headerBytes, byte[] headerInfoBytes, PixelRGB[,] pixels)
        {
            this.imagePixels = pixels;
            PopulateHeaderProperties(headerBytes, headerInfoBytes);
        }

        //Used to generate a blank image
        public MyImage(int width, int height)
        {
            imagePixels = new PixelRGB[width, height];

            imageType = "BM";
            this.width = width;
            this.height = height;
            bytesPerLine = (int)Math.Ceiling(bitsPerPixel * width / 32.0) * 4;

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    imagePixels[i, j] = new PixelRGB(255, 255, 255);
                }
            }
        }

        public MyImage()
        {
            imageType = "BM";
            width = 4000;
            height = 4000;
            bytesPerLine = (int)Math.Ceiling(bitsPerPixel * width / 32.0) * 4;

            imagePixels = new PixelRGB[width, height];

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    imagePixels[i, j] = new PixelRGB(0, 0, 0);
                }
            }
        }
        #endregion constructors


        #region sync
        void PopulateHeaderProperties(byte[] headerBytes, byte[] headerInfoBytes)
        {
            //Header properties
            imageType = (char)headerBytes[0] + "" + (char)headerBytes[1];
            fileSize = Convertir_Endian_To_Int(new byte[] { headerBytes[2], headerBytes[3], headerBytes[4], headerBytes[5] });
            reservedBytes = new byte[] { headerBytes[6], headerBytes[7], headerBytes[8], headerBytes[9] };

            //DIM Header properties
            width = Convertir_Endian_To_Int(new byte[] { headerInfoBytes[4], headerInfoBytes[5], headerInfoBytes[6], headerInfoBytes[7] });
            height = Convertir_Endian_To_Int(new byte[] { headerInfoBytes[8], headerInfoBytes[9], headerInfoBytes[10], headerInfoBytes[11] });

            numberOfPlans = Convertir_Endian_To_Int(new byte[] { headerInfoBytes[12], headerInfoBytes[13] });
            bitsPerPixel = Convertir_Endian_To_Int(new byte[] { headerInfoBytes[14], headerInfoBytes[15] });

            compressionType = Convertir_Endian_To_Int(new byte[] { headerInfoBytes[16], headerInfoBytes[17], headerInfoBytes[18], headerInfoBytes[19] });
            imageSize = Convertir_Endian_To_Int(new byte[] { headerInfoBytes[20], headerInfoBytes[21], headerInfoBytes[22], headerInfoBytes[23] });
            horizontalResolution = Convertir_Endian_To_Int(new byte[] { headerInfoBytes[24], headerInfoBytes[25], headerInfoBytes[26], headerInfoBytes[27] });
            verticalResolution = Convertir_Endian_To_Int(new byte[] { headerInfoBytes[28], headerInfoBytes[29], headerInfoBytes[30], headerInfoBytes[31] });
            colorsInPalette = Convertir_Endian_To_Int(new byte[] { headerInfoBytes[32], headerInfoBytes[33], headerInfoBytes[34], headerInfoBytes[35] });
            importantColor = Convertir_Endian_To_Int(new byte[] { headerInfoBytes[36], headerInfoBytes[37], headerInfoBytes[38], headerInfoBytes[39] });


            bytesPerLine = (int)Math.Ceiling(bitsPerPixel * width / 32.0) * 4;
        }

        internal byte[] BuildHeader(int fileSize)
        {
            byte[] imageTypeBytes = new byte[] { (byte)imageType[0], (byte)imageType[1] };
            byte[] fileSizeBytes = Convertir_Int_To_Endian(fileSize, 4);
            byte[] offsetBytes = Convertir_Int_To_Endian(imgOffset, 4);

            return imageTypeBytes.Concat(fileSizeBytes).Concat(reservedBytes).Concat(offsetBytes).ToArray();
        }

        internal byte[] BuildHeaderInfo(int width, int height, int imageSize)
        {
            byte[] sizeBytes = Convertir_Int_To_Endian(40, 4);
            byte[] widthBytes = Convertir_Int_To_Endian(width, 4);
            byte[] heightBytes = Convertir_Int_To_Endian(height, 4);
            byte[] plansBytes = Convertir_Int_To_Endian(numberOfPlans, 2);
            byte[] pixelBytes = Convertir_Int_To_Endian(bitsPerPixel, 2);
            byte[] compressionBytes = Convertir_Int_To_Endian(compressionType, 4);
            byte[] imageSizeBytes = Convertir_Int_To_Endian(imageSize, 4);
            byte[] horizontalResolutionBytes = Convertir_Int_To_Endian(horizontalResolution, 4);
            byte[] verticalResolutionBytes = Convertir_Int_To_Endian(verticalResolution, 4);
            byte[] colorPaletteBytes = Convertir_Int_To_Endian(colorsInPalette, 4);
            byte[] importantColorBytes = Convertir_Int_To_Endian(importantColor, 4);

            return sizeBytes.Concat(widthBytes).Concat(heightBytes).Concat(plansBytes).Concat(pixelBytes).Concat(compressionBytes).Concat(imageSizeBytes)
                .Concat(horizontalResolutionBytes).Concat(verticalResolutionBytes).Concat(colorPaletteBytes).Concat(importantColorBytes).ToArray();
        }

        #endregion sync


        public static int Convertir_Endian_To_Int(byte[] bytes)
        {
            int value = 0;

            for (int i = 0; i < bytes.Length; i++)
            {
                value += (int)Math.Pow(256, i) * bytes[i];
            }

            return value;
        }

        static byte[] Convertir_Int_To_Endian(int value, int size)
        {
            byte[] rep = new byte[size];
            size -= 1;

            while (size >= 0)
            {
                int calc = value / (int)Math.Pow(256, size);
                rep[size] = (byte)calc;

                size -= 1;
            }
            return rep;
        }

        public void From_Image_To_File(string path)
        {
            //Convert pixel matrix to bytes with padding
            byte[] newImageBytes = new byte[bytesPerLine * height];

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    newImageBytes[i * bytesPerLine + j * 3] = imagePixels[j, i].red;
                    newImageBytes[i * bytesPerLine + j * 3 + 1] = imagePixels[j, i].green;
                    newImageBytes[i * bytesPerLine + j * 3 + 2] = imagePixels[j, i].blue;
                }
            }
            File.WriteAllBytes(path, BuildHeader(fileSize)
                .Concat(BuildHeaderInfo(width, height, imageSize)).Concat(newImageBytes).ToArray());
        }


        public MyImage ToGrayScale()
        {
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    PixelRGB p = imagePixels[i, j];
                    int grey = Convert.ToInt32(0.299 * p.red + 0.587 * p.green + 0.114 * p.blue);

                    p.red = Convert.ToByte(grey);
                    p.green = Convert.ToByte(grey);
                    p.blue = Convert.ToByte(grey);
                }
            }

            return new MyImage(BuildHeader(fileSize), BuildHeaderInfo(width, height, imageSize), imagePixels);
        }


        public MyImage ToBlackAndWhite()
        {
            MyImage im = new MyImage(BuildHeader(fileSize), BuildHeaderInfo(width, height, imageSize), imagePixels);

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

            PixelRGB[,] newPixels = new PixelRGB[newWidth, newHeight];

            for (int i = 0; i < newHeight; i++)
            {
                for (int j = 0; j < newWidth; j++)
                {
                    int sourceX = (int)(j / xFactor);
                    int sourceY = (int)(i / yFactor);

                    newPixels[j, i] = imagePixels[sourceX, sourceY];
                }
            }

            return new MyImage(BuildHeader(fileSize), BuildHeaderInfo(newWidth, newHeight, 0), newPixels);
        }





        public MyImage Rotate(float rotationAngle) // Angle in degree
        {
            float rotationAngleRadian = (float)(2 * Math.PI * (-1) * rotationAngle / 360); // Converting the angle in radiant.
            uint newWidth = (uint)Math.Ceiling(Math.Abs(Width * Math.Cos(rotationAngle * Math.PI / 180)) + Math.Abs(Height * Math.Sin(rotationAngle * Math.PI / 180)));
            uint newHeight = (uint)Math.Ceiling(Math.Abs(Width * Math.Sin(rotationAngle * Math.PI / 180)) + Math.Abs(Height * Math.Cos(rotationAngle * Math.PI / 180)));
            PixelRGB[,] imageRotated = new PixelRGB[newWidth, newHeight]; // new image rotated
            float newCos = (float)Math.Cos(rotationAngleRadian);
            float newSin = (float)Math.Sin(rotationAngleRadian);
            PixelRGB origin = ImagePixels[0, 0]; // Ã  remove
                                                 //Pixel rightOrigin = ImagePixels[0, Get.];

            // R=Right, L=Left, U=Up, D=Down, N=New

            PixelRGB nUL = imagePixels[0, 0];

            float Point1x = height * newSin;
            float Point1y = height * newCos;
            float Point2x = width * newCos - height * newSin;
            float Point2y = height * newCos + width * newSin;
            float Point3x = width * newCos;
            float Point3y = width * newSin;


            float maxx = Math.Max(0, Math.Max(Point1x, Math.Max(Point2x, Point3x))); //We proceed to determine the maximum coordinates.
            float maxy = Math.Max(0, Math.Max(Point1y, Math.Max(Point2y, Point3y)));
            float minx = Math.Min(0, Math.Min(Point1x, Math.Min(Point2x, Point3x))); // We proceed to determine the minimum coordinates.
            float miny = Math.Min(0, Math.Min(Point1y, Math.Min(Point2y, Point3y)));





            for (int i = 0; i < newWidth; i++)
            {
                for (int j = 0; j < newHeight; j++)
                {


                    float originImageX = (i - newWidth / 2) * newCos - (j - newHeight / 2) * newSin + (width / 2);
                    float originImageY = (i - newWidth / 2) * newSin + (j - newHeight / 2) * newCos + (height / 2);

                    if (originImageX >= 0 && originImageX < width && originImageY >= 0 && originImageY < height)
                    {
                        imageRotated[i, j] = imagePixels[(int)originImageX, (int)originImageY];
                    }
                    else
                    {
                        imageRotated[i, j] = new PixelRGB(0, 0, 0);
                    }
                }
            }

            return new MyImage(BuildHeader(fileSize), BuildHeaderInfo((int)newWidth, (int)newHeight, imageSize), imageRotated);

        }

        public void Mirror_Horizontal()
        {
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width / 2; j++)
                {
                    PixelRGB p = imagePixels[j, i];
                    imagePixels[j, i] = imagePixels[width - j - 1, i];
                    imagePixels[width - j - 1, i] = p;
                }
            }
        }

        public void Mirror_Vertical()
        {
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height / 2; j++)
                {
                    PixelRGB p = imagePixels[i, j];
                    imagePixels[i, j] = imagePixels[i, height - j - 1];
                    imagePixels[i, height - j - 1] = p;
                }
            }
        }

        public static int[,] kernel = new int[,] { { -1, -1, -1 }, { -1, 8, -1 }, { -1, -1, -1 } };
        public static int[,] contrast = new int[,] { { 0, 0, 0, 0, 0 }, { 0, 0, -1, 0, 0 }, { 0, -1, 5, -1, 0 }, { 0, 0, -1, 0, 0 }, { 0, 0, 0, 0, 0 } };
        public static int[,] blur = new int[,] { { 1, 1, 1 }, { 1, 1, 1 }, { 1, 1, 1 } };
        public static int[,] bordReinforcement = new int[,] { { 0, 0, 0 }, { -1, 1, 0 }, { 0, 0, 0 } };
        public static int[,] pushback = new int[,] { { -2, -1, 0 }, { -1, 1, 1 }, { 0, 1, 2 } };
        public static int[,] bordDetection = new int[,] { { 0, 1, 0 }, { 1, -4, 1 }, { 0, 1, 0 } };



        public static int[][,] convolutions = {kernel, contrast, blur, bordReinforcement, pushback, bordDetection};

        public MyImage Convolution11(int[,] conv)
        {

            PixelRGB[,] imageModified = new PixelRGB[width, height];
            int convHeight = conv.GetLength(1);
            int convWidth = conv.GetLength(0);
            //Used for the edge pixels
            int ignoredPixX = convWidth / 2;
            int ignoredPixY = convHeight / 2;

            //image 
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)

                {
                    int newValueR = 0;
                    int newValueG = 0;
                    int newValueB = 0;

                    //conv 
                    for (int i = 0; i < convWidth; i++)
                    {
                        for (int j = 0; j < convHeight; j++)
                        {
                            //coordinates calculus
                            int pixelX = x + i - ignoredPixX;
                            int pixelY = y + j - ignoredPixY;

                            //Edge treated using the reflexion method

                            if (pixelX < 0)
                            {
                                pixelX = -pixelX;
                            }

                            else if (pixelX >= width)
                            {
                                pixelX = 2 * (width - 1) - pixelX;
                            }

                            if (pixelY < 0)
                            {
                                pixelY = -pixelY;
                            }

                            else if (pixelY >= height)
                            {
                                pixelY = 2 * (height - 1) - pixelY;
                            }



                            PixelRGB pixel = ImagePixels[pixelX, pixelY];
                            newValueR += conv[i, j] * pixel.red;
                            newValueG += conv[i, j] * pixel.green;
                            newValueB += conv[i, j] * pixel.blue;


                        }
                    }

                    PixelRGB pixelModified = new PixelRGB((byte)Math.Max(Math.Min(newValueR, 255), 0), (byte)Math.Max(Math.Min(newValueG, 255), 0), (byte)Math.Max(Math.Min(newValueB, 255), 0));

                    imageModified[x, y] = pixelModified;
                }
            }

            MyImage ImageConv = new MyImage(BuildHeader(fileSize), BuildHeaderInfo(width, height, imageSize), imageModified);

            return ImageConv;
        }

        public MyImage Negative()
        {
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    imagePixels[j, i] = new PixelRGB((byte)(255 - (int)imagePixels[j, i].red), (byte)(255 - (int)imagePixels[j, i].green), (byte)(255 - (int)imagePixels[j, i].blue));
                }
            }

            return this;
        }

        public MyImage Maths(int a, int b, int c, int d)
        {
            PixelRGB[,] pixels = new PixelRGB[200, 200];

            for (int i = 0; i < 200; i++)
            {
                for (int j = 0; j < 200; j++)
                {
                    if (j == 100 || i == 100)
                    {
                        pixels[i, j] = new PixelRGB(128, 128, 128);
                    }
                }
            }

            for (double x = -10; x < 10; x += 0.1)
            {
                if (a * x * x * x + b * x * x + c * x + d > -10 && a * x * x * x + b * x * x + c * x + d < 10)
                {
                    double nplus1 = a * (x + 0.1) * (x + 0.1) * (x + 0.1) + b * (x + 0.1) * (x + 0.1) + c * (x + 0.1) + d;
                    double nminus1 = a * (x - 0.1) * (x - 0.1) * (x - 0.1) + b * (x - 0.1) * (x - 0.1) + c * (x - 0.1) + d;

                    if (nplus1 > -10 & nplus1 < 10 & nminus1 > -10 & nminus1 < 10)
                    {
                        int nmax = (int)((nplus1 + 10) * 10);
                        int nmin = (int)((nminus1 + 10) * 10);

                        for (int i = Math.Min(nmax, nmin); i < Math.Max(nmax, nmin); i++)
                        {
                            pixels[(int)((x + 10) * 10), i] = new PixelRGB(0, 0, 0);
                        }
                    }
                }
            }

            for (int i = 0; i < 200; i++) // We fill the rest with white
            {
                for (int j = 0; j < 200; j++)
                {
                    if (pixels[i, j] == null)
                    {
                        pixels[i, j] = new PixelRGB(255, 255, 255);
                    }
                }
            }

            MyImage ImageMathRepresentation = new MyImage(BuildHeader(fileSize), BuildHeaderInfo(width, height, imageSize), pixels);
            return ImageMathRepresentation;
        }

        #region Fractals
        public void Compute_Mandelbrot(int maxIterations, PixelRGB color)
        {
            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    ComplexNumber c = new ComplexNumber((i - 2000) / (double)1000, (j - 2000) / (double)1000);
                    bool diverging = false;

                    ComplexNumber z = new ComplexNumber(0, 0);

                    int iterations = 0;
                    while (!diverging && iterations < maxIterations)
                    {
                        iterations++;

                        z = (z * z) + c;

                        if (z.abs > 2)
                        {
                            diverging = true;
                        }
                    }

                    if (!diverging)
                    {
                        imagePixels[i, j] = color;
                    }
                    else
                    {
                        {
                            double smooth = (double)iterations / maxIterations;
                            PixelRGB newColor = new PixelRGB(
                                (byte)(9 * (1 - smooth) * (1 - smooth) * smooth * smooth * smooth * 255),
                                (byte)(15 * (1 - smooth) * (1 - smooth) * smooth * smooth * 255),
                                (byte)(8.5 * (1 - smooth) * (1 - smooth) * (1 - smooth) * smooth * 255));
                            imagePixels[i, j] = newColor;
                        }
                    }
                }
            }
        }

        public void Compute_Julia(int maxIterations, PixelRGB color, double cr, double ci)
        {
            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    ComplexNumber z = new ComplexNumber((i - 2000) / (double)1000, (j - 2000) / (double)1000);
                    bool diverging = false;

                    ComplexNumber c = new ComplexNumber(cr, ci);

                    int iterations = 0;
                    while (!diverging && iterations < maxIterations)
                    {
                        iterations++;

                        z = (z * z) + c;

                        if (z.abs > 2)
                        {
                            diverging = true;
                        }
                    }

                    if (!diverging)
                    {
                        imagePixels[i, j] = color;
                    }
                    else
                    {
                        {
                            double smooth = (double)iterations / maxIterations;
                            PixelRGB newColor = new PixelRGB(
                                (byte)(9 * (1 - smooth) * (1 - smooth) * smooth * smooth * smooth * 255),
                                (byte)(15 * (1 - smooth) * (1 - smooth) * smooth * smooth * 255),
                                (byte)(8.5 * (1 - smooth) * (1 - smooth) * (1 - smooth) * smooth * 255));
                            imagePixels[i, j] = newColor;
                        }
                    }
                }
            }
        }

        public void Julia(double cr, double ci)
        {
            Compute_Julia(100, new PixelRGB(200, 200, 10), cr, ci);
        }

        public void Mandelbrot()
        {
            Compute_Mandelbrot(100, new PixelRGB(200, 200, 10));
        }

        #endregion Fractals

    }
}

using System;
using System.IO;
using System.Linq;

namespace BMP_Console
{
    partial class MyImage
    {
        #region properties
        protected Pixel[,] imagePixels;

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
        public Pixel[,] ImagePixels
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

            //Populate other properties
            width = Convertir_Endian_To_Int(new byte[] { headerInfoBytes[4], headerInfoBytes[5], headerInfoBytes[6], headerInfoBytes[7] });
            height = Convertir_Endian_To_Int(new byte[] { headerInfoBytes[8], headerInfoBytes[9], headerInfoBytes[10], headerInfoBytes[11] });
            bitsPerPixel = Convertir_Endian_To_Int(new byte[] { headerInfoBytes[14], headerInfoBytes[15] });
            bytesPerLine = (int)Math.Ceiling(bitsPerPixel * width / 32.0) * 4;

            //Populate imageBytes
            imageBytes = new byte[fileBytes.Length - imgOffset];

            for (int i = imgOffset; i < fileBytes.Length; i++)
                imageBytes[i - imgOffset] = fileBytes[i];
        }


        public MyImage(byte[] headerBytes, byte[] headerInfoBytes, byte[] imageBytes)
        {
            this.imagePixels = pixels;

            PopulateHeaderProperties(headerBytes, headerInfoBytes);
        }

        //Used to generate a blank image
        public MyImage(int width, int height)
        {
            imagePixels = new Pixel[width, height];

            imageType = "BM";
            this.width = width;
            this.height = height;
            bytesPerLine = (int)Math.Ceiling(bitsPerPixel * width / 32.0) * 4;

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    imagePixels[i, j] = new Pixel(0, 0, 0);
                }
            }
        }

        //Used to generate a blank image of 2000x2000, for fractals
        public MyImage()
        {
            imageType = "BM";
            width = 4000;
            height = 4000;
            bytesPerLine = (int)Math.Ceiling(bitsPerPixel * width / 32.0) * 4;

            imagePixels = new Pixel[width, height];

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    imagePixels[i, j] = new Pixel(0, 0, 0);
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
                    try
                    {
                        newImageBytes[i * bytesPerLine + j * 3] = imagePixels[j, i].red;
                        newImageBytes[i * bytesPerLine + j * 3 + 1] = imagePixels[j, i].green;
                        newImageBytes[i * bytesPerLine + j * 3 + 2] = imagePixels[j, i].blue;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(i * bytesPerLine + j * 3);
                    }
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
                    int r = imageBytes[i * bytesPerLine + j];
                    int g = imageBytes[i * bytesPerLine + j + 1];
                    int b = imageBytes[i * bytesPerLine + j + 2];

                    int grey = Convert.ToInt32(0.299 * r + 0.587 * g + 0.114 * b);

                    newImageBytes[i * bytesPerLine + j] = Convert.ToByte(grey);
                    newImageBytes[i * bytesPerLine + j + 1] = Convert.ToByte(grey);
                    newImageBytes[i * bytesPerLine + j + 2] = Convert.ToByte(grey);

                    j += 3;
                }
            }

            return new MyImage(BuildHeader(fileSize), BuildHeaderInfo(width, height, imageSize), imagePixels);
        }

        public MyImage ToBlackAndWhite()
        {
            MyImage im = new MyImage(BuildHeader(fileSize), BuildHeaderInfo(width, height, imageSize), imagePixels);

            for (int i = 0; i < im.width; i++)
            {
                if (image.imageBytes[i] >= 64)      //Adapt treshold
                {
                    image.imageBytes[i] = 255;
                }
                else
                {
                    image.imageBytes[i] = 0;
                }
            }

            return image;
        }

        public MyImage RescaleByFactor(double xFactor, double yFactor)
        {
            int newWidth = (int)(width * xFactor);
            int newHeight = (int)(height * yFactor);

            Pixel[,] newPixels = new Pixel[newWidth, newHeight];

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


        public MyImage Rotate(double rotationAngle) // Angle in degree
        {
            double rotationAngleRadian = (2 * Math.PI / 360); // Converting the angle in radiant.
            int newWidth = (int)(this.width * Math.Cos(rotationAngleRadian) + this.height * Math.Sin(rotationAngleRadian));
            int newHeight = (int)(this.height * Math.Cos(rotationAngleRadian) - this.width * Math.Sin(rotationAngleRadian));
            Pixel[,] newPixels = new Pixel[newWidth, newHeight];

            Pixel origin = ImagePixels[0, 0];
            //Pixel rightOrigin = ImagePixels[0, Get.];

            //return new MyImage(headerBytes, headerInfoBytes, newPixels);
            return null;
        }

        public void Mirror_Horizontal()
        {
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width / 2; j++)
                {
                    Pixel p = imagePixels[j, i];
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
                    Pixel p = imagePixels[i, j];
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

        public MyImage Conv(int[,] conv)
        {
            //FIX TRES TEMPORAIRE C'EST MOYEN
            Pixel[,] imageModified = imagePixels;

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    //Calcul de la nouvelle valeur du pixel en appliquant la matrice de convolution
                    int newValueR = 0;
                    int newValueG = 0;
                    int newValueB = 0;

                    for (int i = -1; i <= 1; i++)
                    {
                        for (int j = -1; j <= 1; j++)
                        {
                            Pixel pixelOrigin = ImagePixels[x + i, y + j];
                            newValueR += conv[i + 1, j + 1] * pixelOrigin.red;
                            newValueG += conv[i + 1, j + 1] * pixelOrigin.green;
                            newValueB += conv[i + 1, j + 1] * pixelOrigin.blue;
                        }
                    }

                    //On s'assure que la valeur reste dans la plage de couleurs de 0 à 255
                    byte newRedvalue = (byte)Math.Max(Math.Min(newValueR, 255), 0);
                    byte newGreenvalue = (byte)Math.Max(Math.Min(newValueG, 255), 0);
                    byte newBluevalue = (byte)Math.Max(Math.Min(newValueB, 255), 0);


                    //Création du nouveau pixel avec la valeur calculée
                    Pixel pixelModified = new Pixel(newRedvalue, newGreenvalue, newBluevalue);

                    //On affecte le nouveau pixel à l'image résultante
                    imageModified[x - 1, y - 1] = pixelModified;
                }
            }
            return new MyImage(BuildHeader(fileSize), BuildHeaderInfo(width, height, imageSize), imageModified);
        }
    }
}

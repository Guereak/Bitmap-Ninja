using System;
using System.IO;
using System.Linq;

namespace BMP_Console
{
    class MyImage
    {
        #region properties

        protected Pixel[,] imagePixels;


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
            set { width = value; }
        }

        public int Height
        {
            get { return height; }
            set { height = value; }
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
                for(int j = 0; j < height; j++)
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
                        Console.WriteLine(i - yAddedPixels);
                        //newPixels[j, i] = ImagePixels[j - xAddedPixels, i - yAddedPixels];
                    }
                    
                }
            }

            return new MyImage(headerBytes, BuildHeaderInfoBytes(newWidth, newHeight), newPixels);
        }

        public MyImage Rotate(float rotationAngle) // Angle in degree
        {
            float rotationAngleRadian = (float) (2 * Math.PI * rotationAngle /360); // Converting the angle in radiant.
            int newWidth = (int)(this.width * Math.Cos(rotationAngleRadian) + this.height * Math.Sin(rotationAngleRadian)); // New dimensions
            int newHeight = (int)(this.height * Math.Cos(rotationAngleRadian) - this.width * Math.Sin(rotationAngleRadian));
            Pixel[,] imageRotated = new Pixel[newWidth, newHeight]; // new image rotated
            float newCos = (float)Math.Cos(rotationAngleRadian);
            float newSin = (float)Math.Sin(rotationAngleRadian);
            Pixel origin = ImagePixels[0, 0]; // à remove
                                              //Pixel rightOrigin = ImagePixels[0, Get.];

            // R=Right, L=Left, U=Up, D=Down, N=New

            Pixel nUL = imagePixels[0, 0];

            float Point1x = height * newSin;
            float Point1y = height * newCos;
            float Point2x = width * newCos - height * newSin;
            float Point2y = height * newCos + width * newSin;
            float Point3x = width * newCos;
            float Point3y = width * newSin;

            /*
            float maxx = Math.Max(0, Math.Max(Point1x, Math.Max(Point2x, Point3x))); //We proceed to determine the maximum coordinates.
            float maxy = Math.Max(0, Math.Max(Point1y, Math.Max(Point2y, Point3y)));
            */
            
        
            float minx = Math.Min(0, Math.Min(Point1x, Math.Min(Point2x, Point3x))); // We proceed to determine the minimum coordinates.
            float miny = Math.Min(0, Math.Min(Point1y, Math.Min(Point2y, Point3y)));
       
            


            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {

                    int originImageY = (int)((j + miny) * newCos - (i + minx) * newSin);

                    int originImageX = (int)((i + minx) * newCos + (j + miny) * newSin);
                    
                    if (originImageX >= 0 && originImageX < newWidth && originImageY >= 0 && originImageY < newHeight)
                    {
                        imageRotated[i, j] = imagePixels[originImageX, originImageY];
                    }
                }
            }



            return new MyImage(headerBytes, headerInfoBytes, imageRotated);

        }

        public MyImage Conv()
        {
            int[,] kernel = new int[,] { { -1, -1, -1 }, { -1, 8, -1 }, { -1, -1, -1 } };
            int[,] contrast = new int[,] { { 0, 0, 0, 0, 0 }, { 0, 0, -1, 0, 0 }, { 0, -1, 5, -1, 0 }, { 0, 0, -1, 0, 0 }, { 0, 0, 0, 0, 0 } };
            int[,] blur = new int[,] { { 0, 0, 0, 0, 0 }, { 0, 1, 1, 1, 0 }, { 0, 1, 1, 1, 0 }, { 0, 1, 1, 1, 0 }, { 0, 0, 0, 0, 0 } };
            int[,] bordReinforcement = new int[,] { { 0, 0, 0 }, { -1, 1, 0 }, { 0, 0, 0 } };
            int[,] pushback = new int[,] { { -2, -1, 0 }, { -1, 1, 1 }, { 0, 1, 2 } };
            int[,] bordDetection = new int[,] { { 0, 1, 0 }, { 1, -4, 1 }, { 0, 1, 0 } };


            Console.WriteLine("What are you looking to do ? Choose between blurring, bordReinforcement, pushback and bordDetection by typing the word");
            string answ = Console.ReadLine();
            int[,] conv=null;

            if (answ == "blurrring")
            {
                conv = blur;
            }
            else if (answ == "bordReinforcement")
            {
                conv = bordReinforcement;
            }
            else if (answ == "bordDetection")
            {
                conv = bordDetection;
            }
            else if (answ == "pushback")
            {
                conv = pushback;
            }



            //Définition de la taille de l'image
            int width = this.width;
            int height = this.height;

            //Création d'une nouvelle image pour stocker le résultat
            Pixel [,] imageModified = new Pixel [width,height];

            //Boucle à travers chaque pixel de l'image
            for (int x = 1; x < width - 1; x++)
            {
                for (int y = 1; y < height - 1; y++)
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
                    Pixel pixelModified =new Pixel(newRedvalue, newGreenvalue, newBluevalue);

                    //On affecte le nouveau pixel à l'image résultante
                    imageModified[x - 1, y - 1] = pixelModified;
                }
            }


            return new MyImage(headerBytes, headerInfoBytes, imageModified);


        }


    }
}

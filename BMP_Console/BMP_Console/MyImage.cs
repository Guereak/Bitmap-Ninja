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

        //Make it into a matrix
        byte[] imageBytes;
        Pixel[,] imagePixels;

        int height;
        int width;
        int imgOffset;
        int bitsPerPixel;
        int bytesPerLine;

        string imageType;

        #endregion properties

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

            //Populate other properties
            width = Convertir_Endian_To_Int(new byte[] { headerInfoBytes[4], headerInfoBytes[5], headerInfoBytes[6], headerInfoBytes[7] });
            height = Convertir_Endian_To_Int(new byte[] { headerInfoBytes[8], headerInfoBytes[9], headerInfoBytes[10], headerInfoBytes[11] });
            bitsPerPixel = Convertir_Endian_To_Int(new byte[] { headerInfoBytes[14], headerInfoBytes[15] });
            bytesPerLine = (int)Math.Ceiling(bitsPerPixel * width / 32.0) * 4;

            //Populate imageBytes
            imageBytes = new byte[fileBytes.Length - imgOffset];
            //
            //for (int i = imgOffset; i < fileBytes.Length; i++)
            //    imageBytes[i - imgOffset] = fileBytes[i];

            PopulatePixels();
        }

        //Discarded
        public MyImage(byte[] headerBytes, byte[] headerInfoBytes, byte[] imageBytes)
        {
            this.headerBytes = headerBytes;
            this.headerInfoBytes = headerInfoBytes;
            this.imageBytes = imageBytes;
        }

        public MyImage(byte[] headerBytes, byte[] headerInfoBytes, Pixel[,] pixels)
        {
            this.headerBytes = headerBytes;
            this.headerInfoBytes = headerInfoBytes;
            this.imagePixels = pixels;
        }

        #endregion constructors

        public void PopulatePixels()
        {
            imagePixels = new Pixel[width, height];

            for(int i = 0; i < height; i++)
            {
                for(int j = 0; j < width; j++)
                {
                    Pixel p = new Pixel(imageBytes[j + i * bytesPerLine], imageBytes[j + 1 + i * bytesPerLine], imageBytes[j + 2 + i * bytesPerLine]);

                    imagePixels[j, i] = p;
                }
            }
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
            //Rewrite without System.Linq
            File.WriteAllBytes(path, headerBytes.Concat(headerInfoBytes).Concat(imageBytes).ToArray());
        }

        public void From_Image_To_File2(string path)
        {
            //Convert pixel matrix to bytes with padding
            byte[] newImageBytes = new byte[bytesPerLine * height];

            for(int i = 0; i < height; i++)
            {
                for(int j = 0; j < width; j++)
                {
                    newImageBytes[i * bytesPerLine + j * 3] = imagePixels[j, i].red;
                    newImageBytes[i * bytesPerLine + j * 3 + 1] = imagePixels[j, i].green;
                    newImageBytes[i * bytesPerLine + j * 3 + 2] = imagePixels[j, i].blue;
                }
            }

            //Rewrite without System.Linq
            File.WriteAllBytes(path, headerBytes.Concat(headerInfoBytes).Concat(newImageBytes).ToArray());
        }

        public MyImage ToGrayScale2()
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


        public MyImage ToGrayScale()
        {
            byte[] newImageBytes = new byte[imageBytes.Length];

            for (int i = 0; i < height; i++)
            {
                int j = 0;
                while (j + 2 <= bytesPerLine)
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
            return new MyImage(headerBytes, headerInfoBytes, newImageBytes);
        }

        public MyImage ToBlackAndWhite()
        {

            MyImage image = ToGrayScale();

            for(int i = 0; i < image.imageBytes.Length; i++)
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
    }
}

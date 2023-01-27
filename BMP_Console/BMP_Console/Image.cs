using System;
using System.IO;
using System.Security.Cryptography;

namespace BMP_Console
{
    class Image
    {
        #region properties
        byte[] fileBytes;
        byte[] headerBytes = new byte[14];
        byte[] headerInfoBytes = new byte[40];
        byte[] imageBytes;

        int height;
        int width;
        int bitsPerPixel;
        int bytesPerLine;

        #endregion properties

        #region constructors
        public Image(string path)
        {
            fileBytes = File.ReadAllBytes(path);

            ResolveHeaders();
        }


        public Image(byte[] headerBytes, byte[] headerInfoBytes, byte[] imageBytes)
        {
            fileBytes = new byte[imageBytes.Length + 54];

            for(int i = 0; i < 14; i++)
            {
                fileBytes[i] += headerBytes[i];
            }

            for (int i = 0; i < 40; i++)
            {
                fileBytes[i + 14] += headerInfoBytes[i];
            }
            for (int i = 0; i < imageBytes.Length; i++)
            {
                fileBytes[i + 54] += imageBytes[i];
            }
        }
        #endregion constructors


        public void ResolveHeaders()
        {
            //Populate headerBytes
            for (int i = 0; i < 14; i++)
                headerBytes[i] = fileBytes[i];

            //Check that the headers first bytes correspond to BMP File
            if (headerBytes[0] != 66 || headerBytes[1] != 77)
                throw new Exception("Invalid header");

            //Get image offset from header
            int imgOffset = ReadLittleEndian(new byte[] { headerBytes[10], headerBytes[11], headerBytes[12], headerBytes[13] });

            //Read the size of the DIM Header
            int DIMSize = ReadLittleEndian(new byte[]{ fileBytes[14], fileBytes[15], fileBytes[16], fileBytes[17] });

            headerInfoBytes = new byte[DIMSize];

            //Populate headerInfoBytes
            for (int i = 14; i < 14 + DIMSize; i++)
                headerInfoBytes[i - 14] = fileBytes[i];

            //Populate other properties
            width = ReadLittleEndian(new byte[] { headerInfoBytes[4], headerInfoBytes[5], headerInfoBytes[6], headerInfoBytes[7] });
            height = ReadLittleEndian(new byte[] { headerInfoBytes[8], headerInfoBytes[9], headerInfoBytes[10], headerInfoBytes[11] });
            bitsPerPixel = ReadLittleEndian(new byte[] { headerInfoBytes[14], headerInfoBytes[15] });
            bytesPerLine = (int)Math.Ceiling(bitsPerPixel * width / 32.0) * 4;

            //Populate imageBytes
            imageBytes = new byte[fileBytes.Length - imgOffset];

            for (int i = imgOffset; i < fileBytes.Length; i++)
                imageBytes[i - imgOffset] = fileBytes[i];
        }

        public static int ReadLittleEndian(byte[] bytes)
        {
            int value = 0;

            for(int i = 0; i < bytes.Length; i++)
            {
                value += (int)Math.Pow(256, i) * bytes[i];
            }

            return value;
        }


        public void Save(string path)
        {
            File.WriteAllBytes(path, fileBytes);
        }


        public Image ToGrayScale()
        {
            byte[] newImageBytes = new byte[imageBytes.Length];

            for(int i = 0; i < height; i++)
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
            return new Image(headerBytes, headerInfoBytes, newImageBytes);
        }
    }
}

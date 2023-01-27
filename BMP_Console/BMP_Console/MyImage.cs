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

            for (int i = imgOffset; i < fileBytes.Length; i++)
                imageBytes[i - imgOffset] = fileBytes[i];
        }


        public MyImage(byte[] headerBytes, byte[] headerInfoBytes, byte[] imageBytes)
        {
            this.headerBytes = headerBytes;
            this.headerInfoBytes = headerInfoBytes;
            this.imageBytes = imageBytes;
        }
        #endregion constructors

        public static int Convertir_Endian_To_Int(byte[] bytes)
        {
            int value = 0;

            for (int i = 0; i < bytes.Length; i++)
            {
                value += (int)Math.Pow(256, i) * bytes[i];
            }

            return value;
        }

        public byte[] Convertir_Int_To_Endian(int val)      //TODO
        {
            throw new NotImplementedException();
        }

        public void From_Image_To_File(string path)
        {
            //Rewrite without System.Linq
            File.WriteAllBytes(path, headerBytes.Concat(headerInfoBytes).Concat(imageBytes).ToArray());
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
    }
}

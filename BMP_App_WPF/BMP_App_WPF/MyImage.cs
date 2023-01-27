using System;
using System.IO;

namespace BMP_App_WPF
{
    class MyImage
    {
        byte[] fileBytes;
        byte[] headerBytes = new byte[14];
        byte[] headerInfoBytes = new byte[40];
        byte[] imageBytes;

        public byte[] ImageBytes
        {
            get { return imageBytes; }
        }

        public MyImage(string path)
        {
            fileBytes = File.ReadAllBytes(path);

            //File metadata: header
            for (int i = 0; i < 14; i++)
                headerBytes[i] = fileBytes[i];

            //Image metadata: DIB header 
            for (int i = 14; i < 54; i++)
                headerInfoBytes[i - 14] = fileBytes[i];

            //Pixel storage
            imageBytes = new byte[fileBytes.Length - 54];

            for (int i = 54; i < fileBytes.Length; i++)
                imageBytes[i - 54] = fileBytes[i];
        }


        public MyImage(byte[] headerBytes, byte[] headerInfoBytes, byte[] imageBytes)
        {
            fileBytes = new byte[imageBytes.Length + 54];

            for (int i = 0; i < 14; i++)
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


        public void Save(string path)
        {
            File.WriteAllBytes(path, fileBytes);
        }


        public MyImage ToGrayScale()
        {
            byte[] newImageBytes = new byte[imageBytes.Length];

            for (int i = 0; i < imageBytes.Length; i += 3)
            {
                int r = imageBytes[i];
                int g = imageBytes[i + 1];
                int b = imageBytes[i + 2];

                //TODO: might OutOfRange if more headers

                int grey = Convert.ToInt32(0.299 * r + 0.587 * g + 0.114 * b);

                newImageBytes[i] = Convert.ToByte(grey);
                newImageBytes[i + 1] = Convert.ToByte(grey);
                newImageBytes[i + 2] = Convert.ToByte(grey);
            }

            return new MyImage(headerBytes, headerInfoBytes, newImageBytes);
        }
    }
}

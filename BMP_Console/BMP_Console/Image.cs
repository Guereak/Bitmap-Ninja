using System;
using System.IO;
using System.Security.Cryptography;

namespace BMP_Console
{
    class Image
    {

        byte[] fileBytes;
        byte[] headerBytes = new byte[14];
        byte[] headerInfoBytes = new byte[40];
        byte[] imageBytes;

        public Image(string path)
        {
            fileBytes = File.ReadAllBytes(path);

            //File metadata: header
            for (int i = 0; i < 14; i++)
                headerBytes[i] = fileBytes[i];

            //Image metadata: DIB header 
            for (int i = 14; i < 54; i++)
                headerInfoBytes[i-14] = fileBytes[i];

            //Pixel storage
            imageBytes = new byte[fileBytes.Length - 54];

            for (int i = 54; i < fileBytes.Length; i++)
                imageBytes[i - 54] = fileBytes[i];
        }

        public void Save(string path)
        {
            File.WriteAllBytes(path, fileBytes);
        }
    }
}

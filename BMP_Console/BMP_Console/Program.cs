﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BMP_Console
{
    class Program
    {
        static void Main(string[] args)
        {
            TestRotateByFactor();
            Console.WriteLine("Done");
            Console.ReadKey();
        }

        static void TestGrayScale()
        {
            MyImage img = new MyImage("../../SampleImages/lac.bmp");

            MyImage imgGrey = img.ToGrayScale();

            imgGrey.From_Image_To_File("../../OutputImages/lacGreyscale.bmp");
        }

        static void TestBlackAndWhite()
        {
            MyImage img = new MyImage("../../SampleImages/lac.bmp");

            MyImage imgBW = img.ToBlackAndWhite();

            imgBW.From_Image_To_File("../../OutputImages/lacBlackAndWhite.bmp");
        }
        static void TestRescaleByFactor()
        {
            MyImage img = new MyImage("../../SampleImages/lac.bmp");

            MyImage imgRescaled = img.RescaleByFactor(1.2, 1.2);

            imgRescaled.From_Image_To_File("../../OutputImages/lacRescaled.bmp");
        }

        static void TestRotateByFactor()
        {
            MyImage img = new MyImage("../../SampleImages/lac.bmp");
            MyImage imgRotated = img.Rotate(90);
            imgRotated.From_Image_To_File("../../OutputImages/lacRescaled.bmp");


        }

        static void TestMatConv()
        {

            MyImage img = new MyImage("../../SampleImages/lac.bmp");
            MyImage imgConv = img.Conv();
            imgConv.From_Image_To_File("../../OutputImages/lacRescaled.bmp");



        }
    }
}

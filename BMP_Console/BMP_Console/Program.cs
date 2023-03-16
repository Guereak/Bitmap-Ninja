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
            //TestFractal();
            TestConv();
            //TestGrayScale();
            //TestBlackAndWhite();
            //TestRescaleByFactor();
            //TestMirror();
            //TestBlankImage();
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

            MyImage imgRescaled = img.RescaleByFactor(0.1, 0.1);

            Console.WriteLine(imgRescaled.Height);
            Console.WriteLine(imgRescaled.Width);

            imgRescaled.From_Image_To_File("../../OutputImages/lacRescaled.bmp");
        }

        static void TestBlankImage()
        {
            MyImage img1 = new MyImage(1500, 1300);
            MyImage img2 = new MyImage();

            img1.From_Image_To_File("../../OutputImages/blank1.bmp");
            img2.From_Image_To_File("../../OutputImages/blank2.bmp");
        }

        static void TestMirror()
        {
            MyImage img1 = new MyImage("../../SampleImages/lac.bmp");
            MyImage img2 = new MyImage("../../SampleImages/lac.bmp");
            img1.Mirror_Vertical();
            img2.Mirror_Horizontal();

            img1.From_Image_To_File("../../OutputImages/lacMirroredVertical.bmp");
            img2.From_Image_To_File("../../OutputImages/lacMirroredHorizontal.bmp");
        }

        static void TestFractal()
        {
            //Used to time performance of the method
            
            FractalImage fractal = new FractalImage(4000, 4000);
            fractal.Mandelbrot();

            fractal.From_Image_To_File("../../OutputImages/Mandelbrot.bmp");
        }

        static void TestConv()
        {
            MyImage img = new MyImage("../../SampleImages/coco.bmp");

            img = img.Conv(MyImage.contrast);

            img.From_Image_To_File("../../OutputImages/cocoConv.bmp");
        }
    }
}

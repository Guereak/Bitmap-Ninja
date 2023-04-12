using System;
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
            //var watch = System.Diagnostics.Stopwatch.StartNew();
            //JPEG.DisplayMatrix(JPEG.DiscreteCosineTransform(JPEG.test));
            //
            //watch.Stop();
            //Console.WriteLine(watch.ElapsedMilliseconds);
            //byte[] huff = { 00, 01, 02, 05, 02, 07, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 11, 12, 31, 00, 01, 02, 13, 14, 21, 51, 03, 15, 32, 33, 34, 61, 72 };
            //
            //Huffman.PrintDict(Huffman.BinaryToHuffmanTable(huff));

            TestCompression();

            //TestRGBYCbCr();

            //TestMandelbrot();
            //TestJulia();
            //TestConv();
            //TestGrayScale();
            //TestBlackAndWhite();
            //TestRescaleByFactor();
            //TestMirror();
            //TestBlankImage();
            //TestRotate();
            //TestConv();
            //TestStega();
            Console.WriteLine("Done");
            Console.ReadKey();
        }

        static void TestGrayScale()
        {
            MyImage img = new MyImage("../../SampleImages/test2.bmp");

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

        static void TestMandelbrot()
        {
            //Used to time performance of the method

            FractalImage fractal = new FractalImage(4000, 4000);
            fractal.Mandelbrot();

            fractal.From_Image_To_File("../../OutputImages/Mandelbrot.bmp");
        }

        static void TestJulia()
        {
            FractalImage fractal = new FractalImage(4000, 4000);
            fractal.Julia();

            fractal.From_Image_To_File("../../OutputImages/Julia.bmp");
        }

        static void TestRotate()
        {
            MyImage img = new MyImage("../../SampleImages/lac.bmp");

            img = img.RotateV3(60);

            img.From_Image_To_File("../../OutputImages/lacRot.bmp");
        }


        static void TestConv()
        {
            MyImage img = new MyImage("../../SampleImages/lac.bmp");
            img = img.Convolution11(MyImage.pushback);
            img.From_Image_To_File("../../OutputImages/lacConvo.bmp");

        }

        static void TestStega()
        {
            MyImage parentImage = new MyImage("../../SampleImages/lena.bmp");
            MyImage hiddenImage = new MyImage("../../SampleImages/coco.bmp");

            MyImage result = Steganography.Hide(parentImage, hiddenImage, 4);
            result.From_Image_To_File("../../OutputImages/stega.bmp");

            MyImage retrieved = Steganography.RestoreHidden(result, 4);
            retrieved.From_Image_To_File("../../OutputImages/stegaHidden.bmp");
        }

        static void TestRGBYCbCr()
        {
            PixelRGB p1 = new PixelRGB(102, 255, 255);
            Console.WriteLine($"{p1.red}, {p1.green}, {p1.blue}");

            PixelYCbCr p2 = p1.ToYCbCr();
            Console.WriteLine($"{p2.Y}, {p2.Cb}, {p2.Cr}");

            PixelRGB p3 = p2.toRGB();
            Console.WriteLine($"{p3.red}, {p3.green}, {p3.blue}");
        }

        static void TestCompression()
        {
            MyImage parentImage = new MyImage("../../SampleImages/test2.bmp");

            JPEG.FullJpegCompression(parentImage);
        }
    }
}
    
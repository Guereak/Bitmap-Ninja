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
            TestGrayScale();
            TestBlackAndWhite();
            TestRescaleByFactor();
            TestBlankImage();
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
    }
}

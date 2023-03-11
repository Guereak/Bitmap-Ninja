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

            MyImage imgRescaled = img.RescaleByFactor(0.9, 1.5);

            Console.WriteLine(imgRescaled.Height);
            Console.WriteLine(imgRescaled.Width);

            imgRescaled.From_Image_To_File("../../OutputImages/lacRescaled.bmp");
        }
    }
}

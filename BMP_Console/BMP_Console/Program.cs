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
            TestGrayScale2();
            Console.ReadKey();
        }

        static void TestGrayScale()
        {
            MyImage img = new MyImage("../../SampleImages/lac.bmp");

            MyImage imgGrey = img.ToGrayScale();

            imgGrey.From_Image_To_File("../../SampleImages/lacGreyscale.bmp");
        }

        static void TestGrayScale2()
        {
            MyImage img = new MyImage("../../SampleImages/lac.bmp");

            //MyImage imgGrey = img.ToGrayScale2();

            img.From_Image_To_File2("../../SampleImages/lacGreyscale2.bmp");
        }

        static void TestBlackAndWhite()
        {
            MyImage Initial = new MyImage("../../SampleImages/lac.bmp");

            MyImage NoirEtBlanc = Initial.ToBlackAndWhite();

            NoirEtBlanc.From_Image_To_File("../../SampleImages/lacBlackAndWhite.bmp");
        }
    }
}

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
            MyImage Initial = new MyImage("../../SampleImages/lac.bmp");

            MyImage NoirEtBlanc = Initial.ToBlackAndWhite();

            NoirEtBlanc.From_Image_To_File("../../OutputImages/lacBlackAndWhite.bmp");
        }
    }
}

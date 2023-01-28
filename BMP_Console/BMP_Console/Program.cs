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
            Console.ReadKey();
        }

        static void TestGrayScale()
        {
            MyImage img = new MyImage("../../SampleImages/lac.bmp");

            MyImage imgGrey = img.ToGrayScale();

            imgGrey.Save("../../SampleImages/lacGreyscale.bmp");
        }

        static void TestBlackAndWhite()
        {
            MyImage Initial = new MyImage("../../SampleImages/lac.bmp");

            MyImage NoirEtBlanc = Initial.ToBlackAndWhite();

            NoirEtBlanc.Save("../../SampleImages/lacBlackAndWhite.bmp");
        }
    }
}

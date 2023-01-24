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
            TestGrayScale();   
        }

        static void TestGrayScale()
        {
            Image img = new Image("../../SampleImages/coco.bmp");

            Image imgGrey = img.ToGrayScale();

            imgGrey.Save("../../SampleImages/coco.bmp");
        }
    }
}

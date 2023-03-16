using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BMP_Console
{
    class FractalImage : MyImage
    {
        public FractalImage(int width, int height)
        {
            Width = width;
            Height = height;
        }

        int threshold = 2;

        public void Compute_Mandelbrot(int maxIterations, Pixel color)
        {
            for(int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    ComplexNumber c = new ComplexNumber(((i - 2000) / (double)1000), ((j - 2000) / (double)1000));
                    bool diverging = false;

                    ComplexNumber z = new ComplexNumber(0, 0);

                    int iterations = 0;
                    while(!diverging && iterations < maxIterations)
                    {
                        iterations++;

                        z = (z * z) + c;

                        if(z.abs > threshold)
                        {
                            diverging = true;
                        }
                    }

                    if (!diverging)
                    {
                        imagePixels[i, j] = color;
                    }
                }
            }
        }

        public void Compute_Julia(int maxIterations, Pixel color)
        {
            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    ComplexNumber z = new ComplexNumber(((i - 2000) / (double)1000), ((j - 2000) / (double)1000));
                    bool diverging = false;

                    ComplexNumber c = new ComplexNumber(0.3, 0.5);

                    int iterations = 0;
                    while (!diverging && iterations < maxIterations)
                    {
                        iterations++;

                        z = (z * z) + c;

                        if (z.abs > threshold)
                        {
                            diverging = true;
                        }
                    }

                    if(!diverging)
                    {
                        imagePixels[i, j] = color;
                    }
                }
            }
        }

        public void Julia()
        {
            Pixel[] colorScheme = new Pixel[] {new Pixel(254, 135, 135), new Pixel(250, 179, 64), new Pixel(109, 153, 162), new Pixel(2, 128, 143), new Pixel(18, 76, 96) };
            int[] detailLevels = new int[] { 2, 4, 8, 12, 255 };
            for(int i = 0; i < colorScheme.Length; i++)
            {
                var watch = System.Diagnostics.Stopwatch.StartNew();
                Compute_Julia(detailLevels[i], colorScheme[i]);

                watch.Stop();
                Console.WriteLine(watch.ElapsedMilliseconds);
            }
        }

        public void Mandelbrot()
        {
            Pixel[] colorScheme = new Pixel[] { new Pixel(255, 141, 112), new Pixel(252, 170, 130), new Pixel(115, 162, 172), new Pixel(11, 93, 105) };
            int[] detailLevels = new int[] { 2, 4, 14, 255 };
            for (int i = 0; i < colorScheme.Length; i++)
            {
                Console.WriteLine(i);
                var watch = System.Diagnostics.Stopwatch.StartNew();
                Compute_Mandelbrot(detailLevels[i], colorScheme[i]);

                watch.Stop();
                Console.WriteLine(watch.ElapsedMilliseconds);
            }
        }
    }
}

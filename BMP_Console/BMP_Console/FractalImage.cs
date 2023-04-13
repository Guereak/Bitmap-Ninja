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

        public void Compute_Mandelbrot(int maxIterations, PixelRGB color)
        {
            for(int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    ComplexNumber c = new ComplexNumber((i - 2000) / (double)1000, (j - 2000) / (double)1000);
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

        public void Compute_Julia(int maxIterations, PixelRGB color)
        {
            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    ComplexNumber z = new ComplexNumber(((i - 2000) / (double)1000), ((j - 2000) / (double)1000));
                    bool diverging = false;

                    ComplexNumber c = new ComplexNumber(-0.4, 0.6);

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
            PixelRGB[] colorScheme = new PixelRGB[] {new PixelRGB(254, 135, 135), new PixelRGB(250, 179, 64), new PixelRGB(109, 153, 162), new PixelRGB(2, 128, 143), new PixelRGB(18, 76, 96) };
            int[] detailLevels = new int[] { 2, 4, 8, 12, 100 };
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
            PixelRGB[] colorScheme = new PixelRGB[] { new PixelRGB(255, 141, 112), new PixelRGB(252, 170, 130), new PixelRGB(115, 162, 172), new PixelRGB(11, 93, 105) };
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

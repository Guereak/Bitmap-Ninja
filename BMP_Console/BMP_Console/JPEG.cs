using System;
using System.Collections.Generic;
using System.Linq;

namespace BMP_Console
{
    class JPEG
    {
        public static void FullJpegCompression(MyImage image)
        {
            // Step 1: Convert the image to YCbCr color space
            PixelYCbCr[,] yCbCrPixels = ColorSpaceConversion(image);

            // Step 2: Perform downsampling on the YCbCr image
            Downsampled downsampledImage = Downsampling(yCbCrPixels);

            // Step 3: Split the downsampled image into 8x8 blocks
            Blocks blocks = Split8x8(downsampledImage);

            // Step 4: Apply Discrete Cosine Transform (DCT) to each block
            for (int i = 0; i < blocks.blockY.GetLength(0); i++)
            {
                for (int j = 0; j < blocks.blockY.GetLength(1); j++)
                {
                    blocks.blockY = DiscreteCosineTransform(blocks.blockY);
                    blocks.blockCb = DiscreteCosineTransform(blocks.blockCb);
                    blocks.blockCr = DiscreteCosineTransform(blocks.blockCr);
                }
            }

            // Step 5: Quantize the DCT coefficients using the quantization matrix
            BlocksInt quantizedBlocks = Quantization(blocks);

            // Step 6: Perform ZigZag ordering and Run-Length Encoding (RLE) on the quantized coefficients
            int[] zigzagY = ZigZags(quantizedBlocks.blockY);
            int[] zigzagCb = ZigZags(quantizedBlocks.blockCb);
            int[] zigzagCr = ZigZags(quantizedBlocks.blockCr);

            string rleY = RLE(string.Join(", ", zigzagY));
            string rleCb = RLE(string.Join(", ", zigzagCb));
            string rleCr = RLE(string.Join(", ", zigzagCr));

            Console.WriteLine(rleY);
            Console.WriteLine("----------");
            Console.WriteLine(rleCb);
            Console.WriteLine("----------");
            Console.WriteLine(rleCr);

            // Step 7: Encode the RLE data using a suitable encoding algorithm (e.g. Huffman encoding)
            Noeud huffmanTreeY = Huffman.BuildHuffmanTree(rleY);
            Noeud huffmanTreeCb = Huffman.BuildHuffmanTree(rleCb);
            Noeud huffmanTreeCr = Huffman.BuildHuffmanTree(rleCr);

            Dictionary<char, string> huffmanCodesY = new Dictionary<char, string>();
            Dictionary<char, string> huffmanCodesCb = new Dictionary<char, string>();
            Dictionary<char, string> huffmanCodesCr = new Dictionary<char, string>();

            Huffman.GenerateHuffmanCodes(huffmanTreeY, huffmanCodesY, "");
            Huffman.GenerateHuffmanCodes(huffmanTreeCb, huffmanCodesCb, "");
            Huffman.GenerateHuffmanCodes(huffmanTreeCr, huffmanCodesCr, "");

            string encodedY = Huffman.HuffmanEncoding(rleY, huffmanCodesY);
            string encodedCb = Huffman.HuffmanEncoding(rleCb, huffmanCodesCb);
            string encodedCr = Huffman.HuffmanEncoding(rleCr, huffmanCodesCr);

            Console.WriteLine(encodedY);
            Console.WriteLine("----------");
            Console.WriteLine(encodedCb);
            Console.WriteLine("----------");
            Console.WriteLine(encodedCr);
        }



        public static double[,,,] DiscreteCosineTransform(double[,,,] imageMatrix)
        {
            double[,,,] returnTransform = new double[imageMatrix.GetLength(0), imageMatrix.GetLength(1), 8, 8];

            for (int i = 0; i < imageMatrix.GetLength(0); i++)
            {
                for (int j = 0; j < imageMatrix.GetLength(1); j++)
                {
                    for (int u = 0; u < 8; u++)
                    {
                        for (int v = 0; v < 8; v++)
                        {
                            double dct = 0;

                            for (int x = 0; x < 8; x++)
                            {
                                for (int y = 0; y < 8; y++)
                                {
                                    dct += imageMatrix[i, j, x, y] * Math.Cos(Math.PI / 8 * u * (x + 0.5)) * Math.Cos(Math.PI / 8 * v * (y + 0.5));
                                }
                            }

                            returnTransform[i, j, u, v] = dct * c(u) * c(v) * 0.25;
                        }
                    }
                }
            }

            return returnTransform;
        }

        static double c(int w)
        {
            return w == 0 ? (1 / Math.Sqrt(2)) : 1;
        }

        public static PixelYCbCr[,] ColorSpaceConversion(MyImage image)
        {
            PixelYCbCr[,] pixelsYCbCr = new PixelYCbCr[image.Width, image.Height];
            for (int i = 0; i < image.Height; i++)
            {
                for (int j = 0; j < image.Width; j++)
                {
                    pixelsYCbCr[j, i] = image.ImagePixels[j, i].ToYCbCr();
                }
            }

            return pixelsYCbCr;
        }

        public struct Downsampled
        {
            public int[,] Y;
            public int[,] Cb;
            public int[,] Cr;
        }

        public static Downsampled Downsampling(PixelYCbCr[,] pixels)
        {
            Downsampled d = new Downsampled();
            d.Y = new int[pixels.GetLength(0), pixels.GetLength(1)];
            d.Cb = new int[pixels.GetLength(0) / 2, pixels.GetLength(1) / 2];
            d.Cr = new int[pixels.GetLength(0) / 2, pixels.GetLength(1) / 2];


            for (int i = 0; i < pixels.GetLength(1); i += 2)
            {
                for (int j = 0; j < pixels.GetLength(0); j += 2)
                {
                    d.Y[j, i] = pixels[j, i].Y;
                    d.Y[j + 1, i] = pixels[j + 1, i].Y;
                    d.Y[j, i + 1] = pixels[j, i + 1].Y;
                    d.Y[j + 1, i + 1] = pixels[j + 1, i + 1].Y;

                    d.Cb[j / 2, i / 2] = (pixels[j, i].Cb + pixels[j + 1, i].Cb + pixels[j, i + 1].Cb + pixels[j + 1, i + 1].Cb) / 4;
                    d.Cr[j / 2, i / 2] = (pixels[j, i].Cr + pixels[j + 1, i].Cr + pixels[j, i + 1].Cr + pixels[j + 1, i + 1].Cr) / 4;
                }
            }

            return d;
        }

        public struct Blocks
        {
            public double[,,,] blockY;
            public double[,,,] blockCb;
            public double[,,,] blockCr;
        }

        public struct BlocksInt
        {
            public int[,,,] blockY;
            public int[,,,] blockCb;
            public int[,,,] blockCr;
        }

        public static Blocks Split8x8(Downsampled d)
        {
            Blocks b = new Blocks();
            int width = d.Y.GetLength(0);
            int height = d.Y.GetLength(1);

            b.blockY = new double[width % 8 == 0 ? width / 8 : width / 8 + 1, height % 8 == 0 ? height / 8 : height / 8 + 1, 8, 8];

            b.blockCb = new double[(width / 2) % 8 == 0 ? (width / 2) / 8 : (width / 2) / 8 + 1, (height / 2) % 8 == 0 ? (height / 2) / 8 : (height / 2) / 8 + 1, 8, 8];
            b.blockCr = new double[(width / 2) % 8 == 0 ? (width / 2) / 8 : (width / 2) / 8 + 1, (height / 2) % 8 == 0 ? (height / 2) / 8 : (height / 2) / 8 + 1, 8, 8];

            for (int i = 0; i < height / 8; i++)
            {
                for (int j = 0; j < width / 8; j++)
                {
                    for (int k = 0; k < 8; k++)
                    {
                        for (int l = 0; l < 8; l++)
                        {
                            b.blockY[j, i, k, l] = d.Y[j * 8 + k, i * 8 + l];
                        }
                    }
                }
            }

            for (int i = 0; i < height / 2 / 8; i++)
            {
                for (int j = 0; j < width / 2 / 8; j++)
                {
                    for (int k = 0; k < 8; k++)
                    {
                        for (int l = 0; l < 8; l++)
                        {
                            b.blockCb[j, i, k, l] = d.Cb[j * 8 + k, i * 8 + l];
                            b.blockCr[j, i, k, l] = d.Cr[j * 8 + k, i * 8 + l];
                        }
                    }
                }
            }

            //TODO: For the edge blocks

            return b;
        }

        public static int[,,,] Quantization_Process(double[,,,] blocks)
        {
            int[,,,] intblocks = new int[blocks.GetLength(0), blocks.GetLength(1), 8, 8];

            for (int i = 0; i < blocks.GetLength(0); i++)
            {
                for (int j = 0; j < blocks.GetLength(1); j++)
                {
                    for (int k = 0; k < 8; k++)
                    {
                        for (int l = 0; l < 8; l++)
                        {
                            intblocks[i, j, k, l] = (int)(blocks[i, j, k, l] / Quantification_Matrix[k, l]);
                        }
                    }
                }
            }

            return intblocks;
        }

        public static BlocksInt Quantization(Blocks b)
        {
            BlocksInt block = new BlocksInt();

            block.blockY = Quantization_Process(b.blockY);
            block.blockCb = Quantization_Process(b.blockCb);
            block.blockCr = Quantization_Process(b.blockCr);

            return block;
        }

        public static string RLE(string s)
        {
            int c = 1;
            string ret = "";

            for (int i = 0; i < s.Length - 1; i++)
            {
                if (s[i] == s[i + 1])
                {
                    c++;
                }
                else
                {
                    c = 1;
                    ret += c;
                    ret += s[i];
                }

            }

            return ret;
        }

        public static int[] ZigZags(int[,,,] matrix)
        {
            int[] zigzags = new int[0];

            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for(int j = 0; j < matrix.GetLength(1); j++)
                {
                    //Recontruct 2D matrix
                    int[,] zigzagmatrix = new int[8, 8];

                    for(int k = 0; k < 8; k++)
                    {
                        for(int l = 0; l < 8; l++)
                        {
                            zigzagmatrix[k, l] = matrix[i, j, k, l];
                        }
                    }

                    zigzags = zigzags.Concat(ZigZag(zigzagmatrix)).ToArray();
                }
            }

            return zigzags;
        }

        public static int[] ZigZag(int[,] matrix)
        {
            int size = matrix.GetLength(0) * matrix.GetLength(1);
            int[] result = new int[size];

            int i = 0;
            int j = 0;
            int k = 0;

            bool directionUp = true;

            while (k < 64)
            {
                result[k++] = matrix[i, j] < 0 ? 0 : matrix[i,j];

                if (directionUp)
                {
                    if (j == 0 || i == matrix.GetLength(0) - 1)
                    {
                        directionUp = false;
                        if (i == matrix.GetLength(0) - 1)
                            j++;
                        else
                            i++;
                    }
                    else
                    {
                        i++;
                        j--;
                    }
                }
                else
                {
                    if (i == 0 || j == matrix.GetLength(1) - 1)
                    {
                        directionUp = true;
                        if (j == matrix.GetLength(1) - 1)
                            i++;
                        else
                            j++;
                    }
                    else
                    {
                        i--;
                        j++;
                    }
                }
            }

            return result;

        }

        public static int[,] test = { { 140, 144, 147, 140, 140, 155, 179, 175 },
                                      {144, 152, 140, 147, 140, 148, 167, 179 },
                                      {152, 155, 136, 167, 163, 162, 152, 172},
                                      { 168, 145, 156, 160, 152, 155, 136, 160 },
                                      { 162, 148, 156, 148, 140, 136, 147, 162},
                                      { 147, 167, 140, 155, 155, 140, 136, 162},
                                      { 136, 156, 123, 167, 162, 144, 140, 147},
                                      { 148, 155, 136, 155, 152, 147, 147, 136} };

        static int[,] Quantification_Matrix = { {3, 5, 7, 9, 11, 13, 15, 17},
                                                {5, 7, 9, 11, 13, 15, 17, 19},
                                                {7, 9, 11, 13, 15, 17, 19, 21},
                                                {9, 11, 13, 15, 17, 19, 21, 23},
                                                {11, 13, 15, 17, 19, 21, 23, 25},
                                                {13, 15, 17, 19, 21, 23, 25, 27},
                                                {15, 17, 19, 21, 23, 25, 27, 29},
                                                {17, 19, 21, 23, 25, 27, 29, 31} };


        //For debug delete
        public static void DisplayMatrix(int[,] matrix)
        {
            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);
            int maxLen = 0;

            // Find the maximum length of any number in the matrix
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    int len = matrix[i, j].ToString().Length;
                    if (len > maxLen)
                    {
                        maxLen = len;
                    }
                }
            }

            // Display the matrix with aligned numbers
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    Console.Write(matrix[i, j].ToString().PadLeft(maxLen + 1));
                }
                Console.WriteLine();
            }
        }

        public static void Display4DMatrix(int[,,,] matrix)
        {
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for (int j = 0; j < matrix.GetLength(1); j++)
                {
                    for (int k = 0; k < matrix.GetLength(2); k++)
                    {
                        for (int l = 0; l < matrix.GetLength(3); l++)
                        {
                            Console.Write(matrix[i, j, k, l] + " ");
                        }
                        Console.WriteLine();
                    }
                    Console.WriteLine();
                }
                Console.WriteLine();
            }
        }
    }
}

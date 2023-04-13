using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BMP_App_WPF
{
    class Huffman
    {
        public static Dictionary<int, int[]> BinaryToHuffmanTable(byte[] treeInfo)
        {
            Dictionary<int, int[]> retrieved = new Dictionary<int, int[]>();

            int num = 0;

            for (int i = 0; i < 16; i++)
            {
                int numberOfValues = treeInfo[i];
                int[] ret = new int[numberOfValues];

                for (int j = 0; j < numberOfValues; j++)
                {
                    ret[j] = treeInfo[j + num + 16];
                }
                num += numberOfValues;

                retrieved.Add(i, ret);
            }

            return retrieved;
        }

        public static void GenerateHuffmanCodes(Noeud node, Dictionary<char, string> codes, string code)
        {
            if (node.isLeaf())
            {
                codes.Add(node.character, code);
                return;
            }

            GenerateHuffmanCodes(node.left, codes, code + "0");
            GenerateHuffmanCodes(node.right, codes, code + "1");
        }

        public static string HuffmanEncoding(string input, Dictionary<char, string> huffmanCodes)
        {
            string encoded = "";
            foreach (char c in input)
            {
                encoded += huffmanCodes[c];
            }
            return encoded;
        }
    }
}

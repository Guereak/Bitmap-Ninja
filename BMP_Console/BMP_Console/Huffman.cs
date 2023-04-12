using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BMP_Console
{
    class Huffman
    {
        Noeud root;

        Dictionary<int, int> dictionary = new Dictionary<int, int>();

        public static Dictionary<int, int[]> BinaryToHuffmanTable(byte[] treeInfo)
        {
            Dictionary<int, int[]> retrieved = new Dictionary<int, int[]>();

            int elapsed = 0;

            for (int i = 0; i < 16; i++)
            {
                int numberOfValues = treeInfo[i];

                int[] ret = new int[numberOfValues];

                for (int j = 0; j < numberOfValues; j++)
                {
                    ret[j] = treeInfo[j + elapsed + 16];
                }
                elapsed += numberOfValues;

                retrieved.Add(i, ret);
            }

            return retrieved;
        }


        //TODO
        public static void TableToTree(Dictionary<int, int[]> tableInfo)
        {
            //Dictionary<string, int>  

            foreach (KeyValuePair<int, int[]> pair in tableInfo)
            {
                
            }

        }


        //for testing remove
        public static void PrintDict(Dictionary<int, int[]> myDict)
        {
            foreach (KeyValuePair<int, int[]> pair in myDict)
            {
                Console.Write(pair.Key + ": [");

                for (int i = 0; i < pair.Value.Length; i++)
                {
                    Console.Write(pair.Value[i]);

                    if (i < pair.Value.Length - 1)
                    {
                        Console.Write(", ");
                    }
                }

                Console.WriteLine("]");
            }

        }
    }
}

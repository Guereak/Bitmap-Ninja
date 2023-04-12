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

        public static Noeud BuildHuffmanTree(string input)
        {
            // Calculate character frequencies
            Dictionary<char, int> frequencies = new Dictionary<char, int>();
            foreach (char c in input)
            {
                if (!frequencies.ContainsKey(c))
                {
                    frequencies.Add(c, 0);
                }
                frequencies[c]++;
            }

            // Initialize priority queue with leaf nodes
            List<Noeud> priorityQueue = new List<Noeud>();
            foreach (KeyValuePair<char, int> frequency in frequencies)
            {
                priorityQueue.Add(new Noeud(frequency.Key, frequency.Value));
            }

            // Build the Huffman tree
            while (priorityQueue.Count > 1)
            {
                priorityQueue = priorityQueue.OrderBy(x => x.Frequency).ToList();

                Noeud left = priorityQueue[0];
                Noeud right = priorityQueue[1];

                Noeud parent = new Noeud('\0', left.Frequency + right.Frequency, left, right);
                priorityQueue.Remove(left);
                priorityQueue.Remove(right);
                priorityQueue.Add(parent);
            }

            return priorityQueue[0];
        }

        public static void GenerateHuffmanCodes(Noeud node, Dictionary<char, string> codes, string code)
        {
            if (node.IsLeaf)
            {
                codes.Add(node.Character, code);
                return;
            }

            GenerateHuffmanCodes(node.Left, codes, code + "0");
            GenerateHuffmanCodes(node.Right, codes, code + "1");
        }

        public static string HuffmanEncoding(string input, Dictionary<char, string> huffmanCodes)
        {
            StringBuilder encoded = new StringBuilder();
            foreach (char c in input)
            {
                encoded.Append(huffmanCodes[c]);
            }
            return encoded.ToString();
        }
    }
}

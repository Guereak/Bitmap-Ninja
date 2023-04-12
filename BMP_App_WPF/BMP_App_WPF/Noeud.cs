using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BMP_App_WPF
{
    class Noeud
    {
        public char Character { get; set; }
        public int Frequency { get; set; }
        public Noeud Left { get; set; }
        public Noeud Right { get; set; }

        public bool IsLeaf => Left == null && Right == null;

        public Noeud(char character, int frequency, Noeud left = null, Noeud right = null)
        {
            Character = character;
            Frequency = frequency;
            Left = left;
            Right = right;
        }
    }
}

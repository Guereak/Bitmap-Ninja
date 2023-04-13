using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BMP_App_WPF
{
    class Noeud
    {
        public char character;
        public int frequency;
        public Noeud left;
        public Noeud right;

        public bool isLeaf()
        {
            return left == null && right == null;
        }

        public Noeud(char character, int frequency, Noeud left = null, Noeud right = null)
        {
            this.character = character;
            this.frequency = frequency;
            this.left = left;
            this.right = right;
        }
    }
}

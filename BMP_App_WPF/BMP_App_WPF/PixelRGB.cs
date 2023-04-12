using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BMP_App_WPF
{
    public class PixelRGB
    {
        //Can this be made into a struct?
        public byte red;
        public byte green;
        public byte blue;

        public PixelRGB() { }

        public PixelRGB(byte red, byte green, byte blue)
        {
            this.red = red;
            this.green = green;
            this.blue = blue;
        }
    }
}

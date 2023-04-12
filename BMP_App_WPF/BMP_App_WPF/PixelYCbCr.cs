using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BMP_App_WPF
{
    public class PixelYCbCr
    {
        public int Y;
        public int Cb;
        public int Cr;

        public PixelYCbCr() { }

        public PixelYCbCr(int Y, int Cb, int Cr)
        {
            this.Y = Y;
            this.Cb = Cb;
            this.Cr = Cr;
        }

        public PixelRGB toRGB()
        {
            PixelRGB p = new PixelRGB();

            Y += 128;
            Cb += 128;
            Cr += 128;

            p.red = (byte)(298.082 * Y / 256 + 408.583 * Cr / 256 - 222.921);
            p.green = (byte)(298.082 * Y / 256 - 100.291 * Cb / 256 - 208.12 * Cr / 256 + 135.576);
            p.blue = (byte)(298.082 * Y / 256 + 516.412 * Cb / 256 - 276.836);

            return p;
        }
    }
}

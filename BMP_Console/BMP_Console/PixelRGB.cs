namespace BMP_Console
{
    class PixelRGB
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

        public PixelYCbCr ToYCbCr()
        {
            PixelYCbCr p = new PixelYCbCr();

            p.Y = (int)(16 + 65.738 * red / 256 + 129.057 * green / 256 + 25.064 * blue / 256) - 128;
            p.Cb = (int)(128 - 37.945 * red / 256 - 74.494 * green / 256 + 112.439 * blue / 256) - 128;
            p.Cr = (int)(128+112.439 * red/256 - 94.154*green/256 - 18.285 * blue/256) - 128;

            return p;
        }
    }
}

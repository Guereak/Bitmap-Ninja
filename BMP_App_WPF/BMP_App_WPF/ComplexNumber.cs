using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BMP_App_WPF
{
    class ComplexNumber
    {
        public double im;
        public double re;

        public double abs;

        public ComplexNumber(double r, double i)
        {
            re = r;
            im = i;

            abs = Math.Sqrt(r * r + i * i);
        }

        public static ComplexNumber operator *(ComplexNumber c1, ComplexNumber c2)
            => new ComplexNumber(c1.re * c2.re - c1.im * c2.im, c1.re * c2.im + c1.im * c2.re);

        public static ComplexNumber operator +(ComplexNumber c1, ComplexNumber c2)
            => new ComplexNumber(c1.re + c2.re, c1.im + c2.im);
    }
}

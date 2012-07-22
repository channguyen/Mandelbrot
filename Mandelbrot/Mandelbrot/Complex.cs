using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mandelbrot {
    public class Complex {
        private double real;
        private double imaginary;

        public double Real {
            get {
                return real;
            }
            set {
                real = value;
            }
        }

        public double Imaginary {
            get {
                return imaginary;
            }
            set {
                imaginary = value;
            }
        }

        public double Modulus {
            get { 
                return System.Math.Sqrt(real * real + imaginary * imaginary); 
            }
        }


        public Complex(double real, double imaginary) {
            this.real = real;
            this.imaginary = imaginary;
        }

        public Complex(Complex rhs) {
            this.real = rhs.real;
            this.imaginary = rhs.imaginary;
        }

        public static Complex operator +(Complex c1, Complex c2) {
            return new Complex(c1.real + c2.real, c1.imaginary + c2.imaginary);
        }

        public static Complex operator -(Complex c1, Complex c2) {
            return new Complex(c1.real - c2.real, c1.imaginary - c2.imaginary);
        }

        public static Complex operator *(Complex c1, Complex c2) {
            return new Complex(c1.real * c2.real - c1.imaginary * c2.imaginary, c1.real * c2.imaginary + c1.imaginary * c2.real);
        }

        public static Complex operator /(Complex c1, Complex c2) {
            double value = c2.real * c2.real + c2.imaginary * c2.imaginary;
            return new Complex((c1.real * c2.real + c1.imaginary * c2.imaginary) / value, (c1.imaginary * c2.real - c1.real * c2.imaginary) / value);
        }

        public static Complex operator *(Complex rhs, double val) {
            Complex res = new Complex(0, 0);
            res.real = (rhs.real * val);
            res.imaginary = (rhs.imaginary * val);
            return res;
        }

        public static bool operator ==(Complex c1, Complex c2) {
            return (c1.real == c2.real && c1.imaginary == c2.imaginary);
        }

        public static Complex operator /(Complex rhs, double dval) {
            Complex res = new Complex(0, 0);
            res.real = rhs.real / dval;
            res.imaginary = rhs.imaginary / dval;
            return res;
        }

        public static bool operator !=(Complex c1, Complex c2) {
            return (c1.real != c2.real || c1.imaginary != c2.imaginary);
        }
        
        public static Complex operator -(Complex rhs) {
            rhs.real = -rhs.real;
            rhs.imaginary = -rhs.imaginary;
            return rhs;
        }

        public override string ToString() {
            return String.Format("{0} + {1}i", real, imaginary);
        }
    }
}

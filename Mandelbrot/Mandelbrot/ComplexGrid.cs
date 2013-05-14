using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

/*
 *   Ex:
 *        width  = 4.0
 *        height = 4.0
 *
 *        but we can have 512 x 512 (rows x columns)
 *        , and the more rows and columns we have
 *        the better the resolution is. The idea is
 *        we divide this small grid into rows and columns
 *        and examine each pixel in this rectangle
 *
 *                      4.0 x 4.0 grid is divided into 8 x 8

 *        ________________^________________
 *        ----------------- --------------- 
 *        |   |   |   |   |   |   |   |   | 
 *        --------------------------------- 
 *        |   |   |   |   |   |   |   |   |   
 *        --------------------------------- 
 *        |   |   |   |   |   |   |   |   |   
 *        --------------------------------- 
 *        |   |   |   |   |   |   |   |   |    
 *        --------------------------------- 
 *        |   |   |   |   |   |   |   |   |    
 *        --------------------------------- 
 *        |   |   |   |   |   |   |   |   | 
 *        --------------------------------- 
 *        |   |   |   |   |   |   |   |   |    
 *        --------------------------------- 
 *        |   |   |   |   |   |   |   |   | } dx = width / columns
 *        --------------------------------- 
 * 
 */
namespace Mandelbrot {
    public class ComplexGrid {
        /* 
         * Upper left corner of our image
         */
        private double xStart;
        private double yStart;

        /*
         * Rectangle of Mandelbrot
         */
        private double width;
        private double height;

        /*
         * The number of rows and columns within the Mandekbrot
         * rectangle
         */
        private int rows;
        private int cols;

        /* 
         * The actual data of each pixel (x, y)
         */
        int[,] data;
        
        /*
         * This is the upper bound of divergence testing
         */
        private int maxIterations;

        /*
         * This is upper bound for convergence testing
         */
        private double maxModulus;

        /*
         * The ratio in horizontal and vertical 
         */
        private double dx;
        private double dy;

        public ComplexGrid(double xs, double ys, double w, double h, int c, int r, int mi, double mm) {
            xStart = xs;
            yStart = ys;

            width = w;
            height = h;

            rows = r;
            cols = c;

            data = new int[rows, cols];

            maxIterations = mi;
            maxModulus = mm;

            dx = width / (cols - 1);
            dy = height / (rows - 1);
        }

        public int[,] Data {
            get {
                return data;
            } 
        }

        public Complex ToComplex(int row, int col) {
            return new Complex(yStart + col * dy, xStart + row * dx);
        }

        public void GenerateIterationCounts() {
            int value = 0;
            for (int x = 0; x < rows; ++x) {
                for (int y = 0; y < cols; ++y) {
                    Complex c = ToComplex(x, y);
                    value = IsMember(c);
                    data[x, y] = value;
                }
            }
        }

        private int IsMember(Complex c) {
            Complex z = new Complex(0.0, 0.0);
            int count = 0;
            while (true) {
                z = (z * z) + c;
                count++;
                if (z.Modulus >= maxModulus || count >= maxIterations) {
                    break;
                }
            }
            if (count == maxIterations) {
                return 0; // black
            }
            return count;
        }
    }
}

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
        private int maxIter;

        /*
         * This is upper bound for convergence testing
         */
        private double maxModulus;

        /*
         * The ratio in horizontal and vertical 
         */
        private double dx;
        private double dy;

        public ComplexGrid(double xStart, double yStart, double width, double height, int rows, int columns, int maxIteration, double maxModulus) {
            this.xStart = xStart;
            this.yStart = yStart;
            this.width = width;
            this.height = height;
            this.rows = rows;
            this.cols = columns;
            this.maxIter = maxIteration;
            this.maxModulus = maxModulus;
            this.dx = this.width / (cols - 1);
            this.dy = this.height / (rows - 1);
            this.data = new int[rows, cols];
        }

        public int[,] Data {
            get {
                return data;
            } 
        }

        public int MaxIteration {
            get {
                return maxIter;
            }
        }

        public int Rows {
            get {
                return rows;
            }
        }

        public int Columns {
            get {
                return cols;
            }
        }

        public double MaxModulus {
            get {
                return maxModulus;
            }
        }

        public Complex ToComplex(int row, int col) {
            return new Complex(xStart + col * dx, yStart + row * dy);
        }

        public void GenerateIterationCounts() {
            for (int x = 0; x < rows; ++x) {
                for (int y = 0; y < cols; ++y) {
                    Complex c = ToComplex(x, y);
                    data[x, y] = IsMember(c);
                }
            }
        }

        private int IsMember(Complex c) {
            Complex z = new Complex(0.0, 0.0);
            int count = 0;
            while (true) {
                z = (z * z) + c;
                count++;
                if (z.Modulus >= maxModulus || count >= maxIter) {
                    break;
                }
            }
            return count;
        }
    }
}

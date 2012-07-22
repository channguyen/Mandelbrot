//-----------------------------------------------
// MeetTheDockers.cs (c) 2006 by Charles Petzold
//-----------------------------------------------
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Collections.Generic;
using System.IO;
using Microsoft.Win32;

namespace Mandelbrot {
    public class Mandelbrot : Window {
        private const double WIDTH = 4.0;
        private const double HEIGHT = 4.0;

        private DockPanel dock;
        private TextBox widthTextBox;
        private TextBox heightTextBox;
        private Button generateButton;
        private Random random;
        private ComplexGrid complexGrid;
        private Image image;
        private SaveFileDialog saveFileDialog;
        private OpenFileDialog openFileDialog;
        private Canvas canvas;
        private BitmapSource bitmapSource;
        private bool dataGridFlag;

        [STAThread]
        public static void Main() {
            Application app = new Application();
            app.Run(new Mandelbrot());
        }

        public Mandelbrot() {
            Title = "Meet the Dockers";
            dock = new DockPanel();
            dock.Background = Brushes.YellowGreen;
            Content = dock;

            dataGridFlag = false;

            image = new Image();
            random = new Random();

            saveFileDialog = new SaveFileDialog();
            openFileDialog = new OpenFileDialog();

            BuildMenu();
            BuildGrid();
            BuildCanvas();
        }

        private void FillDataGrid(int row, int column) {
            complexGrid = new ComplexGrid(-2.0, -2.0, WIDTH, HEIGHT, row, column, 100, 2.0);
            complexGrid.GenerateIterationCounts();
            dataGridFlag = true;
        }

        private void BuildGrid() {
            // Create the grid.
            Grid grid = new Grid();

            DockPanel.SetDock(grid, Dock.Left);
            dock.Children.Add(grid);

            // Row and column definitions.
            for (int i = 0; i < 3; i++) {
                RowDefinition rowdef = new RowDefinition();
                rowdef.Height = GridLength.Auto;
                grid.RowDefinitions.Add(rowdef);
            }

            for (int i = 0; i < 2; i++) {
                ColumnDefinition coldef = new ColumnDefinition();
                coldef.Width = GridLength.Auto;
                grid.ColumnDefinitions.Add(coldef);
            }

            // [0, 0]
            Label lbl = new Label();
            lbl.Content = "Width: ";
            grid.Children.Add(lbl);
            Grid.SetRow(lbl, 0);
            Grid.SetColumn(lbl, 0);

            // [1, 0]
            lbl = new Label();
            lbl.Content = "Height: ";
            grid.Children.Add(lbl);
            Grid.SetRow(lbl, 1);
            Grid.SetColumn(lbl, 0);

            // [2, 0]
            generateButton = new Button();
            generateButton.Content = "Generate";
            generateButton.Click += GenerateButtonClick;
            grid.Children.Add(generateButton);
            Grid.SetRow(generateButton, 2);
            Grid.SetColumn(generateButton, 0);

            // [0, 1]
            widthTextBox = new TextBox();
            widthTextBox.Width = 100;
            grid.Children.Add(widthTextBox);
            Grid.SetRow(widthTextBox, 0);
            Grid.SetColumn(widthTextBox, 1);

            // [1, 1]
            heightTextBox = new TextBox();
            heightTextBox.Width = 100;
            grid.Children.Add(heightTextBox);
            Grid.SetRow(heightTextBox, 1);
            Grid.SetColumn(heightTextBox, 1);
        }

        private void SaveGridItemClick(object sender, RoutedEventArgs args) {
            if (!dataGridFlag) {
                MessageBox.Show("Data is not available yet");
            }
            else {
                saveFileDialog.ShowDialog();
                saveFileDialog.Filter = "Text file|*.txt";
                saveFileDialog.Title = "Save Grid as text file";
                saveFileDialog.ShowDialog();
                if (saveFileDialog.FileName != "") {
                    int[,] data = complexGrid.Data;
                    using (var stream = new StreamWriter(File.Create(saveFileDialog.FileName))) {

                        // write row 
                        stream.WriteLine(data.GetLength(0));

                        // write column
                        stream.WriteLine(data.GetLength(1));
                        
                        // write data
                        for (int x = 0; x < data.GetLength(0); ++x) {
                            for (int y = 0; y < data.GetLength(1); ++y) {
                                stream.Write(data[x, y]);
                                stream.Write(" ");
                            }
                            stream.WriteLine("");
                        }
                    }
                }
            }
        }

        private void SaveImageItemClick(object sender, RoutedEventArgs args) {
            saveFileDialog.ShowDialog();
            saveFileDialog.Filter = "JPeg Image|*.jpg|Bitmap Image|*.bmp|Gif Image|*.gif";
            saveFileDialog.Title = "Save an Image File";
            saveFileDialog.ShowDialog();
            if (saveFileDialog.FileName != "") {
                BitmapImage src = new BitmapImage();
                src.BeginInit();
                src.UriSource = new Uri(saveFileDialog.FileName, UriKind.Relative);
                src.CacheOption = BitmapCacheOption.OnLoad;
                src.EndInit();
                bitmapSource = src;
                image.Source = bitmapSource;
            }
        }

        private void LoadGridItemClick(object sender, RoutedEventArgs args) {
            openFileDialog.ShowDialog();
            openFileDialog.Filter = "Text File|*.text";
            openFileDialog.Title = "Load a grid text file";
            openFileDialog.ShowDialog();
            if (openFileDialog.FileName != "") {
                int row = 0;
                int column = 0;
                using (var stream = new StreamReader(openFileDialog.FileName)) {
                    // read row
                    row = int.Parse(stream.ReadLine());

                    // read column
                    column = int.Parse(stream.ReadLine());

                    // create a complex grid
                    complexGrid = new ComplexGrid(-2.0, -2.0, WIDTH, HEIGHT, row, column, 100, 2.0);

                    string s = "";
                    string[] split = null;

                    for (int i = 0; (s = stream.ReadLine()) != null; i++) {
                        split = s.Split(new char[] {' '}, StringSplitOptions.RemoveEmptyEntries);
                        for (int j = 0; j < column; j++) {
                            complexGrid.Data[i, j] = int.Parse(split[j]);
                        }
                    }
                }

                bitmapSource = GenerateImageSource(row, column);
                image.Source = bitmapSource;
                widthTextBox.Text = column.ToString();
                heightTextBox.Text = row.ToString();
            }
        }

        private void GenerateButtonClick(object sender, RoutedEventArgs args) {
            if (widthTextBox.Text == "" || heightTextBox.Text == "") {
                MessageBox.Show("Either width or height is empty");
            }
            else {
                int row = Int32.Parse(widthTextBox.Text.ToString());
                int column = Int32.Parse(widthTextBox.Text.ToString());
                FillDataGrid(row, column);
                AddBitmapSource(row, column); 
            }
        }

        private void BuildMenu() {
            // Create menu.
            Menu menu = new Menu();
            MenuItem item = new MenuItem();
            item.Header = "Menu";
            menu.Items.Add(item);

            MenuItem loadGridItem = new MenuItem();
            loadGridItem.Click += LoadGridItemClick; 
            loadGridItem.Header = "Load Grid";
            item.Items.Add(loadGridItem);

            MenuItem saveGridItem = new MenuItem();
            saveGridItem.Header = "Save Grid";
            saveGridItem.Click += SaveGridItemClick;
            item.Items.Add(saveGridItem);

            MenuItem generateImageItem = new MenuItem();
            generateImageItem.Click += GenerateButtonClick;
            generateImageItem.Header = "Generate Image";
            item.Items.Add(generateImageItem);

            MenuItem saveImageItem = new MenuItem();
            saveImageItem.Click += SaveImageItemClick;
            saveImageItem.Header = "Save Image";
            item.Items.Add(saveImageItem);

            // Dock menu at top of panel.
            DockPanel.SetDock(menu, Dock.Top);
            dock.Children.Add(menu);
        }

        private void AddBitmapSource(int row, int col) {
            bitmapSource = GenerateImageSource(row, col);
            image.Source = bitmapSource;
        }

        private BitmapSource GenerateImageSource(int row, int col) {
            ushort[] array = new ushort[row * col];
            int[,] data = complexGrid.Data;

            for (int x = 0; x < row; x++) {
                for (int y = 0; y < col; y++) {
                    // scale it
                    if (data[x, y] != 0)
                        array[x * col + y] = (ushort)(65535 - (ushort)data[x, y] - 20000);
                    else
                        array[x * col + y] = (ushort)data[x, y];
                }
            }

            int bitsPerPixel = 16;
            int stride = (row * bitsPerPixel + 7) / 8;

            // Create bitmap.
            return BitmapSource.Create(row, col, 96, 96, PixelFormats.Gray16, null, array, stride);
        }

        public void BuildCanvas() {
            // Create a canvas
            canvas = new Canvas();

            canvas.Children.Add(image);
            Canvas.SetTop(image, 10);
            Canvas.SetBottom(image, 10);

            DockPanel.SetDock(canvas, Dock.Left);
            dock.Children.Add(canvas);
        }
    }
}

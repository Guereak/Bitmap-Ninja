using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BMP_App_WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            DisplayImageGrid();    
        }

        void DisplayImageGrid()
        {
            //Import the Image
            //MyImage image = new MyImage("path");
            

            // Create the Grid    
            Grid DynamicGrid = new Grid();
            DynamicGrid.HorizontalAlignment = HorizontalAlignment.Center;
            DynamicGrid.VerticalAlignment = VerticalAlignment.Center;

            // Create Columns
            for (int i = 0; i < 10; i++)
            {
                ColumnDefinition gridCol = new ColumnDefinition();
                gridCol.Width = new GridLength(30);
                DynamicGrid.ColumnDefinitions.Add(gridCol);
            }

            // Create Rows
            for (int i = 0; i < 10; i++)
            {
                RowDefinition gridRow = new RowDefinition();
                gridRow.Height = new GridLength(30);
                DynamicGrid.RowDefinitions.Add(gridRow);
            }

            // Try to add pixels to the grid
            Random rnd = new Random();
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    Rectangle rect = new Rectangle();
                    rect.Fill = new SolidColorBrush(Color.FromRgb((byte)rnd.Next(256), (byte)rnd.Next(256), (byte)rnd.Next(256)));
                    Grid.SetRow(rect, j);
                    Grid.SetColumn(rect, i);
                    DynamicGrid.Children.Add(rect);
                }
            }

            // Display grid into a Window
            MainCanvas.Content = DynamicGrid;
        }

    }
}

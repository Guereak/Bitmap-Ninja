﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Diagnostics;
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
    /// Interaction logic for fractals.xaml
    /// </summary>
    public partial class Fractals : Page
    {
        private MainWindow _mainWindow;

        public Fractals(MainWindow mainWindow)
        {
            InitializeComponent();
            _mainWindow = mainWindow;
        }

        private void NumberInput_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !DoubleCharChecker(e.Text);
        }

        private bool DoubleCharChecker(string str)
        {
            foreach (char c in str)
            {
                if (c.Equals(','))
                    return true;
                if (c.Equals('-'))
                    return true;

                else if (char.IsNumber(c))
                    return true;
            }
            return false;
        }

        private void Julia_Click(object sender, RoutedEventArgs e)
        {
            double cr;
            double ci;

            if (Double.TryParse(JuliaTextBox1.Text, out cr) && Double.TryParse(JuliaTextBox2.Text, out ci))
            {
                MainWindow.displayedImage = new MyImage(4000, 4000);
                MainWindow.displayedImage.Julia(cr, ci);
                Trace.WriteLine("Fractal process ended");
                _mainWindow.RefreshDisplayedImage();
            }
            else
            {
                Trace.WriteLine("Invalid values");
                Trace.WriteLine(JuliaTextBox1.Text);
                Trace.WriteLine(JuliaTextBox2.Text);
            }
        }

        private void Mandelbrot_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.displayedImage = new MyImage(4000, 4000);
            MainWindow.displayedImage.Mandelbrot();
            _mainWindow.RefreshDisplayedImage();
        }
    }
}

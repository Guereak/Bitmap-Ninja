using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
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
    public partial class EditingPage : Page
    {
        private MainWindow _mainWindow;

        public EditingPage(MainWindow mainWindow)
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

                else if (char.IsNumber(c))
                    return true;
            }
            return false;
        }

        private void Greyscale_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.displayedImage = MainWindow.displayedImage.ToGrayScale();
            _mainWindow.RefreshDisplayedImage();
        }

        private void BlackAndWhite_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.displayedImage = MainWindow.displayedImage.ToBlackAndWhite();
            _mainWindow.RefreshDisplayedImage();
        }

        private void Negative_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.displayedImage = MainWindow.displayedImage.Negative();
            _mainWindow.RefreshDisplayedImage();
        }

        private void Rotation_Click(object sender, RoutedEventArgs e)
        {
            double rotationValue;
            if (Double.TryParse(RotationTextBox.Text, out rotationValue))
            {
                MainWindow.displayedImage = MainWindow.displayedImage.Rotate((float)rotationValue);
                _mainWindow.RefreshDisplayedImage();
            }
            else
            {
                Trace.WriteLine("Rotation could not occur. Invalid values");
            }

        }

        private void Agrandissement_Click(object sender, RoutedEventArgs e)
        {
            double agrandissementXValue;
            double agrandissementYValue;
            if (Double.TryParse(AgrandissementTextBox1.Text, out agrandissementXValue) && Double.TryParse(AgrandissementTextBox2.Text, out agrandissementYValue))
            {
                MainWindow.displayedImage = MainWindow.displayedImage.RescaleByFactor(agrandissementXValue, agrandissementYValue);
                _mainWindow.RefreshDisplayedImage();
            }
            else
            {
                Trace.WriteLine("Agrandissement could not occur. Invalid values");
            }
        }

        private void Horizontal_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.displayedImage.Mirror_Horizontal();
            _mainWindow.RefreshDisplayedImage();
        }

        private void Vertical_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.displayedImage.Mirror_Vertical();
            _mainWindow.RefreshDisplayedImage();
        }



        private void Convolution_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.displayedImage = MainWindow.displayedImage.Convolution11(MyImage.convolutions[comboBox.SelectedIndex]);
            _mainWindow.RefreshDisplayedImage();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.IO;
using Microsoft.Win32;
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
    /// Interaction logic for Page2.xaml
    /// </summary>
    public partial class SteganographyPage : Page
    {
        private MainWindow _mainWindow;

        public SteganographyPage(MainWindow mainWindow)
        {
            InitializeComponent();
            _mainWindow = mainWindow;
        }

        private void btnBrowse_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Bitmap files | *.bmp";

            if (openFileDialog.ShowDialog() == true)
            {
                txtFilePath.Text = openFileDialog.FileName;
            }
        }


        private void Restore_Click(object sender, RoutedEventArgs e)
        {
            int slVal = (int)slValue2.Value;

            MainWindow.displayedImage = Steganography.RestoreHidden(MainWindow.displayedImage, slVal);
            _mainWindow.RefreshDisplayedImage();
        }

        private void Hide_Click(object sender, RoutedEventArgs e)
        {
            int slVal = (int)slValue1.Value;
            try
            {
                MainWindow.displayedImage = Steganography.Hide(MainWindow.displayedImage, new MyImage(txtFilePath.Text), slVal);
                _mainWindow.RefreshDisplayedImage();
            }
            catch(Exception ex)
            {
                Trace.WriteLine(ex.Message);
            }
        }
    }
}

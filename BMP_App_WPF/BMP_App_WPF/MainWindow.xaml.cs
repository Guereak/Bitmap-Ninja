using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.IO;
using System.Windows.Controls;
using System.Diagnostics;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;

namespace BMP_App_WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static MyImage displayedImage;

        public MainWindow()
        {
            displayedImage = new MyImage("../../CurrentImage.bmp");

            Trace.WriteLine("TEST");

            InitializeComponent();
            RefreshDisplayImage();
            SideFrame.Navigate(new EditingPage(this));
        }

        private void OnPageSelected(object sender, SelectionChangedEventArgs e)
        {

            ListBoxItem item = (ListBoxItem)e.AddedItems[0];
            string pageName = (string)item.Tag;

            if(pageName == "editing.xaml")
            {
                SideFrame.Navigate(new EditingPage(this));
            }
            else if(pageName == "fractals.xaml")
            {
                SideFrame.Navigate(new Fractals(this));
            }
            else
            {
                SideFrame.Navigate(new SteganographyPage(this));
            }

        }

        public void RefreshDisplayImage()
        {
            try
            {
                using (var fileStream = new FileStream("../../CurrentImage.bmp", FileMode.Open, FileAccess.Read))
                {
                    BitmapImage bitmapImage = new BitmapImage();
                    bitmapImage.BeginInit();

                    bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapImage.StreamSource = fileStream;

                    bitmapImage.EndInit();

                    DisplayImage.Source = bitmapImage;
                    DisplayImage.InvalidateVisual();
                    DisplayImage.InvalidateArrange();
                    DisplayImage.InvalidateMeasure();
                }

                Trace.WriteLine("Visual invalidated");
            }
            catch (Exception ex)
            {
                Trace.WriteLine($"Error refreshing display image: {ex.Message}");
            }
        }



        private void OpenImage_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Bitmap files | *.bmp";

            if (openFileDialog.ShowDialog() == true)
            {
                string imagePath = openFileDialog.FileName;

                Trace.WriteLine("Opened");

                MyImage img = new MyImage(imagePath);
                img.From_Image_To_File("../../CurrentImage.bmp");

                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.None;
                bitmap.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                bitmap.UriSource = new Uri("../../CurrentImage.bmp", UriKind.Relative);
                bitmap.EndInit();

                Image image = new Image();
                DisplayImage.Source = bitmap;

            }
        }
    }
}

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
        public static int imageIndex = -1;

        public MainWindow()
        {
            Directory.Delete("../../Temp", true);
            Directory.CreateDirectory("../../Temp");

            InitializeComponent();

            displayedImage = new MyImage(200, 200);
            RefreshDisplayedImage();


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

        public void RefreshDisplayedImage()
        {
            imageIndex++;
            string tempImagePath = $"../../Temp/{imageIndex}.bmp";
            displayedImage.From_Image_To_File(tempImagePath);

            WidthHeight.Header = $"{displayedImage.Width}x{displayedImage.Height}";

            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
            bitmap.UriSource = new Uri(tempImagePath, UriKind.Relative);
            bitmap.EndInit();

            DisplayImage.Source = bitmap;

            DisplayImage.InvalidateVisual();
            DisplayImage.InvalidateArrange();
            DisplayImage.InvalidateMeasure();
        }


        //Modify later
        private void OpenImage_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Bitmap files | *.bmp";

            if (openFileDialog.ShowDialog() == true)
            {
                string imagePath = openFileDialog.FileName;

                Trace.WriteLine("Opened");

                displayedImage = new MyImage(imagePath);
                RefreshDisplayedImage();

            }
        }

        private void NewImage_Click(object sender, RoutedEventArgs e)
        {
            displayedImage = new MyImage(200, 200);
            RefreshDisplayedImage();
        }

        private void SaveImage_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Bitmap files |*.bmp; |All files (*.*)|*.*";

            if (saveFileDialog.ShowDialog() == true)
            {
                string fileName = saveFileDialog.FileName;
                displayedImage.From_Image_To_File(fileName);

                Trace.WriteLine(fileName);
            }
        }

        private void QuitApp_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Undo_Click(object sender, RoutedEventArgs e)
        {
            File.Delete($"../../Temp/{imageIndex}.bmp");

            imageIndex--;

            string tempImagePath = $"../../Temp/{imageIndex}.bmp";

            WidthHeight.Header = $"{displayedImage.Width}x{displayedImage.Height}";

            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
            bitmap.UriSource = new Uri(tempImagePath, UriKind.Relative);
            bitmap.EndInit();

            DisplayImage.Source = bitmap;

            DisplayImage.InvalidateVisual();
            DisplayImage.InvalidateArrange();
            DisplayImage.InvalidateMeasure();
        }
    }
}

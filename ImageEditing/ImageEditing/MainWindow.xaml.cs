using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using OxyPlot;

namespace ImageEditing
{
    public partial class Window1 : Window
    {
        bool obrazekZaladowany = false;
        int width = 512;
        int height = 512;
        int[] macierz = {0,0,0,0,1,0,0,0,0};
        int dzielnikFiltru = 1;
        string bufor;

        uint[] pixelsOriginal;
        uint[] pixelsCopy;
        uint[] pixelsBuffer;
        
        Histogram histOriginal;
        Histogram2 histCopy;

        WriteableBitmap bitmap2;

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog okienko = new OpenFileDialog();
            okienko.ShowDialog();
            okienko.Filter = "|*.bmp";
            string bufor = okienko.FileName;
            if (!bufor.Equals(""))
            wpiszDane(width, height, bufor);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            histCopy = new Histogram2(pixelsCopy);
            histCopy.obliczHistogram();
            PlotCopy.DataContext = histCopy;
        }

        private void wpiszDane(int width, int height, string bufor)
        {
            string path = @bufor;
            BinaryReader b = new BinaryReader(File.Open(path, FileMode.Open));
            byte[] buffer = new byte[width * height * 3 + 54];
            while (true)
            {
                int dataI = b.Read(buffer, 0, width * height * 3 + 54);
                if (dataI == 0)
                {
                    break;
                }
                //stream.Write(buffer, 0, dataI);
            }
            b.Close();

            // Create a writeable bitmap (which is a valid WPF Image Source
            WriteableBitmap bitmap = new WriteableBitmap(width, height, 96, 96, PixelFormats.Bgra32, null); // Create a writeable bitmap (which is a valid WPF Image Source
            bitmap2 = new WriteableBitmap(width, height, 96, 96, PixelFormats.Bgra32, null);

            pixelsOriginal = new uint[width * height];   // Create an array of pixelsOriginal to contain pixel color values
            pixelsCopy = new uint[width * height];
            pixelsBuffer = new uint[width * height];

            int red;
            int green;
            int blue;
            int alpha;

            for (int x = 0; x < width; ++x)
            {
                for (int y = 0; y < height; ++y)
                {
                    int i = width * y + x;
                    blue = buffer[54 + 3 * (x + (((-1) * width * y) + (width * (height - 1))))];
                    green = buffer[54 + 3 * (x + (((-1) * width * y) + (width * (height - 1)))) + 1];
                    red = buffer[54 + 3 * (x + (((-1) * width * y) + (width * (height - 1)))) + 2];
                    alpha = 255;
                    pixelsOriginal[i] = (uint)((alpha << 24) + (red << 16) + (green << 8) + blue);
                    pixelsCopy[i] = (uint)((alpha << 24) + (red << 16) + (green << 8) + blue);
                }
            }

            bitmap.WritePixels(new Int32Rect(0, 0, width, height), pixelsOriginal, width * 4, 0);    // apply pixelsOriginal to bitmap
            this.MainImage.Source = bitmap; // set image source to the new bitmap
            this.MainImage2.Source = bitmap2;

            changeBrightness(0);

            histOriginal = new Histogram(pixelsOriginal);
            histOriginal.obliczHistogram();
            PlotOriginal.DataContext = histOriginal;
            histCopy = new Histogram2(pixelsCopy);
            histCopy.obliczHistogram();
            PlotCopy.DataContext = histCopy;

            obrazekZaladowany = true;
        }

        public Window1()
        {
            InitializeComponent();
        }

        void useFilter() // 100 -> x2; -100 -> /2; 0 -> x1
        {
            int red=0;
            int green=0;
            int blue=0;
            int alpha=0;
            for (int x = 1; x < width-1; ++x)
            {
                for (int y = 1; y < height-1; ++y)
                {
                    int i = width * y + x;
                    red = 0;
                    green = 0;
                    blue = 0;
                    for (int j = 0; j < 3; j++)
                    {
                        for (int k = 0; k < 3; k++)
                        {
                            red += (int)((macierz[j + k * 3]) * (int)((pixelsCopy[i + (j - 1) + ((k - 1) * width)] >> 16) & 0x000000FF));
                            green += (int)((macierz[j + k * 3]) * (int)((pixelsCopy[i + (j - 1) + ((k - 1) * width)] >> 8) & 0x000000FF));
                            blue += (int)((macierz[j + k * 3]) * (int)((pixelsCopy[i + (j - 1) + ((k - 1) * width)]) & 0x000000FF));
                        }
                    }

                    red /= dzielnikFiltru;
                    green /= dzielnikFiltru;
                    blue /= dzielnikFiltru;
                    red = Math.Abs(red);
                    green = Math.Abs(green);
                    blue = Math.Abs(blue);

                    if (red > 255)
                        red = 255;

                    if (green > 255)
                        green = 255;

                    if (blue > 255)
                        blue = 255;

                    alpha = (int)((pixelsCopy[i] >> 24) & 0x000000FF);

                    pixelsBuffer[i] = (uint)((alpha << 24) + (red << 16) + (green << 8) + blue);
                }
            }
            for (int x = 1; x < width - 1; ++x)
            {
                for (int y = 1; y < height - 1; ++y)
                {
                    int i = width * y + x;
                    pixelsCopy[i] = pixelsBuffer[i];
                }
            }
            bitmap2.WritePixels(new Int32Rect(0, 0, width, height), pixelsCopy, width * 4, 0);
        }

        void changeBrightness(int change)
        {
            int red;
            int green;
            int blue;
            int alpha;

            for (int x = 0; x < width; ++x)
            {
                for (int y = 0; y < height; ++y)
                {
                    int i = width * y + x;
                    red = (int)((pixelsCopy[i]>>16) & 0x000000FF ) + change;
                    if (red > 255)
                        red = 255;
                    else if (red < 0)
                        red = 0;
                    green = (int)((pixelsCopy[i] >> 8) & 0x000000FF) + change;
                    if (green > 255)
                        green = 255;
                    else if (green < 0)
                        green = 0;
                    blue = (int)(pixelsCopy[i] & 0x000000FF) + change;
                    if (blue > 255)
                        blue = 255;
                    else if (blue < 0)
                        blue = 0;
                    alpha = (int)((pixelsCopy[i] >> 24) & 0x000000FF);

                    pixelsCopy[i] = (uint)((alpha << 24) + (red << 16) + (green << 8) + blue);
                }
            }
            bitmap2.WritePixels(new Int32Rect(0, 0, width, height), pixelsCopy, width * 4, 0);
        }

        void changeContrast(int change) // 100 -> x2; -100 -> /2; 0 -> x1
        {
            int red;
            int green;
            int blue;
            int alpha;

            for (int x = 0; x < width; ++x)
            {
                for (int y = 0; y < height; ++y)
                {
                    int i = width * y + x;
                    red = (int)((pixelsCopy[i] >> 16) & 0x000000FF);
                    red = (int)((red - 128)*((change+100)/100.0) + 128);
                    if (red > 255)
                        red = 255;
                    else if (red < 0)
                        red = 0;
                    green = (int)((pixelsCopy[i] >> 8) & 0x000000FF);
                    green = (int)((green - 128) * ((change + 100.0) / 100.0) + 128);
                    if (green > 255)
                        green = 255;
                    else if (green < 0)
                        green = 0;
                    blue = (int)(pixelsCopy[i] & 0x000000FF);
                    blue = (int)((blue - 128) * ((change + 100.0) / 100.0) + 128);
                    if (blue > 255)
                        blue = 255;
                    else if (blue < 0)
                        blue = 0;
                    alpha = (int)((pixelsCopy[i] >> 24) & 0x000000FF);

                    pixelsCopy[i] = (uint)((alpha << 24) + (red << 16) + (green << 8) + blue);
                }
            }
            bitmap2.WritePixels(new Int32Rect(0, 0, width, height), pixelsCopy, width * 4, 0);
        }

        void negatyw()
        {
            int red;
            int green;
            int blue;
            int alpha;
            for (int x = 0; x < width; ++x)
            {
                for (int y = 0; y < height; ++y)
                {
                    int i = width * y + x;
                    red = (int)((pixelsCopy[i] >> 16) & 0x000000FF);
                    red = (255 - red);
                    green = (int)((pixelsCopy[i] >> 8) & 0x000000FF);
                    green = (255 - green);
                    blue = (int)(pixelsCopy[i] & 0x000000FF);
                    blue = (255 - blue);
                    alpha = (int)((pixelsCopy[i] >> 24) & 0x000000FF);
                    pixelsCopy[i] = (uint)((alpha << 24) + (red << 16) + (green << 8) + blue);
                }
            }
            bitmap2.WritePixels(new Int32Rect(0, 0, width, height), pixelsCopy, width * 4, 0);
        }

        void gestoscPrawdH1()
        {
            int red;
            int green;
            int blue;
            int alpha;
            for (int x = 0; x < width; ++x)
            {
                for (int y = 0; y < height; ++y)
                {
                    int i = width * y + x;

                    int gmin = 0, gmax = 50, newValue = 0;
                    for (int j = 0; j < (red = (int)((pixelsCopy[i] >> 16) & 0x000000FF)); j++)
                        newValue += histCopy.wykresR[j];
                    if (red != 0)
                        red = (newValue / red) * (gmax - gmin) + gmin;
                    newValue = 0;
                    if (red > 255)
                        red = 255;
                    else if (red < 0)
                        red = 0;

                    for (int j = 0; j < (green = (int)((pixelsCopy[i] >> 16) & 0x000000FF)); j++)
                        newValue += histCopy.wykresR[j];
                    if (green != 0)
                        green = (newValue / green) * (gmax - gmin) + gmin;
                    newValue = 0;
                    if (green > 255)
                        green = 255;
                    else if (green < 0)
                        green = 0;

                    for (int j = 0; j < (blue = (int)((pixelsCopy[i] >> 16) & 0x000000FF)); j++)
                        newValue += histCopy.wykresR[j];
                    if (blue != 0)
                        blue = (newValue / blue) * (gmax - gmin) + gmin;
                    newValue = 0;
                    if (blue > 255)
                        blue = 255;
                    else if (blue < 0)
                        blue = 0;

                    alpha = (int)((pixelsCopy[i] >> 24) & 0x000000FF);
                    pixelsCopy[i] = (uint)((alpha << 24) + (red << 16) + (green << 8) + blue);
                }
            }
            bitmap2.WritePixels(new Int32Rect(0, 0, width, height), pixelsCopy, width * 4, 0);
        }

        void imageChanged()
        {
            for (int i = 0; i < pixelsOriginal.Length - 1; i++)
                pixelsCopy[i] = pixelsOriginal[i];
            useFilter();
            changeBrightness((int)slider.Value);
            changeContrast((int)slider_2.Value);
            if (negatyw_checkBox.IsChecked == true)
                negatyw();
            if (gestosc_checkBox.IsChecked == true)
                gestoscPrawdH1();
            histCopy = new Histogram2(pixelsCopy);
            histCopy.obliczHistogram();
            PlotCopy.DataContext = histCopy;
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (obrazekZaladowany)
            {
                for (int i = 0; i < pixelsOriginal.Length - 1; i++)
                    pixelsCopy[i] = pixelsOriginal[i];
                imageChanged();  //changeBrightness((int)slider.Value);
            }
        }

        private void Slider_ValueChanged_2(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (obrazekZaladowany)
            {
                for (int i = 0; i < pixelsOriginal.Length - 1; i++)
                    pixelsCopy[i] = pixelsOriginal[i];
                imageChanged();  //changeBrightness((int)slider.Value);
            }
        }

        private void TextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if ( obrazekZaladowany && Int32.TryParse(macierz_0.Text, out macierz[0]))
            {
                for (int i = 0; i < pixelsOriginal.Length - 1; i++)
                    pixelsCopy[i] = pixelsOriginal[i];
                imageChanged();  //useFilter();
            }
        }

        private void TextBox_TextChanged_1(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (obrazekZaladowany && Int32.TryParse(macierz_1.Text, out macierz[1]))
            {
                for (int i = 0; i < pixelsOriginal.Length - 1; i++)
                    pixelsCopy[i] = pixelsOriginal[i];
                imageChanged();  //useFilter();
            }
        }

        private void TextBox_TextChanged_2(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (obrazekZaladowany && Int32.TryParse(macierz_2.Text, out macierz[2]))
            {
                for (int i = 0; i < pixelsOriginal.Length - 1; i++)
                    pixelsCopy[i] = pixelsOriginal[i];
                imageChanged();  //useFilter();
            }
        }

        private void TextBox_TextChanged_3(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (obrazekZaladowany && Int32.TryParse(macierz_3.Text, out macierz[3]))
            {
                for (int i = 0; i < pixelsOriginal.Length - 1; i++)
                    pixelsCopy[i] = pixelsOriginal[i];
                imageChanged();  //useFilter();
            }
        }

        private void TextBox_TextChanged_4(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (obrazekZaladowany && Int32.TryParse(macierz_4.Text, out macierz[4]))
            {
                for (int i = 0; i < pixelsOriginal.Length - 1; i++)
                    pixelsCopy[i] = pixelsOriginal[i];
                imageChanged();  //useFilter();
            }
        }

        private void TextBox_TextChanged_5(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (obrazekZaladowany && Int32.TryParse(macierz_5.Text, out macierz[5]))
            {
                for (int i = 0; i < pixelsOriginal.Length - 1; i++)
                    pixelsCopy[i] = pixelsOriginal[i];
                imageChanged();  //useFilter();
            }
        }

        private void TextBox_TextChanged_6(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (obrazekZaladowany && Int32.TryParse(macierz_6.Text, out macierz[6]))
            {
                for (int i = 0; i < pixelsOriginal.Length - 1; i++)
                    pixelsCopy[i] = pixelsOriginal[i];
                imageChanged();  //useFilter();
            }
        }

        private void TextBox_TextChanged_7(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (obrazekZaladowany && Int32.TryParse(macierz_7.Text, out macierz[7]))
            {
                for (int i = 0; i < pixelsOriginal.Length - 1; i++)
                    pixelsCopy[i] = pixelsOriginal[i];
                imageChanged();  //useFilter();
            }
        }

        private void TextBox_TextChanged_8(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (obrazekZaladowany && Int32.TryParse(macierz_8.Text, out macierz[8]))
            {
                for (int i = 0; i < pixelsOriginal.Length - 1; i++)
                    pixelsCopy[i] = pixelsOriginal[i];
                imageChanged();  //useFilter();
            }
        }

        private void TextBox_TextChanged_9(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            int buf = dzielnikFiltru;
            if (obrazekZaladowany && Int32.TryParse(dzielnikFiltru_textBox.Text, out dzielnikFiltru))
            {
                if (dzielnikFiltru != 0)
                {
                    for (int i = 0; i < pixelsOriginal.Length - 1; i++)
                        pixelsCopy[i] = pixelsOriginal[i];
                    imageChanged();  //useFilter();
                }
                else
                    dzielnikFiltru = buf;
            }
        }

        private void CheckBox_Clicked(object sender, RoutedEventArgs e)
        {
            if (obrazekZaladowany)
            {
                for (int i = 0; i < pixelsOriginal.Length - 1; i++)
                    pixelsCopy[i] = pixelsOriginal[i];
                imageChanged();
            }
        }

        private void Gestosc_checkBox_Clicked(object sender, RoutedEventArgs e)
        {
            if (obrazekZaladowany)
            {
                for (int i = 0; i < pixelsOriginal.Length - 1; i++)
                    pixelsCopy[i] = pixelsOriginal[i];
                imageChanged();
            }
        }
    }
}
using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ImageEditing
{
    public partial class Window1 : Window
    {
        int width = 512;
        int height = 512;
        int[] macierz = new int[9];
        string bufor;

        uint[] pixelsOriginal;
        uint[] pixelsCopy;
        uint[] pixelsBuffer;

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

         /*  macierz[0]= {-1, -1, -1, -1, 2, -1, -1, -1, -1 };
            macierz[0] = -1;
            macierz[1] = -1;
            macierz[2] = -1;
            macierz[3] = -1;
            macierz[4] = 0;
            macierz[5] = -1;
            macierz[6] = -1;
            macierz[7] = -1;
            macierz[8] = -1;*/

         //   changeBrightness(2);
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
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////wczytywanie z pliku

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

            //changeBrightness(50);

            //changeBrightness(0);

            //changeContrast(100);// (-99999) - (-101) => negatyw; (-100) => szary full; (-99) - (-1) => zmniejszenie kontrastu; 0 => brak efektu; 1-99999 => zwiekszenie kontrastu

           // int[] filter = { -1, -1, -1, -1, 8, -1, -1, -1, -1 };
           // useFilter(filter);
            changeBrightness(0);


        }

        public Window1()
        {
            InitializeComponent();

            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////wczytywanie z pliku
           
        }

        void useFilter(int[] filter) // 100 -> x2; -100 -> /2; 0 -> x1
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
                            red += (int)((filter[j + k * 3]) * (int)((pixelsCopy[i + (j - 1) + ((k - 1) * width)] >> 16) & 0x000000FF));
                            green += (int)((filter[j + k * 3]) * (int)((pixelsCopy[i + (j - 1) + ((k - 1) * width)] >> 8) & 0x000000FF));
                            blue += (int)((filter[j + k * 3]) * (int)((pixelsCopy[i + (j - 1) + ((k - 1) * width)]) & 0x000000FF));
                        }
                    }

                    red /= 8;
                    green /= 8;
                    blue /= 8;
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
        


        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            for (int i=0;i<pixelsOriginal.Length-1;i++)
            {
                pixelsCopy[i] = pixelsOriginal[i];
            }
            changeBrightness((int)slider.Value);

        }
    }
}
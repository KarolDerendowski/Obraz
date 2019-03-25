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

        uint[] pixels;
        uint[] pixels2;
        uint[] pixelsbuf;

        WriteableBitmap bitmap2;

        public Window1()
        {
            InitializeComponent();

            int width = 512;
            int height = 512;

            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////wczytywanie z pliku
            string path = @"D:\PŁ\Zajęcia\VIII sem (inf)\Przetwarzanie obrazow\ImageEditing\lenac.bmp";
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
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////wczytywanie z pliku

            // Create a writeable bitmap (which is a valid WPF Image Source
            WriteableBitmap bitmap = new WriteableBitmap(width, height, 96, 96, PixelFormats.Bgra32, null); // Create a writeable bitmap (which is a valid WPF Image Source
            bitmap2 = new WriteableBitmap(width, height, 96, 96, PixelFormats.Bgra32, null);

            pixels = new uint[width * height];   // Create an array of pixels to contain pixel color values
            pixels2 = new uint[width * height];
            pixelsbuf = new uint[width * height];

            int red;
            int green;
            int blue;
            int alpha;

            for (int x = 0; x < width; ++x)
            {
                for (int y = 0; y < height; ++y)
                {
                    int i = width * y + x;
                    blue =  buffer[54 + 3*(x+(((-1)* width * y)+(width*(height-1))) )];
                    green =  buffer[54 + 3 * (x + (((-1) * width * y) + (width * (height - 1)))) + 1];
                    red = buffer[54 + 3 * (x + (((-1) * width * y) + (width * (height - 1)))) + 2];
                    alpha = 255;
                    pixels[i] = (uint)((alpha << 24) + (red << 16) + (green << 8) + blue);
                    pixels2[i] = (uint)((alpha << 24) + (red << 16) + (green << 8) + blue);
                }
            }

            bitmap.WritePixels(new Int32Rect(0, 0, width, height), pixels, width * 4, 0);    // apply pixels to bitmap
            this.MainImage.Source = bitmap; // set image source to the new bitmap
            this.MainImage2.Source = bitmap2;

            //changeBrightness(50);

            //changeBrightness(0);

            //changeContrast(100);// (-99999) - (-101) => negatyw; (-100) => szary full; (-99) - (-1) => zmniejszenie kontrastu; 0 => brak efektu; 1-99999 => zwiekszenie kontrastu

            int[] filter = {-1,-1,-1,-1,8,-1,-1,-1,-1};
            useFilter(filter);

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
                    for (int j = 0; j < 2; j++)
                    {
                        for (int k = 0; k < 2; k++)
                        {
                            red += (int)((filter[j + k * 3]) * (int)((pixels2[i + (j - 1) + ((k - 1) * width)] >> 16) & 0x000000FF));
                            green += (int)((filter[j + k * 3]) * (int)((pixels2[i + (j - 1) + ((k - 1) * width)] >> 8) & 0x000000FF));
                            blue += (int)((filter[j + k * 3]) * (int)((pixels2[i + (j - 1) + ((k - 1) * width)]) & 0x000000FF));
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

                    alpha = (int)((pixels2[i] >> 24) & 0x000000FF);

                    pixelsbuf[i] = (uint)((alpha << 24) + (red << 16) + (green << 8) + blue);
                }
            }

            for (int x = 1; x < width - 1; ++x)
            {
                for (int y = 1; y < height - 1; ++y)
                {
                    int i = width * y + x;
                    pixels2[i] = pixelsbuf[i];
                }
            }

            bitmap2.WritePixels(new Int32Rect(0, 0, width, height), pixels2, width * 4, 0);
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
                    red = (int)((pixels2[i]>>16) & 0x000000FF ) + change;
                    if (red > 255)
                        red = 255;
                    green = (int)((pixels2[i] >> 8) & 0x000000FF) + change;
                    if (green > 255)
                        green = 255;
                    blue = (int)(pixels2[i] & 0x000000FF) + change;
                    if (blue > 255)
                        blue = 255;
                    alpha = (int)((pixels2[i] >> 24) & 0x000000FF);

                    pixels2[i] = (uint)((alpha << 24) + (red << 16) + (green << 8) + blue);
                }
            }
            bitmap2.WritePixels(new Int32Rect(0, 0, width, height), pixels2, width * 4, 0);
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
                    red = (int)((pixels2[i] >> 16) & 0x000000FF);
                    red = (int)((red - 128)*((change+100)/100.0) + 128);
                    if (red > 255)
                        red = 255;
                    else if (red < 0)
                        red = 0;
                    green = (int)((pixels2[i] >> 8) & 0x000000FF);
                    green = (int)((green - 128) * ((change + 100.0) / 100.0) + 128);
                    if (green > 255)
                        green = 255;
                    else if (green < 0)
                        green = 0;
                    blue = (int)(pixels2[i] & 0x000000FF);
                    blue = (int)((blue - 128) * ((change + 100.0) / 100.0) + 128);
                    if (blue > 255)
                        blue = 255;
                    else if (blue < 0)
                        blue = 0;
                    alpha = (int)((pixels2[i] >> 24) & 0x000000FF);

                    pixels2[i] = (uint)((alpha << 24) + (red << 16) + (green << 8) + blue);
                }
            }
            bitmap2.WritePixels(new Int32Rect(0, 0, width, height), pixels2, width * 4, 0);
        }
    }
}
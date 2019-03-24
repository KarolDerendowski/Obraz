using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ImageEditing
{
    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();

            int width = 512;
            int height = 512;
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            string path = @"D:\PŁ\Zajęcia\VIII sem (inf)\Przetwarzanie obrazow\ImageEditing\lenac.bmp";
            BinaryReader b = new BinaryReader(File.Open(path, FileMode.Open));
            byte[] buffer = new byte[width * height * 3];
            while (true)
            {
                int dataI = b.Read(buffer, 0, width * height * 3);
                if (dataI == 0)
                {
                    break;
                }
                //stream.Write(buffer, 0, dataI);
            }
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            // Create a writeable bitmap (which is a valid WPF Image Source
            WriteableBitmap bitmap = new WriteableBitmap(width, height, 96, 96, PixelFormats.Bgra32, null);

            // Create an array of pixels to contain pixel color values
            uint[] pixels = new uint[width * height];

            int red;
            int green;
            int blue;
            int alpha;

            for (int x = 0; x < width; ++x)
            {
                for (int y = 0; y < height; ++y)
                {
                    int i = width * y + x;
                    red = buffer[3 * i];
                    green = buffer[3 * i + 1];
                    blue = buffer[3 * i + 2];
                    alpha = 255;
                    /* ( y % 8 == 0 || (y-1) % 8 == 0 || (y-2) % 8 == 0 || (y-3) % 8 == 0 )
                    {
                        red = 0;
                        green = 255 * y / height;
                        blue = 255 * (width - x) / width;
                        alpha = 255;
                    }
                    else
                    {
                        red = 0;
                        green = 0;
                        blue = 0;
                        alpha = 255;
                    }*/
                    pixels[i] = (uint)((alpha << 24) + (red << 16) + (green << 8) + blue);
                }
            }


            addBrightness(pixels, width, height, 0);

            // apply pixels to bitmap
            bitmap.WritePixels(new Int32Rect(0, 0, 512, 512), pixels, width * 4, 0);

            // set image source to the new bitmap
            this.MainImage.Source = bitmap;
        }

        void addBrightness(uint[] pixels, int width, int height, int change)
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
                    red = (int)((pixels[i]>>16) & 0x000000FF ) + change;
                    if (red > 255)
                        red = 255;
                    green = (int)((pixels[i] >> 8) & 0x000000FF) + change;
                    if (green > 255)
                        green = 255;
                    blue = (int)(pixels[i] & 0x000000FF) + change;
                    if (blue > 255)
                        blue = 255;
                    alpha = (int)((pixels[i] >> 24) & 0x000000FF);

                    pixels[i] = (uint)((alpha << 24) + (red << 16) + (green << 8) + blue);
                }
            }
        }
    }
}
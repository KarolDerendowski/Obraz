using System.IO;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ImageEditing
{
    public partial class Window1 : Window
    {
        int width=512, height=512;

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog okienko = new OpenFileDialog();
            okienko.Filter = "|*.bmp";
            okienko.ShowDialog();
            string bufor = okienko.FileName;
            wpiszDane(width,height, okienko.FileName);
            System.Windows.Forms.MessageBox.Show("Wybrano plik: " + okienko.SafeFileName);
        }

       private void Button_Click_2(object sender, RoutedEventArgs e) 
        {
            uint[] pixels = new uint[width * height];
            addBrightness(pixels, width, height, 0);
        }

        private void wpiszDane(int width, int height, string bufor)
        { 
            string  path = bufor;
            BinaryReader b = new BinaryReader(File.Open(path, FileMode.Open));
            {

            }
            
            byte[] buffer = new byte[width * height * 3];
            while (true)
            {
                int dataI = b.Read(buffer, 0, width * height * 3);
                if (dataI == 0)
                {
                    break;
                }
            }
            WriteableBitmap bitmap = new WriteableBitmap(width, height, 96, 96, PixelFormats.Bgra32, null);

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
                    blue = buffer[3 * ((-1) * (i - x) + ((width - 1) * height)) + (3 * x)];
                    green = buffer[3 * ((-1) * (i - x) + ((width - 1) * height)) + 1 + (3 * x)];
                    red = buffer[3 * ((-1) * (i - x) + ((width - 1) * height)) + 2 + (3 * x)];
                    alpha = 255;

                    pixels[i] = (uint)((alpha << 24) + (red << 16) + (green << 8) + blue);
                }
            }
            bitmap.WritePixels(new Int32Rect(0, 0, 512, 512), pixels, width * 4, 0);
            this.MainImage.Source = bitmap;

        }
        void pixels()
        {

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
                    red = (int)((pixels[i] >> 16) & 0x000000FF) + change;
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


        public Window1()
        {
            InitializeComponent();

        }


    }
}
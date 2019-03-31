using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OxyPlot;

namespace ImageEditing
{
    class Histogram
    {

        public int[] wykresR = new int[256];
        public int[] wykresG = new int[256];
        public int[] wykresB = new int[256];
        public uint[] obrazPiksele;
        public int maxWartosc = 0;

        public Histogram(uint[] obrazPiksele)
        {
            this.obrazPiksele = obrazPiksele;
            this.Title = "Window1";
            this.Points = new List<DataPoint>
            {
                new DataPoint(0, 4),
                new DataPoint(10, 13),
                new DataPoint(20, 15),
                new DataPoint(30, 16),
                new DataPoint(40, 12),
                new DataPoint(50, 12)
            };
        }

        public string Title { get; private set; }

        public IList<DataPoint> Points { get; private set; }

        public void obliczHistogram()
        {
            for (int i = 0; i < 256; i++)
            {
                wykresR[i] = 0;
                wykresG[i] = 0;
                wykresB[i] = 0;
            }
            for (int i = 0; i < obrazPiksele.Length; i++ )
            {
                wykresR[((obrazPiksele[i] >> 16) & 0x000000FF)]++;
                wykresG[((obrazPiksele[i] >> 8) & 0x000000FF)]++;
                wykresB[(obrazPiksele[i] & 0x000000FF)]++;
            }
        }
    }
}

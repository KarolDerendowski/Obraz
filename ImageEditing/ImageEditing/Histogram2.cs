using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OxyPlot;

namespace ImageEditing
{
    class Histogram2
    {
        public int[] wykresR = new int[256];
        public int[] wykresG = new int[256];
        public int[] wykresB = new int[256];
        public int[] wykresX = new int[256];
        public uint[] obrazPiksele;

        public Histogram2(uint[] obrazPiksele)
        {
            this.obrazPiksele = obrazPiksele;
            this.Points5 = new List<DataPoint> { };
            this.Points6 = new List<DataPoint> { };
            this.Points7 = new List<DataPoint> { };
            this.Points8 = new List<DataPoint> { };
        }

        public string Title { get; private set; }

        public IList<DataPoint> Points5 { get; private set; }
        public IList<DataPoint> Points6 { get; private set; }
        public IList<DataPoint> Points7 { get; private set; }
        public IList<DataPoint> Points8 { get; private set; }

        public void obliczHistogram()
        {
            Points5.Clear();
            Points6.Clear();
            Points7.Clear();
            Points8.Clear();
            for (int i = 0; i < 256; i++)
            {
                wykresR[i] = 0;
                wykresG[i] = 0;
                wykresB[i] = 0;
                wykresX[i] = 0;
            }
            for (int i = 0; i < obrazPiksele.Length; i++)
            {
                wykresR[((obrazPiksele[i] >> 16) & 0x000000FF)]++;
                wykresG[((obrazPiksele[i] >> 8) & 0x000000FF)]++;
                wykresB[(obrazPiksele[i] & 0x000000FF)]++;
                wykresX[(((obrazPiksele[i] >> 16) & 0x000000FF)+ ((obrazPiksele[i] >> 8) & 0x000000FF)+ (obrazPiksele[i] & 0x000000FF))/3]++;
            }
            for (int i = 0; i < 256; i++)
            {
                Points5.Add(new DataPoint(i, wykresR[i]));
                Points6.Add(new DataPoint(i, wykresG[i]));
                Points7.Add(new DataPoint(i, wykresB[i]));
                Points8.Add(new DataPoint(i, wykresX[i]));
            }
        }
    }
}

﻿using System;
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
        public int[] wykresX = new int[256];
        public uint[] obrazPiksele;

        public Histogram(uint[] obrazPiksele)
        {
            this.obrazPiksele = obrazPiksele;
            this.Points1 = new List<DataPoint> { };
            this.Points2 = new List<DataPoint> { };
            this.Points3 = new List<DataPoint> { };
            this.Points4 = new List<DataPoint> { };
        }

        public string Title { get; private set; }

        public IList<DataPoint> Points1 { get; private set; }
        public IList<DataPoint> Points2 { get; private set; }
        public IList<DataPoint> Points3 { get; private set; }
        public IList<DataPoint> Points4 { get; private set; }

        public void obliczHistogram()
        {
            Points1.Clear();
            Points2.Clear();
            Points3.Clear();
            Points4.Clear();
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
                wykresX[(((obrazPiksele[i] >> 16) & 0x000000FF) + ((obrazPiksele[i] >> 8) & 0x000000FF) + (obrazPiksele[i] & 0x000000FF)) / 3]++;
            }
            for (int i = 0; i < 256; i++)
            {
                Points1.Add(new DataPoint(i, wykresR[i]));
                Points2.Add(new DataPoint(i, wykresG[i]));
                Points3.Add(new DataPoint(i, wykresB[i]));
                Points4.Add(new DataPoint(i, wykresX[i]));
            }
        }
    }
}

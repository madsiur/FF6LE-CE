using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace FF3LE
{
    public class PaletteSet
    {
        private byte[] data; public byte[] Data { get { return this.data; } set { this.data = value; } }

        private int paletteSetOffset; public int PaletteSetOffset { get { return paletteSetOffset; } set { paletteSetOffset = value; } }
        private int paletteSetNum; public int PaletteSetNum { get { return paletteSetNum; } set { paletteSetNum = value; } }

        // every palette set has 112 (ie. 7 * 16) colors, a red green and blue for each
        private int[] paletteColorRed = new int[8 * 16]; public int[] PaletteColorRed { get { return paletteColorRed; } set { paletteColorRed = value; } }
        private int[] paletteColorGreen = new int[8 * 16]; public int[] PaletteColorGreen { get { return paletteColorGreen; } set { paletteColorGreen = value; } }
        private int[] paletteColorBlue = new int[8 * 16]; public int[] PaletteColorBlue { get { return paletteColorBlue; } set { paletteColorBlue = value; } }

        private byte[] stPaletteSet;

        public PaletteSet(byte[] data, int paletteSetNum)
        {
            this.data = data;
            this.paletteSetNum = paletteSetNum;
            InitializePaletteSet(data);
        }
        public PaletteSet(int paletteSetNum, byte[] stPaletteSet)
        {
            this.stPaletteSet = stPaletteSet;
            this.paletteSetNum = paletteSetNum;
            InitializePaletteSet(stPaletteSet);
        }

        private void InitializePaletteSet(byte[] data)
        {
            switch (paletteSetNum)
            {
                case 48: paletteSetOffset = 0x12EC00; break;
                case 49: paletteSetOffset = 0x12ED00; break;
                default: paletteSetOffset = (paletteSetNum * 0x100) + 0x2DC480; break;
            }

            double multiplier = 8.2258064516; // 255 / 31;
            ushort color = 0;

            for (int i = 0; i < 8; i++) // 7 palettes in set
            {
                for (int j = 0; j < 16; j++) // 16 colors in palette
                {
                    if (paletteSetNum == 50)
                        color = ByteManage.GetShort(stPaletteSet, (i * 32) + (j * 2));
                    else
                        color = ByteManage.GetShort(data, paletteSetOffset + (i * 32) + (j * 2));

                    paletteColorRed[(i * 16) + j] = (byte)((color % 0x20) * multiplier);
                    paletteColorGreen[(i * 16) + j] = (byte)(((color >> 5) % 0x20) * multiplier);
                    paletteColorBlue[(i * 16) + j] = (byte)(((color >> 10) % 0x20) * multiplier);
                }
            }
        }
        public void Assemble()
        {
            switch (paletteSetNum)
            {
                case 48: paletteSetOffset = 0x12EC00; break;
                case 49: paletteSetOffset = 0x12ED00; break;
                default: paletteSetOffset = (paletteSetNum * 0x100) + 0x2DC480; break;
            }

            ushort color = 0;
            int r, g, b;

            for (int i = 0; i < 8; i++) // 8 palettes in set
            {
                for (int j = 0; j < 16; j++) // 16 colors in palette
                {
                    r = (int)(paletteColorRed[(i * 16) + j] / (255 / 31));
                    g = (int)(paletteColorGreen[(i * 16) + j] / (255 / 31));
                    b = (int)(paletteColorBlue[(i * 16) + j] / (255 / 31));
                    color = (ushort)((b << 10) | (g << 5) | r);
                    if (paletteSetNum != 50)
                        ByteManage.SetShort(data, paletteSetOffset + (i * 32) + (j * 2), color);
                    else
                        ByteManage.SetShort(stPaletteSet, (i * 32) + (j * 2), color);
                }
            }
        }
        public int[] GetPaletteSetPixels()
        {
            int[] paletteSetPixels = new int[256 * 128];

            for (int i = 0; i < 8; i++) // 7 palette blocks high
            {
                for (int j = 0; j < 16; j++) // 16 palette blocks wide
                {
                    for (int y = 0; y < 16; y++)
                    {
                        for (int x = 0; x < 16; x++)
                            paletteSetPixels[x + (j * 16) + ((y + (i * 16)) * 256)] = Color.FromArgb(255, paletteColorRed[j + (i * 16)], paletteColorGreen[j + (i * 16)], paletteColorBlue[j + (i * 16)]).ToArgb();
                    }
                }
            }

            for (int y = 0; y < 128; y += 16)  // draw the horizontal gridlines
            {
                for (int x = 0; x < 256; x++)
                    paletteSetPixels[y * 256 + x] = Color.Black.ToArgb();
                if (y == 0) y--;
            }
            for (int x = 0; x < 256; x += 16) // draw the vertical gridlines
            {
                for (int y = 0; y < 128; y++)
                    paletteSetPixels[y * 256 + x] = Color.Black.ToArgb();
                if (x == 0) x--;
            }

            return paletteSetPixels;
        }
        public int GetBGColor()
        {
            return Color.FromArgb(255, paletteColorRed[0], paletteColorGreen[0], paletteColorBlue[0]).ToArgb();
        }
        public int[] Get2bppPalette(int paletteIndex)
        {
            int[] temp = new int[4];
            if (paletteIndex < 0 || paletteIndex > 7) paletteIndex = 0;
            paletteIndex *= 4;

            // read the 8 colors
            for (int i = 0; i < 4; i++)
                temp[i] = Color.FromArgb(255, paletteColorRed[i + paletteIndex], paletteColorGreen[i + paletteIndex], paletteColorBlue[i + paletteIndex]).ToArgb();

            return temp;
        }
        public int[] Get4bppPalette(int paletteIndex)
        {
            int[] temp = new int[16];
            if (paletteIndex < 0 || paletteIndex > 7) paletteIndex = 0;
            paletteIndex *= 16;

            // read the 16 colors
            for (int i = 0; i < 16; i++)
                temp[i] = Color.FromArgb(255, paletteColorRed[i + paletteIndex], paletteColorGreen[i + paletteIndex], paletteColorBlue[i + paletteIndex]).ToArgb();

            return temp;
        }
    }
}

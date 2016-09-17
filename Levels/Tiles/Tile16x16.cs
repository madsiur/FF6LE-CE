using System;
using System.Collections.Generic;
using System.Text;

namespace FF3LE
{
    class Tile16x16
    {

        private int tileNumber;
        private Tile8x8[] subtiles = new Tile8x8[4];
        public bool GetPriority1(int placement) { return subtiles[placement].PriorityOne; }
        public Tile8x8 GetSubtile(int placement) { return subtiles[placement]; }
        public int TileNumber { get { return tileNumber; } set { tileNumber = value; } }

        private bool mirrored; public bool Mirrored { get { return mirrored; } set { mirrored = value; } }
        private bool inverted; public bool Inverted { get { return inverted; } set { inverted = value; } }
        
        public Tile16x16(int tileNumber)
        {
            this.tileNumber = tileNumber; // set tile Number
        }
        public void SetSubtile(Tile8x8 tile, int placement)
        {
            //[0][1]
            //[2][3]
            subtiles[placement] = tile;
        }

        /*
         * This method fills the 16x16 pixel buf with the correct graphics from the
         * 8x8 tiles, but only if we have all the subtiles
         */
        private void CopyOverTile8x8(Tile8x8 source, int[] dest, int destinationWidth, int x, int y)
        {
            x *= 8;
            y *= 8;

            int[] src = source.Pixels;
            int counter = 0;
            for (int i = 0; i < 64; i++)
            {
                dest[y * destinationWidth + x + counter] = src[i];
                counter++;
                if (counter % 8 == 0)
                {
                    y++;
                    counter = 0;
                }
            }
        }


    }
}

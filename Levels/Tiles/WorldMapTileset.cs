using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace FF3LE
{
    class WorldMapTileset
    {
        private Model model;
        public PaletteSet paletteSet;

        private int levelNum;

        private byte[] graphicSet = new byte[0x2000]; public byte[] GraphicSet { get { return graphicSet; } set { graphicSet = value; } }
        private byte[] tilepalette = new byte[0x80]; public byte[] TilePalette { get { return tilepalette; } set { tilepalette = value; } }
        private byte[] tileset = new byte[0x480]; public byte[] Tileset { get { return tileset; } set { tileset = value; } }
        private Tile16x16[] tilesetLayer; public Tile16x16[] TileSetLayer { get { return tilesetLayer; } }

        public WorldMapTileset(int levelNum, PaletteSet paletteSet, Model model)
        {
            this.levelNum = levelNum;
            this.model = model; // grab the model
            this.paletteSet = paletteSet; // grab the current Palette Set

            // Create our layers for the tilesets (256x512)
            tilesetLayer = new Tile16x16[16 * 16];

            for (int i = 0; i < tilesetLayer.Length; i++)
                tilesetLayer[i] = new Tile16x16(i);

            if (levelNum == 0)
            {
                for (int i = 0; i < 0x400; i++)
                    tileset[i] = model.WobGraphicSet[i];
                for (int i = 0x2400, a = 0; i < 0x2480; i++, a++)
                    tilepalette[a] = model.WobGraphicSet[i];
                for (int i = 0x400, a = 0; i < 0x2400; i++, a++)
                    graphicSet[a] = model.WobGraphicSet[i];
            }
            else if (levelNum == 1)
            {
                for (int i = 0; i < 0x400; i++)
                    tileset[i] = model.WorGraphicSet[i];
                for (int i = 0x2400, a = 0; i < 0x2480; i++, a++)
                    tilepalette[a] = model.WorGraphicSet[i];
                for (int i = 0x400, a = 0; i < 0x2400; i++, a++)
                    graphicSet[a] = model.WorGraphicSet[i];
            }
            else
            {
                for (int i = 0; i < 0x400; i++)
                    tileset[i] = model.StGraphicSet[i];
                for (int i = 0x2400, a = 0; i < 0x2480; i++, a++)
                    tilepalette[a] = model.StGraphicSet[i];
                for (int i = 0x400, a = 0; i < 0x2400; i++, a++)
                    graphicSet[a] = model.StGraphicSet[i];
            }

            DrawTileset(tilesetLayer);
        }

        public void DrawTileset(Tile16x16[] tilesetLayer)
        {
            byte tile, temp;
            Tile8x8 source;
            int offset = 0;

            for (int y = 0; y < 16; y++)
            {
                for (int x = 0; x < 16; x++)
                {
                    int i = y * 16 + x;

                    for (int z = 0; z < 4; z++)
                    {
                        tile = ByteManage.GetByte(tileset, offset);

                        temp = ByteManage.GetByte(tilepalette, tile / 2);
                        temp = tile % 2 == 0 ? (byte)(temp & 0x07) : (byte)((temp & 0x70) >> 4);

                        source = Draw4bppTile8x8(tile, temp);
                        tilesetLayer[i].SetSubtile(source, z);

                        offset++;
                    }
                }
            }
        }
        public void RedrawTileset()
        {
            DrawTileset(tilesetLayer);
        }

        public Tile8x8 Draw4bppTile8x8(byte tile, byte paletteSetIndex)
        {
            int tileDataOffset = (tile * 0x20);

            if (tileDataOffset >= graphicSet.Length)
                tileDataOffset = 0;

            Tile8x8 temp;
            temp = new Tile8x8(tile, graphicSet, tileDataOffset, paletteSet.Get4bppPalette(paletteSetIndex));
            temp.PaletteSetIndex = paletteSetIndex;
            return temp;
        }

        public int GetTileNumber(int x, int y)
        {
            return tilesetLayer[x + y * 16].TileNumber;
        }

        public int[] GetTilesetPixelArray(Tile16x16[] tiles)
        {
            int[] pixels = new int[tiles.Length * 256];

            for (int y = 0; y < tiles.Length / 16; y++)
            {
                for (int x = 0; x < 16; x++)
                    CopyOverTile16x16(tiles[y * 16 + x], pixels, 256, x, y);
            }
            return pixels;
        }
        public int[] GetRangePixels(Point p, Size s)
        {
            int[] pixels = new int[s.Width * s.Height];

            for (int b = 0, y = p.Y / 16; b < s.Height / 16; b++, y++)
            {
                for (int a = 0, x = p.X / 16; a < s.Width / 16; a++, x++)
                {
                    CopyOverTile16x16(tilesetLayer[y * 16 + x], pixels, s.Width, a, b);
                }
            }

            return pixels;
        }
        private void CopyOverTile16x16(Tile16x16 source, int[] dest, int destinationWidth, int x, int y)
        {
            x *= 16;
            y *= 16;

            CopyOverTile8x8(source.GetSubtile(0), dest, destinationWidth, x, y);
            CopyOverTile8x8(source.GetSubtile(1), dest, destinationWidth, x + 8, y);
            CopyOverTile8x8(source.GetSubtile(2), dest, destinationWidth, x, y + 8);
            CopyOverTile8x8(source.GetSubtile(3), dest, destinationWidth, x + 8, y + 8);
        }

        private void CopyOverTile8x8(Tile8x8 source, int[] dest, int destinationWidth, int x, int y)
        {
            int[] src = source.Pixels;
            int counter = 0;
            for (int i = 0; i < 64; i++)
            {
                if (src[i] != 0)
                    dest[y * destinationWidth + x + counter] = src[i];
                counter++;
                if (counter % 8 == 0)
                {
                    y++;
                    counter = 0;
                }
            }
        }

        public byte GetPhysicalTile(int layer, int x, int y, bool prop)
        {
            int tileNum = y * 16 + x;

            if (levelNum == 0)
            {
                if (prop)
                    return model.WobPhysicalMap[tileNum * 2 + 1];
                else
                    return model.WobPhysicalMap[tileNum * 2];
            }
            else if (levelNum == 1)
            {
                if (prop)
                    return model.WorPhysicalMap[tileNum * 2 + 1];
                else
                    return model.WorPhysicalMap[tileNum * 2];
            }
            else
                return 0;
        }
    }
}

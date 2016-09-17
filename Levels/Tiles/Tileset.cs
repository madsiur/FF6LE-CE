using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace FF3LE
{
    class Tileset
    {
        private Model model;
        private LevelLayer levelLayer;
        public PaletteSet paletteSet;
        private State state;

        private byte[][] tileSets = new byte[3][]; public byte[][] TileSets { get { return tileSets; } set { tileSets = value; } }
        private byte[] graphicSets; public byte[] GraphicSets { get { return graphicSets; } set { graphicSets = value; } }
        private byte[] graphicSetL3; public byte[] GraphicSetL3 { get { return graphicSetL3; } set { graphicSetL3 = value; } }

        Tile16x16[][] tilesetLayers = new Tile16x16[3][];

        public Tile16x16[][] TileSetLayers
        {
            get
            {
                return tilesetLayers;
            }
        }

        public Tileset(LevelLayer levelLayer, PaletteSet paletteSet, Model model)
        {
            this.model = model; // grab the model
            this.levelLayer = levelLayer; // grab the current LevelLayer
            this.paletteSet = paletteSet; // grab the current Palette Set
            state = State.Instance; // grab an instance of our state

            // Create our layers for the tilesets (256x512)
            tilesetLayers[0] = new Tile16x16[16 * 16];

            tilesetLayers[1] = new Tile16x16[16 * 16];
            tilesetLayers[2] = new Tile16x16[16 * 16];

            CreateLayers(); // Create inidividual tiles

            DecompressTileSetData(); // Decompress our required data

            DrawTilesetL1L2(tileSets[0], tilesetLayers[0]);
            DrawTilesetL1L2(tileSets[1], tilesetLayers[1]);
            DrawTilesetL3(tileSets[2], tilesetLayers[2]);
        }
        // Layer 3 = all layers
        public void RedrawTilesets(int layer)
        {
            if (layer == 0)// || layer == 3)
                DrawTilesetL1L2(tileSets[0], tilesetLayers[0]);
            if (layer == 1)// || layer == 3)
                DrawTilesetL1L2(tileSets[1], tilesetLayers[1]);
            if (layer == 2)// || layer == 3)
                DrawTilesetL3(tileSets[2], tilesetLayers[2]);
        }

        private void CreateLayers()
        {
            int i;
            for (i = 0; i < tilesetLayers[0].Length; i++)
                tilesetLayers[0][i] = new Tile16x16(i);
            for (i = 0; i < tilesetLayers[1].Length; i++)
                tilesetLayers[1][i] = new Tile16x16(i);
            for (i = 0; i < tilesetLayers[2].Length; i++)
                tilesetLayers[2][i] = new Tile16x16(i);
        }

        private void DecompressTileSetData()
        {
            byte[] graphicSet0, graphicSet1, graphicSet2, graphicSet3;

            // Decompress data at offsets
            tileSets[0] = model.TileSets[levelLayer.TileSetL1];
            tileSets[1] = model.TileSets[levelLayer.TileSetL2];
            tileSets[2] = new byte[0x1000];

            // Decompress graphic sets
            graphicSet0 = model.GraphicSets[levelLayer.GraphicSetA];
            graphicSet1 = model.GraphicSets[levelLayer.GraphicSetB];
            graphicSet2 = model.GraphicSets[levelLayer.GraphicSetC];
            graphicSet3 = model.GraphicSets[levelLayer.GraphicSetD];
            graphicSetL3 = model.GraphicSetsL3[levelLayer.GraphicSetL3];

            // Create buffer the size of the combined graphicSets
            graphicSets = new byte[0x8000];
            int index = 0;
            graphicSet0.CopyTo(graphicSets, index); index += 0x2000;
            graphicSet1.CopyTo(graphicSets, index); index += 0x1000;
            graphicSet2.CopyTo(graphicSets, index); index += 0x1000;
            graphicSet3.CopyTo(graphicSets, index); index += 0x1000;

            int pointer, offset;

            pointer = ByteManage.GetShort(model.Data, levelLayer.AnimationL2 * 2 + 0x0091D5);
            pointer += 2;

            for (int y = 0; y < 32; y++) // 8 rows
            {
                for (int i = 0; i < 0x80; i++) // 128 bytes each tile
                {
                    offset = ByteManage.GetShort(model.Data, pointer + 0x0091FF + (y * 10));
                    graphicSets[y * 0x80 + i + index] = model.AnimatedGraphics[i + offset];
                }
            }
        }

        public void DrawTilesetL1L2(byte[] tileset, Tile16x16[] tilesetLayer)
        {
            byte temp, tile;
            Tile8x8 source;
            int offset = 0;

            for (int i = 0; i < tilesetLayer.Length; i++)
            {
                for (int z = 0; z < 4; z++)
                {
                    tile = ByteManage.GetByte(tileset, offset); offset += 0x400;
                    temp = ByteManage.GetByte(tileset, offset); offset -= 0x400;
                    source = Draw4bppTile8x8(tile, temp);
                    tilesetLayer[i].SetSubtile(source, z);
                    offset += 0x100;
                }

                offset++;
                offset -= 0x400;
            }
        }

        public void DrawTilesetL3(byte[] tileset, Tile16x16[] tilesetLayer)
        {
            byte tile;
            Tile8x8 source;

            for (int i = 0; i < tilesetLayer.Length; i++)
            {
                for (int z = 0; z < 4; z++)
                {
                    tile = (byte)((i * 4) + z);
                    source = Draw2bppTile8x8(tile, 0);
                    tilesetLayer[i].SetSubtile(source, z);
                }
            }
        }

        private Tile8x8 Draw2bppTile8x8(byte tile, byte temp)
        {
            bool twobpp = true;

            int tileDataOffset = (tile * 0x10);

            if (tileDataOffset >= graphicSetL3.Length)
                tileDataOffset = 0;

            Tile8x8 tempTile = new Tile8x8(tile, graphicSetL3, tileDataOffset, paletteSet.Get2bppPalette(0), false, false, false, twobpp);
            tempTile.PaletteSetIndex = 0;
            tempTile.GfxSetIndex = 0;
            return tempTile;
        }

        private Tile8x8 Draw4bppTile8x8(byte tile, byte temp)
        {
            byte graphicSetIndex, paletteSetIndex;
            bool mirrored, inverted, priorityOne;
            bool twobpp = false;

            graphicSetIndex = (byte)(temp & 0x03);
            paletteSetIndex = (byte)((temp & 0x1F) >> 2);

            inverted = (temp & 0x80) == 0x80;
            mirrored = (temp & 0x40) == 0x40;
            priorityOne = (temp & 0x20) == 0x20;

            int tileDataOffset = (tile * 0x20) + (graphicSetIndex * 0x2000);

            if (tileDataOffset >= graphicSets.Length)
                tileDataOffset = 0;

            Tile8x8 tempTile = new Tile8x8(tile, graphicSets, tileDataOffset, paletteSet.Get4bppPalette(paletteSetIndex), mirrored, inverted, priorityOne, twobpp);

            tempTile.GfxSetIndex = graphicSetIndex;
            tempTile.PaletteSetIndex = (byte)(paletteSetIndex + 1);
            return tempTile;
        }

        public int GetTileNumber(int layer, int x, int y)
        {
            if (layer < 3)
                return tilesetLayers[layer][x + y * 16].TileNumber;
            else return 0;
        }
        public int[] GetTilesetPixelArray(Tile16x16[] tiles)
        {
            int[] pixels = new int[tiles.Length * 256];

            for (int y = 0; y < tiles.Length / 16; y++)
            {
                for (int x = 0; x < 16; x++)
                {
                    CopyOverTile16x16(tiles[y * 16 + x], pixels, 256, x, y);
                }
            }
            return pixels;
        }
        public int[] GetRangePixels(int layer, Point p, Size s)
        {
            int[] pixels = new int[s.Width * s.Height];

            for (int b = 0, y = p.Y / 16; b < s.Height / 16; b++, y++)
            {
                for (int a = 0, x = p.X / 16; a < s.Width / 16; a++, x++)
                {
                    CopyOverTile16x16(tilesetLayers[layer][y * 16 + x], pixels, s.Width, a, b);
                }
            }

            return pixels;
        }
        private void CopyOverTile16x16(Tile16x16 source, int[] dest, int destinationWidth, int x, int y)
        {
            x *= 16;
            y *= 16;

            if ((source.GetPriority1(0) && state.Priority1) || !source.GetPriority1(0))
                CopyOverTile8x8(source.GetSubtile(0), dest, destinationWidth, x, y);
            if ((source.GetPriority1(1) && state.Priority1) || !source.GetPriority1(1))
                CopyOverTile8x8(source.GetSubtile(1), dest, destinationWidth, x + 8, y);
            if ((source.GetPriority1(2) && state.Priority1) || !source.GetPriority1(2))
                CopyOverTile8x8(source.GetSubtile(2), dest, destinationWidth, x, y + 8);
            if ((source.GetPriority1(3) && state.Priority1) || !source.GetPriority1(3))
                CopyOverTile8x8(source.GetSubtile(3), dest, destinationWidth, x + 8, y + 8);

        }

        /*
        * This method fills the 16x16 pixel buf with the correct graphics from the
        * 8x8 tiles, but only if we have all the subtiles
        */
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

        public void Clear(int count)
        {
            if (count == 1)
            {
                model.TileSets[levelLayer.TileSetL1] = new byte[0x2000];
                model.TileSets[levelLayer.TileSetL2] = new byte[0x2000];
            }
            else
            {
                for (int i = 0; i < count; i++)
                {
                    if (i < 0x20) model.TileSets[i] = new byte[0x1000];
                    else model.TileSets[i] = new byte[0x2000];
                }
            }
            for (int i = 0; i < 3; i++) RedrawTilesets(i);
        }

        public byte GetPhysicalTile(int layer, int x, int y, bool prop)
        {
            int tileNum = y * 16 + x;

            if (model.PhysicalMaps[levelLayer.PhysicalMap] == null) // if empty physical map
                return 0;

            if (prop)
                return model.PhysicalMaps[levelLayer.PhysicalMap][tileNum + 256];
            else
                return model.PhysicalMaps[levelLayer.PhysicalMap][tileNum];
        }
    }
}

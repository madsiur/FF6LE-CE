using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace FF3LE
{
    class WorldMapTilemap
    {
        private Model model;
        private WorldMapTileset tileset;
        private PaletteSet paletteSet;
        private State state;

        private int levelNum;

        private Tile16x16[] layer;
        private byte[] tileMap; public byte[] TileMap { get { return tileMap; } set { tileMap = value; } }
        private int[] mainscreen; public int[] Mainscreen { get { return mainscreen; } }
        private Size screen;

        private int[] tile = new int[256];
        private int[] buffer = new int[256];

        public WorldMapTilemap(int levelNum, LevelLayer layer, PaletteSet paletteSet, WorldMapTileset tileset, Model model)
        {
            this.levelNum = levelNum;
            this.model = model;
            this.tileset = tileset;
            this.paletteSet = paletteSet;
            this.state = State.Instance;

            switch (levelNum)
            {
                case 0:
                    tileMap = model.WobTileMap;
                    screen = new Size(4096, 4096); break;
                case 1:
                    tileMap = model.WorTileMap;
                    screen = new Size(4096, 4096); break;
                case 2:
                    tileMap = model.StTileMap;
                    screen = new Size(2048, 2048); break;
            }

            mainscreen = new int[screen.Width * screen.Height];

            CreateLayer();
            DrawLayer(mainscreen);
        }

        private void CreateLayer()
        {
            int offset = 0;
            byte tileNum;

            layer = new Tile16x16[(screen.Width / 16) * (screen.Height / 16)]; // Create our layer here

            int temp = 0;
            int i = 0;
            for (int y = 0; y < screen.Height / 16; y++)
            {
                for (int x = 0; x < screen.Width / 16; x++)
                {
                    i = y * (screen.Width / 16) + x;

                    tileNum = ByteManage.GetByte(tileMap, offset); offset++;

                    temp = tileNum;

                    layer[i] = new Tile16x16(tileNum);

                    for (int a = 0; a < 4; a++)
                        layer[i].SetSubtile(tileset.TileSetLayer[tileNum].GetSubtile(a), a);
                }
            }
        }
        private int[] DrawLayer(int[] dest)
        {
            if (dest.Length != mainscreen.Length || layer == null)
                return null;

            for (int y = 0; y < screen.Height / 16; y++)
            {
                for (int x = 0; x < screen.Width / 16; x++)
                    CopyOverTile16x16(layer[x * (screen.Width / 16) + y], dest, screen.Width, y * 16, x * 16, true);
            }

            return dest;
        }
        private void CopyOverTile8x8(Tile8x8 source, int[] dest, int destinationWidth, int x, int y)
        {
            int[] src = new int[source.Pixels.Length];
            source.Pixels.CopyTo(src, 0);

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
        private void CopyOverTile16x16(Tile16x16 source, int[] dest, int destinationWidth, int x, int y, bool priority)
        {
            int[] pixels = new int[16 * 16];

            for (int a = 0; a < 4; a++)
                CopyOverTile8x8(source.GetSubtile(a), pixels, 16, a % 2 * 8, a / 2 * 8);

            if (source.Mirrored)
                MirrorTile16x16(pixels);
            if (source.Inverted)
                InvertTile16x16(pixels);

            for (int b = 0; b < 16; b++)
            {
                for (int a = 0; a < 16; a++)
                {
                    if (pixels[b * 16 + a] != 0)
                        dest[(b + y) * destinationWidth + a + x] = pixels[b * 16 + a];
                }
            }
        }

        private void MirrorTile16x16(int[] src)
        {
            int temp = 0;

            for (int y = 0; y < 16; y++)
            {
                for (int a = 0, b = 15; a < 8; a++, b--)
                {
                    temp = src[(y * 16) + a];
                    src[(y * 16) + a] = src[(y * 16) + b];
                    src[(y * 16) + b] = temp;
                }
            }
        }
        private void InvertTile16x16(int[] src)
        {
            int temp = 0;

            for (int x = 0; x < 16; x++)
            {
                for (int a = 0, b = 15; a < 8; a++, b--)
                {
                    temp = src[(a * 16) + x];
                    src[(a * 16) + x] = src[(b * 16) + x];
                    src[(b * 16) + x] = temp;
                }
            }
        }

        public int GetTileNum(int x, int y)
        {
            Math.Min(screen.Width - 1, Math.Max(0, x));
            Math.Min(screen.Height - 1, Math.Max(0, y));
            y /= 16;
            x /= 16;
            int placement = y * (screen.Width / 16) + x;

            if (layer != null)
                return layer[placement].TileNumber;
            else return 0;
        }
        public int[] GetRangePixels(Point p, Size s)
        {
            int[] pixels = new int[s.Width * s.Height];

            for (int b = 0, y = p.Y; b < s.Height; b++, y++)
            {
                for (int a = 0, x = p.X; a < s.Width; a++, x++)
                {
                    pixels[b * s.Width + a] = mainscreen[y * screen.Width + x];
                    if (mainscreen[y * screen.Width + x] != 0)
                        pixels[b * s.Width + a] = mainscreen[y * screen.Width + x];
                }
            }

            return pixels;
        }

        public void MakeEdit(int tileNum, int x, int y)
        {
            // x and y are in pixel format
            Math.Min(screen.Width - 1, Math.Max(0, x));
            Math.Min(screen.Height - 1, Math.Max(0, y));
            y /= 16;
            x /= 16;
            int tile = y * (screen.Width / 16) + x;
            try
            {
                if (levelNum < 2)
                {
                    if (x >= 0 && y >= 0 && tile < 0x10000)
                        ChangeSingleTile(tile, tileNum, x * 16, y * 16);
                }
                else
                {
                    if (x >= 0 && y >= 0 && tile < 0x4000)
                        ChangeSingleTile(tile, tileNum, x * 16, y * 16);
                }
            }
            catch
            {
                // invalid layer/tile
                System.Windows.Forms.MessageBox.Show("MAKE EDIT PROBLEM");
            }
        }

        // change single tile
        private void ChangeSingleTile(int placement, int tile, int x, int y)
        {
            layer[placement] = tileset.TileSetLayer[tile]; // Change the tile in the layer map

            Tile16x16 source = layer[placement]; // Grab the new tile

            CopyOverTile8x8(source.GetSubtile(0), mainscreen, screen.Width, x, y);
            CopyOverTile8x8(source.GetSubtile(1), mainscreen, screen.Width, x + 8, y);
            CopyOverTile8x8(source.GetSubtile(2), mainscreen, screen.Width, x, y + 8);
            CopyOverTile8x8(source.GetSubtile(3), mainscreen, screen.Width, x + 8, y + 8);

            DrawSingleMainscreenTile(x, y);
        }
        private void ClearSingleTile(int[] arr, int x, int y)
        {
            int counter = 0;
            for (int i = 0; i < 256; i++)
            {
                arr[y * screen.Width + x + counter] = 0;

                counter++;
                if (counter % 16 == 0)
                {
                    y++;
                    counter = 0;
                }
            }
        }
        private void CopySingleTileToArray(int[] dest, int[] source, int width, int x, int y)
        {
            int counter = 0;
            for (int i = 0; i < 256; i++)
            {
                if (source[i] != 0)
                    dest[y * width + x + counter] = source[i];

                counter++;
                if (counter % 16 == 0)
                {
                    y++;
                    counter = 0;
                }
            }
        }
        private void DrawSingleMainscreenTile(int x, int y)
        {
            //int bgcolor = paletteSet.GetBGColor();

            if (state.Layer1)
                CopySingleTileToArray(mainscreen, GetTileFromPriorityArray(mainscreen, x, y), screen.Width, x, y);
        }
        private int[] GetTileFromPriorityArray(int[] arr, int x, int y)
        {
            int counter = 0;
            for (int i = 0; i < 256; i++)
            {
                if (arr[y * screen.Width + x + counter] != 0)
                    tile[i] = arr[y * screen.Width + x + counter];

                counter++;
                if (counter % 16 == 0)
                {
                    y++;
                    counter = 0;
                }
            }
            return tile;
        }

        private void ClearArray(IList arr)
        {
            if (arr == null) return;
            arr.Clear();
        }

        public void AssembleIntoModel()
        {
            for (int i = 0; i < layer.Length; i++)
            {
                tileMap[i] = (byte)layer[i].TileNumber;
            }
        }

        public byte GetPhysicalTile(int x, int y, bool prop)
        {
            if (levelNum == 0)
            {
                if (prop)
                    return model.WobPhysicalMap[GetTileNum(x, y) * 2 + 1];
                else
                    return model.WobPhysicalMap[GetTileNum(x, y) * 2];
            }
            else if (levelNum == 1)
            {
                if (prop)
                    return model.WorPhysicalMap[GetTileNum(x, y) * 2 + 1];
                else
                    return model.WorPhysicalMap[GetTileNum(x, y) * 2];
            }
            else
            {
                return 0;
            }
        }
        public byte GetPhysicalTile(int tileNum, bool prop)
        {
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
            {
                return 0;
            }
        }

        public void RedrawTileMap()
        {
            DrawLayer(mainscreen);
        }
    }
}

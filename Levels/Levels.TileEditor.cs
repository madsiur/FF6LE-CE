using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace FF3LE
{
    public partial class Levels
    {
        #region Variables

        private bool updatingSubtile;
        private bool updatingPhystile;

        private int currentTile { get { return overlay.TileSelected; } }
        private int currentSubtile = 0;
        private int clickedSubTile = 0;

        private Bitmap graphicSetImage, tileImage, subtileImage;

        #endregion

        #region Methods

        private void InitializeTile()
        {
            updatingPhystile = true;

            if (currentLevel < 2 && tabControl2.SelectedIndex == 0)
            {
                int byte0, byte1;
                if (currentLevel == 0)
                {
                    byte0 = model.WobPhysicalMap[currentTile * 2];
                    byte1 = model.WobPhysicalMap[currentTile * 2 + 1];
                }
                else
                {
                    byte0 = model.WorPhysicalMap[currentTile * 2];
                    byte1 = model.WorPhysicalMap[currentTile * 2 + 1];
                }
                physicalAirship.SelectedIndex = (byte0 & 0x0C) >> 2;
                physicalBattleBG.SelectedIndex = byte1 & 0x07;
                physicalProperties.SetItemChecked(0, (byte0 & 0x01) == 0x01);
                physicalProperties.SetItemChecked(1, (byte0 & 0x02) == 0x02);
                physicalProperties.SetItemChecked(2, (byte0 & 0x10) == 0x10);
                physicalProperties.SetItemChecked(3, (byte0 & 0x20) == 0x20);
                physicalProperties.SetItemChecked(4, (byte0 & 0x40) == 0x40);
                physicalUnknown.SetItemChecked(0, (byte0 & 0x80) == 0x80);
                physicalUnknown.SetItemChecked(1, (byte1 & 0x08) == 0x08);
                physicalUnknown.SetItemChecked(2, (byte1 & 0x10) == 0x10);
                physicalUnknown.SetItemChecked(3, (byte1 & 0x20) == 0x20);
                physicalUnknown.SetItemChecked(4, (byte1 & 0x40) == 0x40);
                physicalUnknown.SetItemChecked(5, (byte1 & 0x80) == 0x80);
            }
            else if (currentLevel > 2)
            {
                int byte0 = model.PhysicalMaps[layer.PhysicalMap][currentTile];
                int byte1 = model.PhysicalMaps[layer.PhysicalMap][currentTile + 0x100];

                physicalSolidTile.Checked = (byte0 & 0x04) == 0x04;

                physicalStairs.SelectedIndex = Math.Min((byte0 & 0xC0) >> 6, 2);
                physicalTileProperties.SetItemChecked(0, (byte1 & 0x01) == 0x01);
                physicalTileProperties.SetItemChecked(1, (byte1 & 0x02) == 0x02);
                physicalTileProperties.SetItemChecked(2, (byte1 & 0x04) == 0x04);
                physicalTileProperties.SetItemChecked(3, (byte1 & 0x08) == 0x08);
                physicalTileProperties.SetItemChecked(4, (byte1 & 0x40) == 0x40);
                physicalOtherBits.SetItemChecked(0, (byte0 & 0x01) == 0x01);
                physicalOtherBits.SetItemChecked(1, (byte0 & 0x02) == 0x02);
                physicalOtherBits.SetItemChecked(2, (byte0 & 0x08) == 0x08);
                physicalOtherBits.SetItemChecked(3, (byte0 & 0x10) == 0x10);
                physicalOtherBits.SetItemChecked(4, (byte0 & 0x20) == 0x20);
                physicalOtherBits.SetItemChecked(5, (byte1 & 0x10) == 0x10);
                physicalOtherBits.SetItemChecked(6, (byte1 & 0x20) == 0x20);
                physicalOtherBits.SetItemChecked(7, (byte1 & 0x80) == 0x80);
            }

            updatingPhystile = false;

            if (currentLevel < 3)
            {
                tileGFXSet.Enabled = false;
                tileAttributes.Enabled = false;
            }
            else
            {
                tileGFXSet.Enabled = true;
                tileAttributes.Enabled = true;
            }

            InitializeSubtile();

            SetTileImage();
            SetSubtileImage();
            SetGraphicSetImage();
        }
        private void InitializeSubtile()
        {
            updatingSubtile = true;

            if (currentLevel > 2)
            {
                tile8x8Tile.Value = tileSet.TileSetLayers[tabControl2.SelectedIndex][(ushort)currentTile].GetSubtile(currentSubtile).TileNum;
                tileGFXSet.Value = tileSet.TileSetLayers[tabControl2.SelectedIndex][(ushort)currentTile].GetSubtile(currentSubtile).GfxSetIndex;
                tilePalette.Value = tileSet.TileSetLayers[tabControl2.SelectedIndex][(ushort)currentTile].GetSubtile(currentSubtile).PaletteSetIndex;
                tileAttributes.SetItemChecked(0, tileSet.TileSetLayers[tabControl2.SelectedIndex][(ushort)currentTile].GetSubtile(currentSubtile).PriorityOne);
                tileAttributes.SetItemChecked(1, tileSet.TileSetLayers[tabControl2.SelectedIndex][(ushort)currentTile].GetSubtile(currentSubtile).Mirrored);
                tileAttributes.SetItemChecked(2, tileSet.TileSetLayers[tabControl2.SelectedIndex][(ushort)currentTile].GetSubtile(currentSubtile).Inverted);
            }
            else
            {
                tile8x8Tile.Value = wmTileSet.TileSetLayer[(ushort)currentTile].GetSubtile(currentSubtile).TileNum;

                byte temp;
                if (tile8x8Tile.Value % 2 == 0)
                    temp = (byte)(wmTileSet.TilePalette[(byte)(tile8x8Tile.Value / 2)] & 0x0F);
                else
                    temp = (byte)(wmTileSet.TilePalette[(byte)(tile8x8Tile.Value / 2)] >> 4);

                tilePalette.Value = temp;
            }

            updatingSubtile = false;
        }

        // set images
        private void SetTileImage()
        {
            int[] temp = new int[16 * 16];
            int[] pixels = new int[32 * 32];

            for (int y = 0; y < 2; y++)
            {
                for (int x = 0; x < 2; x++)
                {
                    if (currentLevel > 2)
                        CopyOverTile8x8(tileSet.TileSetLayers[tabControl2.SelectedIndex][currentTile].GetSubtile(y * 2 + x), temp, 16, x, y);
                    else
                        CopyOverTile8x8(wmTileSet.TileSetLayer[currentTile].GetSubtile(y * 2 + x), temp, 16, x, y);
                }
            }
            for (int y = 0; y < 32; y++)
            {
                for (int x = 0; x < 32; x++)
                    pixels[y * 32 + x] = temp[y / 2 * 16 + (x / 2)];
            }
            tileImage = new Bitmap(DrawImageFromIntArr(pixels, 32, 32));
            pictureBoxTile.BackColor = Color.FromArgb(paletteSet.PaletteColorRed[0], paletteSet.PaletteColorGreen[0], paletteSet.PaletteColorBlue[0]);
            pictureBoxTile.Invalidate();
        }
        private void SetSubtileImage()
        {
            int[] temp = new int[8 * 8];
            int[] pixels = new int[32 * 32];

            if (currentLevel > 2)
                CopyOverTile8x8(tileSet.TileSetLayers[tabControl2.SelectedIndex][currentTile].GetSubtile(currentSubtile), temp, 8, 0, 0);
            else
                CopyOverTile8x8(wmTileSet.TileSetLayer[currentTile].GetSubtile(currentSubtile), temp, 8, 0, 0);

            for (int y = 0; y < 32; y++)
            {
                for (int x = 0; x < 32; x++)
                    pixels[y * 32 + x] = temp[y / 4 * 8 + (x / 4)];
            }
            subtileImage = new Bitmap(DrawImageFromIntArr(pixels, 32, 32));
            pictureBoxSubtile.BackColor = Color.FromArgb(paletteSet.PaletteColorRed[0], paletteSet.PaletteColorGreen[0], paletteSet.PaletteColorBlue[0]);
            pictureBoxSubtile.Invalidate();
        }
        private void SetGraphicSetImage()
        {
            int[] pixels = new int[256 * 256];
            int[] temp = new int[128 * 128];
            Tile8x8 subtile;

            for (int y = 0; y < 16; y++)
            {
                for (int x = 0; x < 16; x++)
                {
                    if (currentLevel > 2)
                        subtile = Draw4bppTile8x8(
                            y * 16 + x,
                            (byte)tileGFXSet.Value,
                            (byte)tilePalette.Value,
                            false, false, false);
                    else
                    {
                        byte tmp;
                        if ((y * 16 + x) % 2 == 0)
                            tmp = (byte)(wmTileSet.TilePalette[(byte)((y * 16 + x) / 2)] & 0x0F);
                        else
                            tmp = (byte)(wmTileSet.TilePalette[(byte)((y * 16 + x) / 2)] >> 4);

                        subtile = Draw4bppTile8x8(y * 16 + x, tmp);
                    }

                    CopyOverTile8x8(subtile, temp, 128, x, y);
                }
            }
            for (int y = 0; y < 256; y++)
            {
                for (int x = 0; x < 256; x++)
                    pixels[y * 256 + x] = temp[y / 2 * 128 + (x / 2)];
            }

            graphicSetImage = new Bitmap(DrawImageFromIntArr(pixels, 256, 256));
            pictureBoxGraphicSet.BackColor = Color.FromArgb(paletteSet.PaletteColorRed[0], paletteSet.PaletteColorGreen[0], paletteSet.PaletteColorBlue[0]);
            pictureBoxGraphicSet.Invalidate();
        }

        // drawing
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
        private Tile8x8 Draw4bppTile8x8(int tile, byte graphicSetIndex, byte paletteSetIndex, bool mirrored, bool inverted, bool priorityOne)
        {
            paletteSetIndex--;

            int tileDataOffset = (tile * 0x20) + (graphicSetIndex * 0x2000);

            if (tileDataOffset >= tileSet.GraphicSets.Length)
                tileDataOffset = 0;

            Tile8x8 temp = new Tile8x8(tile, tileSet.GraphicSets, tileDataOffset, paletteSet.Get4bppPalette(paletteSetIndex), mirrored, inverted, priorityOne, false);

            temp.GfxSetIndex = graphicSetIndex;
            temp.PaletteSetIndex = (byte)(paletteSetIndex + 1);
            return temp;
        }
        private Tile8x8 Draw4bppTile8x8(int tile, byte paletteSetIndex)
        {
            int tileDataOffset = (tile * 0x20);

            if (tileDataOffset >= wmTileSet.GraphicSet.Length)
                tileDataOffset = 0;

            return new Tile8x8(tile, wmTileSet.GraphicSet, tileDataOffset, paletteSet.Get4bppPalette(paletteSetIndex));
        }

        private Tile8x8 CreateNewSubtile()
        {
            // The current 8x8 tile that we are going to draw all the options for
            Tile8x8 currentSubTile;
            if (currentLevel > 2)
                currentSubTile = tileSet.TileSetLayers[tabControl2.SelectedIndex][(ushort)currentTile].GetSubtile(currentSubtile);
            else
                currentSubTile = wmTileSet.TileSetLayer[(ushort)currentTile].GetSubtile(currentSubtile);

            if (currentLevel > 2)
                return Draw4bppTile8x8((byte)this.tile8x8Tile.Value,
                    (byte)this.tileGFXSet.Value,
                    (byte)this.tilePalette.Value,
                    this.tileAttributes.GetItemChecked(1),
                    this.tileAttributes.GetItemChecked(2),
                    this.tileAttributes.GetItemChecked(0));
            else
                return Draw4bppTile8x8((byte)this.tile8x8Tile.Value, (byte)this.tilePalette.Value);
        }

        #endregion

        #region Event Handlers

        private void tileGFXSet_ValueChanged(object sender, EventArgs e)
        {
            if (updatingSubtile) return;

            int offset = currentTile + (currentSubtile * 0x100) + 0x400;
            this.tileSet.TileSetLayers[tabControl2.SelectedIndex][(ushort)currentTile].SetSubtile(CreateNewSubtile(), currentSubtile);
            ByteManage.SetByteBits(tileSet.TileSets[tabControl2.SelectedIndex], offset, (byte)tileGFXSet.Value, 0x03);

            if (tabControl2.SelectedIndex == 0)
                model.EditTileSets[layer.TileSetL1] = true;
            else
                model.EditTileSets[layer.TileSetL2] = true;

            SetTileImage();
            SetSubtileImage();
            SetGraphicSetImage();
        }
        private void tilePalette_ValueChanged(object sender, EventArgs e)
        {
            if (updatingSubtile) return;

            int offset = 0;

            if (currentLevel > 2)
            {
                offset = currentTile + (currentSubtile * 0x100) + 0x400;
                this.tileSet.TileSetLayers[tabControl2.SelectedIndex][(ushort)currentTile].SetSubtile(CreateNewSubtile(), currentSubtile);
                ByteManage.SetByteBits(tileSet.TileSets[tabControl2.SelectedIndex], offset, (byte)((byte)tilePalette.Value << 2), 0x1C);

                if (tabControl2.SelectedIndex == 0)
                    model.EditTileSets[layer.TileSetL1] = true;
                else
                    model.EditTileSets[layer.TileSetL2] = true;
            }
            else
            {
                offset = (byte)(tile8x8Tile.Value / 2) + 0x2400;
                this.wmTileSet.TileSetLayer[(ushort)currentTile].SetSubtile(CreateNewSubtile(), currentSubtile);

                byte check = (byte)tile8x8Tile.Value % 2 == 0 ? (byte)0x0F : (byte)0xF0;
                byte set = (byte)tile8x8Tile.Value % 2 == 0 ? (byte)tilePalette.Value : (byte)((byte)tilePalette.Value << 4);
                ByteManage.SetByteBits(wmTileSet.GraphicSet, offset, set, check);
                ByteManage.SetByteBits(wmTileSet.TilePalette, offset - 0x2400, set, check);

                switch (currentLevel)
                {
                    case 0: model.EditWobGraphicSet = true; break;
                    case 1: model.EditWorGraphicSet = true; break;
                    case 2: model.EditStGraphicSet = true; break;
                }
            }

            SetTileImage();
            SetSubtileImage();
            SetGraphicSetImage();
        }
        private void tileAttributes_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (updatingSubtile) return;

            int offset = currentTile + (currentSubtile * 0x100) + 0x400;
            this.tileSet.TileSetLayers[tabControl2.SelectedIndex][(ushort)currentTile].SetSubtile(CreateNewSubtile(), currentSubtile);
            ByteManage.SetBit(tileSet.TileSets[tabControl2.SelectedIndex], offset, 5, tileAttributes.GetItemChecked(0));
            ByteManage.SetBit(tileSet.TileSets[tabControl2.SelectedIndex], offset, 6, tileAttributes.GetItemChecked(1));
            ByteManage.SetBit(tileSet.TileSets[tabControl2.SelectedIndex], offset, 7, tileAttributes.GetItemChecked(2));

            if (tabControl2.SelectedIndex == 0)
                model.EditTileSets[layer.TileSetL1] = true;
            else
                model.EditTileSets[layer.TileSetL2] = true;

            SetTileImage();
            SetSubtileImage();
        }
        private void tile8x8Tile_ValueChanged(object sender, EventArgs e)
        {
            if (updatingSubtile) return;

            int offset = 0;

            if (currentLevel > 2)
            {
                offset = currentTile + (currentSubtile * 0x100);
                this.tileSet.TileSetLayers[tabControl2.SelectedIndex][(ushort)currentTile].SetSubtile(CreateNewSubtile(), currentSubtile);
                tileSet.TileSets[tabControl2.SelectedIndex][offset] = (byte)tile8x8Tile.Value;

                if (tabControl2.SelectedIndex == 0)
                    model.EditTileSets[layer.TileSetL1] = true;
                else
                    model.EditTileSets[layer.TileSetL2] = true;
            }
            else
            {
                offset = currentTile * 4 + currentSubtile;
                this.wmTileSet.TileSetLayer[(ushort)currentTile].SetSubtile(CreateNewSubtile(), currentSubtile);
                wmTileSet.GraphicSet[offset] = (byte)tile8x8Tile.Value;

                byte temp;
                if (tile8x8Tile.Value % 2 == 0)
                    temp = (byte)(wmTileSet.TilePalette[(byte)(tile8x8Tile.Value / 2)] & 0x0F);
                else
                    temp = (byte)(wmTileSet.TilePalette[(byte)(tile8x8Tile.Value / 2)] >> 4);

                tilePalette.Value = temp;

                switch (currentLevel)
                {
                    case 0: model.EditWobGraphicSet = true; break;
                    case 1: model.EditWorGraphicSet = true; break;
                    case 2: model.EditStGraphicSet = true; break;
                }
            }
            SetTileImage();
            SetSubtileImage();
        }

        private void pictureBoxSubtile_Paint(object sender, PaintEventArgs e)
        {
            if (subtileImage != null)
                e.Graphics.DrawImage(subtileImage, 0, 0);
        }
        private void pictureBoxTile_MouseClick(object sender, MouseEventArgs e)
        {
            currentSubtile = e.X / 16 + ((e.Y / 16) * 2);

            InitializeSubtile();
            SetSubtileImage();
            SetGraphicSetImage();
        }
        private void pictureBoxTile_Paint(object sender, PaintEventArgs e)
        {
            if (tileImage != null)
                e.Graphics.DrawImage(tileImage, 0, 0);

            if (buttonToggleCartGrid.Checked)
            {
                Color c = Color.Gray;
                Pen p = new Pen(new SolidBrush(c));
                Point h = new Point();
                Point v = new Point();
                for (h.Y = 16; h.Y < 32; h.Y += 16)
                    e.Graphics.DrawLine(p, h, new Point(h.X + 256, h.Y));
                for (v.X = 16; v.X < 32; v.X += 16)
                    e.Graphics.DrawLine(p, v, new Point(v.X, v.Y + 256));
            }
        }
        private void pictureBoxGraphicSet_MouseClick(object sender, MouseEventArgs e)
        {
            clickedSubTile = ((e.Y - (e.Y % 16)) + (e.X / 16)); // Calculate tile number
        }
        private void pictureBoxGraphicSet_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            clickedSubTile = ((e.Y - (e.Y % 16)) + (e.X / 16)); // Calculate tile number
            tile8x8Tile.Value = clickedSubTile;
        }
        private void pictureBoxGraphicSet_Paint(object sender, PaintEventArgs e)
        {
            if (graphicSetImage != null)
                e.Graphics.DrawImage(graphicSetImage, 0, 0);

            if (buttonToggleCartGrid.Checked)
            {
                Color c = Color.Gray;
                Pen p = new Pen(new SolidBrush(c));
                Point h = new Point();
                Point v = new Point();
                for (h.Y = 16; h.Y < 256; h.Y += 16)
                    e.Graphics.DrawLine(p, h, new Point(h.X + 256, h.Y));
                for (v.X = 16; v.X < 256; v.X += 16)
                    e.Graphics.DrawLine(p, v, new Point(v.X, v.Y + 256));
            }
        }

        private void setSubtileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.tile8x8Tile.Value = clickedSubTile;
        }

        private void tileEditorUpdate_Click(object sender, EventArgs e)
        {
            if (currentLevel > 2)
            {
                tileMap = new TileMap(layer, paletteSet, tileSet, layer, prioritySets, model);
            }
            else
            {
                wmTileMap = new WorldMapTilemap(currentLevel, layer, paletteSet, wmTileSet, model);
            }

            SetLevelImage();
            SetTilesetImage();
        }

        // physical tiles
        private void physicalAirship_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (updatingPhystile) return;

            if (currentLevel == 0)
                ByteManage.SetByteBits(model.WobPhysicalMap, currentTile * 2, (byte)((byte)physicalAirship.SelectedIndex << 2), 0x0C);
            else
                ByteManage.SetByteBits(model.WorPhysicalMap, currentTile * 2, (byte)((byte)physicalAirship.SelectedIndex << 2), 0x0C);
        }
        private void physicalBattleBG_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (updatingPhystile) return;

            if (currentLevel == 0)
                ByteManage.SetByteBits(model.WobPhysicalMap, currentTile * 2 + 1, (byte)((byte)physicalBattleBG.SelectedIndex), 0x07);
            else
                ByteManage.SetByteBits(model.WorPhysicalMap, currentTile * 2 + 1, (byte)((byte)physicalBattleBG.SelectedIndex), 0x07);
        }
        private void physicalProperties_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (updatingPhystile) return;

            if (currentLevel == 0)
            {
                ByteManage.SetBit(model.WobPhysicalMap, currentTile * 2, 0, physicalProperties.GetItemChecked(0));
                ByteManage.SetBit(model.WobPhysicalMap, currentTile * 2, 1, physicalProperties.GetItemChecked(1));
                ByteManage.SetBit(model.WobPhysicalMap, currentTile * 2, 4, physicalProperties.GetItemChecked(2));
                ByteManage.SetBit(model.WobPhysicalMap, currentTile * 2, 5, physicalProperties.GetItemChecked(3));
                ByteManage.SetBit(model.WobPhysicalMap, currentTile * 2, 6, physicalProperties.GetItemChecked(4));
            }
            else
            {
                ByteManage.SetBit(model.WorPhysicalMap, currentTile * 2, 0, physicalProperties.GetItemChecked(0));
                ByteManage.SetBit(model.WorPhysicalMap, currentTile * 2, 1, physicalProperties.GetItemChecked(1));
                ByteManage.SetBit(model.WorPhysicalMap, currentTile * 2, 4, physicalProperties.GetItemChecked(2));
                ByteManage.SetBit(model.WorPhysicalMap, currentTile * 2, 5, physicalProperties.GetItemChecked(3));
                ByteManage.SetBit(model.WorPhysicalMap, currentTile * 2, 6, physicalProperties.GetItemChecked(4));
            }
            if (state.PhysicalLayer)
            {
                pictureBoxTilesetL1.Invalidate();
                pictureBoxLevel.Invalidate();
            }
        }
        private void physicalUnknown_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (updatingPhystile) return;

            if (currentLevel == 0)
            {
                ByteManage.SetBit(model.WobPhysicalMap, currentTile * 2, 7, physicalUnknown.GetItemChecked(0));
                ByteManage.SetBit(model.WobPhysicalMap, currentTile * 2 + 1, 3, physicalUnknown.GetItemChecked(1));
                ByteManage.SetBit(model.WobPhysicalMap, currentTile * 2 + 1, 4, physicalUnknown.GetItemChecked(2));
                ByteManage.SetBit(model.WobPhysicalMap, currentTile * 2 + 1, 5, physicalUnknown.GetItemChecked(3));
                ByteManage.SetBit(model.WobPhysicalMap, currentTile * 2 + 1, 6, physicalUnknown.GetItemChecked(4));
                ByteManage.SetBit(model.WobPhysicalMap, currentTile * 2 + 1, 7, physicalUnknown.GetItemChecked(5));
            }
            else
            {
                ByteManage.SetBit(model.WorPhysicalMap, currentTile * 2, 7, physicalUnknown.GetItemChecked(0));
                ByteManage.SetBit(model.WorPhysicalMap, currentTile * 2 + 1, 3, physicalUnknown.GetItemChecked(1));
                ByteManage.SetBit(model.WorPhysicalMap, currentTile * 2 + 1, 4, physicalUnknown.GetItemChecked(2));
                ByteManage.SetBit(model.WorPhysicalMap, currentTile * 2 + 1, 5, physicalUnknown.GetItemChecked(3));
                ByteManage.SetBit(model.WorPhysicalMap, currentTile * 2 + 1, 6, physicalUnknown.GetItemChecked(4));
                ByteManage.SetBit(model.WorPhysicalMap, currentTile * 2 + 1, 7, physicalUnknown.GetItemChecked(5));
            }
        }

        private void physicalSolidTile_CheckedChanged(object sender, EventArgs e)
        {
            physicalSolidTile.ForeColor = physicalSolidTile.Checked ? Color.Black : SystemColors.ControlDark;

            if (updatingPhystile) return;

            model.EditPhysicalMaps[layer.PhysicalMap] = true;
            ByteManage.SetBit(model.PhysicalMaps[layer.PhysicalMap], currentTile, 2, physicalSolidTile.Checked);
        }
        private void physicalStairs_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (updatingPhystile) return;

            model.EditPhysicalMaps[layer.PhysicalMap] = true;
            ByteManage.SetByteBits(model.PhysicalMaps[layer.PhysicalMap], currentTile, (byte)(physicalStairs.SelectedIndex << 6), 0xC0);
        }
        private void physicalTileProperties_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (updatingPhystile) return;

            model.EditPhysicalMaps[layer.PhysicalMap] = true;
            ByteManage.SetBit(model.PhysicalMaps[layer.PhysicalMap], currentTile + 0x100, 0, physicalTileProperties.GetItemChecked(0));
            ByteManage.SetBit(model.PhysicalMaps[layer.PhysicalMap], currentTile + 0x100, 1, physicalTileProperties.GetItemChecked(1));
            ByteManage.SetBit(model.PhysicalMaps[layer.PhysicalMap], currentTile + 0x100, 2, physicalTileProperties.GetItemChecked(2));
            ByteManage.SetBit(model.PhysicalMaps[layer.PhysicalMap], currentTile + 0x100, 3, physicalTileProperties.GetItemChecked(3));
            ByteManage.SetBit(model.PhysicalMaps[layer.PhysicalMap], currentTile + 0x100, 6, physicalTileProperties.GetItemChecked(4));
        }
        private void physicalOtherBits_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (updatingPhystile) return;

            model.EditPhysicalMaps[layer.PhysicalMap] = true;
            ByteManage.SetBit(model.PhysicalMaps[layer.PhysicalMap], currentTile, 0, physicalOtherBits.GetItemChecked(0));
            ByteManage.SetBit(model.PhysicalMaps[layer.PhysicalMap], currentTile, 1, physicalOtherBits.GetItemChecked(1));
            ByteManage.SetBit(model.PhysicalMaps[layer.PhysicalMap], currentTile, 3, physicalOtherBits.GetItemChecked(2));
            ByteManage.SetBit(model.PhysicalMaps[layer.PhysicalMap], currentTile, 4, physicalOtherBits.GetItemChecked(3));
            ByteManage.SetBit(model.PhysicalMaps[layer.PhysicalMap], currentTile, 5, physicalOtherBits.GetItemChecked(4));
            ByteManage.SetBit(model.PhysicalMaps[layer.PhysicalMap], currentTile + 0x100, 4, physicalOtherBits.GetItemChecked(5));
            ByteManage.SetBit(model.PhysicalMaps[layer.PhysicalMap], currentTile + 0x100, 5, physicalOtherBits.GetItemChecked(6));
            ByteManage.SetBit(model.PhysicalMaps[layer.PhysicalMap], currentTile + 0x100, 7, physicalOtherBits.GetItemChecked(7));
        }

        #endregion
    }
}

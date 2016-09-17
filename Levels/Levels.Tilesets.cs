using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace FF3LE
{
    public partial class Levels
    {
        private Tileset tileSet;
        private WorldMapTileset wmTileSet;
        private int[] tileSetPixels;
        private Bitmap tileSetImage;

        private void ResetTileReplace()
        {
            replaceTile = 0;
            replaceWith = 0;
            replaceChoose = false;
            replaceSet = false;
            label102.BackColor = SystemColors.ControlDark;
            label102.ForeColor = SystemColors.Control;
            label102.Text = "TILESETS";
        }
        private void pictureBoxTilesetL1_MouseDown(object sender, MouseEventArgs e)
        {
            if (replaceChoose)
            {
                replaceTile = e.Y / 16 * 16 + (e.X / 16);
                replaceChoose = false;
                replaceSet = true;
                label102.Text = "SELECT TILE # TO REPLACE TILE #" + replaceTile.ToString() + " WITH";
                return;
            }
            else if (replaceSet)
            {
                replaceWith = e.Y / 16 * 16 + (e.X / 16);

                if (replaceWith != replaceTile)
                {
                    switch (currentLevel)
                    {
                        case 0:
                            model.EditWobTileMap = true;
                            for (int i = 0; i < model.WobTileMap.Length; i++)
                            {
                                if (model.WobTileMap[i] == replaceTile)
                                    model.WobTileMap[i] = (byte)replaceWith;
                            }
                            wmTileMap = new WorldMapTilemap(currentLevel, layer, paletteSet, wmTileSet, model);
                            break;
                        case 1:
                            model.EditWorTileMap = true;
                            for (int i = 0; i < model.WorTileMap.Length; i++)
                            {
                                if (model.WorTileMap[i] == replaceTile)
                                    model.WorTileMap[i] = (byte)replaceWith;
                            }
                            wmTileMap = new WorldMapTilemap(currentLevel, layer, paletteSet, wmTileSet, model);
                            break;
                        case 2:
                            model.EditStTileMap = true;
                            for (int i = 0; i < model.StTileMap.Length; i++)
                            {
                                if (model.StTileMap[i] == replaceTile)
                                    model.StTileMap[i] = (byte)replaceWith;
                            }
                            wmTileMap = new WorldMapTilemap(currentLevel, layer, paletteSet, wmTileSet, model);
                            break;
                        default:
                            model.EditTileMaps[layer.TileMapL1] = true;
                            for (int i = 0; i < model.TileMaps[layer.TileMapL1].Length; i++)
                            {
                                if (model.TileMaps[layer.TileMapL1][i] == replaceTile)
                                    model.TileMaps[layer.TileMapL1][i] = (byte)replaceWith;
                            }
                            tileMap = new TileMap(layer, paletteSet, tileSet, layer, prioritySets, model);
                            break;
                    }
                }

                ResetTileReplace();
                SetLevelImage();

                return;
            }

            pictureBoxTilesetL1.Focus();
            TileSetDown(e);

            InitializeTile();
            pictureBoxTilesetL1.Invalidate();
        }
        private void pictureBoxTilesetL1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left || e.Button == MouseButtons.Right)
            {
                overlay.TileSetDragStop = new Point(e.X / 16 * 16 + 16, e.Y / 16 * 16 + 16);
                drawBufWidth = GetTileSetSelection(ref this.drawBuf);
            }

            mouse = e.Location;
            pictureBoxTilesetL1.Invalidate();
        }
        private void pictureBoxTilesetL1_MouseUp(object sender, MouseEventArgs e)
        {
        }
        private void pictureBoxTilesetL1_Paint(object sender, PaintEventArgs e)
        {
            Rectangle rdst = new Rectangle(0, 0, 256, 256);

            if (currentLevel > 2 && state.BG)
                e.Graphics.FillRectangle(new SolidBrush(Color.Black), rdst);

            if (tileSetImage != null)
                e.Graphics.DrawImage(tileSetImage, 0, 0);

            if (state.PhysicalLayer)
            {
                if (currentLevel > 2)
                    overlay.DrawPhysicalTileset(e.Graphics, tileSet);
                else
                    overlay.DrawPhysicalTileset(e.Graphics, wmTileSet);
            }

            if (state.CartesianGrid)
                overlay.DrawCartographicGrid(e.Graphics, Color.Gray, new Size(256, 256), new Size(16, 16), 1);

            overlay.DrawSelectionBox(e.Graphics, overlay.TileSetDragStop, overlay.TileSetDragStart, 1);
        }

        private void pictureBoxTilesetL2_MouseDown(object sender, MouseEventArgs e)
        {
            if (replaceChoose)
            {
                replaceTile = e.Y / 16 * 16 + (e.X / 16);
                replaceChoose = false;
                replaceSet = true;
                label102.Text = "SELECT TILE # TO REPLACE TILE #" + replaceTile.ToString() + " WITH";
                return;
            }
            else if (replaceSet)
            {
                replaceWith = e.Y / 16 * 16 + (e.X / 16);

                if (replaceWith != replaceTile)
                {
                    model.EditTileMaps[layer.TileMapL2] = true;
                    for (int i = 0; i < model.TileMaps[layer.TileMapL2].Length; i++)
                    {
                        if (model.TileMaps[layer.TileMapL2][i] == replaceTile)
                            model.TileMaps[layer.TileMapL2][i] = (byte)replaceWith;
                    }
                    tileMap = new TileMap(layer, paletteSet, tileSet, layer, prioritySets, model);
                }
                ResetTileReplace();

                SetLevelImage();

                return;
            }

            pictureBoxTilesetL2.Focus();
            TileSetDown(e);

            InitializeTile();
            pictureBoxTilesetL2.Invalidate();
        }
        private void pictureBoxTilesetL2_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left || e.Button == MouseButtons.Right)
            {
                overlay.TileSetDragStop = new Point(e.X / 16 * 16 + 16, e.Y / 16 * 16 + 16);
                drawBufWidth = GetTileSetSelection(ref this.drawBuf);
            }

            mouse = e.Location;
            pictureBoxTilesetL2.Invalidate();
        }
        private void pictureBoxTilesetL2_MouseUp(object sender, MouseEventArgs e)
        {
        }
        private void pictureBoxTilesetL2_Paint(object sender, PaintEventArgs e)
        {
            Rectangle rdst = new Rectangle(0, 0, 256, 256);

            if (state.BG)
                e.Graphics.FillRectangle(new SolidBrush(Color.Black), rdst);

            if (tileSetImage != null)
                e.Graphics.DrawImage(tileSetImage, 0, 0);

            if (state.CartesianGrid)
                overlay.DrawCartographicGrid(e.Graphics, Color.Gray, new Size(256, 256), new Size(16, 16), 1);

            overlay.DrawSelectionBox(e.Graphics, overlay.TileSetDragStop, overlay.TileSetDragStart, 1);
        }

        private void pictureBoxTilesetL3_MouseDown(object sender, MouseEventArgs e)
        {
            if (replaceChoose)
            {
                replaceTile = e.Y / 16 * 16 + (e.X / 16);
                replaceChoose = false;
                replaceSet = true;
                label102.Text = "SELECT TILE # TO REPLACE TILE #" + replaceTile.ToString() + " WITH";
                return;
            }
            else if (replaceSet)
            {
                replaceWith = e.Y / 16 * 16 + (e.X / 16);

                if (replaceWith != replaceTile)
                {
                    model.EditTileMaps[layer.TileMapL3] = true;
                    for (int i = 0; i < model.TileMaps[layer.TileMapL3].Length; i++)
                    {
                        if (model.TileMaps[layer.TileMapL3][i] == replaceTile)
                            model.TileMaps[layer.TileMapL3][i] = (byte)replaceWith;
                    }
                    tileMap = new TileMap(layer, paletteSet, tileSet, layer, prioritySets, model);
                }
                ResetTileReplace();

                SetLevelImage();

                return;
            }

            pictureBoxTilesetL3.Focus();
            TileSetDown(e);
            pictureBoxTilesetL3.Invalidate();
        }
        private void pictureBoxTilesetL3_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left || e.Button == MouseButtons.Right)
            {
                overlay.TileSetDragStop = new Point(e.X / 16 * 16 + 16, e.Y / 16 * 16 + 16);
                drawBufWidth = GetTileSetSelection(ref this.drawBuf);
            }

            mouse = e.Location;
            pictureBoxTilesetL3.Invalidate();
        }
        private void pictureBoxTilesetL3_MouseUp(object sender, MouseEventArgs e)
        {
        }
        private void pictureBoxTilesetL3_Paint(object sender, PaintEventArgs e)
        {
            Rectangle rdst = new Rectangle(0, 0, 256, 256);

            if (state.BG)
                e.Graphics.FillRectangle(new SolidBrush(Color.Black), rdst);

            if (tileSetImage != null)
                e.Graphics.DrawImage(tileSetImage, 0, 0);

            if (state.CartesianGrid)
                overlay.DrawCartographicGrid(e.Graphics, Color.Gray, new Size(256, 256), new Size(16, 16), 1);

            overlay.DrawSelectionBox(e.Graphics, overlay.TileSetDragStop, overlay.TileSetDragStart, 1);
        }
    }
}

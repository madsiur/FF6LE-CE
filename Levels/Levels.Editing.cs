using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using FF3LE.Undo;

namespace FF3LE
{
    public partial class Levels
    {
        private int copyWidth = 0;
        private int[] copyPaste = null; // Acts as a storage for the copied/cut/paste features
        private int[] drawBuf = new int[] { 0 };
        private int drawBufWidth = 1;

        // for when moving selections, these will preserve the originals
        private int[] copyPasteBuf = null;
        private int[] drawBufBuf = new int[] { 0 };
        private int drawBufWidthBuf = 1;

        private void TileSetDown(MouseEventArgs e)
        {
            int x = e.X / 16 * 16;
            int y = e.Y / 16 * 16;

            overlay.TileSetDragStart = new Point(x, y);
            overlay.TileSetDragStop = new Point(x + 16, y + 16);

            drawBufWidth = GetTileSetSelection(ref this.drawBuf);

            x /= 16; y /= 16;
            overlay.TileSelected = y * 16 + x; // Calculate tile number

            labelTileNum.Text = "(" + overlay.TileSelected + ")  Tile #";
        }
        private int GetTileSetSelection(ref int[] dest)
        {
            int dx = (overlay.TileSetDragStop.X - overlay.TileSetDragStart.X) / 16;
            int dy = (overlay.TileSetDragStop.Y - overlay.TileSetDragStart.Y) / 16;

            dest = new int[dx * dy];
            int entry;

            for (int i = 0; i < dest.Length; i++)
            {
                if (currentLevel > 2)
                    entry = tileSet.GetTileNumber(tabControl2.SelectedIndex, (i % dx) + (overlay.TileSetDragStart.X / 16), (i / dx) + (overlay.TileSetDragStart.Y / 16));
                else
                    entry = wmTileSet.GetTileNumber((i % dx) + (overlay.TileSetDragStart.X / 16), (i / dx) + (overlay.TileSetDragStart.Y / 16));

                dest[i] = entry;
            }

            return dx;
        }

        private void MakeEditDraw(MouseEventArgs e, Graphics g)
        {
            try
            {
                if (currentLevel > 2)
                {
                    if (tileMap.GetTileNum(tabControl2.SelectedIndex, e.X, e.Y) == overlay.TileSelected &&
                        Math.Abs(overlay.TileSetDragStop.X - overlay.TileSetDragStart.X) == 16 &&
                        Math.Abs(overlay.TileSetDragStop.Y - overlay.TileSetDragStart.Y) == 16)
                        return; // We are overwriting the same tile over itself, no need to make an edit
                }
                else
                {
                    if (wmTileMap.GetTileNum(e.X, e.Y) == overlay.TileSelected &&
                        Math.Abs(overlay.TileSetDragStop.X - overlay.TileSetDragStart.X) == 16 &&
                        Math.Abs(overlay.TileSetDragStop.Y - overlay.TileSetDragStart.Y) == 16)
                        return; // We are overwriting the same tile over itself, no need to make an edit
                }

                Point tL = new Point(e.X, e.Y);
                Point bR = new Point(e.X + (drawBufWidth * 16), e.Y + ((drawBuf.Length / drawBufWidth) * 16));

                if (currentLevel > 3)
                {
                    switch (tabControl2.SelectedIndex)
                    {
                        case 0: model.EditTileMaps[layer.TileMapL1] = true; break;
                        case 1: model.EditTileMaps[layer.TileMapL2] = true; break;
                        case 2: model.EditTileMaps[layer.TileMapL3] = true; break;
                    }
                    commandStack.Push(new TileMapEditCommand(this, tileMap, tabControl2.SelectedIndex, tL, bR, drawBuf, false));
                }
                else
                {
                    switch (currentLevel)
                    {
                        case 0: model.EditWobTileMap = true; break;
                        case 1: model.EditWorTileMap = true; break;
                        case 2: model.EditStTileMap = true; break;
                    }
                    commandStack.Push(new TileMapEditCommand(this, wmTileMap, tL, bR, drawBuf, false));
                }

                Point p = new Point(tile.X * 16, tile.Y * 16);
                Size s = new Size();
                s.Width = overlay.TileSetDragStop.X - overlay.TileSetDragStart.X;
                s.Height = overlay.TileSetDragStop.Y - overlay.TileSetDragStart.Y;

                Bitmap image;
                if (currentLevel > 2)
                    image = DrawImageFromIntArr(tileMap.GetRangePixels(p, s), s.Width, s.Height);
                else
                    image = DrawImageFromIntArr(wmTileMap.GetRangePixels(p, s), s.Width, s.Height);

                p.X = tile.X * (int)(16 * zoom);
                p.Y = tile.Y * (int)(16 * zoom);
                Rectangle rsrc = new Rectangle(0, 0, image.Width, image.Height);
                Rectangle rdst = new Rectangle(p.X, p.Y, (int)(image.Width * zoom), (int)(image.Height * zoom));

                g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.Half;
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                g.DrawImage(image, rdst, rsrc, GraphicsUnit.Pixel);
            }
            catch
            {
                // No command
            }
        }
        private void MakeEditErase(MouseEventArgs e, Graphics g)
        {
            if (currentLevel > 2)
            {
                if (tileMap.GetTileNum(tabControl2.SelectedIndex, e.X, e.Y) == 0)
                    return; // We are overwriting the same tile over itself, no need to make an edit
            }
            else
            {
                if (wmTileMap.GetTileNum(e.X, e.Y) == (currentLevel == 2 ? 48 : 6))
                    return; // We are overwriting the same tile over itself, no need to make an edit
            }

            Point tL = new Point(e.X, e.Y);
            Point bR = new Point(e.X + 16, e.Y + 16);

            if (currentLevel > 2)
            {
                switch (tabControl2.SelectedIndex)
                {
                    case 0: model.EditTileMaps[layer.TileMapL1] = true; break;
                    case 1: model.EditTileMaps[layer.TileMapL2] = true; break;
                    case 2: model.EditTileMaps[layer.TileMapL3] = true; break;
                }
                commandStack.Push(new TileMapEditCommand(this, tileMap, tabControl2.SelectedIndex, tL, bR, new int[] { 0 }, false));
            }
            else
            {
                switch (currentLevel)
                {
                    case 0: model.EditWobTileMap = true; break;
                    case 1: model.EditWorTileMap = true; break;
                    case 2: model.EditStTileMap = true; break;
                }
                commandStack.Push(new TileMapEditCommand(this, wmTileMap, tL, bR, new int[] { currentLevel == 2 ? 48 : 6 }, false));
            }

            Point p = new Point(tile.X * 16, tile.Y * 16);

            Bitmap image;
            if (currentLevel > 2)
                image = DrawImageFromIntArr(tileMap.GetRangePixels(p, new Size(16, 16)), 16, 16);
            else
                image = DrawImageFromIntArr(wmTileMap.GetRangePixels(p, new Size(16, 16)), 16, 16);

            p.X = tile.X * (int)(16 * zoom);
            p.Y = tile.Y * (int)(16 * zoom);
            Rectangle rsrc = new Rectangle(0, 0, image.Width, image.Height);
            Rectangle rdst = new Rectangle(p.X, p.Y, (int)(image.Width * zoom), (int)(image.Height * zoom));

            g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.Half;
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            g.DrawImage(image, rdst, rsrc, GraphicsUnit.Pixel);
        }
        private void MakeEditDelete()
        {
            Point tL = overlay.DragStart;
            Point bR = overlay.DragStop;
            int layer = tabControl2.SelectedIndex;
            int[] changes = new int[(bR.X - tL.X) * (bR.Y - tL.Y)];

            if (currentLevel < 3)
                for (int i = 0; i < changes.Length; i++)
                    changes[i] = currentLevel == 2 ? 48 : 6;

            try
            {
                // Verify layer before creating command
                if (currentLevel > 2)
                {
                    switch (tabControl2.SelectedIndex)
                    {
                        case 0: model.EditTileMaps[this.layer.TileMapL1] = true; break;
                        case 1: model.EditTileMaps[this.layer.TileMapL2] = true; break;
                        case 2: model.EditTileMaps[this.layer.TileMapL3] = true; break;
                    }
                    commandStack.Push(new TileMapEditCommand(this, tileMap, tabControl2.SelectedIndex, tL, bR, changes, false));
                }
                else
                {
                    switch (currentLevel)
                    {
                        case 0: model.EditWobTileMap = true; break;
                        case 1: model.EditWorTileMap = true; break;
                        case 2: model.EditStTileMap = true; break;
                    }
                    commandStack.Push(new TileMapEditCommand(this, wmTileMap, tL, bR, changes, false));
                }
            }
            catch
            {
                // No layer 3
            }

            SetLevelImage();
        }
        private void MakeEditMove(Graphics g)
        {
            Point p = new Point((int)(overlay.DragStart.X * zoom), (int)(overlay.DragStart.Y * zoom));
            Size s = overlay.SelectionSize;
            Rectangle rsrc = new Rectangle(0, 0, s.Width, s.Height);
            Rectangle rdst = new Rectangle(p.X, p.Y, (int)(zoom * s.Width), (int)(zoom * s.Height));

            g.DrawImage(new Bitmap(moveImage), rdst, rsrc, GraphicsUnit.Pixel);
        }

        private void Paste(Point p)
        {
            int layer = tabControl2.SelectedIndex;
            Point tL = p;
            Point bR;
            try
            {
                bR = new Point(p.X + (copyWidth * 16), p.Y + ((copyPaste.Length / copyWidth) * 16));
            }
            catch
            { return; }

            try
            {
                // Check layer before doing this!
                if (currentLevel > 2)
                {
                    switch (tabControl2.SelectedIndex)
                    {
                        case 0: model.EditTileMaps[this.layer.TileMapL1] = true; break;
                        case 1: model.EditTileMaps[this.layer.TileMapL2] = true; break;
                        case 2: model.EditTileMaps[this.layer.TileMapL3] = true; break;
                    }
                    commandStack.Push(new TileMapEditCommand(this, tileMap, layer, tL, bR, copyPaste, true));
                }
                else
                {
                    switch (currentLevel)
                    {
                        case 0: model.EditWobTileMap = true; break;
                        case 1: model.EditWorTileMap = true; break;
                        case 2: model.EditStTileMap = true; break;
                    }
                    commandStack.Push(new TileMapEditCommand(this, wmTileMap, tL, bR, copyPaste, true));
                }
            }
            catch
            {
                return;
            }

            SetLevelImage();
        }
        private void PasteFinal()
        {
            if (state.Move)
            {
                Paste(new Point(overlay.DragStart.X, overlay.DragStart.Y));

                copyPaste = copyPasteBuf;
                drawBuf = drawBufBuf;
                drawBufWidth = drawBufWidthBuf;

                state.Move = false;
            }
        }
        private void Cut()
        {
            Copy();
            MakeEditDelete();
        }
        private void Copy()
        {
            Size s = overlay.SelectionSize;

            if (currentLevel > 2)
                moveImage = new Bitmap(DrawImageFromIntArr(tileMap.GetRangePixels(tabControl2.SelectedIndex, overlay.DragStart, s), s.Width, s.Height));
            else
                moveImage = new Bitmap(DrawImageFromIntArr(wmTileMap.GetRangePixels(overlay.DragStart, s), s.Width, s.Height));

            try
            {
                GetSelectionIntArr(overlay.DragStart, overlay.DragStop, tabControl2.SelectedIndex);
            }
            catch
            {
                // No layer 3
                return;
            }

            copyPasteBuf = copyPaste;
            drawBufBuf = drawBuf;
            drawBufWidthBuf = drawBufWidth;
        }

        private void Undo()
        {
            commandStack.UndoCommand();

            SetLevelImage();
        }
        private void Redo()
        {
            commandStack.RedoCommand();

            SetLevelImage();
        }

        private void GetSelectionIntArr(Point tL, Point bR, int layer)
        {
            int xDiff = (bR.X / 16 - tL.X / 16);
            int yDiff = (bR.Y / 16 - tL.Y / 16);

            int tileX, tileY;

            copyPaste = new int[xDiff * yDiff];
            copyWidth = xDiff;

            for (int y = 0; y < yDiff; y++)
            {
                for (int x = 0; x < xDiff; x++)
                {
                    tileX = tL.X + (x * 16);
                    tileY = tL.Y + (y * 16);

                    if (currentLevel > 2)
                        copyPaste[x + y * xDiff] = tileMap.GetTileNum(layer, tileX, tileY);
                    else
                        copyPaste[x + y * xDiff] = wmTileMap.GetTileNum(tileX, tileY);
                }
            }
        }
    }
}

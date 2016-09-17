using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using FF3LE.Properties;

namespace FF3LE
{
    class Overlay
    {
        private State state;
        public int bfX = 0;
        public int bfY = 0;
        public int alpha = 255;

        private int tileSelected = 0; public int TileSelected { get { return tileSelected; } set { tileSelected = value; } }
        private int mapTileSelected = 0; public int MapTileSelected { get { return mapTileSelected; } set { mapTileSelected = value; } }
        public bool WorldMap = true;

        private Point dragStart, dragStop, tileSetDragStart, tileSetDragStop;
        public Point DragStart
        {
            get { return GetTopLeft(dragStart, dragStop); }
            set { dragStart = WorldMap ? WithinBounds(value, 4096, 4096) : WithinBounds(value, 2048, 2048); }
        }
        public Point DragStop
        {
            get { return GetBottomRight(dragStart, dragStop); }
            set { dragStop = WorldMap ? WithinBounds(value, 4096, 4096) : WithinBounds(value, 2048, 2048); }
        }
        public Point TileSetDragStart
        {
            get { return GetTopLeft(tileSetDragStart, tileSetDragStop); }
            set { tileSetDragStart = WithinBounds(value, 256, 256); }
        }
        public Point TileSetDragStop
        {
            get { return GetBottomRight(tileSetDragStart, tileSetDragStop); }
            set { tileSetDragStop = WithinBounds(value, 256, 256); }
        }
        private bool clearSelection = false;

        public Size SelectionSize;

        public bool ClearSelection { get { return clearSelection; } set { clearSelection = value; } }

        private Bitmap npcsImage;
        public Bitmap NPCsImage { get { return npcsImage; } set { npcsImage = value; } }

        private Point origHoverStart; public Point OrigHoverStart { get { return origHoverStart; } set { origHoverStart = value; } }   // the point of the image to redraw
        private Size origHoverSize; public Size OrigHoverSize { get { return origHoverSize; } set { origHoverSize = value; } }     // the size of the image to redraw

        public Overlay()
        {
            state = State.Instance;
        }

        private void CopyToArray(int[] source, int[] dest)
        {
            for (int i = 0; i < source.Length; i++)
                if (source[i] != 0)
                    dest[i] = source[i];
        }

        public Point WithinBounds(Point p, int maxX, int maxY)
        {
            if (p.X > maxX)
                p.X = maxX;
            if (p.Y > maxY)
                p.Y = maxY;
            if (p.X < 0)
                p.X = 0;
            if (p.Y < 0)
                p.Y = 0;

            return p;
        }
        private Point GetTopLeft(Point start, Point end)
        {
            int sx, sy, ex, ey;
            sx = start.X;
            sy = start.Y;
            ex = end.X;
            ey = end.Y;

            // Case 1, s.x < e.x && s.y < e.y
            // Do nothing, were fine
            // Case 2, s.x > e.x && s.y > e.y
            if (sx > ex && sy > ey)
            {
                Swap(ref sx, ref ex);
                Swap(ref sy, ref ey);
            }
            // Case 3, s.x < e.x && s.y > e.y
            if (sx < ex && sy > ey)
            {
                Swap(ref sy, ref ey);
            }
            // Case 4, s.x > e.x && s.y < e.y
            if (sx > ex && sy < ey)
            {
                Swap(ref sx, ref ex);
            }

            return new Point(sx, sy);
        }
        private Point GetBottomRight(Point start, Point end)
        {
            int sx, sy, ex, ey;
            sx = start.X;
            sy = start.Y;
            ex = end.X;
            ey = end.Y;

            // Case 1, s.x < e.x && s.y < e.y
            // Do nothing, were fine
            // Case 2, s.x > e.x && s.y > e.y
            if (sx > ex && sy > ey)
            {
                Swap(ref sx, ref ex);
                Swap(ref sy, ref ey);
            }
            // Case 3, s.x < e.x && s.y > e.y
            if (sx < ex && sy > ey)
            {
                Swap(ref sy, ref ey);
            }
            // Case 4, s.x > e.x && s.y < e.y
            if (sx > ex && sy < ey)
            {
                Swap(ref sx, ref ex);
            }

            return new Point(ex, ey);
        }

        private void Swap(ref int a, ref int b)
        {
            int temp;
            temp = a;
            a = b;
            b = temp;
        }
        public void ClearArray(IList arr)
        {
            if (arr == null)
                return;
            arr.Clear();
        }

        // "Size s" is the size of the control, "Size u" is the distance between grid lines
        public void DrawCartographicGrid(Graphics g, Color c, Size s, Size u, double z)
        {
            c = Color.FromArgb(alpha, c);
            Pen p = new Pen(new SolidBrush(c));
            Point h = new Point();
            Point v = new Point();
            for (h.Y = (int)(z * u.Height); h.Y < s.Height; h.Y += (int)(z * u.Height))
                g.DrawLine(p, h, new Point(h.X + s.Width, h.Y));
            for (v.X = (int)(z * u.Width); v.X < s.Width; v.X += (int)(z * u.Width))
                g.DrawLine(p, v, new Point(v.X, v.Y + s.Height));
        }
        public void DrawLevelMask(Graphics g, Point stop, Point start, double z)
        {
            if (start.X == stop.X || start.Y == stop.Y) return;

            Point p = new Point((int)(start.X * 16 * z), (int)(start.Y * 16 * z));
            Size s = new Size((int)((stop.X - start.X) * (int)(16 * z)), (int)((stop.Y - start.Y) * (int)(16 * z)));
            Pen n = new Pen(new SolidBrush(Color.Orange), (int)(2 * z)); n.Alignment = PenAlignment.Inset;
            Rectangle r = new Rectangle(p, s);
            if (r.Right >= 2048 - 1 * z) r.Width = (int)(2048 - 2 * z);
            if (r.Bottom >= 2048 - 1 * z) r.Height = (int)(2048 - 2 * z);
            g.DrawRectangle(n, r);
        }
        public void DrawVisibleBounds(Graphics g, Point start, Point mask, double z)
        {
            if (mask.X == 0) mask.X = 256;
            if (mask.Y == 0) mask.Y = 256;
            Pen border = new Pen(Color.FromArgb(128, 128, 128), (int)(8 * z));
            Rectangle bounds = new Rectangle();

            bounds.X = Math.Max(Math.Min(((start.X - 7) * 16) - 8, mask.X * 16 - 256), 0);
            bounds.X = (int)(bounds.X * z);

            bounds.Y = Math.Max(Math.Min(((start.Y - 7) * 16), mask.Y * 16 - 224), 0);
            bounds.Y = (int)(bounds.Y * z);

            bounds.Width = (int)(256 * z);
            bounds.Height = (int)(224 * z);

            border.Alignment = System.Drawing.Drawing2D.PenAlignment.Inset;

            g.DrawRectangle(border, bounds);

            SolidBrush darkness = new SolidBrush(Color.FromArgb(128, Color.Black));
            Rectangle dark = new Rectangle();

            dark.X = (int)g.ClipBounds.X;
            dark.Y = (int)g.ClipBounds.Y;
            dark.Width = (int)g.ClipBounds.Width;
            dark.Height = bounds.Y - (int)g.ClipBounds.Y;
            g.FillRectangle(darkness, dark);

            dark.Y = bounds.Y;
            dark.Width = bounds.X - (int)g.ClipBounds.X;
            dark.Height = bounds.Height;
            g.FillRectangle(darkness, dark);

            dark.X = bounds.Right;
            dark.Width = (int)g.ClipBounds.Right - bounds.Right;
            g.FillRectangle(darkness, dark);

            dark.X = (int)g.ClipBounds.X;
            dark.Y = bounds.Y + bounds.Height;
            dark.Width = (int)g.ClipBounds.Width;
            dark.Height = (int)g.ClipBounds.Bottom - bounds.Bottom;
            g.FillRectangle(darkness, dark);
        }
        public void DrawZoneGrid(Graphics g, Color c, Size s, Size u, double z)
        {
            c = Color.FromArgb(alpha, c);
            Pen p = new Pen(new SolidBrush(c));
            Point h = new Point();
            Point v = new Point();
            for (h.Y = (int)(z * u.Height); h.Y < s.Height; h.Y += (int)(z * u.Height))
                g.DrawLine(p, h, new Point(h.X + s.Width, h.Y));
            for (v.X = (int)(z * u.Width); v.X < s.Width; v.X += (int)(z * u.Width))
                g.DrawLine(p, v, new Point(v.X, v.Y + s.Height));

            Font zoneFont = new Font("Tahoma", 8.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            for (int y = 0; y < 8; y++)
            {
                for (int x = 0; x < 8; x++)
                {
                    g.DrawString("ZONE #" + (y * 8 + x), zoneFont, new SolidBrush(c), new PointF(x * (int)(512 * z) + 8, y * (int)(512 * z) + 8));
                }
            }
        }
        public void DrawSelectionBox(Graphics g, Point stop, Point start, double z)
        {
            if (stop.X == start.X) return;
            if (stop.Y == start.Y) return;

            Point p = new Point((int)(start.X * z), (int)(start.Y * z));
            Size s = new Size((int)(stop.X * z) - (int)(start.X * z) - 1, (int)(stop.Y * z) - (int)(start.Y * z) - 1);

            Pen penw = new Pen(Color.White);
            Pen penb = new Pen(Color.Black); penb.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;
            Rectangle ro = new Rectangle(p, s); p.X++; p.Y++; s.Width -= 2; s.Height -= 2;
            Rectangle ri = new Rectangle(p, s);

            g.DrawRectangle(penw, ro);
            g.DrawRectangle(penw, ri);
            g.DrawRectangle(penb, ro);
            g.DrawRectangle(penb, ri);
        }
        public void DrawHoverBox(Graphics g, Point start, double z)
        {
            SolidBrush b = new SolidBrush(Color.FromArgb(96, 0, 0, 0));
            Rectangle r;

            r = new Rectangle(new Point((int)(start.X * z), (int)(start.Y * z)), new Size((int)(16 * z), (int)(16 * z)));
            g.FillRectangle(b, r);

            origHoverStart = r.Location;
            origHoverSize = new Size(r.Width + 1, r.Height + 1);
        }

        public void DrawLevelExits(Graphics g, LevelExits exits, double z)
        {
            LevelExits.ExitShort temp;
            Rectangle r;
            Pen p = new Pen(Color.Yellow, 2);
            SolidBrush b = new SolidBrush(Color.FromArgb(128, Color.Yellow));

            for (int i = 0; i < exits.ExitsShort.Count; i++)
            {
                temp = (LevelExits.ExitShort)exits.ExitsShort[i];
                r = new Rectangle(new Point((int)(temp.CoordX * 16 * z), (int)(temp.CoordY * 16 * z)), new Size((int)(16 * z), (int)(16 * z)));
                r.X++; r.Y++;
                r.Width -= 2; r.Height -= 2;

                if (i == exits.SelectedExitShort)
                    g.FillRectangle(b, r);

                g.DrawRectangle(p, r);
            }

            LevelExits.ExitLong tmp;
            for (int i = 0; i < exits.ExitsLong.Count; i++)
            {
                tmp = (LevelExits.ExitLong)exits.ExitsLong[i];
                if (tmp.Direction == 0)
                    r = new Rectangle(new Point((int)(tmp.CoordX * 16 * z), (int)(tmp.CoordY * 16 * z)), new Size((int)((tmp.Width + 1) * 16 * z), (int)(16 * z)));
                else
                    r = new Rectangle(new Point((int)(tmp.CoordX * 16 * z), (int)(tmp.CoordY * 16 * z)), new Size((int)(16 * z), (int)((tmp.Width + 1) * 16 * z)));
                r.X++; r.Y++;
                r.Width -= 2; r.Height -= 2;

                if (i == exits.SelectedExitLong)
                    g.FillRectangle(b, r);

                g.DrawRectangle(p, r);
            }
        }
        public void DrawLevelEvents(Graphics g, LevelEvents events, double z)
        {
            LevelEvents.Event temp;
            Rectangle r;
            Pen p = new Pen(Color.FromArgb(255, 0, 255, 0), 2);
            SolidBrush b = new SolidBrush(Color.FromArgb(128, 0, 255, 0));

            for (int i = 0; i < events.Events.Count; i++)
            {
                temp = (LevelEvents.Event)events.Events[i];
                r = new Rectangle(new Point((int)(temp.CoordX * 16 * z), (int)(temp.CoordY * 16 * z)), new Size((int)(16 * z), (int)(16 * z)));
                r.X++; r.Y++;
                r.Width -= 2; r.Height -= 2;

                if (i == events.SelectedEvent)
                    g.FillRectangle(b, r);

                g.DrawRectangle(p, r);
            }
        }
        public void DrawLevelNPCs(Graphics g, LevelNPCs npcs, double z)
        {
            LevelNPCs.NPC temp;
            Rectangle r;
            Pen p = new Pen(Color.Red, 2);
            SolidBrush b = new SolidBrush(Color.FromArgb(128, Color.Red));

            Bitmap sprite;

            for (int i = 0; i < npcs.NPCs.Count; i++)
            {
                temp = (LevelNPCs.NPC)npcs.NPCs[i];

                r = new Rectangle();
                r.Location = new Point((int)(temp.CoordX * 16 * z), (int)(temp.CoordY * 16 * z));
                r.Size = new Size((int)(16 * z), (int)(32 * z));
                r.X++; r.Y -= (int)(16 * z); r.Y++;
                r.Width -= 2; r.Height -= 2;

                sprite = new Bitmap(DrawImageFromIntArr(npcs.GetSpritePixels(i), 16, 32));

                Rectangle rdst = new Rectangle((int)(r.X - z), (int)(r.Y - z), (int)(16 * z), (int)(32 * z));
                Rectangle rsrc = new Rectangle(0, 0, 16, 32);

                if (i == npcs.SelectedNPC)
                    g.FillRectangle(b, r);

                g.DrawRectangle(p, r);

                g.DrawImage(sprite, rdst, rsrc, GraphicsUnit.Pixel);
            }
        }
        public void DrawLevelTreasures(Graphics g, LevelNPCs npcs, double z)
        {
            LevelNPCs.Treasure temp;
            Rectangle r;
            Pen p = new Pen(Color.Blue, 2);
            SolidBrush b = new SolidBrush(Color.FromArgb(128, Color.Blue));

            for (int i = 0; i < npcs.Treasures.Count; i++)
            {
                temp = (LevelNPCs.Treasure)npcs.Treasures[i];

                r = new Rectangle();
                r.Location = new Point((int)(temp.CoordX * 16 * z), (int)(temp.CoordY * 16 * z));
                r.Size = new Size((int)(16 * z), (int)(16 * z));
                r.X++; r.Y++;
                r.Width -= 2; r.Height -= 2;

                if (i == npcs.SelectedTreasure)
                    g.FillRectangle(b, r);

                g.DrawRectangle(p, r);
            }
        }
        public void DrawPhysicalMap(Graphics g, LevelLayer layer, TileMap tileMap, double z)
        {
            Size s = new Size();
            SolidBrush b = new SolidBrush(Color.FromArgb(128, Color.Orange));
            Rectangle r = new Rectangle(0, 0, (int)(16 * z), (int)(16 * z));
            int tileNum, tileProp;
            bool[] quadrants = new bool[4];

            s.Width = (int)(256 * Math.Pow(2, layer.LayerWidth[0]) / 16);
            s.Height = (int)(256 * Math.Pow(2, layer.LayerHeight[0]) / 16);

            Point p = new Point();
            Size e = new Size();
            for (int y = 0; y < s.Height; y++)
            {
                for (int x = 0; x < s.Width; x++)
                {
                    r.X = (int)(x * (int)(16 * z));
                    r.Y = (int)(y * (int)(16 * z));
                    try
                    {
                        tileNum = tileMap.GetPhysicalTile(0, x * 16, y * 16, false);
                        tileProp = tileMap.GetPhysicalTile(0, x * 16, y * 16, true);
                    }
                    catch
                    {
                        return;
                    }

                    quadrants[0] = (tileProp & 0x08) != 0x08 || (tileProp & 0x02) != 0x02;  // north or west
                    quadrants[1] = (tileProp & 0x08) != 0x08 || (tileProp & 0x01) != 0x01;  // north or east
                    quadrants[2] = (tileProp & 0x04) != 0x04 || (tileProp & 0x02) != 0x02;  // south or west
                    quadrants[3] = (tileProp & 0x04) != 0x04 || (tileProp & 0x01) != 0x01;  // south or east

                    if ((tileNum & 0x04) == 0x04)
                        g.FillRectangle(b, r);
                    else if ((tileNum & 0x80) == 0x80)
                        g.FillPolygon(b,
                            new Point[] { new Point(0 + r.X, 0 + r.Y), new Point(16 + r.X, 16 + r.Y), new Point(0 + r.X, 16 + r.Y) }, FillMode.Winding);
                    else if ((tileNum & 0x40) == 0x40)
                        g.FillPolygon(b,
                            new Point[] { new Point(16 + r.X, 0 + r.Y), new Point(0 + r.X, 16 + r.Y), new Point(16 + r.X, 16 + r.Y) }, FillMode.Winding);
                    else
                    {
                        e = new Size(r.Width / 2, r.Height / 2);
                        for (int i = 0; i < 4; i++)
                        {
                            p = new Point(r.X + ((i % 2) * (int)(8 * z)), r.Y + ((i / 2) * (int)(8 * z)));
                            if (quadrants[i])
                                g.FillRectangle(new SolidBrush(Color.FromArgb(128, Color.OrangeRed)), new Rectangle(p, e));
                        }
                    }
                }
            }
        }
        public void DrawPhysicalMap(Graphics g, WorldMapTilemap tileMap, double z)
        {
            SolidBrush b = new SolidBrush(Color.FromArgb(128, Color.Orange));
            Rectangle r = new Rectangle(0, 0, (int)(16 * z), (int)(16 * z));
            int tileNum, tileProp;
            bool[] quadrants = new bool[4];

            //Point p = new Point();
            //Size e = new Size();
            for (int y = 0; y < 256; y++)
            {
                for (int x = 0; x < 256; x++)
                {
                    r.X = (int)(x * (int)(16 * z));
                    r.Y = (int)(y * (int)(16 * z));
                    try
                    {
                        tileNum = tileMap.GetPhysicalTile(x * 16, y * 16, false);
                        tileProp = tileMap.GetPhysicalTile(x * 16, y * 16, true);
                    }
                    catch
                    {
                        return;
                    }

                    quadrants[0] = (tileProp & 0x08) != 0x08 || (tileProp & 0x02) != 0x02;  // north or west
                    quadrants[1] = (tileProp & 0x08) != 0x08 || (tileProp & 0x01) != 0x01;  // north or east
                    quadrants[2] = (tileProp & 0x04) != 0x04 || (tileProp & 0x02) != 0x02;  // south or west
                    quadrants[3] = (tileProp & 0x04) != 0x04 || (tileProp & 0x01) != 0x01;  // south or east

                    if ((tileNum & 0x10) == 0x10)
                        g.FillRectangle(b, r);
                    //else
                    //{
                    //    e = new Size(r.Width / 2, r.Height / 2);
                    //    for (int i = 0; i < 4; i++)
                    //    {
                    //        p = new Point(r.X + ((i % 2) * (int)(8 * z)), r.Y + ((i / 2) * (int)(8 * z)));
                    //        if (quadrants[i])
                    //            g.FillRectangle(b, new Rectangle(p, e));
                    //    }
                    //}
                }
            }
        }
        public void DrawPhysicalTileset(Graphics g, Tileset tileSet)
        {
            SolidBrush b = new SolidBrush(Color.FromArgb(128, Color.Orange));
            Rectangle r = new Rectangle(0, 0, 16, 16);
            int tileNum, tileProp;
            bool[] quadrants = new bool[4];

            Point p = new Point();
            Size e = new Size();
            for (int y = 0; y < 16; y++)
            {
                for (int x = 0; x < 16; x++)
                {
                    r.X = x * 16;
                    r.Y = y * 16;
                    tileNum = tileSet.GetPhysicalTile(0, x, y, false);
                    tileProp = tileSet.GetPhysicalTile(0, x, y, true);

                    quadrants[0] = (tileProp & 0x08) != 0x08 || (tileProp & 0x02) != 0x02;  // north or west
                    quadrants[1] = (tileProp & 0x08) != 0x08 || (tileProp & 0x01) != 0x01;  // north or east
                    quadrants[2] = (tileProp & 0x04) != 0x04 || (tileProp & 0x02) != 0x02;  // south or west
                    quadrants[3] = (tileProp & 0x04) != 0x04 || (tileProp & 0x01) != 0x01;  // south or east

                    if ((tileNum & 0x04) == 0x04)
                        g.FillRectangle(b, r);
                    else
                    {
                        e = new Size(r.Width / 2, r.Height / 2);
                        for (int i = 0; i < 4; i++)
                        {
                            p = new Point(r.X + ((i % 2) * 8), r.Y + ((i / 2) * 8));
                            if (quadrants[i])
                                g.FillRectangle(new SolidBrush(Color.FromArgb(128, Color.OrangeRed)), new Rectangle(p, e));
                        }
                    }
                }
            }
        }
        public void DrawPhysicalTileset(Graphics g, WorldMapTileset tileSet)
        {
            SolidBrush b = new SolidBrush(Color.FromArgb(128, Color.Orange));
            Rectangle r = new Rectangle(0, 0, 16, 16);
            int tileNum, tileProp;
            bool[] quadrants = new bool[4];

            //Point p = new Point();
            //Size e = new Size();
            for (int y = 0; y < 16; y++)
            {
                for (int x = 0; x < 16; x++)
                {
                    r.X = x * 16;
                    r.Y = y * 16;
                    tileNum = tileSet.GetPhysicalTile(0, x, y, false);
                    tileProp = tileSet.GetPhysicalTile(0, x, y, true);

                    quadrants[0] = (tileProp & 0x08) != 0x08 || (tileProp & 0x02) != 0x02;  // north or west
                    quadrants[1] = (tileProp & 0x08) != 0x08 || (tileProp & 0x01) != 0x01;  // north or east
                    quadrants[2] = (tileProp & 0x04) != 0x04 || (tileProp & 0x02) != 0x02;  // south or west
                    quadrants[3] = (tileProp & 0x04) != 0x04 || (tileProp & 0x01) != 0x01;  // south or east

                    if ((tileNum & 0x10) == 0x10)
                        g.FillRectangle(b, r);
                    //else
                    //{
                    //    e = new Size(r.Width / 2, r.Height / 2);
                    //    for (int i = 0; i < 4; i++)
                    //    {
                    //        p = new Point(r.X + ((i % 2) * (int)(8 * z)), r.Y + ((i / 2) * (int)(8 * z)));
                    //        if (quadrants[i])
                    //            g.FillRectangle(b, new Rectangle(p, e));
                    //    }
                    //}
                }
            }
        }

        private void CopySuboverlayToOverlay(int[] overlay, int overlayWidth, int[] sub, int subWidth, int subHeight, int xPixel, int yPixel)
        {
            int temp;
            for (int y = 0; y < subHeight; y++)
            {
                for (int x = 0; x < subWidth; x++)
                {
                    temp = sub[y * subWidth + x];
                    if (yPixel + y >= 0 && xPixel + x >= 0 && temp != 0)
                        overlay[((yPixel + y) * overlayWidth) + (xPixel + x)] = temp;
                }
            }
        }

        private Bitmap DrawImageFromIntArr(int[] arr, int width, int height)
        {
            Bitmap image = null;
            unsafe
            {
                fixed (void* firstPixel = &arr[0])
                {
                    IntPtr ip = new IntPtr(firstPixel);
                    if (image != null)
                        image.Dispose();
                    image = new Bitmap(width, height, width * 4, System.Drawing.Imaging.PixelFormat.Format32bppPArgb, ip);

                }
            }
            return image;
        }
    }
}

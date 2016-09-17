using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace FF3LE.Undo
{
    class TileMapEditCommand : Command
    {
        Levels updater;
        TileMap tileMap;
        WorldMapTilemap wmTileMap;
        Point topLeft, bottomRight;
        State state;
        int[] changes;
        int layer;
        bool pasting;
        private bool autoRedo = false; public bool AutoRedo() { return this.autoRedo; }
        public TileMapEditCommand(Levels updater, TileMap tileMap, int layer, Point topLeft, Point bottomRight, int[] changes, bool pasting)
        {
            this.updater = updater;
            this.tileMap = tileMap;
            this.layer = layer;
            this.state = State.Instance;
            if (topLeft.Y < bottomRight.Y)
            {
                this.topLeft = topLeft;
                this.bottomRight = bottomRight;
            }
            else if (topLeft == bottomRight && topLeft.X <= bottomRight.X)
            {
                this.topLeft = topLeft;
                this.bottomRight = bottomRight;
            }
            else if (bottomRight.Y < topLeft.Y)
            {
                this.topLeft = bottomRight;
                this.bottomRight = topLeft;
            }
            this.changes = new int[changes.Length];
            changes.CopyTo(this.changes, 0);

            this.pasting = pasting;
            Execute();
        }
        public TileMapEditCommand(Levels updater, WorldMapTilemap wmTileMap, Point topLeft, Point bottomRight, int[] changes, bool pasting)
        {
            this.updater = updater;
            this.wmTileMap = wmTileMap;
            this.state = State.Instance;
            if (topLeft.Y < bottomRight.Y)
            {
                this.topLeft = topLeft;
                this.bottomRight = bottomRight;
            }
            else if (topLeft == bottomRight && topLeft.X <= bottomRight.X)
            {
                this.topLeft = topLeft;
                this.bottomRight = bottomRight;
            }
            else if (bottomRight.Y < topLeft.Y)
            {
                this.topLeft = bottomRight;
                this.bottomRight = topLeft;
            }
            this.changes = new int[changes.Length];
            changes.CopyTo(this.changes, 0);

            this.pasting = pasting;
            Execute();
        }
        public void Execute()
        {
            int deltaX = (bottomRight.X / 16) - (topLeft.X / 16);
            int deltaY = (bottomRight.Y / 16) - (topLeft.Y / 16);

            int tileX, tileY;
            int temp;
            bool empty;

            for (int y = 0; y < deltaY; y++)
            {
                for (int x = 0; x < deltaX; x++)
                {
                    tileX = topLeft.X + (x * 16);
                    tileY = topLeft.Y + (y * 16);

                    if (tileMap != null)
                    {
                        temp = tileMap.GetTileNum(layer, tileX, tileY); // Save the current tileNum for later undo
                        empty = changes[x + y * deltaX] == 0;
                        if (temp != changes[x + y * deltaX])
                        {
                            tileMap.MakeEdit(changes[x + y * deltaX], layer, tileX, tileY); // Set the tile to the new tileNum
                            changes[x + y * deltaX] = temp; // Replace the change with the old tileNum, so that all we have to do is Execute to undo this command
                        }
                        if (pasting && empty && state.Move)
                            tileMap.MakeEdit(temp, layer, tileX, tileY);
                    }
                    else if (wmTileMap != null)
                    {
                        temp = wmTileMap.GetTileNum(tileX, tileY); // Save the current tileNum for later undo

                        wmTileMap.MakeEdit(changes[x + y * deltaX], tileX, tileY); // Set the tile to the new tileNum
                        changes[x + y * deltaX] = temp; // Replace the change with the old tileNum, so that all we have to do is Execute to undo this command
                    }
                }
            }
        }
    }
}

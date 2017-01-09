using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace FF3LE
{
    public class LevelNPCs
    {
        private byte[] data; public byte[] Data { get { return this.data; } set { this.data = value; } }

        public int startingOffset; public int StartingOffset { get { return this.startingOffset; } }

        private ArrayList npcs = new ArrayList(); public ArrayList NPCs { get { return npcs; } }

        private NPC npc;
        private int currentNPC;
        public int CurrentNPC
        {
            get { return currentNPC; }
            set
            {
                if (this.npcs.Count > value)
                {
                    npc = (NPC)npcs[value];
                    this.currentNPC = value;
                }
            }
        }
        public void RemoveCurrentNPC()
        {
            if (currentNPC < npcs.Count)
            {
                npcs.Remove(npcs[currentNPC]);
                this.currentNPC = 0;
            }
        }
        public void ClearNPCs()
        {
            npcs.Clear();
            this.currentNPC = 0;
        }
        public void AddNewNPC(int index)
        {
            NPC e = new NPC();
            e.NullNPC();
            if (index < npcs.Count)
                npcs.Insert(index, e);
            else
                npcs.Add(e);
        }

        private int selectedNPC; public int SelectedNPC { get { return selectedNPC; } set { selectedNPC = value; } }

        private int levelNum; public int LevelNum { get { return levelNum; } set { levelNum = value; } }

        public int EventPointer { get { return npc.EventPointer; } set { npc.EventPointer = value; } }
        public byte Palette { get { return npc.Palette; } set { npc.Palette = value; } }
        public bool SolidifyActionPath { get { return npc.SolidifyActionPath; } set { npc.SolidifyActionPath = value; } }
        public byte CoordX { get { return npc.CoordX; } set { npc.CoordX = value; } }
        public byte CoordY { get { return npc.CoordY; } set { npc.CoordY = value; } }

        public ushort CheckMem { get { return npc.CheckMem; } set { npc.CheckMem = value; } }
        public byte CheckBit { get { return npc.CheckBit; } set { npc.CheckBit = value; } }

        public byte Speed { get { return npc.Speed; } set { npc.Speed = value; } }
        public byte SpriteNum { get { return npc.SpriteNum; } set { npc.SpriteNum = value; } }

        public byte Action { get { return npc.Action; } set { npc.Action = value; } }
        public bool WalkUnder { get { return npc.WalkUnder; } set { npc.WalkUnder = value; } }
        public bool WalkOver { get { return npc.WalkOver; } set { npc.WalkOver = value; } }
        public byte Vehicle { get { return npc.Vehicle; } set { npc.Vehicle = value; } }
        public byte Facing { get { return npc.Facing; } set { npc.Facing = value; } }

        public bool DontFaceOnTrigger { get { return npc.DontFaceOnTrigger; } set { npc.DontFaceOnTrigger = value; } }

        public bool Byte4bit7 { get { return npc.Byte4bit7; } set { npc.Byte4bit7 = value; } }
        public bool Byte8bit3 { get { return npc.Byte8bit3; } set { npc.Byte8bit3 = value; } }
        public bool Byte8bit4 { get { return npc.Byte8bit4; } set { npc.Byte8bit4 = value; } }
        public bool Byte8bit5 { get { return npc.Byte8bit5; } set { npc.Byte8bit5 = value; } }
        public bool Byte8bit6 { get { return npc.Byte8bit6; } set { npc.Byte8bit6 = value; } }
        public bool Byte8bit7 { get { return npc.Byte8bit7; } set { npc.Byte8bit7 = value; } }

        public LevelNPCs(byte[] data, int levelNum)
        {
            this.data = data;
            this.levelNum = levelNum;
            InitializeNPCs(data);
            InitializeTreasures(data);
        }

        //madsiur
        private void InitializeNPCs(byte[] data)
        {
            int offset;
            ushort offsetStart = 0;
            ushort offsetEnd = 0;
            NPC tNPC;
            
            int pointerOffset = (levelNum * 2) + Model.BASE_NPC_PTR;

            offsetStart = ByteManage.GetShort(data, pointerOffset); pointerOffset += 2;
            offsetEnd = ByteManage.GetShort(data, pointerOffset);

            if (offsetStart >= offsetEnd) return; // no npc fields for level

            offset = offsetStart + Model.BASE_NPC_PTR;
            startingOffset = offset;

            while (offset < offsetEnd + Model.BASE_NPC_PTR)
            {
                tNPC = new NPC();
                tNPC.InitializeNPC(data, offset);
                npcs.Add(tNPC);

                offset += 9;
            }
        }

        //madsiur
        public ushort AssembleNPCs(ushort offsetStart)
        {
            int offset = 0;
            
            int pointerOffset = (levelNum * 2) + Model.BASE_NPC_PTR;

            ByteManage.SetShort(data, pointerOffset, offsetStart);  // set the new pointer for the fields

            if (npcs.Count == 0) return offsetStart; // no exit fields for level

            offset = offsetStart + Model.BASE_NPC_PTR;

            foreach (NPC n in npcs)
            {
                n.AssembleNPC(data, offset);
                offset += 9;
            }

            offsetStart = (ushort)(offset - Model.BASE_NPC_PTR);

            return offsetStart;
        }

        //madsiur
        public ushort AssembleTreasures(ushort offsetStart)
        {
            int offset = 0;
            int pointerOffset = (levelNum * 2) + Model.BASE_CHEST_PTR;

            ByteManage.SetShort(data, pointerOffset, offsetStart);  // set the new pointer for the fields

            if (treasures.Count == 0) return offsetStart; // no exit fields for level

            offset = offsetStart + Model.BASE_CHEST;

            foreach (Treasure t in treasures)
            {
                t.AssembleTreasure(data, offset);
                offset += 5;
            }

            offsetStart = (ushort)(offset - Model.BASE_CHEST);

            return offsetStart;
        }
        public class NPC
        {
            private int eventPointer; public int EventPointer { get { return eventPointer; } set { eventPointer = value; } }
            private byte palette; public byte Palette { get { return palette; } set { palette = value; } }
            private bool solidifyActionPath; public bool SolidifyActionPath { get { return solidifyActionPath; } set { solidifyActionPath = value; } }
            private byte coordX; public byte CoordX { get { return coordX; } set { coordX = value; } }
            private byte coordY; public byte CoordY { get { return coordY; } set { coordY = value; } }

            private ushort checkMem; public ushort CheckMem { get { return checkMem; } set { checkMem = value; } }
            private byte checkBit; public byte CheckBit { get { return checkBit; } set { checkBit = value; } }

            private byte speed; public byte Speed { get { return speed; } set { speed = value; } }
            private byte spriteNum; public byte SpriteNum { get { return spriteNum; } set { spriteNum = value; } }

            private byte action; public byte Action { get { return action; } set { action = value; } }
            private bool walkUnder; public bool WalkUnder { get { return walkUnder; } set { walkUnder = value; } }
            private bool walkOver; public bool WalkOver { get { return walkOver; } set { walkOver = value; } }
            private byte vehicle; public byte Vehicle { get { return vehicle; } set { vehicle = value; } }
            private byte facing; public byte Facing { get { return facing; } set { facing = value; } }

            private bool dontFaceOnTrigger; public bool DontFaceOnTrigger { get { return dontFaceOnTrigger; } set { dontFaceOnTrigger = value; } }

            private bool byte4bit7; public bool Byte4bit7 { get { return byte4bit7; } set { byte4bit7 = value; } }
            private bool byte8bit3; public bool Byte8bit3 { get { return byte8bit3; } set { byte8bit3 = value; } }
            private bool byte8bit4; public bool Byte8bit4 { get { return byte8bit4; } set { byte8bit4 = value; } }
            private bool byte8bit5; public bool Byte8bit5 { get { return byte8bit5; } set { byte8bit5 = value; } }
            private bool byte8bit6; public bool Byte8bit6 { get { return byte8bit6; } set { byte8bit6 = value; } }
            private bool byte8bit7; public bool Byte8bit7 { get { return byte8bit7; } set { byte8bit7 = value; } }

            public void InitializeNPC(byte[] data, int offset)
            {
                eventPointer = (int)(ByteManage.GetInt(data, offset) & 0x03FFFF); offset += 2;
                palette = (byte)((data[offset] & 0x1C) >> 2);
                solidifyActionPath = (data[offset] & 0x20) == 0x20;

                checkMem = (ushort)((ByteManage.GetShort(data, offset) >> 9) + 0x1EE0);
                checkBit = (byte)((ByteManage.GetShort(data, offset) >> 6) & 0x07); offset += 2;

                coordX = (byte)(data[offset] & 0x7F);
                byte4bit7 = (data[offset] & 0x80) == 0x80; offset++;
                coordY = (byte)(data[offset] & 0x3F);
                speed = (byte)((data[offset] & 0xC0) >> 6); offset++;
                spriteNum = data[offset]; offset++;
                action = (byte)(data[offset] & 0x0F);
                walkUnder = (data[offset] & 0x10) == 0x10;
                walkOver = (data[offset] & 0x20) == 0x20;
                vehicle = (byte)((data[offset] & 0xC0) >> 6); offset++;
                facing = (byte)(data[offset] & 0x03);
                dontFaceOnTrigger = (data[offset] & 0x04) == 0x04;
                byte8bit3 = (data[offset] & 0x08) == 0x08;
                byte8bit4 = (data[offset] & 0x10) == 0x10;
                byte8bit5 = (data[offset] & 0x20) == 0x20;
                byte8bit6 = (data[offset] & 0x40) == 0x40;
                byte8bit7 = (data[offset] & 0x80) == 0x80;
                offset++;
            }
            public void NullNPC()
            {
                eventPointer = 0;
                palette = 0;
                solidifyActionPath = false;

                checkMem = 0x1EE0;
                checkBit = 0;

                coordX = 0;
                coordY = 0;
                speed = 0;
                spriteNum = 0;
                action = 0;
                walkUnder = false;
                walkOver = false;
                vehicle = 0;
                facing = 0;
                dontFaceOnTrigger = false;
            }
            public void AssembleNPC(byte[] data, int offset)
            {
                data[offset] = (byte)(eventPointer & 0xFF); offset++;
                data[offset] = (byte)((eventPointer & 0xFF00) >> 8); offset++;
                data[offset] = (byte)((eventPointer & 0x030000) >> 16);
                ByteManage.SetByteBits(data, offset, (byte)(palette << 2), 0x1C);
                ByteManage.SetBit(data, offset, 5, solidifyActionPath);

                ByteManage.SetShortBits(data, offset, (ushort)((checkMem - 0x1EE0) << 9), 0xFE00);
                ByteManage.SetShortBits(data, offset, (ushort)(checkBit << 6), 0x01C0); offset += 2;

                data[offset] = coordX;
                ByteManage.SetBit(data, offset, 7, byte4bit7); offset++;
                data[offset] = coordY;
                ByteManage.SetByteBits(data, offset, (byte)(speed << 6), 0xC0); offset++;
                data[offset] = spriteNum; offset++;
                data[offset] = action;
                ByteManage.SetBit(data, offset, 4, walkUnder);
                ByteManage.SetBit(data, offset, 5, walkOver);
                ByteManage.SetByteBits(data, offset, (byte)(vehicle << 6), 0xC0); offset++;
                data[offset] = facing;
                ByteManage.SetBit(data, offset, 2, dontFaceOnTrigger);
                ByteManage.SetBit(data, offset, 3, byte8bit3);
                ByteManage.SetBit(data, offset, 4, byte8bit4);
                ByteManage.SetBit(data, offset, 5, byte8bit5);
                ByteManage.SetBit(data, offset, 6, byte8bit6);
                ByteManage.SetBit(data, offset, 7, byte8bit7);
            }
        }
        public int[] GetSpritePixels(int npcNum)
        {
            int[] pixels = new int[16 * 32];
            int[] tile = new int[8 * 8];

            NPC npc = (NPC)npcs[npcNum];

            int pointer = ByteManage.GetShort(data, npc.SpriteNum * 2 + 0x00D0F2);
            int bank = data[npc.SpriteNum * 2 + 0x00D23C]; bank -= 0xC0;
            int size = data[npc.SpriteNum * 2 + 0x00D23D];

            /*int pointer = ByteManage.GetShort(data, npc.SpriteNum * 2 + 0x44FC00);
            int bank = data[npc.SpriteNum * 2 + 0x44FE00];
            int size = data[npc.SpriteNum * 2 + 0x44FE01];*/

            int offset = (bank << 16) + pointer;

            switch (npc.Facing)
            {
                case 0: offset += !npc.Byte8bit6 ? 0x180 : 0; break; //north
                case 1: offset += !npc.Byte8bit6 ? 0x300 : 0x80; break;  //east
                case 2: offset += !npc.Byte8bit6 ? 0 : 0x100; break; //south
                case 3: offset += !npc.Byte8bit6 ? 0x300 : 0x180; break;  //west
            }
            Size s = new Size(2, 3);
            if (npc.Byte8bit5)
                offset -= 0x180;
            if (npc.Byte8bit6)
                offset += 0x20;

            byte[] sprite = ByteManage.GetByteArray(data, offset, 0x200);
            int[] palette = GetPalette(npc.Palette);

            Tile8x8 temp;
            int i;

            for (int y = 0; y < s.Height; y++)
            {
                for (int x = 0; x < s.Width; x++)
                {
                    i = y * 2 + x;
                    temp = new Tile8x8(i, sprite, i * 0x20, palette, false, false, false, false);
                    CopyOverTile8x8(temp, pixels, 16, x * 8, (y + (npc.Byte8bit6 ? 1 : 0)) * 8);
                }
            }

            if (npc.Facing == 1)    // mirror if facing east
            {
                int o = 0;
                for (int y = 0; y < 32; y++)
                {
                    for (int a = 0, b = 15; a < 8; a++, b--)
                    {
                        o = pixels[(y * 16) + a];
                        pixels[(y * 16) + a] = pixels[(y * 16) + b];
                        pixels[(y * 16) + b] = o;
                    }
                }
            }
            return pixels;
        }
        private void CopyOverTile8x8(Tile8x8 source, int[] dest, int width, int x, int y)
        {
            int[] src = source.Pixels;
            int counter = 0;
            for (int i = 0; i < 64; i++)
            {
                if (src[i] != 0)
                    dest[y * width + x + counter] = src[i];
                counter++;
                if (counter % 8 == 0)
                {
                    y++;
                    counter = 0;
                }
            }
        }
        private int[] GetPalette(byte num)
        {
            int[] temp = new int[16];

            int offset = (num * 0x20) + 0x268000;

            int r, g, b;
            double multiplier = 8.2258064516; // 255 / 31;
            ushort color = 0;

            for (int i = 0; i < 16; i++) // 7 palettes in set
            {
                color = ByteManage.GetShort(data, offset + (i * 2));

                r = (byte)((color % 0x20) * multiplier);
                g = (byte)(((color >> 5) % 0x20) * multiplier);
                b = (byte)(((color >> 10) % 0x20) * multiplier);

                temp[i] = Color.FromArgb(255, r, g, b).ToArgb();
            }

            return temp;
        }

        // Treasures
        public int startingOffsetT; public int StartingOffsetT { get { return this.startingOffsetT; } }

        private ArrayList treasures = new ArrayList(); public ArrayList Treasures { get { return treasures; } }

        private Treasure treasure;
        private int currentTreasure;
        public int CurrentTreasure
        {
            get { return currentTreasure; }
            set
            {
                if (this.treasures.Count > value)
                {
                    treasure = (Treasure)treasures[value];
                    this.currentTreasure = value;
                }
            }
        }
        public void RemoveCurrentTreasure()
        {
            if (currentTreasure < treasures.Count)
            {
                treasures.Remove(treasures[currentTreasure]);
                this.currentTreasure = 0;
            }
        }
        public void ClearTreasures()
        {
            treasures.Clear();
            this.currentTreasure = 0;
        }
        public void AddNewTreasure(int index)
        {
            Treasure e = new Treasure();
            e.NullTreasure();
            if (index < treasures.Count)
                treasures.Insert(index, e);
            else
                treasures.Add(e);
        }

        private int selectedTreasure; public int SelectedTreasure { get { return selectedTreasure; } set { selectedTreasure = value; } }

        public byte CoordXTreasure { get { return treasure.CoordX; } set { treasure.CoordX = value; } }
        public byte CoordYTreasure { get { return treasure.CoordY; } set { treasure.CoordY = value; } }

        public byte PropertyNum { get { return treasure.PropertyNum; } set { treasure.PropertyNum = value; } }

        public ushort CheckMemTreasure { get { return treasure.CheckMem; } set { treasure.CheckMem = value; } }
        public byte CheckBitTreasure { get { return treasure.CheckBit; } set { treasure.CheckBit = value; } }

        public byte TreasureType { get { return treasure.TreasureType; } set { treasure.TreasureType = value; } }

        //madsiur
        private void InitializeTreasures(byte[] data)
        {
            int offset;
            ushort offsetStart = 0;
            ushort offsetEnd = 0;
            Treasure tTreasure;

            int pointerOffset = (levelNum * 2) + Model.BASE_CHEST_PTR;

            offsetStart = ByteManage.GetShort(data, pointerOffset); pointerOffset += 2;
            offsetEnd = ByteManage.GetShort(data, pointerOffset);

            if (offsetStart >= offsetEnd) return; // no npc fields for level

            offset = offsetStart + Model.BASE_CHEST;
            startingOffsetT = offset;

            while (offset < offsetEnd + Model.BASE_CHEST)
            {
                tTreasure = new Treasure();
                tTreasure.InitializeTreasure(data, offset);
                treasures.Add(tTreasure);

                offset += 5;
            }
        }
        public class Treasure
        {
            private byte coordX; public byte CoordX { get { return coordX; } set { coordX = value; } }
            private byte coordY; public byte CoordY { get { return coordY; } set { coordY = value; } }

            private byte propertyNum; public byte PropertyNum { get { return propertyNum; } set { propertyNum = value; } }

            private ushort checkMem; public ushort CheckMem { get { return checkMem; } set { checkMem = value; } }
            private byte checkBit; public byte CheckBit { get { return checkBit; } set { checkBit = value; } }

            private byte treasureType; public byte TreasureType { get { return treasureType; } set { treasureType = value; } }

            public void InitializeTreasure(byte[] data, int offset)
            {
                coordX = data[offset]; offset++;
                coordY = data[offset]; offset++;

                if (Model.IsChestsExpanded)
                {
                    checkMem = (ushort)(((ByteManage.GetShort(data, offset) & 0x3FF) >> 3) + 0x1E20);
                }
                else
                {
                    checkMem = (ushort) (((ByteManage.GetShort(data, offset) & 0x1FF) >> 3) + 0x1E40);
                }

                checkBit = (byte)(data[offset] & 0x07); offset++;
                switch (data[offset] >> 4)
                {
                    case 0: treasureType = 0; break;
                    case 1: treasureType = 1; break;
                    case 2: treasureType = 2; break;
                    case 4: treasureType = 3; break;
                    case 8: treasureType = 4; break;
                }
                offset++;
                propertyNum = data[offset];
            }
            public void NullTreasure()
            {
                coordX = 0;
                coordY = 0;
                checkMem = 0x1E40;
                checkBit = 0;
                treasureType = 0;
                propertyNum = 0;
            }
            public void AssembleTreasure(byte[] data, int offset)
            {
                data[offset] = coordX; offset++;
                data[offset] = coordY; offset++;

                if (Model.IsChestsExpanded)
                {
                    ByteManage.SetShortBits(data, offset, (ushort)((checkMem - 0x1E20) << 3), 0x03F8);
                }
                else
                {
                    ByteManage.SetShortBits(data, offset, (ushort) ((checkMem - 0x1E40) << 3), 0x01F8);
                }

                ByteManage.SetByteBits(data, offset, checkBit, 0x07); offset++;

                data[offset] &= 0x0F;
                switch (treasureType)
                {
                    case 0: break;
                    case 1: ByteManage.SetBit(data, offset, 4, true); break;
                    case 2: ByteManage.SetBit(data, offset, 5, true); break;
                    case 3: ByteManage.SetBit(data, offset, 6, true); break;
                    case 4: ByteManage.SetBit(data, offset, 7, true); break;
                }
                offset++;
                data[offset] = propertyNum;
            }
        }
    }
}

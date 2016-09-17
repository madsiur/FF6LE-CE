using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace FF3LE
{
    public class LevelExits
    {
        private byte[] data; public byte[] Data { get { return this.data; } set { this.data = value; } }

        public int startingOffsetS; public int StartingOffsetS { get { return this.startingOffsetS; } }
        public int startingOffsetL; public int StartingOffsetL { get { return this.startingOffsetL; } }

        private ArrayList exitsShort = new ArrayList(); public ArrayList ExitsShort { get { return exitsShort; } }
        private ArrayList exitsLong = new ArrayList(); public ArrayList ExitsLong { get { return exitsLong; } }

        private ExitShort exitShort;
        private int currentExitShort;
        public int CurrentExitShort
        {
            get { return currentExitShort; }
            set
            {
                if (this.exitsShort.Count > value)
                {
                    exitShort = (ExitShort)exitsShort[value];
                    this.currentExitShort = value;
                }
            }
        }
        public void RemoveCurrentExitShort()
        {
            if (currentExitShort < exitsShort.Count)
            {
                exitsShort.Remove(exitsShort[currentExitShort]);
                this.currentExitShort = 0;
            }
        }
        public void ClearExitsShort()
        {
            exitsShort.Clear();
            this.currentExitShort = 0;
        }
        public void AddNewExitShort(int index)
        {
            ExitShort e = new ExitShort();
            e.NullExit();
            if (index < exitsShort.Count)
                exitsShort.Insert(index, e);
            else
                exitsShort.Add(e);
        }

        private ExitLong exitLong;
        private int currentExitLong;
        public int CurrentExitLong
        {
            get { return currentExitLong; }
            set
            {
                if (this.exitsLong.Count > value)
                {
                    exitLong = (ExitLong)exitsLong[value];
                    this.currentExitLong = value;
                }
            }
        }
        public void RemoveCurrentExitLong()
        {
            if (currentExitLong < exitsLong.Count)
            {
                exitsLong.Remove(exitsLong[currentExitLong]);
                this.currentExitLong = 0;
            }
        }
        public void ClearExitsLong()
        {
            exitsLong.Clear();
            this.currentExitLong = 0;
        }
        public void AddNewExitLong(int index)
        {
            ExitLong e = new ExitLong();
            e.NullExit();
            if (index < exitsLong.Count)
                exitsLong.Insert(index, e);
            else
                exitsLong.Add(e);
        }

        private int selectedExitShort; public int SelectedExitShort { get { return selectedExitShort; } set { selectedExitShort = value; } }
        private int selectedExitLong; public int SelectedExitLong { get { return selectedExitLong; } set { selectedExitLong = value; } }

        private int levelNum; public int LevelNum { get { return levelNum; } set { levelNum = value; } }

        public byte CoordXShort { get { return exitShort.CoordX; } set { exitShort.CoordX = value; } }
        public byte CoordYShort { get { return exitShort.CoordY; } set { exitShort.CoordY = value; } }

        public bool ToWorldMapShort { get { return exitShort.ToWorldMap; } set { exitShort.ToWorldMap = value; } }
        public ushort DestinationShort { get { return exitShort.Destination; } set { exitShort.Destination = value; } }
        public byte DestinationXCoordShort { get { return exitShort.DestinationXCoord; } set { exitShort.DestinationXCoord = value; } }
        public byte DestinationYCoordShort { get { return exitShort.DestinationYCoord; } set { exitShort.DestinationYCoord = value; } }
        public byte DestinationFacingShort { get { return exitShort.DestinationFacing; } set { exitShort.DestinationFacing = value; } }

        public bool ShowMessageShort { get { return exitShort.ShowMessage; } set { exitShort.ShowMessage = value; } }

        public bool Byte3bit1Short { get { return exitShort.Byte3bit1; } set { exitShort.Byte3bit1 = value; } }
        public bool Byte3bit2Short { get { return exitShort.Byte3bit2; } set { exitShort.Byte3bit2 = value; } }
        public bool Byte3bit4Short { get { return exitShort.Byte3bit4; } set { exitShort.Byte3bit4 = value; } }
        public bool Byte3bit5Short { get { return exitShort.Byte3bit5; } set { exitShort.Byte3bit5 = value; } }

        public byte CoordXLong { get { return exitLong.CoordX; } set { exitLong.CoordX = value; } }
        public byte CoordYLong { get { return exitLong.CoordY; } set { exitLong.CoordY = value; } }
        public byte CoordWidthLong { get { return exitLong.Width; } set { exitLong.Width = value; } }
        public byte DirectionLong { get { return exitLong.Direction; } set { exitLong.Direction = value; } }

        public bool ToWorldMapLong { get { return exitLong.ToWorldMap; } set { exitLong.ToWorldMap = value; } }
        public ushort DestinationLong { get { return exitLong.Destination; } set { exitLong.Destination = value; } }
        public byte DestinationXCoordLong { get { return exitLong.DestinationXCoord; } set { exitLong.DestinationXCoord = value; } }
        public byte DestinationYCoordLong { get { return exitLong.DestinationYCoord; } set { exitLong.DestinationYCoord = value; } }
        public byte DestinationFacingLong { get { return exitLong.DestinationFacing; } set { exitLong.DestinationFacing = value; } }

        public bool ShowMessageLong { get { return exitLong.ShowMessage; } set { exitLong.ShowMessage = value; } }

        public bool Byte3bit1Long { get { return exitLong.Byte3bit1; } set { exitLong.Byte3bit1 = value; } }
        public bool Byte3bit2Long { get { return exitLong.Byte3bit2; } set { exitLong.Byte3bit2 = value; } }
        public bool Byte3bit4Long { get { return exitLong.Byte3bit4; } set { exitLong.Byte3bit4 = value; } }
        public bool Byte3bit5Long { get { return exitLong.Byte3bit5; } set { exitLong.Byte3bit5 = value; } }

        public LevelExits(byte[] data, int levelNum)
        {
            this.data = data;
            this.levelNum = levelNum;
            InitializeExitsShort(data);
            InitializeExitsLong(data);
        }
        public LevelExits(byte[] data)
        {
            // Used as storage for the Events Previewer
            this.data = data;
        }

        //madsiur
        private void InitializeExitsShort(byte[] data)
        {
            int offset;
            ushort offsetStart = 0;
            ushort offsetEnd = 0;
            ExitShort tExitShort;

            int pointerOffset = (levelNum * 2) + Model.BASE_SHORT_EXIT_PTR;

            offsetStart = ByteManage.GetShort(data, pointerOffset); pointerOffset += 2;
            offsetEnd = ByteManage.GetShort(data, pointerOffset);

            if (offsetStart >= offsetEnd) return; // no npc fields for level

            offset = offsetStart + Model.BASE_SHORT_EXIT_PTR;
            startingOffsetS = offset;

            while (offset < offsetEnd + Model.BASE_SHORT_EXIT_PTR)
            {
                tExitShort = new ExitShort();
                tExitShort.InitializeExitShort(data, offset);
                exitsShort.Add(tExitShort);

                offset += 6;
            }
        }

        //madsiur
        private void InitializeExitsLong(byte[] data)
        {
            int offset;
            ushort offsetStart = 0;
            ushort offsetEnd = 0;
            ExitLong tExitLong;

            int pointerOffset = (levelNum * 2) + Model.BASE_LONG_EXIT_PTR;

            offsetStart = ByteManage.GetShort(data, pointerOffset); pointerOffset += 2;
            offsetEnd = ByteManage.GetShort(data, pointerOffset);

            if (offsetStart >= offsetEnd) return; // no npc fields for level

            offset = offsetStart + Model.BASE_LONG_EXIT_PTR;
            startingOffsetL = offset;

            while (offset < offsetEnd + Model.BASE_LONG_EXIT_PTR)
            {
                tExitLong = new ExitLong();
                tExitLong.InitializeExitLong(data, offset);
                exitsLong.Add(tExitLong);

                offset += 7;
            }
        }

        //madsiur
        public ushort AssembleExitsShort(ushort offsetStart)
        {
            int offset = 0;
            int pointerOffset = (levelNum * 2) + Model.BASE_SHORT_EXIT_PTR;

            ByteManage.SetShort(data, pointerOffset, offsetStart);  // set the new pointer for the fields

            if (exitsShort.Count == 0) return offsetStart; // no exit fields for level

            offset = offsetStart + Model.BASE_SHORT_EXIT_PTR;

            foreach (ExitShort es in exitsShort)
            {
                es.AssembleExit(data, offset);
                offset += 6;
            }

            offsetStart = (ushort)(offset - Model.BASE_SHORT_EXIT_PTR);

            return offsetStart;
        }
        public ushort AssembleExitsLong(ushort offsetStart)
        {
            int offset = 0;
            int pointerOffset = (levelNum * 2) + Model.BASE_LONG_EXIT_PTR;

            ByteManage.SetShort(data, pointerOffset, offsetStart);  // set the new pointer for the fields

            if (exitsLong.Count == 0) return offsetStart; // no exit fields for level

            offset = offsetStart + Model.BASE_LONG_EXIT_PTR;

            foreach (ExitLong es in exitsLong)
            {
                es.AssembleExit(data, offset);
                offset += 7;
            }

            offsetStart = (ushort)(offset - Model.BASE_LONG_EXIT_PTR);

            return offsetStart;
        }
        public class ExitShort
        {
            private byte coordX; public byte CoordX { get { return coordX; } set { coordX = value; } }
            private byte coordY; public byte CoordY { get { return coordY; } set { coordY = value; } }

            private bool toWorldMap; public bool ToWorldMap { get { return toWorldMap; } set { toWorldMap = value; } }
            private ushort destination; public ushort Destination { get { return destination; } set { destination = value; } }
            private byte destinationXCoord; public byte DestinationXCoord { get { return destinationXCoord; } set { destinationXCoord = value; } }
            private byte destinationYCoord; public byte DestinationYCoord { get { return destinationYCoord; } set { destinationYCoord = value; } }
            private byte destinationFacing; public byte DestinationFacing { get { return destinationFacing; } set { destinationFacing = value; } }

            private bool showMessage; public bool ShowMessage { get { return showMessage; } set { showMessage = value; } }

            private bool byte3bit1; public bool Byte3bit1 { get { return byte3bit1; } set { byte3bit1 = value; } }
            private bool byte3bit2; public bool Byte3bit2 { get { return byte3bit2; } set { byte3bit2 = value; } }
            private bool byte3bit4; public bool Byte3bit4 { get { return byte3bit4; } set { byte3bit4 = value; } }
            private bool byte3bit5; public bool Byte3bit5 { get { return byte3bit5; } set { byte3bit5 = value; } }

            public void InitializeExitShort(byte[] data, int offset)
            {
                coordX = data[offset]; offset++;
                coordY = data[offset]; offset++;
                toWorldMap = (ushort)(ByteManage.GetShort(data, offset) & 0x1FF) == 0x1FF;
                if (!toWorldMap)
                {
                    destination = (ushort)(ByteManage.GetShort(data, offset) & 0x1FF);
                    offset++;
                }
                else
                {
                    offset++;
                    destination = (data[offset] & 0x02) == 0x02 ? (ushort)0 : (ushort)1;
                }

                byte3bit1 = (data[offset] & 0x02) == 0x02;
                byte3bit2 = (data[offset] & 0x04) == 0x04;

                destinationFacing = (byte)((data[offset] & 0x30) >> 4);

                showMessage = (data[offset] & 0x08) == 0x08; offset++;

                destinationXCoord = data[offset]; offset++;
                destinationYCoord = data[offset];
            }
            public void NullExit()
            {
                coordX = 0;
                coordY = 0;
                toWorldMap = false;
                byte3bit1 = false;
                byte3bit2 = false;
                byte3bit4 = false;
                byte3bit5 = false;
                destination = 0;
                destinationXCoord = 0;
                destinationYCoord = 0;
                destinationFacing = 0;
            }
            public void AssembleExit(byte[] data, int offset)
            {
                data[offset] = coordX; offset++;
                data[offset] = coordY; offset++;
                if (!toWorldMap)
                    ByteManage.SetShort(data, offset, destination);
                else
                    ByteManage.SetShort(data, offset, 0x1FF);
                offset++;
                ByteManage.SetBit(data, offset, 1, byte3bit1);
                ByteManage.SetBit(data, offset, 2, byte3bit2);
                ByteManage.SetByteBits(data, offset, (byte)(destinationFacing << 4), 0x30);
                ByteManage.SetBit(data, offset, 3, showMessage); offset++;

                data[offset] = destinationXCoord; offset++;
                data[offset] = destinationYCoord;
            }
        }
        public class ExitLong
        {
            private byte coordX; public byte CoordX { get { return coordX; } set { coordX = value; } }
            private byte coordY; public byte CoordY { get { return coordY; } set { coordY = value; } }
            private byte width; public byte Width { get { return width; } set { width = value; } }
            private byte direction; public byte Direction { get { return direction; } set { direction = value; } }

            private bool toWorldMap; public bool ToWorldMap { get { return toWorldMap; } set { toWorldMap = value; } }
            private ushort destination; public ushort Destination { get { return destination; } set { destination = value; } }
            private byte destinationXCoord; public byte DestinationXCoord { get { return destinationXCoord; } set { destinationXCoord = value; } }
            private byte destinationYCoord; public byte DestinationYCoord { get { return destinationYCoord; } set { destinationYCoord = value; } }
            private byte destinationFacing; public byte DestinationFacing { get { return destinationFacing; } set { destinationFacing = value; } }

            private bool showMessage; public bool ShowMessage { get { return showMessage; } set { showMessage = value; } }

            private bool byte3bit1; public bool Byte3bit1 { get { return byte3bit1; } set { byte3bit1 = value; } }
            private bool byte3bit2; public bool Byte3bit2 { get { return byte3bit2; } set { byte3bit2 = value; } }
            private bool byte3bit4; public bool Byte3bit4 { get { return byte3bit4; } set { byte3bit4 = value; } }
            private bool byte3bit5; public bool Byte3bit5 { get { return byte3bit5; } set { byte3bit5 = value; } }

            public void InitializeExitLong(byte[] data, int offset)
            {
                coordX = data[offset]; offset++;
                coordY = data[offset]; offset++;
                width = (byte)(data[offset] & 0x7F);
                direction = (data[offset] & 0x80) == 0x80 ? (byte)1 : (byte)0; offset++;
                toWorldMap = (ushort)(ByteManage.GetShort(data, offset) & 0x1FF) == 0x1FF;
                if (!toWorldMap)
                {
                    destination = (ushort)(ByteManage.GetShort(data, offset) & 0x1FF);
                    offset++;
                }
                else
                {
                    offset++;
                    destination = (data[offset] & 0x02) == 0x02 ? (ushort)0 : (ushort)1;
                }

                byte3bit1 = (data[offset] & 0x02) == 0x02;
                byte3bit2 = (data[offset] & 0x04) == 0x04;

                destinationFacing = (byte)((data[offset] & 0x30) >> 4);

                showMessage = (data[offset] & 0x08) == 0x08; offset++;

                destinationXCoord = data[offset]; offset++;
                destinationYCoord = data[offset];
            }
            public void NullExit()
            {
                coordX = 0;
                coordY = 0;
                width = 0;
                direction = 0;
                toWorldMap = false;
                byte3bit1 = false;
                byte3bit2 = false;
                byte3bit4 = false;
                byte3bit5 = false;
                destination = 0;
                destinationXCoord = 0;
                destinationYCoord = 0;
                destinationFacing = 0;
            }
            public void AssembleExit(byte[] data, int offset)
            {
                data[offset] = coordX; offset++;
                data[offset] = coordY; offset++;
                data[offset] = width;
                ByteManage.SetBit(data, offset, 7, direction == 1); offset++;
                if (!toWorldMap)
                    ByteManage.SetShort(data, offset, destination);
                else
                    ByteManage.SetShort(data, offset, 0x1FF);
                offset++;
                ByteManage.SetBit(data, offset, 1, byte3bit1);
                ByteManage.SetBit(data, offset, 2, byte3bit2);
                ByteManage.SetByteBits(data, offset, (byte)(destinationFacing << 4), 0x30);
                ByteManage.SetBit(data, offset, 3, showMessage); offset++;

                data[offset] = destinationXCoord; offset++;
                data[offset] = destinationYCoord;
            }
        }
    }
}

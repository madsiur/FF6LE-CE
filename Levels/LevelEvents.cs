using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace FF3LE
{
    public class LevelEvents
    {
        private byte[] data; public byte[] Data { get { return this.data; } set { this.data = value; } }

        public int startingOffset; public int StartingOffset { get { return this.startingOffset; } }

        private ArrayList events = new ArrayList(); public ArrayList Events { get { return events; } }

        private Event theEvent;

        private int currentEvent;
        public int CurrentEvent
        {
            get { return currentEvent; }
            set
            {
                if (this.events.Count > value)
                {
                    theEvent = (Event)events[value];
                    this.currentEvent = value;
                }
            }
        }
        public void RemoveCurrentEvent()
        {
            if (currentEvent < events.Count)
            {
                events.Remove(events[currentEvent]);
                this.currentEvent = 0;
            }
        }
        public void ClearEvents()
        {
            events.Clear();
            this.currentEvent = 0;
        }
        public void AddNewEvent(int index)
        {
            Event e = new Event();
            e.NullEvent();
            if (index < events.Count)
                events.Insert(index, e);
            else
                events.Add(e);
        }
        
        private int selectedEvent; public int SelectedEvent { get { return selectedEvent; } set { selectedEvent = value; } }

        private int levelNum; public int LevelNum { get { return levelNum; } set { levelNum = value; } }

        private int entranceEvent; public int EntranceEvent { get { return entranceEvent; } set { entranceEvent = value; } }

        public byte CoordX { get { return theEvent.CoordX; } set { theEvent.CoordX = value; } }
        public byte CoordY { get { return theEvent.CoordY; } set { theEvent.CoordY = value; } }

        public int EventNum { get { return theEvent.EventNum; } set { theEvent.EventNum = value; } }

        public LevelEvents(byte[] data, int levelNum)
        {
            this.data = data;
            this.levelNum = levelNum;
            InitializeEvents(data);
        }

        //madsiur
        private void InitializeEvents(byte[] data)
        {
            entranceEvent = ByteManage.GetInt(data, (levelNum * 3) + 0x11FA00);

            int offset;
            ushort offsetStart = 0;
            ushort offsetEnd = 0;
            Event tEvent;
            string n = "d";
            if (levelNum == 0x1AF)
                n.ToString();
            int pointerOffset = (levelNum * 2) + Model.BASE_EVENT_PTR;

            offsetStart = ByteManage.GetShort(data, pointerOffset); pointerOffset += 2;
            offsetEnd = ByteManage.GetShort(data, pointerOffset);

            if (offsetStart >= offsetEnd) return; // no npc fields for level

            offset = offsetStart + Model.BASE_EVENT_PTR;
            startingOffset = offset;

            while (offset < offsetEnd + Model.BASE_EVENT_PTR)
            {
                tEvent = new Event();
                tEvent.InitializeEvent(data, offset);
                events.Add(tEvent);

                offset += 5;
            }
        }

        //madsiur
        public ushort Assemble(ushort offsetStart)
        {
            ByteManage.SetShort(data, (levelNum * 3) + 0x11FA00, (ushort)entranceEvent);
            ByteManage.SetByte(data, (levelNum * 3) + 0x11FA02, (byte)(entranceEvent >> 16));

            int offset = 0;
            int pointerOffset = (levelNum * 2) + Model.BASE_EVENT_PTR;

            string n = "d";
            if (levelNum == 0x1AF)
                n.ToString();

            ByteManage.SetShort(data, pointerOffset, offsetStart);  // set the new pointer for the fields

            if (events.Count == 0) return offsetStart; // no exit fields for level

            offset = offsetStart + Model.BASE_EVENT_PTR;

            foreach (Event e in events)
            {
                e.AssembleEvent(data, offset);
                offset += 5;
            }

            offsetStart = (ushort)(offset - Model.BASE_EVENT_PTR);

            return offsetStart;
        }
        public class Event
        {
            private byte coordX; public byte CoordX { get { return coordX; } set { coordX = value; } }
            private byte coordY; public byte CoordY { get { return coordY; } set { coordY = value; } }

            private int eventNum; public int EventNum { get { return eventNum; } set { eventNum = value; } }

            public void InitializeEvent(byte[] data, int offset)
            {
                coordX = data[offset]; offset++;
                coordY = data[offset]; offset++;
                eventNum = (int)ByteManage.GetInt(data, offset);
            }
            public void NullEvent()
            {
                coordX = 0;
                coordY = 0;
                eventNum = 0;
            }
            public void AssembleEvent(byte[] data,int offset)
            {
                data[offset] = coordX; offset++;
                data[offset] = coordY; offset++;
                data[offset] = (byte)(eventNum & 0xFF); offset++;
                data[offset] = (byte)((eventNum & 0xFF00) >> 8); offset++;
                data[offset] = (byte)((eventNum & 0xFF0000) >> 16); offset++;
            }
        }
    }
}

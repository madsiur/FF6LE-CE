using System;
using System.Collections.Generic;
using System.Text;

namespace FF3LE
{
    public sealed class State
    {

        static State instance = null;
        static readonly object padlock = new object();

        // Initial settings
        private int levelNum = 0; public int LevelNum { get { return levelNum; } set { levelNum = value; } }

        private bool layer1 = true; public bool Layer1 { get { return layer1; } set { layer1 = value; } }
        private bool layer2 = true; public bool Layer2 { get { return layer2; } set { layer2 = value; } }
        private bool layer3 = true; public bool Layer3 { get { return layer3; } set { layer3 = value; } }
        private bool priority1 = true; public bool Priority1 { get { return priority1; } set { priority1 = value; } }
        private bool bg = true; public bool BG { get { return bg; } set { bg = value; } }
        private bool physicalLayer = false; public bool PhysicalLayer { get { return physicalLayer; } set { physicalLayer = value; } }
        private bool mask = false; public bool Mask { get { return mask; } set { mask = value; } }
        private bool zones = false; public bool Zones { get { return zones; } set { zones = value; } }
        private bool objects = false; public bool Objects { get { return objects; } set { objects = value; } }
        private bool treasures = false; public bool Treasures { get { return treasures; } set { treasures = value; } }
        private bool exits = false; public bool Exits { get { return exits; } set { exits = value; } }
        private bool events = false; public bool Events { get { return events; } set { events = value; } }
        private bool cartesianGrid = false; public bool CartesianGrid { get { return cartesianGrid; } set { cartesianGrid = value; } }
        private bool orthographicGrid = false; public bool OrthographicGrid { get { return orthographicGrid; } set { orthographicGrid = value; } }
        private bool draw = false; public bool Draw { get { return draw; } set { ClearDrawSelectErase(); draw = value; } }
        private bool select = false; public bool Select { get { return select; } set { ClearDrawSelectErase(); select = value; } }
        private bool erase = false; public bool Erase { get { return erase; } set { ClearDrawSelectErase(); erase = value; } }
        private bool move = false; public bool Move { get { return move; } set { move = value; } }
        private bool paste = false; public bool Paste { get { return paste; } set { paste = value; } }

        State()
        {

        }
        public static State Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new State();
                    }
                    return instance;
                }

            }
        }
        private void ClearDrawSelectErase()
        {
            draw = false;
            select = false;
            erase = false;
        }
        public bool IsDrawingLayer(int layer)
        {
            switch (layer)
            {
                case 0:
                    return layer1;
                case 1:
                    return layer2;
                case 2:
                    return layer3;
                default:
                    return false;
            }
        }
        public void ResetAll()
        {
            layer1 = true;
            layer2 = true;
            layer3 = true;
            priority1 = true;
            bg = true;
            physicalLayer = false;
            mask = false;
            objects = false;
            treasures = false;
            exits = false;
            events = false;
            cartesianGrid = false;
            orthographicGrid = false;
            draw = false;
            select = false;
            erase = false;
            move = false;
            paste = false;
        }
    }
}

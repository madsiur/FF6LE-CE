using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace FF3LE
{
    [Serializable()]
    public class LevelLayer
    {
        // Local Variables
        // All properties of this class should be private
        private byte[] data; public byte[] Data { get { return this.data; } set { this.data = value; } }

        private int levelNum; public int LevelNum { get { return levelNum; } set { levelNum = value; } }

        private byte messageBox; public byte MessageBox { get { return messageBox; } set { messageBox = value; } }

        private bool heatWaveL1; public bool HeatWaveL1 { get { return heatWaveL1; } set { heatWaveL1 = value; } }
        private bool heatWaveL2; public bool HeatWaveL2 { get { return heatWaveL2; } set { heatWaveL2 = value; } }
        private bool searchLights; public bool SearchLights { get { return searchLights; } set { searchLights = value; } }
        private bool byte1bit0; public bool Byte1bit0 { get { return byte1bit0; } set { byte1bit0 = value; } }
        private bool warpEnabled; public bool WarpEnabled { get { return warpEnabled; } set { warpEnabled = value; } }
        private bool byte1bit2; public bool Byte1bit2 { get { return byte1bit2; } set { byte1bit2 = value; } }
        private bool byte1bit6; public bool Byte1bit6 { get { return byte1bit6; } set { byte1bit6 = value; } }
        private bool byte1bit7; public bool Byte1bit7 { get { return byte1bit7; } set { byte1bit7 = value; } }

        private byte byte17; public byte Byte17 { get { return byte17; } set { byte17 = value; } }

        private bool byte6bit0; public bool Byte6bit0 { get { return byte6bit0; } set { byte6bit0 = value; } }
        private bool byte6bit1; public bool Byte6bit1 { get { return byte6bit1; } set { byte6bit1 = value; } }
        private bool byte6bit7; public bool Byte6bit7 { get { return byte6bit7; } set { byte6bit7 = value; } }

        private byte battleBG; public byte BattleBG { get { return battleBG; } set { battleBG = value; } }
        private byte battleZone; public byte BattleZone { get { return battleZone; } set { battleZone = value; } }
        private bool randomBattle; public bool RandomBattle { get { return randomBattle; } set { randomBattle = value; } }

        private byte maskHighX; public byte MaskHighX { get { return maskHighX; } set { maskHighX = value; } }
        private byte maskHighY; public byte MaskHighY { get { return maskHighY; } set { maskHighY = value; } }

        private byte leftShiftL2; public byte LeftShiftL2 { get { return leftShiftL2; } set { leftShiftL2 = value; } }
        private byte upShiftL2; public byte UpShiftL2 { get { return upShiftL2; } set { upShiftL2 = value; } }
        private byte leftShiftL3; public byte LeftShiftL3 { get { return leftShiftL3; } set { leftShiftL3 = value; } }
        private byte upShiftL3; public byte UpShiftL3 { get { return upShiftL3; } set { upShiftL3 = value; } }

        private byte scrolling; public byte Scrolling { get { return scrolling; } set { scrolling = value; } }

        private byte layerPrioritySet; public byte LayerPrioritySet { get { return layerPrioritySet; } set { layerPrioritySet = value; } }

        private bool waveL2; public bool WaveL2 { get { return waveL2; } set { waveL2 = value; } }

        private byte animationL2; public byte AnimationL2 { get { return animationL2; } set { animationL2 = value; } }
        private byte animationL3; public byte AnimationL3 { get { return animationL3; } set { animationL3 = value; } }

        private byte graphicSetA; public byte GraphicSetA { get { return graphicSetA; } set { graphicSetA = value; } }
        private byte graphicSetB; public byte GraphicSetB { get { return graphicSetB; } set { graphicSetB = value; } }
        private byte graphicSetC; public byte GraphicSetC { get { return graphicSetC; } set { graphicSetC = value; } }
        private byte graphicSetD; public byte GraphicSetD { get { return graphicSetD; } set { graphicSetD = value; } }
        private byte graphicSetL3; public byte GraphicSetL3 { get { return graphicSetL3; } set { graphicSetL3 = value; } }

        private bool topPriorityL3; public bool TopPriorityL3 { get { return topPriorityL3; } set { topPriorityL3 = value; } }
        private bool worldMapBG; public bool WorldMapBG { get { return worldMapBG; } set { worldMapBG = value; } }

        private byte tileSetL1; public byte TileSetL1 { get { return tileSetL1; } set { tileSetL1 = value; } }
        private byte tileSetL2; public byte TileSetL2 { get { return tileSetL2; } set { tileSetL2 = value; } }

        private ushort tileMapL1; public ushort TileMapL1 { get { return tileMapL1; } set { tileMapL1 = value; } }
        private ushort tileMapL2; public ushort TileMapL2 { get { return tileMapL2; } set { tileMapL2 = value; } }
        private ushort tileMapL3; public ushort TileMapL3 { get { return tileMapL3; } set { tileMapL3 = value; } }

        private ushort physicalMap; public ushort PhysicalMap { get { return physicalMap; } set { physicalMap = value; } }

        private byte[] layerWidth = new byte[3]; public byte[] LayerWidth { get { return layerWidth; } set { layerWidth = value; } }
        private byte[] layerHeight = new byte[3]; public byte[] LayerHeight { get { return layerHeight; } set { layerHeight = value; } }

        private byte paletteSet; public byte PaletteSet { get { return paletteSet; } set { paletteSet = value; } }
        private byte music; public byte Music { get { return music; } set { music = value; } }

        public LevelLayer(byte[] data, int levelNum)
        {
            this.data = data;
            this.levelNum = levelNum;
            InitializeLevel(data);
        }
        //madsiur
        private void InitializeLevel(byte[] data)
        {
            int offset = (levelNum * 33) + Model.BASE_LOCATION;

            messageBox = ByteManage.GetByte(data, offset); offset++;

            byte1bit0 = (data[offset] & 0x01) == 0x01;
            warpEnabled = (data[offset] & 0x02) == 0x02;
            byte1bit2 = (data[offset] & 0x04) == 0x04;
            heatWaveL2 = (data[offset] & 0x08) == 0x08;
            heatWaveL1 = (data[offset] & 0x10) == 0x10;
            searchLights = (data[offset] & 0x20) == 0x20;
            byte1bit6 = (data[offset] & 0x40) == 0x40;
            byte1bit7 = (data[offset] & 0x80) == 0x80;

            offset++;

            battleBG = (byte)(data[offset] & 0x7F);
            topPriorityL3 = (data[offset] & 0x80) == 0x80; offset++;

            worldMapBG = (ByteManage.GetByte(data, offset) == 0x15); offset++;
            physicalMap = ByteManage.GetByte(data, offset); offset++;
            randomBattle = (data[offset] & 0x80) == 0x80; offset++;

            byte6bit0 = (data[offset] & 0x01) == 0x01;
            byte6bit1 = (data[offset] & 0x02) == 0x02;
            byte6bit7 = (data[offset] & 0x80) == 0x80; 
            
            offset++; //change later

            graphicSetA = (byte)(ByteManage.GetByte(data, offset) & 0x7F);
            graphicSetB = (byte)(((ByteManage.GetShort(data, offset) * 2) & 0x7F00) / 0x100); offset++;
            graphicSetC = (byte)(((ByteManage.GetShort(data, offset) * 4) & 0x7F00) / 0x100); offset++;
            graphicSetD = (byte)(((ByteManage.GetShort(data, offset) * 8) & 0x7F00) / 0x100); offset++;
            graphicSetL3 = (byte)((ByteManage.GetShort(data, offset) / 16) & 0x3F); offset++;

            tileSetL1 = (byte)((ByteManage.GetShort(data, offset) / 4) & 0x7F); offset++;
            tileSetL2 = (byte)((ByteManage.GetByte(data, offset) / 2) & 0x7F); offset++;

            tileMapL1 = (ushort)(ByteManage.GetShort(data, offset) & 0x3FF); offset++;
            tileMapL2 = (ushort)((ByteManage.GetShort(data, offset) / 2 & 0x7FE) / 2); offset++;
            tileMapL3 = (ushort)(ByteManage.GetShort(data, offset) / 16); offset++;

            if (tileMapL1 > 0x1FE) tileMapL1 = 0x1FE;
            if (tileMapL2 > 0x1FE) tileMapL2 = 0x1FE;

            offset++; // change later

            byte17 = data[offset]; offset++;

            leftShiftL2 = ByteManage.GetByte(data, offset); offset++;
            upShiftL2 = ByteManage.GetByte(data, offset); offset++;
            leftShiftL3 = ByteManage.GetByte(data, offset); offset++;
            upShiftL3 = ByteManage.GetByte(data, offset); offset++;

            scrolling = ByteManage.GetByte(data, offset); offset++;

            layerHeight[0] = (byte)(ByteManage.GetByte(data, offset) >> 4 & 0x03);
            layerWidth[0] = (byte)(ByteManage.GetByte(data, offset) >> 6);
            layerHeight[1] = (byte)(ByteManage.GetByte(data, offset) & 0x03);
            layerWidth[1] = (byte)(ByteManage.GetByte(data, offset) >> 2 & 0x03); offset++;
            layerHeight[2] = (byte)(ByteManage.GetByte(data, offset) >> 4 & 0x03);
            layerWidth[2] = (byte)(ByteManage.GetByte(data, offset) >> 6); offset++;

            paletteSet = ByteManage.GetByte(data, offset); offset += 2;

            animationL2 = (byte)(ByteManage.GetByte(data, offset) & 0x1F);
            animationL3 = (byte)((ByteManage.GetByte(data, offset) & 0xE0) >> 5); offset++;

            music = ByteManage.GetByte(data, offset); offset += 2;

            maskHighX = ByteManage.GetByte(data, offset); offset++;
            maskHighY = ByteManage.GetByte(data, offset); offset++;

            layerPrioritySet = ByteManage.GetByte(data, offset);

            battleZone = data[levelNum + 0x0F5600];
        }
        private int GetPaletteSetOffset()
        {
            return (paletteSet * 0x100) + 0x2DC480;
        }
        public int GetBGColor()
        {
            ushort bgColor = ByteManage.GetShort(data, GetPaletteSetOffset());

            // Set the background color for the level
            double multiplier = 8.2258064516; // 255 / 31;

            byte bgColorRed = (byte)((bgColor % 0x20) * multiplier); if (bgColorRed != 0) bgColorRed++;
            byte bgColorGreen = (byte)(((bgColor >> 5) % 0x20) * multiplier); if (bgColorGreen != 0) bgColorGreen++;
            byte bgColorBlue = (byte)(((bgColor >> 10) % 0x20) * multiplier); if (bgColorBlue != 0) bgColorBlue++;

            return Color.FromArgb(255, bgColorRed, bgColorGreen, bgColorBlue).ToArgb();
        }
        public byte[] GetPalette(int paletteNum)
        {
            return ByteManage.GetByteArray(data, GetPaletteSetOffset() + (paletteNum * 32), 32);
        }
        public byte[] Get2bppPalette(int paletteNum)
        {
            return ByteManage.GetByteArray(data, GetPaletteSetOffset() + (paletteNum * 8), 8);
        }

        //madsiur
        public void Assemble()
        {
            //if (levelNum < 2) return;

            int offset = (levelNum * 33) + Model.BASE_LOCATION;
            data[offset] = messageBox; offset++;

            ByteManage.SetBit(data, offset, 0, byte1bit0);
            ByteManage.SetBit(data, offset, 1, warpEnabled);
            ByteManage.SetBit(data, offset, 2, byte1bit2);
            ByteManage.SetBit(data, offset, 3, heatWaveL2);
            ByteManage.SetBit(data, offset, 4, heatWaveL1);
            ByteManage.SetBit(data, offset, 5, searchLights);
            ByteManage.SetBit(data, offset, 6, byte1bit6);
            ByteManage.SetBit(data, offset, 7, byte1bit7);

            offset++;

            data[offset] = battleBG;
            ByteManage.SetBit(data, offset, 7, topPriorityL3); offset++;

            data[offset] = worldMapBG ? (byte)0x15 : (byte)0; offset++;
            data[offset] = (byte)physicalMap; offset++;
            ByteManage.SetBit(data, offset, 7, randomBattle); offset++;

            ByteManage.SetBit(data, offset, 0, byte6bit0);
            ByteManage.SetBit(data, offset, 1, byte6bit1);
            ByteManage.SetBit(data, offset, 7, byte6bit7);

            offset++; //change later

            data[offset] = graphicSetA;
            ByteManage.SetShortBits(data, offset, (ushort)(graphicSetB << 7), 0x3F80); offset++;
            ByteManage.SetShortBits(data, offset, (ushort)(graphicSetC << 6), 0x1FC0); offset++;
            ByteManage.SetShortBits(data, offset, (ushort)(graphicSetD << 5), 0x0FE0); offset++;
            ByteManage.SetShortBits(data, offset, (ushort)(graphicSetL3 << 4), 0x03F0); offset++;

            ByteManage.SetShortBits(data, offset, (ushort)(tileSetL1 << 2), 0x01FC); offset++;
            ByteManage.SetShortBits(data, offset, (ushort)(tileSetL2 << 1), 0x00FE); offset++;

            ByteManage.SetShortBits(data, offset, (ushort)(tileMapL1), 0x03FF); offset++;
            ByteManage.SetShortBits(data, offset, (ushort)(tileMapL2 << 2), 0x1FF8); offset++;
            ByteManage.SetShortBits(data, offset, (ushort)(tileMapL3 << 4), 0xFFF0); offset++;

            offset += 2; // change later

            data[offset] = leftShiftL2; offset++;
            data[offset] = upShiftL2; offset++;
            data[offset] = leftShiftL3; offset++;
            data[offset] = upShiftL3; offset++;

            data[offset] = scrolling; offset++;

            data[offset] = (byte)(layerHeight[0] << 4);
            data[offset] |= (byte)(layerWidth[0] << 6);
            data[offset] |= layerHeight[1];
            data[offset] |= (byte)(layerWidth[1] << 2); offset++;
            ByteManage.SetByteBits(data, offset, (byte)(layerHeight[2] << 4), 0x30);
            ByteManage.SetByteBits(data, offset, (byte)(layerWidth[2] << 6), 0xC0); offset++;

            data[offset] = paletteSet; offset += 2;

            data[offset] = animationL2;
            data[offset] |= (byte)(animationL3 << 5); offset++;

            data[offset] = music; offset += 2;

            data[offset] = maskHighX; offset++;
            data[offset] = maskHighY; offset++;

            data[offset] = layerPrioritySet;

            data[levelNum + 0x0F5600] = battleZone;
        }
        public void Clear()
        {
            messageBox = 0;

            byte1bit0 = false;
            warpEnabled = false;
            byte1bit2 = false;
            heatWaveL2 = false;
            heatWaveL1 = false;
            searchLights = false;
            byte1bit6 = false;
            byte1bit7 = false;

            battleBG = 0;
            topPriorityL3 = false;

            worldMapBG = false;
            physicalMap = 0;
            randomBattle = false;

            byte6bit0 = false;
            byte6bit1 = false;
            byte6bit7 = false;

            graphicSetA = 0;
            graphicSetB = 0;
            graphicSetC = 0;
            graphicSetD = 0;
            graphicSetL3 = 0;

            tileSetL1 = 0;
            tileSetL2 = 0;

            tileMapL1 = 0;
            tileMapL2 = 0;
            tileMapL3 = 0;

            tileMapL1 = 0;
            tileMapL2 = 0;

            leftShiftL2 = 0;
            upShiftL2 = 0;
            leftShiftL3 = 0;
            upShiftL3 = 0;

            scrolling = 0;

            layerHeight[0] = 0;
            layerWidth[0] = 0;
            layerHeight[1] = 0;
            layerWidth[1] = 0;
            layerHeight[2] = 0;
            layerWidth[2] = 0;

            paletteSet = 0;

            animationL2 = 0;
            animationL3 = 0;

            music = 0;

            maskHighX = 0;
            maskHighY = 0;

            layerPrioritySet = 0;

            battleZone = 0;
        }
    }
}

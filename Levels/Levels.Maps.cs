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

        private int currentColor;

        private int[] paletteSetPixels;
        private Bitmap paletteSetImage;

        // palette effects
        private Stack<int[]> colorReds = new Stack<int[]>();
        private Stack<int[]> colorGreens = new Stack<int[]>();
        private Stack<int[]> colorBlues = new Stack<int[]>();
        private Stack<int[]> redoColorReds = new Stack<int[]>();
        private Stack<int[]> redoColorGreens = new Stack<int[]>();
        private Stack<int[]> redoColorBlues = new Stack<int[]>();

        #endregion

        #region Methods

        private void InitializeLayerProperties()
        {
            updatingLevel = true;

            // layer properties
            this.message.Value = layer.MessageBox;
            this.messageName.SelectedIndex = layer.MessageBox;
            this.music.Value = layer.Music;
            this.musicName.SelectedIndex = layer.Music;

            this.layerPrioritySet.Value = layer.LayerPrioritySet;
            this.layerMainscreenL1.Checked = prioritySets[layer.LayerPrioritySet].MainscreenL1;
            this.layerMainscreenL2.Checked = prioritySets[layer.LayerPrioritySet].MainscreenL2;
            this.layerMainscreenL3.Checked = prioritySets[layer.LayerPrioritySet].MainscreenL3;
            this.layerMainscreenNPC.Checked = prioritySets[layer.LayerPrioritySet].MainscreenOBJ;
            this.layerSubscreenL1.Checked = prioritySets[layer.LayerPrioritySet].SubscreenL1;
            this.layerSubscreenL2.Checked = prioritySets[layer.LayerPrioritySet].SubscreenL2;
            this.layerSubscreenL3.Checked = prioritySets[layer.LayerPrioritySet].SubscreenL3;
            this.layerSubscreenNPC.Checked = prioritySets[layer.LayerPrioritySet].SubscreenOBJ;
            this.layerColorMathL1.Checked = prioritySets[layer.LayerPrioritySet].ColorMathL1;
            this.layerColorMathL2.Checked = prioritySets[layer.LayerPrioritySet].ColorMathL2;
            this.layerColorMathL3.Checked = prioritySets[layer.LayerPrioritySet].ColorMathL3;
            this.layerColorMathNPC.Checked = prioritySets[layer.LayerPrioritySet].ColorMathOBJ;
            this.layerColorMathBG.Checked = prioritySets[layer.LayerPrioritySet].ColorMathBG;
            this.layerColorMathIntensity.SelectedIndex = prioritySets[layer.LayerPrioritySet].ColorMathHalfIntensity;
            this.layerColorMathMode.SelectedIndex = prioritySets[layer.LayerPrioritySet].ColorMathMinusSubscreen;

            this.layerMaskHighX.Value = layer.MaskHighX;
            this.layerMaskHighY.Value = layer.MaskHighY;
            this.topSync.Value = layer.Scrolling;
            this.mapSetL3Priority.Checked = layer.TopPriorityL3;

            this.layerL2LeftShift.Value = layer.LeftShiftL2;
            this.layerL2UpShift.Value = layer.UpShiftL2;
            this.layerL3LeftShift.Value = layer.LeftShiftL3;
            this.layerL3UpShift.Value = layer.UpShiftL3;

            this.l1height.SelectedIndex = layer.LayerHeight[0];
            this.l1width.SelectedIndex = layer.LayerWidth[0];
            this.l2height.SelectedIndex = layer.LayerHeight[1];
            this.l2width.SelectedIndex = layer.LayerWidth[1];
            this.l3height.SelectedIndex = layer.LayerHeight[2];
            this.l3width.SelectedIndex = layer.LayerWidth[2];

            // map properties
            this.mapGFXSet1Num.Value = layer.GraphicSetA;
            this.mapGFXSet2Num.Value = layer.GraphicSetB;
            this.mapGFXSet3Num.Value = layer.GraphicSetC;
            this.mapGFXSet4Num.Value = layer.GraphicSetD;
            this.mapGFXSetL3Num.Value = layer.GraphicSetL3;
            this.animationA.Value = layer.AnimationL2;
            this.animationB.Value = layer.AnimationL3;

            this.mapTilemapL1Num.Value = layer.TileMapL1;
            this.mapTilemapL2Num.Value = layer.TileMapL2;
            this.mapTilemapL3Num.Value = layer.TileMapL3;
            this.mapTilesetL1Num.Value = layer.TileSetL1;
            this.mapTilesetL2Num.Value = layer.TileSetL2;
            this.mapPhysicalMapNum.Value = layer.PhysicalMap;
            this.useWorldMapBG.Checked = layer.WorldMapBG;
            this.mapPaletteSetNum.Value = layer.PaletteSet;

            this.mapBattleBG.Value = layer.BattleBG;
            this.mapBattleBGName.SelectedIndex = layer.BattleBG;
            this.mapBattleZone.Value = layer.BattleZone;
            this.mapRandomBattles.Checked = layer.RandomBattle;

            this.layerEffects.SetItemChecked(0, layer.WarpEnabled);
            this.layerEffects.SetItemChecked(1, layer.HeatWaveL2);
            this.layerEffects.SetItemChecked(2, layer.HeatWaveL1);
            this.layerEffects.SetItemChecked(3, layer.SearchLights);
            this.layerUnknownBits.SetItemChecked(0, layer.Byte1bit0);
            this.layerUnknownBits.SetItemChecked(1, layer.Byte1bit2);
            this.layerUnknownBits.SetItemChecked(2, layer.Byte1bit6);
            this.layerUnknownBits.SetItemChecked(3, layer.Byte1bit7);
            this.layerUnknownBits.SetItemChecked(4, layer.Byte6bit0);
            this.layerUnknownBits.SetItemChecked(5, layer.Byte6bit1);
            this.layerUnknownBits.SetItemChecked(6, layer.Byte6bit7);
            this.layerByte17.Value = layer.Byte17;

            updatingLevel = false;
        }
        private void InitializeCurrentColor()
        {
            updatingLevel = true;

            this.mapPaletteRedNum.Value = paletteSet.PaletteColorRed[currentColor];
            this.mapPaletteGreenNum.Value = paletteSet.PaletteColorGreen[currentColor];
            this.mapPaletteBlueNum.Value = paletteSet.PaletteColorBlue[currentColor];

            this.mapPaletteRedBar.Value = paletteSet.PaletteColorRed[currentColor];
            this.mapPaletteGreenBar.Value = paletteSet.PaletteColorGreen[currentColor];
            this.mapPaletteBlueBar.Value = paletteSet.PaletteColorBlue[currentColor];

            pictureBoxColor.BackColor = System.Drawing.Color.FromArgb((int)mapPaletteRedNum.Value, (int)mapPaletteGreenNum.Value, (int)mapPaletteBlueNum.Value);
            SetPaletteSetImage();

            updatingLevel = false;
        }

        private void SetPaletteSetImage()
        {
            paletteSetPixels = paletteSet.GetPaletteSetPixels();
            paletteSetImage = new Bitmap(DrawImageFromIntArr(paletteSetPixels, 256, 128));

            palettePictureBox.Invalidate();
        }

        private Size LayerSize(int layer)
        {
            Size s = new Size();
            s.Width = (int)(256 * Math.Pow(2, this.layer.LayerWidth[layer]));
            s.Height = (int)(256 * Math.Pow(2, this.layer.LayerHeight[layer]));
            return s;
        }

        #endregion

        #region Event Handlers

        private void layerPrioritySet_ValueChanged(object sender, EventArgs e)
        {
            if (updatingLevel) return;

            layer.LayerPrioritySet = (byte)layerPrioritySet.Value;

            updatingLevel = true;

            this.layerMainscreenL1.Checked = prioritySets[layer.LayerPrioritySet].MainscreenL1;
            this.layerMainscreenL2.Checked = prioritySets[layer.LayerPrioritySet].MainscreenL2;
            this.layerMainscreenL3.Checked = prioritySets[layer.LayerPrioritySet].MainscreenL3;
            this.layerMainscreenNPC.Checked = prioritySets[layer.LayerPrioritySet].MainscreenOBJ;
            this.layerSubscreenL1.Checked = prioritySets[layer.LayerPrioritySet].SubscreenL1;
            this.layerSubscreenL2.Checked = prioritySets[layer.LayerPrioritySet].SubscreenL2;
            this.layerSubscreenL3.Checked = prioritySets[layer.LayerPrioritySet].SubscreenL3;
            this.layerSubscreenNPC.Checked = prioritySets[layer.LayerPrioritySet].SubscreenOBJ;
            this.layerColorMathL1.Checked = prioritySets[layer.LayerPrioritySet].ColorMathL1;
            this.layerColorMathL2.Checked = prioritySets[layer.LayerPrioritySet].ColorMathL2;
            this.layerColorMathL3.Checked = prioritySets[layer.LayerPrioritySet].ColorMathL3;
            this.layerColorMathNPC.Checked = prioritySets[layer.LayerPrioritySet].ColorMathOBJ;
            this.layerColorMathBG.Checked = prioritySets[layer.LayerPrioritySet].ColorMathBG;
            this.layerColorMathIntensity.SelectedIndex = prioritySets[layer.LayerPrioritySet].ColorMathHalfIntensity;
            this.layerColorMathMode.SelectedIndex = prioritySets[layer.LayerPrioritySet].ColorMathMinusSubscreen;

            updatingLevel = false;

            tileMap = new TileMap(layer, paletteSet, tileSet, layer, prioritySets, model);
            SetLevelImage();
        }
        private void layerEditPrioritySet_CheckedChanged(object sender, EventArgs e)
        {
            layerEditPrioritySet.ForeColor = layerEditPrioritySet.Checked ? SystemColors.ControlText : SystemColors.ControlDark;
            panelPriorities.Enabled = layerEditPrioritySet.Checked;
        }
        private void layerMainscreenL1_CheckedChanged(object sender, EventArgs e)
        {
            layerMainscreenL1.ForeColor = layerMainscreenL1.Checked ? SystemColors.ControlText : SystemColors.ControlDark;

            if (updatingLevel) return;

            prioritySets[layer.LayerPrioritySet].MainscreenL1 = layerMainscreenL1.Checked;

            tileMap.RedrawTileMap();
            SetLevelImage();
        }
        private void layerMainscreenL2_CheckedChanged(object sender, EventArgs e)
        {
            layerMainscreenL2.ForeColor = layerMainscreenL2.Checked ? SystemColors.ControlText : SystemColors.ControlDark;

            if (updatingLevel) return;

            prioritySets[layer.LayerPrioritySet].MainscreenL2 = layerMainscreenL2.Checked;

            tileMap.RedrawTileMap();
            SetLevelImage();
        }
        private void layerMainscreenL3_CheckedChanged(object sender, EventArgs e)
        {
            layerMainscreenL3.ForeColor = layerMainscreenL3.Checked ? SystemColors.ControlText : SystemColors.ControlDark;

            if (updatingLevel) return;

            prioritySets[layer.LayerPrioritySet].MainscreenL3 = layerMainscreenL3.Checked;

            tileMap.RedrawTileMap();
            SetLevelImage();
        }
        private void layerMainscreenNPC_CheckedChanged(object sender, EventArgs e)
        {
            layerMainscreenNPC.ForeColor = layerMainscreenNPC.Checked ? SystemColors.ControlText : SystemColors.ControlDark;

            if (updatingLevel) return;

            prioritySets[layer.LayerPrioritySet].MainscreenOBJ = layerMainscreenNPC.Checked;

            tileMap.RedrawTileMap();
            SetLevelImage();
        }
        private void layerSubscreenL1_CheckedChanged(object sender, EventArgs e)
        {
            layerSubscreenL1.ForeColor = layerSubscreenL1.Checked ? SystemColors.ControlText : SystemColors.ControlDark;

            if (updatingLevel) return;

            prioritySets[layer.LayerPrioritySet].SubscreenL1 = layerMainscreenL1.Checked;

            tileMap.RedrawTileMap();
            SetLevelImage();
        }
        private void layerSubscreenL2_CheckedChanged(object sender, EventArgs e)
        {
            layerSubscreenL2.ForeColor = layerSubscreenL2.Checked ? SystemColors.ControlText : SystemColors.ControlDark;

            if (updatingLevel) return;

            prioritySets[layer.LayerPrioritySet].SubscreenL2 = layerMainscreenL2.Checked;

            tileMap.RedrawTileMap();
            SetLevelImage();
        }
        private void layerSubscreenL3_CheckedChanged(object sender, EventArgs e)
        {
            layerSubscreenL3.ForeColor = layerSubscreenL3.Checked ? SystemColors.ControlText : SystemColors.ControlDark;

            if (updatingLevel) return;

            prioritySets[layer.LayerPrioritySet].SubscreenL3 = layerMainscreenL3.Checked;

            tileMap.RedrawTileMap();
            SetLevelImage();
        }
        private void layerSubscreenNPC_CheckedChanged(object sender, EventArgs e)
        {
            layerSubscreenNPC.ForeColor = layerSubscreenNPC.Checked ? SystemColors.ControlText : SystemColors.ControlDark;

            if (updatingLevel) return;

            prioritySets[layer.LayerPrioritySet].SubscreenOBJ = layerMainscreenNPC.Checked;

            tileMap.RedrawTileMap();
            SetLevelImage();
        }
        private void layerColorMathL1_CheckedChanged(object sender, EventArgs e)
        {
            layerColorMathL1.ForeColor = layerColorMathL1.Checked ? SystemColors.ControlText : SystemColors.ControlDark;

            if (updatingLevel) return;

            prioritySets[layer.LayerPrioritySet].ColorMathL1 = layerColorMathL1.Checked;

            tileMap.RedrawTileMap();
            SetLevelImage();
        }
        private void layerColorMathL2_CheckedChanged(object sender, EventArgs e)
        {
            layerColorMathL2.ForeColor = layerColorMathL2.Checked ? SystemColors.ControlText : SystemColors.ControlDark;

            if (updatingLevel) return;

            prioritySets[layer.LayerPrioritySet].ColorMathL2 = layerColorMathL2.Checked;

            tileMap.RedrawTileMap();
            SetLevelImage();
        }
        private void layerColorMathL3_CheckedChanged(object sender, EventArgs e)
        {
            layerColorMathL3.ForeColor = layerColorMathL3.Checked ? SystemColors.ControlText : SystemColors.ControlDark;

            if (updatingLevel) return;

            prioritySets[layer.LayerPrioritySet].ColorMathL3 = layerColorMathL3.Checked;

            tileMap.RedrawTileMap();
            SetLevelImage();
        }
        private void layerColorMathNPC_CheckedChanged(object sender, EventArgs e)
        {
            layerColorMathNPC.ForeColor = layerColorMathNPC.Checked ? SystemColors.ControlText : SystemColors.ControlDark;

            if (updatingLevel) return;

            prioritySets[layer.LayerPrioritySet].ColorMathOBJ = layerColorMathNPC.Checked;

            tileMap.RedrawTileMap();
            SetLevelImage();
        }
        private void layerColorMathBG_CheckedChanged(object sender, EventArgs e)
        {
            layerColorMathBG.ForeColor = layerColorMathBG.Checked ? SystemColors.ControlText : SystemColors.ControlDark;

            if (updatingLevel) return;

            prioritySets[layer.LayerPrioritySet].ColorMathBG = layerColorMathBG.Checked;

            tileMap.RedrawTileMap();
            SetLevelImage();
        }
        private void layerColorMathIntensity_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (updatingLevel) return;

            prioritySets[layer.LayerPrioritySet].ColorMathHalfIntensity = (byte)layerColorMathIntensity.SelectedIndex;

            tileMap.RedrawTileMap();
            SetLevelImage();
        }
        private void layerColorMathMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (updatingLevel) return;

            prioritySets[layer.LayerPrioritySet].ColorMathMinusSubscreen = (byte)layerColorMathMode.SelectedIndex;

            tileMap.RedrawTileMap();
            SetLevelImage();
        }

        private void layerUnknownBits_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (updatingLevel) return;

            layer.Byte1bit0 = layerUnknownBits.GetItemChecked(0);
            layer.Byte1bit2 = layerUnknownBits.GetItemChecked(1);
            layer.Byte1bit6 = layerUnknownBits.GetItemChecked(2);
            layer.Byte1bit7 = layerUnknownBits.GetItemChecked(3);
            layer.Byte6bit0 = layerUnknownBits.GetItemChecked(4);
            layer.Byte6bit1 = layerUnknownBits.GetItemChecked(5);
            layer.Byte6bit7 = layerUnknownBits.GetItemChecked(6);
        }
        private void layerByte17_ValueChanged(object sender, EventArgs e)
        {
            if (updatingLevel) return;

            layer.Byte17 = (byte)layerByte17.Value;
        }

        private void animationA_ValueChanged(object sender, EventArgs e)
        {
            if (updatingLevel) return;

            layer.AnimationL2 = (byte)animationA.Value;

            tileSet = new Tileset(layer, paletteSet, model);
            tileMap = new TileMap(layer, paletteSet, tileSet, layer, prioritySets, model);
            SetLevelImage();
            SetTilesetImage();
        }
        private void animationB_ValueChanged(object sender, EventArgs e)
        {
            if (updatingLevel) return;

            layer.AnimationL3 = (byte)animationB.Value;
        }
        private void l1width_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (updatingLevel) return;

            layer.LayerWidth[0] = (byte)l1width.SelectedIndex;

            tileMap = new TileMap(layer, paletteSet, tileSet, layer, prioritySets, model);
            SetLevelImage();

            Size s = LayerSize(0);
            model.TileMapSizes[layer.TileMapL1] = (ushort)((s.Width / 16) * (s.Height / 16));
        }
        private void l2width_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (updatingLevel) return;

            layer.LayerWidth[1] = (byte)l2width.SelectedIndex;

            tileMap = new TileMap(layer, paletteSet, tileSet, layer, prioritySets, model);
            SetLevelImage();

            Size s = LayerSize(1);
            model.TileMapSizes[layer.TileMapL2] = (ushort)((s.Width / 16) * (s.Height / 16));
        }
        private void l3width_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (updatingLevel) return;

            layer.LayerWidth[2] = (byte)l3width.SelectedIndex;

            tileMap = new TileMap(layer, paletteSet, tileSet, layer, prioritySets, model);
            SetLevelImage();

            Size s = LayerSize(2);
            model.TileMapSizes[layer.TileMapL3] = (ushort)((s.Width / 16) * (s.Height / 16));
        }
        private void l1height_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (updatingLevel) return;

            layer.LayerHeight[0] = (byte)l1height.SelectedIndex;

            tileMap = new TileMap(layer, paletteSet, tileSet, layer, prioritySets, model);
            SetLevelImage();

            Size s = LayerSize(0);
            model.TileMapSizes[layer.TileMapL1] = (ushort)((s.Width / 16) * (s.Height / 16));
        }
        private void l2height_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (updatingLevel) return;

            layer.LayerHeight[1] = (byte)l2height.SelectedIndex;

            tileMap = new TileMap(layer, paletteSet, tileSet, layer, prioritySets, model);
            SetLevelImage();

            Size s = LayerSize(1);
            model.TileMapSizes[layer.TileMapL2] = (ushort)((s.Width / 16) * (s.Height / 16));
        }
        private void l3height_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (updatingLevel) return;

            layer.LayerHeight[2] = (byte)l3height.SelectedIndex;

            tileMap = new TileMap(layer, paletteSet, tileSet, layer, prioritySets, model);
            SetLevelImage();

            Size s = LayerSize(2);
            model.TileMapSizes[layer.TileMapL3] = (ushort)((s.Width / 16) * (s.Height / 16));
        }
        private void layerL2LeftShift_ValueChanged(object sender, EventArgs e)
        {
            if (updatingLevel) return;

            layer.LeftShiftL2 = (byte)layerL2LeftShift.Value;
        }
        private void layerL2UpShift_ValueChanged(object sender, EventArgs e)
        {
            if (updatingLevel) return;

            layer.UpShiftL2 = (byte)layerL2UpShift.Value;
        }
        private void layerL3LeftShift_ValueChanged(object sender, EventArgs e)
        {
            if (updatingLevel) return;

            layer.LeftShiftL3 = (byte)layerL3LeftShift.Value;
        }
        private void layerL3UpShift_ValueChanged(object sender, EventArgs e)
        {
            if (updatingLevel) return;

            layer.UpShiftL3 = (byte)layerL3UpShift.Value;
        }
        private void layerMaskHighX_ValueChanged(object sender, EventArgs e)
        {
            if (updatingLevel) return;

            layer.MaskHighX = (byte)layerMaskHighX.Value;

            if (state.Mask)
                pictureBoxLevel.Invalidate();
        }
        private void layerMaskHighY_ValueChanged(object sender, EventArgs e)
        {
            if (updatingLevel) return;

            layer.MaskHighY = (byte)layerMaskHighY.Value;

            if (state.Mask)
                pictureBoxLevel.Invalidate();
        }
        private void mapGFXSet1Num_ValueChanged(object sender, EventArgs e)
        {
            if (updatingLevel) return;

            layer.GraphicSetA = (byte)mapGFXSet1Num.Value;

            tileSet = new Tileset(layer, paletteSet, model);
            tileMap = new TileMap(layer, paletteSet, tileSet, layer, prioritySets, model);
            SetLevelImage();
            SetTilesetImage();
        }
        private void mapGFXSet2Num_ValueChanged(object sender, EventArgs e)
        {
            if (updatingLevel) return;

            layer.GraphicSetB = (byte)mapGFXSet2Num.Value;

            tileSet = new Tileset(layer, paletteSet, model);
            tileMap = new TileMap(layer, paletteSet, tileSet, layer, prioritySets, model);
            SetLevelImage();
            SetTilesetImage();
        }
        private void mapGFXSet3Num_ValueChanged(object sender, EventArgs e)
        {
            if (updatingLevel) return;

            layer.GraphicSetC = (byte)mapGFXSet3Num.Value;

            tileSet = new Tileset(layer, paletteSet, model);
            tileMap = new TileMap(layer, paletteSet, tileSet, layer, prioritySets, model);
            SetLevelImage();
            SetTilesetImage();
        }
        private void mapGFXSet4Num_ValueChanged(object sender, EventArgs e)
        {
            if (updatingLevel) return;

            layer.GraphicSetD = (byte)mapGFXSet4Num.Value;

            tileSet = new Tileset(layer, paletteSet, model);
            tileMap = new TileMap(layer, paletteSet, tileSet, layer, prioritySets, model);
            SetLevelImage();
            SetTilesetImage();
        }
        private void mapGFXSetL3Num_ValueChanged(object sender, EventArgs e)
        {
            if (updatingLevel) return;

            layer.GraphicSetL3 = (byte)mapGFXSetL3Num.Value;

            tileSet = new Tileset(layer, paletteSet, model);
            tileMap = new TileMap(layer, paletteSet, tileSet, layer, prioritySets, model);
            SetLevelImage();
            SetTilesetImage();
        }
        private void mapPhysicalMapNum_ValueChanged(object sender, EventArgs e)
        {
            if (updatingLevel) return;

            layer.PhysicalMap = (ushort)mapPhysicalMapNum.Value;

            if (state.PhysicalLayer)
            {
                pictureBoxLevel.Invalidate();
                if (tabControl2.SelectedIndex == 0)
                    pictureBoxTilesetL1.Invalidate();
            }
        }
        private void mapSetL3Priority_CheckedChanged(object sender, EventArgs e)
        {
            mapSetL3Priority.ForeColor = mapSetL3Priority.Checked ? Color.Black : SystemColors.ControlDark;

            if (updatingLevel) return;

            layer.TopPriorityL3 = mapSetL3Priority.Checked;

            tileMap = new TileMap(layer, paletteSet, tileSet, layer, prioritySets, model);
            SetLevelImage();
        }
        private void mapTilemapL1Num_ValueChanged(object sender, EventArgs e)
        {
            if (updatingLevel) return;
            
            layer.TileMapL1 = (ushort)mapTilemapL1Num.Value;

            tileMap = new TileMap(layer, paletteSet, tileSet, layer, prioritySets, model);
            SetLevelImage();
        }
        private void mapTilemapL2Num_ValueChanged(object sender, EventArgs e)
        {
            if (updatingLevel) return;

            layer.TileMapL2 = (ushort)mapTilemapL2Num.Value;

            tileMap = new TileMap(layer, paletteSet, tileSet, layer, prioritySets, model);
            SetLevelImage();
        }
        private void mapTilemapL3Num_ValueChanged(object sender, EventArgs e)
        {
            if (updatingLevel) return;

            layer.TileMapL3 = (ushort)mapTilemapL3Num.Value;

            tileMap = new TileMap(layer, paletteSet, tileSet, layer, prioritySets, model);
            SetLevelImage();
        }
        private void mapTilesetL1Num_ValueChanged(object sender, EventArgs e)
        {
            if (updatingLevel) return;

            layer.TileSetL1 = (byte)mapTilesetL1Num.Value;

            tileSet = new Tileset(layer, paletteSet, model);
            tileMap = new TileMap(layer, paletteSet, tileSet, layer, prioritySets, model);
            SetTilesetImage();
        }
        private void mapTilesetL2Num_ValueChanged(object sender, EventArgs e)
        {
            if (updatingLevel) return;

            layer.TileSetL2 = (byte)mapTilesetL2Num.Value;

            tileSet = new Tileset(layer, paletteSet, model);
            tileMap = new TileMap(layer, paletteSet, tileSet, layer, prioritySets, model);
            SetTilesetImage();
        }
        private void message_ValueChanged(object sender, EventArgs e)
        {
            if (updatingLevel) return;

            layer.MessageBox = (byte)message.Value;

            messageName.SelectedIndex = (int)message.Value;
            tbMessageName.Text = messageNames[(int)message.Value];
        }
        //aaa
        private void messageName_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (updatingLevel) return;

            message.Value = messageName.SelectedIndex;

            tbMessageName.Text = messageNames[messageName.SelectedIndex];
        }
        private void music_ValueChanged(object sender, EventArgs e)
        {
            if (updatingLevel) return;

            layer.Music = (byte)music.Value;

            musicName.SelectedIndex = (int)music.Value;
        }
        private void musicName_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (updatingLevel) return;

            music.Value = musicName.SelectedIndex;
        }
        private void topSync_ValueChanged(object sender, EventArgs e)
        {
            if (updatingLevel) return;

            layer.Scrolling = (byte)topSync.Value;
        }
        private void useWorldMapBG_CheckedChanged(object sender, EventArgs e)
        {
            useWorldMapBG.ForeColor = useWorldMapBG.Checked ? Color.Black : SystemColors.ControlDark;

            if (updatingLevel) return;

            layer.WorldMapBG = useWorldMapBG.Checked;
        }
        private void mapBattleBG_ValueChanged(object sender, EventArgs e)
        {
            if (updatingLevel) return;

            layer.BattleBG = (byte)mapBattleBG.Value;
            mapBattleBGName.SelectedIndex = (int)mapBattleBG.Value;
        }
        private void mapBattleBGName_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (updatingLevel) return;

            mapBattleBG.Value = mapBattleBGName.SelectedIndex;
        }
        private void mapBattleZone_ValueChanged(object sender, EventArgs e)
        {
            if (updatingLevel) return;

            layer.BattleZone = (byte)mapBattleZone.Value;
        }
        private void mapRandomBattles_CheckedChanged(object sender, EventArgs e)
        {
            mapRandomBattles.ForeColor = mapRandomBattles.Checked ? Color.Black : SystemColors.ControlDark;

            if (updatingLevel) return;

            layer.RandomBattle = mapRandomBattles.Checked;
        }
        private void layerEffects_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (updatingLevel) return;

            layer.WarpEnabled = this.layerEffects.GetItemChecked(0);
            layer.HeatWaveL2 = this.layerEffects.GetItemChecked(1);
            layer.HeatWaveL1 = this.layerEffects.GetItemChecked(2);
            layer.SearchLights = this.layerEffects.GetItemChecked(3);
        }

        private void mapPaletteSetNum_ValueChanged(object sender, EventArgs e)
        {
            colorReds.Clear();
            colorGreens.Clear();
            colorBlues.Clear();
            redoColorReds.Clear();
            redoColorGreens.Clear();
            redoColorBlues.Clear();

            if (updatingLevel) return;

            layer.PaletteSet = (byte)mapPaletteSetNum.Value;

            paletteSet = paletteSets[levels[currentLevel].Layer.PaletteSet];
            tileSet = new Tileset(layer, paletteSet, model);
            tileMap = new TileMap(layer, paletteSet, tileSet, layer, prioritySets, model);

            SetLevelImage();
            SetTilesetImage();
            SetPaletteSetImage();
            SetGraphicSetImage();
            SetTileImage();
            SetSubtileImage();
        }
        private void paletteColor_ValueChanged(object sender, EventArgs e)
        {
            if (updatingLevel) return;

            currentColor = (int)paletteColor.Value;

            InitializeCurrentColor();
            palettePictureBox.Invalidate();
        }
        private void mapPaletteRedNum_ValueChanged(object sender, EventArgs e)
        {
            if (updatingLevel) return;

            paletteSet.PaletteColorRed[currentColor] = (byte)this.mapPaletteRedNum.Value;

            pictureBoxColor.BackColor = System.Drawing.Color.FromArgb((int)mapPaletteRedNum.Value, (int)mapPaletteGreenNum.Value, (int)mapPaletteBlueNum.Value);

            SetPaletteSetImage();

            if (paletteAutoUpdate.Checked)
            {
                if (currentLevel > 2)
                {
                    tileSet = new Tileset(layer, paletteSet, model);
                    tileMap = new TileMap(layer, paletteSet, tileSet, layer, prioritySets, model);
                }
                else
                {
                    wmTileSet = new WorldMapTileset(currentLevel, paletteSet, model);
                    wmTileMap = new WorldMapTilemap(currentLevel, layer, paletteSet, wmTileSet, model);
                }

                SetLevelImage();
                SetTilesetImage();
                SetGraphicSetImage();
                SetTileImage();
                SetSubtileImage();
            }
        }
        private void mapPaletteGreenNum_ValueChanged(object sender, EventArgs e)
        {
            if (updatingLevel) return;

            paletteSet.PaletteColorGreen[currentColor] = (byte)this.mapPaletteGreenNum.Value;

            pictureBoxColor.BackColor = System.Drawing.Color.FromArgb((int)mapPaletteRedNum.Value, (int)mapPaletteGreenNum.Value, (int)mapPaletteBlueNum.Value);

            SetPaletteSetImage();

            if (paletteAutoUpdate.Checked)
            {
                if (currentLevel > 2)
                {
                    tileSet = new Tileset(layer, paletteSet, model);
                    tileMap = new TileMap(layer, paletteSet, tileSet, layer, prioritySets, model);
                }
                else
                {
                    wmTileSet = new WorldMapTileset(currentLevel, paletteSet, model);
                    wmTileMap = new WorldMapTilemap(currentLevel, layer, paletteSet, wmTileSet, model);
                }

                SetLevelImage();
                SetTilesetImage();
                SetGraphicSetImage();
                SetTileImage();
                SetSubtileImage();
            }
        }
        private void mapPaletteBlueNum_ValueChanged(object sender, EventArgs e)
        {
            if (updatingLevel) return;

            paletteSet.PaletteColorBlue[currentColor] = (byte)this.mapPaletteBlueNum.Value;

            pictureBoxColor.BackColor = System.Drawing.Color.FromArgb((int)mapPaletteRedNum.Value, (int)mapPaletteGreenNum.Value, (int)mapPaletteBlueNum.Value);

            SetPaletteSetImage();

            if (paletteAutoUpdate.Checked)
            {
                if (currentLevel > 2)
                {
                    tileSet = new Tileset(layer, paletteSet, model);
                    tileMap = new TileMap(layer, paletteSet, tileSet, layer, prioritySets, model);
                }
                else
                {
                    wmTileSet = new WorldMapTileset(currentLevel, paletteSet, model);
                    wmTileMap = new WorldMapTilemap(currentLevel, layer, paletteSet, wmTileSet, model);
                }

                SetLevelImage();
                SetTilesetImage();
                SetGraphicSetImage();
                SetTileImage();
                SetSubtileImage();
            }
        }
        private void mapPaletteRedBar_Scroll(object sender, EventArgs e)
        {
            if (updatingLevel) return;

            mapPaletteRedNum.Value = mapPaletteRedBar.Value;
        }
        private void mapPaletteGreenBar_Scroll(object sender, EventArgs e)
        {
            if (updatingLevel) return;

            mapPaletteGreenNum.Value = mapPaletteGreenBar.Value;
        }
        private void mapPaletteBlueBar_Scroll(object sender, EventArgs e)
        {
            if (updatingLevel) return;

            mapPaletteBlueNum.Value = mapPaletteBlueBar.Value;
        }
        private void paletteAutoUpdate_CheckedChanged(object sender, EventArgs e)
        {

        }
        private void paletteUpdate_Click(object sender, EventArgs e)
        {
            if (currentLevel > 2)
            {
                tileSet = new Tileset(layer, paletteSet, model);
                tileMap = new TileMap(layer, paletteSet, tileSet, layer, prioritySets, model);
            }
            else
            {
                wmTileSet = new WorldMapTileset(currentLevel, paletteSet, model);
                wmTileMap = new WorldMapTilemap(currentLevel, layer, paletteSet, wmTileSet, model);
            }

            SetLevelImage();
            SetTilesetImage();
            SetGraphicSetImage();
            SetTileImage();
            SetSubtileImage();
        }

        private void palettePictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            currentColor = (e.X / 16) + ((e.Y / 16) * 16);
            paletteColor.Value = currentColor;
        }
        private void palettePictureBox_Paint(object sender, PaintEventArgs e)
        {
            if (paletteSetImage != null)
                e.Graphics.DrawImage(paletteSetImage, 0, 0);

            Point p = new Point(currentColor % 16 * 16, currentColor / 16 * 16);
            //if (p.Y == 0) p.Y++;
            overlay.DrawSelectionBox(e.Graphics, new Point(p.X + 15, p.Y + 15 - (p.Y % 16)), p, 1);
        }

        private void colorBalance_Click(object sender, EventArgs e)
        {
            panelColorBalance.Visible = !panelColorBalance.Visible;
        }
        private void coleditSelectCommand_SelectedIndexChanged(object sender, EventArgs e)
        {
            colEditComboBoxA.Enabled = false;
            colEditComboBoxB.Enabled = false;
            colEditValueA.Enabled = false;
            colEditReds.Enabled = false;
            colEditGreens.Enabled = false;
            colEditBlues.Enabled = false;
            colEditLabelA.Text = "";
            colEditLabelB.Text = "";
            colEditLabelC.Text = "";
            colEditLabelD.Text = "";

            switch (coleditSelectCommand.SelectedIndex)
            {
                case 0:
                    colEditComboBoxA.Enabled = true;
                    colEditComboBoxB.Enabled = true;
                    if (coleditSelectCommand.SelectedIndex == 0)
                        colEditLabelA.Text = "Switch";
                    colEditLabelB.Text = "with";
                    colEditComboBoxA.SelectedIndex = 0;
                    colEditComboBoxB.SelectedIndex = 0;
                    break;
                case 1: colEditLabelC.Text = "Add"; goto case 4;
                case 2: colEditLabelC.Text = "Subtract"; goto case 4;
                case 3: colEditLabelC.Text = "Multiply by"; goto case 4;
                case 4:
                    if (coleditSelectCommand.SelectedIndex == 4)
                        colEditLabelC.Text = "Divide by";
                    colEditLabelD.Text = "for";
                    colEditValueA.Enabled = true;
                    colEditReds.Enabled = true;
                    colEditGreens.Enabled = true;
                    colEditBlues.Enabled = true;
                    break;
                case 5: colEditLabelA.Text = "Equate"; goto case 0;
                case 6: colEditLabelC.Text = "Set to"; goto case 4;
            }
        }
        private void colEditReds_CheckedChanged(object sender, EventArgs e)
        {
            colEditReds.ForeColor = colEditReds.Checked ? Color.Black : Color.Gray;
        }
        private void colEditGreens_CheckedChanged(object sender, EventArgs e)
        {
            colEditGreens.ForeColor = colEditGreens.Checked ? Color.Black : Color.Gray;
        }
        private void colEditBlues_CheckedChanged(object sender, EventArgs e)
        {
            colEditBlues.ForeColor = colEditBlues.Checked ? Color.Black : Color.Gray;
        }
        private void colEditSelectNone_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < colEditColors.Items.Count; i++)
                colEditColors.SetItemChecked(i, false);
        }
        private void colEditSelectAll_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < colEditColors.Items.Count; i++)
                colEditColors.SetItemChecked(i, true);
        }
        private void colEditRowSelectAll_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            for (int i = e.Index < 8 ? e.Index : (e.Index - 8), o = 0; o < 16; i += 8, o++)
                colEditColors.SetItemChecked(i, e.Index < 8);
            e.NewValue = CheckState.Unchecked;
        }
        private void colEditApply_Click(object sender, EventArgs e)
        {
            int[] temp;
            temp = new int[paletteSet.PaletteColorRed.Length];
            paletteSet.PaletteColorRed.CopyTo(temp, 0); colorReds.Push(temp);
            temp = new int[paletteSet.PaletteColorGreen.Length];
            paletteSet.PaletteColorGreen.CopyTo(temp, 0); colorGreens.Push(temp);
            temp = new int[paletteSet.PaletteColorBlue.Length];
            paletteSet.PaletteColorBlue.CopyTo(temp, 0); colorBlues.Push(temp);

            int tempA = 0;
            int tempB = 0;
            for (int i = 0; i < 128; i++)
            {
                int index = ((i & 15) * 8) + (i / 16);
                if (colEditColors.GetItemChecked(index))
                {
                    switch (coleditSelectCommand.SelectedIndex)
                    {
                        case 0:
                            switch (colEditComboBoxA.SelectedIndex)
                            {
                                case 0: tempA = paletteSet.PaletteColorRed[i]; break;
                                case 1: tempA = paletteSet.PaletteColorGreen[i]; break;
                                case 2: tempA = paletteSet.PaletteColorBlue[i]; break;
                            }
                            switch (colEditComboBoxB.SelectedIndex)
                            {
                                case 0: tempB = paletteSet.PaletteColorRed[i]; break;
                                case 1: tempB = paletteSet.PaletteColorGreen[i]; break;
                                case 2: tempB = paletteSet.PaletteColorBlue[i]; break;
                            }
                            switch (colEditComboBoxA.SelectedIndex)
                            {
                                case 0: paletteSet.PaletteColorRed[i] = tempB; break;
                                case 1: paletteSet.PaletteColorGreen[i] = tempB; break;
                                case 2: paletteSet.PaletteColorBlue[i] = tempB; break;
                            }
                            switch (colEditComboBoxB.SelectedIndex)
                            {
                                case 0: paletteSet.PaletteColorRed[i] = tempA; break;
                                case 1: paletteSet.PaletteColorGreen[i] = tempA; break;
                                case 2: paletteSet.PaletteColorBlue[i] = tempA; break;
                            }
                            break;
                        case 1:
                            if (colEditReds.Checked)
                                paletteSet.PaletteColorRed[i] = (int)Math.Min(255, paletteSet.PaletteColorRed[i] + colEditValueA.Value);
                            if (colEditGreens.Checked)
                                paletteSet.PaletteColorGreen[i] = (int)Math.Min(255, paletteSet.PaletteColorGreen[i] + colEditValueA.Value);
                            if (colEditBlues.Checked)
                                paletteSet.PaletteColorBlue[i] = (int)Math.Min(255, paletteSet.PaletteColorBlue[i] + colEditValueA.Value);
                            break;
                        case 2:
                            if (colEditReds.Checked)
                                paletteSet.PaletteColorRed[i] = (int)Math.Max(0, paletteSet.PaletteColorRed[i] - colEditValueA.Value);
                            if (colEditGreens.Checked)
                                paletteSet.PaletteColorGreen[i] = (int)Math.Max(0, paletteSet.PaletteColorGreen[i] - colEditValueA.Value);
                            if (colEditBlues.Checked)
                                paletteSet.PaletteColorBlue[i] = (int)Math.Max(0, paletteSet.PaletteColorBlue[i] - colEditValueA.Value);
                            break;
                        case 3:
                            if (colEditReds.Checked)
                                paletteSet.PaletteColorRed[i] = (int)Math.Min(255, paletteSet.PaletteColorRed[i] * colEditValueA.Value);
                            if (colEditGreens.Checked)
                                paletteSet.PaletteColorGreen[i] = (int)Math.Min(255, paletteSet.PaletteColorGreen[i] * colEditValueA.Value);
                            if (colEditBlues.Checked)
                                paletteSet.PaletteColorBlue[i] = (int)Math.Min(255, paletteSet.PaletteColorBlue[i] * colEditValueA.Value);
                            break;
                        case 4:
                            if (colEditReds.Checked)
                                paletteSet.PaletteColorRed[i] /= (int)colEditValueA.Value;
                            if (colEditGreens.Checked)
                                paletteSet.PaletteColorGreen[i] /= (int)colEditValueA.Value;
                            if (colEditBlues.Checked)
                                paletteSet.PaletteColorBlue[i] /= (int)colEditValueA.Value;
                            break;
                        case 5:
                            switch (colEditComboBoxB.SelectedIndex)
                            {
                                case 0: tempA = paletteSet.PaletteColorRed[i]; break;
                                case 1: tempA = paletteSet.PaletteColorGreen[i]; break;
                                case 2: tempA = paletteSet.PaletteColorBlue[i]; break;
                            }
                            switch (colEditComboBoxA.SelectedIndex)
                            {
                                case 0: paletteSet.PaletteColorRed[i] = tempA; break;
                                case 1: paletteSet.PaletteColorGreen[i] = tempA; break;
                                case 2: paletteSet.PaletteColorBlue[i] = tempA; break;
                            }
                            break;
                        case 6:
                            if (colEditReds.Checked)
                                paletteSet.PaletteColorRed[i] = (int)colEditValueA.Value;
                            if (colEditGreens.Checked)
                                paletteSet.PaletteColorGreen[i] = (int)colEditValueA.Value;
                            if (colEditBlues.Checked)
                                paletteSet.PaletteColorBlue[i] = (int)colEditValueA.Value;
                            break;
                    }
                }
            }
            mapPaletteRedNum_ValueChanged(null, null);
        }
        private void colEditReset_Click(object sender, EventArgs e)
        {
            if (colorReds.Count == 0)
                return;
            for (int i = 0; i < colorReds.Count; i++)
            {
                redoColorReds.Push(paletteSet.PaletteColorRed);
                redoColorGreens.Push(paletteSet.PaletteColorGreen);
                redoColorBlues.Push(paletteSet.PaletteColorBlue);

                paletteSet.PaletteColorRed = colorReds.Peek();
                paletteSet.PaletteColorGreen = colorGreens.Peek();
                paletteSet.PaletteColorBlue = colorBlues.Peek();

                colorReds.Pop();
                colorGreens.Pop();
                colorBlues.Pop();
            }

            mapPaletteRedNum_ValueChanged(null, null);
        }
        private void colEditUndo_Click(object sender, EventArgs e)
        {
            if (colorReds.Count == 0)
                return;

            redoColorReds.Push(paletteSet.PaletteColorRed);
            redoColorGreens.Push(paletteSet.PaletteColorGreen);
            redoColorBlues.Push(paletteSet.PaletteColorBlue);

            paletteSet.PaletteColorRed = colorReds.Peek();
            paletteSet.PaletteColorGreen = colorGreens.Peek();
            paletteSet.PaletteColorBlue = colorBlues.Peek();

            colorReds.Pop();
            colorGreens.Pop();
            colorBlues.Pop();

            mapPaletteRedNum_ValueChanged(null, null);
        }
        private void colEditRedo_Click(object sender, EventArgs e)
        {
            if (redoColorReds.Count == 0)
                return;

            colorReds.Push(paletteSet.PaletteColorRed);
            colorGreens.Push(paletteSet.PaletteColorGreen);
            colorBlues.Push(paletteSet.PaletteColorBlue);

            paletteSet.PaletteColorRed = redoColorReds.Peek();
            paletteSet.PaletteColorGreen = redoColorGreens.Peek();
            paletteSet.PaletteColorBlue = redoColorBlues.Peek();

            redoColorReds.Pop();
            redoColorGreens.Pop();
            redoColorBlues.Pop();

            mapPaletteRedNum_ValueChanged(null, null);
        }

        #endregion
    }
}

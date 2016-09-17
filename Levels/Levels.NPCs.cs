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

        private LevelNPCs npcs;
        private int overNPC = 0;
        private int clickNPC = 0;
        private int overTreasure = 0;
        private int clickTreasure = 0;

        #endregion

        #region Methods

        private void InitializeNPCProperties()
        {
            updatingLevel = true;

            npcListBox.Items.Clear();

            if (npcs.NPCs.Count == 0)
            {
                panel13.Enabled = false;
                return;
            }
            else
                panel13.Enabled = true;

            for (int i = 0; i < npcs.NPCs.Count; i++)
                npcListBox.Items.Add("NPC #" + i.ToString());

            npcs.CurrentNPC = npcs.SelectedNPC = npcListBox.SelectedIndex = 0;

            RefreshNPCProperties();

            updatingLevel = false;
        }
        private void InitializeTreasureProperties()
        {
            updatingLevel = true;

            treasureListBox.Items.Clear();

            if (npcs.Treasures.Count == 0)
            {
                panel35.Enabled = false;
                return;
            }
            else
                panel35.Enabled = true;

            for (int i = 0; i < npcs.Treasures.Count; i++)
                treasureListBox.Items.Add("TREASURE #" + i.ToString());

            npcs.CurrentTreasure = npcs.SelectedTreasure = treasureListBox.SelectedIndex = 0;

            RefreshTreasureProperties();

            updatingLevel = false;
        }
        private void RefreshNPCProperties()
        {
            updatingLevel = true;

            npcEventPointer.Value = npcs.EventPointer;
            npcPalette.Value = npcs.Palette;
            npcCheckMem.Value = npcs.CheckMem;
            npcCheckBit.Value = npcs.CheckBit;
            npcXCoord.Value = npcs.CoordX;
            npcYCoord.Value = npcs.CoordY;
            npcSpeed.SelectedIndex = npcs.Speed;
            npcSpriteSet.Value = npcs.SpriteNum;
            npcSpriteIndex.Value = npcs.Action;
            npcWalkability.SetItemChecked(0, npcs.SolidifyActionPath);
            npcWalkability.SetItemChecked(1, npcs.WalkUnder);
            npcWalkability.SetItemChecked(2, npcs.WalkOver);
            npcWalkability.SetItemChecked(3, npcs.DontFaceOnTrigger);
            npcVehicle.SelectedIndex = npcs.Vehicle;
            npcRadialPosition.SelectedIndex = npcs.Facing;

            npcUnknownBits.SetItemChecked(0, npcs.Byte4bit7);
            npcUnknownBits.SetItemChecked(1, npcs.Byte8bit3);
            npcUnknownBits.SetItemChecked(2, npcs.Byte8bit4);
            npcUnknownBits.SetItemChecked(3, npcs.Byte8bit5);
            npcUnknownBits.SetItemChecked(4, npcs.Byte8bit6);
            npcUnknownBits.SetItemChecked(5, npcs.Byte8bit7);

            updatingLevel = false;
        }
        private void RefreshTreasureProperties()
        {
            updatingLevel = true;

            treasureCheckMem.Value = npcs.CheckMemTreasure;
            treasureCheckBit.Value = npcs.CheckBitTreasure;
            treasureXCoord.Value = npcs.CoordXTreasure;
            treasureYCoord.Value = npcs.CoordYTreasure;
            treasureType.SelectedIndex = npcs.TreasureType;
            switch (treasureType.SelectedIndex)
            {
                case 0:
                case 1:
                    treasurePropertyNum.Enabled = false;
                    treasurePropertyName.Enabled = false;
                    label84.Text = "";
                    label83.Text = "";
                    break;
                case 2:
                    treasurePropertyNum.Enabled = true;
                    treasurePropertyName.Enabled = false;
                    treasurePropertyNum.Maximum = 255;
                    treasurePropertyNum.Increment = 1;
                    treasurePropertyNum.Value = npcs.PropertyNum;
                    label84.Text = "Pack #";
                    label83.Text = "";
                    break;
                case 3:
                    treasurePropertyNum.Enabled = true;
                    treasurePropertyName.Enabled = true;
                    treasurePropertyNum.Maximum = 255;
                    treasurePropertyNum.Increment = 1;
                    treasurePropertyNum.Value = npcs.PropertyNum;
                    treasurePropertyName.SelectedIndex = npcs.PropertyNum;
                    label84.Text = "Item #";
                    label83.Text = "Item Name";
                    break;
                case 4:
                    treasurePropertyName.Enabled = false;
                    treasurePropertyNum.Enabled = true;
                    treasurePropertyNum.Maximum = 25500;
                    treasurePropertyNum.Increment = 100;
                    treasurePropertyNum.Value = npcs.PropertyNum * 100;
                    label84.Text = "GP Amount";
                    label83.Text = "";
                    break;
            }

            updatingLevel = false;
        }

        //madsiur
        private bool CalculateFreeNPCSpace()
        {
            int used = 0;

            for (int i = 0; i < Model.NUM_LOCATIONS; i++)
            {
                for (int a = 0; a < levels[i].LevelNPCs.NPCs.Count; a++)
                {
                    used += 9;

                    if (used + 9 > Model.SIZE_NPC_DATA)
                    {
                        MessageBox.Show("WARNING: Cannot insert the NPC. The total number of NPCs for all levels has exceeded the maximum allotted space.", "TOTAL NPCS LENGTH EXCEEDED", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return false;
                    }
                }
            }

            return true;
        }
        private bool CalculateFreeTreasureSpace()
        {
            int used = 0;

            for (int i = 0; i < Model.NUM_LOCATIONS; i++)
            {
                for (int a = 0; a < levels[i].LevelNPCs.Treasures.Count; a++)
                {
                    used += 5;

                    if (used + 5 > Model.SIZE_CHEST_DATA)
                    {
                        MessageBox.Show("WARNING: Cannot insert the treasure. The total number of treasures for all levels has exceeded the maximum allotted space.", "TOTAL TREASURES LENGTH EXCEEDED", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return false;
                    }
                }
            }

            return true;
        }

        #endregion

        #region Event Handlers

        private void npcListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (updatingLevel) return;

            npcs.CurrentNPC = npcListBox.SelectedIndex;
            npcs.SelectedNPC = npcListBox.SelectedIndex;

            RefreshNPCProperties();

            if (state.Objects)
                pictureBoxLevel.Invalidate();
        }
        private void npcInsertObject_Click(object sender, EventArgs e)
        {
            if (!CalculateFreeNPCSpace()) return;

            this.npcListBox.Focus();
            if (npcs.NPCs.Count < 32)
            {
                if (npcListBox.Items.Count > 0)
                    npcs.AddNewNPC(npcListBox.SelectedIndex + 1);
                else
                {
                    panel13.Enabled = true;
                    npcs.AddNewNPC(0);
                }

                int reselect;

                if (npcListBox.Items.Count > 0)
                    reselect = npcListBox.SelectedIndex;
                else
                    reselect = -1;

                npcListBox.BeginUpdate();
                npcListBox.Items.Clear();

                for (int i = 0; i < npcs.NPCs.Count; i++)
                    npcListBox.Items.Add("NPC #" + i.ToString());

                npcListBox.SelectedIndex = reselect + 1;
                npcListBox.EndUpdate();
            }
            else
                MessageBox.Show("WARNING: Cannot insert anymore NPCs. The maximum number of NPCs allowed is 32.", "WARNING: Cannot insert any more NPCs", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }
        private void npcRemoveObject_Click(object sender, EventArgs e)
        {
            npcListBox.Focus();
            if (npcListBox.SelectedIndex != -1 && npcs.CurrentNPC == npcListBox.SelectedIndex)
            {
                npcs.RemoveCurrentNPC();

                int reselect = npcListBox.SelectedIndex;
                if (reselect == npcListBox.Items.Count - 1)
                    reselect--;

                npcListBox.BeginUpdate();
                npcListBox.Items.Clear();

                for (int i = 0; i < npcs.NPCs.Count; i++)
                    npcListBox.Items.Add("NPC #" + i.ToString());

                if (npcListBox.Items.Count > 0)
                    npcListBox.SelectedIndex = reselect;
                else
                {
                    npcListBox.SelectedIndex = -1;
                    pictureBoxLevel.Invalidate();

                    panel13.Enabled = false;

                    RefreshNPCProperties();
                }
                npcListBox.EndUpdate();
            }
        }
        private void npcEventPointer_ValueChanged(object sender, EventArgs e)
        {
            if (updatingLevel) return;

            npcs.EventPointer = (int)npcEventPointer.Value;
        }
        private void npcPalette_ValueChanged(object sender, EventArgs e)
        {
            if (updatingLevel) return;

            npcs.Palette = (byte)npcPalette.Value;

            if (state.Objects)
                pictureBoxLevel.Invalidate();
        }
        private void npcXCoord_ValueChanged(object sender, EventArgs e)
        {
            if (updatingLevel) return;

            npcs.CoordX = (byte)npcXCoord.Value;

            if (state.Objects && !waitBothCoords)
                pictureBoxLevel.Invalidate();
        }
        private void npcYCoord_ValueChanged(object sender, EventArgs e)
        {
            if (updatingLevel) return;

            npcs.CoordY = (byte)npcYCoord.Value;

            if (state.Objects && !waitBothCoords)
                pictureBoxLevel.Invalidate();
        }
        private void npcSpeed_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (updatingLevel) return;

            npcs.Speed = (byte)npcSpeed.SelectedIndex;
        }
        private void npcSpriteSet_ValueChanged(object sender, EventArgs e)
        {
            if (updatingLevel) return;

            npcs.SpriteNum = (byte)npcSpriteSet.Value;

            if (state.Objects)
                pictureBoxLevel.Invalidate();
        }
        private void npcSpriteIndex_ValueChanged(object sender, EventArgs e)
        {
            if (updatingLevel) return;

            npcs.Action = (byte)npcSpriteIndex.Value;
        }
        private void npcWalkability_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (updatingLevel) return;

            npcs.SolidifyActionPath = npcWalkability.GetItemChecked(0);
            npcs.WalkUnder = npcWalkability.GetItemChecked(1);
            npcs.WalkOver = npcWalkability.GetItemChecked(2);
            npcs.DontFaceOnTrigger = npcWalkability.GetItemChecked(3);
        }
        private void npcVehicle_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (updatingLevel) return;

            npcs.Vehicle = (byte)npcVehicle.SelectedIndex;
        }
        private void npcRadialPosition_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (updatingLevel) return;

            npcs.Facing = (byte)npcRadialPosition.SelectedIndex;

            pictureBoxLevel.Invalidate();
        }
        private void npcCheckMem_ValueChanged(object sender, EventArgs e)
        {
            if (updatingLevel) return;

            npcs.CheckMem = (ushort)npcCheckMem.Value;
        }
        private void npcCheckBit_ValueChanged(object sender, EventArgs e)
        {
            if (updatingLevel) return;

            npcs.CheckBit = (byte)npcCheckBit.Value;
        }

        private void npcUnknownBits_SelectedIndexChanged(object sender, EventArgs e)
        {
            npcs.Byte4bit7 = npcUnknownBits.GetItemChecked(0);
            npcs.Byte8bit3 = npcUnknownBits.GetItemChecked(1);
            npcs.Byte8bit4 = npcUnknownBits.GetItemChecked(2);
            npcs.Byte8bit5 = npcUnknownBits.GetItemChecked(3);
            npcs.Byte8bit6 = npcUnknownBits.GetItemChecked(4);
            npcs.Byte8bit7 = npcUnknownBits.GetItemChecked(5);
        }
        
        private void treasureListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (updatingLevel) return;

            npcs.CurrentTreasure = treasureListBox.SelectedIndex;
            npcs.SelectedTreasure = treasureListBox.SelectedIndex;

            RefreshTreasureProperties();

            if (state.Treasures)
                pictureBoxLevel.Invalidate();
        }
        private void buttonInsertTreasure_Click(object sender, EventArgs e)
        {
            if (!CalculateFreeTreasureSpace()) return;

            this.treasureListBox.Focus();
            if (npcs.Treasures.Count < 16)
            {
                if (treasureListBox.Items.Count > 0)
                    npcs.AddNewTreasure(treasureListBox.SelectedIndex + 1);
                else
                {
                    panel35.Enabled = true;
                    npcs.AddNewTreasure(0);
                }

                int reselect;

                if (treasureListBox.Items.Count > 0)
                    reselect = treasureListBox.SelectedIndex;
                else
                    reselect = -1;

                treasureListBox.BeginUpdate();
                treasureListBox.Items.Clear();

                for (int i = 0; i < npcs.Treasures.Count; i++)
                    treasureListBox.Items.Add("TREASURE #" + i.ToString());

                treasureListBox.SelectedIndex = reselect + 1;
                treasureListBox.EndUpdate();
            }
            else
                MessageBox.Show("WARNING: Cannot insert anymore treasures. The maximum number of treasures allowed is 16.", "WARNING: Cannot insert any more treasures", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }
        private void buttonDeleteTreasure_Click(object sender, EventArgs e)
        {
            treasureListBox.Focus();
            if (treasureListBox.SelectedIndex != -1 && npcs.CurrentTreasure == treasureListBox.SelectedIndex)
            {
                npcs.RemoveCurrentTreasure();

                int reselect = treasureListBox.SelectedIndex;
                if (reselect == treasureListBox.Items.Count - 1)
                    reselect--;

                treasureListBox.BeginUpdate();
                treasureListBox.Items.Clear();

                for (int i = 0; i < npcs.Treasures.Count; i++)
                    treasureListBox.Items.Add("TREASURE #" + i.ToString());

                if (treasureListBox.Items.Count > 0)
                    treasureListBox.SelectedIndex = reselect;
                else
                {
                    treasureListBox.SelectedIndex = -1;
                    pictureBoxLevel.Invalidate();

                    panel35.Enabled = false;

                    RefreshTreasureProperties();
                }
                treasureListBox.EndUpdate();
            }
        }
        private void treasureXCoord_ValueChanged(object sender, EventArgs e)
        {
            if (updatingLevel) return;

            npcs.CoordXTreasure = (byte)treasureXCoord.Value;

            if (state.Treasures && !waitBothCoords)
                pictureBoxLevel.Invalidate();
        }
        private void treasureYCoord_ValueChanged(object sender, EventArgs e)
        {
            if (updatingLevel) return;

            npcs.CoordYTreasure = (byte)treasureYCoord.Value;

            if (state.Treasures && !waitBothCoords)
                pictureBoxLevel.Invalidate();
        }
        private void treasureCheckMem_ValueChanged(object sender, EventArgs e)
        {
            if (updatingLevel) return;

            npcs.CheckMemTreasure = (ushort)treasureCheckMem.Value;
        }
        private void treasureCheckBit_ValueChanged(object sender, EventArgs e)
        {
            if (updatingLevel) return;

            npcs.CheckBitTreasure = (byte)treasureCheckBit.Value;
        }
        private void treasureType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (updatingLevel) return;

            npcs.TreasureType = (byte)treasureType.SelectedIndex;

            updatingLevel = true;

            switch (treasureType.SelectedIndex)
            {
                case 0:
                case 1:
                    treasurePropertyNum.Enabled = false;
                    treasurePropertyName.Enabled = false;
                    label84.Text = "";
                    label83.Text = "";
                    break;
                case 2:
                    treasurePropertyNum.Enabled = true;
                    treasurePropertyName.Enabled = false;
                    treasurePropertyNum.Maximum = 255;
                    treasurePropertyNum.Increment = 1;
                    treasurePropertyNum.Value = npcs.PropertyNum;
                    label84.Text = "Pack #";
                    label83.Text = "";
                    break;
                case 3:
                    treasurePropertyNum.Enabled = true;
                    treasurePropertyName.Enabled = true;
                    treasurePropertyNum.Maximum = 255;
                    treasurePropertyNum.Increment = 1;
                    treasurePropertyNum.Value = npcs.PropertyNum;
                    treasurePropertyName.SelectedIndex = npcs.PropertyNum;
                    label84.Text = "Item #";
                    label83.Text = "Item Name";
                    break;
                case 4:
                    treasurePropertyName.Enabled = false;
                    treasurePropertyNum.Enabled = true;
                    treasurePropertyNum.Maximum = 25500;
                    treasurePropertyNum.Increment = 100;
                    treasurePropertyNum.Value = npcs.PropertyNum * 100;
                    label84.Text = "GP Amount";
                    label83.Text = "";
                    break;
            }

            updatingLevel = false;
        }
        private void treasurePropertyNum_ValueChanged(object sender, EventArgs e)
        {
            if (updatingLevel) return;

            if (treasureType.SelectedIndex == 4)
                npcs.PropertyNum = (byte)(treasurePropertyNum.Value / 100);
            else
                npcs.PropertyNum = (byte)treasurePropertyNum.Value;

            if (treasureType.SelectedIndex == 3)
                treasurePropertyName.SelectedIndex = (int)treasurePropertyNum.Value;
        }
        private void treasurePropertyName_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (updatingLevel) return;

            treasurePropertyNum.Value = treasurePropertyName.SelectedIndex;
        }

        #endregion
    }
}

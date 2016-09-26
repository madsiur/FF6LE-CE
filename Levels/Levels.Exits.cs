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

        private LevelExits exits;

        private int overExitShort = 0;
        private int overExitLong = 0;
        private int clickExitShort = 0;
        private int clickExitLong = 0;

        #endregion

        #region Methods

        private void InitializeExitShortProperties()
        {
            updatingLevel = true;

            exitShortListBox.Items.Clear();

            if (exits.ExitsShort.Count == 0)
            {
                panel15.Enabled = false;
                return;
            }
            else
                panel15.Enabled = true;

            for (int i = 0; i < exits.ExitsShort.Count; i++)
                exitShortListBox.Items.Add("EXIT #" + i.ToString());

            exits.CurrentExitShort = exits.SelectedExitShort = exitShortListBox.SelectedIndex = 0;

            RefreshExitShortProperties();

            updatingLevel = false;
        }
        private void InitializeExitLongProperties()
        {
            updatingLevel = true;

            exitLongListBox.Items.Clear();

            if (exits.ExitsLong.Count == 0)
            {
                panel16.Enabled = false;
                return;
            }
            else
                panel16.Enabled = true;

            for (int i = 0; i < exits.ExitsLong.Count; i++)
                exitLongListBox.Items.Add("EXIT #" + i.ToString());

            exits.CurrentExitLong = exits.SelectedExitLong = exitLongListBox.SelectedIndex = 0;

            RefreshExitLongProperties();

            updatingLevel = false;
        }
        private void RefreshExitShortProperties()
        {
            updatingLevel = true;

            exitShortXCoord.Value = exits.CoordXShort;
            exitShortYCoord.Value = exits.CoordYShort;
            exitShortToWorldMap.Checked = exits.ToWorldMapShort;
            if (exits.ToWorldMapShort)
            {
                exitShortDestination.Enabled = false;
                exitShortDestination.SelectedIndex = 0;
            }
            else
            {
                exitShortDestination.Enabled = true;
                exitShortDestination.SelectedIndex = exits.DestinationShort;
            }
            exitDestinationXCoord.Value = exits.DestinationXCoordShort;
            exitDestinationYCoord.Value = exits.DestinationYCoordShort;
            exitDestinationFacing.SelectedIndex = exits.DestinationFacingShort;
            exitShortShowMessage.Checked = exits.ShowMessageShort;

            exitShortUnknownBits.SetItemChecked(0, exits.Byte3bit1Short);
            exitShortUnknownBits.SetItemChecked(1, exits.Byte3bit2Short);

            updatingLevel = false;
        }
        private void RefreshExitLongProperties()
        {
            updatingLevel = true;

            exitLongXCoord.Value = exits.CoordXLong;
            exitLongYCoord.Value = exits.CoordYLong;
            exitLongWidth.Value = exits.CoordWidthLong;
            exitLongDirection.SelectedIndex = exits.DirectionLong;
            exitLongToWorldMap.Checked = exits.ToWorldMapLong;
            if (exits.ToWorldMapLong)
            {
                exitLongDestination.Enabled = false;
                exitLongDestination.SelectedIndex = 0;
            }
            else
            {
                exitLongDestination.Enabled = true;
                exitLongDestination.SelectedIndex = exits.DestinationLong;
            }
            exitLongDestinationXCoord.Value = exits.DestinationXCoordLong;
            exitLongDestinationYCoord.Value = exits.DestinationYCoordLong;
            exitLongDestinationFacing.SelectedIndex = exits.DestinationFacingLong;
            exitLongShowMessage.Checked = exits.ShowMessageLong;

            exitLongUnknownBits.SetItemChecked(0, exits.Byte3bit1Long);
            exitLongUnknownBits.SetItemChecked(1, exits.Byte3bit2Long);

            updatingLevel = false;
        }

        //madsiur
        private bool CalculateFreeExitShortSpace()
        {
            int used = 0;

            for (int i = 0; i < Model.NUM_LOCATIONS; i++)
            {
                for (int a = 0; a < levels[i].LevelExits.ExitsShort.Count; a++)
                {
                    used += 6;

                    if (used + 6 > Model.SIZE_SHORT_EXIT_DATA)
                    {
                        MessageBox.Show("WARNING: Cannot insert the exit field. The total number of short exit fields for all levels has exceeded the maximum allotted space.", "TOTAL SHORT EXITS LENGTH EXCEEDED", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return false;
                    }
                }
            }

            return true;
        }

        //madsiur
        private bool CalculateFreeExitLongSpace()
        {
            int used = 0;

            for (int i = 0; i < Model.NUM_LOCATIONS; i++)
            {
                for (int a = 0; a < levels[i].LevelExits.ExitsLong.Count; a++)
                {
                    used += 7;

                    if (used + 7 > Model.SIZE_LONG_EXIT_DATA)
                    {
                        MessageBox.Show("WARNING: Cannot insert the exit field. The total number of long exit fields for all levels has exceeded the maximum allotted space.", "TOTAL LONG EXITS LENGTH EXCEEDED", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return false;
                    }
                }
            }

            return true;
        }

        #endregion

        #region Event Handlers

        private void exitShortListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (updatingLevel) return;

            exits.CurrentExitShort = exitShortListBox.SelectedIndex;
            exits.SelectedExitShort = exitShortListBox.SelectedIndex;

            RefreshExitShortProperties();

            if (state.Exits)
                pictureBoxLevel.Invalidate();
        }
        private void buttonInsertExitShort_Click(object sender, EventArgs e)
        {
            if (!CalculateFreeExitShortSpace()) return;

            this.exitShortListBox.Focus();
            if (exits.ExitsShort.Count < 48)
            {
                if (exitShortListBox.Items.Count > 0)
                    exits.AddNewExitShort(exitShortListBox.SelectedIndex + 1);
                else
                {
                    exits.AddNewExitShort(0);
                    panel15.Enabled = true;
                }

                int reselect;

                if (exitShortListBox.Items.Count > 0)
                    reselect = exitShortListBox.SelectedIndex;
                else
                    reselect = -1;

                exitShortListBox.BeginUpdate();
                exitShortListBox.Items.Clear();

                for (int i = 0; i < exits.ExitsShort.Count; i++)
                    exitShortListBox.Items.Add("EXIT #" + i.ToString());

                exitShortListBox.SelectedIndex = reselect + 1;
                exitShortListBox.EndUpdate();
            }
            else
                MessageBox.Show("WARNING: Cannot insert anymore exit fields. The maximum number of short exit fields allowed is 48.", "WARNING: Cannot insert any more exit fields", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }
        private void buttonDeleteExitShort_Click(object sender, EventArgs e)
        {
            exitShortListBox.Focus();
            if (exitShortListBox.SelectedIndex != -1 && exits.CurrentExitShort == exitShortListBox.SelectedIndex)
            {
                exits.RemoveCurrentExitShort();

                int reselect = exitShortListBox.SelectedIndex;
                if (reselect == exitShortListBox.Items.Count - 1)
                    reselect--;

                exitShortListBox.BeginUpdate();
                exitShortListBox.Items.Clear();

                for (int i = 0; i < exits.ExitsShort.Count; i++)
                    exitShortListBox.Items.Add("EXIT #" + i.ToString());

                if (exitShortListBox.Items.Count > 0)
                    exitShortListBox.SelectedIndex = reselect;
                else
                {
                    exitShortListBox.SelectedIndex = -1;
                    pictureBoxLevel.Invalidate();

                    panel15.Enabled = false;

                    RefreshExitShortProperties();
                }
                exitShortListBox.EndUpdate();
            }
        }
        private void exitShortXCoord_ValueChanged(object sender, EventArgs e)
        {
            if (updatingLevel) return;

            exits.CoordXShort = (byte)exitShortXCoord.Value;

            if (state.Exits && !waitBothCoords)
                pictureBoxLevel.Invalidate();
        }
        private void exitShortYCoord_ValueChanged(object sender, EventArgs e)
        {
            if (updatingLevel) return;

            exits.CoordYShort = (byte)exitShortYCoord.Value;

            if (state.Exits && !waitBothCoords)
                pictureBoxLevel.Invalidate();
        }
        private void exitShortToWorldMap_CheckedChanged(object sender, EventArgs e)
        {
            exitShortToWorldMap.ForeColor = exitShortToWorldMap.Checked ? Color.Black : SystemColors.ControlDark;

            if (updatingLevel) return;

            exits.ToWorldMapShort = exitShortToWorldMap.Checked;

            exitShortDestination.Enabled = !exits.ToWorldMapShort;
        }
        private void exitShortDestination_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (updatingLevel) return;

            exits.DestinationShort = (ushort)exitShortDestination.SelectedIndex;
        }
        private void exitDestinationXCoord_ValueChanged(object sender, EventArgs e)
        {
            if (updatingLevel) return;

            exits.DestinationXCoordShort = (byte)exitDestinationXCoord.Value;
        }
        private void exitDestinationYCoord_ValueChanged(object sender, EventArgs e)
        {
            if (updatingLevel) return;

            exits.DestinationYCoordShort = (byte)exitDestinationYCoord.Value;
        }
        private void exitDestinationFacing_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (updatingLevel) return;

            exits.DestinationFacingShort = (byte)exitDestinationFacing.SelectedIndex;
        }
        private void exitShortUnknownBits_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (updatingLevel) return;

            exits.Byte3bit1Short = exitShortUnknownBits.GetItemChecked(0);
            exits.Byte3bit2Short = exitShortUnknownBits.GetItemChecked(1);
        }
        private void exitShortShowMessage_CheckedChanged(object sender, EventArgs e)
        {
            exitShortShowMessage.ForeColor = exitShortShowMessage.Checked ? SystemColors.ControlText : SystemColors.ControlDark;

            if (updatingLevel) return;

            exits.ShowMessageShort = exitShortShowMessage.Checked;
        }

        private void exitLongListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (updatingLevel) return;

            exits.CurrentExitLong = exitLongListBox.SelectedIndex;
            exits.SelectedExitLong = exitLongListBox.SelectedIndex;

            RefreshExitLongProperties();

            if (state.Exits)
                pictureBoxLevel.Invalidate();
        }
        private void buttonInsertExitLong_Click(object sender, EventArgs e)
        {
            if (!CalculateFreeExitLongSpace()) return;

            this.exitLongListBox.Focus();
            if (exits.ExitsLong.Count < 8)
            {
                if (exitLongListBox.Items.Count > 0)
                    exits.AddNewExitLong(exitLongListBox.SelectedIndex + 1);
                else
                {
                    exits.AddNewExitLong(0);
                    panel16.Enabled = true;
                }

                int reselect;

                if (exitLongListBox.Items.Count > 0)
                    reselect = exitLongListBox.SelectedIndex;
                else
                    reselect = -1;

                exitLongListBox.BeginUpdate();
                exitLongListBox.Items.Clear();

                for (int i = 0; i < exits.ExitsLong.Count; i++)
                    exitLongListBox.Items.Add("EXIT #" + i.ToString());

                exitLongListBox.SelectedIndex = reselect + 1;
                exitLongListBox.EndUpdate();
            }
            else
                MessageBox.Show("WARNING: Cannot insert anymore exit fields. The maximum number of long exit fields allowed is 8.", "WARNING: Cannot insert any more exit fields", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }
        private void buttonDeleteExitLong_Click(object sender, EventArgs e)
        {
            exitLongListBox.Focus();
            if (exitLongListBox.SelectedIndex != -1 && exits.CurrentExitLong == exitLongListBox.SelectedIndex)
            {
                exits.RemoveCurrentExitLong();

                int reselect = exitLongListBox.SelectedIndex;
                if (reselect == exitLongListBox.Items.Count - 1)
                    reselect--;

                exitLongListBox.BeginUpdate();
                exitLongListBox.Items.Clear();

                for (int i = 0; i < exits.ExitsLong.Count; i++)
                    exitLongListBox.Items.Add("EXIT #" + i.ToString());

                if (exitLongListBox.Items.Count > 0)
                    exitLongListBox.SelectedIndex = reselect;
                else
                {
                    exitLongListBox.SelectedIndex = -1;
                    pictureBoxLevel.Invalidate();

                    panel16.Enabled = false;
                    RefreshExitLongProperties();
                }
                exitLongListBox.EndUpdate();
            }
        }
        private void exitLongXCoord_ValueChanged(object sender, EventArgs e)
        {
            if (updatingLevel) return;

            exits.CoordXLong = (byte)exitLongXCoord.Value;

            if (state.Exits && !waitBothCoords)
                pictureBoxLevel.Invalidate();
        }
        private void exitLongYCoord_ValueChanged(object sender, EventArgs e)
        {
            if (updatingLevel) return;

            exits.CoordYLong = (byte)exitLongYCoord.Value;

            if (state.Exits && !waitBothCoords)
                pictureBoxLevel.Invalidate();
        }
        private void exitLongWidth_ValueChanged(object sender, EventArgs e)
        {
            if (updatingLevel) return;

            exits.CoordWidthLong = (byte)exitLongWidth.Value;

            if (state.Exits)
                pictureBoxLevel.Invalidate();
        }
        private void exitLongDirection_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (updatingLevel) return;

            exits.DirectionLong = (byte)exitLongDirection.SelectedIndex;

            if (state.Exits)
                pictureBoxLevel.Invalidate();
        }
        private void exitLongToWorldMap_CheckedChanged(object sender, EventArgs e)
        {
            exitLongToWorldMap.ForeColor = exitLongToWorldMap.Checked ? Color.Black : SystemColors.ControlDark;

            if (updatingLevel) return;

            exits.ToWorldMapLong = exitLongToWorldMap.Checked;

            exitLongDestination.Enabled = !exits.ToWorldMapLong;
        }
        private void exitLongDestination_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (updatingLevel) return;

            exits.DestinationLong = (ushort)exitLongDestination.SelectedIndex;
        }
        private void exitLongDestinationXCoord_ValueChanged(object sender, EventArgs e)
        {
            if (updatingLevel) return;

            exits.DestinationXCoordLong = (byte)exitLongDestinationXCoord.Value;
        }
        private void exitLongDestinationYCoord_ValueChanged(object sender, EventArgs e)
        {
            if (updatingLevel) return;

            exits.DestinationYCoordLong = (byte)exitLongDestinationYCoord.Value;
        }
        private void exitLongDestinationFacing_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (updatingLevel) return;

            exits.DestinationFacingLong = (byte)exitLongDestinationFacing.SelectedIndex;
        }
        private void exitLongUnknownBits_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (updatingLevel) return;

            exits.Byte3bit1Long = exitLongUnknownBits.GetItemChecked(0);
            exits.Byte3bit2Long = exitLongUnknownBits.GetItemChecked(1);
        }
        private void exitLongShowMessage_CheckedChanged(object sender, EventArgs e)
        {
            exitLongShowMessage.ForeColor = exitLongShowMessage.Checked ? SystemColors.ControlText : SystemColors.ControlDark;

            if (updatingLevel) return;

            exits.ShowMessageLong = exitLongShowMessage.Checked;
        }

        #endregion
    }
}

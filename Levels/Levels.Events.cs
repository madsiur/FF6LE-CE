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

        private LevelEvents events;

        private int overEvent = 0;
        private int clickEvent = 0;

        #endregion

        #region Methods

        private void InitializeEventProperties()
        {
            updatingLevel = true;

            eventListBox.Items.Clear();

            entranceEvent.Value = events.EntranceEvent;
            
            if (events.Events.Count == 0)
            {
                panel18.Enabled = false;
                return;
            }
            else
                panel18.Enabled = true;

            for (int i = 0; i < events.Events.Count; i++)
                eventListBox.Items.Add("EVENT #" + i.ToString());

            events.CurrentEvent = events.SelectedEvent = eventListBox.SelectedIndex = 0;

            RefreshEventProperties();

            updatingLevel = false;
        }
        private void RefreshEventProperties()
        {
            updatingLevel = true;

            eventXCoord.Value = events.CoordX;
            eventYCoord.Value = events.CoordY;
            eventEventNum.Value = events.EventNum;

            updatingLevel = false;
        }

        //madsiur
        private bool CalculateFreeEventSpace()
        {
            int used = 0;

            for (int i = 0; i < Model.NUM_LOCATIONS; i++)
            {
                for (int a = 0; a < levels[i].LevelEvents.Events.Count; a++)
                {
                    used += 5;

                    if (used + 5 > Model.SIZE_EVENT_DATA)
                    {
                        MessageBox.Show("WARNING: Cannot insert the event field. The total number of event fields for all levels has exceeded the maximum allotted space.", "TOTAL EVENTS LENGTH EXCEEDED", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return false;
                    }
                }
            }

            return true;
        }

        #endregion

        #region Event Handlers

        private void entranceEvent_ValueChanged(object sender, EventArgs e)
        {
            if (updatingLevel) return;

            events.EntranceEvent = (int)entranceEvent.Value;
        }
        private void eventListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (updatingLevel) return;

            events.CurrentEvent = eventListBox.SelectedIndex;
            events.SelectedEvent = eventListBox.SelectedIndex;

            RefreshEventProperties();

            if (state.Events)
                pictureBoxLevel.Invalidate();
        }
        private void buttonInsertEvent_Click(object sender, EventArgs e)
        {
            if (!CalculateFreeEventSpace()) return;

            this.eventListBox.Focus();
            if (events.Events.Count < 72)
            {
                if (eventListBox.Items.Count > 0)
                    events.AddNewEvent(eventListBox.SelectedIndex + 1);
                else
                {
                    events.AddNewEvent(0);
                    panel18.Enabled = true;
                }

                int reselect;

                if (eventListBox.Items.Count > 0)
                    reselect = eventListBox.SelectedIndex;
                else
                    reselect = -1;

                eventListBox.BeginUpdate();
                eventListBox.Items.Clear();

                for (int i = 0; i < events.Events.Count; i++)
                    eventListBox.Items.Add("EVENT #" + i.ToString());

                eventListBox.SelectedIndex = reselect + 1;
                eventListBox.EndUpdate();
            }
            else
                MessageBox.Show("WARNING: Cannot insert anymore event fields. The maximum number of event fields allowed is 72.", "WARNING: Cannot insert any more event fields", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }
        private void buttonDeleteEvent_Click(object sender, EventArgs e)
        {
            eventListBox.Focus();
            if (eventListBox.SelectedIndex != -1 && events.CurrentEvent == eventListBox.SelectedIndex)
            {
                events.RemoveCurrentEvent();

                int reselect = eventListBox.SelectedIndex;
                if (reselect == eventListBox.Items.Count - 1)
                    reselect--;

                eventListBox.BeginUpdate();
                eventListBox.Items.Clear();

                for (int i = 0; i < events.Events.Count; i++)
                    eventListBox.Items.Add("EVENT #" + i.ToString());

                if (eventListBox.Items.Count > 0)
                    eventListBox.SelectedIndex = reselect;
                else
                {
                    eventListBox.SelectedIndex = -1;
                    pictureBoxLevel.Invalidate();

                    panel18.Enabled = false;

                    RefreshEventProperties();
                }
                eventListBox.EndUpdate();
            }
        }
        private void eventXCoord_ValueChanged(object sender, EventArgs e)
        {
            if (updatingLevel) return;

            events.CoordX = (byte)eventXCoord.Value;

            if (state.Events && !waitBothCoords)
                pictureBoxLevel.Invalidate();
        }
        private void eventYCoord_ValueChanged(object sender, EventArgs e)
        {
            if (updatingLevel) return;

            events.CoordY = (byte)eventYCoord.Value;

            if (state.Events && !waitBothCoords)
                pictureBoxLevel.Invalidate();
        }
        private void eventEventNum_ValueChanged(object sender, EventArgs e)
        {
            if (updatingLevel) return;

            events.EventNum = (int)eventEventNum.Value;

            pictureBoxLevel.Invalidate();
        }

        #endregion
    }
}

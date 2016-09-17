using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using FF3LE.Properties;

namespace FF3LE.Previewer
{
    public partial class Previewer : Form
    {
        #region Variables

        private Settings settings;
        private Model model;
        private ArrayList eventTriggers;
        private string romPath;
        private string emulatorPath = "INVALID";
        private bool rom = false, emulator = false, savestate = false, eventchoice = false, initializing = false;
        private int selectNum;
        LevelModel levelModel;

        private int behaviour;
        private enum Behaviours
        {
            EventPreviewer,
            LevelPreviewer,
            ActionPreviewer,
            BattlePreviewer
        }

        #endregion

        public Previewer(Model model, int num, int behaviour)
        {
            this.model = model;
            this.settings = Settings.Default;
            this.selectNum = num;
            this.eventTriggers = new ArrayList();
            this.behaviour = behaviour;

            if (model.LevelModel == null)
                model.CreateLevelModel();
            this.levelModel = model.LevelModel;

            InitializeComponent();
            InitializePreviewer();

            this.emulator = GetEmulator();
            if (num == 0)
                this.selectNumericUpDown_ValueChanged(null, null);
            UpdateGui();

            if (settings.PreviewFirstTime)
            {
                DialogResult result = MessageBox.Show("The generated Preview ROM should not be used for anything other than Previews.\nDoing so will yield unpredictable results. Do you understand?", "WARNING", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (result == DialogResult.Yes)
                {
                    settings.PreviewFirstTime = false;
                    settings.Save();
                }
            }
        }

        #region Methods

        private void InitializePreviewer()
        {
            this.initializing = true;

            this.Text = "PREVIEW LEVEL";
            this.label1.Text = "Level #";
            this.label2.Text = "SELECT ENTRANCE TO PREVIEW";
            this.selectNumericUpDown.Maximum = 511;

            this.argsTextBox.Text = settings.PreviewArguments;
            this.checkBox1.Checked = settings.PreviewDynamicRomName;

            this.toggleAssembleLevels.Checked = settings.PreviewAssembleLevels;
            this.enterEventCheckBox.Checked = settings.PreviewEnterEvent;

            romPath = GetRomPath();
            this.initializing = false;
        }
        private void ScanForEntrancesToLevel(int lvlNum)
        {
            Entrance ent;
            int save;

            foreach (Level lvl in levelModel.Levels) // For every level
            {
                save = lvl.LevelExits.CurrentExitShort; // Save current index to restore later
                for (int i = 0; i < lvl.LevelExits.ExitsShort.Count; i++) // For every event in each level
                {
                    lvl.LevelExits.CurrentExitShort = i;
                    if (lvl.LevelExits.DestinationShort == lvlNum) // If this exit points to the level we want
                    {
                        ent = new Entrance();
                        ent.Source = (ushort)lvl.LevelNum;
                        ent.LevelNum = (ushort)lvl.LevelExits.DestinationShort;
                        ent.CoordX = lvl.LevelExits.DestinationXCoordShort;
                        ent.CoordY = lvl.LevelExits.DestinationYCoordShort;
                        ent.RadialPosition = lvl.LevelExits.DestinationFacingShort;
                        ent.ShowMessage = lvl.LevelExits.ShowMessageShort;
                        ent.Flag = true; // Indicates an enter event
                        eventTriggers.Add(ent); // Add the event trigger
                    }
                }
                lvl.LevelExits.CurrentExitShort = save; // Restore current index for this levels events                        

                save = lvl.LevelExits.CurrentExitLong; // Save current index to restore later
                for (int i = 0; i < lvl.LevelExits.ExitsLong.Count; i++) // For every event in each level
                {
                    lvl.LevelExits.CurrentExitLong = i;
                    if (lvl.LevelExits.DestinationLong == lvlNum) // If this exit points to the level we want
                    {
                        ent = new Entrance();
                        ent.Source = (ushort)lvl.LevelNum;
                        ent.LevelNum = (ushort)lvl.LevelExits.DestinationLong;
                        ent.CoordX = lvl.LevelExits.DestinationXCoordLong;
                        ent.CoordY = lvl.LevelExits.DestinationYCoordLong;
                        ent.RadialPosition = lvl.LevelExits.DestinationFacingLong;
                        ent.ShowMessage = lvl.LevelExits.ShowMessageLong;
                        ent.Flag = true; // Indicates an enter event
                        eventTriggers.Add(ent); // Add the event trigger
                    }
                }
                lvl.LevelExits.CurrentExitLong = save; // Restore current index for this levels events                        
            }

        }

        // Launching
        private void Launch()
        {
            settings.PreviewArguments = argsTextBox.Text;
            settings.Save();
            if (rom && emulator && savestate && eventchoice)
                LaunchZSNES(this.emulatorPath, this.romPath, argsTextBox.Text);
            else
            {
                if (!rom)
                    MessageBox.Show("There was a problem generating the preview rom");
                if (!emulator)
                    MessageBox.Show("There is a problem with the emulator. ZSNESW is the only emulator supported.");
                if (!savestate)
                    MessageBox.Show("There was a problem generating the preview SaveState");
                if (!eventchoice)
                    MessageBox.Show("An invalid destination was selected to preview.");
            }
        }
        private bool Prelaunch()
        {
            this.rom = GeneratePreviewRom();
            this.savestate = GeneratePreviewSaveState();
            return (rom == savestate == true);
        }
        private void LaunchZSNES(string zsnesPath, string romPath, string args)
        {
            System.Diagnostics.Process proc = new System.Diagnostics.Process();

            proc.EnableRaisingEvents = false;
            proc.StartInfo.FileName = zsnesPath;
            proc.StartInfo.Arguments = args + " " + "\"" + romPath + "\"";
            proc.Start();

            this.Close();
        }
        private string SelectFile(string filter, string initDir, string title)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = filter;
            dialog.InitialDirectory = initDir;
            dialog.Title = title;
            return (dialog.ShowDialog() == DialogResult.OK)
               ? dialog.FileName : null;
        }

        // Generation
        private bool GeneratePreviewRom()
        {
            // Save all assemble flags
            //bool stats = model.AssembleStats;
            bool levels = model.AssembleLevels;
            //bool sprites = model.AssembleSprites;
            //bool scripts = model.AssembleScripts;
            bool final = model.AssembleFinal;
            bool ret = false;
            // Make backup of current data                
            byte[] backup = new byte[model.Data.Length];
            model.Data.CopyTo(backup, 0);

            // Assemble into backup
            //model.AssembleLevels = settings.PreviewAssembleLevels;
            //model.AssembleScripts = settings.PreviewAssembleScripts;
            //model.AssembleSprites = settings.PreviewAssembleSprites;
            //model.AssembleStats = settings.PreviewAssembleStats;
            model.AssembleFinal = false;

            if (!((this.behaviour == (int)Behaviours.EventPreviewer || this.behaviour == (int)Behaviours.ActionPreviewer) && this.eventListBox.SelectedIndex < 0 || this.eventListBox.SelectedIndex >= this.eventTriggers.Count))
            {
                this.model.Program.Assemble();
                PrepareImage();
                // Backup filename
                string fileNameBackup = model.GetFileName();
                // Generate preview name;
                this.romPath = GetRomPath();
                string oldFileName = model.GetFileName();

                model.SetFileName(romPath);
                ret = model.WriteRom(); // Save temp rom
                // Restore name
                model.SetFileName(oldFileName);
            }

            // Restore assemble flags
            model.AssembleFinal = final;
            model.AssembleLevels = levels;
            //model.AssembleScripts = scripts;
            //model.AssembleSprites = sprites;
            //model.AssembleStats = stats;
            //Restore Rom Image
            backup.CopyTo(model.Data, 0);

            return ret;
        }
        private bool GeneratePreviewSaveState()
        {
            try
            {
                FileInfo fInfo = new FileInfo(model.GetEditorPathWithoutFileName() + "RomPreviewBaseSave.zst");
                if (fInfo.Exists)
                {
                    fInfo = new FileInfo(GetStatePath());

                    if (!fInfo.Exists)
                    {
                        File.Copy(model.GetEditorPathWithoutFileName() + "RomPreviewBaseSave.zst", GetStatePath());
                    }
                }
            }
            catch
            {
                return false;
            }

            return true;
        }
        private bool PrepareImage()
        {
            Entrance ent = new Entrance();
            int index = this.eventListBox.SelectedIndex;

            if ((this.behaviour == (int)Behaviours.EventPreviewer || this.behaviour == (int)Behaviours.ActionPreviewer || this.behaviour == (int)Behaviours.BattlePreviewer) && index < 0 || index >= this.eventTriggers.Count)
            {
                this.eventchoice = false;
                return false;
            }

            LevelExits storage = new LevelExits(model.Data);
            storage.AddNewExitShort(0);
            storage.CurrentExitShort = 0;
            storage.CoordXShort = 30;
            storage.CoordYShort = 6;

            if (this.eventTriggers.Count > 0)
            {
                ent = (Entrance)eventTriggers[index];
                storage.DestinationShort = (ushort)ent.LevelNum;
                storage.DestinationFacingShort = ent.RadialPosition;
                storage.ShowMessageShort = ent.ShowMessage;
            }
            else
            {
                storage.DestinationShort = (ushort)this.selectNumericUpDown.Value;
                storage.DestinationFacingShort = 7;
                storage.ShowMessageShort = false;
            }
            storage.DestinationXCoordShort = (byte)this.adjustXNumericUpDown.Value;
            storage.DestinationYCoordShort = (byte)this.adjustYNumericUpDown.Value;

            if (this.settings.PreviewEnterEvent == false || this.behaviour == (int)Behaviours.BattlePreviewer)
            {
                int save = this.levelModel.Levels[storage.DestinationShort].LevelEvents.EntranceEvent;
                this.levelModel.Levels[storage.DestinationShort].LevelEvents.EntranceEvent = 0x5EB3;

                //SetEvent(new byte[] { 0x70, 0xFE }, 0); // Fade in from black

                SaveLevelExitEvents();
                this.levelModel.Levels[storage.DestinationShort].LevelEvents.EntranceEvent = save;
            }

            storage.AssembleExitsShort(0x0402);
            this.eventchoice = true;
            return true;
        }
        private void SaveLevelExitEvents()
        {
            ushort offsetStart = 0x0342;
            for (int i = 0; i < 0x19F; i++)
                offsetStart = levelModel.Levels[i].LevelEvents.Assemble(offsetStart);
        }

        // Get paths
        private string GetRomPath()
        {
            if (settings.PreviewDynamicRomName)
                return model.GetPathWithoutFileName() + "PreviewROM_" + model.GetFileNameWithoutPath();
            else
                return model.GetEditorPathWithoutFileName() + "PreviewRom.smc";
        }
        private string GetStatePath()
        {
            if (settings.PreviewDynamicRomName)
                return model.GetPathWithoutFileName() + "PreviewROM_" + model.GetFileNameWithoutPathOrExtension() + ".zst";
            else
                return model.GetEditorPathWithoutFileName() + "PreviewRom.zst";
        }
        private bool GetEmulator()
        {
            this.emulatorPath = settings.ZSNESPath; // Gets the saved emulator path
            FileInfo fi;
            try
            {
                fi = new FileInfo(this.emulatorPath);

                if (fi.Exists) // Checks if its a valid path
                    return true;
                else
                    throw new Exception();
            }
            catch
            {
                this.emulatorPath = SelectFile("exe files (*.exe)|*.exe|All files (*.*)|*.*", "C:\\", "Select Emulator ZSNESW.exe");

                if (this.emulatorPath == null || !this.emulatorPath.EndsWith(".exe"))
                    return false;

                fi = new FileInfo(this.emulatorPath);

                if (fi.Exists)
                {
                    settings.ZSNESPath = this.emulatorPath;
                    settings.Save();
                    return true;
                }
                else
                    return false;
            }
        }

        private void UpdateGui()
        {
            this.emuPathTextBox.Text = this.emulatorPath;
            this.romPathTextBox.Text = this.romPath;
            this.selectNumericUpDown.Value = this.selectNum;
            this.eventListBox.Items.Clear();
            Entrance ent;

            for (int i = 0; i < eventTriggers.Count; i++)
            {
                ent = (Entrance)eventTriggers[i];
                this.eventListBox.Items.Add("Enter - X:" + ent.CoordX.ToString("00") + " Y:" + ent.CoordY.ToString("000") + " - From Area: [" + ent.Source.ToString("X3") + "]  " + settings.LevelNames[ent.Source]);
            }
            if (this.eventListBox.Items.Count > 0)
                this.eventListBox.SelectedIndex = 0;
        }

        #endregion

        #region Event Handlers

        private void toggleAssembleLevels_CheckedChanged(object sender, EventArgs e)
        {
            this.toggleAssembleLevels.ForeColor = toggleAssembleLevels.Checked ? SystemColors.ControlText : SystemColors.ControlDark;
            if (!this.initializing)
            {
                this.settings.PreviewAssembleLevels = !this.settings.PreviewAssembleLevels;
                this.settings.Save();
            }
        }
        private void enterEventCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            this.enterEventCheckBox.ForeColor = enterEventCheckBox.Checked ? SystemColors.ControlText : SystemColors.ControlDark;
            if (!this.initializing)
            {
                this.settings.PreviewEnterEvent = enterEventCheckBox.Checked;
                settings.Save();
            }
        }
        private void launchButton_Click(object sender, EventArgs e)
        {
            if (Prelaunch())
                Launch();
        }
        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void changeEmuButton_Click(object sender, EventArgs e)
        {
            string path = SelectFile("exe files (*.exe)|*.exe|All files (*.*)|*.*", "C:\\", "Select Emulator ZSNESW.exe");

            if (path == null || !path.EndsWith(".exe"))
                return;

            FileInfo fi = new FileInfo(path);

            if (fi.Exists)
            {
                this.emulatorPath = path;
                this.emulator = true;
                settings.ZSNESPath = this.emulatorPath;
                settings.Save();
                UpdateGui();
            }
        }
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            this.checkBox1.ForeColor = checkBox1.Checked ? SystemColors.ControlText : SystemColors.ControlDark;
            if (!this.initializing)
            {
                settings.PreviewDynamicRomName = checkBox1.Checked;
                settings.Save();
                this.romPath = GetRomPath();
            }
            UpdateGui();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            this.argsTextBox.Text = settings.PreviewArgsDefault;
        }
        private void eventListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index = this.eventListBox.SelectedIndex;

            if (index < 0 || index >= this.eventTriggers.Count)
                return;

            // Set the XY values
            this.adjustXNumericUpDown.Value = ((Entrance)eventTriggers[index]).CoordX;
            this.adjustYNumericUpDown.Value = ((Entrance)eventTriggers[index]).CoordY;
        }
        private void selectNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            this.selectNum = (int)selectNumericUpDown.Value;

            this.eventTriggers.Clear();
            ScanForEntrancesToLevel((int)selectNumericUpDown.Value);

            UpdateGui();
        }
        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://zsnes-docs.sourceforge.net/html/advanced.htm#command_line");
        }

        #endregion

        public struct Entrance
        {
            public ushort Source;
            public bool ShowMessage;
            public byte CoordX;
            public byte CoordY;
            public byte RadialPosition;
            public ushort LevelNum;
            public bool Flag;
            public string msg;
        }
    }
}
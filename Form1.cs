using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Microsoft.Win32;
using FF3LE.Properties;

namespace FF3LE
{
    public partial class Form1 : Form, IMRUClient
    {
        private ProgramController AppControl;
        private Settings settings;
        //private bool cancelAnotherLoad;

        private MRUManager mruManager;      // MRU list manager
        private string initialDirectory;    // Initial directory for Save/Load operations
        int m_expBank;
        const string registryPath = "SOFTWARE\\FF3LE\\FF6LE";  // Registry path to keep persistent data

        // madsiur [CE Edition 1.0]
        private ExpansionWindow expWindow;

        public Form1(ProgramController controls)
        {
            this.AppControl = controls;
            settings = Settings.Default;
            InitializeComponent();

            // MRU
            LoadSettingsFromRegistry();
            mruManager = new MRUManager();
            mruManager.Initialize(this, recentFilesToolStripMenuItem, registryPath);

            LoadBankSettingsFromRegistry();
        }
        public void OpenMRUFile(string fileName)
        {
            try
            {
                open(fileName);
            }
            catch (Exception e)
            {
                MessageBox.Show("ERROR: " + e.Message);
            }
        }
        private void LoadSettingsFromRegistry()
        {
            try
            {
                RegistryKey key = Registry.CurrentUser.CreateSubKey(registryPath);

                initialDirectory = (string)key.GetValue(
                    "InitDir",                          // value name
                    Directory.GetCurrentDirectory());   // default value
            }
            catch
            {
                Trace.WriteLine("LoadSettingsFromRegistry failed");
            }
        }

        public static void GuiMain(ProgramController AppControl)
        {
            // Start the application.
            //Application.VisualStyleState = System.Windows.Forms.VisualStyles.VisualStyleState.NoneEnabled;
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1(AppControl));
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            open(null);
        }
        private void button5_Click(object sender, EventArgs e)
        {
            open(null);
        }

        private void open(string filename)
        {
            if ((filename == null) ? AppControl.OpenRomFile() : AppControl.OpenRomFile(filename) && AppControl.VerifyRom()) // Load the rom and verify it is a SMRPG rom of the correct version
            {
                if (!AppControl.HeaderPresent()) // If the rom does not have a header, we enable all the buttons
                {
                    this.button3.Enabled = true;
                    AppControl.CreateNewMd5Checksum(); // Create a new checksum for a new rom
                }
                else if (AppControl.HeaderPresent()) // If the rom does have a header, we disable all the buttons and enable the Remove Header buttons
                {
                    this.button3.Enabled = true;
                    AppControl.RemoveHeader();  // remove the header for editing purposes
                }

                if (AppControl.GameCode() != "F6  ")
                    MessageBox.Show("The game code for this ROM is invalid. There are likely to be problems when editing the ROM.", "WARNING: Invalid Game Code", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                //this.saveToolStripMenuItem.Enabled = true;
                //this.saveAsToolStripMenuItem.Enabled = true;
                //this.publishRomToolStripMenuItem.Enabled = true;
                //this.viewRomSignatureToolStripMenuItem.Enabled = true;

                UpdateRomInfo();

                // madsiur [CE Edition 1.0] 
                Log.InitLog();
                AppControl.Model().CheckExpansion();

                //LoadInitialSettings();
                AppControl.ClearLevelData();

                // madsiur - Enable expansion menu only if ROM loaded [CE Edition 1.0]
                expansionToolStripMenuItem.Enabled = true;
            }
            else
            {
                if (AppControl.VerifyRom())
                    return;
                this.button3.Enabled = false;

                // madsiur - Disable expansion menu if bad ROM [CE Edition 1.0]
                expansionToolStripMenuItem.Enabled = true;
            }

            mruManager.Add(AppControl.GetPathWithoutFileName() + AppControl.GetFileNameWithoutPath());
        
        }

        private void UpdateRomInfo()
        {
            this.richTextBox1.Text = AppControl.GetFileNameWithoutPath();
            this.richTextBox4.Text = "ROM Path.........." + AppControl.GetPathWithoutFileName() +
                "\nROM Name.........." + AppControl.GetRomName() +
                "\nHeader............" + AppControl.HeaderPresent() +
                "\nChecksum.........." + AppControl.RomChecksum() +
                "\nGamecode.........." + AppControl.GameCode() +
                "\nOv. Map Exp. Bank." + "0x" + m_expBank.ToString("X");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            AppControl.Levels(m_expBank);
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AppControl.SaveRomFile();
        }
        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AppControl.Assemble();
            if (AppControl.SaveRomFileAs())
                UpdateRomInfo();
            else
                MessageBox.Show("There was an error saving. Try again.", "SAVE ERROR", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void LoadBankSettingsFromRegistry()
        {
            try
            {
                RegistryKey key = Registry.CurrentUser.CreateSubKey(registryPath);

                m_expBank = (int)key.GetValue(
                    "ExpBank",              // value name
                    241);                   // default value, bank $F1
            }
            catch
            {
                Trace.WriteLine("LoadBankSettingsFromRegistry failed");
            }
        }

        private void SaveBankSettingsFromRegistry()
        {
            try
            {
                RegistryKey key = Registry.CurrentUser.CreateSubKey(registryPath);
                key.SetValue(
                    "ExpBank",              // value name
                    m_expBank);              // default value
            }
            catch
            {
                Trace.WriteLine("SaveBankSettingsFromRegistry failed");
            }
        }

        // madsiur
        private void expansionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (AppControl.GetFileSize() >= 0x400000)
            {
                expWindow = new ExpansionWindow(AppControl);
                expWindow.ShowDialog(this);
            }
            else
            {
                MessageBox.Show("You need an expanded ROM of at least $400000 bytes.\n\n" + "Current size: $" + AppControl.GetFileSize().ToString("X6"));
            }
        }
    }
}
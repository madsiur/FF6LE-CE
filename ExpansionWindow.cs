using FF3LE.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Windows.Forms;
using System.Xml.Linq;

namespace FF3LE
{
    /// <summary>
    /// madsiur [CE Edition 1.0]
    /// Form that handle the Expansion
    /// </summary>
    public partial class ExpansionWindow : Form
    {
        private ProgramController pc;
        private int numBanks;
        private int dataBank;
        private int tilemapBank;
        private bool isExpanded;
        private bool isChestExpanded;
        private bool isZplus;
        private string filePath;

        public ExpansionWindow(ProgramController pc)
        {
            InitializeComponent();
            tbLocationFile.Height = 17;
            this.pc = pc;
            initValues();
        }

        private void initValues()
        {
            isExpanded = false;
            isZplus = false;
            isChestExpanded = false;
            numBanks = 5;
            dataBank = 0xF2;
            tilemapBank = 0xF3;
            string filePath = Settings.Default.SettingsFile;
            Model.SettingsFile = null;

            if (filePath.Length != 0 && !filePath.Equals(""))
            {
                if (File.Exists(filePath))
                {
                    try
                    {
                        Model.SettingsFile = XDocument.Load(filePath);
                        XElement root = Model.SettingsFile.Element("Settings");
                        isExpanded = bool.Parse(root.Element("MapExpansion").Value);
                        isChestExpanded = bool.Parse(root.Element("ChestExpansion").Value);
                        isZplus = bool.Parse(root.Element("FF6LEPlus").Value);
                        dataBank = int.Parse(root.Element("DataBank").Value, NumberStyles.HexNumber);
                        tilemapBank = int.Parse(root.Element("TilemapBank").Value, NumberStyles.HexNumber);
                        numBanks = int.Parse(root.Element("NumBanksTilemap").Value);

                        if (isExpanded)
                        {
                            btnExpand.Enabled = false;
                            ckZdPlus.Enabled = false;
                            tbExpansionData.Enabled = false;
                            tbExpansionTilemaps.Enabled = false;
                            btnExpandChests.Enabled = true;
                        }

                        if (isChestExpanded)
                        {
                            btnExpandChests.Enabled = false;
                        }
                    }
                    catch (Exception e)
                    {

                        MessageBox.Show("Error readiong XML settings file.\n\n Error: " + e.Message, Model.APPNAME,
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    
                }
            }
            else
            {
                filePath = Directory.GetCurrentDirectory() + "settings.xml";
            }

            if (numBanks <= 4 || numBanks >= 8)
            {
                Model.SettingsFile.Element("Settings").Element("NumBanksTilemap").Value = "5";
                numBanks = 5;
            }

            nudBanks.Value = numBanks;
            tbExpansionData.Text = dataBank.ToString("X2");
            tbExpansionTilemaps.Text = tilemapBank.ToString("X2");
            ckZdPlus.Checked = isZplus;
            tbLocationFile.Text = filePath;
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            bool valid = true;
            string f = tbLocationFile.Text;
            if (!f.Equals("") && f.Length != 0)
            {
                DirectoryInfo folders = new DirectoryInfo(Path.GetDirectoryName(f));

                if (!folders.Exists)
                {
                    valid = false;
                    MessageBox.Show("Folder path does not exists!", Model.APPNAME, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else if (!Bits.IsValidFilePath(f))
                {
                    valid = false;
                    MessageBox.Show("Invalid file path!", Model.APPNAME, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                if (valid)
                {
                    filePath = tbLocationFile.Text;

                    if (filePath.Length != 0 && !filePath.Equals(""))
                    {
                        if (File.Exists(filePath))
                        {
                            numBanks = (int) nudBanks.Value;

                            try
                            {
                                if (filePath.Equals(Settings.Default.SettingsFile))
                                {
                                    Model.SettingsFile.Element("Settings").Element("NumBanksTilemap").Value =
                                        numBanks.ToString();
                                    Model.SettingsFile.Save(filePath);
                                }

                                Settings.Default.SettingsFile = filePath;
                                Settings.Default.Save();
                                initValues();
                            }
                            catch (Exception g)
                            {
                                MessageBox.Show(
                                    "Unable to save XML settings file. You may not have write rights.\n\n  Error: " +
                                    g.Message, Model.APPNAME, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                        else
                        {
                            Settings.Default.SettingsFile = filePath;
                            Settings.Default.Save();

                            isExpanded = false;
                            isChestExpanded = false;
                            isZplus = false;
                            dataBank = 0xF2;
                            tilemapBank = 0xF3;
                            numBanks = 5;

                            btnExpand.Enabled = true;
                            ckZdPlus.Enabled = true;
                            tbExpansionData.Enabled = true;
                            tbExpansionTilemaps.Enabled = true;
                            btnExpandChests.Enabled = false;

                            nudBanks.Value = numBanks;
                            tbExpansionData.Text = dataBank.ToString("X2");
                            tbExpansionTilemaps.Text = tilemapBank.ToString("X2");
                            ckZdPlus.Checked = isZplus;
                        }
                    }
                }
            }
            else
            {
                Settings.Default.SettingsFile = f;
                Settings.Default.Save();
            }
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnLocNamesPath_Click(object sender, EventArgs e)
        {
            ExpFolderBrowserDialog.RootFolder = Environment.SpecialFolder.MyComputer;
            ExpFolderBrowserDialog.ShowDialog();

            if (!string.IsNullOrWhiteSpace(ExpFolderBrowserDialog.SelectedPath))
            {
                string f = tbLocationFile.Text;
                
                if (Bits.IsValidFilePath(f))
                {
                    tbLocationFile.Text = Path.Combine(ExpFolderBrowserDialog.SelectedPath, Path.GetFileName(f));
                }
                else
                {
                    tbLocationFile.Text = Path.Combine(ExpFolderBrowserDialog.SelectedPath, "settings.xml");
                }
            }
        }

        private void btnExpand_Click(object sender, EventArgs e)
        {
            string message = String.Empty;
            bool error = false;
            byte dataBank = 0;
            byte tilemapBank = 0;

            string strTilemapBank = tbExpansionTilemaps.Text;
            string strDataBank = tbExpansionData.Text;
            string strFileName = tbLocationFile.Text;
            isZplus = ckZdPlus.Checked;
            numBanks = (int) nudBanks.Value;
            
            byte size = (byte)((pc.GetFileSize() - 1) >> 16);
            string strSize = pc.GetFileSize().ToString("X6");

            if (isZplus)
            {
                if (numBanks < 6)
                {
                    message = "Zone Doctor+ and FF6LE+ were expanding Tilemap. You must select at least 6 banks of data for tilemaps to make sure all your previous tilemap data is transfered.";
                    error = true;
                }
            }
            if ((strDataBank.Equals(String.Empty) || strDataBank.Length == 0) && !error)
            {
                message = "Please enter a valid bank for data expansion.";
                error = true;
            }

            if ((strTilemapBank.Equals(String.Empty) || strTilemapBank.Length == 0) && !error)
            {
                message = "Please enter a valid starting bank for tilemaps expansion.";
                error = true;
            }

            if (!error)
            {
                if (!Byte.TryParse(strDataBank, NumberStyles.HexNumber, new NumberFormatInfo(), out dataBank))
                {
                    message = "Bank number must be between $00 to $6F or between $C0 to $FF.";
                    error = true;
                }

                if (!Byte.TryParse(strTilemapBank, NumberStyles.HexNumber, new NumberFormatInfo(), out tilemapBank) && !error)
                {
                    message = "Bank number must be between $00 to $" + (0x70 - numBanks).ToString("X2") +
                              " or between $C0 to $" + (0x100 - numBanks).ToString("X2") + ".";
                    error = true;
                }
            }

            if (!error)
            {
                if (dataBank < 0x70 && dataBank > Bits.ToAbs(size))
                {
                    message = "Your data bank cannot be higher than your filesize.\n\n Current filesize: $" + strSize;
                    error = true;
                }
                else if (dataBank >= 0x70 && dataBank < 0xC0)
                {
                    message = "Invalid data bank! Bank must be in the $00-$6F or $C0-$FF range.";
                    error = true;
                }
                else if (dataBank == 0xDA || dataBank == 0x1A)
                {
                    message = "Invalid data bank. Bank $" + dataBank + " is needed for some expanded data such as location names and location data.";
                    error = true;
                }
            }

            if (!error)
            {
                if (tilemapBank > 0xC0 && pc.GetFileSize() == 0x400000 && tilemapBank > Bits.ToHiROM(size) - numBanks - 1)
                {
                    message = "Your HiROM Tilemaps starting bank added to the number of banks cannot be higher than filesize.\n\n Current filesize: $" + strSize + ", Number of banks: " +numBanks +  ".";
                    error = true;
                }
                else if (tilemapBank < 0x70 && tilemapBank > Bits.ToAbs(size) - numBanks - 1)
                {
                    message = "Your Tilemaps starting bank added to the number of banks cannot be higher than your filesize.\n\n Current filesize: $" + strSize + ", Number of banks: " + numBanks + ".";
                    error = true;
                }
                else if (tilemapBank >= 0x70 && tilemapBank < 0xC0)
                {
                    message = "Invalid Tilemaps starting bank! Bank must be in the $00-$" + (0x70 - numBanks).ToString("X2") + " or $C0-$" + (0x100 - numBanks).ToString("X2") + " range.";
                    error = true;
                }
                else if (tilemapBank == 0xDA || (tilemapBank < 0xDA && tilemapBank + numBanks > 0xDA) || tilemapBank == 0x1A || (tilemapBank < 0x1A && tilemapBank + numBanks > 0x1A))
                {
                    message = "Invalid Tilemaps starting bank. Bank $"+ tilemapBank + " is in the selected range and this bank is needed for some expanded data such as location names and location data.";
                    error = true;
                }
            }

            if (!error)
            {
                DirectoryInfo folders = new DirectoryInfo(Path.GetDirectoryName(strFileName));

                if (folders.Exists)
                {
                    if (Bits.IsValidFilePath(strFileName))
                    {
                        Settings.Default.SettingsFile = strFileName;
                        Settings.Default.Save();
                    }
                    else
                    {
                        message = "Invalid setting file path!";
                        error = true;
                    }
                }
                else
                {
                    message = "Folder path does not exists!";
                    error = true;
                }
            }

            if (!error)
            {
                int dataOffset = Bits.ToHiROM(dataBank << 16);
                int tilemapOffset = Bits.ToHiROM(tilemapBank << 16);
                int tilemapSize = numBanks << 16;

                DialogResult dialog =
                    MessageBox.Show("You want to expand data at $" + dataOffset.ToString("X6") + " to $" +
                                    (dataOffset + 0xFFFF).ToString("X6") +
                                    " and expand tilemaps data at $" + tilemapOffset.ToString("X6") + " to $" +
                                    (tilemapOffset + tilemapSize - 1).ToString("X6") + "?", Model.APPNAME,
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question);

                if (dialog == DialogResult.Yes)
                {
                    List<int[]> faults = new List<int[]>();
                    DialogResult d = DialogResult.OK;

                    if (isZplus)
                    {
                       d = MessageBox.Show("Coming from Zone Doctor+ of FF6LE+ skips most of the ROM ASM validation.",
                            Model.APPNAME, MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                    }

                    if (d == DialogResult.OK)
                    {
                        faults = pc.ValidateRom(isZplus);

                        if (faults.Count == 0)
                        {
                            pc.InitFields();

                            if (pc.ExpandRom(dataOffset, tilemapOffset, tilemapSize, isZplus))
                            {
                                try
                                {
                                    Model.LevelNames = Model.ConvertLocNames(Settings.Default.ExpandedLevelNames);
                                    Model.BuildSettingXml(Bits.ToHiROM(tilemapBank), Bits.ToHiROM(dataBank), numBanks, true, false, isZplus, Model.LevelNames);
                                    Model.SettingsFile.Save(Settings.Default.SettingsFile);

                                    MessageBox.Show("Expansion completed!", Model.APPNAME, MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);

                                    tbExpansionData.Enabled = false;
                                    tbExpansionTilemaps.Enabled = false;
                                    ckZdPlus.Enabled = false;
                                    btnExpand.Enabled = false;
                                    btnExpandChests.Enabled = true;
                                }
                                catch (Exception f)
                                {
                                    MessageBox.Show(
                                        "Error creating XML file.\n\n Error: " + f.Message,
                                        Model.APPNAME, MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                                
                            }
                            else
                            {
                                MessageBox.Show(
                                    "Expansion has failed for an unknow reason. Check the file log.txt in the executable folder and report this!",
                                    Model.APPNAME, MessageBoxButtons.OK,
                                    MessageBoxIcon.Error);
                            }
                        }
                        else
                        {
                            message = "You have the following error(s) in your ROM:\n";

                            for (int i = 0; i < faults.Count && i < 8; i++)
                            {
                                message += "\n" + (i + 1) + "- Offset $" + faults[i][0].ToString("X6") + ", value of $" +
                                           faults[i][2].ToString("X6") + " found. Expected: $" +
                                           faults[i][1].ToString("X6");
                            }

                            if (faults.Count > 8)
                            {
                                message += "\n\nDisplaying only the first 8 errors. You have more.\n";
                            }

                            dialog =
                                MessageBox.Show(
                                    message + "\nThere WILL be problems with the ROM after expansion. Continue anyway?",
                                    Model.APPNAME, MessageBoxButtons.YesNo,
                                    MessageBoxIcon.Exclamation);

                            if (dialog == DialogResult.Yes)
                            {
                                pc.InitFields();

                                if (pc.ExpandRom(dataOffset, tilemapOffset, tilemapSize, isZplus))
                                {
                                    try
                                    {
                                        Model.LevelNames = Model.ConvertLocNames(Settings.Default.ExpandedLevelNames);
                                        Model.BuildSettingXml(Bits.ToHiROM(tilemapBank), Bits.ToHiROM(dataBank), numBanks, true, false, isZplus, Model.LevelNames);
                                        Model.SettingsFile.Save(Settings.Default.SettingsFile);

                                        MessageBox.Show("Expansion completed!", Model.APPNAME, MessageBoxButtons.OK,
                                        MessageBoxIcon.Information);

                                        tbExpansionData.Enabled = false;
                                        tbExpansionTilemaps.Enabled = false;
                                        ckZdPlus.Enabled = false;
                                        btnExpand.Enabled = false;
                                        btnExpandChests.Enabled = true;
                                    }
                                    catch (Exception f)
                                    {
                                        MessageBox.Show(
                                            "Error creating XML file. You must select a folder with write rights and redo expansion.",
                                            Model.APPNAME, MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    }
                                }
                                else
                                {
                                    MessageBox.Show(
                                        "Expansion has failed for an unknow reason. Check the file log.txt in the executable folder and report this!",
                                        Model.APPNAME, MessageBoxButtons.OK,
                                        MessageBoxIcon.Error);
                                }

                                this.Close();
                            }
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Error! " + message, Model.APPNAME, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnExpandChests_Click(object sender, EventArgs e)
        {
            List<int[]> faults = pc.ValidateChestExpansion();
            string message = "";

            DialogResult dialog =
                    MessageBox.Show("This will add $20 bytes available for chest memory. The range will now be $1E20 to $1E7F for a total of 768 bits. Proceed?", Model.APPNAME,
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question);

            if (dialog == DialogResult.Yes)
            {
                if (faults.Count == 0)
                {
                    if (pc.ExpandChests())
                    {
                        try
                        {
                            if (Model.SettingsFile != null)
                            {
                                Model.SettingsFile.Element("Settings").Element("ChestExpansion").Value = true.ToString();
                                Model.SettingsFile.Save(Settings.Default.SettingsFile);
                                btnExpandChests.Enabled = false;
                            }

                            MessageBox.Show("Chest memory expansion completed!", Model.APPNAME, MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
                        }
                        catch (Exception g)
                        {
                            MessageBox.Show(
                            "Unable to save XML settings file. You may not have write rights or file may not exist.\n\n  Error: " +
                            g.Message, Model.APPNAME, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                else
                {
                    message = "You have the following error(s) in your ROM:\n";

                    for (int i = 0; i < faults.Count && i < 8; i++)
                    {
                        message += "\n" + (i + 1) + "- Offset $" + faults[i][0].ToString("X6") + ", value of $" +
                                   faults[i][2].ToString("X4") + " found. Expected: $" +
                                   faults[i][1].ToString("X4");
                    }

                    dialog = MessageBox.Show(message +
                            "\nThere might be problems with the ROM after chest expansion. Continue anyway?",
                            Model.APPNAME, MessageBoxButtons.YesNo,
                            MessageBoxIcon.Exclamation);

                    if (dialog == DialogResult.Yes)
                    {
                        if (pc.ExpandChests())
                        {
                            try
                            {
                                if (Model.SettingsFile != null)
                                {
                                    Model.SettingsFile.Element("Settings").Element("ChestExpansion").Value = true.ToString();
                                    Model.SettingsFile.Save(Settings.Default.SettingsFile);
                                    btnExpandChests.Enabled = false;
                                }

                                MessageBox.Show("Chest memory expansion completed!", Model.APPNAME, MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);
                            }
                            catch (Exception g)
                            {
                                MessageBox.Show(
                                "Unable to save XML settings file. You may not have write rights or file may not exist.\n\n  Error: " +
                                g.Message, Model.APPNAME, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }

                        }
                    }
                }
            }
        }


    }
}

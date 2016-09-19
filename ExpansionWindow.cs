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

namespace FF3LE
{
    /// <summary>
    /// madsiur [CE Edition 1.0]
    /// Form that handle the Expansion
    /// </summary>
    public partial class ExpansionWindow : Form
    {
        private ProgramController pc;
        private int memLoc;
        private byte memByte;
        private int numBanks;
        private int dataBank;
        private int tilemapBank;
        private bool isExpanded;
        private bool isZplus;
        private string filepath;
        private byte a;

        public ExpansionWindow(ProgramController pc)
        {
            InitializeComponent();
            tbLocationFile.Height = 17;
            this.pc = pc;
            initValues();
        }

        private void initValues()
        {
            if (Properties.Settings.Default.MemoryByte < pc.Data().Length)
            {
                memLoc = Bits.ToAbs(Settings.Default.MemoryByte);
            }
            else
            {
                memLoc = 0x2DC47F;
            }

            memByte = pc.Data()[memLoc];
            isExpanded = false;
            isZplus = false;
            numBanks = 5;
            dataBank = 0xF2;
            tilemapBank = 0xF3;
            string file = Settings.Default.LevelNamesPath;
            filepath = file.Equals("") || file.Length == 0 || file.Equals("") || !Uri.IsWellFormedUriString(file, UriKind.Absolute) ? Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "mapnames.bin") : Properties.Settings.Default.LevelNamesPath;

            if ((memByte & 0xFF) != 0xFF)
            {
                if ((memByte & 0x80) == 0x80)
                {
                    isExpanded = true;

                    if ((memByte & 0x40) == 0x40)
                    {
                        isZplus = true;
                    }

                    numBanks = ((memByte & 0x07) + 4);

                    dataBank = Bits.ToHiROM(ByteManage.GetInt(pc.Data(), 0x0052C3)) >> 16;
                    tilemapBank = Bits.ToHiROM(pc.Data()[0x0028A4]);

                    tbExpansionData.Enabled = false;
                    tbExpansionTilemaps.Enabled = false;
                    ckZdPlus.Enabled = false;
                    btnExpand.Enabled = false;
                }
            }
            else
            {
                tbExpansionData.Enabled = true;
                tbExpansionTilemaps.Enabled = true;
                ckZdPlus.Enabled = true;
                btnExpand.Enabled = true;
            }

            tbExpansionData.Text = dataBank.ToString("X2");
            tbExpansionTilemaps.Text = tilemapBank.ToString("X2");
            tbExpansionMemory.Text = Bits.ToHiROM(memLoc).ToString("X6");
            ckZdPlus.Checked = isZplus;
            nudBanks.Value = numBanks > 4 && numBanks < 8 ? numBanks: 5;
            tbLocationFile.Text = filepath;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            bool valid = true;
            int memoLoc = 0;
            if(int.TryParse(tbExpansionMemory.Text, NumberStyles.HexNumber, new NumberFormatInfo(), out memoLoc))
            {
                

                if ((memoLoc > 0xC00000 && memoLoc < 0xFFFFFF) || (memoLoc > 0x000000 && memoLoc < 0x6FFFFF))
                {
                    memoLoc = Bits.ToAbs(memoLoc);
                }
                else
                {
                    MessageBox.Show("Invalid memory byte location! Value must between $C00000 to $FFFFFF or $400000 to $6FFFFF.\n\n Current value: $" + tbExpansionMemory.Text);
                    valid = false;
                }
            }
            else
            {
                MessageBox.Show("Invalid memory byte location! Value must between $C00000 to $FFFFFF or $400000 to $6FFFFF.\n\n Current value: $" + tbExpansionMemory.Text);
                valid = false;
            }

            string f = tbLocationFile.Text;
            DirectoryInfo folders = new DirectoryInfo(Path.GetDirectoryName(f));

            if (!folders.Exists)
            {
                valid = false;
                MessageBox.Show("Folder path does not exists!", "FF6LE", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (!Bits.IsValidFilePath(f))
            {
                valid = false;
                MessageBox.Show("Invalid file path!", "FF6LE", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }


            if (valid)
            {
                if ((memByte & 0xFF) != 0xFF)
                {

                    if ((memByte & 0x80) == 0x80)
                    {
                        isExpanded = true;

                        if(memoLoc != 0)
                        {
                            Settings.Default.MemoryByte = memoLoc;
                        }

                        Settings.Default.LevelNamesPath = f;
                        Settings.Default.Save();

                        int s = (memByte & 0x07) + 4;

                        if(s > (int)nudBanks.Value)
                        {
                            MessageBox.Show("Number of tilemap banks cannot be smaller than in your expanded ROM!", "FF6LE", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        else
                        {
                            a = (byte)(memByte >> 4);
                            a = (byte)(a << 4);
                            a += (byte)((int)nudBanks.Value - 4);
                            pc.setMemByte(memLoc, a);
                        }
                    }
                    else
                    {
                        MessageBox.Show("ROM has not done the Editor expansion. Changes will not be saved!", "FF6LE", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }

                this.Close();
            }
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
                    tbLocationFile.Text = Path.Combine(ExpFolderBrowserDialog.SelectedPath, "mapnames.bin");
                }
            }
        }

        private void btnExpand_Click(object sender, EventArgs e)
        {
            string message = String.Empty;
            bool error = false;
            byte dataBank = 0;
            byte tilemapBank = 0;
            int memoryByte = 0;

            string strTilemapBank = tbExpansionTilemaps.Text;
            string strDataBank = tbExpansionData.Text;
            string strMemoryLocation = tbExpansionMemory.Text;
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

            if ((strMemoryLocation.Equals(String.Empty) || strMemoryLocation.Length == 0) && !error)
            {
                message = "Please enter a valid offset for expansion memory byte.";
                error = true;
            }

            if (!error)
            {
                if (!int.TryParse(strMemoryLocation, NumberStyles.HexNumber, new NumberFormatInfo(), out memoryByte))
                {
                    message = "Memory offset is not a number.";
                    error = true;
                }
            }

            if (!error)
            {
                byte b = (byte) (memoryByte >> 16);

                
                if (b < 0x70 && b > Bits.ToAbs(size))
                {
                    message = "Your memory byte offset cannot be higher than your filesize.\n\n Current filesize: $" + strSize;
                    error = true;
                }
                else if (b >= 0x70 && b < 0xC0)
                {
                    message = "Invalid memory byte offset! Offset must be in the $000000-$6FFFFF or $C00000-$FFFFFF range.";
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
                        Settings.Default.LevelNamesPath = strFileName;
                        Settings.Default.Save();
                    }
                    else
                    {
                        message = "Invalid file path!";
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
                                    (tilemapOffset + tilemapSize - 1).ToString("X6") + "?", "ZONE DOCTOR",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question);

                if (dialog == DialogResult.Yes)
                {
                    List<int[]> faults = new List<int[]>();
                    DialogResult d = DialogResult.OK;

                    if (isZplus)
                    {
                       d = MessageBox.Show("Coming from Zone Doctor+ of FF6LE+ skips most of the ROM ASM validation.",
                            "FF6LE", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                    }

                    if (d == DialogResult.OK)
                    {
                        faults = pc.ValidateRom(isZplus);

                        if (faults.Count == 0)
                        {
                            pc.InitFields();

                            if (pc.ExpandRom(dataOffset, tilemapOffset, memoryByte, tilemapSize, isZplus))
                            {
                                MessageBox.Show("Expansion completed!", "FF6LE", MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);

                                tbExpansionData.Enabled = false;
                                tbExpansionTilemaps.Enabled = false;
                                tbExpansionMemory.Enabled = false;
                                ckZdPlus.Enabled = false;
                                btnExpand.Enabled = false;
                            }
                            else
                            {
                                MessageBox.Show(
                                    "Expansion has failed for an unknow reason. Check the file log.txt in the executable folder and report this!",
                                    "FF6LE", MessageBoxButtons.OK,
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
                                    "ZONE DOCTOR", MessageBoxButtons.YesNo,
                                    MessageBoxIcon.Exclamation);

                            if (dialog == DialogResult.Yes)
                            {
                                pc.InitFields();

                                if (pc.ExpandRom(dataOffset, tilemapOffset, memoryByte, tilemapSize, isZplus))
                                {
                                    MessageBox.Show("Expansion completed!", "FF6LE", MessageBoxButtons.OK,
                                        MessageBoxIcon.Information);
                                    
                                }
                                else
                                {
                                    MessageBox.Show(
                                        "Expansion has failed for an unknow reason. Check the file log.txt in the executable folder and report this!",
                                        "FF6LE", MessageBoxButtons.OK,
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
                MessageBox.Show("Error! " + message, "ZONE DOCTOR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}

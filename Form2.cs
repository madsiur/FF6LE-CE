using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Windows.Forms;
using FF3LE.Properties;

namespace FF3LE
{
    public partial class ImportWindow : Form
    {
        private string[] locNames;
        private int memory;
        private byte memoryByte;
        private int numBanks;
        private int dataBank;
        private int tilemapBank;
        private bool isExpanded;
        private bool isChestExpanded;
        private bool isZplus;
        private Model model;
        public ImportWindow(Model model)
        {
            InitializeComponent();
            this.model = model;
            locNames = null;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            isZplus = false;
            isExpanded = false;
            isChestExpanded = false;
            numBanks = 5;
            bool error = false;

            DirectoryInfo folders = new DirectoryInfo(Path.GetDirectoryName(tbSettingFile.Text));

            if (folders.Exists)
            {
                if (!Bits.IsValidFilePath(tbSettingFile.Text))
                {
                    error = true;
                }
            }
            else
            {
                error = true;
            }

            if (!error)
            {
                if (int.TryParse(tbMemory.Text, NumberStyles.HexNumber, new NumberFormatInfo(), out memory))
                {
                    if (Bits.ToAbs(memory) < model.Data.Length)
                    {
                        if (memory < 0x400000 || memory > 0xC00000)
                        {
                            memoryByte = model.Data[Bits.ToAbs(memory)];

                            if ((memoryByte & 0x80) == 0x80)
                            {
                                isExpanded = true;

                                if ((memoryByte & 0x40) == 0x40)
                                {
                                    isZplus = true;
                                }

                                if ((memoryByte & 0x20) == 0x20)
                                {
                                    isChestExpanded = true;
                                }

                                numBanks = (memoryByte & 0x07) + 4;
                                dataBank = ByteManage.GetInt(model.Data, 0x004BE1) >> 16;
                                tilemapBank = model.Data[0x0028A4];

                                if (locNames == null)
                                {
                                    locNames = Model.ConvertLocNames(Properties.Settings.Default.ExpandedLevelNames);
                                }

                                try
                                {
                                    Model.BuildSettingXml(tilemapBank, dataBank, numBanks, isExpanded, isChestExpanded,
                                        isZplus, locNames);
                                    Model.SettingsFile.Save(tbSettingFile.Text);
                                    Settings.Default.SettingsFile = tbSettingFile.Text;
                                    Settings.Default.Save();
                                    this.Close();
                                }
                                catch (Exception g)
                                {
                                    MessageBox.Show(
                                        "Unable to save XML settings file. You may not have write rights or file may not exist.\n\n  Error: " +
                                        g.Message, Model.APPNAME, MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }

                            }
                            else
                            {
                                MessageBox.Show("ROM is not expanded according to memory byte. Operation has stopped.",
                                    Model.APPNAME, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            }
                        }
                        else
                        {
                            MessageBox.Show("Invalid memory byte offset.", Model.APPNAME, MessageBoxButtons.OK,
                                MessageBoxIcon.Exclamation);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Invalid memory byte offset.", Model.APPNAME, MessageBoxButtons.OK,
                            MessageBoxIcon.Exclamation);
                    }
                }
                else
                {
                    MessageBox.Show("Invalid memory byte offset.", Model.APPNAME, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
            else
            {
                MessageBox.Show("Invalid settings file path.", Model.APPNAME, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void btnLocNames_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.InitialDirectory = Directory.GetCurrentDirectory();
            openFileDialog1.Filter = "All files (*.*)|*.*";

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                FileStream fs = new FileStream(openFileDialog1.FileName, FileMode.Open);

                try
                {
                    BinaryFormatter formatter = new BinaryFormatter();

                    // Deserialize the hashtable from the file and 
                    // assign the reference to the local variable.
                    locNames = (string[])formatter.Deserialize(fs);
                    tbLocNames.Text = openFileDialog1.FileName;
                }
                catch (SerializationException f)
                {
                    MessageBox.Show("Failed to deserialize.\n\n Error: " + f.Message, Model.APPNAME, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    fs.Close();
                }
            }
        }

        private void btnSettingFile_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.RootFolder = Environment.SpecialFolder.MyComputer;
            folderBrowserDialog1.ShowDialog();

            string file = Path.GetFileName(tbSettingFile.Text);
            
            if (!string.IsNullOrWhiteSpace(folderBrowserDialog1.SelectedPath))
            {
                if (file != "" && file.Length != 0)
                {
                    tbSettingFile.Text = Path.Combine(folderBrowserDialog1.SelectedPath, file);
                }
                else
                {
                    tbSettingFile.Text = Path.Combine(folderBrowserDialog1.SelectedPath, "settings.xml");
                }
                    
            }
        }
    }
}

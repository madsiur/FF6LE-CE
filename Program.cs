using System;
using System.Collections.Generic;
using System.Windows.Forms;
using FF3LE.Properties;
using System.IO;

namespace FF3LE
{
    public class Program
    {
        private Model model;
        private Settings settings;
        private Levels levels;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        public static void Main(string[] args)
        {
            Program App = new Program();
        }

        public Program()
        {

            model = new Model(this);
            settings = Settings.Default;

            ProgramController controls = new ProgramController(model, this);

            Form1.GuiMain(controls);

        }

        public bool OpenRomFile()
        {
            string filename;

            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.InitialDirectory = settings.LastRomPath;
            openFileDialog1.Title = "Select a FF3 ROM";
            openFileDialog1.Filter = "SMC files (*.SMC)|*.SMC|All files (*.*)|*.*";
            openFileDialog1.FilterIndex = 2;
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() != DialogResult.Cancel)
            {
                filename = openFileDialog1.FileName;
                model.SetFileName(filename);
                if (model.ReadRom())
                {
                    settings.SettingsFile = settings.SettingsFile.Equals(String.Empty) || settings.SettingsFile.Equals(" ") 
                        || !Bits.IsValidFilePath(settings.SettingsFile) || settings.SettingsFile.Length == 0
                        ? Path.Combine(Environment.CurrentDirectory, "mapnames.bin")
                        : settings.SettingsFile;

                    settings.LastRomPath = model.GetPathWithoutFileName();
                    settings.Save();
                    
                    return true;
                }
            }
            else
                filename = "";
            return false;

        }
        public bool OpenRomFile(string filename)
        {
            model.SetFileName(filename);
            if (model.ReadRom())
            {
                settings.SettingsFile = settings.SettingsFile.Equals(String.Empty) || settings.SettingsFile.Equals(" ")
                        || !Bits.IsValidFilePath(settings.SettingsFile) || settings.SettingsFile.Length == 0
                        ? Path.Combine(Environment.CurrentDirectory, "mapnames.bin")
                        : settings.SettingsFile;

                settings.LastRomPath = model.GetPathWithoutFileName();
                settings.Save();
                return true;
            }
            return false;
        }
        public void AssembleAndCloseWindows()
        {
            if (levels != null && levels.Visible)
                levels.Close();
        }
        public bool SaveRomFile()
        {
            // todo: this is called by the main panel
            Assemble();
            if (model.WriteRom())
            {
                Model.SaveXML();
                model.CreateNewMD5Checksum();
                return true;
            }
            return false;
        }
        public bool SaveRomFileAs()
        {

            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "SMC files (*.SMC)|*.SMC|All files (*.*)|*.*";
            saveFileDialog1.FilterIndex = 2;
            saveFileDialog1.RestoreDirectory = true;

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                model.SetFileName(saveFileDialog1.FileName);

                // Assemble all changes
                Assemble();

                if (model.WriteRom())
                {
                    Model.SaveXML();
                    settings.LastRomPath = model.GetPathWithoutFileName();
                    settings.Save();
                    model.CreateNewMD5Checksum();
                    return true;
                }
                else
                {
                    model.SetFileName("Invalid File Name");
                }
            }

            return false;
        }
        public void Assemble()
        {
            if (levels != null && model.AssembleLevels)
                levels.Assemble();
            // Rest of assemblers here

            //madsiur, checksum is now correctly calculated in WriteRom()
            //model.CalculateAndSetNewRomChecksum(); // TODO: DOES NOT WORK!
        }
        public void CreateLevelsWindow(int iExpBank)
        {
            if (levels == null || !levels.Visible)
            {
                
                model.AssembleLevels = true;
                levels = new Levels(model);
                levels.m_expBank = iExpBank;
                levels.Show();
            }
        }
        public void CreateNewLevelsCommandStack()
        {
        //    try
        //    {
        //        if (levels != null)
        //            levels.CreateNewCommandStack();
        //    }
        //    catch
        //    {

        //    }
        }
    }
}
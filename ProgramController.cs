using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;//remove later

namespace FF3LE
{
    public class ProgramController
    {
        private Program App;
        private Model model;
        
        public ProgramController(Model model, Program app)
        {
            this.model = model;
            this.App = app;
        }

        public void testControls(string test)
        {
            MessageBox.Show(test);
        }

        public void exit()
        {
            Application.Exit();
        }
        public bool OpenRomFile()
        {
            if (App.OpenRomFile())
            {
                return true;
            }
            else
                return false;
        }
        public bool OpenRomFile(string filename)
        {
            if (App.OpenRomFile(filename))
            {
                return true;
            }
            else
                return false;
        }
        public bool SaveRomFile()
        {
            return App.SaveRomFile();
        }
        public bool SaveRomFileAs()
        {
            return App.SaveRomFileAs();
        }
        public bool CloseRomFile()
        {
            return false;
        }
        public bool VerifyRom()
        {
            return model.VerifyRom();
        }
        public string GetFileName()
        {
            return model.GetFileName();
        }
        public bool HeaderPresent()
        {
            return model.HeaderPresent();
        }
        public string GetFileNameWithoutPath()
        {
            return model.GetFileNameWithoutPath();
        }
        public string GetFileNameWithoutPathOrExtension()
        {
            return model.GetFileNameWithoutPathOrExtension();
        }
        public string GetRomName()
        {
            return model.GetRomName();
        }
        public string GetPathWithoutFileName()
        {
            return model.GetPathWithoutFileName();
        }
        public string RomChecksum()
        {
            return model.RomChecksum();
        }
        public string GameCode()
        {
            return model.GameCode();
        }
        public bool RemoveHeader()
        {
            return model.RemoveHeader();
        }
        public long GetFileSize()
        {
            return model.GetFileSize();
        }
        public void Levels(int iExpBank)
        {
            App.CreateLevelsWindow(iExpBank);
        }
        public void CreateNewLevelsCommandStack()
        {
            App.CreateNewLevelsCommandStack();
        }
        public void ClearLevelData()
        {
            model.ClearLevelData();
        }
        public void CreateNewMd5Checksum()
        {
            model.CreateNewMD5Checksum();
        }
        public bool VerifyMD5Checksum()
        {
            return model.VerifyMD5Checksum();
        }
        public void Assemble()
        {
            App.Assemble();
        }
        public void AssembleAndCloseWindows()
        {
            App.AssembleAndCloseWindows();
        }
        public void AssembleFinal(bool val)
        {
            model.AssembleFinal = val;
            if(val)
                Assemble();
        }

        //madsiur
        public List<int[]> ValidateRom(bool isZplus)
        {
            return model.ValidateROM(isZplus);
        }

        public List<int[]> ValidateChestExpansion()
        {
            return model.ValidateChestExpansion();
        }

        //madsiur
        public bool ExpandRom(int dataOffset, int tilemapOffset, int tilemapSize, bool isZplus)
        {
            return model.ExpandROM(dataOffset, tilemapOffset, tilemapSize, isZplus);
        }

        public bool ExpandChests()
        {
            return model.ExpandChests();
        }

        //madsiur
        public byte[] Data()
        {
            return model.Data;
        }

        public Model Model()
        {
            return model;
        }

        public void setMemByte(int offset, byte b)
        {
            model.Data[offset] = b;
        }

        public void InitFields()
        {
            model.InitExpansionFields(false);
        }
    }
}

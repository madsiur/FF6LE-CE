using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using FF3LE.Properties;
using FF3LE.Undo;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.RegularExpressions;

namespace FF3LE
{
    public partial class Levels : Form
    {
        #region Variables

        // Local Variables
        private Model model;
        private LevelModel levelModel;
        private State state;
        private Settings settings;
        private CommandStack commandStack;

        private int currentLevel = 0;

        private Level[] levels; // Array of levels
        private PrioritySet[] prioritySets;
        private PaletteSet[] paletteSets;

        private int[] levelPixels;
        private Bitmap levelImage;

        private PaletteSet paletteSet;
        private TileMap tileMap;
        private WorldMapTilemap wmTileMap;
        private LevelLayer layer;

        private Overlay overlay;
        private ProgressBar pBar;

        private bool updatingLevel = false;

        private Point mouse = new Point();
        private Point tile = new Point();
        private double zoom = 1;

        private Size moveDiff;
        private Point coordDelta;
        private Bitmap moveImage;
        private bool insideSelection = false;
        private bool isOverSomething = false;
        private bool waitBothCoords = false;

        private bool mouseOver;
        private bool replaceChoose;
        private bool replaceSet;
        private int replaceTile;
        private int replaceWith;

        private string[] levelNames, itemNames, messageNames;
        private MatchCollection matches;

        //madsiur
        bool isLevelNameChanged;
        bool isUpdatingMessageName;

        private string[] dialogueTable = new string[]
            {
                "", "*", "<TERRA>", "<LOCKE>", "<CYAN>", "<SHADOW>", "<EDGAR>", "<SABIN>", 
                "<CELES>", "<STRAGO>", "<RELM>", "<SETZER>", "<MOG>", "<GAU>", "<GOGO>", "<UMARO>", 
                "", "", "", "*", "", "", "", "", 
                "", "", "", "", "", "", "", "", 
                "A", "B", "C", "D", "E", "F", "G", "H",
                "I", "J", "K", "L", "M", "N", "O", "P", 
                "Q", "R", "S", "T", "U", "V", "W", "X", 
                "Y", "Z", "a", "b", "c", "d", "e", "f", 
                "g", "h", "i", "j", "k", "l", "m", "n",
                "o", "p", "q", "r", "s", "t", "u", "v", 
                "w", "x", "y", "z", "0", "1", "2", "3",
                "4", "5", "6", "7", "8", "9", "!", "?", 
                "", ":", "\"", "'", "-", ".", ",", "...",
                ";", "#", "+", "(", ")", "%", "~", "", 
                "@", "<note>", "=", "\"", "74", "75", "<pearl>", "<death>",
                "<lit>", "<wind>", "<earth>", "<ice>", "<fire>", "<water>", "<poison>", " ", 
                "e ", " t", ": ", "th", "t ", "he", "s ", "er", 
                " a", "re", "in", "ou", "d ", " w", " s", "an", 
                "o ", " h", " o", "r ", "n ", "at", "to", " i",
                ", ", "ve", "ng", "ha", " m", "Th", "st", "on", 
                "yo", " b", "me", "y ", "en", "it", "ar", "ll",
                "ea", "I ", "ed", " f", " y", "hi", "is", "es", 
                "or", "l ", " c", "ne", "'s", "nd", "le", "se", 
                " I", "a ", "te", " l", "pe", "as", "ur", "u ", 
                "al", " p", "g ", "om", " d", "f ", " g", "ow",
                "rs", "be", "ro", "us", "ri", "wa", "we", "Wh", 
                "et", " r", "nt", "m ", "ma", "I'", "li", "ho",
                "of", "Yo", "h ", " n", "ee", "de", "so", "gh", 
                "ca", "ra", "n'", "ta", "ut", "el", "! ", "fo",
                "ti", "We", "lo", "e!", "ld", "no", "ac", "ce", 
                "k ", " u", "oo", "ke", "ay", "w ", "!!", "ag",
                "il", "ly", "co", ". ", "ch", "go", "ge", "e..."
            };
        private string[] nameTable = new string[]
            {
                "A","B","C","D","E","F","G","H","I","J","K","L","M","N","O","P",
                "Q","R","S","T","U","V","W","X","Y","Z","a","b","c","d","e","f",
                "g","h","i","j","k","l","m","n","o","p","q","r","s","t","u","v",
                "w","x","y","z","0","1","2","3","4","5","6","7","8","9","!","?",
                "ú",":","\"","'","-",".",".","ú",";","#","+","(",")","%","~","ú",
                "ú","ú","="
            };

        public int m_expBank = 0xF1;

        #endregion

        public Levels(Model model)
        {
            this.model = model;
            this.state = State.Instance;


            

            this.settings = Settings.Default;
            this.overlay = new Overlay();
            this.commandStack = new CommandStack(50);

            model.CreateLevelModel();

            levelModel = model.GetLevelModel();

            DecompressLevelData();

            levelModel.PaletteSets[50] = new PaletteSet(50, model.StPaletteSet);

            InitializeComponent();

            levelNames = GetLevelNames();
            this.levelName.Items.AddRange(levelNames);
            this.exitShortDestination.Items.AddRange(levelNames);
            this.exitLongDestination.Items.AddRange(levelNames);

            messageNames = GetMessageNames();
            this.messageName.Items.AddRange(messageNames);

            itemNames = GetItemNames();
            this.treasurePropertyName.Items.AddRange(itemNames);

            levels = levelModel.Levels;
            paletteSets = levelModel.PaletteSets;
            prioritySets = levelModel.PrioritySets;

            InitializeLevel(); // Sets initial control settings

            //madsiur
            isLevelNameChanged = false;

            if (Model.IsExpanded)
            {
                tbLocationName.Enabled = true;
            }
            
            mapTilemapL1Num.Maximum = Model.NUM_TILEMAPS - 1;
            mapTilemapL2Num.Maximum = Model.NUM_TILEMAPS - 1;
            mapTilemapL3Num.Maximum = Model.NUM_TILEMAPS - 1;
            levelNum.Maximum = Model.NUM_LOCATIONS - 1;
            message.Maximum = Model.NUM_LOC_NAMES - 1;
            
        }

        #region Methods

        private void InitializeLevel()
        {
            updatingLevel = true;

            levelNum.Value = state.LevelNum;

            ResetTileReplace();

            if (RefreshLevel.IsBusy) return;

            levelNum.Enabled = false;
            levelName.Enabled = false;
            panel27.Hide();
            panel61.Hide();

            levelName.SelectedIndex = currentLevel = (int)levelNum.Value;
            state.LevelNum = (int)levelNum.Value;

            if (currentLevel > 1)
            {
                panelPhysicalTile.BringToFront();
                panelPhysicalTile.Visible = true;
                panelPhysicalTileWorld.Visible = false;
            }
            else
            {
                panelPhysicalTileWorld.BringToFront();
                panelPhysicalTileWorld.Visible = true;
                panelPhysicalTile.Visible = false;
            }

            // Code that must happen before a level changes goes here
            state.Layer1 = buttonToggleL1.Checked = layer1ToolStripMenuItem.Checked = true;
            state.Layer2 = buttonToggleL2.Checked = layer2ToolStripMenuItem.Checked = true;
            state.Layer3 = buttonToggleL3.Checked = layer3ToolStripMenuItem.Checked = true;
            state.Priority1 = buttonToggleP1.Checked = priority1ToolStripMenuItem.Checked = true;
            state.BG = buttonToggleBG.Checked = backgroundToolStripMenuItem.Checked = true;

            coleditSelectCommand.SelectedIndex = 0;
            colEditReds.Checked = true;
            colEditGreens.Checked = true;
            colEditBlues.Checked = true;
            RefreshLevel.RunWorkerAsync();

            

            updatingLevel = false;
        }
        private void RefreshLevel_DoWork(object sender, DoWorkEventArgs e)
        {
            updatingLevel = true; // Start

            layer = levels[currentLevel].Layer;

            RefreshLevel.ReportProgress(0);

            if (currentLevel <= 2)
            {
                switch (currentLevel)
                {
                    case 0: paletteSet = paletteSets[48]; break;
                    case 1: paletteSet = paletteSets[49]; break;
                    case 2: paletteSet = paletteSets[50]; break;
                }
                wmTileSet = new WorldMapTileset(currentLevel, paletteSet, model);

                RefreshLevel.ReportProgress(25);

                wmTileMap = new WorldMapTilemap(currentLevel, layer, paletteSet, wmTileSet, model);
                tileSet = null;
                tileMap = null;
                overlay.WorldMap = true;
            }
            else
            {
                paletteSet = paletteSets[levels[currentLevel].Layer.PaletteSet];
                tileSet = new Tileset(layer, paletteSet, model);

                RefreshLevel.ReportProgress(25);

                tileMap = new TileMap(layer, paletteSet, tileSet, layer, prioritySets, model);
                wmTileSet = null;
                wmTileMap = null;
                overlay.WorldMap = false;
            }

            npcs = levels[currentLevel].LevelNPCs;
            exits = levels[currentLevel].LevelExits;
            events = levels[currentLevel].LevelEvents;

            RefreshLevel.ReportProgress(50);

            Thread.Sleep(100);

            RefreshLevel.ReportProgress(75);

            Thread.Sleep(100);

            RefreshLevel.ReportProgress(100);

            Thread.Sleep(100);
        }
        private void RefreshLevel_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            labelProgress.BackColor = Color.FromArgb(255, 128, 160, 255);
            progressBarLevel.Value = e.ProgressPercentage;

            if (e.ProgressPercentage < 25)
            {
                if (currentLevel == 2)
                    tabControl3.SelectedIndex = 0;
                if (currentLevel < 3)
                {
                    if (tabControl1.SelectedIndex == 0 || tabControl1.SelectedIndex == 2)
                        tabControl1.SelectedIndex = 1;
                }

                labelProgress.Text = "DRAWING TILESET...";
            }
            else if (e.ProgressPercentage < 50)
            {
                labelProgress.Text = "DRAWING TILEMAP...";

                if (currentLevel < 3)
                {
                    tabControl2.SelectedIndex = 0;
                    panel39.Enabled = false;
                    panel28.Enabled = false;
                    panel29.Enabled = false;
                    mapPaletteSetNum.Enabled = false;
                    panel6.Enabled = false;
                    panel13.Enabled = false;
                    panel36.Enabled = false;
                    panel35.Enabled = false;
                }
                else
                {
                    panel39.Enabled = true;
                    panel28.Enabled = true;
                    panel29.Enabled = true;
                    mapPaletteSetNum.Enabled = true;
                    panel6.Enabled = true;
                    panel13.Enabled = true;
                    panel36.Enabled = true;
                    panel35.Enabled = true;
                }
            }
            else if (e.ProgressPercentage < 75)
            {
                labelProgress.Text = "INITIALIZING PROPERTIES...";

                if (currentLevel > 2)
                {
                    InitializeLayerProperties();
                    InitializeCurrentColor();
                    InitializeNPCProperties();
                    InitializeTreasureProperties();
                }
                InitializeExitShortProperties();
                InitializeExitLongProperties();
                InitializeEventProperties();

                InitializeTile();
            }
            else if (e.ProgressPercentage < 100)
            {
                labelProgress.Text = "SETTING IMAGES...";

                pictureBoxLevel.Size = currentLevel < 2 ? new Size(4096, 4096) : new Size(2048, 2048);

                SetLevelImage();
                SetTilesetImage();
                SetPaletteSetImage();
            }

            if (Model.IsExpanded)
            {
                tbLocationName.Text = Bits.IsValidMapId(levelNames[currentLevel].Substring(0, 6))
                    ? levelNames[currentLevel].Substring(6, levelNames[currentLevel].Length - 6).Trim()
                    : levelNames[currentLevel].Trim();
            }

            tbMessageName.Text = messageNames[layer.MessageBox];
        }
        private void RefreshLevel_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            updatingLevel = false; // Done

            labelProgress.Text = "FINISHED LOADING LEVEL";

            panel27.Show();
            panel61.Show();
            levelNum.Enabled = true;
            levelName.Enabled = true;

            GC.Collect();

            progressBarLevel.Value = 0;
            labelProgress.BackColor = Color.FromArgb(255, 0, 255, 0);

            if (currentLevel < 3)
            {
                l2TilemapToolStripMenuItem.Enabled = false;
                l3TilemapToolStripMenuItem.Enabled = false;
                l2TilemapToolStripMenuItem1.Enabled = false;
                l3TilemapToolStripMenuItem1.Enabled = false;
            }
            else
            {
                l2TilemapToolStripMenuItem.Enabled = true;
                l3TilemapToolStripMenuItem.Enabled = true;
                l2TilemapToolStripMenuItem1.Enabled = true;
                l3TilemapToolStripMenuItem1.Enabled = true;
            }

            overEvent = 0;
            overExitLong = 0;
            overExitShort = 0;
            overNPC = 0;
            overTreasure = 0;
            isOverSomething = false;
        }

        private void DecompressLevelData()
        {
            pBar = new ProgressBar(this.model, model.Data, "DECOMPRESSING LEVEL DATA...", 570);
            pBar.Show();

            model.LoadVarCompDataAbsPtrs();   // LJ 2011-12-28: interoperability fix for FF3usME

            int pointer = 0;
            int offset = 0;
            byte[] temps;
            string labelText;

            for (int i = 0; i < 82; i++)
            {
                temps = new byte[0x2000];
                pointer = (i * 3) + 0x1FDA00;
                offset = (int)(ByteManage.GetInt(model.Data, pointer) + 0x1FDB00);

                for (int j = 0; j < 0x2000; j++)
                {
                    temps[j] = ByteManage.GetByte(model.Data, offset);
                    offset++;
                }
                model.GraphicSets[i] = temps;

                labelText = "STORING GRAPHIC SET 0x" + i.ToString("X3");
                pBar.PerformStep(labelText);
            }

            for (int i = 0; i < 19; i++)
            {
                temps = new byte[0x1040];
                model.GraphicSetsL3[i] = new byte[0x1000];
                pointer = (i * 3) + 0x26CD60;
                offset = (int)(ByteManage.GetInt(model.Data, pointer) + 0x268780);

                temps = model.Decompress(offset, 0x1040);

                for (int j = 0; j < 0x1000; j++)
                {
                    model.GraphicSetsL3[i][j] = temps[j + 0x40];
                }

                labelText = "STORING GRAPHIC SET 0x" + i.ToString("X3");
                pBar.PerformStep(labelText);
            }


            // LJ 2011-12-28: interoperability fix for FF3usME
/*
            model.WobGraphicSet = model.Decompress(0x2F114F, 0x2480);
            model.WobTileMap = model.Decompress(0x2ED434, 0x10000);
            model.WorGraphicSet = model.Decompress(0x2F4A46, 0x2480);
            model.WorTileMap = model.Decompress(0x2F6A56, 0x10000);
*/
            //
	        // LJ 2011-12-28: load and decompress regular maps (WoB & WoR)
            model.WobTileMap = model.Decompress((int)model.HiROMToSMC(model.m_varCompDataAbsPtrs[model.IDX_VARP_MAP_WOB]), 0x10000);
            model.WorTileMap = model.Decompress((int)model.HiROMToSMC(model.m_varCompDataAbsPtrs[model.IDX_VARP_MAP_WOR]), 0x10000);

	        //
	        // LJ 2011-12-28: load and decompress maps tile data (WoB & WoR)
            model.WobGraphicSet = model.Decompress((int)model.HiROMToSMC(model.m_varCompDataAbsPtrs[model.IDX_VARP_TILE_WOB]), 0x2480);
            model.WorGraphicSet = model.Decompress((int)model.HiROMToSMC(model.m_varCompDataAbsPtrs[model.IDX_VARP_TILE_WOR]), 0x2480);

            //
            // LJ 2011-12-28: load and decompress mini-maps bitmaps (WoB & WoR)
            model.WobMiniMap = model.Decompress((int)model.HiROMToSMC(model.m_varCompDataAbsPtrs[model.IDX_VARP_MMAP_WOB]), 0x800);
            model.WorMiniMap = model.Decompress((int)model.HiROMToSMC(model.m_varCompDataAbsPtrs[model.IDX_VARP_MMAP_WOR]), 0x800);


            model.StGraphicSet = model.Decompress(0x2FB631, 0x2480);
            model.StTileMap = model.Decompress(0x2F9D17, 0x4000);
            model.StPaletteSet = model.Decompress(0x18E6BA, 0x200);

            model.WobPhysicalMap = ByteManage.GetByteArray(model.Data, 0x2E9B14, 0x200);
            model.WorPhysicalMap = ByteManage.GetByteArray(model.Data, 0x2E9D14, 0x200);

            for (int i = 0; i < 75; i++)
            {
                pointer = (i * 3) + 0x1FBA00;
                offset = (int)(ByteManage.GetInt(model.Data, pointer) + 0x1E0000);

                model.TileSets[i] = model.Decompress(offset, 0x800);

                labelText = "DECOMPRESSING TILE SET 0x" + i.ToString("X3");
                pBar.PerformStep(labelText);
            }

            //madsiur
            for (int i = 0; i < Model.NUM_TILEMAPS; i++)
            {
                pointer = (i * 3) + Model.BASE_TILEMAP_PTR;
                offset = (int)(ByteManage.GetInt(model.Data, pointer) + Model.BASE_TILEMAP);

                model.TileMaps[i] = model.Decompress(offset, 0x4000, ref model.TileMapSizes[i]);

                labelText = "DECOMPRESSING TILE MAP 0x" + i.ToString("X3");
                pBar.PerformStep(labelText);
            }

            model.AnimatedGraphics = ByteManage.GetByteArray(model.Data, 0x260000, 0x8000);

            for (int i = 0; i < 43; i++)
            {
                pointer = (i * 2) + 0x19CD10;
                offset = (int)(ByteManage.GetShort(model.Data, pointer) + 0x19A800);

                model.PhysicalMaps[i] = model.Decompress(offset, 0x200);

                labelText = "DECOMPRESSING SOLIDITY SET 0x" + i.ToString("X3");
                pBar.PerformStep(labelText);
            }

            pBar.Close();
        }

        private void RecompressLevelData(ProgressBar pBar)
        {
            int size, offset, pointer;
            byte[] compressed = new byte[0x10000];
            byte[] data = model.Data;

            pBar = new ProgressBar(this.model, model.Data, "COMPRESSING AND SAVING LEVEL DATA...", 476);
            // + whatever else
            pBar.Show();


            // LJ 2011-12-28: interoperability fix for FF3usME
            int lSmcOffsetRegBank;
            int lSmcOffsetExpBank;
            int lRemBytesInRegBank;
            int lRemBytesInExpBank = 0x10000;

            // LJ: this preps the data pointer to the expanded bank, every section shall be stiched to each other, so this will be incremented in the following lines
            model.m_savedExpandedBytes = 0;
//            lSmcOffsetExpBank = model.OFFS_FF3ED_DTE_D_EX + model.m_savedExpandedBytes;
            lSmcOffsetExpBank = ((m_expBank - 0xC0)*0x10000) + model.m_savedExpandedBytes;

            //
            // LJ 2011-12-28: WoB map data and tile set
            lRemBytesInRegBank = model.LEN_WOB_MAP_DT_TL;
            // LJ: this is remaining bytes for both mini-maps, for sure at least one of 'em 'll fit
            lSmcOffsetRegBank = model.OFFS_WOB_MAP_DT_TL;

//            if (model.EditWobTileMap)
            {
                compressed = new byte[0x10000];
                size = model.Compress(model.WobTileMap, compressed);

                // data bigger than remaining bytes in regular section
                if (size > lRemBytesInRegBank)
                {
                    // file is expanded, park the data in expanded section
                    if (model.GetFileSize() >= 0x400000) // LJ: only deal with 32-Mbit expansion for the moment
                    {
                        if (size <= lRemBytesInExpBank)
                        {
                            model.m_varCompDataAbsPtrs[model.IDX_VARP_MAP_WOB] =
                                model.SMCToHiROM((ulong) lSmcOffsetExpBank);
                            ByteManage.SetByteArray(data, lSmcOffsetExpBank, compressed, 0, size);
                            lSmcOffsetExpBank += size;
                            lRemBytesInExpBank -= size;
                        }
                        else
                        {
                            MessageBox.Show(
                                "Recompressed WoB tilemap exceeds allotted space in exp. bank.\n" +
                                "The WoB tilemap was not saved.",
                                "WARNING: NOT ENOUGH SPACE FOR WOB TILEMAP",
                                MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        }
                    }
                    // file is not expanded, there is nothing that we can do further
                    else
                    {
                        MessageBox.Show(
                            "Recompressed WOB tilemap exceeds allotted space.\n" +
                            "The WOB tilemap was not saved.",
                            "WARNING: NOT ENOUGH SPACE FOR WOB TILEMAP",
                            MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    }
                }
                // data fits in regular section
                else
                {
                    lRemBytesInRegBank -= size;
                    model.m_varCompDataAbsPtrs[model.IDX_VARP_MAP_WOB] = model.SMCToHiROM((ulong) lSmcOffsetRegBank);
                    ByteManage.SetByteArray(data, lSmcOffsetRegBank, compressed, 0, size);
                    lSmcOffsetRegBank += size;
                }
            }
            pBar.PerformStep("COMPRESSING WOB TILEMAP");

//            if (model.EditWobGraphicSet)
            {
                compressed = new byte[0x2480];
                size = model.Compress(model.WobGraphicSet, compressed);

                // data bigger than remaining bytes in regular section
                if (size > lRemBytesInRegBank)
                {
                    // file is expanded, park the data in expanded section
                    if (model.GetFileSize() >= 0x400000) // LJ: only deal with 32-Mbit expansion for the moment
                    {
                        if (size <= lRemBytesInExpBank)
                        {
                            model.m_varCompDataAbsPtrs[model.IDX_VARP_TILE_WOB] =
                                model.SMCToHiROM((ulong) lSmcOffsetExpBank);
                            ByteManage.SetByteArray(data, lSmcOffsetExpBank, compressed, 0, size);
                            lSmcOffsetExpBank += size;
                            lRemBytesInExpBank -= size;
                        }
                        else
                        {
                            MessageBox.Show(
                                "Recompressed WoB tileset exceeds allotted space in exp. bank.\n" +
                                "The WoB tileset was not saved.",
                                "WARNING: NOT ENOUGH SPACE FOR WOB TILESET",
                                MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        }
                    }
                    // file is not expanded, there is nothing that we can do further
                    else
                    {
                        MessageBox.Show(
                            "Recompressed WOB tileset exceeds allotted space.\n" +
                            "The WOB tileset was not saved.",
                            "WARNING: NOT ENOUGH SPACE FOR WOB TILESET",
                            MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    }
                }
                // data fits in regular section
                else
                {
                    lRemBytesInRegBank -= size;
                    model.m_varCompDataAbsPtrs[model.IDX_VARP_TILE_WOB] = model.SMCToHiROM((ulong) lSmcOffsetRegBank);
                    ByteManage.SetByteArray(data, lSmcOffsetRegBank, compressed, 0, size);
                    lSmcOffsetRegBank += size;
                }
            }
            pBar.PerformStep("COMPRESSING WOB TILESET");


            //
            // LJ 2011-12-28: WoR map data and tile set
            lRemBytesInRegBank = model.LEN_WOR_MAP_DT_TL;
            // LJ: this is remaining bytes for both mini-maps, for sure at least one of 'em 'll fit
            lSmcOffsetRegBank = model.OFFS_WOR_MAP_DT_TL;

//            if (model.EditWorTileMap)
            {
                compressed = new byte[0x10000];
                size = model.Compress(model.WorTileMap, compressed);

                // data bigger than remaining bytes in regular section
                if (size > lRemBytesInRegBank)
                {
                    // file is expanded, park the data in expanded section
                    if (model.GetFileSize() >= 0x400000) // LJ: only deal with 32-Mbit expansion for the moment
                    {
                        if (size <= lRemBytesInExpBank)
                        {
                            model.m_varCompDataAbsPtrs[model.IDX_VARP_MAP_WOR] =
                                model.SMCToHiROM((ulong) lSmcOffsetExpBank);
                            model.m_varCompDataAbsPtrs[model.IDX_VARP_MAP_WOR2] =
                                model.SMCToHiROM((ulong) lSmcOffsetExpBank);
                            ByteManage.SetByteArray(data, lSmcOffsetExpBank, compressed, 0, size);
                            lSmcOffsetExpBank += size;
                            lRemBytesInExpBank -= size;
                        }
                        else
                        {
                            MessageBox.Show(
                                "Recompressed WoR tilemap exceeds allotted space in exp. bank.\n" +
                                "The WoR tilemap was not saved.",
                                "WARNING: NOT ENOUGH SPACE FOR WOR TILEMAP",
                                MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        }
                    }
                    // file is not expanded, there is nothing that we can do further
                    else
                    {
                        MessageBox.Show(
                            "Recompressed WOR tilemap exceeds allotted space.\n" +
                            "The WOR tilemap was not saved.",
                            "WARNING: NOT ENOUGH SPACE FOR WOR TILEMAP",
                            MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    }
                }
                // data fits in regular section
                else
                {
                    lRemBytesInRegBank -= size;
                    model.m_varCompDataAbsPtrs[model.IDX_VARP_MAP_WOR] = model.SMCToHiROM((ulong) lSmcOffsetRegBank);
                    model.m_varCompDataAbsPtrs[model.IDX_VARP_MAP_WOR2] = model.SMCToHiROM((ulong) lSmcOffsetRegBank);
                    ByteManage.SetByteArray(data, lSmcOffsetRegBank, compressed, 0, size);
                    lSmcOffsetRegBank += size;
                }
            }
            pBar.PerformStep("COMPRESSING WOR TILEMAP");

//            if (model.EditWorGraphicSet)
            {
                compressed = new byte[0x2480];
                size = model.Compress(model.WorGraphicSet, compressed);

                // data bigger than remaining bytes in regular section
                if (size > lRemBytesInRegBank)
                {
                    // file is expanded, park the data in expanded section
                    if (model.GetFileSize() >= 0x400000) // LJ: only deal with 32-Mbit expansion for the moment
                    {
                        if (size <= lRemBytesInExpBank)
                        {
                            model.m_varCompDataAbsPtrs[model.IDX_VARP_TILE_WOR] =
                                model.SMCToHiROM((ulong) lSmcOffsetExpBank);
                            ByteManage.SetByteArray(data, lSmcOffsetExpBank, compressed, 0, size);
                            lSmcOffsetExpBank += size;
                            lRemBytesInExpBank -= size;
                        }
                        else
                        {
                            MessageBox.Show(
                                "Recompressed WoR tileset exceeds allotted space in exp. bank.\n" +
                                "The WoR tileset was not saved.",
                                "WARNING: NOT ENOUGH SPACE FOR WOR TILESET",
                                MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        }
                    }
                    // file is not expanded, there is nothing that we can do further
                    else
                    {
                        MessageBox.Show(
                            "Recompressed WoR tileset exceeds allotted space.\n" +
                            "The WoR tileset was not saved.",
                            "WARNING: NOT ENOUGH SPACE FOR WOR TILESET",
                            MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    }
                }
                // data fits in regular section
                else
                {
                    lRemBytesInRegBank -= size;
                    model.m_varCompDataAbsPtrs[model.IDX_VARP_TILE_WOR] = model.SMCToHiROM((ulong) lSmcOffsetRegBank);
                    ByteManage.SetByteArray(data, lSmcOffsetRegBank, compressed, 0, size);
                    lSmcOffsetRegBank += size;
                }
            }
            pBar.PerformStep("COMPRESSING WOR TILESET");


            //
            // LJ 2011-12-28: Mini-maps data is now handled by FF3LE, though not modified yet
            lRemBytesInRegBank = model.LEN_WOB_WOR_MMAP;
            // LJ: this is remaining bytes for both mini-maps, for sure at least one of 'em 'll fit
            lSmcOffsetRegBank = model.OFFS_WOB_WOR_MMAP;

//            if (model.EditWobTileMap)
            {
                // TODO: get the code from FF3Ed that auto-creates the mini-map from what the user had done to the map data

                compressed = new byte[0x800];
                size = model.Compress(model.WobMiniMap, compressed);

                // data bigger than remaining bytes in regular section
                if (size > lRemBytesInRegBank)
                {
                    // file is expanded, park the data in expanded section
                    if (model.GetFileSize() >= 0x400000) // LJ: only deal with 32-Mbit expansion for the moment
                    {
                        if (size <= lRemBytesInExpBank)
                        {
                            model.m_varCompDataAbsPtrs[model.IDX_VARP_MMAP_WOB] =
                                model.SMCToHiROM((ulong) lSmcOffsetExpBank);
                            ByteManage.SetByteArray(data, lSmcOffsetExpBank, compressed, 0, size);
                            lSmcOffsetExpBank += size;
                            lRemBytesInExpBank -= size;
                        }
                        else
                        {
                            MessageBox.Show(
                                "Recompressed WoB mini-map exceeds allotted space in exp. bank.\n" +
                                "The WoB mini-map was not saved.",
                                "WARNING: NOT ENOUGH SPACE FOR WOB MINI-MAP",
                                MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        }
                    }
                    // file is not expanded, there is nothing that we can do further
                    else
                    {
                        MessageBox.Show(
                            "Recompressed WoB mini-map exceeds allotted space.\n" +
                            "The WoB mini-map was not saved.",
                            "WARNING: NOT ENOUGH SPACE FOR WOB MINI-MAP",
                            MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    }
                }
                // data fits in regular section
                else
                {
                    lRemBytesInRegBank -= size;
                    model.m_varCompDataAbsPtrs[model.IDX_VARP_MMAP_WOB] = model.SMCToHiROM((ulong) lSmcOffsetRegBank);
                    ByteManage.SetByteArray(data, lSmcOffsetRegBank, compressed, 0, size);
                    lSmcOffsetRegBank += size;
                }
            }
            pBar.PerformStep("COMPRESSING WOB MINI-MAP...");


//            if (model.EditWorTileMap)
            {
                // TODO: get the code from FF3Ed that auto-creates the mini-map from what the user had done to the map data

                compressed = new byte[0x800];
                size = model.Compress(model.WorMiniMap, compressed);

                // data bigger than remaining bytes in regular section
                if (size > lRemBytesInRegBank)
                {
                    // file is expanded, park the data in expanded section
                    if (model.GetFileSize() >= 0x400000) // LJ: only deal with 32-Mbit expansion for the moment
                    {
                        if (size <= lRemBytesInExpBank)
                        {
                            model.m_varCompDataAbsPtrs[model.IDX_VARP_MMAP_WOR] =
                                model.SMCToHiROM((ulong) lSmcOffsetExpBank);
                            ByteManage.SetByteArray(data, lSmcOffsetExpBank, compressed, 0, size);
                            lSmcOffsetExpBank += size;
                            lRemBytesInExpBank -= size;
                        }
                        else
                        {
                            MessageBox.Show(
                                "Recompressed WoR mini-map exceeds allotted space in exp. bank.\n" +
                                "The WoR mini-map was not saved.",
                                "WARNING: NOT ENOUGH SPACE FOR WOR MINI-MAP",
                                MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        }
                    }
                    // file is not expanded, there is nothing that we can do further
                    else
                    {
                        MessageBox.Show(
                            "Recompressed WoR mini-map exceeds allotted space.\n" +
                            "The WoR mini-map was not saved.",
                            "WARNING: NOT ENOUGH SPACE FOR WOR MINI-MAP",
                            MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    }
                }
                // data fits in regular section
                else
                {
                    lRemBytesInRegBank -= size;
                    model.m_varCompDataAbsPtrs[model.IDX_VARP_MMAP_WOR] = model.SMCToHiROM((ulong) lSmcOffsetRegBank);
                    ByteManage.SetByteArray(data, lSmcOffsetRegBank, compressed, 0, size);
                    lSmcOffsetRegBank += size;
                }
            }
            pBar.PerformStep("COMPRESSING WOR MINI-MAP...");


            // LJ 2011-12-28: we're finished taking expanded bytes for now, update the amount counter:
//            model.m_savedExpandedBytes = lSmcOffsetExpBank - model.OFFS_FF3ED_DTE_D_EX;
            model.m_savedExpandedBytes = lSmcOffsetExpBank - ((m_expBank - 0xC0)*0x10000);









            // SERPENT TRENCH
            if (model.EditStTileMap)
            {
                compressed = new byte[0x10000];
                size = model.Compress(model.StTileMap, compressed);

                if (size > 0x191A)
                    MessageBox.Show(
                        "Recompressed SERPENT TRENCH tilemap exceeds allotted space.\n" +
                        "The SERPENT TRENCH tilemap was not saved.",
                        "WARNING: NOT ENOUGH SPACE FOR SERPENT TRENCH TILEMAP",
                        MessageBoxButtons.OK, MessageBoxIcon.Stop);
                else
                    ByteManage.SetByteArray(data, 0x2F9D17, compressed, 0, size);
            }
            pBar.PerformStep("COMPRESSING SERPENT TRENCH TILEMAP");

            if (model.EditStGraphicSet)
            {
                compressed = new byte[0x2480];
                size = model.Compress(model.StGraphicSet, compressed);

                if (size > 0x0FF3)
                    MessageBox.Show(
                        "Recompressed SERPENT TRENCH tileset exceeds allotted space.\n" +
                        "The SERPENT TRENCH tileset was not saved.",
                        "WARNING: NOT ENOUGH SPACE FOR SERPENT TRENCH TILESET",
                        MessageBoxButtons.OK, MessageBoxIcon.Stop);
                else
                    ByteManage.SetByteArray(data, 0x2FB631, compressed, 0, size);
            }
            pBar.PerformStep("COMPRESSING SERPENT TRENCH TILESET");

            compressed = new byte[0x200];
            size = model.Compress(model.StPaletteSet, compressed);

            if (size > 0x0146)
                MessageBox.Show(
                    "Recompressed SERPENT TRENCH palette exceeds allotted space.\n" +
                    "The SERPENT TRENCH palette was not saved.",
                    "WARNING: NOT ENOUGH SPACE FOR SERPENT TRENCH PALETTE",
                    MessageBoxButtons.OK, MessageBoxIcon.Stop);
            else
                ByteManage.SetByteArray(data, 0x18E6BA, compressed, 0, size);

            pBar.PerformStep("COMPRESSING SERPENT TRENCH PALETTE");

            ByteManage.SetByteArray(data, 0x2E9B14, model.WobPhysicalMap);
            ByteManage.SetByteArray(data, 0x2E9D14, model.WorPhysicalMap);

            // STORE ORIGINAL TILESETS FOR REWRITING NON-MODIFIED TILESETS
            byte[][] tilesetsTemp = new byte[75][];
            for (int i = 0; i < 75; i++)
            {
                pointer = (i*3) + 0x1FBA00;
                offset = (int) (ByteManage.GetInt(model.Data, pointer) + 0x1E0000);

                tilesetsTemp[i] = ByteManage.GetByteArray(data, offset, ByteManage.GetShort(data, offset));
            }
            // COMPRESS ONLY THE EDITED TILESETS
            int temp = 0;
            pointer = 0;
            offset = 0x1E0000;
            for (int i = 0; i < model.TileSets.Length; i++)
            {
                if (model.EditTileMaps[i])
                {
                    compressed = new byte[0x800];
                    size = model.Compress(model.TileSets[i], compressed);
                }
                else
                {
                    compressed = tilesetsTemp[i];
                    size = compressed.Length;
                }

                temp = (ushort) ((offset - 0x1E0000) & 0xFFFF) + (byte) ((offset - 0x1E0000) >> 16);
                if (offset + size > 0x1FB3FF) // not enough space
                {
                    MessageBox.Show(
                        "Recompressed tilesets exceed allotted space.\n" +
                        "The editor stopped saving at tileset 0x" + i.ToString("X3"),
                        "WARNING: NOT ENOUGH SPACE FOR TILESETS",
                        MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    break;
                }
                else
                {
                    ByteManage.SetShort(data, 0x1FBA00 + (i*3), (ushort) ((offset - 0x1E0000) & 0xFFFF));
                    data[0x1FBA00 + (i*3) + 2] = (byte) ((offset - 0x1E0000) >> 16);
                    ByteManage.SetByteArray(data, offset, compressed);
                }

                offset += size;

                pBar.PerformStep("COMPRESSING TILESET 0x" + i.ToString("X3"));
            }

            // STORE ORIGINAL TILEMAPS FOR REWRITING NON-MODIFIED TILEMAPS
            byte[][] tilemapsTemp = new byte[Model.NUM_TILEMAPS][];
            for (int i = 0; i < Model.NUM_TILEMAPS; i++)
            {
                pointer = (i*3) + Model.BASE_TILEMAP_PTR;
                offset = (int) (ByteManage.GetInt(model.Data, pointer) + Model.BASE_TILEMAP);

                tilemapsTemp[i] = ByteManage.GetByteArray(data, offset, ByteManage.GetShort(data, offset));
            }
            //madsiur
            // COMPRESS ONLY THE EDITED TILEMAPS
            temp = 0;
            pointer = 0;
            offset = Model.BASE_TILEMAP;
            for (int i = 0; i < model.TileMaps.Length; i++)
            {
                if (model.EditTileMaps[i])
                {
                    compressed = new byte[model.TileMapSizes[i]];
                    size = model.CompressT(model.TileMaps[i], compressed);
                }
                else
                {
                    compressed = tilemapsTemp[i];
                    size = compressed.Length;
                }

                //madsiur
                temp = (ushort) ((offset - Model.BASE_TILEMAP) & 0xFFFF) + (byte) ((offset - Model.BASE_TILEMAP) >> 16);
                if (offset + size >= Model.BASE_TILEMAP + Model.SIZE_TILEMAP_DATA) // not enough space
                {
                    if (i == Model.NUM_TILEMAPS - 1)
                    {
                        ByteManage.SetShort(data, Model.BASE_TILEMAP_PTR + (i*3),
                            (ushort) ((offset - Model.BASE_TILEMAP) & 0xFFFF));
                        data[Model.BASE_TILEMAP_PTR + (i*3) + 2] = (byte) ((offset - Model.BASE_TILEMAP) >> 16);
                    }
                    else
                    {
                        MessageBox.Show(
                            "Recompressed tilemaps exceed allotted space.\n" +
                            "The editor stopped saving at tilemap 0x" + i.ToString("X3"),
                            "WARNING: NOT ENOUGH SPACE FOR TILEMAPS",
                            MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        break;
                    }
                }
                else
                {
                    ByteManage.SetShort(data, Model.BASE_TILEMAP_PTR + (i*3),
                        (ushort) ((offset - Model.BASE_TILEMAP) & 0xFFFF));
                    data[Model.BASE_TILEMAP_PTR + (i*3) + 2] = (byte) ((offset - Model.BASE_TILEMAP) >> 16);
                    ByteManage.SetByteArray(data, offset, compressed);
                }

                offset += size;

                pBar.PerformStep("COMPRESSING TILEMAP 0x" + i.ToString("X3"));
            }

            // STORE ORIGINAL PHYSICAL MAPS FOR REWRITING NON-MODIFIED TILEMAPS
            byte[][] physmapsTemp = new byte[43][];
            for (int i = 0; i < 43; i++)
            {
                pointer = (i*2) + 0x19CD10;
                offset = (int) (ByteManage.GetShort(model.Data, pointer) + 0x19A800);

                physmapsTemp[i] = ByteManage.GetByteArray(data, offset, ByteManage.GetShort(data, offset));
            }
            // COMPRESS ONLY THE EDITED PHYSICAL MAPS
            temp = 0;
            pointer = 0;
            offset = 0x19A800;
            for (int i = 0; i < model.PhysicalMaps.Length; i++)
            {
                if (model.EditPhysicalMaps[i])
                {
                    compressed = new byte[0x200];
                    size = model.Compress(model.PhysicalMaps[i], compressed);
                }
                else
                {
                    compressed = physmapsTemp[i];
                    size = compressed.Length;
                }

                temp = (ushort) ((offset - 0x19A800) & 0xFFFF) + (byte) ((offset - 0x19A800) >> 16);
                if (offset + size > 0x19CD0F) // not enough space
                {
                    if (i == 42)
                    {
                        ByteManage.SetShort(data, 0x19CD10 + (i*3), (ushort) ((offset - 0x19A800) & 0xFFFF));
                    }
                    else
                    {
                        MessageBox.Show(
                            "Recompressed solidity sets exceed allotted space.\n" +
                            "The editor stopped saving at solidity set 0x" + i.ToString("X3"),
                            "WARNING: NOT ENOUGH SPACE FOR SOLIDITY SETS",
                            MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        break;
                    }
                }
                else
                {
                    ByteManage.SetShort(data, 0x19CD10 + (i*2), (ushort) ((offset - 0x19A800) & 0xFFFF));
                    ByteManage.SetByteArray(data, offset, compressed);
                }

                offset += size;

                pBar.PerformStep("COMPRESSING SOLIDITY SET 0x" + i.ToString("X3"));
            }

            model.SaveVarCompDataAbsPtrs(); // LJ 2011-12-28: interoperability fix for FF3usME

            //madsiur, saving message names
            offset = 0;
            for (int i = 0; i < Model.NUM_LOC_NAMES; i++)
            {
                byte[] nameArray = SetLocNameArray(messageNames[i], i);

                // For DTE optimization
                //byte[] nameArray = SetLocNameArrayDTE(messageNames[i]);

                if (offset + nameArray.Length < Model.SIZE_LOC_NAMES)
                {
                    ByteManage.SetShort(model.Data, i * 2 + Model.BASE_LOC_NAMES_PTR, (ushort)offset);
                    ByteManage.SetByteArray(model.Data, offset + Model.BASE_LOC_NAMES, nameArray);
                    offset += nameArray.Length;
                    pBar.PerformStep("SAVING LOCATION NAME 0x" + i.ToString("X2"));
                }
                else
                {
                    MessageBox.Show(
                        "Location Names exceed allotted space ($" + Model.SIZE_LOC_NAMES.ToString("X4") + " bytes).\n" +
                        "The editor stopped saving at Location Name $" + i.ToString("X2"),
                        "WARNING: NOT ENOUGH SPACE FOR LOCATION NAMES",
                        MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    break;
                }
            }

            pBar.Close();
        }

        private byte[] SetLocNameArray(string s, int index)
        {
            char[] ch = s.ToCharArray();
            byte[] tc = new byte[ch.Length + 1];
            bool error = false;

            if (!(s.Equals("") || s.Equals(" ") || s.Length == 0))
            {
                for (int j = 0; j < ch.Length; j++)
                {
                    if (!error)
                    {
                        for (int k = 0; k < dialogueTable.Length; k++)
                        {
                            if (ch[j].ToString().Equals(dialogueTable[k]))
                            {
                                tc[j] = (byte)k;
                                k = dialogueTable.Length;

                                if (k == dialogueTable.Length - 1)
                                {
                                    MessageBox.Show("Unable to save message name " + index.ToString("X3") + " \"" +
                                                    messageNames[index] +
                                                    "\"). Default entry \"INN\" will be saved instead.");

                                    error = true;
                                }
                            }
                        }
                    }

                    tc[tc.Length - 1] = 0;
                }

                if (error)
                {
                    tc = Expansion.DEFAULT_LOC_NAME;
                }
            }
            else
            {
                tc = new byte[] { 0x00 };
            }

            return tc;
        }
        private byte[] SetLocNameArrayDTE(string s)
        {
            List<char> charList = s.ToList();
            byte[] result = new byte[charList.Count + 1];
            byte[] final;

            if (!(s.Equals("") || s.Equals(" ") || s.Length == 0))
            {
                int j = 0;
                for (j = 0; j < charList.Count; j++)
                {
                    string pattern = "";
                    int position = 0;

                    if (charList[j].Equals('<'))
                    {
                        int k = j - 1;

                        do
                        {
                            k++;
                            pattern += charList[k];
                        } while (!charList[k].Equals('>'));

                    }
                    else
                    {
                        string twoPattern = "";

                        if (j != charList.Count - 1)
                        {
                            twoPattern = charList[j] + charList[j + 1].ToString();
                        }

                        for (int l = 0; l < dialogueTable.Length; l++)
                        {
                            if (twoPattern.Equals(dialogueTable[l]))
                            {
                                position = l;
                                pattern = twoPattern;
                                break;
                            }
                        }
                    }

                    if (pattern.Equals("") || pattern.Length > 2)
                    {
                        if (pattern.Equals(""))
                        {
                            pattern = charList[j].ToString();
                        }

                        for (int m = 0; m < dialogueTable.Length; m++)
                        {
                            if (pattern.Equals(dialogueTable[m]))
                            {
                                position = m;
                                break;
                            }
                        }
                    }

                    if (position != 0)
                    {
                        result[j] = (byte)position;
                    }
                    else
                    {
                        result[j] = 0xFF;
                    }

                    if (pattern.Length > 1)
                    {
                        for (int n = 0; n < pattern.Length - 1; n++)
                        {
                            charList.RemoveAt(j + 1);
                        }
                    }
                }

                final = new byte[charList.Count + 1];
                Buffer.BlockCopy(result, 0, final, 0, final.Length);
                final[final.Length - 1] = 0x00;
            }
            else
            {
                final = new byte[] {0};
            }

            return final;
        }
        private string[] GetLevelNames()
        { 
            if (Model.IsExpanded)
            {
                return Model.Deserialized();
            }

            string[] names = new string[settings.LevelNames.Count];
            settings.LevelNames.CopyTo(names, 0);
            return Model.IterateLocations(names);
        }
        private string[] GetItemNames()
        {
            byte temp;
            string[] names = new string[256];
            for (int i = 0; i < 256; i++)
            {
                for (int a = 0; a < 13; a++)
                {
                    temp = model.Data[i * 13 + 0x12B301 + a];
                    if (temp == 0 || temp == 0xFE)
                        names[i] += " ";
                    else if (temp > 0x7F && temp < 0xD3 && temp != 0xFF)
                        names[i] += nameTable[temp - 0x80];
                }
            }
            return names;
        }
        ///madsiur
        private string[] GetMessageNames()
        {
            int offset;

            byte temp;
            string[] names = new string[Model.NUM_LOC_NAMES];
            for (int i = 0; i < Model.NUM_LOC_NAMES; i++)
            {
                offset = ByteManage.GetShort(model.Data, i * 2 + Model.BASE_LOC_NAMES_PTR) + Model.BASE_LOC_NAMES;
                names[i] = "";
                while (model.Data[offset] != 0)
                {
                    temp = model.Data[offset];

                    names[i] += dialogueTable[temp];

                    offset++;
                }
            }
            return names;
        }


        private void LoadSearch()
        {
            listBoxLevelNames.BeginUpdate();
            listBoxLevelNames.Items.Clear();

            for (int i = 0; i < levelName.Items.Count; i++)
            {
                if (Contains(levelName.Items[i].ToString(), nameTextBox.Text, StringComparison.CurrentCultureIgnoreCase))
                    listBoxLevelNames.Items.Add(levelName.Items[i]);
            }
            listBoxLevelNames.EndUpdate();
        }
        public static bool Contains(string original, string value, StringComparison comparisionType)
        {
            return original.IndexOf(value, comparisionType) >= 0;
        }

        private Bitmap DrawImageFromIntArr(int[] arr, int width, int height)
        {
            Bitmap image = null;

            unsafe
            {
                fixed (void* firstPixel = &arr[0])
                {
                    IntPtr ip = new IntPtr(firstPixel);
                    if (image != null)
                        image.Dispose();
                    image = new Bitmap(width, height, width * 4, System.Drawing.Imaging.PixelFormat.Format32bppPArgb, ip);

                }
            }

            return image;
        }

        public void SetLevelImage()
        {
            if (currentLevel < 3)
            {
                levelPixels = wmTileMap.Mainscreen;
                if (currentLevel < 2)
                    levelImage = new Bitmap(DrawImageFromIntArr(levelPixels, 4096, 4096));
                else
                    levelImage = new Bitmap(DrawImageFromIntArr(levelPixels, 2048, 2048));
            }
            else
            {
                levelPixels = tileMap.Mainscreen;
                levelImage = new Bitmap(DrawImageFromIntArr(levelPixels, 2048, 2048));
            }
            pictureBoxLevel.Invalidate();
        }
        public void SetTilesetImage()
        {
            if (currentLevel > 2)
            {
                tileSetPixels = tileSet.GetTilesetPixelArray(tileSet.TileSetLayers[tabControl2.SelectedIndex]);
                switch (tabControl2.SelectedIndex)
                {
                    case 0:
                        tileSetImage = new Bitmap(DrawImageFromIntArr(tileSetPixels, 256, 256));
                        pictureBoxTilesetL1.Invalidate(); break;
                    case 1:
                        tileSetImage = new Bitmap(DrawImageFromIntArr(tileSetPixels, 256, 256));
                        pictureBoxTilesetL2.Invalidate(); break;
                    case 2:
                        tileSetImage = new Bitmap(DrawImageFromIntArr(tileSetPixels, 256, 64));
                        pictureBoxTilesetL3.Invalidate(); break;
                    default: break;
                }
            }
            else
            {
                tileSetPixels = wmTileSet.GetTilesetPixelArray(wmTileSet.TileSetLayer);
                tileSetImage = new Bitmap(DrawImageFromIntArr(tileSetPixels, 256, 256));
                pictureBoxTilesetL1.Invalidate();
            }
        }

        public void Assemble()
        {
            //if (!model.AssembleLevels)
            //    return;
            //if (model.AssembleFinal)
            model.AssembleLevels = false;

            pBar = new ProgressBar(this.model, model.Data, "COMPRESSING AND SAVING LEVEL DATA...", 615); // + whatever else
            //pBar.Show();

            foreach (Level l in levels)
            {
                l.Assemble();
                //pBar.PerformStep("ASSEMBLING LEVEL DATA");
            }
            foreach (PrioritySet ps in prioritySets)
            {
                ps.Assemble();
            }
            foreach (PaletteSet ps in paletteSets)
            {
                ps.Assemble();
                //pBar.PerformStep("ASSEMBLING PALETTE SETS");
            }

            ushort offsetStart = (ushort)Model.SIZE_SHORT_EXIT_PTR;
            for (int i = 0; i < Model.NUM_LOCATIONS; i++)
            {
                offsetStart = levels[i].LevelExits.AssembleExitsShort(offsetStart);
                //pBar.PerformStep("LEVEL 0x" + i.ToString("X3") + " EXITS");
            }
            offsetStart = (ushort)Model.SIZE_LONG_EXIT_PTR;
            for (int i = 0; i < Model.NUM_LOCATIONS; i++)
            {
                offsetStart = levels[i].LevelExits.AssembleExitsLong(offsetStart);
                //pBar.PerformStep("LEVEL 0x" + i.ToString("X3") + " EXITS");
            }

            offsetStart = (ushort)Model.SIZE_EVENT_PTR;
            for (int i = 0; i < Model.NUM_LOCATIONS; i++)
            {
                offsetStart = levels[i].LevelEvents.Assemble(offsetStart);
                //pBar.PerformStep("LEVEL 0x" + i.ToString("X3") + " EVENTS");
            }

            offsetStart = (ushort)Model.SIZE_NPC_PTR;
            for (int i = 0; i < Model.NUM_LOCATIONS; i++)
            {
                offsetStart = levels[i].LevelNPCs.AssembleNPCs(offsetStart);
                //pBar.PerformStep("LEVEL 0x" + i.ToString("X3") + " NPC'S");
            }

            offsetStart = 0;
            for (int i = 0; i < Model.NUM_LOCATIONS; i++)
            {
                offsetStart = levels[i].LevelNPCs.AssembleTreasures(offsetStart);
                //pBar.PerformStep("LEVEL 0x" + i.ToString("X3") + " NPC'S");
            }

            // ASSEMBLE THE TILEMAPS
            if (wmTileMap == null)
                tileMap.AssembleIntoModel(); // Assemble the edited tileMap into the model
            else
                wmTileMap.AssembleIntoModel();

            RecompressLevelData(pBar);

            //pBar.Close();
        }
        //public void CreateNewCommandStack()
        //{
        //    this.commandStack = new CommandStack(settings.UndoStackSize);
        //}

        #endregion

        #region Event Handlers

            //madsiur
        private void levelName_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (updatingLevel) return;

            levelNum.Value = levelName.SelectedIndex;
        }
        private void levelNum_ValueChanged(object sender, EventArgs e)
        {
            if (updatingLevel) return;

            overlay.DragStart = new Point(0, 0);
            overlay.DragStop = new Point(0, 0);
            overlay.TileSetDragStart = new Point(0, 0);
            overlay.TileSetDragStop = new Point(0, 0);

            ResetTileReplace();

            if (RefreshLevel.IsBusy) return;

            levelNum.Enabled = false;
            levelName.Enabled = false;
            panel27.Hide();
            panel61.Hide();

            levelName.SelectedIndex = currentLevel = (int)levelNum.Value;
            state.LevelNum = (int)levelNum.Value;

            if (currentLevel > 1)
            {
                panelPhysicalTile.BringToFront();
                panelPhysicalTile.Visible = true;
                panelPhysicalTileWorld.Visible = false;
            }
            else
            {
                panelPhysicalTileWorld.BringToFront();
                panelPhysicalTileWorld.Visible = true;
                panelPhysicalTile.Visible = false;
            }

            // Code that must happen before a level changes goes here
            state.Layer1 = buttonToggleL1.Checked = layer1ToolStripMenuItem.Checked = true;
            state.Layer2 = buttonToggleL2.Checked = layer2ToolStripMenuItem.Checked = true;
            state.Layer3 = buttonToggleL3.Checked = layer3ToolStripMenuItem.Checked = true;
            state.Priority1 = buttonToggleP1.Checked = priority1ToolStripMenuItem.Checked = true;
            state.BG = buttonToggleBG.Checked = backgroundToolStripMenuItem.Checked = true;

            if (wmTileMap == null)
                tileMap.AssembleIntoModel(); // Assemble the edited tileMap into the model
            else
                wmTileMap.AssembleIntoModel();

            //madsiur
            if(isLevelNameChanged)
            {
                isLevelNameChanged = false;
                int id = (int)((NumericUpDown)sender).Value;
                levelName.Items.Clear();
                levelNames = GetLevelNames();
                levelName.Items.AddRange(levelNames);
                levelName.SelectedIndex = id < levelNames.Length? id: 0;
            }
            RefreshLevel.RunWorkerAsync();

            
        }

        private void buttonToggleProperties_Click(object sender, EventArgs e)
        {
            if (this.tabControl1.Visible == true)
            {
                this.tabControl1.Visible = false;
                this.panelLevelPicture.Left -= tabControl1.Width + 2;
                this.panelLevelPicture.Width += tabControl1.Width + 2;
                this.buttonToggleProperties.Checked = false;
            }
            else if (this.tabControl1.Visible == false)
            {
                this.tabControl1.Visible = true;
                this.panelLevelPicture.Left += tabControl1.Width + 2;
                this.panelLevelPicture.Width -= tabControl1.Width + 2;
                this.buttonToggleProperties.Checked = true;
            }
        }
        private void buttonToggleCartGrid_Click(object sender, EventArgs e)
        {
            state.CartesianGrid = buttonToggleCartGrid.Checked;
            cartesianGridToolStripMenuItem.Checked = state.CartesianGrid;

            pictureBoxLevel.Invalidate();
            pictureBoxTilesetL1.Invalidate();
            pictureBoxTilesetL2.Invalidate();
            pictureBoxTilesetL3.Invalidate();
            pictureBoxTile.Invalidate();
            pictureBoxGraphicSet.Invalidate();
        }
        private void buttonToggleBounds_Click(object sender, EventArgs e)
        {
            pictureBoxLevel.Invalidate();
        }
        private void buttonToggleMask_Click(object sender, EventArgs e)
        {
            state.Mask = buttonToggleMask.Checked;
            maskToolStripMenuItem.Checked = state.Mask;

            pictureBoxLevel.Invalidate();
        }
        private void buttonToggleZoning_Click(object sender, EventArgs e)
        {
            state.Zones = buttonToggleZoning.Checked;
            battleZonesToolStripMenuItem.Checked = state.Zones;

            pictureBoxLevel.Invalidate();
        }
        private void buttonToggleL1_Click(object sender, EventArgs e)
        {
            state.Layer1 = buttonToggleL1.Checked;
            layer1ToolStripMenuItem.Checked = state.Layer1;

            if (currentLevel > 2)
            {
                tileMap.RedrawTileMap();
                SetLevelImage();
            }
        }
        private void buttonToggleL2_Click(object sender, EventArgs e)
        {
            state.Layer2 = buttonToggleL2.Checked;
            layer2ToolStripMenuItem.Checked = state.Layer2;

            if (currentLevel > 2)
            {
                tileMap.RedrawTileMap();
                SetLevelImage();
            }
        }
        private void buttonToggleL3_Click(object sender, EventArgs e)
        {
            state.Layer3 = buttonToggleL3.Checked;
            layer3ToolStripMenuItem.Checked = state.Layer3;

            if (currentLevel > 2)
            {
                tileMap.RedrawTileMap();
                SetLevelImage();
            }
        }
        private void buttonToggleP1_Click(object sender, EventArgs e)
        {
            state.Priority1 = buttonToggleP1.Checked;
            priority1ToolStripMenuItem.Checked = state.Priority1;

            if (currentLevel > 2)
            {
                tileMap.RedrawTileMap();
                tileSet.RedrawTilesets(tabControl2.SelectedIndex);
                SetLevelImage();
                SetTilesetImage();
            }
        }
        private void buttonToggleBG_Click(object sender, EventArgs e)
        {
            state.BG = buttonToggleBG.Checked;
            backgroundToolStripMenuItem.Checked = state.BG;

            switch (tabControl2.SelectedIndex)
            {
                case 0: pictureBoxTilesetL1.Invalidate(); break;
                case 1: pictureBoxTilesetL2.Invalidate(); break;
                case 2: pictureBoxTilesetL3.Invalidate(); break;
            }

            if (currentLevel > 2)
            {
                tileMap.RedrawTileMap();
                SetLevelImage();
            }
        }
        private void buttonTogglePhys_Click(object sender, EventArgs e)
        {
            state.PhysicalLayer = buttonTogglePhys.Checked;
            physicalMapToolStripMenuItem.Checked = state.PhysicalLayer;

            pictureBoxLevel.Invalidate();

            if (tabControl2.SelectedIndex == 0)
                pictureBoxTilesetL1.Invalidate();
        }
        private void buttonToggleNPCs_Click(object sender, EventArgs e)
        {
            state.Objects = buttonToggleNPCs.Checked;
            npcsToolStripMenuItem.Checked = state.Objects;

            pictureBoxLevel.Invalidate();
        }
        private void buttonToggleTreasures_Click(object sender, EventArgs e)
        {
            state.Treasures = buttonToggleTreasures.Checked;
            treasuresToolStripMenuItem.Checked = state.Treasures;

            pictureBoxLevel.Invalidate();
        }
        private void buttonToggleExits_Click(object sender, EventArgs e)
        {
            state.Exits = buttonToggleExits.Checked;
            exitFieldsToolStripMenuItem.Checked = state.Exits;

            pictureBoxLevel.Invalidate();
        }
        private void buttonToggleEvents_Click(object sender, EventArgs e)
        {
            state.Events = buttonToggleEvents.Checked;
            eventFieldsToolStripMenuItem.Checked = state.Events;

            pictureBoxLevel.Invalidate();
        }
        private void buttonEditSelect_Click(object sender, EventArgs e)
        {
            state.Select = buttonEditSelect.Checked;
            buttonEditDraw.Checked = false;
            buttonEditErase.Checked = false;
            buttonZoomIn.Checked = false;
            buttonZoomOut.Checked = false;
            if (state.Select)
                this.pictureBoxLevel.Cursor = System.Windows.Forms.Cursors.Cross;
            else if (!state.Select)
                this.pictureBoxLevel.Cursor = System.Windows.Forms.Cursors.Arrow;

            overlay.ClearSelection = true;
        }
        private void buttonEditErase_Click(object sender, EventArgs e)
        {
            state.Erase = buttonEditErase.Checked;
            buttonEditDraw.Checked = false;
            buttonEditSelect.Checked = false;
            buttonZoomIn.Checked = false;
            buttonZoomOut.Checked = false;
            if (state.Erase)
                this.pictureBoxLevel.Cursor = new System.Windows.Forms.Cursor(GetType(), "CursorErase.cur");
            else if (!state.Erase)
                this.pictureBoxLevel.Cursor = System.Windows.Forms.Cursors.Arrow;

            overlay.ClearSelection = true;
        }
        private void buttonEditDraw_Click(object sender, EventArgs e)
        {
            state.Draw = buttonEditDraw.Checked;
            buttonEditSelect.Checked = false;
            buttonEditErase.Checked = false;
            buttonZoomIn.Checked = false;
            buttonZoomOut.Checked = false;
            if (buttonEditDraw.Checked)
                this.pictureBoxLevel.Cursor = new System.Windows.Forms.Cursor(GetType(), "CursorDraw.cur");
            else if (!buttonEditDraw.Checked)
                this.pictureBoxLevel.Cursor = System.Windows.Forms.Cursors.Arrow;

            overlay.ClearSelection = true;
        }
        private void buttonEditDelete_Click(object sender, EventArgs e)
        {
            if (state.Move)
            {
                moveImage = null;
                state.Move = false;
                overlay.DragStart = new Point(0, 0);
                overlay.DragStop = new Point(0, 0);
                pictureBoxLevel.Invalidate();
            }
            else if (state.Select)
                MakeEditDelete();
        }
        private void buttonEditCut_Click(object sender, EventArgs e)
        {
            if (state.Select)
            {
                PasteFinal();
                Cut();
            }
        }
        private void buttonEditCopy_Click(object sender, EventArgs e)
        {
            if (state.Select)
            {
                PasteFinal();
                Copy();
            }
        }
        private void buttonEditPaste_Click(object sender, EventArgs e)
        {
            PasteFinal();

            Point p;
            if (state.Select && moveImage != null)
            {
                overlay.SelectionSize = moveImage.Size;

                p = new Point();
                p.X = (int)(panelLevelPicture.HorizontalScroll.Value / zoom) / (int)(16 * zoom) * (int)(16 * zoom);
                p.Y = (int)(panelLevelPicture.VerticalScroll.Value / zoom) / (int)(16 * zoom) * (int)(16 * zoom);
                overlay.DragStart = p;
                overlay.DragStop = new Point(p.X + moveImage.Width, p.Y + moveImage.Height);

                state.Move = true;

                pictureBoxLevel.Invalidate();
            }
        }
        private void buttonEditUndo_Click(object sender, EventArgs e)
        {

        }
        private void buttonEditRedo_Click(object sender, EventArgs e)
        {

        }
        private void buttonZoomIn_Click(object sender, EventArgs e)
        {
            buttonEditDraw.Checked = false;
            buttonEditErase.Checked = false;
            buttonEditSelect.Checked = false;
            buttonZoomOut.Checked = false;
            if (buttonZoomIn.Checked)
                this.pictureBoxLevel.Cursor = new System.Windows.Forms.Cursor(GetType(), "CursorZoomIn.cur");
            else if (!buttonZoomIn.Checked)
                this.pictureBoxLevel.Cursor = System.Windows.Forms.Cursors.Arrow;
        }
        private void buttonZoomOut_Click(object sender, EventArgs e)
        {
            buttonEditDraw.Checked = false;
            buttonEditErase.Checked = false;
            buttonEditSelect.Checked = false;
            buttonZoomIn.Checked = false;
            if (buttonZoomOut.Checked)
                this.pictureBoxLevel.Cursor = new System.Windows.Forms.Cursor(GetType(), "CursorZoomOut.cur");
            else if (!buttonZoomOut.Checked)
                this.pictureBoxLevel.Cursor = System.Windows.Forms.Cursors.Arrow;
        }
        private void buttonZoomIn_CheckedChanged(object sender, EventArgs e)
        {
            pictureBoxLevel.ContextMenuStrip = buttonZoomIn.Checked ? null : contextMenuStripLevel;
        }
        private void buttonZoomOut_CheckedChanged(object sender, EventArgs e)
        {
            pictureBoxLevel.ContextMenuStrip = buttonZoomOut.Checked ? null : contextMenuStripLevel;
        }

        private string SelectFile(string title, string filter)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.InitialDirectory = settings.LastRomPath;
            openFileDialog1.Title = title;
            openFileDialog1.Filter = filter;
            openFileDialog1.FilterIndex = 1;
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() != DialogResult.Cancel)
                return openFileDialog1.FileName;
            return null;
        }
        // todo: this is called by the level panel
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Assemble();
        }
        private void l1TilemapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string path = SelectFile("Import Tilemap...", "All Files (*.*)|*.*");

            if (path == null) return;

            FileStream fs = File.OpenRead(path);
            BinaryReader br = new BinaryReader(fs);

            byte[] buffer;

            if (currentLevel == 0)
            {
                model.EditWobTileMap = true;
                buffer = new byte[0x10000];
                model.WobTileMap = br.ReadBytes(Math.Min(buffer.Length, (int)fs.Length));
                wmTileMap = new WorldMapTilemap(currentLevel, layer, paletteSet, wmTileSet, model);
                SetLevelImage();
            }
            else if (currentLevel == 1)
            {
                model.EditWorTileMap = true;
                buffer = new byte[0x10000];
                model.WorTileMap = br.ReadBytes(Math.Min(buffer.Length, (int)fs.Length));
                wmTileMap = new WorldMapTilemap(currentLevel, layer, paletteSet, wmTileSet, model);
                SetLevelImage();
            }
            else if (currentLevel == 2)
            {
                model.EditStTileMap = true;
                buffer = new byte[0x4000];
                model.StTileMap = br.ReadBytes(Math.Min(buffer.Length, (int)fs.Length));
                wmTileMap = new WorldMapTilemap(currentLevel, layer, paletteSet, wmTileSet, model);
                SetLevelImage();
            }
            else
            {
                model.EditTileMaps[layer.TileMapL1] = true;
                buffer = new byte[0x4000];
                model.TileMaps[layer.TileMapL1] = br.ReadBytes(Math.Min(buffer.Length, (int)fs.Length));
                tileMap = new TileMap(layer, paletteSet, tileSet, layer, prioritySets, model);
                levelNum_ValueChanged(null, null);
            }
        }
        private void l2TilemapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string path = SelectFile("Import Tilemap...", "All Files (*.*)|*.*");

            if (path == null) return;

            FileStream fs = File.OpenRead(path);
            BinaryReader br = new BinaryReader(fs);

            //madsiur
            model.EditTileMaps[layer.TileMapL2] = true;

            byte[] buffer = new byte[0x4000];
            model.TileMaps[layer.TileMapL2] = br.ReadBytes(buffer.Length);
            tileMap = new TileMap(layer, paletteSet, tileSet, layer, prioritySets, model);
            levelNum_ValueChanged(null, null);
        }
        private void l3TilemapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string path = SelectFile("Import Tilemap...", "All Files (*.*)|*.*");

            if (path == null) return;

            FileStream fs = File.OpenRead(path);
            BinaryReader br = new BinaryReader(fs);
            
            //madsiur
            model.EditTileMaps[layer.TileMapL3] = true;

            byte[] buffer = new byte[0x4000];
            model.TileMaps[layer.TileMapL3] = br.ReadBytes(buffer.Length);
            tileMap = new TileMap(layer, paletteSet, tileSet, layer, prioritySets, model);
            levelNum_ValueChanged(null, null);
        }
        private void l1TilemapToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "BIN files (*.bin)|*.bin|All files (*.*)|*.*";
            saveFileDialog.FilterIndex = 1;
            if (currentLevel == 0)
                saveFileDialog.FileName = "wobTileMap.bin";
            else if (currentLevel == 1)
                saveFileDialog.FileName = "worTileMap.bin";
            else
                saveFileDialog.FileName = "tileMap." + layer.TileMapL1.ToString("X3") + ".bin";
            saveFileDialog.RestoreDirectory = true;

            if (saveFileDialog.ShowDialog() != DialogResult.OK) return;

            FileStream fs = new FileStream(saveFileDialog.FileName, FileMode.Create, FileAccess.ReadWrite); ;
            BinaryWriter bw = new BinaryWriter(fs);

            if (currentLevel == 0)
            {
                wmTileMap.AssembleIntoModel();
                bw.Write(model.WobTileMap, 0, 0x10000);
            }
            else if (currentLevel == 1)
            {
                wmTileMap.AssembleIntoModel();
                bw.Write(model.WorTileMap, 0, 0x10000);
            }
            else
            {
                tileMap.AssembleIntoModel();
                bw.Write(model.TileMaps[layer.TileMapL1], 0, 0x4000);
            }
        }
        private void l2TilemapToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "BIN files (*.bin)|*.bin|All files (*.*)|*.*";
            saveFileDialog.FilterIndex = 1;
            saveFileDialog.FileName = "tileMap." + layer.TileMapL2.ToString("X3") + ".bin";
            saveFileDialog.RestoreDirectory = true;

            if (saveFileDialog.ShowDialog() != DialogResult.OK) return;

            FileStream fs = new FileStream(saveFileDialog.FileName, FileMode.Create, FileAccess.ReadWrite); ;
            BinaryWriter bw = new BinaryWriter(fs);

            tileMap.AssembleIntoModel();
            bw.Write(model.TileMaps[layer.TileMapL2], 0, 0x4000);
        }
        private void l3TilemapToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "BIN files (*.bin)|*.bin|All files (*.*)|*.*";
            saveFileDialog.FilterIndex = 1;
            saveFileDialog.FileName = "tileMap." + layer.TileMapL3.ToString("X3") + ".bin";
            saveFileDialog.RestoreDirectory = true;

            if (saveFileDialog.ShowDialog() != DialogResult.OK) return;

            FileStream fs = new FileStream(saveFileDialog.FileName, FileMode.Create, FileAccess.ReadWrite); ;
            BinaryWriter bw = new BinaryWriter(fs);

            tileMap.AssembleIntoModel();
            bw.Write(model.TileMaps[layer.TileMapL3], 0, 0x4000);
        }
        private void toolStripMenuItem3_Click(object sender, EventArgs e)
        {
            // clear all components for all levels
            DialogResult result = MessageBox.Show(
                "WARNING: You are about to clear all level data, tilesets,\n" +
                "tilemaps, physical maps and battlefields for all levels.\n" +
                "This will essentially wipe the slate clean for anything\n" +
                "having to do with levels.\n\n" +
                "Are you sure you want to do this?",
                "WARNING: Clearing all Level Components...",
                MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation);

            if (result == DialogResult.OK)
            {
                foreach (Level l in levels)
                {
                    l.Layer.Clear();
                    l.LevelNPCs.ClearNPCs();
                    l.LevelExits.ClearExitsLong();
                    l.LevelExits.ClearExitsShort();
                    l.LevelNPCs.ClearTreasures();
                    l.LevelEvents.ClearEvents();
                }
                model.WobTileMap = new byte[0x10000];
                model.WorTileMap = new byte[0x10000];
                for (int i = 0; i < 0x400; i++)
                {
                    model.WobGraphicSet[i] = 0;
                    model.WorGraphicSet[i] = 0;
                }
                for (int i = 0x2400; i < 0x2480; i++)
                {
                    model.WobGraphicSet[i] = 0;
                    model.WorGraphicSet[i] = 0;
                }
                for (int i = 0; i < model.TileMaps.Length; i++)
                {
                    model.TileMaps[i] = new byte[0x4000];
                    model.TileMapSizes[i] = 0x100;
                }
                for (int i = 0; i < model.TileSets.Length; i++)
                    model.TileSets[i] = new byte[0x800];

                levelNum_ValueChanged(null, null);
            }
        }
        private void toolStripMenuItem4_Click(object sender, EventArgs e)
        {
            // clear all components for current level
            DialogResult result = MessageBox.Show(
                "WARNING: You are about to clear all level data, tilesets,\n" +
                "tilemaps, physical maps and battlefields for this level.\n\n" +
                "Are you sure you want to do this?",
                "WARNING: Clearing all Level Components...",
                MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation);

            if (result == DialogResult.OK)
            {
                if (currentLevel == 0)
                {
                    model.WobTileMap = new byte[0x10000];
                    for (int i = 0; i < 0x400; i++)
                        model.WobGraphicSet[i] = 0;
                    for (int i = 0x2400; i < 0x2480; i++)
                        model.WobGraphicSet[i] = 0;
                }
                else if (currentLevel == 1)
                {
                    model.WorTileMap = new byte[0x10000];
                    for (int i = 0; i < 0x400; i++)
                        model.WorGraphicSet[i] = 0;
                    for (int i = 0x2400; i < 0x2480; i++)
                        model.WorGraphicSet[i] = 0;
                }
                else
                {
                    tileMap.Clear(1);
                    model.TileMapSizes[layer.TileMapL1] = 0x100;
                    model.TileMapSizes[layer.TileMapL2] = 0x100;
                    model.TileMapSizes[layer.TileMapL3] = 0x100;
                    tileSet.Clear(1);
                }

                layer.Clear();
                npcs.ClearNPCs();
                exits.ClearExitsLong();
                exits.ClearExitsShort();
                npcs.ClearTreasures();
                events.ClearEvents();

                levelNum_ValueChanged(null, null);
            }
        }
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Undo();
            overlay.DragStart = new Point(0, 0);
            overlay.DragStop = new Point(0, 0);
            pictureBoxLevel.Invalidate();
        }
        private void redoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Redo();
        }
        private void cutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            buttonEditCut_Click(null, null);
        }
        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            buttonEditCopy_Click(null, null);
        }
        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            buttonEditPaste_Click(null, null);
        }
        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            buttonEditDelete_Click(null, null);
        }
        private void clearSelectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PasteFinal();

            overlay.DragStart = new Point(0, 0);
            overlay.DragStop = new Point(0, 0);
            overlay.TileSetDragStart = new Point(0, 0);
            overlay.TileSetDragStop = new Point(0, 0);
            pictureBoxLevel.Invalidate();
            pictureBoxTilesetL1.Invalidate();
            pictureBoxTilesetL2.Invalidate();
            pictureBoxTilesetL3.Invalidate();
        }
        private void replaceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            label102.ForeColor = Color.Black;
            label102.BackColor = Color.Orange;
            label102.Text = "SELECT TILE # TO REPLACE IN TILEMAP...";

            replaceChoose = true;
        }
        private void selectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            buttonEditSelect.Checked = true;
            buttonEditSelect_Click(null, null);
            overlay.DragStart = new Point(0, 0);
            if (currentLevel > 2)
            {
                overlay.SelectionSize = LayerSize(tabControl2.SelectedIndex);
                overlay.DragStop = new Point(overlay.SelectionSize.Width, overlay.SelectionSize.Height);
            }
            else
            {
                overlay.SelectionSize = new Size(4096, 4096);
                overlay.DragStop = new Point(4096, 4096);
            }

            pictureBoxLevel.Invalidate();
        }
        private void cartesianGridToolStripMenuItem_Click(object sender, EventArgs e)
        {
            buttonToggleCartGrid.Checked = cartesianGridToolStripMenuItem.Checked;
            buttonToggleCartGrid_Click(null, null);
        }
        private void maskToolStripMenuItem_Click(object sender, EventArgs e)
        {
            buttonToggleMask.Checked = maskToolStripMenuItem.Checked;
            buttonToggleMask_Click(null, null);
        }
        private void battleZonesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            buttonToggleZoning.Checked = battleZonesToolStripMenuItem.Checked;
            buttonToggleZoning_Click(null, null);
        }
        private void layer1ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            buttonToggleL1.Checked = layer1ToolStripMenuItem.Checked;
            buttonToggleL1_Click(null, null);
        }
        private void layer2ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            buttonToggleL2.Checked = layer2ToolStripMenuItem.Checked;
            buttonToggleL2_Click(null, null);
        }
        private void layer3ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            buttonToggleL3.Checked = layer3ToolStripMenuItem.Checked;
            buttonToggleL3_Click(null, null);
        }
        private void priority1ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            buttonToggleP1.Checked = priority1ToolStripMenuItem.Checked;
            buttonToggleP1_Click(null, null);
        }
        private void backgroundToolStripMenuItem_Click(object sender, EventArgs e)
        {
            buttonToggleBG.Checked = backgroundToolStripMenuItem.Checked;
            buttonToggleBG_Click(null, null);
        }
        private void physicalMapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            buttonTogglePhys.Checked = physicalMapToolStripMenuItem.Checked;
            buttonTogglePhys_Click(null, null);
        }
        private void npcsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            buttonToggleNPCs.Checked = npcsToolStripMenuItem.Checked;
            buttonToggleNPCs_Click(null, null);
        }
        private void exitFieldsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            buttonToggleExits.Checked = exitFieldsToolStripMenuItem.Checked;
            buttonToggleExits_Click(null, null);
        }
        private void eventFieldsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            buttonToggleEvents.Checked = eventFieldsToolStripMenuItem.Checked;
            buttonToggleEvents_Click(null, null);
        }
        private void treasuresToolStripMenuItem_Click(object sender, EventArgs e)
        {
            buttonToggleTreasures.Checked = treasuresToolStripMenuItem.Checked;
            buttonToggleTreasures_Click(null, null);
        }

        private void preview_Click(object sender, EventArgs e)
        {
            Previewer.Previewer lp = new FF3LE.Previewer.Previewer(model, (int)this.levelNum.Value, 1);
            lp.Show();
        }

        private void selectInTilesetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            pictureBoxLevel_MouseDoubleClick(null, null);
        }
        private void saveImageAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "PNG files (*.PNG)|*.PNG|All files (*.*)|*.*";
            saveFileDialog.FilterIndex = 1;
            saveFileDialog.FileName = "level." + currentLevel.ToString("X3") + ".png";
            saveFileDialog.RestoreDirectory = true;
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
                levelImage.Save(saveFileDialog.FileName, System.Drawing.Imaging.ImageFormat.Png);
        }

        private void pictureBoxLevel_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (buttonZoomIn.Checked || buttonZoomOut.Checked) return;

            int tile, tilex, tiley;

            if (currentLevel > 2)
                tile = tileMap.GetTileNum(tabControl2.SelectedIndex, mouse.X, mouse.Y);
            else
                tile = wmTileMap.GetTileNum(mouse.X, mouse.Y);
            tilex = (tile % 16);
            tiley = (tile / 16);

            MouseEventArgs m = new MouseEventArgs(MouseButtons.Left, 1, tilex * 16, tiley * 16, 0);

            switch (tabControl2.SelectedIndex) // Set the image to the correct pictureBox
            {
                case 0:
                    pictureBoxTilesetL1_MouseDown(null, m);
                    pictureBoxTilesetL1_MouseUp(null, null);
                    break;
                case 1:
                    pictureBoxTilesetL2_MouseDown(null, m);
                    pictureBoxTilesetL2_MouseUp(null, null);
                    break;
                case 2:
                    pictureBoxTilesetL3_MouseDown(null, m);
                    pictureBoxTilesetL3_MouseUp(null, null);
                    break;
                default: break;
            }
        }
        private void pictureBoxLevel_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Clicks > 1) return;

            // ZOOMING

            Point p = new Point();

            p.X = Math.Abs(panelLevelPicture.AutoScrollPosition.X);
            p.Y = Math.Abs(panelLevelPicture.AutoScrollPosition.Y);

            if ((buttonZoomIn.Checked && e.Button == MouseButtons.Left) || (buttonZoomOut.Checked && e.Button == MouseButtons.Right))
            {
                if ((currentLevel > 1 && zoom < 4) || (currentLevel <= 1 && zoom < 2))
                {
                    zoom *= 2;

                    p = new Point(Math.Abs(pictureBoxLevel.Left), Math.Abs(pictureBoxLevel.Top));
                    p.X += e.X;
                    p.Y += e.Y;

                    if (currentLevel > 1)
                    {
                        pictureBoxLevel.Width = (int)(2048 * zoom);
                        pictureBoxLevel.Height = (int)(2048 * zoom);
                    }
                    else
                    {
                        pictureBoxLevel.Width = (int)(4096 * zoom);
                        pictureBoxLevel.Height = (int)(4096 * zoom);
                    }
                    pictureBoxLevel.Focus();
                    panelLevelPicture.AutoScrollPosition = p;
                    panelLevelPicture.VerticalScroll.SmallChange *= 2;
                    panelLevelPicture.HorizontalScroll.SmallChange *= 2;
                    panelLevelPicture.VerticalScroll.LargeChange *= 2;
                    panelLevelPicture.HorizontalScroll.LargeChange *= 2;
                    pictureBoxLevel.Invalidate();
                    return;
                }
                return;
            }
            else if ((buttonZoomOut.Checked && e.Button == MouseButtons.Left) || (buttonZoomIn.Checked && e.Button == MouseButtons.Right))
            {
                if (zoom > 0.5)
                {
                    zoom /= 2;

                    p = new Point(Math.Abs(pictureBoxLevel.Left), Math.Abs(pictureBoxLevel.Top));
                    p.X -= e.X / 2;
                    p.Y -= e.Y / 2;

                    if (currentLevel > 1)
                    {
                        pictureBoxLevel.Width = (int)(2048 * zoom);
                        pictureBoxLevel.Height = (int)(2048 * zoom);
                    }
                    else
                    {
                        pictureBoxLevel.Width = (int)(4096 * zoom);
                        pictureBoxLevel.Height = (int)(4096 * zoom);
                    }
                    pictureBoxLevel.Focus();
                    panelLevelPicture.AutoScrollPosition = p;
                    panelLevelPicture.VerticalScroll.SmallChange /= 2;
                    panelLevelPicture.HorizontalScroll.SmallChange /= 2;
                    panelLevelPicture.VerticalScroll.LargeChange /= 2;
                    panelLevelPicture.HorizontalScroll.LargeChange /= 2;
                    pictureBoxLevel.Invalidate();
                    return;
                }
                return;
            }

            if (e.Button == MouseButtons.Right) return;

            // DRAWING, ERASING, SELECTING

            int x = e.X / (int)(16 * zoom) * (int)(16 * zoom); x = (int)(x / zoom);
            int y = e.Y / (int)(16 * zoom) * (int)(16 * zoom); y = (int)(y / zoom);

            if (state.Move && !insideSelection)
            {
                Paste(new Point(overlay.DragStart.X, overlay.DragStart.Y));

                copyPaste = copyPasteBuf;
                drawBuf = drawBufBuf;
                drawBufWidth = drawBufWidthBuf;

                state.Move = false;
            }
            if (state.Select)
            {
                if (!insideSelection)   // if we're not inside a current selection to move it, create a new selection image
                {
                    overlay.DragStart = new Point(x, y);
                    overlay.DragStop = new Point(x + 16, y + 16);
                    overlay.SelectionSize = new Size(16, 16);
                }
                else if (insideSelection)
                {
                    moveDiff = new Size(x - overlay.DragStart.X, y - overlay.DragStart.Y);

                    if (!state.Move)    // only do this if the current selection has not been initially moved
                    {
                        state.Move = true;

                        Size s = new Size(overlay.DragStop.X - overlay.DragStart.X, overlay.DragStop.Y - overlay.DragStart.Y);
                        overlay.SelectionSize = s;

                        Cut();
                    }
                }
            }
            MouseEventArgs m = new MouseEventArgs(e.Button, e.Clicks, (int)(e.X / zoom), (int)(e.Y / zoom), e.Delta);
            if (e.Button == MouseButtons.Left)
            {
                if (state.Draw)
                {
                    MakeEditDraw(m, pictureBoxLevel.CreateGraphics());

                    panelLevelPicture.AutoScrollPosition = p;
                    return;
                }
                if (state.Erase)
                {
                    MakeEditErase(m, pictureBoxLevel.CreateGraphics());

                    panelLevelPicture.AutoScrollPosition = p;
                    return;
                }
            }

            // OBJECT SELECTING

            if (!state.Draw && !state.Select && !state.Erase && e.Button == MouseButtons.Left)
            {
                if (state.Objects && overNPC != 0)
                {
                    clickNPC = overNPC;
                    npcListBox.SelectedIndex = clickNPC - 1;
                }
                if (state.Exits && overNPC == 0)
                {
                    if (overExitShort != 0)
                    {
                        clickExitShort = overExitShort;
                        exitShortListBox.SelectedIndex = clickExitShort - 1;
                    }
                    else if (overExitLong != 0)
                    {
                        clickExitLong = overExitLong;
                        exitLongListBox.SelectedIndex = clickExitLong - 1;
                    }
                }
                if (state.Events && overNPC == 0 && overExitShort == 0 && overExitLong == 0)
                {
                    if (overEvent != 0)
                    {
                        clickEvent = overEvent;
                        eventListBox.SelectedIndex = clickEvent - 1;
                    }
                }
                if (state.Treasures && overNPC == 0 && overExitShort == 0 && overExitLong == 0 && overEvent == 0)
                {
                    if (overTreasure != 0)
                    {
                        clickTreasure = overTreasure;
                        treasureListBox.SelectedIndex = clickTreasure - 1;
                    }
                }
            }

            panelLevelPicture.AutoScrollPosition = p;

            pictureBoxLevel.Invalidate();
        }
        private void pictureBoxLevel_MouseMove(object sender, MouseEventArgs e)
        {
            MouseEventArgs m = new MouseEventArgs(e.Button, e.Clicks, (int)(e.X / zoom), (int)(e.Y / zoom), e.Delta);
            int x = e.X / (int)(16 * zoom) * (int)(16 * zoom); x = (int)(x / zoom);
            int y = e.Y / (int)(16 * zoom) * (int)(16 * zoom); y = (int)(y / zoom);

            UpdateCoordLabels(e);

            Point tile = new Point();
            tile.X = Math.Max(Math.Min((int)((e.X / 16) / zoom), 255), 0);
            tile.Y = Math.Max(Math.Min((int)((e.Y / 16) / zoom), 255), 0);

            insideSelection = false;

            if (buttonZoomIn.Checked || buttonZoomOut.Checked)
            {
                pictureBoxLevel.Invalidate();
                return;
            }

            // DRAWING, ERASING, SELECTING

            if (state.Draw && e.Button == MouseButtons.Left)
            {
                MakeEditDraw(m, pictureBoxLevel.CreateGraphics());
                return;
            }
            else if (state.Select)
            {
                if (!state.Move)
                {
                    if (overlay.DragStop == new Point(x, y)) return;

                    if (e.Button == MouseButtons.Left)
                    {
                        overlay.DragStop = new Point(x + 16, y + 16);
                        overlay.SelectionSize = new Size(overlay.DragStop.X - overlay.DragStart.X, overlay.DragStop.Y - overlay.DragStart.Y);
                    }
                    pictureBoxLevel.Invalidate();
                }
                else
                {
                    if (e.Button == MouseButtons.Left)
                    {
                        overlay.DragStart = new Point(x - moveDiff.Width, y - moveDiff.Height);
                        overlay.DragStop = new Point(overlay.DragStart.X + overlay.SelectionSize.Width, overlay.DragStart.Y + overlay.SelectionSize.Height);
                    }
                    pictureBoxLevel.Invalidate();
                }
                if ((e.Button == MouseButtons.None || state.Move) &&
                    overlay.DragStart != overlay.DragStop &&
                    x >= overlay.DragStart.X &&
                    x < overlay.DragStop.X &&
                    y >= overlay.DragStart.Y &&
                    y < overlay.DragStop.Y)
                {
                    insideSelection = true;
                    pictureBoxLevel.Cursor = Cursors.SizeAll;
                }
                else
                {
                    pictureBoxLevel.Cursor = Cursors.Cross;
                }
                return;
            }
            else if (state.Erase && e.Button == MouseButtons.Left)
            {
                MakeEditErase(m, pictureBoxLevel.CreateGraphics());
                return;
            }

            // OBJECT MOVING

            if (!state.Draw && !state.Select && !state.Erase && e.Button == MouseButtons.Left)
            {
                if (clickNPC != 0)
                {
                    tile.X = Math.Max(Math.Min(tile.X, 127), 0);
                    tile.Y = Math.Max(Math.Min(tile.Y, 63), 0);
                    waitBothCoords = npcXCoord.Value != tile.X && npcYCoord.Value != tile.Y + coordDelta.Y;
                    npcXCoord.Value = tile.X;
                    waitBothCoords = false;
                    npcYCoord.Value = tile.Y + coordDelta.Y;
                }
                else if (clickExitShort != 0)
                {
                    waitBothCoords = exitShortXCoord.Value != tile.X && exitShortYCoord.Value != tile.Y;
                    exitShortXCoord.Value = tile.X;
                    waitBothCoords = false;
                    exitShortYCoord.Value = tile.Y;
                }
                else if (clickExitLong != 0)
                {
                    if (exits.DirectionLong == 0)
                    {
                        waitBothCoords = exitLongXCoord.Value != tile.X - coordDelta.X && exitLongYCoord.Value != tile.Y;
                        exitLongXCoord.Value = Math.Max(tile.X - coordDelta.X, 0);
                        waitBothCoords = false;
                        exitLongYCoord.Value = tile.Y;
                    }
                    else
                    {
                        waitBothCoords = exitLongXCoord.Value != tile.X && exitLongYCoord.Value != tile.Y - coordDelta.Y;
                        exitLongXCoord.Value = tile.X;
                        waitBothCoords = false;
                        exitLongYCoord.Value = Math.Max(tile.Y - coordDelta.Y, 0);
                    }
                }
                else if (clickEvent != 0)
                {
                    waitBothCoords = eventXCoord.Value != tile.X && eventYCoord.Value != tile.Y;
                    eventXCoord.Value = tile.X;
                    waitBothCoords = false;
                    eventYCoord.Value = tile.Y;
                }
                else if (clickTreasure != 0)
                {
                    waitBothCoords = treasureXCoord.Value != tile.X && treasureYCoord.Value != tile.Y;
                    treasureXCoord.Value = tile.X;
                    waitBothCoords = false;
                    treasureYCoord.Value = tile.Y;
                }

                return;
            }

            int counter = 0;
            if (!state.Draw && !state.Select && !state.Erase)
                pictureBoxLevel.Cursor = Cursors.Arrow;
            if (!state.Draw && !state.Select && !state.Erase && state.Objects)
            {
                foreach (LevelNPCs.NPC npc in npcs.NPCs)
                {
                    if (npc.CoordX == this.tile.X &&
                        (npc.CoordY == this.tile.Y || npc.CoordY - 1 == this.tile.Y))
                    {
                        coordDelta = new Point(0, npc.CoordY - tile.Y);
                        overNPC = counter + 1;
                        pictureBoxLevel.Cursor = Cursors.Hand;
                        isOverSomething = true;
                        break;
                    }
                    else
                    {
                        overNPC = 0;
                        pictureBoxLevel.Cursor = Cursors.Arrow;
                        isOverSomething = false;
                    }
                    counter++;
                }
            }
            if (!state.Draw && !state.Select && !state.Erase && state.Exits && !isOverSomething)
            {
                counter = 0;
                foreach (LevelExits.ExitShort exs in exits.ExitsShort)
                {
                    if (exs.CoordX == this.tile.X && exs.CoordY == this.tile.Y)
                    {
                        overExitShort = counter + 1;
                        pictureBoxLevel.Cursor = Cursors.Hand;
                        isOverSomething = true;
                        break;
                    }
                    else
                    {
                        overExitShort = 0;
                        pictureBoxLevel.Cursor = Cursors.Arrow;
                        isOverSomething = false;
                    }
                    counter++;
                }
                if (!isOverSomething)
                {
                    counter = 0;
                    foreach (LevelExits.ExitLong exl in exits.ExitsLong)
                    {
                        if (exl.Direction == 0 && exl.CoordY == this.tile.Y)
                        {
                            if (exl.CoordX == this.tile.X ||
                                (this.tile.X >= exl.CoordX && this.tile.X <= exl.CoordX + exl.Width))
                            {
                                coordDelta = new Point(tile.X - exl.CoordX, 0);
                                overExitLong = counter + 1;
                                pictureBoxLevel.Cursor = Cursors.Hand;
                                isOverSomething = true;
                                break;
                            }
                        }
                        else if (exl.Direction == 1 && exl.CoordX == this.tile.X)
                        {
                            if (exl.CoordY == this.tile.Y ||
                                (this.tile.Y >= exl.CoordY && this.tile.Y <= exl.CoordY + exl.Width))
                            {
                                coordDelta = new Point(0, tile.Y - exl.CoordY);
                                overExitLong = counter + 1;
                                pictureBoxLevel.Cursor = Cursors.Hand;
                                isOverSomething = true;
                                break;
                            }
                        }
                        else
                        {
                            overExitLong = 0;
                            pictureBoxLevel.Cursor = Cursors.Arrow;
                            isOverSomething = false;
                        }
                        counter++;
                    }
                }
            }
            if (!state.Draw && !state.Select && !state.Erase && state.Events && !isOverSomething)
            {
                counter = 0;
                foreach (LevelEvents.Event theEvent in events.Events)
                {
                    if (theEvent.CoordX == this.tile.X && theEvent.CoordY == this.tile.Y)
                    {
                        overEvent = counter + 1;
                        pictureBoxLevel.Cursor = Cursors.Hand;
                        isOverSomething = true;
                        break;
                    }
                    else
                    {
                        overEvent = 0;
                        pictureBoxLevel.Cursor = Cursors.Arrow;
                        isOverSomething = false;
                    }
                    counter++;
                }
            }
            if (!state.Draw && !state.Select && !state.Erase && state.Treasures && !isOverSomething)
            {
                counter = 0;
                foreach (LevelNPCs.Treasure theTreasure in npcs.Treasures)
                {
                    if (theTreasure.CoordX == this.tile.X && theTreasure.CoordY == this.tile.Y)
                    {
                        overTreasure = counter + 1;
                        pictureBoxLevel.Cursor = Cursors.Hand;
                        isOverSomething = true;
                        break;
                    }
                    else
                    {
                        overTreasure = 0;
                        pictureBoxLevel.Cursor = Cursors.Arrow;
                        isOverSomething = false;
                    }
                    counter++;
                }
            }

            pictureBoxLevel.Invalidate();

            isOverSomething = false;
        }
        private void pictureBoxLevel_MouseUp(object sender, MouseEventArgs e)
        {
            clickNPC = 0;
            clickExitShort = 0;
            clickExitLong = 0;
            clickEvent = 0;
            clickTreasure = 0;

            if (state.Draw || state.Erase)
                SetLevelImage();
        }
        private void pictureBoxLevel_MouseEnter(object sender, EventArgs e)
        {
            mouseOver = true;
            pictureBoxLevel.Invalidate();
        }
        private void pictureBoxLevel_MouseLeave(object sender, EventArgs e)
        {
            mouseOver = false;
            pictureBoxLevel.Invalidate();
        }
        private void pictureBoxLevel_Paint(object sender, PaintEventArgs e)
        {
            RectangleF clone = e.ClipRectangle;
            SizeF remainder = new SizeF((int)(clone.Width % zoom), (int)(clone.Height % zoom));
            clone.Location = new PointF((int)(clone.X / zoom), (int)(clone.Y / zoom));
            clone.Size = new SizeF((int)(clone.Width / zoom), (int)(clone.Height / zoom));
            clone.Width += (int)(remainder.Width * zoom) + 1;
            clone.Height += (int)(remainder.Height * zoom) + 1;
            RectangleF source, dest;

            e.Graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.Half;
            e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;

            if (currentLevel > 2 && state.BG)
                e.Graphics.FillRectangle(new SolidBrush(Color.Black), e.ClipRectangle);

            if (levelImage != null)
            {
                clone.Width = Math.Min(levelImage.Width, clone.X + clone.Width) - clone.X;
                clone.Height = Math.Min(levelImage.Height, clone.Y + clone.Height) - clone.Y;

                source = clone; source.Location = new PointF(0, 0);
                dest = new RectangleF((int)(clone.X * zoom), (int)(clone.Y * zoom), (int)(clone.Width * zoom), (int)(clone.Height * zoom));

                e.Graphics.DrawImage(levelImage.Clone(clone, PixelFormat.DontCare), dest, source, GraphicsUnit.Pixel);
            }

            if (state.Move)
                MakeEditMove(e.Graphics);

            if (state.PhysicalLayer && state.Layer1)
            {
                if (tileMap != null && currentLevel > 2)
                    overlay.DrawPhysicalMap(e.Graphics, layer, tileMap, zoom);
                else if (wmTileMap != null)
                    overlay.DrawPhysicalMap(e.Graphics, wmTileMap, zoom);
            }

            if (state.CartesianGrid)
                overlay.DrawCartographicGrid(e.Graphics, Color.Gray, pictureBoxLevel.Size, new Size(16, 16), zoom);

            if (state.Mask && currentLevel > 2)
                overlay.DrawLevelMask(e.Graphics, new Point(layer.MaskHighX, layer.MaskHighY), new Point(0, 0), zoom);

            if (buttonToggleBounds.Checked && currentLevel > 2)
                overlay.DrawVisibleBounds(e.Graphics, tile, new Point(layer.MaskHighX, layer.MaskHighY), zoom);
            else if (buttonToggleBounds.Checked)
                overlay.DrawVisibleBounds(e.Graphics, tile, new Point(256, 256), zoom);

            if (state.Zones && currentLevel < 2)
                overlay.DrawZoneGrid(e.Graphics, Color.White, pictureBoxLevel.Size, new Size(512, 512), zoom);

            if (state.Exits)
                overlay.DrawLevelExits(e.Graphics, exits, zoom);

            if (state.Events)
                overlay.DrawLevelEvents(e.Graphics, events, zoom);

            if (state.Objects && currentLevel > 2)
                overlay.DrawLevelNPCs(e.Graphics, npcs, zoom);

            if (state.Treasures && currentLevel > 2)
                overlay.DrawLevelTreasures(e.Graphics, npcs, zoom);

            if (state.Select)
                overlay.DrawSelectionBox(e.Graphics, overlay.DragStop, new Point(overlay.DragStart.X + 1, overlay.DragStart.Y + 1), zoom);

            if (mouseOver)
                overlay.DrawHoverBox(e.Graphics, new Point(this.tile.X * 16, this.tile.Y * 16), zoom);
        }
        private void panelLevelPicture_Scroll(object sender, ScrollEventArgs e)
        {
            pictureBoxLevel.Invalidate();
        }

        private void UpdateCoordLabels(MouseEventArgs e)
        {
            int x = (int)(e.X / zoom);
            int y = (int)(e.Y / zoom);
            mouse.X = x;
            mouse.Y = y;
            int tx = x / 16;
            int ty = y / 16;
            tile.X = tx;
            tile.Y = ty;

            this.labelPixelCoords.Text = "(" + x + ", " + y + ")  Pixel Coord";
            this.labelTileCoords.Text = "(" + tx + ", " + ty + ")  Tile Coord";
        }

        private void searchLevelNames_Click(object sender, EventArgs e)
        {
            panelSearchLevelNames.Visible = !panelSearchLevelNames.Visible;
            if (panelSearchLevelNames.Visible)
                nameTextBox.Focus();
        }
        private void searchButton_Click(object sender, EventArgs e)
        {
            LoadSearch();
        }
        private void listBoxLevelNames_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                levelName.SelectedItem = listBoxLevelNames.SelectedItem;
            }
            catch
            {
                MessageBox.Show("There was a problem loading the search item. Try doing another search.");
            }
        }
        private void nameTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Escape)
                panelSearchLevelNames.Visible = false;
        }
        private void nameTextBox_TextChanged(object sender, EventArgs e)
        {
            LoadSearch();
        }
        private void listBoxLevelNames_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Escape)
                panelSearchLevelNames.Visible = false;
        }
        private void searchButton_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Escape)
                panelSearchLevelNames.Visible = false;
        }

        private void tabControl1_Selecting(object sender, TabControlCancelEventArgs e)
        {
            if (currentLevel < 3 && (e.TabPageIndex == 0 || e.TabPageIndex == 2))
                e.Cancel = true;
        }
        private void tabControl2_SelectedIndexChanged(object sender, EventArgs e)
        {
            tileSet.RedrawTilesets(tabControl2.SelectedIndex);

            tileSetPixels = tileSet.GetTilesetPixelArray(tileSet.TileSetLayers[tabControl2.SelectedIndex]);
            tileSetImage = DrawImageFromIntArr(tileSetPixels, 256, 256);

            switch (tabControl2.SelectedIndex)
            {
                case 0:
                    panel57.Visible = true;
                    InitializeTile();
                    pictureBoxTilesetL1.Invalidate(); break;
                case 1:
                    tabControl3.SelectedIndex = 0;
                    panel57.Visible = true;
                    InitializeTile();
                    pictureBoxTilesetL2.Invalidate(); break;
                case 2:
                    tabControl3.SelectedIndex = 0;
                    panel57.Visible = false;
                    pictureBoxTilesetL3.Invalidate(); break;
                default: break;
            }

            if (tabControl2.SelectedIndex < 3)
                this.label26.Text = "EDITING: LAYER " + (tabControl2.SelectedIndex + 1).ToString();
            else
                this.label26.Text = "EDITING: <NONE>";
        }
        private void tabControl2_Selecting(object sender, TabControlCancelEventArgs e)
        {
            if (currentLevel < 3 && e.TabPageIndex != 0)
                e.Cancel = true;
        }
        private void tabControl3_Selecting(object sender, TabControlCancelEventArgs e)
        {
            if ((currentLevel == 2 || tabControl2.SelectedIndex != 0) && e.TabPageIndex == 1)
                e.Cancel = true;
        }

        private void Levels_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Escape)
            {
                if (replaceChoose || replaceSet)
                    ResetTileReplace();
            }
        }

        
        //madsiur
        private void tbLocationName_MouseLeave(object sender, EventArgs e)
        {
            SetLocationName();
        }

        private void tbLocationName_Leave(object sender, EventArgs e)
        {
            SetLocationName();
        }

        private void SetLocationName()
        {
            if (Model.IsExpanded)
            {
                string name = tbLocationName.Text.Trim();

                if (!levelNames[currentLevel].Trim().Equals(name))
                {
                    levelNames[currentLevel] = Bits.AddMapId(currentLevel, name);

                    if (Model.Serialized(levelNames))
                    {
                        levelName.Items.Clear();
                        levelNames = GetLevelNames();
                        levelName.Items.AddRange(levelNames);
                        levelName.SelectedIndex = (int)levelNum.Value < levelNames.Length ? (int)levelNum.Value : 0;
                        isLevelNameChanged = true;
                    }
                }
            }
        }

        private void tbMessageName_MouseLeave(object sender, EventArgs e)
        {
            ValidateMessageName();
        }

        private void tbMessageName_Leave(object sender, EventArgs e)
        {
            ValidateMessageName();
        }

        private void ValidateMessageName()
        {
            string mess = tbMessageName.Text;
            //MatchCollection matches = Bits.GetMatchCollection(mess);

            if (mess.Length <= 37)
            {
                if (!Bits.IsValidMapName(mess))
                {
                    MessageBox.Show("Invalid Message name: \"" + tbMessageName.Text + "\". For allowed characters, see readme.", "FF6LE",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    tbMessageName.Text = messageNames[messageName.SelectedIndex];
                    tbMessageName.Focus();
                }
                else
                {
                    messageNames[messageName.SelectedIndex] = mess;
                    this.messageName.Items.Clear();
                    this.messageName.Items.AddRange(messageNames);
                    this.messageName.SelectedIndex = (int)this.message.Value;
                }
            }
            else
            {
                MessageBox.Show("Invalid Message Name length. Message Name byte count must be inferior or equal to 37.",
                    "FF6LE",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                tbMessageName.Text = messageNames[messageName.SelectedIndex];
                tbMessageName.Focus();
            }
        }

        private void levelName_MouseEnter(object sender, EventArgs e)
        {
            if (isLevelNameChanged)
            {
                isLevelNameChanged = false;
                levelName.Items.Clear();
                levelNames = GetLevelNames();
                levelName.Items.AddRange(levelNames);
                levelName.SelectedIndex = (int)levelNum.Value < levelNames.Length ? (int)levelNum.Value : 0;
            }
        }

        private void Levels_FormClosed(object sender, FormClosedEventArgs e)
        {
            state.ResetAll();
        }
        private void Levels_FormClosing(object sender, FormClosingEventArgs e)
        {
            //ExportLevelImages.CancelAsync(); // if exporting images, cancel when form closed

            state = State.Instance;

            DialogResult result;

            if (model.AssembleLevels)
            {
                result = MessageBox.Show("Would you like to save changes?", "Save and quit Level Editor?", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                    saveToolStripMenuItem_Click(null, null);
                else if (result == DialogResult.Cancel)
                {
                    e.Cancel = true;
                    return;
                }

            }
            model.AssembleLevels = false;

            GC.Collect();
        }

        #endregion
    }
}
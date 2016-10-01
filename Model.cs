using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms; // remove later
using System.IO;
using System.Security.Cryptography;
using System.Globalization;
using System.Runtime.Serialization.Formatters.Binary;
using FF3LE.Properties;
using System.Collections.Specialized;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace FF3LE
{

    public class Model
    {
        #region CE expansion variables

        // CE Expansions
        public static bool IsExpanded;
        public static bool IsChestsExpanded;

        // Settings file
        public static XDocument SettingsFile;

        // Map names
        public static string[] LevelNames;

        // Number of entries
        public static int NUM_LOCATIONS;
        public static int NUM_TILEMAPS;
        public static int NUM_LOC_NAMES;

        // Size of data
        public static int SIZE_EVENT_PTR;
        public static int SIZE_EVENT_DATA;
        public static int SIZE_NPC_PTR;
        public static int SIZE_NPC_DATA;
        public static int SIZE_SHORT_EXIT_PTR;
        public static int SIZE_SHORT_EXIT_DATA;
        public static int SIZE_LONG_EXIT_PTR;
        public static int SIZE_LONG_EXIT_DATA;
        public static int SIZE_CHEST_PTR;
        public static int SIZE_CHEST_DATA;
        public static int SIZE_TILEMAP_PTR;
        public static int SIZE_TILEMAP_DATA;
        public static int SIZE_LOCATION;
        public static int SIZE_LOC_NAMES;
        public static int SIZE_LOC_NAMES_PTR;

        // ROM Offsets
        public static int BASE_EVENT_PTR;
        public static int BASE_EVENT;
        public static int BASE_NPC_PTR;
        public static int BASE_NPC;
        public static int BASE_SHORT_EXIT_PTR;
        public static int BASE_SHORT_EXIT;
        public static int BASE_LONG_EXIT_PTR;
        public static int BASE_LONG_EXIT;
        public static int BASE_CHEST_PTR;
        public static int BASE_CHEST;
        public static int BASE_TILEMAP_PTR;
        public static int BASE_TILEMAP;
        public static int BASE_LOCATION;
        public static int BASE_LOC_NAMES_PTR;
        public static int BASE_LOC_NAMES;

        #endregion

        // LJ 2011-12-28: interoperability fix for FF3usME
        public readonly int OFFS_MAP_GFXPIX_PTR = 0x2EB200;      // offset for headerless ROM
        public readonly int OFFS_WOB_MAP_DT_TL = 0x2ED434;		// at 0x2ED634 we have 15643 bytes for data, at 0x2F134F we have 8449 bytes for tiles, end at 0x2F3450
        public readonly int OFFS_WOR_MAP_DT_TL = 0x2F4A46;		// at 0x2F4C46 we have 12993 bytes for data, at 0x2F7F07 we have 8208 bytes for tiles, end at 0x2F9F17
        public readonly int OFFS_ST_MAP_DT_TL = 0x2F9E17;		// at 0x2F9F17 we have 6426 bytes for data, at 0x2FB831 we have 4083 bytes for tiles, end at 0x2FC824
        public readonly int OFFS_WOB_WOR_MMAP = 0x2FE49B;		// at 0x2FE69B we have 1048 bytes for WoB mini-map, at 0x2FEAB3 we have 1139 bytes for WoR mini-map, end at 2FEF26
        public readonly int OFFS_WOB_MMAP = 0x2FE49B;		    // at 0x2FE69B we have 1048 bytes for WoB mini-map end at 0x2FEAB3
        public readonly int OFFS_WOR_MMAP = 0x2FE8B3;		    // at 0x2FEAB3 we have 1139 bytes for WoR mini-map, end at 2FEF26

        public readonly int LEN_MAP_GFXPIX_PTR = 0x60;			// byte wize, 3 * NB_VAR_COMP_PTR
        public readonly int LEN_WOB_MAP_DT_TL = (15643+8449);	// at 0x2ED634 we have 15643 bytes for data, at 0x2F134F we have 8449 bytes for tiles
        public readonly int LEN_WOR_MAP_DT_TL = (8208 + 12993);	// at 0x2F7F07 we have 8208 bytes for tiles, at 0x2F4C46 we have 12993 bytes for data
        public readonly int LEN_ST_MAP_DT_TL = (6426 + 4083);	// at 0x2F9F17 we have 6426 bytes for data, at 0x2FB831 we have 4083 bytes for tiles
        public readonly int LEN_WOB_WOR_MMAP = (1048 + 1139);   // at 0x2FE69B we have 1048 bytes for WoB mini-map, at 0x2FEAB3 we have 1139 bytes for WoR mini-map
        public const int LEN_MAX_MMAP_DATA = 0x800;			    // decompressed mini maps are 0x800 bytes long
        public const int NB_VAR_COMP_PTR = 0x20;			    // 0x20 times 24-bit pointers to compressed stuff like mini-maps, maps, tiles...
        public readonly int NB_MMAPS = 2;				        // nb. mini-maps
        public readonly int NB_REGMAPS = 2;				        // nb. regular maps
        public readonly int NB_MAP_TILE_BANKS = 2;				// nb. maps tile banks
        public readonly int IDX_VARP_TILE_ASHP1 = 0x04;			// Various abs. pointer to compressed data: tiles for Black Jack
        public readonly int IDX_VARP_MAP_WOB = 0x05;			// Various abs. pointer to compressed data: WoB map data
        public readonly int IDX_VARP_TILE_WOB = 0x06;			// Various abs. pointer to compressed data: WoB tile data
        public readonly int IDX_VARP_TILE_WOR = 0x0A;			// Various abs. pointer to compressed data: WoR tile data
        public readonly int IDX_VARP_MAP_WOR = 0x0B;			// Various abs. pointer to compressed data: WoR map data
        public readonly int IDX_VARP_MAP_WOR2 = 0x0C;			// Various abs. pointer to compressed data: WoR map data (duplicate)
        public readonly int IDX_VARP_MAP_ANIM = 0x17;			// Various abs. pointer to compressed data: tiles for map animations (Figaro castle, Terra flying, sand dust, Narshe boat, ...)
        public readonly int IDX_VARP_TILE_CHOC = 0x18;			// Various abs. pointer to compressed data: tiles for Chocobo
        public readonly int IDX_VARP_MMAP_WOB = 0x19;			// Various abs. pointer to compressed data: WoB mini-map data
        public readonly int IDX_VARP_MMAP_WOR = 0x1A;			// Various abs. pointer to compressed data: WoR mini-map data
        public readonly int IDX_VARP_TILE_ASHP2 = 0x1B;			// Various abs. pointer to compressed data: tiles for Falcon
        public ulong[] m_varCompDataAbsPtrs = new ulong[NB_VAR_COMP_PTR];	// various absolute pointer redirection to compressed stuff like mini-maps, maps, tiles...
        private byte[] wobMiniMap = new byte[LEN_MAX_MMAP_DATA]; public byte[] WobMiniMap { get { return wobMiniMap; } set { wobMiniMap = value; } }
        private byte[] worMiniMap = new byte[LEN_MAX_MMAP_DATA]; public byte[] WorMiniMap { get { return worMiniMap; } set { worMiniMap = value; } }

        // LJ 2011-12-28: atm, all things dialog are not (and should not) be handled with FF3LE, consequence of this will be to put the expanded data in bank $F1, not following the expanded dialog in $F0 like FF3usME and FF3Ed
        public int m_savedExpandedBytes = 0;	                // running counter for all the things that couldn't fit in original bound (dialog, maps, ...) (useless for FF3LE)
//        public readonly int OFFS_FF3ED_DTE_D_EX = 0x300000;		// FF3Edit exclusive 3rd Town Dialog page data location
        public readonly int OFFS_FF3ED_DTE_D_EX = 0x310000;		// this is bank $F1



        private byte[] data; public byte[] Data { get { return this.data; } set { this.data = value; } }
 
        private Program program; public Program Program { get { return this.program; } }

        private bool hasHeader; public bool HasHeader { get { return this.hasHeader; } set { this.hasHeader = value; } }
        private byte[] header;

        private long numBytes = 0;
        private string fileName;
        private LevelModel levelModel; public LevelModel LevelModel { get { return levelModel; } }
        private LCDecomp lcDecomp;
        private byte[] dataHash;
        private long checkSum = 0;

        private byte[][] graphicSets = new byte[82][]; public byte[][] GraphicSets { get { return graphicSets; } set { graphicSets = value; } }
        private byte[] animatedGraphics = new byte[0x8000]; public byte[] AnimatedGraphics { get { return animatedGraphics; } set { animatedGraphics = value; } }
        private byte[][] graphicSetsL3 = new byte[19][]; public byte[][] GraphicSetsL3 { get { return graphicSetsL3; } set { graphicSetsL3 = value; } }
        private byte[][] tileSets = new byte[75][]; public byte[][] TileSets { get { return tileSets; } set { tileSets = value; } }
        private byte[][] tileMaps; public byte[][] TileMaps { get { return tileMaps; } set { tileMaps = value; } }
        private byte[][] physicalMaps = new byte[43][]; public byte[][] PhysicalMaps { get { return physicalMaps; } set { physicalMaps = value; } }

        private byte[] wobGraphicSet; public byte[] WobGraphicSet { get { return wobGraphicSet; } set { wobGraphicSet = value; } }
        private byte[] wobTileMap; public byte[] WobTileMap { get { return wobTileMap; } set { wobTileMap = value; } }
        private byte[] wobPhysicalMap; public byte[] WobPhysicalMap { get { return wobPhysicalMap; } set { wobPhysicalMap = value; } }
        private byte[] worGraphicSet; public byte[] WorGraphicSet { get { return worGraphicSet; } set { worGraphicSet = value; } }
        private byte[] worTileMap; public byte[] WorTileMap { get { return worTileMap; } set { worTileMap = value; } }
        private byte[] worPhysicalMap; public byte[] WorPhysicalMap { get { return worPhysicalMap; } set { worPhysicalMap = value; } }
        private byte[] stGraphicSet; public byte[] StGraphicSet { get { return stGraphicSet; } set { stGraphicSet = value; } }
        private byte[] stTileMap; public byte[] StTileMap { get { return stTileMap; } set { stTileMap = value; } }

        private bool editWobGraphicSet; public bool EditWobGraphicSet { get { return editWobGraphicSet; } set { editWobGraphicSet = value; } }
        private bool editWobTileMap; public bool EditWobTileMap { get { return editWobTileMap; } set { editWobTileMap = value; } }
        private bool editWorGraphicSet; public bool EditWorGraphicSet { get { return editWorGraphicSet; } set { editWorGraphicSet = value; } }
        private bool editWorTileMap; public bool EditWorTileMap { get { return editWorTileMap; } set { editWorTileMap = value; } }
        private bool editStGraphicSet; public bool EditStGraphicSet { get { return editStGraphicSet; } set { editStGraphicSet = value; } }
        private bool editStTileMap; public bool EditStTileMap { get { return editStTileMap; } set { editStTileMap = value; } }
        private bool[] editPhysicalMaps = new bool[43]; public bool[] EditPhysicalMaps { get { return editPhysicalMaps; } set { editPhysicalMaps = value; } }

        private byte[] stPaletteSet; public byte[] StPaletteSet { get { return stPaletteSet; } set { stPaletteSet = value; } }

        private bool[] editTileSets = new bool[75]; public bool[] EditTileSets { get { return editTileSets; } set { editTileSets = value; } }
        private bool[] editTileMaps; public bool[] EditTileMaps { get { return editTileMaps; } set { editTileMaps = value; } }
        private ushort[] tileMapSizes; public ushort[] TileMapSizes { get { return tileMapSizes; } set { tileMapSizes = value; } }

        private bool assembleLevels = false; public bool AssembleLevels { get { return assembleLevels; } set { assembleLevels = value; } }
        private bool assembleFinal = false; public bool AssembleFinal { get { return assembleFinal; } set { assembleFinal = value; } }
        private bool locked = false; public bool Locked { get { return this.locked; } set { this.locked = value; } }

        public Model(Program program)
        {
            this.program = program;
        }

        /************************************************************************************
        * File Handling Methods
        * *********************************************************************************/
        public bool VerifyRom()
        {
            // not implemented yet!
            if (!locked) // and verified
                return true;
            return false;
        }
        public bool HeaderPresent()
        {
            return (numBytes & (long)0x200) == 0x200;
        }
        public bool RemoveHeader()
        {
            try
            {
                hasHeader = true;
                header = new byte[0x200];

                for (int i = 0; i < 0x200; i++)
                    header[i] = data[i];

                byte[] noHeader = new byte[numBytes - 0x200];

                for (int i = 0; i < numBytes - 0x200; i++)
                {
                    noHeader[i] = data[i + 0x200];

                }
                numBytes -= 0x200;
                data = noHeader;

                return true;
            }
            catch
            {
                MessageBox.Show("Error removing header, please remove manually");
                return false;
            }
        }
        public long GetFileSize()
        {
            return numBytes;
        }
        public void SetFileName(string fileName)
        {
            this.fileName = fileName;
        }
        public string GetFileName()
        {
            return fileName;
        }
        public string GetFileNameWithoutPath()
        {
            try
            {
                return fileName.Substring(fileName.LastIndexOf('\x5c') + 1);
            }
            catch
            {
                return null;
            }
        }
        public string GetFileNameWithoutPathOrExtension()
        {
            string ret = fileName.Substring(fileName.LastIndexOf('\x5c') + 1);
            return ret.Substring(0, ret.LastIndexOf('.'));
        }
        public string GetPathWithoutFileName()
        {
            return fileName.Substring(0, fileName.LastIndexOf('\x5c') + 1);
        }
        public string GetEditorPath()
        {
            return Application.ExecutablePath;
        }
        public string GetEditorPathWithoutFileName()
        {
            return GetEditorPath().Substring(0, GetEditorPath().LastIndexOf('\\') + 1);
        }
        public string GetRomName()
        {
            if (HeaderPresent())
                return ByteToStr(ByteManage.GetByteArray(data, 0x101c0, 21));
            return ByteToStr(ByteManage.GetByteArray(data, 0xffc0, 21));
        }
        public string RomChecksum()
        {
            int chunk0 = 0;
            int chunk1 = 0;
            for (int i = 0; i < data.Length; i++)
            {
                if (i < 0x200000)
                    chunk0 += data[i];
                else
                    chunk1 += data[i];
            }

            checkSum = (chunk0 + chunk1 + chunk1) & 0xFFFF;

            if ((ushort)checkSum == ByteManage.GetShort(data, 0x00FFDE))
                return "0x" + checkSum.ToString("X") + " (OK)";
            else
                return "0x" + checkSum.ToString("X") + " (FAIL)";
//                return "0x" + checkSum.ToString("X") + " (FAIL, against 0x" + ByteManage.GetShort(data, 0x00FFDE).ToString("X") + ")";
        }
        public ushort RomChecksumBin()
        {
            checkSum = 0;
            for (int i = 0; i < data.Length; i++)
                checkSum += data[i];
            checkSum &= 0xFFFF;
            return (ushort)checkSum;
        }

        /*
         * This code does not work because we essentially are 
         * modifying the data after calculating the new checksum thus changing it
         */
        public void CalculateAndSetNewRomChecksum()
        {
            int check = 0;

            for (int i = 0; i < data.Length; i++)
                check += data[i];
            check = -check;     // LJ 2013-05-25: bug fix for checksum calculation
            check &= 0xFFFF;

            ByteManage.SetShort(data, 0x00FFDE, (ushort)check);
        }
        public void CreateNewMD5Checksum()
        {
            MD5 md5Hasher = MD5.Create();

            if (data != null)
                dataHash = md5Hasher.ComputeHash(data);
        }
        public bool VerifyMD5Checksum()
        {
            MD5 md5Hasher = MD5.Create();
            byte[] hash;

            if (dataHash != null)
                hash = md5Hasher.ComputeHash(data);
            else
                return true;

            for (int i = 0; i < dataHash.Length && i < hash.Length; i++)
                if (dataHash[i] != hash[i])
                    return false;

            return true;
        }
        public string GameCode()
        {
            return ByteToStr(ByteManage.GetByteArray(data, 0xFFB2, 4));
        }
        public bool ReadRom()
        {
            try
            {
                FileInfo fInfo = new FileInfo(fileName);
                numBytes = fInfo.Length;
                FileStream fStream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                BinaryReader br = new BinaryReader(fStream);
                data = br.ReadBytes((int)numBytes);
                br.Close();
                fStream.Close();
                
                return true;

            }
            catch (Exception e)
            {
                MessageBox.Show("FF6LE was unable to load the rom :" + e.Message);

                fileName = "Invalid File";
                return false;
            }

        }
        public bool WriteRom()
        {
            try
            {
                //madsiur, set correctly reverse checksum and checksum
                SetRomChecksum();

                // if the loaded rom contained a header, use a buffer that starts the data at 0x200
                // and store the header data to the rom
                if (hasHeader && header != null)
                {
                    byte[] temp = new byte[data.Length + 0x200];
                    header.CopyTo(temp, 0);
                    data.CopyTo(temp, 0x200);
                    BinaryWriter binWriter = new BinaryWriter(File.Open(fileName, FileMode.Create));
                    binWriter.Write(temp);
                    binWriter.Close();
                    return true;
                }
                else
                {
                    BinaryWriter binWriter = new BinaryWriter(File.Open(fileName, FileMode.Create));
                    binWriter.Write(data);
                    binWriter.Close();
                    return true;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("FF6LE was unable to save the rom \n" + e.Message);
                fileName = "Invalid File";
                return false;
            }

        }

        /// <summary>
        /// madsiur, set correctly checksum and reverse checksum
        /// </summary>
        public void SetRomChecksum()
        {
            int chunk0 = 0;
            int chunk1 = 0;
            for (int i = 0; i < data.Length; i++)
            {
                if (i < 0x200000)
                    chunk0 += data[i];
                else
                    chunk1 += data[i];
            }
            checkSum = (chunk0 + chunk1 + chunk1) & 0xFFFF;
            //
            Bits.SetShort(data, 0xFFDE, (int)(checkSum & 0xFFFF));
            Bits.SetShort(data, 0xFFDC, (int)(checkSum ^ 0xFFFF));
        }
        /************************************************************************************
        * Level Editor Methods
        * *********************************************************************************/
        public void CreateLevelModel()
        {
            lcDecomp = new LCDecomp(data);
            levelModel = new LevelModel(data, this);
        }
        public byte[] Decompress(int offset, int maxSize, ref ushort totalSize)
        {
            return lcDecomp.Decompress(offset, maxSize, ref totalSize);
        }
        public byte[] Decompress(int offset, int maxSize)
        {
            return lcDecomp.Decompress(offset, maxSize);
        }
        public int Compress(byte[] source, byte[] dest)
        {
            return lcDecomp.Compress(source, dest);
        }

        //madsiur
        public int CompressT(byte[] source, byte[] dest)
        {
            return lcDecomp.CompressT(source, dest);
        }
        public LevelModel GetLevelModel() { return this.levelModel; }
        public void ClearLevelData()
        {
            for (int i = 0; i < graphicSets.Length; i++)
                graphicSets[i] = null;
            for (int i = 0; i < tileSets.Length; i++)
                tileSets[i] = null;
            for (int i = 0; i < tileMaps.Length; i++)
                tileMaps[i] = null;
            for (int i = 0; i < physicalMaps.Length; i++)
                physicalMaps[i] = null;
        }
        /************************************************************************************
        * Private Supporting Methods
        * *********************************************************************************/
        private string ByteToStr(byte[] toStr)
        {
            System.Text.Encoding encoding = System.Text.Encoding.UTF8;

            return encoding.GetString(toStr);
        }

        // LJ 2011-12-28: interoperability fix for FF3usME
        public void LoadVarCompDataAbsPtrs()
        {
	        byte[] lGfxPtr24bits = new byte[LEN_MAP_GFXPIX_PTR];

            lGfxPtr24bits = ByteManage.GetByteArray(data, OFFS_MAP_GFXPIX_PTR, LEN_MAP_GFXPIX_PTR);

	        genFnts_24bitsToDword(lGfxPtr24bits, m_varCompDataAbsPtrs, NB_VAR_COMP_PTR);
        }

        public void SaveVarCompDataAbsPtrs()
        {
	        byte[] lGfxPtr24bits = new byte[LEN_MAP_GFXPIX_PTR];

	        genFnts_dwordTo24bits(m_varCompDataAbsPtrs, lGfxPtr24bits, NB_VAR_COMP_PTR);

            ByteManage.SetByteArray(data, OFFS_MAP_GFXPIX_PTR, lGfxPtr24bits);
        }

        public ulong HiROMToSMC(ulong iRomAddr)
        {
            return(iRomAddr - 0xC00000);
        }

        public ulong SMCToHiROM(ulong iSmcAddrHeaderless)
        {
            return(iSmcAddrHeaderless + 0xC00000);
        }

        public void genFnts_24bitsToDword(byte[] i24bitsArray, ulong[] iDwordArray, int iNb)
        {
	        int lCnt;
	        int lByteArrayCnt = 0;

	        for(lCnt=0; lCnt<iNb; lCnt++, lByteArrayCnt+=3)
	        {
		        iDwordArray[lCnt] = (ulong)i24bitsArray[lByteArrayCnt+2]<<16;
		        iDwordArray[lCnt] |= (ulong)i24bitsArray[lByteArrayCnt+1]<<8;
		        iDwordArray[lCnt] |= (ulong)i24bitsArray[lByteArrayCnt];
	        }
        }

        public void genFnts_dwordTo24bits(ulong[] iDwordArray, byte[] i24bitsArray, int iNb)
        {
	        int lCnt;
	        int lByteArrayCnt = 0;

	        for(lCnt=0; lCnt<iNb; lCnt++, lByteArrayCnt+=3)
	        {
		        i24bitsArray[lByteArrayCnt+2] = (byte)((iDwordArray[lCnt]>>16) & 0xFF);
		        i24bitsArray[lByteArrayCnt+1] = (byte)((iDwordArray[lCnt]>>8) & 0xFF);
		        i24bitsArray[lByteArrayCnt]   = (byte)(iDwordArray[lCnt] & 0xFF);
	        }
        }

        #region CE Expansion Methods

        /// <summary>
        /// madsiur [CE Edition 1.0]
        /// Init sizes, number of entries and offset by fetching from the ROM.
        /// </summary>
        /// <param name="isExpanded">True if ROM has expansion</param>
        public void InitExpansionFields(bool isExpanded)
        {
            //if ROM is expanded
            if (isExpanded)
            {
                SIZE_EVENT_PTR = 0x402;
                SIZE_EVENT_DATA = 0x3000;
                SIZE_NPC_PTR = 0x402;
                SIZE_NPC_DATA = 0x8BFF;
                SIZE_SHORT_EXIT_PTR = 0x402;
                SIZE_SHORT_EXIT_DATA = 0x3BFF;
                SIZE_LONG_EXIT_PTR = 0x402;
                SIZE_LONG_EXIT_DATA = 0xBFF;
                SIZE_CHEST_PTR = 0x400;
                SIZE_CHEST_DATA = 0x827;
                SIZE_TILEMAP_PTR = 0x5FD;
                
                SIZE_LOCATION = 0x4200;
                SIZE_LOC_NAMES = 0x2500;

                NUM_TILEMAPS = 511;
                NUM_LOCATIONS = 511;
                NUM_LOC_NAMES = 256;
            }
            else
            {
                SIZE_EVENT_PTR = 0x342;
                SIZE_EVENT_DATA = 0x16CE;
                SIZE_NPC_PTR = 0x342;
                SIZE_NPC_DATA = 0x4D6E;
                SIZE_SHORT_EXIT_PTR = 0x402;
                SIZE_SHORT_EXIT_DATA = 0x1AFE;
                SIZE_LONG_EXIT_PTR = 0x402;
                SIZE_LONG_EXIT_DATA = 0x57E;
                SIZE_CHEST_PTR = 0x340;
                SIZE_CHEST_DATA = 0x827;
                SIZE_TILEMAP_PTR = 0x41D;
                SIZE_TILEMAP_DATA = 0x42E50;
                SIZE_LOCATION = 0x3580;
                SIZE_LOC_NAMES = 0x500;

                NUM_TILEMAPS = 351;
                NUM_LOCATIONS = 415;
                NUM_LOC_NAMES = 73;
            }

            // fetch offsets from the ROM
            BASE_EVENT_PTR = Bits.ToAbs(ByteManage.GetInt(data, 0x00BCB5));
            Log.SetEntry("InitFields (isExpanded = " + isExpanded + ")", "Init", "BASE_EVENT_PTR", BASE_EVENT_PTR);
            BASE_EVENT = Bits.ToAbs(BASE_EVENT_PTR + SIZE_EVENT_PTR);
            Log.SetEntry("InitFields (isExpanded = " + isExpanded + ")", "Init", "BASE_EVENT", BASE_EVENT_PTR + SIZE_EVENT_PTR);
            BASE_NPC_PTR = Bits.ToAbs(ByteManage.GetInt(data, 0x0052C3));
            Log.SetEntry("InitFields (isExpanded = " + isExpanded + ")", "Init", "BASE_NPC_PTR", BASE_NPC_PTR);
            BASE_NPC = Bits.ToAbs(BASE_NPC_PTR + SIZE_NPC_PTR);
            Log.SetEntry("InitFields (isExpanded = " + isExpanded + ")", "Init", "BASE_NPC", BASE_NPC_PTR + SIZE_NPC_PTR);
            BASE_SHORT_EXIT_PTR = Bits.ToAbs(ByteManage.GetInt(data, 0x001A84));
            Log.SetEntry("InitFields (isExpanded = " + isExpanded + ")", "Init", "BASE_SHORT_EXIT_PTR", BASE_SHORT_EXIT_PTR);
            BASE_SHORT_EXIT = Bits.ToAbs(BASE_SHORT_EXIT_PTR + SIZE_SHORT_EXIT_PTR);
            Log.SetEntry("InitFields (isExpanded = " + isExpanded + ")", "Init", "BASE_SHORT_EXIT", BASE_SHORT_EXIT_PTR + SIZE_SHORT_EXIT_PTR);
            BASE_LONG_EXIT_PTR = Bits.ToAbs(ByteManage.GetInt(data, 0x0018F1));
            Log.SetEntry("InitFields (isExpanded = " + isExpanded + ")", "Init", "BASE_LONG_EXIT_PTR", BASE_LONG_EXIT_PTR);
            BASE_LONG_EXIT = Bits.ToAbs(BASE_LONG_EXIT_PTR + SIZE_LONG_EXIT_PTR);
            Log.SetEntry("InitFields (isExpanded = " + isExpanded + ")", "Init", "BASE_LONG_EXIT", BASE_LONG_EXIT_PTR + SIZE_LONG_EXIT_PTR);
            BASE_CHEST_PTR = Bits.ToAbs(ByteManage.GetInt(data, 0x004BE1));
            Log.SetEntry("InitFields (isExpanded = " + isExpanded + ")", "Init", "BASE_CHEST_PTR", BASE_CHEST_PTR);
            BASE_CHEST = Bits.ToAbs(BASE_CHEST_PTR + SIZE_CHEST_PTR);
            Log.SetEntry("InitFields (isExpanded = " + isExpanded + ")", "Init", "BASE_CHEST", BASE_CHEST_PTR + SIZE_CHEST_PTR);
            BASE_TILEMAP_PTR = Bits.ToAbs(ByteManage.GetInt(data, 0x002893));
            Log.SetEntry("InitFields (isExpanded = " + isExpanded + ")", "Init", "BASE_TILEMAP_PTR", BASE_TILEMAP_PTR);
            BASE_TILEMAP = Bits.ToAbs((data[0x0028A4] << 16) + ByteManage.GetShort(data, 0x002898));
            Log.SetEntry("InitFields (isExpanded = " + isExpanded + ")", "Init", "BASE_TILEMAP", (data[0x0028A4] << 16) + ByteManage.GetShort(data, 0x002898));
            BASE_LOCATION = Bits.ToAbs(ByteManage.GetInt(data, 0x001CC0));
            Log.SetEntry("InitFields (isExpanded = " + isExpanded + ")", "Init", "BASE_LOCATION", ByteManage.GetInt(data, 0x001CC0));
            BASE_LOC_NAMES_PTR = Bits.ToAbs(ByteManage.GetInt(data, 0x008009));
            Log.SetEntry("InitFields (isExpanded = " + isExpanded + ")", "Init", "BASE_LOC_NAMES_PTR", ByteManage.GetInt(data, 0x008009));
            BASE_LOC_NAMES = Bits.ToAbs((data[0x007FFE] << 16) + ByteManage.GetShort(data, 0x00800D));
            Log.SetEntry("InitFields (isExpanded = " + isExpanded + ")", "Init", "BASE_LOC_NAMES", (data[0x007FFE] << 16) + ByteManage.GetShort(data, 0x00800D));

            tileMaps = tileMaps = new byte[NUM_TILEMAPS][];
            editTileMaps = new bool[NUM_TILEMAPS];
            TileMapSizes = new ushort[NUM_TILEMAPS];
        }

        public bool ExpandROM(int dataOffset, int tilemapOffset, int tilemapSize, bool isZplus)
        {
            Log.InitLog();

            try
            {
                // if user was using FF6LE+ or ZD+ before
                if (isZplus)
                {
                    Log.SetEntry("IsZdPlus(true)");
                    SIZE_EVENT_DATA = 0x1C46;
                    SIZE_SHORT_EXIT_DATA = 0x2132;
                    SIZE_NPC_DATA = 0x5606;
                    SIZE_TILEMAP_DATA = 0x060000 > tilemapSize ? 0x60000: tilemapSize; // Approximate but it is trimmed later on.
                }

                Log.SetEntry("In Expansion", "Init", "TilemapsSize", tilemapSize);
                Log.SetEntry("In Expansion", "Init", "dataOffset", dataOffset);
                Log.SetEntry("In Expansion", "Init", "tilemapOffset", tilemapOffset);

                // Get original data
                byte[] EventPtrs = ByteManage.GetByteArray(data, BASE_EVENT_PTR, SIZE_EVENT_PTR);
                byte[] EventData = ByteManage.GetByteArray(data, BASE_EVENT, SIZE_EVENT_DATA);
                byte[] NpcPtrs = ByteManage.GetByteArray(data, BASE_NPC_PTR, SIZE_NPC_PTR);
                byte[] NpcData = ByteManage.GetByteArray(data, BASE_NPC, SIZE_NPC_DATA);
                byte[] ShortExitPtrs = ByteManage.GetByteArray(data, BASE_SHORT_EXIT_PTR, SIZE_SHORT_EXIT_PTR);
                byte[] ShortExitData = ByteManage.GetByteArray(data, BASE_SHORT_EXIT, SIZE_SHORT_EXIT_DATA);
                byte[] LongExitPtrs = ByteManage.GetByteArray(data, BASE_LONG_EXIT_PTR, SIZE_SHORT_EXIT_PTR);
                byte[] LongExitData = ByteManage.GetByteArray(data, BASE_LONG_EXIT, SIZE_LONG_EXIT_DATA);
                byte[] ChestPtrs = ByteManage.GetByteArray(data, BASE_CHEST_PTR, SIZE_CHEST_PTR);
                byte[] ChestData = ByteManage.GetByteArray(data, BASE_CHEST, SIZE_CHEST_DATA);
                byte[] TilemapPtrs = ByteManage.GetByteArray(data, BASE_TILEMAP_PTR, SIZE_TILEMAP_PTR);
                byte[] TilemapData = ByteManage.GetByteArray(data, BASE_TILEMAP, SIZE_TILEMAP_DATA);
                byte[] LocationData = ByteManage.GetByteArray(data, BASE_LOCATION, SIZE_LOCATION);
                byte[] LocNamesPtr = ByteManage.GetByteArray(data, BASE_LOC_NAMES_PTR, NUM_LOC_NAMES * 2);
                byte[] LocNames = ByteManage.GetByteArray(data, BASE_LOC_NAMES, SIZE_LOC_NAMES);

                Log.SetEntry("INIT ARRAYS");
                Log.SetEntry("In Expansion", "Init Arrays", "BASE_EVENT_PTR", BASE_EVENT_PTR);
                Log.SetEntry("EventPtrs", EventPtrs.Length);
                Log.SetEntry("In Expansion", "Init Arrays", "BASE_EVENT", BASE_EVENT);
                Log.SetEntry("EventData", EventData.Length);
                Log.SetEntry("In Expansion", "Init Arrays", "BASE_NPC_PTR", BASE_NPC_PTR);
                Log.SetEntry("NpcPtrs", NpcPtrs.Length);
                Log.SetEntry("In Expansion", "Init Arrays", "BASE_NPC", BASE_NPC);
                Log.SetEntry("NpcData", NpcData.Length);
                Log.SetEntry("In Expansion", "Init Arrays", "BASE_SHORT_EXIT_PTR", BASE_SHORT_EXIT_PTR);
                Log.SetEntry("ShortExitPtrs", ShortExitPtrs.Length);
                Log.SetEntry("In Expansion", "Init Arrays", "BASE_SHORT_EXIT", BASE_SHORT_EXIT);
                Log.SetEntry("ShortExitData", ShortExitData.Length);
                Log.SetEntry("In Expansion", "Init Arrays", "BASE_LONG_EXIT_PTR", BASE_LONG_EXIT_PTR);
                Log.SetEntry("LongExitPtrs", LongExitPtrs.Length);
                Log.SetEntry("In Expansion", "Init Arrays", "BASE_LONG_EXIT", BASE_LONG_EXIT);
                Log.SetEntry("LongExitData", LongExitData.Length);
                Log.SetEntry("In Expansion", "Init Arrays", "BASE_CHEST_PTR", BASE_CHEST_PTR);
                Log.SetEntry("ChestPtrs", ChestPtrs.Length);
                Log.SetEntry("In Expansion", "Init Arrays", "BASE_CHEST", BASE_CHEST);
                Log.SetEntry("ChestData", ChestData.Length);
                Log.SetEntry("In Expansion", "Init Arrays", "BASE_TILEMAP_PTR", BASE_TILEMAP_PTR);
                Log.SetEntry("TilemapPtrs", TilemapPtrs.Length);
                Log.SetEntry("In Expansion", "Init Arrays", "BASE_TILEMAP", BASE_TILEMAP);
                Log.SetEntry("TilemapData", TilemapData.Length);
                Log.SetEntry("In Expansion", "Init Arrays", "BASE_LOCATION", BASE_LOCATION);
                Log.SetEntry("LocationData", LocationData.Length);
                Log.SetEntry("In Expansion", "Init Arrays", "BASE_LOC_NAMES_PTR", BASE_LOC_NAMES_PTR);
                Log.SetEntry("LocNamesPtr", LocNamesPtr.Length);
                Log.SetEntry("In Expansion", "Init Arrays", "BASE_LOC_NAMES", BASE_LOC_NAMES);
                Log.SetEntry("LocNames", LocNames.Length);



                // Erase original data in ROM (except location names)
                Bits.Fill(data, Expansion.FILLER, BASE_EVENT_PTR, SIZE_EVENT_PTR + SIZE_EVENT_DATA);
                Bits.Fill(data, Expansion.FILLER, BASE_NPC_PTR, SIZE_NPC_PTR + SIZE_NPC_DATA);
                Bits.Fill(data, Expansion.FILLER, BASE_SHORT_EXIT_PTR, SIZE_SHORT_EXIT_PTR + SIZE_SHORT_EXIT_DATA);
                Bits.Fill(data, Expansion.FILLER, BASE_LONG_EXIT_PTR, SIZE_LONG_EXIT_PTR + SIZE_LONG_EXIT_DATA);
                Bits.Fill(data, Expansion.FILLER, BASE_CHEST_PTR, SIZE_CHEST_PTR + SIZE_CHEST_DATA);
                Bits.Fill(data, Expansion.FILLER, BASE_TILEMAP_PTR, SIZE_TILEMAP_PTR + SIZE_TILEMAP_DATA + 3);
                Bits.Fill(data, Expansion.FILLER, BASE_LOCATION, SIZE_LOCATION);

                // Pointer incremention value (for 2 bytes pointers)
                ushort inctPtr = (ushort)(Expansion.NEW_ENTRIES * 2);

                Log.SetEntry("POINTERS INCREMENTATION");
                Log.SetEntry("In Expansion", "Ptr inc", "inctPtr", inctPtr);
                // Increment all pointers (except chests and tilemaps)
                Bits.IncShort(EventPtrs, inctPtr);
                Bits.IncShort(NpcPtrs, inctPtr);
                Bits.IncShort(ShortExitPtrs, inctPtr);
                Bits.IncShort(LongExitPtrs, inctPtr);

                Log.SetEntry("MAXIMUMS FINDING");
                // Get highests / last pointers
                ushort LastEventPtr = (ushort)Bits.findArrayMax(EventPtrs, 2);
                ushort LastNpcPtr = (ushort)Bits.findArrayMax(NpcPtrs, 2);
                ushort LastShortExitPtr = (ushort)Bits.findArrayMax(ShortExitPtrs, 2);
                ushort LastLongExitPtr = (ushort)Bits.findArrayMax(LongExitPtrs, 2);
                ushort LastChestPtr = (ushort)Bits.findArrayMax(ChestPtrs, 2);
                int LastTilemapPtr = Bits.findArrayMax(TilemapPtrs, 3);
                ushort LastLocNamePtr = (ushort)Bits.findArrayMax(LocNamesPtr, 2);

                // Create expansion pointers arrays
                byte[] ExpEventPtrs = new byte[inctPtr];
                byte[] ExpNpcPtrs = new byte[inctPtr];
                byte[] ExpShortExitPtrs = new byte[inctPtr];
                byte[] ExpLongExitPtrs = new byte[inctPtr];
                byte[] ExpChestPtrs = new byte[inctPtr];
                byte[] ExpTilemapPtrs = new byte[(511 - NUM_TILEMAPS) * 3];

                Log.SetEntry("FILL EXP POINTERS ARRAY");
                // Fill each array with highest pointer
                Bits.FillShort(ExpEventPtrs, LastEventPtr);
                Bits.FillShort(ExpNpcPtrs, LastNpcPtr);
                Bits.FillShort(ExpShortExitPtrs, LastShortExitPtr);
                Bits.FillShort(ExpLongExitPtrs, LastLongExitPtr);
                Bits.FillShort(ExpChestPtrs, LastChestPtr);

                Log.SetEntry("In Expansion", "Filling", "LastEventPtr", LastEventPtr);
                Log.SetEntry("In Expansion", "Filling", "LastNpcPtr", LastNpcPtr);
                Log.SetEntry("In Expansion", "Filling", "LastShortExitPtr", LastShortExitPtr);
                Log.SetEntry("In Expansion", "Filling", "LastLongExitPtr", LastLongExitPtr);
                Log.SetEntry("In Expansion", "Filling", "LastChestPtr", LastChestPtr);


                Log.SetEntry("FILL TILEMAPS");
                // Last pointer points to nothing so we need to resize tilemap data accordingly.
                TilemapData = ByteManage.GetByteArray(TilemapData, 0, LastTilemapPtr);
                Log.SetEntry("In Expansion", "Tilemaps", "LastTilemapPtr", LastTilemapPtr);
                Log.SetEntry("TilemapData", TilemapData.Length);
                Log.SetEntry("Expansion.DEFAULT_TILEMAP", Expansion.DEFAULT_TILEMAP.Length);

                string lg1 = "";
                Log.SetEntry("TILEMAPS NEW POINTERS");
                // Since we're going to copy default tilemap numerous times, increase each pointer by its length
                for (int i = 0; i < ExpTilemapPtrs.Length; i += 3)
                {
                    LastTilemapPtr += Expansion.DEFAULT_TILEMAP.Length;

                    ExpTilemapPtrs[i] = (byte)(LastTilemapPtr & 0xFF);
                    ExpTilemapPtrs[i + 1] = (byte)((LastTilemapPtr >> 8) & 0xFF);
                    ExpTilemapPtrs[i + 2] = (byte)((LastTilemapPtr >> 16) & 0xFF);

                    lg1 += ByteManage.GetInt(ExpTilemapPtrs, i).ToString("X6") + " ";
                    if (i % 16 == 0)
                    {
                        lg1 += "\n";
                    }
                }

                Log.log.Add(lg1);
                Log.SetEntry("END TILEMPAS POINTERS");


                ByteManage.SetByteArray(data, Expansion.NEW_LOC_NAME, LocNamesPtr);
                ByteManage.SetByteArray(data, Expansion.NEW_LOC_NAME + 0x200, LocNames);

                ushort j = LastLocNamePtr;

                while (data[j] != 0)
                    j++;

                for (int i = NUM_LOC_NAMES; i < 256; i++)
                {
                    ByteManage.SetShort(data, Expansion.NEW_LOC_NAME + i * 2, j);
                    ByteManage.SetByteArray(data, Expansion.NEW_LOC_NAME + 0x200 + j, Expansion.DEFAULT_LOC_NAME);
                    j += (ushort)Expansion.DEFAULT_LOC_NAME.Length;
                }

                // put back the banks offsets to absolute value because we are going to write to the ROM
                dataOffset = Bits.ToAbs(dataOffset);
                tilemapOffset = Bits.ToAbs(tilemapOffset);

                Log.SetEntry("In Expansion", "Init Exp banks", "dataOffset", dataOffset);
                Log.SetEntry("In Expansion", "Init Exp banks", "tilemapOffset", tilemapOffset);

                // Inset events, npcs, exits, chests ptrs and data
                Log.SetEntry("SetData Events");
                Bits.setData(data, BASE_EVENT_PTR, EventPtrs, ExpEventPtrs, EventData);
                Log.SetEntry("SetData NPC");
                Bits.setData(data, dataOffset, NpcPtrs, ExpNpcPtrs, NpcData);
                Log.SetEntry("SetData Short Exits");
                Bits.setData(data, dataOffset + Expansion.NEW_SHORT_EXIT, ShortExitPtrs, ExpShortExitPtrs, ShortExitData);
                Log.SetEntry("SetData Long Exits");
                Bits.setData(data, dataOffset + Expansion.NEW_LONG_EXIT, LongExitPtrs, ExpLongExitPtrs, LongExitData);
                Log.SetEntry("SetData Chests");
                Bits.setData(data, dataOffset + Expansion.NEW_CHEST, ChestPtrs, ExpChestPtrs, ChestData);

                Log.SetEntry("LOCATIONS");
                // Insert locations data
                int offset = Expansion.NEW_LOCATION;
                ByteManage.SetByteArray(data, offset, LocationData);

                Log.SetEntry("In Expansion", "Set location data", "offset", offset);
                Log.SetEntry("LocationData", LocationData.Length);

                offset += NUM_LOCATIONS * 33;

                Log.SetEntry("In Expansion", "increase offset", "NUM_LOCATIONS * 33", NUM_LOCATIONS * 33);
                Log.SetEntry("In Expansion", "increase offset", "offset", offset);
                Log.SetEntry("Expansion.DEFAULT_LOCATION", Expansion.DEFAULT_LOCATION.Length);

                for (int i = 0; i < Expansion.NEW_ENTRIES; i++)
                {
                    Log.SetEntry("In Expansion", "add location", "offset", offset);
                    ByteManage.SetByteArray(data, offset, Expansion.DEFAULT_LOCATION);
                    offset += Expansion.DEFAULT_LOCATION.Length;
                }

                Log.SetEntry("TILEMAPS");
                // Get last tilemap pointer
                LastTilemapPtr = Bits.findArrayMax(TilemapPtrs, 3);
                Log.SetEntry("In Expansion", "get max", "LastTilemapPtr", LastTilemapPtr);

                // Insert Tilemaps ptrs and data
                offset = BASE_TILEMAP_PTR;
                ByteManage.SetByteArray(data, offset, TilemapPtrs);
                Log.SetEntry("In Expansion", "set tilemaps A", "offset", offset);
                Log.SetEntry("TilemapPtrs", TilemapPtrs.Length);
                offset += SIZE_TILEMAP_PTR;
                ByteManage.SetByteArray(data, offset, ExpTilemapPtrs);
                Log.SetEntry("In Expansion", "set tilemaps B", "offset", offset);
                Log.SetEntry("ExpTilemapPtrs", ExpTilemapPtrs.Length);
                offset = tilemapOffset;
                ByteManage.SetByteArray(data, offset, TilemapData);
                Log.SetEntry("In Expansion", "set tilemaps C", "offset", offset);
                Log.SetEntry("TilemapData", TilemapData.Length);
                offset = tilemapOffset + LastTilemapPtr;

                Log.SetEntry("Add new tilemaps");
                for (int i = 0; i < (511 - NUM_TILEMAPS); i++)
                {
                    Log.SetEntry("In Expansion", "Add extra tilemap" + i, "offset", offset);
                    ByteManage.SetByteArray(data, offset, Expansion.DEFAULT_TILEMAP);
                    offset += Expansion.DEFAULT_TILEMAP.Length;
                }

                // We ned the new banks to HiROM
                dataOffset = Bits.ToHiROM(dataOffset);

                Log.SetEntry("In Expansion", "set data bank", "dataOffset", dataOffset);

                // ASM code changes for events, NPCs, Exits and Chests (LDAs)
                Log.SetEntry("Writing Event ASM");
                Bits.setAsmArray(data, Expansion.ROM_EVENT, Expansion.ROM_EVENT_VAR, Bits.ToHiROM(BASE_EVENT_PTR));
                Log.SetEntry("Writing NPC ASM");
                Bits.setAsmArray(data, Expansion.ROM_NPC, Expansion.ROM_NPC_VAR, dataOffset);
                Log.SetEntry("Writing Short Exit ASM");
                Bits.setAsmArray(data, Expansion.ROM_SHORT_EXIT, Expansion.ROM_SHORT_EXIT_VAR, dataOffset + Expansion.NEW_SHORT_EXIT);
                Log.SetEntry("Write Long Exit ASM");
                Bits.setAsmArray(data, Expansion.ROM_LONG_EXIT, Expansion.ROM_LONG_EXIT_VAR, dataOffset + Expansion.NEW_LONG_EXIT);
                Log.SetEntry("Write Chest ASM");
                Bits.setAsmArray(data, Expansion.ROM_CHEST, Expansion.ROM_CHEST_VAR_EXP, dataOffset + Expansion.NEW_CHEST);

                Log.SetEntry("Write Locations ASM");
                // LDA change for Locations
                Bits.SetInt(data, Bits.ToAbs(Expansion.ROM_LOCATION + 1), Bits.ToHiROM(Expansion.NEW_LOCATION));
                Log.SetEntry("In Expansion", "Write Locations ASM", "Expansion.ROM_LOCATION", Expansion.ROM_LOCATION + 1);
                Log.SetEntry("In Expansion", "Write Locations ASM", "Expansion.NEW_LOCATION", Bits.ToHiROM(Expansion.NEW_LOCATION));

                Log.SetEntry("Set Tilemaps ADC & LDA");
                // ADC and LDA changes for tilemaps
                Bits.setAsmArray(data, Expansion.ROM_TILEMAP_SHORT, (ushort)0x0000);
                Bits.setAsmArray(data, Expansion.ROM_TILEMAP_BYTE, Bits.ToHiROM((byte)(tilemapOffset >> 16)));

                // ADC and LDA changes for loc names
                Log.SetEntry("Writing Location Names ASM");
                Bits.SetInt(data, 0x008009, Bits.ToHiROM(Expansion.NEW_LOC_NAME));
                Bits.setAsmArray(data, Expansion.ROM_LOC_NAME_SHORT, 0x4400);
                Bits.setAsmArray(data, Expansion.ROM_LOC_NAME_BYTE, 0xDA);

                // load expanded variables values
                //InitExpansionFields(true);
                
                //IsExpanded = true;

                Log.SetEntry("END OF LOG " + DateTime.Now.ToString(new CultureInfo("en-US")));
                Log.WriteLog();
                
                return true;
            }
            catch (Exception e)
            {
                Log.SetEntry("In Expansion", "ERROR", e.Message, e.GetHashCode());
                Log.SetEntry("END OF LOG " + DateTime.Now.ToString(new CultureInfo("en-US")));
                Log.WriteLog();
                return false;
            }
        }

        public bool ExpandChests()
        {
            try
            {
                Bits.setAsmArray(data, Expansion.ROM_EXP_CHEST_SHORT_MEM, 0x1E20);
                Bits.setAsmArray(data, Expansion.ROM_EXP_CHEST_SHORT_NUM, 0x03FF);
                ByteManage.SetShort(data, 0x00BB1F, 0x0060);
                IsChestsExpanded = true;

                return true;
            }
            catch (Exception e)
            {
                MessageBox.Show("Unable to perform chest expansion.\n\n Error: " + e.Message, "FF6LE",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);

                return false;
            }
            
        }

        public List<int[]> ValidateChestExpansion()
        {
            List<int[]> faults = new List<int[]>();

            for (int i = 0; i < Expansion.ROM_EXP_CHEST_SHORT_MEM.Length; i++)
            {
                Bits.IsMatchingShort(data, 0x1E40, Expansion.ROM_EXP_CHEST_SHORT_MEM[i], ref faults);
            }

            for (int i = 0; i < Expansion.ROM_EXP_CHEST_SHORT_NUM.Length; i++)
            {
                Bits.IsMatchingShort(data, 0x01FF, Expansion.ROM_EXP_CHEST_SHORT_NUM[i], ref faults);
            }

            Bits.IsMatchingShort(data, 0x0030, 0x00BB1F, ref faults);

            return faults;
        }
        public List<int[]> ValidateROM(bool isZplus)
        {
            List<int[]> faults = new List<int[]>();
            if (!isZplus)
            {
                for (int i = 0; i < Expansion.ROM_EVENT.Length; i++)
                {
                    Bits.IsMatchingOffset(data, BASE_EVENT_PTR + Expansion.ROM_EVENT_VAR[i],
                        Expansion.ROM_EVENT[i], ref faults);
                }
            }

            for (int i = 0; i < Expansion.ROM_CHEST.Length; i++)
            {
                Bits.IsMatchingOffset(data, BASE_CHEST_PTR + Expansion.ROM_CHEST_VAR[i],
                    Expansion.ROM_CHEST[i], ref faults);
            }

            if (!isZplus)
            {
                for (int i = 0; i < Expansion.ROM_NPC.Length; i++)
                {
                    Bits.IsMatchingOffset(data, BASE_NPC_PTR + Expansion.ROM_NPC_VAR[i],
                        Expansion.ROM_NPC[i], ref faults);
                }
            }

            if (!isZplus)
            {
                for (int i = 0; i < Expansion.ROM_LONG_EXIT.Length; i++)
                {
                    Bits.IsMatchingOffset(data, BASE_LONG_EXIT_PTR + Expansion.ROM_LONG_EXIT_VAR[i],
                        Expansion.ROM_LONG_EXIT[i], ref faults);
                }
            }

            if (!isZplus)
            {
                for (int i = 0; i < Expansion.ROM_SHORT_EXIT.Length; i++)
                {
                    Bits.IsMatchingOffset(data, BASE_SHORT_EXIT_PTR + Expansion.ROM_SHORT_EXIT_VAR[i],
                        Expansion.ROM_SHORT_EXIT[i], ref faults);
                }
            }

            if (!isZplus)
            {
                for (int i = 0; i < Expansion.ROM_TILEMAP_INT.Length; i++)
                {
                    Bits.IsMatchingOffset(data, BASE_TILEMAP_PTR + Expansion.ROM_TILEMAP_INT_VAR[i],
                        Expansion.ROM_TILEMAP_INT[i], ref faults);
                }


                for (int i = 0; i < Expansion.ROM_TILEMAP_SHORT.Length; i++)
                {
                    Bits.IsMatchingShort(data, Expansion.ROM_TILEMAP_SHORT_VAL[i], Expansion.ROM_TILEMAP_SHORT[i], ref faults);
                }

                for (int i = 0; i < Expansion.ROM_TILEMAP_BYTE.Length; i++)
                {
                    Bits.IsMatchingByte(data, Expansion.ROM_TILEMAP_BYTE_VAL[i], Expansion.ROM_TILEMAP_BYTE[i], ref faults);
                }
            }

            for (int i = 0; i < Expansion.ROM_LOC_NAME_SHORT.Length; i++)
            {
                Bits.IsMatchingShort(data, Expansion.ROM_LOC_NAME_SHORT_VAL[i], Expansion.ROM_LOC_NAME_SHORT[i], ref faults);
            }

            for (int i = 0; i < Expansion.ROM_LOC_NAME_BYTE.Length; i++)
            {
                Bits.IsMatchingByte(data, Expansion.ROM_LOC_NAME_BYTE_VAL[i], Expansion.ROM_LOC_NAME_BYTE[i], ref faults);
            }

            if (Bits.ToHiROM(BASE_LOCATION) != (int)0xED8F00)
            {
                faults.Add(new[] { Expansion.ROM_LOCATION + 1, Bits.ToHiROM(BASE_LOCATION), 0xED8F00 });
            }

            return faults;
        }
        /// <summary>
        /// madsiur [CE Edition 1.0]
        /// Check if expansion is done and set variable accordingly
        /// </summary>
        /// <returns></returns>
        public void CheckExpansion()
        {
            IsExpanded = false;
            IsChestsExpanded = false;

            if (File.Exists(Settings.Default.SettingsFile))
            {
                try
                {
                    Model.SettingsFile = XDocument.Load(Settings.Default.SettingsFile);
                    XElement root = Model.SettingsFile.Element("Settings");
                    IsExpanded = bool.Parse(root.Element("MapExpansion").Value);
                    IsChestsExpanded = bool.Parse(root.Element("ChestExpansion").Value);

                    if (IsExpanded)
                    {
                        SIZE_TILEMAP_DATA = int.Parse(root.Element("NumBanksTilemap").Value) << 16;
                        XElement mapNames = root.Element("MapNames");
                        LevelNames = new string[mapNames.Elements().Count()];

                        for (int i = 0; i < mapNames.Elements().Count(); i++)
                        {
                            LevelNames[i] = mapNames.Elements().ElementAt(i).Value;
                        }
                    }

                    InitExpansionFields(IsExpanded);
                }
                catch (Exception e)
                {
                    MessageBox.Show("Corrupted XML file. Default values will be loaded.\n\n Error: " + e.Message);
                    InitExpansionFields(false);

                    if (IsExpanded)
                    {
                        LevelNames = ConvertLocNames(Settings.Default.ExpandedLevelNames);
                    }
                    else
                    {
                        LevelNames = ConvertLocNames(Settings.Default.LevelNames);
                    }
                }
            }
            else
            {
                DialogResult dr = MessageBox.Show(
                    "No setting file found... Load ROM memory byte settings? (if version 0.6 was used before)", "FF6LE",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dr == DialogResult.Yes)
                {
                    ImportWindow iw = new ImportWindow(this);
                    iw.ShowDialog();

                    if (File.Exists(Settings.Default.SettingsFile))
                    {
                        try
                        {
                            Model.SettingsFile = XDocument.Load(Settings.Default.SettingsFile);
                            XElement root = Model.SettingsFile.Element("Settings");
                            IsExpanded = bool.Parse(root.Element("MapExpansion").Value);
                            IsChestsExpanded = bool.Parse(root.Element("ChestExpansion").Value);

                            if (IsExpanded)
                            {
                                SIZE_TILEMAP_DATA = int.Parse(root.Element("NumBanksTilemap").Value) << 16;
                                XElement mapNames = root.Element("MapNames");
                                LevelNames = new string[mapNames.Elements().Count()];

                                for (int i = 0; i < mapNames.Elements().Count(); i++)
                                {
                                    LevelNames[i] = mapNames.Elements().ElementAt(i).Value;
                                }
                            }

                            InitExpansionFields(IsExpanded);
                        }
                        catch (Exception e)
                        {
                            MessageBox.Show("Corrupted XML file. Default values will be loaded.\n\n Error: " + e.Message);
                            InitExpansionFields(false);

                            if (IsExpanded)
                            {
                                LevelNames = ConvertLocNames(Settings.Default.ExpandedLevelNames);
                            }
                            else
                            {
                                LevelNames = ConvertLocNames(Settings.Default.LevelNames);
                            }
                        }
                    }
                    else
                    {
                        InitExpansionFields(false);
                        LevelNames = ConvertLocNames(Settings.Default.LevelNames);
                    }
                    
                }
            }

        }

        public static string[] IterateLocations(string[] array)
        {
            string[] array2 = new string[array.Length];

            for (int i = 0; i < array.Length; i++)
            {
                array2[i] = "[$" + i.ToString("X3") + "] " + array[i];
            }

            return array2;
        }

        public static void BuildSettingXml(int tilemapBank, int dataBank, int tilemapBankNum, bool mapExpansion, bool chestExpansion, bool isZplus, string[] locNames)
        {
            SettingsFile = new XDocument(new XElement("Settings",
                new XElement("MapExpansion", mapExpansion.ToString()),
                new XElement("ChestExpansion", chestExpansion.ToString()),
                new XElement("FF6LEPlus", isZplus.ToString()),
                new XElement("DataBank", dataBank.ToString("X2")),
                new XElement("TilemapBank", tilemapBank.ToString("X2")),
                new XElement("NumBanksTilemap", tilemapBankNum.ToString()),
                new XElement("MapNames")));

            for (int i = 0; i < locNames.Length; i++)
            {
                SettingsFile.Element("Settings").Element("MapNames").Add(new XElement("Name", locNames[i]));
            }
        }

        public static void SaveXML()
        {
            if (File.Exists(Settings.Default.SettingsFile) && SettingsFile != null)
            {
                SettingsFile.Element("Settings").Element("MapNames").RemoveNodes();

                for (int i = 0; i < LevelNames.Length; i++)
                {
                    SettingsFile.Element("Settings").Element("MapNames").Add(new XElement("Name", LevelNames[i]));
                }

                try
                {
                    SettingsFile.Save(Settings.Default.SettingsFile);
                }
                catch (Exception e)
                {
                    MessageBox.Show(
                        "Unable to save XML settings file. You may not have write rights or file may not exist.\n\n  Error: " +
                        e.Message, "FF6LE", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        public static string[] ConvertLocNames(StringCollection collection)
        {
            string[] locNames = new string[collection.Count];
            collection.CopyTo(locNames, 0);
            return locNames;
        }
        #endregion

    }

}

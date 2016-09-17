using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace FF3LE
{
    // Creates and owns all the levels
    public class LevelModel
    {
        private Model model;
        private byte[] data;
        private Level[] levels;
        private PaletteSet[] paletteSets;
        private PrioritySet[] prioritySets;
        //private Battlefield[] battlefields;
        //private PhysicalTile[] physicalTiles;
        //private NPCProperties[] npcProperties;
        //private NPCSpritePartitions[] npcSpritePartitions;

        public Level[] Levels { get { return levels; } set { levels = value; } }
        public PaletteSet[] PaletteSets { get { return paletteSets; } set { paletteSets = value; } }
        public PrioritySet[] PrioritySets { get { return prioritySets; } set { prioritySets = value; } }
        //public Battlefield[] Battlefields { get { return battlefields; } }
        //public PhysicalTile[] PhysicalTiles { get { return physicalTiles; } }
        //public NPCProperties[] NPCProperties { get { return npcProperties; } }
        //public NPCSpritePartitions[] NPCSpritePartitions { get { return npcSpritePartitions; } }

        public LevelModel(byte[] data, Model model)
        {
            this.data = data;
            this.model = model;
            // Create Level Objects here
            CreateLevels();
            CreateLevelPaletteSets();
            CreatePrioritySets();
            //CreateBattlefields();
            //CreatePhysicalTiles();
            //CreateNPCProperties();
            //CreateNPCSpritePartitions();

        }
        private void CreateLevels()
        {
            levels = new Level[Model.NUM_LOCATIONS];
            for (int i = 0; i < levels.Length; i++)
            {
                levels[i] = new Level(data, i);
            }
        }
        private void CreatePrioritySets()
        {
            prioritySets = new PrioritySet[19];
            for (int i = 0; i < prioritySets.Length; i++)
            {
                prioritySets[i] = new PrioritySet(data, i);
            }

        }
        private void CreateLevelPaletteSets()
        {
            paletteSets = new PaletteSet[51]; // 48 palette sets, 7 palettes each with 16 colors each
            for (int i = 0; i < 50; i++)
            {
                paletteSets[i] = new PaletteSet(data, i);
            }
        }

        //}
        //private void CreatePhysicalTiles()
        //{
        //    physicalTiles = new PhysicalTile[1024];
        //    for (int i = 0; i < physicalTiles.Length; i++)
        //    {
        //        physicalTiles[i] = new PhysicalTile(data, i);
        //    }
        //}
        //private void CreateNPCProperties()
        //{
        //    npcProperties = new NPCProperties[512];
        //    for (int i = 0; i < npcProperties.Length; i++)
        //    {
        //        npcProperties[i] = new NPCProperties(data, i);
        //    }
        //}
        //private void CreateNPCSpritePartitions()
        //{
        //    npcSpritePartitions = new NPCSpritePartitions[78];
        //    for (int i = 0; i < npcSpritePartitions.Length; i++)
        //    {
        //        npcSpritePartitions[i] = new NPCSpritePartitions(data, i);
        //    }
        //}
        //private void CreateBattlefields()
        //{
        //    battlefields = new Battlefield[64];
        //    for (int i = 0; i < battlefields.Length; i++)
        //    {
        //        battlefields[i] = new Battlefield(data, i);
        //    }
        //}

    }
}

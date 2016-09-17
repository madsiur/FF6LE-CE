using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace FF3LE
{
    class TileMap
    {
        private Model model;
        private Tileset tileset;
        private PaletteSet paletteSet;
        private LevelLayer levelLayer;
        private PrioritySet[] prioritySets;
        private State state;

        //private int[] tileMapOffset;
        private byte[][] tileMaps = new byte[3][]; public byte[][] TileMaps { get { return tileMaps; } set { tileMaps = value; } }

        private Tile16x16[][] layers = new Tile16x16[3][];

        private int[] mainscreen = new int[2048 * 2048]; public int[] Mainscreen { get { return mainscreen; } }
        private int[] subscreenPixels = null;
        private int[] colorMath = null;

        private int[]
            layer1Priority0 = new int[2048 * 2048],
            layer1Priority1 = new int[2048 * 2048],
            layer2Priority0 = new int[2048 * 2048],
            layer2Priority1 = new int[2048 * 2048],
            layer3Priority0 = new int[2048 * 2048],
            layer3Priority1 = new int[2048 * 2048];

        public TileMap(LevelLayer levelLayer, PaletteSet paletteSet, Tileset tileset, LevelLayer layer, PrioritySet[] prioritySets, Model model)
        {
            this.model = model;
            this.tileset = tileset;
            this.levelLayer = levelLayer;
            this.paletteSet = paletteSet;
            this.prioritySets = prioritySets;
            this.levelLayer = layer;
            this.state = State.Instance;

            DecompressLevelData(); // Decompress all the data we need

            for (int i = 0; i < 3; i++)
                CreateLayer(i); // Create any required layers

            DrawAllLayers();

            if ((prioritySets[levelLayer.LayerPrioritySet].SubscreenL1 && state.Layer1) ||
                (prioritySets[levelLayer.LayerPrioritySet].SubscreenL2 && state.Layer2) ||
                (prioritySets[levelLayer.LayerPrioritySet].SubscreenL3 && state.Layer3) ||
                (prioritySets[levelLayer.LayerPrioritySet].SubscreenOBJ && state.Objects))
            {
                if (subscreenPixels == null)
                    subscreenPixels = new int[2048 * 2048];
                CreateSubscreen(); // Create the subscreen if needed
            }

            CreateMainscreen();
        }

        private void DrawAllLayers()
        {

            if (prioritySets[levelLayer.LayerPrioritySet].SubscreenL1 || prioritySets[levelLayer.LayerPrioritySet].MainscreenL1)
            {
                DrawLayerByPriorityOne(layer1Priority0, 0, false);
                DrawLayerByPriorityOne(layer1Priority1, 0, true);
            }
            if (prioritySets[levelLayer.LayerPrioritySet].SubscreenL2 || prioritySets[levelLayer.LayerPrioritySet].MainscreenL2)
            {
                DrawLayerByPriorityOne(layer2Priority0, 1, false);
                DrawLayerByPriorityOne(layer2Priority1, 1, true);
            }
            if ((prioritySets[levelLayer.LayerPrioritySet].SubscreenL3 || prioritySets[levelLayer.LayerPrioritySet].MainscreenL3))
            {
                DrawLayerByPriorityOne(layer3Priority0, 2, false);
                //DrawLayerByPriorityOne(layer3Priority1, 2, true);
            }
        }

        public void RedrawTileMap()
        {
            DrawAllLayers();

            ClearArray(mainscreen);
            if (subscreenPixels != null)
                ClearArray(subscreenPixels);

            if ((prioritySets[levelLayer.LayerPrioritySet].SubscreenL1 && state.Layer1) ||
                    (prioritySets[levelLayer.LayerPrioritySet].SubscreenL2 && state.Layer2) ||
                    (prioritySets[levelLayer.LayerPrioritySet].SubscreenL3 && state.Layer3) ||
                    (prioritySets[levelLayer.LayerPrioritySet].SubscreenOBJ && state.Objects))
            {
                if (subscreenPixels == null)
                    subscreenPixels = new int[2048 * 2048];
                CreateSubscreen(); // Create the subscreen if needed
            }

            CreateMainscreen();
        }

        int[] tile = new int[256];
        int[] tileColorMath = new int[256];

        private void ChangeSingleTile(int layer, int placement, int tile, int x, int y)
        {
            layers[layer][placement] = tileset.TileSetLayers[layer][tile]; // Change the tile in the layer map

            Tile16x16 source = layers[layer][placement]; // Grab the new tile
            int[] layerA = null, layerB = null; // Just used to save space


            if (layer == 0)
            {
                layerA = layer1Priority0;
                layerB = layer1Priority1;
            }
            else if (layer == 1)
            {
                layerA = layer2Priority0;
                layerB = layer2Priority1;
            }
            else if (layer == 2)
            {
                layerA = layer3Priority0;
                layerB = layer3Priority1;
            }

            ClearSingleTile(layerA, x, y);
            ClearSingleTile(layerB, x, y);

            // Draw all 4 subtiles to the appropriate array based on priority
            if (!source.GetPriority1(0)) // tile 0
                CopyOverTile8x8(source.GetSubtile(0), layerA, 2048, x, y);
            else
                CopyOverTile8x8(source.GetSubtile(0), layerB, 2048, x, y);
            if (!source.GetPriority1(1)) // tile 1
                CopyOverTile8x8(source.GetSubtile(1), layerA, 2048, x + 8, y);
            else
                CopyOverTile8x8(source.GetSubtile(1), layerB, 2048, x + 8, y);
            if (!source.GetPriority1(2)) // tile 2
                CopyOverTile8x8(source.GetSubtile(2), layerA, 2048, x, y + 8);
            else
                CopyOverTile8x8(source.GetSubtile(2), layerB, 2048, x, y + 8);
            if (!source.GetPriority1(3)) // tile 3
                CopyOverTile8x8(source.GetSubtile(3), layerA, 2048, x + 8, y + 8);
            else
                CopyOverTile8x8(source.GetSubtile(3), layerB, 2048, x + 8, y + 8);

            // If we have a subscreen, draw the new tile to it
            if ((prioritySets[levelLayer.LayerPrioritySet].SubscreenL1 && state.Layer1) ||
                (prioritySets[levelLayer.LayerPrioritySet].SubscreenL2 && state.Layer2) ||
                (prioritySets[levelLayer.LayerPrioritySet].SubscreenL3 && state.Layer3) ||
                (prioritySets[levelLayer.LayerPrioritySet].SubscreenOBJ && state.Objects))
            {
                ClearSingleTile(subscreenPixels, x, y);
                DrawSingleSubscreenTile(x, y);
            }
            ClearSingleTile(mainscreen, x, y);
            DrawSingleMainscreenTile(x, y);
        }
        private void ClearSingleTile(int[] arr, int x, int y)
        {
            int counter = 0;
            for (int i = 0; i < 256; i++)
            {
                arr[y * 2048 + x + counter] = 0;

                counter++;
                if (counter % 16 == 0)
                {
                    y++;
                    counter = 0;
                }
            }
        }
        private void DrawSingleSubscreenTile(int x, int y)
        {
            if (levelLayer.TopPriorityL3) //[3,0][2,0][1,0][2,1][1,1][3,1]
            {
                if (prioritySets[levelLayer.LayerPrioritySet].SubscreenL2 && state.Layer2)
                {
                    tile = GetTileFromPriorityArray(layer2Priority0, x, y);
                    CopySingleTileToArray(subscreenPixels, tile, 2048, x, y);
                    ClearArray(tile);
                }
                if (prioritySets[levelLayer.LayerPrioritySet].SubscreenL1 && state.Layer1)
                {
                    tile = GetTileFromPriorityArray(layer1Priority0, x, y);
                    CopySingleTileToArray(subscreenPixels, tile, 2048, x, y);
                    ClearArray(tile);
                }
                if (prioritySets[levelLayer.LayerPrioritySet].SubscreenL2 && state.Layer2 && state.Priority1)
                {
                    tile = GetTileFromPriorityArray(layer2Priority1, x, y);
                    CopySingleTileToArray(subscreenPixels, tile, 2048, x, y);
                    ClearArray(tile);
                }
                if (prioritySets[levelLayer.LayerPrioritySet].SubscreenL1 && state.Layer1 && state.Priority1)
                {
                    tile = GetTileFromPriorityArray(layer1Priority1, x, y);
                    CopySingleTileToArray(subscreenPixels, tile, 2048, x, y);
                    ClearArray(tile);
                }
                if (prioritySets[levelLayer.LayerPrioritySet].SubscreenL3 && state.Layer3)
                {
                    tile = GetTileFromPriorityArray(layer3Priority0, x, y);
                    CopySingleTileToArray(subscreenPixels, tile, 2048, x, y);
                    ClearArray(tile);
                }
                if (prioritySets[levelLayer.LayerPrioritySet].SubscreenL3 && state.Layer3 && state.Priority1)
                {
                    tile = GetTileFromPriorityArray(layer3Priority1, x, y);
                    CopySingleTileToArray(subscreenPixels, tile, 2048, x, y);
                    ClearArray(tile);
                }
            }
            else if (!levelLayer.TopPriorityL3) //[3,0][3,1][2,0][1,0][2,1][1,1]
            {
                if (prioritySets[levelLayer.LayerPrioritySet].SubscreenL3 && state.Layer3)
                {
                    tile = GetTileFromPriorityArray(layer3Priority0, x, y);
                    CopySingleTileToArray(subscreenPixels, tile, 2048, x, y);
                    ClearArray(tile);
                }
                if (prioritySets[levelLayer.LayerPrioritySet].SubscreenL3 && state.Layer3 && state.Priority1)
                {
                    tile = GetTileFromPriorityArray(layer3Priority1, x, y);
                    CopySingleTileToArray(subscreenPixels, tile, 2048, x, y);
                    ClearArray(tile);
                }
                if (prioritySets[levelLayer.LayerPrioritySet].SubscreenL2 && state.Layer2)
                {
                    tile = GetTileFromPriorityArray(layer2Priority0, x, y);
                    CopySingleTileToArray(subscreenPixels, tile, 2048, x, y);
                    ClearArray(tile);
                }
                if (prioritySets[levelLayer.LayerPrioritySet].SubscreenL1 && state.Layer1)
                {
                    tile = GetTileFromPriorityArray(layer1Priority0, x, y);
                    CopySingleTileToArray(subscreenPixels, tile, 2048, x, y);
                    ClearArray(tile);
                }
                if (prioritySets[levelLayer.LayerPrioritySet].SubscreenL2 && state.Layer2 && state.Priority1)
                {
                    tile = GetTileFromPriorityArray(layer2Priority1, x, y);
                    CopySingleTileToArray(subscreenPixels, tile, 2048, x, y);
                    ClearArray(tile);
                }
                if (prioritySets[levelLayer.LayerPrioritySet].SubscreenL1 && state.Layer1 && state.Priority1)
                {
                    tile = GetTileFromPriorityArray(layer1Priority1, x, y);
                    CopySingleTileToArray(subscreenPixels, tile, 2048, x, y);
                    ClearArray(tile);
                }
            }
        }
        private void CopySingleTileToArray(int[] dest, int[] source, int width, int x, int y)
        {
            int counter = 0;
            for (int i = 0; i < 256; i++)
            {
                if (source[i] != 0)
                    dest[y * width + x + counter] = source[i];

                counter++;
                if (counter % 16 == 0)
                {
                    y++;
                    counter = 0;
                }
            }

        }
        private void DrawSingleMainscreenTile(int x, int y)
        {
            int bgcolor = paletteSet.GetBGColor();
            ClearArray(tile);
            ClearArray(tileColorMath);
            if (HaveSubscreen())
            {
                if (prioritySets[levelLayer.LayerPrioritySet].ColorMathBG && state.BG)
                {
                    //for (int i = 0; i < 256; i++)
                    //    tileColorMath[i] = bgcolor;
                    DoColorMathOnSingleTile(colorMath, x, y);
                    CopySingleTileToArray(mainscreen, tileColorMath, 2048, x, y);
                    ClearArray(tileColorMath);
                }
                else if (state.BG)
                {
                    //for (int i = 0; i < 256; i++)
                    //    tileColorMath[i] = bgcolor;
                    CopySingleTileToArray(mainscreen, tileColorMath, 2048, x, y);
                    ClearArray(tileColorMath);
                }

                if (levelLayer.TopPriorityL3) // [3,0][2,0][1,0][2,1][1,1][3,1]
                {
                    if (prioritySets[levelLayer.LayerPrioritySet].MainscreenL2 && state.Layer2)
                    {
                        tileColorMath = GetTileFromPriorityArray(layer2Priority0, x, y);
                        if (prioritySets[levelLayer.LayerPrioritySet].ColorMathL2)
                            DoColorMathOnSingleTile(tileColorMath, x, y);
                        CopySingleTileToArray(mainscreen, tileColorMath, 2048, x, y);
                        ClearArray(tileColorMath);
                    }
                    if (prioritySets[levelLayer.LayerPrioritySet].MainscreenL1 && state.Layer1)
                    {
                        tileColorMath = GetTileFromPriorityArray(layer1Priority0, x, y);
                        if (prioritySets[levelLayer.LayerPrioritySet].ColorMathL1)
                            DoColorMathOnSingleTile(tileColorMath, x, y);
                        CopySingleTileToArray(mainscreen, tileColorMath, 2048, x, y);
                        ClearArray(tileColorMath);
                    }
                    if (prioritySets[levelLayer.LayerPrioritySet].MainscreenL2 && state.Layer2 && state.Priority1)
                    {
                        tileColorMath = GetTileFromPriorityArray(layer2Priority1, x, y);
                        if (prioritySets[levelLayer.LayerPrioritySet].ColorMathL2)
                            DoColorMathOnSingleTile(tileColorMath, x, y);
                        CopySingleTileToArray(mainscreen, tileColorMath, 2048, x, y);
                        ClearArray(tileColorMath);
                    }
                    if (prioritySets[levelLayer.LayerPrioritySet].MainscreenL1 && state.Layer1 && state.Priority1)
                    {
                        tileColorMath = GetTileFromPriorityArray(layer1Priority1, x, y);
                        if (prioritySets[levelLayer.LayerPrioritySet].ColorMathL1)
                            DoColorMathOnSingleTile(tileColorMath, x, y);
                        CopySingleTileToArray(mainscreen, tileColorMath, 2048, x, y);
                        ClearArray(tileColorMath);
                    }
                    if (prioritySets[levelLayer.LayerPrioritySet].MainscreenL3 && state.Layer3)
                    {
                        tileColorMath = GetTileFromPriorityArray(layer3Priority0, x, y);
                        if (prioritySets[levelLayer.LayerPrioritySet].ColorMathL3)
                            DoColorMathOnSingleTile(tileColorMath, x, y);
                        CopySingleTileToArray(mainscreen, tileColorMath, 2048, x, y);
                        ClearArray(tileColorMath);
                    }
                    if (prioritySets[levelLayer.LayerPrioritySet].MainscreenL3 && state.Layer3 && state.Priority1)
                    {
                        tileColorMath = GetTileFromPriorityArray(layer3Priority1, x, y);
                        if (prioritySets[levelLayer.LayerPrioritySet].ColorMathL3)
                            DoColorMathOnSingleTile(tileColorMath, x, y);
                        CopySingleTileToArray(mainscreen, tileColorMath, 2048, x, y);
                        ClearArray(tileColorMath);
                    }
                }
                else if (!levelLayer.TopPriorityL3) // [3,0][3,1][2,0][1,0][2,1][1,1]
                {
                    if (prioritySets[levelLayer.LayerPrioritySet].MainscreenL3 && state.Layer3)
                    {
                        tileColorMath = GetTileFromPriorityArray(layer3Priority0, x, y);
                        if (prioritySets[levelLayer.LayerPrioritySet].ColorMathL3)
                            DoColorMathOnSingleTile(tileColorMath, x, y);
                        CopySingleTileToArray(mainscreen, tileColorMath, 2048, x, y);
                        ClearArray(tileColorMath);
                    }
                    if (prioritySets[levelLayer.LayerPrioritySet].MainscreenL3 && state.Layer3 && state.Priority1)
                    {
                        tileColorMath = GetTileFromPriorityArray(layer3Priority1, x, y);
                        if (prioritySets[levelLayer.LayerPrioritySet].ColorMathL3)
                            DoColorMathOnSingleTile(tileColorMath, x, y);
                        CopySingleTileToArray(mainscreen, tileColorMath, 2048, x, y);
                        ClearArray(tileColorMath);
                    }
                    if (prioritySets[levelLayer.LayerPrioritySet].MainscreenL2 && state.Layer2)
                    {
                        tileColorMath = GetTileFromPriorityArray(layer2Priority0, x, y);
                        if (prioritySets[levelLayer.LayerPrioritySet].ColorMathL2)
                            DoColorMathOnSingleTile(tileColorMath, x, y);
                        CopySingleTileToArray(mainscreen, tileColorMath, 2048, x, y);
                        ClearArray(tileColorMath);
                    }
                    if (prioritySets[levelLayer.LayerPrioritySet].MainscreenL1 && state.Layer1)
                    {
                        tileColorMath = GetTileFromPriorityArray(layer1Priority0, x, y);
                        if (prioritySets[levelLayer.LayerPrioritySet].ColorMathL1)
                            DoColorMathOnSingleTile(tileColorMath, x, y);
                        CopySingleTileToArray(mainscreen, tileColorMath, 2048, x, y);
                        ClearArray(tileColorMath);
                    }
                    if (prioritySets[levelLayer.LayerPrioritySet].MainscreenL2 && state.Layer2 && state.Priority1)
                    {
                        tileColorMath = GetTileFromPriorityArray(layer2Priority1, x, y);
                        if (prioritySets[levelLayer.LayerPrioritySet].ColorMathL2)
                            DoColorMathOnSingleTile(tileColorMath, x, y);
                        CopySingleTileToArray(mainscreen, tileColorMath, 2048, x, y);
                        ClearArray(tileColorMath);
                    }
                    if (prioritySets[levelLayer.LayerPrioritySet].MainscreenL1 && state.Layer1 && state.Priority1)
                    {
                        tileColorMath = GetTileFromPriorityArray(layer1Priority1, x, y);
                        if (prioritySets[levelLayer.LayerPrioritySet].ColorMathL1)
                            DoColorMathOnSingleTile(tileColorMath, x, y);
                        CopySingleTileToArray(mainscreen, tileColorMath, 2048, x, y);
                        ClearArray(tileColorMath);
                    }
                }
            }
            else // No color math, we can go ahead and draw the mainscreen
            {
                if (levelLayer.TopPriorityL3) // [3,0][2,0][1,0][2,1][1,1][3,1]
                {
                    if (prioritySets[levelLayer.LayerPrioritySet].MainscreenL2 && state.Layer2)
                        CopySingleTileToArray(mainscreen, GetTileFromPriorityArray(layer2Priority0, x, y), 2048, x, y);
                    if (prioritySets[levelLayer.LayerPrioritySet].MainscreenL1 && state.Layer1)
                        CopySingleTileToArray(mainscreen, GetTileFromPriorityArray(layer1Priority0, x, y), 2048, x, y);
                    if (prioritySets[levelLayer.LayerPrioritySet].MainscreenL2 && state.Layer2 && state.Priority1)
                        CopySingleTileToArray(mainscreen, GetTileFromPriorityArray(layer2Priority1, x, y), 2048, x, y);
                    if (prioritySets[levelLayer.LayerPrioritySet].MainscreenL1 && state.Layer1 && state.Priority1)
                        CopySingleTileToArray(mainscreen, GetTileFromPriorityArray(layer1Priority1, x, y), 2048, x, y);
                    if (prioritySets[levelLayer.LayerPrioritySet].MainscreenL3 && state.Layer3)
                        CopySingleTileToArray(mainscreen, GetTileFromPriorityArray(layer3Priority0, x, y), 2048, x, y);
                    if (prioritySets[levelLayer.LayerPrioritySet].MainscreenL3 && state.Layer3 && state.Priority1)
                        CopySingleTileToArray(mainscreen, GetTileFromPriorityArray(layer3Priority1, x, y), 2048, x, y);
                }
                else if (!levelLayer.TopPriorityL3) // [3,0][3,1][2,0][1,0][2,1][1,1]
                {
                    if (prioritySets[levelLayer.LayerPrioritySet].MainscreenL3 && state.Layer3)
                        CopySingleTileToArray(mainscreen, GetTileFromPriorityArray(layer3Priority0, x, y), 2048, x, y);
                    if (prioritySets[levelLayer.LayerPrioritySet].MainscreenL3 && state.Layer3 && state.Priority1)
                        CopySingleTileToArray(mainscreen, GetTileFromPriorityArray(layer3Priority1, x, y), 2048, x, y);
                    if (prioritySets[levelLayer.LayerPrioritySet].MainscreenL2 && state.Layer2)
                        CopySingleTileToArray(mainscreen, GetTileFromPriorityArray(layer2Priority0, x, y), 2048, x, y);
                    if (prioritySets[levelLayer.LayerPrioritySet].MainscreenL1 && state.Layer1)
                        CopySingleTileToArray(mainscreen, GetTileFromPriorityArray(layer1Priority0, x, y), 2048, x, y);
                    if (prioritySets[levelLayer.LayerPrioritySet].MainscreenL2 && state.Layer2 && state.Priority1)
                        CopySingleTileToArray(mainscreen, GetTileFromPriorityArray(layer2Priority1, x, y), 2048, x, y);
                    if (prioritySets[levelLayer.LayerPrioritySet].MainscreenL1 && state.Layer1 && state.Priority1)
                        CopySingleTileToArray(mainscreen, GetTileFromPriorityArray(layer1Priority1, x, y), 2048, x, y);
                }

                // Apply BG color
                if (state.BG)
                {
                    for (int i = 0; i < 2048 * 2048; i++)
                    {
                        if (mainscreen[i] == 0)
                            mainscreen[i] = bgcolor;
                    }
                }
            }
        }
        private int[] GetTileFromPriorityArray(int[] arr, int x, int y)
        {
            int counter = 0;
            for (int i = 0; i < 256; i++)
            {
                if (arr[y * 2048 + x + counter] != 0)
                    tile[i] = arr[y * 2048 + x + counter];

                counter++;
                if (counter % 16 == 0)
                {
                    y++;
                    counter = 0;
                }
            }
            return tile;

        }
        private void DoColorMathOnSingleTile(int[] tile, int x, int y)
        {
            // NEW CODE TEST
            int r, g, b;

            for (int w = 0; w < 16; w++)
            {
                for (int v = 0; v < 16; v++)
                {
                    if (subscreenPixels[(y + w) * 2048 + (x + v)] != 0 && tile[w * 16 + v] != 0)
                    {
                        r = Color.FromArgb(tile[w * 16 + v]).R;
                        g = Color.FromArgb(tile[w * 16 + v]).G;
                        b = Color.FromArgb(tile[w * 16 + v]).B;

                        if (prioritySets[levelLayer.LayerPrioritySet].ColorMathMinusSubscreen == 0)
                        {
                            if (prioritySets[levelLayer.LayerPrioritySet].ColorMathHalfIntensity == 1)
                            {
                                r /= 2; g /= 2; b /= 2;
                                r += Color.FromArgb(subscreenPixels[(y + w) * 2048 + (x + v)]).R / 2;
                                g += Color.FromArgb(subscreenPixels[(y + w) * 2048 + (x + v)]).G / 2;
                                b += Color.FromArgb(subscreenPixels[(y + w) * 2048 + (x + v)]).B / 2;
                            }
                            else
                            {
                                r += Color.FromArgb(subscreenPixels[(y + w) * 2048 + (x + v)]).R;
                                g += Color.FromArgb(subscreenPixels[(y + w) * 2048 + (x + v)]).G;
                                b += Color.FromArgb(subscreenPixels[(y + w) * 2048 + (x + v)]).B;
                            }

                            if (r > 255) r = 255; if (g > 255) g = 255; if (b > 255) b = 255;
                        }
                        else if (prioritySets[levelLayer.LayerPrioritySet].ColorMathMinusSubscreen == 1)
                        {
                            if (prioritySets[levelLayer.LayerPrioritySet].ColorMathHalfIntensity == 1)
                            {
                                r /= 2; g /= 2; b /= 2;
                                r -= Color.FromArgb(subscreenPixels[(y + w) * 2048 + (x + v)]).R / 2;
                                g -= Color.FromArgb(subscreenPixels[(y + w) * 2048 + (x + v)]).G / 2;
                                b -= Color.FromArgb(subscreenPixels[(y + w) * 2048 + (x + v)]).B / 2;
                            }
                            else
                            {
                                r -= Color.FromArgb(subscreenPixels[(y + w) * 2048 + (x + v)]).R;
                                g -= Color.FromArgb(subscreenPixels[(y + w) * 2048 + (x + v)]).G;
                                b -= Color.FromArgb(subscreenPixels[(y + w) * 2048 + (x + v)]).B;
                            }

                            if (r < 0) r = 0; if (g < 0) g = 0; if (b < 0) b = 0;
                        }

                        tile[w * 16 + v] = Color.FromArgb(255, r, g, b).ToArgb();

                    }
                }
            }
        }
        private bool HaveSubscreen()
        {
            if ((prioritySets[levelLayer.LayerPrioritySet].SubscreenL1 && state.Layer1) ||
                (prioritySets[levelLayer.LayerPrioritySet].SubscreenL2 && state.Layer2) ||
                (prioritySets[levelLayer.LayerPrioritySet].SubscreenL3 && state.Layer3) ||
                (prioritySets[levelLayer.LayerPrioritySet].SubscreenOBJ && state.Objects))
                return true;

            return false;
        }

        private void CopyToPixelArray(int[] dest, int[] source)
        {
            //int bgcolor = paletteSet.GetBGColor();
            Size s = FindMaxSize();
            for (int y = 0; y < s.Height; y++)
            {
                for (int x = 0; x < s.Width; x++)
                {
                    if (source[y * 2048 + x] != 0)
                        dest[y * 2048 + x] = source[y * 2048 + x];
                    //else
                    //    dest[y * 2048 + x] = bgcolor;
                }
            }
        }

        private void ClearArray(IList arr)
        {
            if (arr == null) return;
            arr.Clear();
        }

        private Size FindMaxSize()
        {
            int w = levelLayer.LayerWidth[0];
            int h = levelLayer.LayerHeight[0];
            for (int i = 1; i < 3; i++)
            {
                if (levelLayer.LayerWidth[i] > w)
                    w = levelLayer.LayerWidth[i];
                if (levelLayer.LayerHeight[i] > h)
                    h = levelLayer.LayerHeight[i];
            }
            return new Size((int)(256 * Math.Pow(2, w)), (int)(256 * Math.Pow(2, h)));
        }

        private void CreateMainscreen()
        {
            //int bgcolor = paletteSet.GetBGColor();
            if (HaveSubscreen()) // We are doing color math by the layer
            {
                if (colorMath == null)
                    colorMath = new int[2048 * 2048];
                else
                    ClearArray(colorMath);

                Size s = FindMaxSize();
                if (prioritySets[levelLayer.LayerPrioritySet].ColorMathBG && state.BG)
                {
                    //for (int y = 0; y < s.Height; y++)
                    //{
                    //    for (int x = 0; x < s.Width; x++)
                    //        colorMath[y * 2048 + x] = bgcolor;
                    //}
                    DoColorMath(colorMath);
                    CopyToPixelArray(mainscreen, colorMath);
                    ClearArray(colorMath);
                }
                else if (state.BG)
                {
                    //for (int y = 0; y < s.Height; y++)
                    //{
                    //    for (int x = 0; x < s.Width; x++)
                    //        mainscreen[y * 2048 + x] = bgcolor;
                    //}
                }

                if (levelLayer.TopPriorityL3) // [3,0][2,0][1,0][2,1][1,1][3,1]
                {
                    if (prioritySets[levelLayer.LayerPrioritySet].MainscreenL2 && state.Layer2)
                    {
                        CopyToPixelArray(colorMath, layer2Priority0);
                        if (prioritySets[levelLayer.LayerPrioritySet].ColorMathL2)
                            DoColorMath(colorMath);
                        CopyToPixelArray(mainscreen, colorMath);
                        ClearArray(colorMath);
                    }
                    if (prioritySets[levelLayer.LayerPrioritySet].MainscreenL1 && state.Layer1)
                    {
                        CopyToPixelArray(colorMath, layer1Priority0);
                        if (prioritySets[levelLayer.LayerPrioritySet].ColorMathL1)
                            DoColorMath(colorMath);
                        CopyToPixelArray(mainscreen, colorMath);
                        ClearArray(colorMath);
                    }
                    if (prioritySets[levelLayer.LayerPrioritySet].MainscreenL2 && state.Layer2 && state.Priority1)
                    {
                        CopyToPixelArray(colorMath, layer2Priority1);
                        if (prioritySets[levelLayer.LayerPrioritySet].ColorMathL2)
                            DoColorMath(colorMath);
                        CopyToPixelArray(mainscreen, colorMath);
                        ClearArray(colorMath);
                    }
                    if (prioritySets[levelLayer.LayerPrioritySet].MainscreenL1 && state.Layer1 && state.Priority1)
                    {
                        CopyToPixelArray(colorMath, layer1Priority1);
                        if (prioritySets[levelLayer.LayerPrioritySet].ColorMathL1)
                            DoColorMath(colorMath);
                        CopyToPixelArray(mainscreen, colorMath);
                        ClearArray(colorMath);
                    }
                    if (prioritySets[levelLayer.LayerPrioritySet].MainscreenL3 && state.Layer3)
                    {
                        CopyToPixelArray(colorMath, layer3Priority0);
                        if (prioritySets[levelLayer.LayerPrioritySet].ColorMathL3)
                            DoColorMath(colorMath);
                        CopyToPixelArray(mainscreen, colorMath);
                        ClearArray(colorMath);
                    }
                    if (prioritySets[levelLayer.LayerPrioritySet].MainscreenL3 && state.Layer3 && state.Priority1)
                    {
                        CopyToPixelArray(colorMath, layer3Priority1);
                        if (prioritySets[levelLayer.LayerPrioritySet].ColorMathL3)
                            DoColorMath(colorMath);
                        CopyToPixelArray(mainscreen, colorMath);
                        ClearArray(colorMath);
                    }
                }
                else if (!levelLayer.TopPriorityL3) // [3,0][3,1][2,0][1,0][2,1][1,1]
                {
                    if (prioritySets[levelLayer.LayerPrioritySet].MainscreenL3 && state.Layer3)
                    {
                        CopyToPixelArray(colorMath, layer3Priority0);
                        if (prioritySets[levelLayer.LayerPrioritySet].ColorMathL3)
                            DoColorMath(colorMath);
                        CopyToPixelArray(mainscreen, colorMath);
                        ClearArray(colorMath);
                    }
                    if (prioritySets[levelLayer.LayerPrioritySet].MainscreenL3 && state.Layer3 && state.Priority1)
                    {
                        CopyToPixelArray(colorMath, layer3Priority1);
                        if (prioritySets[levelLayer.LayerPrioritySet].ColorMathL3)
                            DoColorMath(colorMath);
                        CopyToPixelArray(mainscreen, colorMath);
                        ClearArray(colorMath);
                    }
                    if (prioritySets[levelLayer.LayerPrioritySet].MainscreenL2 && state.Layer2)
                    {
                        CopyToPixelArray(colorMath, layer2Priority0);
                        if (prioritySets[levelLayer.LayerPrioritySet].ColorMathL2)
                            DoColorMath(colorMath);
                        CopyToPixelArray(mainscreen, colorMath);
                        ClearArray(colorMath);
                    }
                    if (prioritySets[levelLayer.LayerPrioritySet].MainscreenL1 && state.Layer1)
                    {
                        CopyToPixelArray(colorMath, layer1Priority0);
                        if (prioritySets[levelLayer.LayerPrioritySet].ColorMathL1)
                            DoColorMath(colorMath);
                        CopyToPixelArray(mainscreen, colorMath);
                        ClearArray(colorMath);
                    }
                    if (prioritySets[levelLayer.LayerPrioritySet].MainscreenL2 && state.Layer2 && state.Priority1)
                    {
                        CopyToPixelArray(colorMath, layer2Priority1);
                        if (prioritySets[levelLayer.LayerPrioritySet].ColorMathL2)
                            DoColorMath(colorMath);
                        CopyToPixelArray(mainscreen, colorMath);
                        ClearArray(colorMath);
                    }
                    if (prioritySets[levelLayer.LayerPrioritySet].MainscreenL1 && state.Layer1 && state.Priority1)
                    {
                        CopyToPixelArray(colorMath, layer1Priority1);
                        if (prioritySets[levelLayer.LayerPrioritySet].ColorMathL1)
                            DoColorMath(colorMath);
                        CopyToPixelArray(mainscreen, colorMath);
                        ClearArray(colorMath);
                    }
                }

            }
            else // No color math, we can go ahead and draw the mainscreen
            {
                if (levelLayer.TopPriorityL3) // [3,0][2,0][1,0][2,1][1,1][3,1]
                {
                    if (prioritySets[levelLayer.LayerPrioritySet].MainscreenL2 && state.Layer2)
                        CopyToPixelArray(mainscreen, layer2Priority0);
                    if (prioritySets[levelLayer.LayerPrioritySet].MainscreenL1 && state.Layer1)
                        CopyToPixelArray(mainscreen, layer1Priority0);
                    if (prioritySets[levelLayer.LayerPrioritySet].MainscreenL2 && state.Layer2 && state.Priority1)
                        CopyToPixelArray(mainscreen, layer2Priority1);
                    if (prioritySets[levelLayer.LayerPrioritySet].MainscreenL1 && state.Layer1 && state.Priority1)
                        CopyToPixelArray(mainscreen, layer1Priority1);
                    if (prioritySets[levelLayer.LayerPrioritySet].MainscreenL3 && state.Layer3)
                        CopyToPixelArray(mainscreen, layer3Priority0);
                    if (prioritySets[levelLayer.LayerPrioritySet].MainscreenL3 && state.Layer3 && state.Priority1)
                        CopyToPixelArray(mainscreen, layer3Priority1);
                }
                else if (!levelLayer.TopPriorityL3) // [3,0][3,1][2,0][1,0][2,1][1,1]
                {
                    if (prioritySets[levelLayer.LayerPrioritySet].MainscreenL3 && state.Layer3)
                        CopyToPixelArray(mainscreen, layer3Priority0);
                    if (prioritySets[levelLayer.LayerPrioritySet].MainscreenL3 && state.Layer3 && state.Priority1)
                        CopyToPixelArray(mainscreen, layer3Priority1);
                    if (prioritySets[levelLayer.LayerPrioritySet].MainscreenL2 && state.Layer2)
                        CopyToPixelArray(mainscreen, layer2Priority0);
                    if (prioritySets[levelLayer.LayerPrioritySet].MainscreenL1 && state.Layer1)
                        CopyToPixelArray(mainscreen, layer1Priority0);
                    if (prioritySets[levelLayer.LayerPrioritySet].MainscreenL2 && state.Layer2 && state.Priority1)
                        CopyToPixelArray(mainscreen, layer2Priority1);
                    if (prioritySets[levelLayer.LayerPrioritySet].MainscreenL1 && state.Layer1 && state.Priority1)
                        CopyToPixelArray(mainscreen, layer1Priority1);
                }
            }
        }

        private void CreateLayer(int layer)
        {
            int offset = 0;
            byte tileNum;

            layers[layer] = new Tile16x16[128 * 128]; // Create our layer here

            Size s = new Size();
            s.Width = (int)(256 * Math.Pow(2, levelLayer.LayerWidth[layer]) / 16);
            s.Height = (int)(256 * Math.Pow(2, levelLayer.LayerHeight[layer]) / 16);
            int temp = 0;
            int i = 0;
            for (int y = 0; y < s.Height; y++)
            {
                for (int x = 0; x < s.Width; x++)
                {
                    i = y * s.Width + x;

                    tileNum = ByteManage.GetByte(tileMaps[layer], offset); offset++;
                    if (layer == 2)
                    {
                        temp = tileNum;
                        tileNum &= 0x3F;

                        layers[layer][i] = new Tile16x16(tileNum);

                        for (int a = 0; a < 4; a++)
                            layers[layer][i].SetSubtile(tileset.TileSetLayers[layer][tileNum].GetSubtile(a), a);

                        layers[layer][i].Mirrored = (temp & 0x40) == 0x40;
                        layers[layer][i].Inverted = (temp & 0x80) == 0x80;
                    }
                    else
                        layers[layer][i] = tileset.TileSetLayers[layer][tileNum];
                }
            }
        }
        private void DecompressLevelData()
        {
            tileMaps[0] = model.TileMaps[levelLayer.TileMapL1];
            tileMaps[1] = model.TileMaps[levelLayer.TileMapL2];
            tileMaps[2] = model.TileMaps[levelLayer.TileMapL3];
        }
        private void DoColorMath(int[] layer)
        {
            // NEW CODE TEST
            int r, g, b;
            //int bgcolor = paletteSet.GetBGColor();

            Size s = FindMaxSize();

            for (int y = 0; y < s.Height; y++)
            {
                for (int x = 0; x < s.Width; x++)
                {
                    if (subscreenPixels[y * 2048 + x] != 0 && layer[y * 2048 + x] != 0)
                    {
                        r = Color.FromArgb(layer[y * 2048 + x]).R;
                        g = Color.FromArgb(layer[y * 2048 + x]).G;
                        b = Color.FromArgb(layer[y * 2048 + x]).B;

                        if (prioritySets[levelLayer.LayerPrioritySet].ColorMathMinusSubscreen == 0)
                        {
                            if (prioritySets[levelLayer.LayerPrioritySet].ColorMathHalfIntensity == 1)
                            {
                                r /= 2; g /= 2; b /= 2;
                                r += Color.FromArgb(subscreenPixels[y * 2048 + x]).R / 2;
                                g += Color.FromArgb(subscreenPixels[y * 2048 + x]).G / 2;
                                b += Color.FromArgb(subscreenPixels[y * 2048 + x]).B / 2;
                            }
                            else
                            {
                                r += Color.FromArgb(subscreenPixels[y * 2048 + x]).R;
                                g += Color.FromArgb(subscreenPixels[y * 2048 + x]).G;
                                b += Color.FromArgb(subscreenPixels[y * 2048 + x]).B;
                            }

                            if (r > 255) r = 255; if (g > 255) g = 255; if (b > 255) b = 255;
                        }
                        else if (prioritySets[levelLayer.LayerPrioritySet].ColorMathMinusSubscreen == 1)
                        {
                            if (prioritySets[levelLayer.LayerPrioritySet].ColorMathHalfIntensity == 1)
                            {
                                r /= 2; g /= 2; b /= 2;
                                r -= Color.FromArgb(subscreenPixels[y * 2048 + x]).R / 2;
                                g -= Color.FromArgb(subscreenPixels[y * 2048 + x]).G / 2;
                                b -= Color.FromArgb(subscreenPixels[y * 2048 + x]).B / 2;
                            }
                            else
                            {
                                r -= Color.FromArgb(subscreenPixels[y * 2048 + x]).R;
                                g -= Color.FromArgb(subscreenPixels[y * 2048 + x]).G;
                                b -= Color.FromArgb(subscreenPixels[y * 2048 + x]).B;
                            }

                            if (r < 0) r = 0; if (g < 0) g = 0; if (b < 0) b = 0;
                        }

                        layer[y * 2048 + x] = Color.FromArgb(255, r, g, b).ToArgb();

                    }
                }
            }
        }

        private int[] DrawLayerByPriorityOne(int[] dest, int layer, bool priority)
        {
            if (dest.Length != 2048 * 2048 || layers[layer] == null)
                return null;

            Size s = new Size();
            s.Width = (int)(256 * Math.Pow(2, levelLayer.LayerWidth[layer]) / 16);
            s.Height = (int)(256 * Math.Pow(2, levelLayer.LayerHeight[layer]) / 16);

            for (int y = 0; y < s.Height; y++)
            {
                for (int x = 0; x < s.Width; x++)
                    CopyOverTile16x16(layers[layer][y * s.Width + x], dest, 2048, x * 16, y * 16, priority);
            }

            return dest;
        }
        private void CreateSubscreen()
        {
            // 2 possible cases
            if (levelLayer.TopPriorityL3) //[3,0][2,0][1,0][2,1][1,1][3,1]
            {
                if (prioritySets[levelLayer.LayerPrioritySet].SubscreenL2 && state.Layer2)
                    CopyToPixelArray(subscreenPixels, layer2Priority0);//DrawLayerByPriorityOne(subscreenPixels, 1, false);
                if (prioritySets[levelLayer.LayerPrioritySet].SubscreenL1 && state.Layer1)
                    CopyToPixelArray(subscreenPixels, layer1Priority0);//DrawLayerByPriorityOne(subscreenPixels, 0, false);
                if (prioritySets[levelLayer.LayerPrioritySet].SubscreenL2 && state.Layer2)
                    CopyToPixelArray(subscreenPixels, layer2Priority1);//DrawLayerByPriorityOne(subscreenPixels, 1, true);
                if (prioritySets[levelLayer.LayerPrioritySet].SubscreenL1 && state.Layer1)
                    CopyToPixelArray(subscreenPixels, layer1Priority1);//DrawLayerByPriorityOne(subscreenPixels, 0, true);
                if (prioritySets[levelLayer.LayerPrioritySet].SubscreenL3 && state.Layer3)
                    CopyToPixelArray(subscreenPixels, layer3Priority0);//DrawLayerByPriorityOne(subscreenPixels, 2, false);
                if (prioritySets[levelLayer.LayerPrioritySet].SubscreenL3 && state.Layer3)
                    CopyToPixelArray(subscreenPixels, layer3Priority1);//DrawLayerByPriorityOne(subscreenPixels, 2, true);


            }
            else if (!levelLayer.TopPriorityL3) //[3,0][3,1][2,0][1,0][2,1][1,1]
            {
                if (prioritySets[levelLayer.LayerPrioritySet].SubscreenL3 && state.Layer3)
                    CopyToPixelArray(subscreenPixels, layer3Priority0);//DrawLayerByPriorityOne(subscreenPixels, 2, false);
                if (prioritySets[levelLayer.LayerPrioritySet].SubscreenL3 && state.Layer3)
                    CopyToPixelArray(subscreenPixels, layer3Priority1);//DrawLayerByPriorityOne(subscreenPixels, 2, true);
                if (prioritySets[levelLayer.LayerPrioritySet].SubscreenL2 && state.Layer2)
                    CopyToPixelArray(subscreenPixels, layer2Priority0);//DrawLayerByPriorityOne(subscreenPixels, 1, false);
                if (prioritySets[levelLayer.LayerPrioritySet].SubscreenL1 && state.Layer1)
                    CopyToPixelArray(subscreenPixels, layer1Priority0);//DrawLayerByPriorityOne(subscreenPixels, 0, false);
                if (prioritySets[levelLayer.LayerPrioritySet].SubscreenL2 && state.Layer2)
                    CopyToPixelArray(subscreenPixels, layer2Priority1);//DrawLayerByPriorityOne(subscreenPixels, 1, true);
                if (prioritySets[levelLayer.LayerPrioritySet].SubscreenL1 && state.Layer1)
                    CopyToPixelArray(subscreenPixels, layer1Priority1);//DrawLayerByPriorityOne(subscreenPixels, 0, true);
            }


        }
        private void CopyOverTile8x8(Tile8x8 source, int[] dest, int destinationWidth, int x, int y)
        {
            int[] src = new int[source.Pixels.Length];
            source.Pixels.CopyTo(src, 0);

            int counter = 0;
            for (int i = 0; i < 64; i++)
            {
                if (src[i] != 0)
                    dest[y * destinationWidth + x + counter] = src[i];
                counter++;
                if (counter % 8 == 0)
                {
                    y++;
                    counter = 0;
                }
            }
        }
        private void CopyOverTile16x16(Tile16x16 source, int[] dest, int destinationWidth, int x, int y, bool priority)
        {
            int[] pixels = new int[16 * 16];

            for (int a = 0; a < 4; a++)
            {
                if (source.GetPriority1(a) == priority)
                    CopyOverTile8x8(source.GetSubtile(a), pixels, 16, a % 2 * 8, a / 2 * 8);
            }

            if (source.Mirrored)
                MirrorTile16x16(pixels);
            if (source.Inverted)
                InvertTile16x16(pixels);

            for (int b = 0; b < 16; b++)
            {
                for (int a = 0; a < 16; a++)
                {
                    if (pixels[b * 16 + a] != 0)
                        dest[(b + y) * destinationWidth + a + x] = pixels[b * 16 + a];
                }
            }
        }

        private void MirrorTile16x16(int[] src)
        {
            int temp = 0;

            for (int y = 0; y < 16; y++)
            {
                for (int a = 0, b = 15; a < 8; a++, b--)
                {
                    temp = src[(y * 16) + a];
                    src[(y * 16) + a] = src[(y * 16) + b];
                    src[(y * 16) + b] = temp;
                }
            }
        }
        private void InvertTile16x16(int[] src)
        {
            int temp = 0;

            for (int x = 0; x < 16; x++)
            {
                for (int a = 0, b = 15; a < 8; a++, b--)
                {
                    temp = src[(a * 16) + x];
                    src[(a * 16) + x] = src[(b * 16) + x];
                    src[(b * 16) + x] = temp;
                }
            }
        }

        public void MakeEdit(int tileNum, int layer, int x, int y)
        {
            // x and y are in pixel format
            Math.Min(2047, Math.Max(0, x));
            Math.Min(2047, Math.Max(0, y));
            y /= 16;
            x /= 16;

            Size s = new Size();
            s.Width = (int)(256 * Math.Pow(2, levelLayer.LayerWidth[layer]) / 16);
            s.Height = (int)(256 * Math.Pow(2, levelLayer.LayerHeight[layer]) / 16);

            int tile = y * s.Width + x;
            try
            {
                if (x >= 0 && y >= 0 && tile < 0x4000)
                    ChangeSingleTile(layer, tile, tileNum, x * 16, y * 16);
            }
            catch
            {
                // invalid layer/tile
                System.Windows.Forms.MessageBox.Show("MAKE EDIT PROBLEM");
            }
        }
        public int GetTileNum(int layer, int x, int y)
        {
            Math.Min(2047, Math.Max(0, x));
            Math.Min(2047, Math.Max(0, y));
            y /= 16;
            x /= 16;

            Size s = new Size();
            s.Width = (int)(256 * Math.Pow(2, levelLayer.LayerWidth[layer])); s.Width /= 16;
            s.Height = (int)(256 * Math.Pow(2, levelLayer.LayerHeight[layer])); s.Height /= 16;

            int placement = y * s.Width + x;

            if (layers[layer] != null)
                return layers[layer][placement].TileNumber;
            else return 0;
        }

        public void Clear(int count)
        {
            if (count == 1)
            {
                model.TileMaps[levelLayer.TileMapL1] = new byte[0x4000];
                model.TileMaps[levelLayer.TileMapL2] = new byte[0x4000];
                model.TileMaps[levelLayer.TileMapL3] = new byte[0x4000];
            }
            else
            {
                for (int i = 0; i < count; i++)
                {
                    model.TileMaps[i] = new byte[0x4000];
                }
            }
        }
        public int[] GetRangePixels(int layer, Point p, Size s)
        {
            int[] pixels = new int[s.Width * s.Height];

            switch (layer)
            {
                case 0:
                    for (int b = 0, y = p.Y; b < s.Height; b++, y++)
                    {
                        for (int a = 0, x = p.X; a < s.Width; a++, x++)
                        {
                            pixels[b * s.Width + a] = layer1Priority0[y * 2048 + x];
                            if (layer1Priority1[y * 2048 + x] != 0)
                                pixels[b * s.Width + a] = layer1Priority1[y * 2048 + x];
                        }
                    }
                    break;
                case 1:
                    for (int b = 0, y = p.Y; b < s.Height; b++, y++)
                    {
                        for (int a = 0, x = p.X; a < s.Width; a++, x++)
                        {
                            pixels[b * s.Width + a] = layer2Priority0[y * 2048 + x];
                            if (layer2Priority1[y * 2048 + x] != 0)
                                pixels[b * s.Width + a] = layer2Priority1[y * 2048 + x];
                        }
                    }
                    break;
                case 2:
                    for (int b = 0, y = p.Y; b < s.Height; b++, y++)
                    {
                        for (int a = 0, x = p.X; a < s.Width; a++, x++)
                        {
                            pixels[b * s.Width + a] = layer3Priority0[y * 2048 + x];
                            if (layer3Priority1[y * 2048 + x] != 0)
                                pixels[b * s.Width + a] = layer3Priority1[y * 2048 + x];
                        }
                    }
                    break;
                default:
                    goto case 0;
            }
            return pixels;
        }

        public void AssembleIntoModel()
        {
            int i = 0;
            Size s = new Size();
            for (int z = 0; z < 3; z++)
            {
                s.Width = (int)(256 * Math.Pow(2, levelLayer.LayerWidth[z]) / 16);
                s.Height = (int)(256 * Math.Pow(2, levelLayer.LayerHeight[z]) / 16);

                for (int y = 0; y < s.Height; y++)
                {
                    for (int x = 0; x < s.Width; x++)
                    {
                        i = y * s.Width + x;

                        tileMaps[z][i] = (byte)layers[z][i].TileNumber;

                        if (z == 2)
                        {
                            ByteManage.SetBit(tileMaps[z], i, 6, layers[z][i].Mirrored);
                            ByteManage.SetBit(tileMaps[z], i, 7, layers[z][i].Inverted);
                        }
                    }
                }
            }
        }

        public byte GetPhysicalTile(int layer, int x, int y, bool prop)
        {
            int tileNum = GetTileNum(layer, x, y);

            if (prop)
                return model.PhysicalMaps[levelLayer.PhysicalMap][tileNum + 256];
            else
                return model.PhysicalMaps[levelLayer.PhysicalMap][tileNum];
        }

        public int[] GetRangePixels(Point p, Size s)
        {
            int[] pixels = new int[s.Width * s.Height];

            for (int b = 0, y = p.Y; b < s.Height; b++, y++)
            {
                for (int a = 0, x = p.X; a < s.Width; a++, x++)
                {
                    pixels[b * s.Width + a] = Color.FromArgb(255, Color.FromArgb(mainscreen[y * 2048 + x])).ToArgb();
                }
            }

            return pixels;
        }
    }
}

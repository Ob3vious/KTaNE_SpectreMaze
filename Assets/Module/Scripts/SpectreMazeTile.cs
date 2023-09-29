﻿using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Reflection;
using UnityEngine.Rendering;

public class SpectreMazeTile
{
    //lowest level subtiles in reading order
    private static readonly string[] _metaTileLabel =
    {
        "Γ", "Δ", "Θ", "Λ", "Ξ", "Π", "Σ", "Φ", "Ψ"
    };

    private static readonly string[][] _subTilePostfix =
    {
        new string[] { "", "", "", "", "", "", ""},
        new string[] { "₀", "", "", "", "₀", "", "₁", "₁" },
        new string[] { "₀", "₀", "", "", "", "", "₁", "₁" },
        new string[] { "₀", "", "", "", "", "", "₁", "" },
        new string[] { "₀", "₀", "", "", "₁", "", "₁", "" },
        new string[] { "₀", "₀", "", "", "₁", "", "₁", "" },
        new string[] { "", "", "", "", "₀", "", "", "₁" },
        new string[] { "₀", "", "", "", "₀", "", "₁", "₁" },
        new string[] { "₀", "₀", "", "", "₁", "", "₁", "₂" }
    };

    //lowest level subtiles in reading order
    private static readonly string[][] _lowestMetaTileLabel =
    {
        new string[] { "Γ₀", "Γ₁" },
        new string[] { "Δ" },
        new string[] { "Θ" },
        new string[] { "Λ" },
        new string[] { "Ξ" },
        new string[] { "Π" },
        new string[] { "Σ" },
        new string[] { "Φ" },
        new string[] { "Ψ" }
    };

    //subtiles in reading order
    private static readonly int[][] _metaTileStructure =
    {
        new int[] {7,4,0,6,5,1,2},
        new int[] {7,5,0,6,4,1,7,4},
        new int[] {7,5,0,6,8,1,7,5},
        new int[] {7,5,0,6,8,1,7,4},
        new int[] {7,8,0,6,8,1,7,5},
        new int[] {7,8,0,6,8,1,7,4},
        new int[] {3,5,0,6,4,1,7,4},
        new int[] {7,5,0,6,8,1,7,8},
        new int[] {7,8,0,6,8,1,7,8}
    };

    //>6 is for inner transitions, subtract 6. Otherwise hex edges
    private static readonly int[][][] _upperTransitions =
    {
        new int[][] {new int[]{8,9,7,2,1,1},new int[]{6,9,3,3,2,2},new int[]{1,0,10,11,9,6},new int[]{8,11,12,3,7,6},new int[]{11,8,0,5,5,4},new int[]{9,8,10,4,4,12},new int[]{9,11,4,4,3,3}},
        new int[][] {new int[]{8,9,7,1,1,1},new int[]{6,9,3,2,2,1},new int[]{1,0,10,11,9,6},new int[]{8,11,12,3,7,6},new int[]{11,8,0,0,5,5},new int[]{9,8,10,5,13,12},new int[]{9,11,13,4,3,3},new int[]{12,11,5,5,4,4}},
        new int[][] {new int[]{8,9,7,1,1,1},new int[]{6,9,3,2,2,1},new int[]{1,0,10,11,9,6},new int[]{8,11,12,3,7,6},new int[]{11,8,0,5,5,5},new int[]{9,8,10,5,13,12},new int[]{9,11,13,3,3,3},new int[]{12,11,5,4,4,3}},
        new int[][] {new int[]{8,9,7,1,1,1},new int[]{6,9,3,2,2,1},new int[]{1,0,10,11,9,6},new int[]{8,11,12,3,7,6},new int[]{11,8,0,5,5,5},new int[]{9,8,10,5,13,12},new int[]{9,11,13,4,3,3},new int[]{12,11,5,5,4,4}},
        new int[][] {new int[]{8,9,7,2,1,1},new int[]{6,9,3,2,2,2},new int[]{1,0,10,11,9,6},new int[]{8,11,12,3,7,6},new int[]{11,8,0,5,5,5},new int[]{9,8,10,5,13,12},new int[]{9,11,13,3,3,3},new int[]{12,11,5,4,4,3}},
        new int[][] {new int[]{8,9,7,2,1,1},new int[]{6,9,3,2,2,2},new int[]{1,0,10,11,9,6},new int[]{8,11,12,3,7,6},new int[]{11,8,0,5,5,5},new int[]{9,8,10,5,13,12},new int[]{9,11,13,4,3,3},new int[]{12,11,5,5,4,4}},
        new int[][] {new int[]{8,9,7,1,1,0},new int[]{6,9,3,2,2,1},new int[]{0,0,10,11,9,6},new int[]{8,11,12,3,7,6},new int[]{11,8,0,0,5,5},new int[]{9,8,10,5,13,12},new int[]{9,11,13,4,3,3},new int[]{12,11,5,5,4,4}},
        new int[][] {new int[]{8,9,7,1,1,1},new int[]{6,9,3,2,2,1},new int[]{1,0,10,11,9,6},new int[]{8,11,12,3,7,6},new int[]{11,8,0,5,5,5},new int[]{9,8,10,5,13,12},new int[]{9,11,13,4,3,3},new int[]{12,11,5,4,4,4}},
        new int[][] {new int[]{8,9,7,2,1,1},new int[]{6,9,3,2,2,2},new int[]{1,0,10,11,9,6},new int[]{8,11,12,3,7,6},new int[]{11,8,0,5,5,5},new int[]{9,8,10,5,13,12},new int[]{9,11,13,4,3,3},new int[]{12,11,5,4,4,4}}
    };

    //when exiting, gives an index
    private static readonly int[][][] _lowerTransitions =
    {
        new int[][] {new int[]{0,0,0,0,2,1},new int[]{0,0,1,0,2,1},new int[]{0,1,0,0,0,0},new int[]{0,0,0,2,0,0},new int[]{0,0,0,1,0,4},new int[]{0,0,0,3,2,0},new int[]{0,0,1,0,4,3}},
        new int[][] {new int[]{0,0,0,3,2,1},new int[]{0,0,0,1,0,4},new int[]{0,2,0,0,0,0},new int[]{0,0,0,1,0,0},new int[]{0,0,1,0,4,3},new int[]{0,0,0,2,0,0},new int[]{0,0,0,0,3,2},new int[]{0,0,1,0,2,1}},
        new int[][] {new int[]{0,0,0,3,2,1},new int[]{0,0,0,1,0,4},new int[]{0,1,0,0,0,0},new int[]{0,0,0,1,0,0},new int[]{0,0,0,4,3,2},new int[]{0,0,0,1,0,0},new int[]{0,0,0,4,3,2},new int[]{0,0,0,1,0,5}},
        new int[][] {new int[]{0,0,0,3,2,1},new int[]{0,0,0,1,0,4},new int[]{0,1,0,0,0,0},new int[]{0,0,0,1,0,0},new int[]{0,0,0,5,4,3},new int[]{0,0,0,2,0,0},new int[]{0,0,0,0,3,2},new int[]{0,0,1,0,2,1}},
        new int[][] {new int[]{0,0,0,0,2,1},new int[]{0,0,0,3,2,1},new int[]{0,1,0,0,0,0},new int[]{0,0,0,1,0,0},new int[]{0,0,0,4,3,2},new int[]{0,0,0,1,0,0},new int[]{0,0,0,4,3,2},new int[]{0,0,0,1,0,5}},
        new int[][] {new int[]{0,0,0,0,2,1},new int[]{0,0,0,3,2,1},new int[]{0,1,0,0,0,0},new int[]{0,0,0,1,0,0},new int[]{0,0,0,5,4,3},new int[]{0,0,0,2,0,0},new int[]{0,0,0,0,3,2},new int[]{0,0,1,0,2,1}},
        new int[][] {new int[]{0,0,0,1,0,4},new int[]{0,0,0,1,0,2},new int[]{3,2,0,0,0,0},new int[]{0,0,0,1,0,0},new int[]{0,0,1,0,4,3},new int[]{0,0,0,2,0,0},new int[]{0,0,0,0,3,2},new int[]{0,0,1,0,2,1}},
        new int[][] {new int[]{0,0,0,3,2,1},new int[]{0,0,0,1,0,4},new int[]{0,1,0,0,0,0},new int[]{0,0,0,1,0,0},new int[]{0,0,0,4,3,2},new int[]{0,0,0,1,0,0},new int[]{0,0,0,0,3,2},new int[]{0,0,0,3,2,1}},
        new int[][] {new int[]{0,0,0,0,2,1},new int[]{0,0,0,3,2,1},new int[]{0,1,0,0,0,0},new int[]{0,0,0,1,0,0},new int[]{0,0,0,4,3,2},new int[]{0,0,0,1,0,0},new int[]{0,0,0,0,3,2},new int[]{0,0,0,3,2,1}}
    };

    private static readonly int[][] _edgeMax =
    {
        new int[]{1,2,2,4,4,1},
        new int[]{2,4,1,3,2,4},
        new int[]{1,4,1,5,1,4},
        new int[]{1,4,1,3,2,5},
        new int[]{1,2,3,5,1,4},
        new int[]{1,2,3,3,2,5},
        new int[]{4,2,1,3,2,4},
        new int[]{1,4,1,3,3,4},
        new int[]{1,2,3,3,3,4}
    };

    private static readonly int[][][] _tileBacktrack =
    {
        new int[][]{new int[]{4,2},new int[]{2,0,0},new int[]{0,1,1},new int[]{1,1,3,6,6},new int[]{6,6,5,5,4},new int[]{4,4}},
        new int[][]{new int[]{4,4,2},new int[]{2,0,0,0,1},new int[]{1,1},new int[]{1,3,6,6},new int[]{6,7,7},new int[]{7,7,5,4,4}},
        new int[][]{new int[]{4,2},new int[]{2,0,0,0,1},new int[]{1,1},new int[]{1,3,6,6,6,7},new int[]{7,7},new int[]{7,5,4,4,4}},
        new int[][]{new int[]{4,2},new int[]{2,0,0,0,1},new int[]{1,1},new int[]{1,3,6,6},new int[]{6,7,7},new int[]{7,7,5,4,4,4}},
        new int[][]{new int[]{4,2},new int[]{2,0,0},new int[]{0,1,1,1},new int[]{1,3,6,6,6,7},new int[]{7,7},new int[]{7,5,4,4,4}},
        new int[][]{new int[]{4,2},new int[]{2,0,0},new int[]{0,1,1,1},new int[]{1,3,6,6},new int[]{6,7,7},new int[]{7,7,5,4,4,4}},
        new int[][]{new int[]{4,4,2,2,0},new int[]{0,0,1},new int[]{1,1},new int[]{1,3,6,6},new int[]{6,7,7},new int[]{7,7,5,4,4}},
        new int[][]{new int[]{4,2},new int[]{2,0,0,0,1},new int[]{1,1},new int[]{1,3,6,6},new int[]{6,7,7,7},new int[]{7,5,4,4,4}},
        new int[][]{new int[]{4,2},new int[]{2,0,0},new int[]{0,1,1,1},new int[]{1,3,6,6},new int[]{6,7,7,7},new int[]{7,5,4,4,4}}
    };

    private static readonly int[][][] _edgeBacktrack =
    {
        new int[][]{new int[]{2,1},new int[]{0,5,4},new int[]{3,5,4},new int[]{3,2,3,5,4},new int[]{3,2,4,3,5},new int[]{4,3}},
        new int[][]{new int[]{3,2,1},new int[]{0,5,4,3,5},new int[]{4,3},new int[]{2,3,5,4},new int[]{3,5,4},new int[]{3,2,3,5,4}},
        new int[][]{new int[]{2,1},new int[]{0,5,4,3,5},new int[]{4,3},new int[]{2,3,5,4,3,5},new int[]{4,3},new int[]{2,3,5,4,3}},
        new int[][]{new int[]{2,1},new int[]{0,5,4,3,5},new int[]{4,3},new int[]{2,3,5,4},new int[]{3,5,4},new int[]{3,2,3,5,4,3}},
        new int[][]{new int[]{2,1},new int[]{0,5,4},new int[]{3,5,4,3},new int[]{2,3,5,4,3,5},new int[]{4,3},new int[]{2,3,5,4,3}},
        new int[][]{new int[]{2,1},new int[]{0,5,4},new int[]{3,5,4,3},new int[]{2,3,5,4},new int[]{3,5,4},new int[]{3,2,3,5,4,3}},
        new int[][]{new int[]{3,2,1,0,5},new int[]{4,3,5},new int[]{4,3},new int[]{2,3,5,4},new int[]{3,5,4},new int[]{3,2,3,5,4}},
        new int[][]{new int[]{2,1},new int[]{0,5,4,3,5},new int[]{4,3},new int[]{2,3,5,4},new int[]{3,5,4,3},new int[]{2,3,5,4,3}},
        new int[][]{new int[]{2,1},new int[]{0,5,4},new int[]{3,5,4,3},new int[]{2,3,5,4},new int[]{3,5,4,3},new int[]{2,3,5,4,3}}
    };

    //>6 is for inner transitions, subtract 6. Otherwise hex edges
    private static readonly int[][][] _lowestUpperTransitions =
    {
        new int[][] {new int[]{2,2,2,1,1,1,0,0,0,5,7,7,7,7},new int[]{3,3,3,3,6,6,6,6,5,5,4,4,4,4}},
        new int[][] {new int[]{3,2,2,2,1,1,1,1,5,5,4,4,4,3}},
        new int[][] {new int[]{3,2,2,2,1,1,0,0,0,5,5,4,4,4}},
        new int[][] {new int[]{3,2,2,2,1,1,0,0,0,5,4,4,4,3}},
        new int[][] {new int[]{3,2,2,1,1,1,0,0,0,5,5,4,4,4}},
        new int[][] {new int[]{3,2,2,1,1,1,0,0,0,5,4,4,4,3}},
        new int[][] {new int[]{3,2,2,2,0,0,0,0,5,5,4,4,4,3}},
        new int[][] {new int[]{3,2,2,2,1,1,0,0,0,5,5,4,4,3}},
        new int[][] {new int[]{3,2,2,1,1,1,0,0,0,5,5,4,4,3}}
    };

    //when exiting, gives an index
    private static readonly int[][][] _lowestLowerTransitions =
    {
        new int[][] {new int[]{2,1,0,2,1,0,2,1,0,2,3,2,1,0},new int[]{3,2,1,0,3,2,1,0,1,0,3,2,1,0}},
        new int[][] {new int[]{0,2,1,0,3,2,1,0,1,0,2,1,0,1}},
        new int[][] {new int[]{0,2,1,0,1,0,2,1,0,1,0,2,1,0}},
        new int[][] {new int[]{0,2,1,0,1,0,2,1,0,0,2,1,0,1}},
        new int[][] {new int[]{0,1,0,2,1,0,2,1,0,1,0,2,1,0}},
        new int[][] {new int[]{0,1,0,2,1,0,2,1,0,0,2,1,0,1}},
        new int[][] {new int[]{0,2,1,0,3,2,1,0,1,0,2,1,0,1}},
        new int[][] {new int[]{0,2,1,0,1,0,2,1,0,1,0,1,0,1}},
        new int[][] {new int[]{0,1,0,2,1,0,2,1,0,1,0,1,0,1}}
    };

    //ohno
    private static readonly int[][] _lowestEdgeMax =
    {
        new int[]{2,2,2,3,3,2},
        new int[]{0,3,2,1,2,1},
        new int[]{2,1,2,0,2,1},
        new int[]{2,1,2,1,2,0},
        new int[]{2,2,1,0,2,1},
        new int[]{2,2,1,1,2,0},
        new int[]{3,0,2,1,2,1},
        new int[]{2,1,2,1,1,1},
        new int[]{2,2,1,1,1,1}
    };

    //I want to die now
    private static readonly int[][][] _lowestEdgeBackTrack =
    {
        new int[][]{new int[]{8,7,6},new int[]{5,4,3},new int[]{2,1,0},new int[]{3,2,1,0},new int[]{13,12,11,10},new int[]{9,8,9}},
        new int[][]{new int[]{},new int[]{7,6,5,4},new int[]{3,2,1},new int[]{0,13},new int[]{12,11,10},new int[]{9,8}},
        new int[][]{new int[]{8,7,6},new int[]{5,4},new int[]{3,2,1},new int[]{0},new int[]{13,12,11},new int[]{10,9}},
        new int[][]{new int[]{8,7,6},new int[]{5,4},new int[]{3,2,1},new int[]{0,13},new int[]{12,11,10},new int[]{9}},
        new int[][]{new int[]{8,7,6},new int[]{5,4,3},new int[]{2,1},new int[]{0},new int[]{13,12,11},new int[]{10,9}},
        new int[][]{new int[]{8,7,6},new int[]{5,4,3},new int[]{2,1},new int[]{0,13},new int[]{12,11,10},new int[]{9}},
        new int[][]{new int[]{7,6,5,4},new int[]{},new int[]{3,2,1},new int[]{0,13},new int[]{12,11,10},new int[]{9,8}},
        new int[][]{new int[]{8,7,6},new int[]{5,4},new int[]{3,2,1},new int[]{0,13},new int[]{12,11},new int[]{10,9}},
        new int[][]{new int[]{8,7,6},new int[]{5,4,3},new int[]{2,1},new int[]{0,13},new int[]{12,11},new int[]{10,9}}
    };

    private static readonly int[][] _gammaFuckery =
    {
        new int[]{0,0,0},new int[]{0,0,0},new int[]{0,0,0},new int[]{1,1,1,1},new int[]{1,1,1,1},new int[]{1,1,0}
    };

    private System.Random _rng;
    private Stack<int> _tileStack;
    private Stack<int> _goalStack;
    private int _familiarDepth;
    private List<MemoryPoint> _memoryPoints;
    public MemoryPoint CurrentMemory { get; private set; }

    public bool Log { private get; set; }

    public SpectreMazeTile(SpectreMazeTile old)
    {
        _rng = old._rng;
        _tileStack = new Stack<int>(new Stack<int>(old._tileStack));
        for (int i = 0; i < 7; i++)
            for (int j = 0; j < 7; j++)
                _edges[i, j] = old._edges[i, j];
    }

    private SpectreMazeTile(Stack<int> tileStack, Stack<int> goalStack, float accessibility, System.Random rng)
    {
        _rng = rng;

        CurrentMemory = null;

        _tileStack = tileStack;
        _goalStack = goalStack;
        _memoryPoints = new List<MemoryPoint>();

        _familiarDepth = 2;
        UpdateRange();
        AddMemory();

        SetWalls(tileStack.Count - 1, accessibility);
        //for (int i = 0; i < 49; i++)
        //    _edges[i / 7, i % 7] = true;
    }

    private SpectreMazeTile() { }

    public static SpectreMazeTile Generate(int layerCount, float accessibility, System.Random rng)
    {
        if (_nerfedSetups == null)
            GenerateNerfSetups();

        Stack<int> tileStack = new Stack<int>();
        Stack<int> goalStack = new Stack<int>();
        int selected = 0;
        int rnd = rng.Next(_highestLayerWeights.Sum());
        while (rnd >= _highestLayerWeights[selected])
        {
            rnd -= _highestLayerWeights[selected];
            selected++;
        }
        int lastType = selected;

        tileStack.Push(selected);

        layerCount--;

        int goalSelected = -1;
        int goalRnd = -1;
        int goalLastType = lastType;

        if (layerCount > 0)
        {
            while (true)
            {
                rnd = rng.Next(GetSubtiles(layerCount, lastType == 0));
                goalRnd = rng.Next(GetSubtiles(layerCount, goalLastType == 0));

                int tempSelected = 0;
                int tempGoalSelected = 0;

                while (rnd >= GetSubtiles(layerCount - 1, _metaTileStructure[lastType][tempSelected] == 0))
                {
                    rnd -= GetSubtiles(layerCount - 1, _metaTileStructure[lastType][tempSelected] == 0);
                    tempSelected++;
                }

                while (goalRnd >= GetSubtiles(layerCount - 1, _metaTileStructure[goalLastType][tempGoalSelected] == 0))
                {
                    goalRnd -= GetSubtiles(layerCount - 1, _metaTileStructure[goalLastType][tempGoalSelected] == 0);
                    tempGoalSelected++;
                }

                if (_metaTileStructure[lastType][tempSelected] == _metaTileStructure[goalLastType][tempGoalSelected])
                    continue;

                selected = tempSelected;
                goalSelected = tempGoalSelected;

                lastType = _metaTileStructure[lastType][selected];
                tileStack.Push(selected);

                goalLastType = _metaTileStructure[goalLastType][goalSelected];
                goalStack.Push(goalLastType);

                layerCount--;

                break;
            }

        }

        while (layerCount > 0)
        {
            selected = 0;
            goalSelected = 0;

            while (rnd >= GetSubtiles(layerCount - 1, _metaTileStructure[lastType][selected] == 0))
            {
                rnd -= GetSubtiles(layerCount - 1, _metaTileStructure[lastType][selected] == 0);
                selected++;
            }

            while (goalRnd >= GetSubtiles(layerCount - 1, _metaTileStructure[goalLastType][goalSelected] == 0))
            {
                goalRnd -= GetSubtiles(layerCount - 1, _metaTileStructure[goalLastType][goalSelected] == 0);
                goalSelected++;
            }

            lastType = _metaTileStructure[lastType][selected];
            tileStack.Push(selected);

            goalLastType = _metaTileStructure[goalLastType][goalSelected];
            goalStack.Push(goalSelected);

            layerCount--;
        }

        tileStack.Push(rnd);
        goalStack.Push(goalRnd);

        SpectreMazeTile tile = new SpectreMazeTile(tileStack, goalStack, accessibility, rng);

        return tile;
    }

    public static void GenerateNerfSetups()
    {
        List<bool[]> candidates = new List<bool[]>();

        foreach (bool[] setup in _possibleSetups)
            for (int i = 0; i < 13; i++)
            {
                if (setup[i])
                    continue;

                bool[] newSetup = Enumerable.Range(0, 13).Select(x => x == i || setup[x]).ToArray();

                if (candidates.Any(x => x.SequenceEqual(newSetup)))
                    continue;

                bool safe = true;
                foreach (bool[] setup2 in _possibleSetups)
                {
                    int diff = 0;
                    for (int j = 0; j < 13; j++)
                        if (newSetup[j] && !setup2[j])
                            diff++;
                        else if (!newSetup[j] && setup2[j])
                        {
                            diff = 0;
                            break;
                        }

                    if (diff >= 2)
                    {
                        safe = false;
                        break;
                    }
                }

                if (!safe)
                    continue;

                candidates.Add(newSetup);
            }

        _nerfedSetups = candidates.ToArray();
    }

    private void SetWalls(int calculationDepth, float accessibility)
    {
        int[][] links = new int[][] { new int[] { 0, 43 }, new int[] { 2, 8, 14 }, new int[] { 3, 9, 15, 21 }, new int[] { 4, 10, 22 }, new int[] { 13, 19, 37 }, new int[] { 18, 30 }, new int[] { 20, 26, 38, 44 }, new int[] { 25, 31 }, new int[] { 27, 39, 45 }, new int[] { 28 }, new int[] { 32 }, new int[] { 33 }, new int[] { 35, 48 } };

        int[] options = links.Select(x => PickRandom(x, _rng)).ToArray();

        Queue<bool[]> mazes = new Queue<bool[]>(Shuffle(_nerfedSetups.ToList(), _rng));

        while (mazes.Count > 0)
        {
            bool[] maze = mazes.Dequeue();

            for (int i = 0; i < options.Length; i++)
            {
                int option = options[i];

                _edges[option / 7, option % 7] = maze[i];
            }

            SpectreMazeTile positionFinder = new SpectreMazeTile(this);
            positionFinder._tileStack = Unlayer(_tileStack, calculationDepth);

            bool targetFound = false;
            bool success = false;

            Queue<Stack<int>> positions = new Queue<Stack<int>>();

            bool[] foundTiles = new bool[GetSubtiles(calculationDepth - 1, Unlayer(_tileStack, calculationDepth).Last() == 0)];
            foundTiles[PositionInt(positionFinder._tileStack)] = true;
            int foundTileCount = 0;

            positions.Enqueue(positionFinder._tileStack);
            while (positions.Count > 0)
            {
                positionFinder._tileStack = positions.Dequeue();
                for (int j = 0; j < 14; j++)
                {
                    SpectreMazeTile copy = new SpectreMazeTile(positionFinder);

                    bool? foundTile = copy.Traverse(j);
                    if (foundTile != null && (bool)foundTile)
                    {
                        int foundNeighbour = PositionInt(copy._tileStack);
                        if (!foundTiles[foundNeighbour])
                        {
                            foundTiles[foundNeighbour] = true;
                            foundTileCount++;

                            float achievedRatio = (float)foundTileCount / foundTiles.Length;
                            targetFound |= IsOfType(copy._tileStack, _goalStack);

                            success |= targetFound && achievedRatio >= accessibility;

                            positions.Enqueue(copy._tileStack);
                        }
                    }
                    if (success)
                        return;
                }
            }
        }
    }

    private void UpdateRange()
    {
        while (true)
        {
            bool needsExpanding = false;

            for (int i = 0; i < 14; i++)
            {
                SpectreMazeTile copy = new SpectreMazeTile(this);

                needsExpanding |= copy.Traverse(i) == null;
            }

            if (needsExpanding)
            {
                AddLayer();

                continue;
            }

            break;
        }
    }

    //close enough, but once again, fuck gamma for not getting us the same probabilities each layer
    private static readonly int[][] _layerUpWeights =
    {
        new int[]{3528,3528,504,441,2656,2663,3528,6111,4761},
        new int[]{3528,3528,504,441,2656,2663,3528,6111,4761},
        new int[]{1,0,0,0,0,0,0,0,0},
        new int[]{0,0,0,0,0,0,1,0,0},
        new int[]{3528,7056,0,441,0,2663,7056,0,0},
        new int[]{3528,3528,1008,441,2656,0,3528,6111,0},
        new int[]{3528,3528,504,441,2656,2663,3528,6111,4761},
        new int[]{1764,3528,504,441,2656,2663,1764,6111,4761},
        new int[]{0,0,504,441,5312,5326,0,12222,14283}
    };
    //same for this one vvv
    private static readonly int[] _highestLayerWeights = { 3528, 3528, 504, 441, 2656, 2663, 3528, 6111, 4761 };

    private void AddLayer()
    {
        Stack<int> reverseCopy = new Stack<int>(_tileStack);
        int type = reverseCopy.Pop();
        int[] layerUpWeight = _layerUpWeights[type];

        int parent = 0;
        int rnd = _rng.Next(layerUpWeight.Sum());
        while (rnd >= layerUpWeight[parent])
        {
            rnd -= layerUpWeight[parent];
            parent++;
        }

        int index = PickRandom(Enumerable.Range(0, _metaTileStructure[parent].Length).Where(x => _metaTileStructure[parent][x] == type), _rng);

        reverseCopy.Push(index);
        reverseCopy.Push(parent);

        foreach (MemoryPoint memoryPoint in _memoryPoints)
        {
            memoryPoint.AddLayer(index);
        }

        _tileStack = new Stack<int>(reverseCopy);
    }

    private void LearnLayer()
    {
        _familiarDepth++;
        if (_familiarDepth < _tileStack.Count)
        {
            if (CurrentMemory == null)
            {
                CurrentMemory = new MemoryPoint(_tileStack, Enumerable.Repeat(false, _familiarDepth - 1).ToList(), false);
                _memoryPoints.Add(CurrentMemory);
            }

            foreach (MemoryPoint memoryPoint in _memoryPoints)
                memoryPoint.KnownDepths.Add(memoryPoint == CurrentMemory);

            return;
        }

        throw new InvalidOperationException("Unable to find a new layer");
    }

    public bool? Traverse(int edge)
    {
        Stack<int> stack = new Stack<int>(new Stack<int>(_tileStack));

        //getting types
        Stack<int> typeStack = new Stack<int>(_tileStack);
        Stack<int> tileTypes = new Stack<int>();
        tileTypes.Push(typeStack.Pop());
        while (typeStack.Count > 1)
            tileTypes.Push(_metaTileStructure[tileTypes.Peek()][typeStack.Pop()]);

        Stack<int> backTrackEdgeIndex = new Stack<int>();
        Stack<int> backTrackCurrentTile = new Stack<int>();
        Stack<int> backTrackMetaTile = new Stack<int>();

        int currentTile = stack.Pop();
        int metaTile = tileTypes.Pop();

        int newTile = _lowestUpperTransitions[metaTile][currentTile][edge];
        int oldEdge = _lowestLowerTransitions[metaTile][currentTile][edge];
        //Very awful Gamma edge case
        if (newTile > 5)
        {
            newTile -= 6;

            stack.Push(newTile);

            int targetEdgeIndex = Enumerable.Range(0, 14).First(x => _lowestUpperTransitions[metaTile][newTile][x] == currentTile + 6 && _lowestLowerTransitions[metaTile][newTile][x] == 3 - oldEdge);

            if (!GetEdgePath(edge, targetEdgeIndex))
                return false;
            else
            {
                _tileStack = stack;
                return true;
            }
        }

        int newEdge = _lowestEdgeMax[metaTile][newTile] - oldEdge;


        while (true)
        {
            backTrackCurrentTile.Push(currentTile);
            backTrackMetaTile.Push(metaTile);

            //if unlayering escalates
            if (tileTypes.Count == 0)
                return null;

            currentTile = stack.Pop();
            metaTile = tileTypes.Pop();


            oldEdge = newTile;

            newTile = _upperTransitions[metaTile][currentTile][oldEdge];

            if (newTile > 5)
                break;

            backTrackEdgeIndex.Push(newEdge);

            newEdge = _edgeMax[metaTile][newTile] - _lowerTransitions[metaTile][currentTile][oldEdge];
        }

        newTile -= 6;

        currentTile = Array.IndexOf(_upperTransitions[metaTile][newTile], currentTile + 6);

        while (true)
        {
            stack.Push(newTile);

            metaTile = _metaTileStructure[metaTile][newTile];

            if (backTrackEdgeIndex.Count < 1)
                break;

            newTile = _tileBacktrack[metaTile][currentTile][newEdge];

            currentTile = _edgeBacktrack[metaTile][currentTile][newEdge];

            newEdge = backTrackEdgeIndex.Pop();
        }

        int finalEdgeIndex = _lowestEdgeBackTrack[metaTile][currentTile][newEdge];

        //NOTE fuck Gamma in particular (upside down L plus ratio)
        stack.Push(metaTile == 0 ? _gammaFuckery[currentTile][newEdge] : 0);

        if (!GetEdgePath(edge, finalEdgeIndex))
            return false;
        else
        {
            _tileStack = stack;
            return true;
        }
    }

    public bool Move(int edge)
    {
        bool? didMove = Traverse(edge);

        if (didMove == null || !(bool)didMove)
        {
            return false;
        }

        UpdateRange();

        int memoryIndex = _memoryPoints.IndexOf(x => x.Location.SequenceEqual(_tileStack.Take(_tileStack.Count - 1)));
        if (memoryIndex != -1)
            CurrentMemory = _memoryPoints[memoryIndex];
        else
            CurrentMemory = null;

        return true;
    }

    public void HandleFail()
    {
        AddMemory();
    }

    public bool TryExplore(bool[] trial)
    {
        for (int i = 0; i < trial.Length; i++)
        {
            SpectreMazeTile copy = new SpectreMazeTile(this);
            copy._tileStack = Unlayer(_tileStack, _familiarDepth);

            if (trial[i] ^ (copy.Traverse(i) == null))
                return false;
        }

        LearnLayer();
        return true;
    }

    public IEnumerable<int> GetUnknownEdges()
    {
        List<int> edges = new List<int>();
        for (int i = 0; i < 14; i++)
        {
            SpectreMazeTile copy = new SpectreMazeTile(this);
            copy._tileStack = Unlayer(_tileStack, _familiarDepth);

            if (copy.Traverse(i) == null)
                edges.Add(i);
        }

        return edges;
    }

    private bool[,] _edges = new bool[7, 7];
    public bool GetEdgePath(int edge1, int edge2)
    {
        if (((edge1 ^ edge2) & 1) != 1)
        {
            throw new InvalidOperationException("Edge parity must differ");
        }

        int oddEdge;
        int evenEdge;

        if (edge1 % 2 == 1)
        {
            oddEdge = edge1 / 2;
            evenEdge = edge2 / 2;
        }
        else
        {
            oddEdge = edge2 / 2;
            evenEdge = edge1 / 2;
        }

        return _edges[oddEdge, evenEdge];
    }

    public IEnumerable<int> GetConnected(int edge)
    {
        return Enumerable.Range(0, 14).Where(x => ((x ^ edge) & 1) != 0 && GetEdgePath(x, edge));
    }

    public IEnumerable<string> GetAllEdgePairs()
    {
        List<string> edgePairs = new List<string>();
        for (int i = 0; i < 14; i++)
            for (int j = i + 1; j < 14; j += 2)
                if (GetEdgePath(i, j))
                    edgePairs.Add(i + "-" + j);

        return edgePairs;
    }

    public Stack<int> GetStack()
    {
        return _tileStack;
    }

    public Stack<int> GetGoal()
    {
        return _goalStack;
    }

    //to bias to subtiles
    public static int GetSubtiles(int layer, bool isGamma)
    {
        int gammaTiles = isGamma ? 1 : 0;
        int regularTiles = 1 - gammaTiles;
        while (layer > 0)
        {
            int newGammaTiles = regularTiles + gammaTiles;
            int newRegularTiles = 6 * gammaTiles + 7 * regularTiles;

            gammaTiles = newGammaTiles;
            regularTiles = newRegularTiles;

            layer--;
        }

        gammaTiles *= 2;

        return regularTiles + gammaTiles;
    }

    public List<string> GetMemoryText()
    {
        if (CurrentMemory == null)
            return null;

        return ParseStack(_tileStack, CurrentMemory.KnownDepths);
    }

    //this can't use the better algorithm due to the depth potentially being too much
    public List<int> GetSolvePath()
    {
        SpectreMazeTile positionFinder = new SpectreMazeTile(this);

        List<Stack<int>> positions = new List<Stack<int>>();
        List<List<int>> paths = new List<List<int>> { new List<int>() };
        List<int> oldestNeighbour = new List<int> { 0 };
        positions.Add(positionFinder._tileStack);
        for (int i = 0; i < positions.Count; i++)
        {
            positionFinder._tileStack = positions[i];
            for (int j = 0; j < 14; j++)
            {
                SpectreMazeTile copy = new SpectreMazeTile(positionFinder);

                bool? foundTile = copy.Traverse(j);
                if (foundTile != null && (bool)foundTile)
                {
                    int foundNeighbour = positions.Skip(oldestNeighbour[i]).IndexOf(x => x.SequenceEqual(copy._tileStack));
                    if (foundNeighbour == -1)
                    {
                        List<int> newPath = paths[i].Concat(new int[] { j }).ToList();

                        if (IsOfType(copy._tileStack, _goalStack))
                            return newPath;

                        positions.Add(copy._tileStack);
                        paths.Add(newPath);
                        oldestNeighbour.Add(i);
                    }
                }
            }
        }

        return null;
    }

    public static Stack<int> Unlayer(Stack<int> tileStack, int remainingLayers)
    {
        if (remainingLayers <= 0)
            throw new InvalidOperationException("Do not the negatives (or 0)");

        Stack<int> stack = new Stack<int>(tileStack);
        while (stack.Count - 1 > remainingLayers)
        {
            int metaTile = stack.Pop();
            int currentTile = stack.Pop();

            stack.Push(_metaTileStructure[metaTile][currentTile]);
        }
        return new Stack<int>(stack);
    }

    public static bool IsOfType(Stack<int> main, Stack<int> subset)
    {
        if (main.Count <= subset.Count)
            return false;
        return Unlayer(main, subset.Count - 1).SequenceEqual(subset);
    }


    //lowerbound is inclusive
    public static List<string> ParseStack(Stack<int> stack)
    {
        Stack<int> typeStack = new Stack<int>(stack);

        List<string> text = new List<string>();

        string postfix = "";

        int metaTile = typeStack.Pop();

        while (typeStack.Count > 1)
        {
            int index = typeStack.Pop();

            text.Add(_metaTileLabel[metaTile] + postfix);
            postfix = _subTilePostfix[metaTile][index];

            metaTile = _metaTileStructure[metaTile][index];
        }

        text.Add(_lowestMetaTileLabel[metaTile][typeStack.Pop()] + postfix);

        return text;
    }

    public static List<string> ParseStack(Stack<int> stack, List<bool> knownDepths)
    {
        Stack<int> typeStack = new Stack<int>(stack);

        List<string> text = new List<string>();

        string postfix = "";

        int metaTile = typeStack.Pop();

        while (typeStack.Count > 1)
        {
            int index = typeStack.Pop();

            bool nonSecret = knownDepths.Count > typeStack.Count ? knownDepths[typeStack.Count] : false;

            text.Add((nonSecret ? _metaTileLabel[metaTile] : "?") + postfix);
            postfix = nonSecret ? _subTilePostfix[metaTile][index] : "";

            metaTile = _metaTileStructure[metaTile][index];
        }

        text.Add((knownDepths[0] ? _lowestMetaTileLabel[metaTile][typeStack.Pop()] : "?") + postfix);

        return text;
    }

    public static int PositionInt(Stack<int> stack)
    {
        int position = 0;
        int layerCount = stack.Count - 2;

        Stack<int> typeStack = new Stack<int>(stack);

        int metaTile = typeStack.Pop();

        while (typeStack.Count > 1)
        {
            int index = typeStack.Pop();

            for (int i = 0; i < index; i++)
                position += GetSubtiles(layerCount - 1, _metaTileStructure[metaTile][i] == 0);

            metaTile = _metaTileStructure[metaTile][index];

            layerCount--;
        }

        position += typeStack.Pop();

        return position;
    }

    public static Stack<int> IntPosition(int index, int depth, int metaTile)
    {
        Stack<int> result = new Stack<int>();
        result.Push(metaTile);

        while (depth > 0)
        {
            int selected = 0;
            while (index >= GetSubtiles(depth - 1, _metaTileStructure[metaTile][selected] == 0))
            {
                index -= GetSubtiles(depth - 1, _metaTileStructure[metaTile][selected] == 0);
                selected++;
            }

            result.Push(selected);
            metaTile = _metaTileStructure[metaTile][selected];

            depth--;
        }
        result.Push(index);

        return result;
    }

    private void AddMemory()
    {
        if (CurrentMemory != null)
            _memoryPoints.Remove(CurrentMemory);
        CurrentMemory = new MemoryPoint(_tileStack, Enumerable.Repeat(true, _familiarDepth).ToList(), true);
        _memoryPoints.Add(CurrentMemory);
    }

    public class MemoryPoint
    {
        //no top layer included, the stack will just be forced to layer up when needed
        public Stack<int> Location { get; private set; }
        public List<bool> KnownDepths { get; private set; }
        public bool ShowAllData { get; private set; }

        public MemoryPoint(Stack<int> currentLocation, List<bool> knownDepths, bool showAllData)
        {
            Location = new Stack<int>(new Stack<int>(currentLocation).Skip(1));
            KnownDepths = knownDepths;
            ShowAllData = showAllData;
        }

        public void AddLayer(int layer)
        {
            Location = new Stack<int>(new Stack<int>(Location.Concat(new int[] { layer })));
        }

        public override string ToString()
        {
            return Location.Join(",") + "; " + KnownDepths.Select(x => x ? 1 : 0).Join("");
        }
    }

    private static List<T> Shuffle<T>(List<T> list, System.Random rng)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int swapIdx = rng.Next(i, list.Count);
            T temp = list[swapIdx];
            list[swapIdx] = list[i];
            list[i] = temp;
        }
        return list;
    }

    private static T PickRandom<T>(IEnumerable<T> collection, System.Random rng)
    {
        List<T> list = collection.ToList();
        int index = rng.Next(0, list.Count);
        return list[index];
    }

    public static List<bool[]> GetAllWallSetups(int calculationDepth, int metaTile, List<bool[]> origOptions = null)
    {
        int[] links = new int[] { 0, 2, 3, 4, 13, 18, 20, 25, 27, 28, 32, 33, 35 };

        bool[] analysed = new bool[1 << links.Length];

        Queue<Queue<bool[]>> optionQueues = new Queue<Queue<bool[]>>();
        Queue<Queue<int>> minIndexQueues = new Queue<Queue<int>>();
        Queue<bool[]> options = new Queue<bool[]>();
        Queue<int> minIndex = new Queue<int>();
        List<bool[]> candidates = new List<bool[]>();

        if (origOptions == null)
        {
            options.Enqueue(new bool[links.Length]);
            minIndex.Enqueue(0);

            optionQueues = new Queue<Queue<bool[]>>(Enumerable.Range(0, links.Length).Select(_ => new Queue<bool[]>()));
            minIndexQueues = new Queue<Queue<int>>(Enumerable.Range(0, links.Length).Select(_ => new Queue<int>()));
        }
        else
        {
            Queue<bool[]>[] optionQueueInitialiser = Enumerable.Range(0, links.Length).Select(_ => new Queue<bool[]>()).ToArray();

            foreach (bool[] option in origOptions)
            {
                int index = option.Count(x => x) - 1;
                if (index == -1)
                    options.Enqueue(option);
                else
                    optionQueueInitialiser[index].Enqueue(option);
            }

            options = new Queue<bool[]>(origOptions);
            minIndex = new Queue<int>(new int[origOptions.Count]);

            optionQueues = new Queue<Queue<bool[]>>(optionQueueInitialiser);
            minIndexQueues = new Queue<Queue<int>>(optionQueues.Select(x => new Queue<int>(x.Select(_ => 0))));
        }

        SpectreMazeTile positionFinder = new SpectreMazeTile();

        bool firstRun = true;
        while (optionQueues.Count > 0 || firstRun)
        {
            if (!firstRun)
            {
                options = optionQueues.Dequeue();
                minIndex = minIndexQueues.Dequeue();
            }

            firstRun = false;

            while (options.Count > 0)
            {
                //Debug.Log(options.Count + " remaining...");

                bool[] attempt = options.Dequeue();
                int currentMinIndex = minIndex.Dequeue();

                int attemptBinary = attempt.Select((x, ix) => x ? (1 << (attempt.Length - 1 - ix)) : 0).Sum();

                if (analysed[attemptBinary])
                    continue;

                analysed[attemptBinary] = true;

                //don't bother because we've got a working subset with some present items
                bool different = true;
                foreach (bool[] candidate in candidates)
                {
                    different = false;
                    for (int i = 0; i < links.Length; i++)
                        different |= !attempt[i] && candidate[i];

                    if (!different)
                        break;
                }

                if (!different)
                    continue;

                for (int i = 0; i < links.Length; i++)
                    positionFinder._edges[links[i] / 7, links[i] % 7] = attempt[i];

                int[] tileGroups = Enumerable.Range(0, GetSubtiles(calculationDepth - 1, metaTile == 0)).ToArray();

                for (int i = 0; i < tileGroups.Length; i++)
                {
                    positionFinder._tileStack = IntPosition(i, calculationDepth - 1, metaTile);
                    //Debug.Log(positionFinder._tileStack.Join("-"));

                    for (int j = 0; j < 14; j++)
                    {
                        SpectreMazeTile copy = new SpectreMazeTile(positionFinder);

                        bool? foundTile = copy.Traverse(j);
                        if (foundTile == null)
                        {
                            int originalGroup = tileGroups[i];

                            if (originalGroup == -1)
                                continue;

                            for (int k = originalGroup; k < tileGroups.Length; k++)
                                if (tileGroups[k] == originalGroup)
                                    tileGroups[k] = -1;
                        }
                        else if ((bool)foundTile)
                        {
                            int foundNeighbour = PositionInt(copy._tileStack);
                            if (tileGroups[foundNeighbour] > tileGroups[i])
                            {
                                int originalGroup = tileGroups[foundNeighbour];

                                for (int k = originalGroup; k < tileGroups.Length; k++)
                                    if (tileGroups[k] == originalGroup)
                                        tileGroups[k] = tileGroups[i];
                            }
                            else if (tileGroups[foundNeighbour] < tileGroups[i])
                            {
                                int originalGroup = tileGroups[i];

                                for (int k = originalGroup; k < tileGroups.Length; k++)
                                    if (tileGroups[k] == originalGroup)
                                        tileGroups[k] = tileGroups[foundNeighbour];
                            }
                        }
                    }
                }

                //it somehow all works out so we proceed...
                if (tileGroups.All(x => x == -1))
                {
                    candidates.Add(attempt);
                    Debug.Log(attempt.Select(x => x ? 1 : 0).Join("") + " seems to work out");
                    continue;
                }

                //Debug.Log(tileGroups.Select(x => x == -1 ? "." : x.ToString()).Join(""));
                //Debug.Log(tileGroups.Select((_, ix) => ix).Where(x => tileGroups[x] != -1).Select(x => ParseStack(IntPosition(x, calculationDepth, metaTile)).Join("-")).Join("; "));

                //generating new options if not candidate-worthy
                for (int i = currentMinIndex; i < links.Length; i++)
                {
                    if (attempt[i])
                        continue;

                    bool[] newOption = new bool[links.Length];
                    for (int j = 0; j < links.Length; j++)
                        newOption[j] = attempt[j];
                    newOption[i] = true;

                    optionQueues.Peek().Enqueue(newOption);
                    minIndexQueues.Peek().Enqueue(i + 1);
                }
            }
        }

        //filter remaining junk
        List<bool[]> finalisedCandidates = new List<bool[]>();

        foreach (bool[] finalist in candidates)
        {
            bool different = true;
            foreach (bool[] candidate in candidates)
            {
                if (finalist == candidate)
                    continue;

                different = false;
                for (int i = 0; i < links.Length; i++)
                    different |= !finalist[i] && candidate[i];

                if (!different)
                    break;
            }

            if (!different)
                continue;

            finalisedCandidates.Add(finalist);
        }

        return finalisedCandidates.OrderBy(x => x.Count(y => y)).ToList();
    }

    private static bool[][] _nerfedSetups = null;

    //do not question these; GetAllWallSetups() has ran these for over 8 hours on layer 7 to try and maximise guarantee for full connectivity with no changes since layer 5 checks
    private static readonly bool[][] _possibleSetups = new bool[][]
    {
        new bool[] { false, false, false, true, true, false, true, false, false, false, false, false, false },
        new bool[] { true, false, false, false, true, true, true, false, false, false, false, false, false },
        new bool[] { true, false, false, false, true, false, true, false, false, false, false, true, false },
        new bool[] { true, false, false, false, false, false, true, false, false, false, false, true, true },
        new bool[] { false, false, false, false, true, false, true, false, false, false, false, true, true },
        new bool[] { true, false, true, true, true, false, false, false, false, false, false, false, false },
        new bool[] { false, true, true, true, true, false, false, false, false, false, false, false, false },
        new bool[] { false, false, true, true, true, false, false, false, false, false, true, false, false },
        new bool[] { false, false, true, true, true, false, false, false, false, false, false, true, false },
        new bool[] { false, false, true, true, true, false, false, false, false, false, false, false, true },
        new bool[] { true, false, false, true, false, true, true, false, false, false, false, false, false },
        new bool[] { true, false, false, true, false, false, true, true, false, false, false, false, false },
        new bool[] { true, false, false, true, false, false, true, false, false, false, true, false, false },
        new bool[] { true, false, false, true, false, false, true, false, false, false, false, true, false },
        new bool[] { true, false, false, true, false, false, true, false, false, false, false, false, true },
        new bool[] { false, false, false, true, false, true, true, false, true, false, false, false, false },
        new bool[] { false, false, false, true, false, true, true, false, false, false, false, true, false },
        new bool[] { false, false, false, true, false, true, true, false, false, false, false, false, true },
        new bool[] { true, false, false, true, true, false, false, true, false, false, false, false, false },
        new bool[] { false, true, false, true, true, false, false, true, false, false, false, false, false },
        new bool[] { false, false, false, true, true, false, false, true, false, false, true, false, false },
        new bool[] { false, false, false, true, true, false, false, true, false, false, false, true, false },
        new bool[] { false, false, false, true, true, false, false, true, false, false, false, false, true },
        new bool[] { true, true, true, false, false, false, false, false, false, false, false, true, true },
        new bool[] { true, false, true, false, true, true, false, true, false, false, false, false, false },
        new bool[] { true, false, true, false, true, true, false, false, false, false, true, false, false },
        new bool[] { true, false, true, false, false, false, false, false, false, false, true, true, true },
        new bool[] { false, true, true, false, true, false, false, false, false, false, false, true, true },
        new bool[] { false, false, true, false, true, false, false, false, false, false, true, true, true },
        new bool[] { true, true, false, false, false, false, false, true, false, false, false, true, true },
        new bool[] { true, false, false, false, true, true, false, true, false, false, true, false, false },
        new bool[] { true, false, false, false, false, false, false, true, false, false, true, true, true },
        new bool[] { false, true, false, false, true, false, false, true, false, false, false, true, true },
        new bool[] { false, false, false, false, true, false, false, true, false, false, true, true, true },
        new bool[] { true, true, true, true, false, true, false, false, false, false, false, false, false },
        new bool[] { true, true, true, true, false, false, false, true, false, false, false, false, false },
        new bool[] { true, true, true, true, false, false, false, false, false, false, true, false, false },
        new bool[] { true, true, true, true, false, false, false, false, false, false, false, true, false },
        new bool[] { true, true, true, true, false, false, false, false, false, false, false, false, true },
        new bool[] { true, false, true, true, false, true, false, false, false, false, true, false, false },
        new bool[] { true, false, true, true, false, false, false, true, false, false, true, false, false },
        new bool[] { true, false, true, true, false, false, false, false, true, false, true, false, false },
        new bool[] { true, false, true, true, false, false, false, false, false, false, true, true, false },
        new bool[] { true, false, true, true, false, false, false, false, false, false, true, false, true },
        new bool[] { true, false, true, false, true, true, false, false, false, false, false, true, false },
        new bool[] { true, false, true, false, true, false, false, false, false, false, false, true, true },
        new bool[] { true, false, true, false, false, true, true, false, false, false, false, true, false },
        new bool[] { true, false, true, false, false, true, true, false, false, false, false, false, true },
        new bool[] { false, true, true, true, false, true, false, false, true, false, false, false, false },
        new bool[] { false, true, true, true, false, true, false, false, false, false, false, true, false },
        new bool[] { false, true, true, true, false, true, false, false, false, false, false, false, true },
        new bool[] { true, false, true, true, false, true, false, false, true, false, false, false, false },
        new bool[] { false, false, true, true, false, true, false, false, true, false, true, false, false },
        new bool[] { false, false, true, true, false, true, false, false, true, false, false, true, false },
        new bool[] { false, false, true, true, false, true, false, false, true, false, false, false, true },
        new bool[] { false, false, true, true, false, true, false, false, false, false, true, true, false },
        new bool[] { false, false, true, true, false, true, false, false, false, false, true, false, true },
        new bool[] { false, false, true, true, false, false, true, false, true, false, true, false, false },
        new bool[] { true, false, true, true, false, false, false, false, true, false, false, true, false },
        new bool[] { true, false, true, true, false, false, false, false, true, false, false, false, true },
        new bool[] { false, false, true, true, false, false, true, false, false, false, true, true, false },
        new bool[] { false, false, true, true, false, false, true, false, false, false, true, false, true },
        new bool[] { false, false, true, true, false, false, true, true, true, false, false, false, false },
        new bool[] { false, false, true, true, false, false, true, true, false, false, false, true, false },
        new bool[] { true, true, false, false, false, true, true, false, false, false, false, false, true },
        new bool[] { true, false, false, false, false, true, true, false, true, false, false, false, true },
        new bool[] { true, false, false, false, false, true, true, false, false, true, false, false, true },
        new bool[] { false, true, true, false, true, true, true, false, false, false, false, false, false },
        new bool[] { false, true, false, false, true, true, true, false, false, false, false, false, true },
        new bool[] { false, false, false, false, true, true, true, false, true, false, false, false, true },
        new bool[] { false, false, false, false, true, true, true, false, false, true, false, false, true },
        new bool[] { true, true, false, true, false, true, false, true, false, false, false, false, false },
        new bool[] { true, true, false, true, false, false, false, true, false, false, true, false, false },
        new bool[] { true, true, false, true, false, false, false, true, false, false, false, true, false },
        new bool[] { true, true, false, true, false, false, false, true, false, false, false, false, true },
        new bool[] { true, false, false, true, false, true, false, true, false, false, true, false, false },
        new bool[] { true, false, false, true, false, false, false, true, true, false, true, false, false },
        new bool[] { true, false, false, true, false, false, false, true, false, false, true, true, false },
        new bool[] { true, false, false, true, false, false, false, true, false, false, true, false, true },
        new bool[] { true, false, false, false, true, true, false, true, false, false, false, true, false },
        new bool[] { true, false, false, false, true, false, false, true, false, false, false, true, true },
        new bool[] { false, true, false, true, false, true, false, true, true, false, false, false, false },
        new bool[] { false, true, false, true, false, true, false, true, false, false, false, true, false },
        new bool[] { false, true, false, true, false, true, false, true, false, false, false, false, true },
        new bool[] { false, true, false, true, false, false, true, true, true, false, false, false, false },
        new bool[] { false, true, false, true, false, false, true, true, false, false, false, true, false },
        new bool[] { false, true, false, true, false, false, true, true, false, false, false, false, true },
        new bool[] { true, false, false, true, false, true, false, true, true, false, false, false, false },
        new bool[] { false, false, false, true, false, true, false, true, true, false, true, false, false },
        new bool[] { false, false, false, true, false, true, false, true, true, false, false, true, false },
        new bool[] { false, false, false, true, false, true, false, true, true, false, false, false, true },
        new bool[] { false, false, false, true, false, true, false, true, false, false, true, true, false },
        new bool[] { false, false, false, true, false, true, false, true, false, false, true, false, true },
        new bool[] { true, false, false, true, false, false, false, true, true, false, false, true, false },
        new bool[] { true, false, false, true, false, false, false, true, true, false, false, false, true },
        new bool[] { false, false, true, true, false, false, true, true, false, false, false, false, true },
        new bool[] { false, false, true, true, false, false, true, false, false, true, false, true, false },
        new bool[] { false, false, true, true, false, false, true, false, false, true, false, false, true },
        new bool[] { false, true, false, true, false, false, true, false, true, false, true, false, false },
        new bool[] { false, true, false, true, false, false, true, false, false, true, false, true, false },
        new bool[] { false, true, false, true, false, false, true, false, false, true, false, false, true },
        new bool[] { false, true, false, true, false, false, true, false, false, false, true, true, false },
        new bool[] { false, true, false, true, false, false, true, false, false, false, true, false, true },
        new bool[] { false, false, false, false, false, true, true, false, false, true, false, true, true },
        new bool[] { true, true, true, false, false, true, false, true, false, false, false, true, false },
        new bool[] { true, true, true, false, false, true, false, true, false, false, false, false, true },
        new bool[] { true, true, true, false, false, true, false, false, false, false, true, false, true },
        new bool[] { true, true, true, false, false, true, false, false, true, false, false, false, true },
        new bool[] { true, true, true, false, false, true, false, false, false, true, false, false, true },
        new bool[] { true, false, true, false, false, true, false, true, false, false, true, true, false },
        new bool[] { true, false, true, false, false, true, false, true, false, false, true, false, true },
        new bool[] { true, false, true, false, false, true, false, false, true, false, true, false, true },
        new bool[] { true, false, true, false, false, true, false, false, false, true, true, false, true },
        new bool[] { false, true, true, false, true, true, false, true, false, false, true, false, false },
        new bool[] { false, true, true, false, true, true, false, true, false, false, false, true, false },
        new bool[] { false, true, true, false, true, true, false, true, false, false, false, false, true },
        new bool[] { false, true, true, false, true, true, false, false, false, false, true, false, true },
        new bool[] { false, true, true, false, true, false, true, false, true, false, false, true, false },
        new bool[] { false, true, true, false, true, true, false, false, true, false, false, false, true },
        new bool[] { false, true, true, false, true, false, true, false, false, true, false, true, false },
        new bool[] { false, true, true, false, true, true, false, false, false, true, false, false, true },
        new bool[] { false, true, true, false, true, false, true, false, false, false, true, true, false },
        new bool[] { false, false, true, false, true, true, false, false, true, false, true, false, true },
        new bool[] { false, false, true, false, true, true, false, false, false, true, true, false, true },
        new bool[] { false, false, true, false, false, false, true, false, true, true, false, true, true },
        new bool[] { false, false, true, false, false, false, true, true, false, true, false, true, true },
        new bool[] { false, false, true, false, false, false, true, false, false, true, true, true, true },
        new bool[] { true, true, false, false, false, true, false, true, false, false, true, false, true },
        new bool[] { true, true, false, false, false, true, false, true, true, false, false, false, true },
        new bool[] { true, true, false, false, false, true, false, true, false, true, false, false, true },
        new bool[] { true, false, false, false, false, true, false, true, true, false, true, false, true },
        new bool[] { true, false, false, false, false, true, false, true, false, true, true, false, true },
        new bool[] { false, true, false, false, true, true, false, true, false, false, true, false, true },
        new bool[] { false, true, false, false, true, true, false, true, true, false, false, false, true },
        new bool[] { false, true, false, false, true, true, false, true, false, true, false, false, true },
        new bool[] { false, false, false, false, true, true, false, true, true, false, true, false, true },
        new bool[] { false, false, false, false, true, true, false, true, false, true, true, false, true },
        new bool[] { true, false, true, false, true, true, false, false, true, false, false, false, true },
        new bool[] { true, false, true, false, true, true, false, false, false, true, false, false, true },
        new bool[] { false, true, true, false, false, true, false, false, false, true, false, true, true },
        new bool[] { true, false, true, true, false, false, false, true, true, true, false, false, false },
        new bool[] { false, false, true, false, true, true, false, false, true, false, false, true, true },
        new bool[] { false, false, true, false, true, true, false, false, false, true, false, true, true },
        new bool[] { false, false, true, false, true, false, false, true, true, false, false, true, true },
        new bool[] { false, false, true, false, true, false, false, true, false, true, false, true, true },
        new bool[] { false, true, true, false, true, false, true, true, false, false, false, true, false },
        new bool[] { false, false, true, false, false, true, false, false, false, true, true, true, true },
        new bool[] { true, false, false, false, true, true, false, true, true, false, false, false, true },
        new bool[] { true, false, false, false, true, true, false, true, false, true, false, false, true },
        new bool[] { false, true, false, false, false, true, false, true, false, true, false, true, true },
        new bool[] { false, false, false, false, true, true, false, true, true, false, false, true, true },
        new bool[] { false, false, false, false, true, true, false, true, false, true, false, true, true },
        new bool[] { false, false, false, false, false, true, false, true, false, true, true, true, true },
        new bool[] { false, true, false, false, false, true, true, false, true, true, false, false, true },
        new bool[] { false, false, true, false, false, true, true, false, true, true, false, false, true },
        new bool[] { false, true, true, false, false, true, false, true, true, true, false, false, true },
        new bool[] { false, true, true, false, false, true, false, false, true, true, true, false, true },
        new bool[] { false, false, true, false, false, true, false, true, true, true, true, false, true },
        new bool[] { false, true, false, false, false, true, false, true, true, true, true, false, true }
    };
}
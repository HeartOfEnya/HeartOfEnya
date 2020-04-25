﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class that contains all the data that has to be saved between states
/// GameState Legend:
/// "A"
/// </summary>
public class PersistentData : MonoBehaviour
{
    public const string gamePhaseIntro = "INTRO";
    public const string gamePhaseTutorial = "A";
    public const string gamePhaseLuaBattle = "B";
    public const string gamePhaseBeginMain = "C";
    public const string gamePhaseLuaUnfrozen = "D";
    public const string gamePhaseAbsoluteZeroBattle = "E";

    public bool InMainPhase => gamePhase == gamePhaseBeginMain || gamePhase == gamePhaseLuaUnfrozen;
    public bool LuaUnfrozen => gamePhase == gamePhaseLuaUnfrozen || gamePhase ==  gamePhaseAbsoluteZeroBattle;

    [Header("Battle")]
    public int partyLevel;
    public Dictionary<string, int> wounds;  // number of times character was hit at 0 hp,
                                            // keys are character names e.g. wounds["Bapy"]
    public bool luaBossDefeated;
    public bool absoluteZeroDefeated;
    public bool absoluteZeroPhase2Defeated;
    public int numEnemiesLeft; // Total in mainEncounter
    public int numEnemiesDefeatedThisEncounter = 0;
    public int waveNum;
    public Encounter lastEncounter; // Used to determing if we are on a new encounter or returning to an old one
    public List<SavedCombatant> listEnemiesLeft = new List<SavedCombatant>(); // data of remaining enemies
    public List<WaveData.SpawnData> listActiveSpawners = new List<WaveData.SpawnData>();
    public List<BuffStruct> buffStructures;  // buffs characters are taking into battle

    [Header("Dialog")]
    public string gamePhase = "A"; // GamePhase: What “day” it is.
    public int dayNum = 0;         // DayNum: The number of days spent on the current Phase
    
    [Header("Soup")]
    public List<string> gatheredIngredients;  // List of gathered ingredients
    // public List<Soup> soupPool;            // List of soups in inventory

    private void Awake()
    {
        // initialize variables
        wounds = new Dictionary<string, int>();
        // buffStructures = new List<
        gatheredIngredients = new List<string>();
        // soupPool = new List<Soup
    }

    [System.Serializable]
    public class SavedCombatant
    {
        public GameObject prefabAsset;
        public int remainingHP;
        public Pos spawnPos;
    }
}

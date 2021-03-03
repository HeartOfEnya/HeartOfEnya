﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelect : MonoBehaviour
{
    public bool goToSoup = true;
    public Encounter mainEncounter = null;
    public FMODUnity.StudioEventEmitter selectSfx;
    private Button[] buttons;

    private void Start()
    {
        var pData = DoNotDestroyOnLoad.Instance.persistentData;
        buttons = GetComponentsInChildren<Button>();
        for (int i = 0; i < buttons.Length - 1; ++i)
        {
            var button = buttons[i];
            int phase = (i + 3) / 4;
            int day = ((i - 1) % 4) / 2;
            if (i % 2 == 0) // Button goes to camp scene
            {
                button.onClick.AddListener(() => GoToCamp(pData.IntToGamePhase(phase), day));
            }
            else // Button goes to battle / soup scene
            {
                button.onClick.AddListener(() => GoToBattle(pData.IntToGamePhase(phase), day));
            }
            button.onClick.AddListener(() => selectSfx.Play());
        }
        var creditsButton = buttons[buttons.Length - 1];
        creditsButton.onClick.AddListener(() => SceneTransitionManager.main.TransitionScenes("Credits"));
        creditsButton.onClick.AddListener(() => selectSfx.Play());
    }

    public void SetGoToSoup(bool b) => goToSoup = b;

    public void GoToBattle(string phase, int dayNum)
    {
        SetPersistantData(phase, dayNum);
        if(goToSoup && !(phase == PersistentData.gamePhaseTut1And2 && dayNum == PersistentData.dayNumStart))
        {
            SceneTransitionManager.main.TransitionScenes("Breakfast");
        }
        else
        {
            SceneTransitionManager.main.TransitionScenes("Battle");
        }
    }

    public void GoToCamp(string phase, int dayNum)
    {
        SetPersistantData(phase, dayNum);
        var pData = DoNotDestroyOnLoad.Instance.persistentData;
        if (phase == PersistentData.gamePhaseIntro)
        {
            SceneTransitionManager.main.TransitionScenes("IntroCamp");
        }
        else if(phase == PersistentData.gamePhaseAbsoluteZeroBattle)
        {
            pData.absoluteZeroPhase1Defeated = true;
            SceneTransitionManager.main.TransitionScenes("OutroCamp");
        }
        else
        {
            if (pData.InLuaBattle)
                pData.luaBossPhase2Defeated = true;
            SceneTransitionManager.main.TransitionScenes("Camp");
        }
    }

    private void SetPersistantData(string phase, int dayNum)
    {
        var pData = DoNotDestroyOnLoad.Instance.persistentData;
        pData.dayNum = dayNum;
        pData.gamePhase = phase;
        if (pData.InMainPhase || pData.LuaUnfrozen)
        {
            pData.luaBossPhase2Defeated = true;
            pData.lastEncounter = mainEncounter;
            if(phase == PersistentData.gamePhaseBeginMain && dayNum == PersistentData.dayNumStart + 1)
            {
                pData.waveNum = 4;
            }
            else if(phase == PersistentData.gamePhaseLuaUnfrozen)
            {
                pData.waveNum = dayNum == PersistentData.dayNumStart ? 8 : 12;
            }
            else if(phase == PersistentData.gamePhaseAbsoluteZeroBattle)
            {
                pData.waveNum = 16;
            }
            int totalEnemies = 0;
            for (int i = mainEncounter.Waves.Length - 1; i >= pData.waveNum; --i)
            {
                totalEnemies += mainEncounter.Waves[i].enemies.Count;
            }
            pData.numEnemiesLeft = totalEnemies;
        }

        //lift any achievement lockouts triggered by retreating during bossfights
        pData.allowAbsoluteVictory = true;
        pData.allowSwiftRescue = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

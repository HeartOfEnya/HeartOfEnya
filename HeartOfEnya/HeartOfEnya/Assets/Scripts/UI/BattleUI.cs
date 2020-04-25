﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[DisallowMultipleComponent]
public class BattleUI : MonoBehaviour
{
    public static BattleUI main;

    public TextMeshProUGUI numEnemiesLeft;

    public Button endTurnButton;

    [Header("Info Panel Fields")]
    public GameObject infoPanel;
    public UIInfoPanelEnemy enemyInfoPanel;
    public UIInfoPanelGeneric genericInfoPanel;
    public UIInfoPanelParty partyInfoPanel;

    [Header("Colors")]
    public Color partyColor;
    public Color neutralColor;
    public Color enemyColor;

    private void Awake()
    {
        if(main == null)
        {
            main = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void UpdateEnemiesRemaining(int numRemaining)
    {
        var pData = DoNotDestroyOnLoad.Instance.persistentData;
        if (pData.InMainPhase || pData.absoluteZeroDefeated)
        {
            numEnemiesLeft.text = numRemaining.ToString() + " Enemies Remain...";
        }
        else
        {
            numEnemiesLeft.text =  "??? Enemies Remain...";
        }

    }

    public void ShowEndTurnButton() => endTurnButton.interactable = true;

    public void HideEndTurnButton() => endTurnButton.interactable = false;

    public void HideInfoPanel()
    {
        infoPanel.SetActive(false);
        enemyInfoPanel.gameObject.SetActive(false);
        genericInfoPanel.gameObject.SetActive(false);
        partyInfoPanel.gameObject.SetActive(false);
    }

    public void ShowInfoPanelEnemy(Enemy e)
    {
        InitializeInfoPanel(e);
        enemyInfoPanel.gameObject.SetActive(true);
        enemyInfoPanel.ShowUI(e);
    }

    public void ShowInfoPanelParty(PartyMember p)
    {
        InitializeInfoPanel(p);
        partyInfoPanel.gameObject.SetActive(true);
        partyInfoPanel.ShowUI(p);
    }

    public void ShowInfoPanelGeneric(Combatant c)
    {
        InitializeInfoPanel(c);
        genericInfoPanel.gameObject.SetActive(true);
        genericInfoPanel.ShowUI(c);
    }

    private void InitializeInfoPanel(Combatant c)
    {
        infoPanel.SetActive(true);
    }
}

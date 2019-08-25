﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MoveCursor))]
public class PartyMember : Combatant
{
    public override Team Allegiance => Team.Party;
    public ActionMenu ActionMenu;
    public bool HasTurn { get; set; } = false;
    private MoveCursor cursor;

    protected override void Initialize()
    {
        base.Initialize();
        cursor = GetComponent<MoveCursor>();
        PhaseManager.main?.PartyPhase.Party.Add(this);
    }

    public override bool Select()
    {
        if(HasTurn)
        {
            cursor.SetActive(true);
            return true;
        }
        return false;
    }

    public override Coroutine StartTurn()
    {
        base.StartTurn();
        cursor.SetActive(true);
        return null;
    }

    public void EndAction()
    {
        var phase = PhaseManager.main.ActivePhase as PartyPhase;
        phase.EndAction(this);
    }

    public void OpenActionMenu()
    {
        ActionMenu.SetActive(true);
    }

    public void CloseActionMenu()
    {
        ActionMenu.SetActive(false);
    }

    public void CancelActionMenu()
    {
        ActionMenu.gameObject.SetActive(false);
        cursor.ResetToLastPosition();
        cursor.SetActive(true);
    }

    public override void OnPhaseStart()
    {
        HasTurn = true;
    }

    public override void Highlight()
    {
        cursor.CalculateTraversable();
        cursor.DisplayTraversable(true);
    }

    public override void UnHighlight()
    {
        cursor.DisplayTraversable(false);
    }
}

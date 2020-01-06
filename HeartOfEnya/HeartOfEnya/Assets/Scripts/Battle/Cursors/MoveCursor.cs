﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[RequireComponent(typeof(PartyMember))]
public class MoveCursor : GridCursor
{
    private Pos lastPosition;
    private PartyMember partyMember;
    private List<Pos> traversable;
    private readonly List<GameObject> squares = new List<GameObject>();
    private void Start()
    {
        partyMember = GetComponent<PartyMember>();
        Pos = partyMember.Pos;
        lastPosition = Pos;
        enabled = false;
    }

    public override void SetActive(bool value)
    {
        enabled = value;
        if(value)
        {
            CalculateTraversable();
        }
        DisplayTraversable(value);
    }

    public void CalculateTraversable()
    {
        lastPosition = partyMember.Pos;
        traversable = BattleGrid.main.Reachable(partyMember.Pos, partyMember.Move, partyMember.CanMoveThrough).Keys.ToList();
        traversable.RemoveAll((p) => !BattleGrid.main.IsEmpty(p));
        traversable.Add(partyMember.Pos);
        Pos = partyMember.Pos;
    }

    public void DisplayTraversable(bool value)
    {
        if(value)
        {
            foreach (var spot in traversable)
                squares.Add(BattleGrid.main.SpawnSquare(spot, BattleGrid.main.moveSquareMat));
        }
        else
        {
            foreach (var obj in squares)
                Destroy(obj);
            squares.Clear();
        }
    }

    public override void Highlight(Pos newPos)
    {
        if (newPos == Pos)
            return;
        if (!traversable.Contains(newPos))
        {
            Pos difference = newPos - Pos;
            if (difference.SquareMagnitude > partyMember.Move * partyMember.Move)
                return;
            if (difference.col > 0)
                ++difference.col;
            else if (difference.col < 0)
                --difference.col;
            else if (difference.row > 0)
                ++difference.row;
            else if (difference.row < 0)
                --difference.row;
            Highlight(Pos + difference);
            return;
        }
        Pos = newPos;
        transform.position = BattleGrid.main.GetSpace(Pos);
    }

    public override void Select()
    {
        lastPosition = partyMember.Pos;
        BattleGrid.main.Move(partyMember, Pos);
        SetActive(false);
        partyMember.OpenActionMenu();
    }

    public void ResetToLastPosition()
    {
        Highlight(lastPosition);
        BattleGrid.main.Move(partyMember, Pos);
    }

    public override void ProcessInput()
    {
        base.ProcessInput();
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ResetToLastPosition();
            SetActive(false);
            PhaseManager.main.PartyPhase.CancelAction(partyMember);
        }

    }
}
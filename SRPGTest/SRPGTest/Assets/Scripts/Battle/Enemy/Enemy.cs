﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Combatant
{
    public override Team Allegiance { get => Team.Enemy; }
    public GameObject squarePrefab;
    private List<Pos> traversable;
    private readonly List<GameObject> squares = new List<GameObject>();

    protected override void Initialize()
    {
        base.Initialize();
        PhaseManager.main?.EnemyPhase.Enemies.Add(this);
    }

    public override void Highlight()
    {
        var traversable = BattleGrid.main.Reachable(Pos, move, CanMoveThrough);
        traversable.RemoveAll((p) => !BattleGrid.main.IsEmpty(p));
        traversable.Add(Pos);
        foreach (var spot in traversable)
            squares.Add(Instantiate(squarePrefab, BattleGrid.main.GetSpace(spot), Quaternion.identity));
    }

    public override void UnHighlight()
    {
        foreach (var obj in squares)
            Destroy(obj);
        squares.Clear();
    }


}

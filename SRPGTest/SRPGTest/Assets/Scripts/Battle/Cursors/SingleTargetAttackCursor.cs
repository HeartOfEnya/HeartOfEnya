﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SingleTargetAttackCursor : SelectNextCursor
{
    public FieldObject.ObjType[] blockedBy;
    public FieldObject.ObjType[] ignore;
    public Combatant attacker;
    private List<GameObject> targetGraphics = new List<GameObject>();

    public void CalculateTargets(int range)
    {
        HideTargets();
        Pos = attacker.Pos;
        SelectionList.Clear();
        var positions = BattleGrid.main.Reachable(Pos, range, blockedBy);
        foreach(var p in positions)
        {
            var obj = BattleGrid.main.GetObject(p) as Combatant;
            if (obj == null || ignore.Any((t) => t == obj.ObjectType))
                continue;
            SelectionList.Add(obj);
        }
        if (SelectionList.Count > 0)
            HighlightFirst();
    }

    public void ShowTargets(int range)
    {
        Pos = attacker.Pos;
        var positions = BattleGrid.main.Reachable(Pos, range, blockedBy);
        foreach (var p in positions)
        {
            var obj = BattleGrid.main.GetObject(p) as Combatant;
            if (obj == null || ignore.Any((t) => t == obj.ObjectType))
                continue;
            targetGraphics.Add(BattleGrid.main.SpawnDebugSquare(p));
        }
    }

    public void HideTargets()
    {
        foreach (var obj in targetGraphics)
            Destroy(obj);
        targetGraphics.Clear();
    }

    public override void Select()
    {
        var target = Selected as Combatant;
        target.Damage(attacker.atk);
        SetActive(false);
        (attacker as PartyMember)?.EndAction();
    }
}

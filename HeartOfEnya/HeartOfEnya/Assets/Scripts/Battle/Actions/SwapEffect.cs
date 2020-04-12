﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwapEffect : ActionEffect
{
    public override IEnumerator ApplyEffect(Combatant user, Combatant target)
    {
        BattleGrid.main.SwapAndSetWorldPos(user, target);
        yield break;
    }
}

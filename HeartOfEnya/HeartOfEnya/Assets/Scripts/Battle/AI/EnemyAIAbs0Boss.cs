﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAIAbs0Boss : AIComponent<Enemy>
{
    public bool secondPhase = false;
    public Action clearObstacles;
    public Action clearObstaclesAndEnemies;
    public Action spawnObstacles;
    public Action moveUnits;
    public Action moveAllToRight;
    private bool wasLastActionResetObstacles = true;
    private bool skipSecondPhaseTurn = true;
    public override IEnumerator DoTurn(Enemy self)
    {
        if(secondPhase)
        {
            if(skipSecondPhaseTurn)
            {
                skipSecondPhaseTurn = false;
                yield break;
            }
            yield return new WaitWhile(() => self.PauseHandle.Paused);
            if (wasLastActionResetObstacles)
            {
                yield return self.UseAction(moveUnits, self.Pos, Pos.Zero);
            }
            else
            {
                yield return self.UseAction(clearObstacles, self.Pos, Pos.Zero);
                yield return self.UseAction(spawnObstacles, self.Pos, Pos.Zero);              
            }
            wasLastActionResetObstacles = !wasLastActionResetObstacles;
        }
        yield return new WaitWhile(() => self.PauseHandle.Paused);
        yield return self.Attack(self.Pos + Pos.Right);
        yield return new WaitWhile(() => self.PauseHandle.Paused);
    }
}

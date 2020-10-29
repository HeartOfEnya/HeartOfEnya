﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPhase : Phase
{
    public override PauseHandle PauseHandle { get; set; } = new PauseHandle(null);

    public List<Enemy> Enemies { get; } = new List<Enemy>();

    public override Coroutine OnPhaseEnd()
    {
        DoNotDestroyOnLoad.Instance.playtestLogger.testData.UpdateAvgEnemies(Enemies.Count);
        RemoveDead();
        Enemies.ForEach((enemy) => enemy.OnPhaseEnd());
        BattleEvents.main.noEnemiesLeftMainPhase._event.Invoke();

        
        FMODBattle.main.EndEnemyTurn();
        return null;
    }

    public override Coroutine OnPhaseStart()
    {
        RemoveDead();
        Enemies.ForEach((enemy) => enemy.OnPhaseStart());
        RemoveDead();
        // Sort by turn order
        Enemies.Sort(CompareTurnOrder);
        SnowParticleController.main.Intensity = 1;
        if(Enemies.Count > 0)
        {
            FMODBattle.main.StartEnemyTurn();
        }
        //FMODBattle.main.music.SetParameter("Text Playing", 1);
        return StartCoroutine(PlayTurns());
    }

    private int CompareTurnOrder(Enemy e1, Enemy e2)
    {
        if (e1.isBoss)
            return 1;
        if (e2.isBoss)
            return -1;
        if (e1.Col != e2.Col)
            return e2.Col.CompareTo(e1.Col);
        return e1.Row.CompareTo(e2.Row);
    }

    public void RemoveDead()
    {
        Enemies.RemoveAll((e) => e == null || (e.Dead && !e.invincible));
    }

    public override void OnPhaseUpdate()
    {
        EndPhase();
    }

    private IEnumerator PlayTurns()
    {
        yield return StartCoroutine(PlayTransition());
        foreach (var enemy in Enemies)
        {
            if (enemy == null || enemy.Dead)
                continue;
            yield return new WaitWhile(() => PauseHandle.Paused);
            if (enemy == null || enemy.Dead)
                continue;
            yield return enemy.StartTurn();
        }               
    }

}

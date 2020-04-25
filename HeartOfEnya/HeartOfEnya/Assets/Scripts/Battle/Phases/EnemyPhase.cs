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

        Enemies.ForEach((enemy) => enemy.OnPhaseEnd());
        return null;
    }

    public override Coroutine OnPhaseStart()
    {
        Enemies.ForEach((enemy) => enemy.OnPhaseStart());
        Enemies.RemoveAll((e) => e == null);
        // Farthest to right takes action first
        Enemies.Sort((e, e2) => e2.Col.CompareTo(e.Col));
        return StartCoroutine(PlayTurns());
    }

    public override void OnPhaseUpdate()
    {
        EndPhase();
    }

    private IEnumerator PlayTurns()
    {
        foreach (var enemy in Enemies)
        {
            if (enemy == null)
                continue;
            yield return new WaitWhile(() => PauseHandle.Paused);
            yield return enemy.StartTurn();
        }               
    }

}

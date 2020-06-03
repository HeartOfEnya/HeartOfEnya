﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleEventsAbs0 : MonoBehaviour
{
    public BattleEvents battleEvents;

    public void Abs0IntroTrigger()
    {
        if (!battleEvents.abs0Battle)
            return;
        if (battleEvents.abs0Intro.flag)
            return;
        battleEvents.abs0Intro.flag = true;
        battleEvents.Pause();
        StartCoroutine(Abs0IntroRoutine());
    }

    private IEnumerator Abs0IntroRoutine()
    {
        var runner = DialogueManager.main.runner;
        runner.StartDialogue("Abs0BossIntro");
        yield return new WaitWhile(() => runner.isDialogueRunning);
        battleEvents.Unpause();
    }

    public void Abs0PhaseChange()
    {
        if (!battleEvents.abs0Battle)
            return;
        if (BattleEvents.main.abs0PhaseChange.flag)
            return;
        var abs0 = BattleGrid.main.Find<Combatant>((c) => c.GetComponent<EnemyAIAbs0Boss>() != null);
        if (abs0 == null || !abs0.Dead)
            return;
        battleEvents.Pause();
        // cancel bonus moves
        foreach (var partyMember in PhaseManager.main.PartyPhase.Party)
        {
            var moveCursor = partyMember.GetComponent<MouseMoveCursor>();
            if(moveCursor.BonusMode)
            {
                moveCursor.CancelBonusMode();
                moveCursor.SetActive(false);
            }
        }
        var pData = DoNotDestroyOnLoad.Instance.persistentData;
        pData.absoluteZeroDefeated = true;
        var aiComponent = abs0.GetComponent<EnemyAIAbs0Boss>();
        aiComponent.secondPhase = true;
        StartCoroutine(Abs0PhaseChangeRoutine(abs0, aiComponent));
    }

    private IEnumerator Abs0PhaseChangeRoutine(Combatant abs0, EnemyAIAbs0Boss aiComponent)
    {
        var pData = DoNotDestroyOnLoad.Instance.persistentData;
        var runner = DialogueManager.main.runner;
        var pManager = PhaseManager.main;
        // Pre-transition
        runner.StartDialogue("Abs0BossPhase2-1");
        yield return new WaitWhile(() => runner.isDialogueRunning);
        // Transition
        abs0.CancelChargingAction();
        yield return abs0.UseAction(aiComponent.clearObstaclesAndEnemies, Pos.Zero, Pos.Zero);
        pManager.SpawnPhase.ClearActiveSpawns();
        yield return abs0.UseAction(aiComponent.moveAllToRight, Pos.Zero, Pos.Zero);
        yield return abs0.UseAction(aiComponent.spawnObstacles, Pos.Zero, Pos.Zero);
        // Revive Abs0
        abs0.Hp = 9;
        if(!pManager.EnemyPhase.Enemies.Contains(abs0 as Enemy))
            pManager.EnemyPhase.Enemies.Add(abs0 as Enemy);
        // Post-transition
        runner.StartDialogue("Abs0BossPhase2-2");
        yield return new WaitWhile(() => runner.isDialogueRunning);
        // Heal the party to full health
        foreach (var partyMember in pManager.PartyPhase.Party)
        {
            partyMember.Hp = partyMember.maxHp;
            partyMember.Fp = partyMember.maxFp;
        }
        yield return StartCoroutine(pManager.SpawnPhase.DeclareNextWave());
        // Update the counter to actually show the number that remain +1 for abs0
        BattleUI.main.UpdateEnemiesRemaining(pData.numEnemiesLeft + 1);
        pManager.NextPhase();
        BattleEvents.main.abs0PhaseChange.flag = true;
        // Unpause here
        battleEvents.Unpause();
    }

    public void Abs0Phase2Defeated()
    {
        if (!battleEvents.abs0Battle)
            return;
        if (BattleEvents.main.abs0Phase2Defeated.flag || !BattleEvents.main.abs0PhaseChange.flag)
            return;
        var abs0 = BattleGrid.main.Find<Combatant>((c) => c.GetComponent<EnemyAIAbs0Boss>() != null);
        if (abs0 == null || !abs0.Dead)
            return;
        battleEvents.Pause();
        abs0.invincible = false;
        abs0.Damage(10);
        var pData = DoNotDestroyOnLoad.Instance.persistentData;
        pData.absoluteZeroPhase2Defeated = true;
        StartCoroutine(Abs0Phase2DefeatedRoutine());
    }

    private IEnumerator Abs0Phase2DefeatedRoutine()
    {
        var runner = DialogueManager.main.runner;
        runner.StartDialogue("Abs0BossOutro");
        yield return new WaitWhile(() => runner.isDialogueRunning);
        SceneTransitionManager.main.TransitionScenes("OutroCamp");
        battleEvents.Unpause();
    }
}

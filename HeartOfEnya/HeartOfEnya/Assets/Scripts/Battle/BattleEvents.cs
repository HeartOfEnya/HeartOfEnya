﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Yarn.Unity;

public class BattleEvents : MonoBehaviour
{
    // struct wrapper for unity event containing flag
    [System.Serializable]
    public struct BattleEvent
    {
        public UnityEvent _event;
        public bool flag;
    }

    public static BattleEvents main;
    private bool tutorial; // whether we are in the tutorial or not

    public BattleEvent tutorialIntro;
    public BattleEvent tutMove;
    public BattleEvent tutRainaAttack;
    public BattleEvent tutBapySelect;
    public BattleEvent tutBapyCancel;
    public BattleEvent tutSoleilSelect;
    public BattleEvent tutSoleilAttack;
    public BattleEvent tutSoleilChargeReminder;
    public BattleEvent tutSoleilChargeExplanation;
    public BattleEvent tutEnemySpawnWarning;
    public BattleEvent tutEnemySpawn;
    public BattleEvent tutEnemyInfo;
    public BattleEvent tutEnemyRanged;
    public BattleEvent tutDD;

    // references to objects that will be needed to disable certain game functions
    private PartyPhase partyPhase;

    private void Awake()
    {
        if (main == null)
        {
            main = this;
        }
        else
        {
            Destroy(gameObject);
        }   
    }

    private void Start()
    {
        tutorial = (DoNotDestroyOnLoad.Instance.persistentData.gamePhase == PersistentData.gamePhaseTutorial);
        partyPhase = GameObject.Find("PartyPhase").GetComponent<PartyPhase>();
    }

    public void IntroTrigger()
    {
        if(tutorial && !tutorialIntro.flag)
        {
            Debug.Log("Battle Triggers: start of battle");
            tutorialIntro.flag = true;
            // Start the dialog (connect to ambers code)
            // Wait for finish StartCoroutine(IntroTriggerPost(runner))

            // (this code should be moved into the post function once dialog merged)
            // post-condition: disable everyone but raina
            string[] units = {"Bapy", "Soleil"};
            partyPhase.DisableUnits(new List<string>(units));
        }
    }

    private IEnumerator IntroTriggerPost(DialogueRunner runner)
    {
        yield return new WaitWhile(() => runner.isDialogueRunning);
        // put the post-code here
    }

    public void MoveTrigger()
    {
        if(tutorial && !tutMove.flag)
        {
            Debug.Log("Battle Triggers: raina move");
            tutMove.flag = true;
        }
    }

    private IEnumerator MoveTriggerPost(DialogueRunner runner)
    {
        yield return new WaitWhile(() => runner.isDialogueRunning);
        // put the post-code here
    }

    public void RainaAttackTrigger()
    {
        if(tutorial && !tutRainaAttack.flag)
        {
            Debug.Log("Battle Triggers: raina attack");
            tutRainaAttack.flag = true;
        }
    }

    private IEnumerator RainaAttackTriggerPost(DialogueRunner runner)
    {
        yield return new WaitWhile(() => runner.isDialogueRunning);
        // put the post-code here
    }

    public void BapySelectTrigger()
    {
        if(tutorial && !tutBapySelect.flag)
        {
            Debug.Log("Battle Triggers: select bapy");
            tutBapySelect.flag = true;

            // re-enable bapy's turn
            partyPhase.EnableUnits("Bapy");
        }
    }

    private IEnumerator BapySelectTriggerPost(DialogueRunner runner)
    {
        yield return new WaitWhile(() => runner.isDialogueRunning);
        // put the post-code here
    }

    public void BapyCancelTrigger()
    {
        if(tutorial && !tutBapyCancel.flag)
        {
            Debug.Log("Battle Triggers: bapy cancel");
            tutBapyCancel.flag = true;
        }
    }

    private IEnumerator BapyCancelTriggerPost(DialogueRunner runner)
    {
        yield return new WaitWhile(() => runner.isDialogueRunning);
        // put the post-code here
    }

    public void SoleilSelectTrigger()
    {
        if(tutorial && !tutSoleilSelect.flag)
        {
            Debug.Log("Battle Triggers: select soleil");
            tutSoleilSelect.flag = true;

            // enable soleil
            partyPhase.EnableUnits("Soleil");
            
            // disable bapy
            partyPhase.DisableUnits("Bapy");
        }
    }

    private IEnumerator SoleilSelectTriggerPost(DialogueRunner runner)
    {
        yield return new WaitWhile(() => runner.isDialogueRunning);
        // put the post-code here
    }

    public void SoleilAttackTrigger()
    {
        if(tutorial && !tutSoleilAttack.flag)
        {
            Debug.Log("Battle Triggers: soleil attack");
            tutSoleilAttack.flag = true;

            // re-enable bapy (and everyone else since their turns are over anyway)
            string[] units = {"Bapy", "Soleil", "Raina"};
            partyPhase.EnableUnits(new List<string>(units));
        }
    }

    private IEnumerator SoleilAttackTriggerPost(DialogueRunner runner)
    {
        yield return new WaitWhile(() => runner.isDialogueRunning);
        // put the post-code here
    }

    public void SoleilChargeReminderTrigger()
    {
        if(tutorial && !tutSoleilChargeReminder.flag)
        {
            Debug.Log("Battle Triggers: SoleilChargeReminder");
            tutSoleilChargeReminder.flag = true;
        }
    }

    private IEnumerator SoleilChargeReminderTriggerPost(DialogueRunner runner)
    {
        yield return new WaitWhile(() => runner.isDialogueRunning);
        // put the post-code here
    }

    public void SoleilChargeExplanationTrigger()
    {
        if(tutorial && !tutSoleilChargeExplanation.flag)
        {
            Debug.Log("Battle Triggers: SoleilChargeExplanation");
            tutSoleilChargeExplanation.flag = true;
        }
    }

    private IEnumerator SoleilChargeExplanationTriggerPost(DialogueRunner runner)
    {
        yield return new WaitWhile(() => runner.isDialogueRunning);
        // put the post-code here
    }

    public void EnemySpawnWarningTrigger()
    {
        if(tutorial && !tutEnemySpawnWarning.flag)
        {
            Debug.Log("Battle Triggers: EnemySpawnWarning");
            tutEnemySpawnWarning.flag = true;
        }
    }

    private IEnumerator EnemySpawnWarningTriggerPost(DialogueRunner runner)
    {
        yield return new WaitWhile(() => runner.isDialogueRunning);
        // put the post-code here
    }

    public void EnemySpawnTrigger()
    {
        if(tutorial && !tutEnemySpawn.flag)
        {
            Debug.Log("Battle Triggers: EnemySpawn");
            tutEnemySpawn.flag = true;
        }
    }

    private IEnumerator EnemySpawnTriggerPost(DialogueRunner runner)
    {
        yield return new WaitWhile(() => runner.isDialogueRunning);
        // put the post-code here
    }

    public void EnemyInfoTrigger()
    {
        if(tutorial && !tutEnemyInfo.flag)
        {
            Debug.Log("Battle Triggers: EnemyInfo");
            tutEnemyInfo.flag = true;
        }
    }

    private IEnumerator EnemyInfoTriggerPost(DialogueRunner runner)
    {
        yield return new WaitWhile(() => runner.isDialogueRunning);
        // put the post-code here
    }

    public void EnemyRangedTrigger()
    {
        if(tutorial && !tutEnemyRanged.flag)
        {
            Debug.Log("Battle Triggers: EnemyRanged");
            tutEnemyRanged.flag = true;
        }
    }

    private IEnumerator EnemyRangedTriggerPost(DialogueRunner runner)
    {
        yield return new WaitWhile(() => runner.isDialogueRunning);
        // put the post-code here
    }

    public void DeathsDoorTrigger()
    {
        if(!tutDD.flag)
        {
            Debug.Log("Battle Triggers: DeathsDoor");
            tutDD.flag = true;
        }
    }

    private IEnumerator DeathsDoorTriggerPost(DialogueRunner runner)
    {
        yield return new WaitWhile(() => runner.isDialogueRunning);
        // put the post-code here
    }
}

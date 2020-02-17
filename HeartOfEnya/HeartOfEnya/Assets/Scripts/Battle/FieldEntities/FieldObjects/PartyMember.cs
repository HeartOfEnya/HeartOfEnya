﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// A Combatant that is a member of the party.
/// Adds the Fp stat, along with functionality to track and display turn status and active action menus.
/// </summary>
[RequireComponent(typeof(MoveCursor))]
public class PartyMember : Combatant, IPausable
{
    public const int maxDeathsDoorCounter = 5;
    public PauseHandle PauseHandle { get; set; }
    public override bool Stunned
    {
        get => stunned;
        set
        {
            base.Stunned = value;
            // Remove the ability to take a turn if just stunned and the unit still had a turn
            if (value && HasTurn)
            {
                HasTurn = false;
            }
        }
    }
    // Party members are always on the Party team
    public override Teams Team => Teams.Party;
    /// <summary>
    /// The number of Fp (Flame Points) the unit currently has
    /// Setting this number will automatically update the Fp UI.
    /// </summary>
    public int Fp
    {
        get => fp;
        set
        {
            fp = value;
            fpText.text = fp.ToString();
        }
    }
    private int fp;
    public TextMeshProUGUI fpText;
    public GameObject deathsDoorUI;
    public TextMeshProUGUI deathsDoorCounterText;
    [SerializeField]
    private SpriteRenderer sprite;
    [SerializeField]
    private Character chara;
    [Header("Party Member Specific Fields")]
    public ActionMenu ActionMenu;
    public AttackCursor attackCursor;
    public int maxFp;
    public int level;
    public override Sprite DisplaySprite => chara.Portrait;
    public override Color DisplaySpriteColor => Color.white;

    private FMODUnity.StudioEventEmitter battleTheme;

    public bool DeathsDoor { get; private set; }
    
    public int DeathsDoorCounter
    {
        get => deathsDoorCounter;
        set
        {
            deathsDoorCounter = Mathf.Max(0,value);
            deathsDoorCounterText.text = deathsDoorCounter.ToString();
            if (deathsDoorCounter == 1)
                deathsDoorUI.GetComponent<Image>().color = Color.red;
            else if (deathsDoorCounter <= 0)
                Kill();
        }
    }
    private int deathsDoorCounter;

    /// <summary>
    /// Can this unit still take an action this turn?
    /// Updates the UI when set. Current effect just makes the unit semi-transparent.
    /// </summary>
    public bool HasTurn
    {
        get => hasTurn;
        set
        {
            hasTurn = value;
            if (value)
            {
                sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, 1f);
            }
            else
            {
                sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, 0.5f);
            }
        }
    }
    private bool hasTurn = false;

    public bool RanAway { get; private set; }

    private MoveCursor moveCursor;
    private MouseMoveCursor mouseMoveCursor;

    protected override void Initialize()
    {
        // Initialize base combatant logic
        base.Initialize();
        moveCursor = GetComponent<MoveCursor>();
        mouseMoveCursor = GetComponent<MouseMoveCursor>();
        // Add this unit to the party phase
        PhaseManager.main?.PartyPhase.Party.Add(this);
        // Add this unit to the party phase's pause dependents, so this unit is paused when the party phase is paused
        PhaseManager.main?.PartyPhase.PauseHandle.Dependents.Add(this);
        Fp = maxFp;
        DeathsDoorCounter = maxDeathsDoorCounter;
        // Initialize the pause handle with the cursors and action menu as dependents
        PauseHandle = new PauseHandle(null, moveCursor, mouseMoveCursor, attackCursor, ActionMenu);

        // Find reference to FMOD event emitter
        battleTheme = GameObject.Find("FMODEventEmitter").GetComponent<FMODUnity.StudioEventEmitter>();
        battleTheme.SetParameter("Loading", 0);
    }


    public override void Damage(int damage)
    {
        Hp = Mathf.Max(0, Hp - damage);
        if (damage > 0 && Dead)
        {
            if (!DeathsDoor)
                EnterDeathsDoor();
            else
                --DeathsDoorCounter;
        }
    }

    /// <summary>
    /// Changes a character to death's door. adds FMOD integration to change
    /// the battle theme when a unit's HP reaches 0
    /// </summary>
    public void EnterDeathsDoor()
    {
        DeathsDoor = true;
        Debug.Log(DisplayName + "Has Enetered Death's Door");
        battleTheme.SetParameter("Crisis", 1);
        deathsDoorUI.SetActive(true);
        CancelChargingAction();
    }

    public override void Kill()
    {
        Debug.Log(DisplayName + " has died");
        // Decide waht to do here but just do this for now.
        SceneTransitionManager.main.TransitionScenes("testBreakfast");
    }

    private void OnDestroy()
    {
        PhaseManager.main?.PartyPhase.PauseHandle.Dependents.Remove(this);
    }

    /// <summary>
    /// Start the unit's turn by activating the Movecursor
    /// If HasTurn == false, do nothing
    /// </summary>
    /// <returns> HasTurn </returns>
    public override bool Select()
    {
        if(HasTurn)
        {
            // moveCursor.SetActive(true);
            mouseMoveCursor.SetActive(true);

            return true;
        }
        return false;
    }
    
    /// <summary>
    /// Fully end the unit's turn after taking an action.
    /// </summary>
    public void EndTurn()
    {
        HasTurn = false;
        PhaseManager.main.PartyPhase.EndAction(this);
    }

    public void OpenActionMenu()
    {
        ActionMenu.SetActive(true);
    }

    /// <summary>
    /// Close the action menu and return to the movement phase of a turn.
    /// Should only be called when the action menu is open
    /// </summary>
    public void CancelActionMenu()
    {
        ActionMenu.gameObject.SetActive(false);
        // moveCursor.ResetToLastPosition();
        // moveCursor.SetActive(true);
        mouseMoveCursor.ResetToLastPosition();
        mouseMoveCursor.SetActive(true);
    }

    new public void ActivateChargedAction()
    {
        StartCoroutine(ActivateChargedActionCr());
    }

    private IEnumerator ActivateChargedActionCr()
    {
        yield return base.ActivateChargedAction();
        EndTurn();
    }

    /// <summary>
    /// Allow the unit to take an action this turn (if not stunned)
    /// </summary>
    public override void OnPhaseStart()
    {
        HasTurn = !Stunned;
        //if (HasTurn && !IsChargingAction && Fp < maxFp)
        //    ++Fp;
    }

    public override void OnPhaseEnd()
    {
        var sprite = GetComponent<SpriteRenderer>();
        if (sprite != null)
            sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, 1);
        if(Stunned)
        {
            Stunned = false;
            if (stunIsBurn)
                Damage(1);
        }            
        if (DeathsDoor)
            --DeathsDoorCounter;
    }

    /// <summary>
    /// If the party member still take a turn this phase, calculate the movement range and display it.
    /// </summary>
    public override void Highlight()
    {
        if (stunned || !hasTurn)
            return;
        // moveCursor.CalculateTraversable();
        // moveCursor.DisplayTraversable(true);
        mouseMoveCursor.CalculateTraversable();
        mouseMoveCursor.DisplayTraversable(true);
        BattleUI.main.ShowInfoPanelParty(this);
    }

    // Hide the movement range display
    public override void UnHighlight()
    {
        // moveCursor.DisplayTraversable(false);
        mouseMoveCursor.DisplayTraversable(false);
        BattleUI.main.HideInfoPanel();
    }

    /// <summary>
    /// Use the special action: run. Simply destroys the unit for now
    /// </summary>
    public void Run()
    {
        RanAway = true;
        EndTurn();
        Destroy(gameObject);
    }

    /// <summary>
    /// Use the action and reduce Fp if the action has an ActionFpCost component
    /// </summary>
    public override Coroutine UseAction(Action action, Pos targetPos)
    {
        //Hide the info panel;
        BattleUI.main.HideInfoPanel();
        var routine = base.UseAction(action, targetPos);
        var fpCost = action.GetComponent<ActionFpCost>();
        if(fpCost != null)
            Fp -= fpCost.fpCost;
        return routine;
    }
}

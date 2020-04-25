﻿using RandomUtils;
using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;

public class MoveEffectRandomDirection : ActionEffect
{
    public enum Direction
    {
        Right,
        Left,
        Up,
        Down,
    }

    public List<Direction> directionChoices;
    public List<float> weights;

    public int squares = 1;
    public int moveDamage = 2; //how much damage do we take if pushed into world border/immovable object?

    private Dictionary<Direction, Pos> directionTranslator = new Dictionary<Direction, Pos>
    {
        {Direction.Right, Pos.Right },
        {Direction.Up, Pos.Up },
        {Direction.Left, Pos.Left },
        {Direction.Down, Pos.Down },
    };


    public override IEnumerator ApplyEffect(Combatant user, Combatant target, ExtraData data)
    {
        //quick test - if initial target is immovable, what should we do?
        if(target != null && !target.isMovable)
        {
            // yield break; // uncomment to just abort always
            // Both the target and user are immovable. Break.
            if (!user.isMovable)
            {
                Debug.Log("Target and user are immovable. Aborting.");
                yield break;
            }

        	//If we don't want to just fail, swap things around so that if A tries to push B (who's immobile),
        	//it becomes B pushes A (e.g. if Bapy pushes a pillar, Bapy gets pushed backwards) 
        	Combatant temp = user;
        	user = target;
        	target = temp;
        }

        Pos direction = directionTranslator[RandomU.instance.Choice(directionChoices, weights)];
        yield return StartCoroutine(DoMove(user, target, direction, moveDamage));
    }
}



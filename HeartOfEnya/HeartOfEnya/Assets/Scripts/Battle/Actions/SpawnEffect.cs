using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnEffect : ActionEffect
{
    public FieldObject objPrefab;
    public override IEnumerator ApplyEffect(Combatant user, Combatant target, ExtraData data)
    {
        Debug.Log(user.DisplayName + " can't spawn " + objPrefab.DisplayName + " in an occupied square!");
        yield break;
    }

    public override IEnumerator ApplyEffect(Combatant user, Pos target, ExtraData data)
    {
        var obj = Instantiate(objPrefab.gameObject, BattleGrid.main.GetSpace(target), Quaternion.identity);
        var fObj = obj.GetComponent<FieldObject>();
        BattleGrid.main.SetObject(target, fObj);
        //if the entity in question was a snowbank, unlock the "Snow Friends" achievement
        if (!AchievementManager.main.IsCompleted(AchievementManager.AchievementID.SNOW_FRIENDS))
        {    if (fObj is Obstacle) //if it's not an obstacle, it's definitely not a snowbank
            {
                //Probably not the best way to test for snowbank, but it should work well enough and building a better system is probably out-of-scope at this point.
                //Also, no need to check who cast the spell since only Bapy can make snowbanks
                if ((fObj.DisplayName == "Snowbank") || (fObj.DisplayName == "Big Snowbank"))
                {
                    AchievementManager.main.CompleteAchievement(AchievementManager.AchievementID.SNOW_FRIENDS);
                }
            }
        }
        yield return new WaitForSeconds(effectWaitTime);
    }
}

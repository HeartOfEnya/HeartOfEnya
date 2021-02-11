using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;
using System;

public class AchievementManager : MonoBehaviour
{
    public enum AchievementID
    {
        SOUPS_ON,
        ICE_AND_FIRE,
        BOXED_OUT,
        SNOW_FRIENDS,
        OPEN_HEART,
        COLD_HEART,
        BRIGHT_HEART,
        DAZZLING_HEART,
        BLAZING_HEART,
        DARING_ESCAPE,
        SAVE_THE_WORLD,
        SWIFT_RESCUE,
        DEFROSTING,
        BUT_NOBODY_CAME,
        ABSOLUTE_VICTORY,
    }

    public static AchievementManager main;

    private readonly Dictionary<AchievementID, Achievement> achievements = new Dictionary<AchievementID, Achievement>()
    {
        { AchievementID.SOUPS_ON, new Achievement() },
        { AchievementID.ICE_AND_FIRE, new Achievement() },
        { AchievementID.BOXED_OUT, new Achievement() },
        { AchievementID.SNOW_FRIENDS, new Achievement() },
        { AchievementID.OPEN_HEART, new Achievement() },
        { AchievementID.COLD_HEART, new Achievement() },
        { AchievementID.BRIGHT_HEART, new Achievement() },
        { AchievementID.DAZZLING_HEART, new Achievement() },
        { AchievementID.BLAZING_HEART, new Achievement() },
        { AchievementID.DARING_ESCAPE, new Achievement() },
        { AchievementID.SAVE_THE_WORLD, new Achievement() },
        { AchievementID.SWIFT_RESCUE, new Achievement() },
        { AchievementID.DEFROSTING, new Achievement() },
        { AchievementID.BUT_NOBODY_CAME, new Achievement() },
        { AchievementID.ABSOLUTE_VICTORY, new Achievement() },
    };

    private CGameID gameID;
    private Callback<UserStatsReceived_t> onUserStatsReceived;
    private void Awake()
    {
        if(main == null)
        {
            main = this;
            DontDestroyOnLoad(transform);
        }
        else
        {
            Destroy(gameObject);
        }
    }


    void OnEnable()
    {
        if (!SteamManager.Initialized)
            return;

        // Cache the GameID for use in the Callbacks
        gameID = new CGameID(SteamUtils.GetAppID());
        onUserStatsReceived = Callback<UserStatsReceived_t>.Create(OnUserStatsReceived);
        // Request the current user stats
        SteamUserStats.RequestCurrentStats();
    }

    public bool IsCompleted(AchievementID id)
    {
        if (!achievements.ContainsKey(id))
            return false;
        return achievements[id].completed;
    }

    public void CompleteAchievement(AchievementID id)
    {
        if (!achievements.ContainsKey(id))
            return;
        var achievement = achievements[id];
        if (achievement.completed)
            return;
        // Complete achievement here
        achievement.completed = true;
        SteamUserStats.SetAchievement(id.ToString());
        SteamUserStats.StoreStats();
    }

    private void OnUserStatsReceived(UserStatsReceived_t pCallback)
    {
        if (!SteamManager.Initialized)
            return;
        if (pCallback.m_nGameID != (ulong)gameID)
        {
            Debug.Log("RequestStats - call back from other game: " + pCallback.m_nGameID);
            return;
        }
        if(pCallback.m_eResult != EResult.k_EResultOK)
        {
            Debug.Log("RequestStats - failed: " + pCallback.m_eResult);
        }
        foreach (var kvp in achievements)
        {
            var ach = kvp.Value;
            var ID = kvp.Key.ToString();
            bool ret = SteamUserStats.GetAchievement(ID, out ach.completed);
            if (ret)
            {
                ach.Name = SteamUserStats.GetAchievementDisplayAttribute(ID, "name");
                ach.Description = SteamUserStats.GetAchievementDisplayAttribute(ID, "desc");
            }
            else
            {
                Debug.LogWarning("SteamUserStats.GetAchievement failed for Achievement " + ID + "\nIs it registered in the Steam Partner site?");
            }
        }
    }
#if DEBUG
    //-----------------------------------------------------------------------------
    // Purpose: Display the user's stats and achievements
    //-----------------------------------------------------------------------------
    private void OnGUI()
    {
        if (!SteamManager.Initialized)
        {
            GUILayout.Label("Steamworks not Initialized");
            return;
        }

        GUILayout.BeginArea(new Rect(Screen.width - 300, 0, 300, 1000));
        GUI.contentColor = Color.green;
        // FOR TESTING PURPOSES ONLY!
        if (GUILayout.Button("RESET STATS AND ACHIEVEMENTS"))
        {
            SteamUserStats.ResetAllStats(true);
            SteamUserStats.RequestCurrentStats();
        }
        // FOR TESTING PURPOSES ONLY!
        if (GUILayout.Button("UNLOCK FIRST ACHIEVEMENT"))
        {
            CompleteAchievement(AchievementID.ABSOLUTE_VICTORY);
        }
        foreach (var kvp in achievements)
        {
            var ach = kvp.Value;
            GUILayout.Label(kvp.Key.ToString() + (ach.completed ? " (Achieved)" : " (NOT Achieved)"));
            //GUILayout.Label(ach.Name + " - " + ach.Description);
            GUILayout.Space(10);
        }

        GUILayout.EndArea();
    }
#endif
    private class Achievement
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool completed;
    }
}

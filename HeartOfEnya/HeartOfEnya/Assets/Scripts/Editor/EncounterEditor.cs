﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;

public class EncounterEditor : EditorWindow
{
    public const string encounterFolderPath = "Assets/ScriptableObjects/Encounters/";
    public const string assetSuffix = ".asset";
    public const string levelEditorScenePath = "Assets/Scenes/LevelEditor/LevelEditor.unity";
    // Wave editing properties
    public Encounter loadedEncounter = null;
    public string newFileName = string.Empty;

    [MenuItem("Window/Encounter Editor")]
    public static void ShowWindow()
    {
        GetWindow(typeof(EncounterEditor));
    }
    private void OnGUI()
    {
        // If there is no grid, we aren't in a battle scene or this one is improperly configured
        if(EditorSceneManager.GetActiveScene().name != "LevelEditor")
        {
            EditorGUILayout.HelpBox("You are not in the LevelEditor scene!", MessageType.Error);
            if(GUILayout.Button(new GUIContent("Go to Level Editor Scene")))
            {
                if(EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                {
                    EditorSceneManager.OpenScene(levelEditorScenePath);
                }              
            }
            return;
        }
        if (BattleGrid.main == null)
        {
            EditorGUILayout.HelpBox("No detected battle grid. Please reload scene or add one", MessageType.Error);
            return;
        }

        #region Save / Load

        GUILayout.BeginVertical("Box");
        EditorGUILayout.LabelField(new GUIContent("Save / Load Encounter"), EditorUtils.BoldCentered);
        var oldEncounter = loadedEncounter;
        loadedEncounter = EditorUtils.ObjectField(new GUIContent("Loaded Encounter"), loadedEncounter, false);
        if(oldEncounter != loadedEncounter)
        {
            if (loadedEncounter != null)
                LoadEncounter(loadedEncounter);
        }
        if(loadedEncounter == null)
        {
            EditorGUILayout.HelpBox("No loaded ecnounter. Set the loaded encounter field or use Create New to create a new asset", MessageType.Info);
        }
        else if(GUILayout.Button(new GUIContent("Save")))
        {
            SaveEncounter(loadedEncounter);
        }
        GUILayout.EndVertical();

        #endregion

        #region Create New / Save As Copy

        GUILayout.BeginVertical("Box");
        EditorGUILayout.LabelField(new GUIContent("Copy / Create New Encounter"), EditorUtils.BoldCentered);
        newFileName = EditorGUILayout.TextField(new GUIContent("New File Name"), newFileName);
        if (newFileName == string.Empty)
        {
            EditorGUILayout.HelpBox("New Filename is empty. To create new encounter or save a copy set the New File Name field", MessageType.Info);
        }
        else if (AssetDatabase.LoadAssetAtPath<Encounter>(encounterFolderPath + newFileName + assetSuffix) != null)
        {
            EditorGUILayout.HelpBox("There is already a WaveData asset named " + newFileName + ". Please choose a different name.", MessageType.Warning);
        }
        else if (loadedEncounter == null)
        {
            if (GUILayout.Button("Create New Encounter"))
            {
                var newEncounter = CreateNewEncounter(newFileName);
                loadedEncounter = newEncounter;
                newFileName = string.Empty;
            }
        }
        else
        {
            if (GUILayout.Button(new GUIContent("Save as Copy")))
            {
                var newEncounter = CreateNewEncounter(newFileName);
                SaveEncounter(newEncounter);
                newFileName = string.Empty;
            }
            if (GUILayout.Button(new GUIContent("Save as Copy + Load")))
            {
                var newEncounter = CreateNewEncounter(newFileName);
                SaveEncounter(newEncounter);
                loadedEncounter = newEncounter;
                newFileName = string.Empty;
            }

        }
        GUILayout.EndVertical();

        #endregion

        #region Wave Property Editor

        if (loadedEncounter != null)
        {
            EditEncounterProperties(loadedEncounter);
        }

        #endregion

    }

    #region Helper Methods

    void EditEncounterProperties(Encounter encounter)
    {
        GUILayout.BeginVertical("Box");
        EditorGUILayout.LabelField(new GUIContent("Encounter Properties"), EditorUtils.BoldCentered);
        EditorGUI.BeginChangeCheck();

        if (EditorGUI.EndChangeCheck())
        {
            EditorUtility.SetDirty(encounter);
        }
        GUILayout.EndVertical();
    }

    Encounter CreateNewEncounter(string name)
    {
        var encounter = ScriptableObject.CreateInstance<Encounter>();
        AssetDatabase.CreateAsset(encounter, encounterFolderPath + name + assetSuffix);
        return encounter;
    }

    void LoadEncounter(Encounter encounter)
    {

    }

    void SaveEncounter(Encounter encounter)
    {
        EditorUtility.SetDirty(encounter);
    }

    #endregion
}

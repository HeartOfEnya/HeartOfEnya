﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using SerializableCollections.EditorGUIUtils;

[CustomEditor(typeof(CharacterData))]
public class CharacterDataInspector : Editor
{
    private CharacterData data;
    private string toAdd;
    private void OnEnable()
    {
        data = target as CharacterData;
    }
    public override void OnInspectorGUI()
    {
        data.voiceEvent = EditorGUILayout.TextField(new GUIContent("Voice Event"), data.voiceEvent);
        data.dialogBoxPrefab = EditorUtils.ObjectField(new GUIContent("Dialog Box Prefab"), data.dialogBoxPrefab, false);
        data.dialogBoxSolo = EditorUtils.ObjectField(new GUIContent("Dialog Box Prefab (Solo)"), data.dialogBoxSolo, false);
        data.dialogBoxBattle = EditorUtils.ObjectField(new GUIContent("Dialog Box Prefab (Battle)"), data.dialogBoxBattle, false);
        data.soloBackground = EditorUtils.ObjectField(new GUIContent("Solo Scene Background"), data.soloBackground, false);
        data.portraits.DoGUILayout(data.portraits.ValueGUIObj, () => data.portraits.StringAddGUID(ref toAdd), "Portraits", true);
        if (GUI.changed)
            EditorUtility.SetDirty(data);
    }
}

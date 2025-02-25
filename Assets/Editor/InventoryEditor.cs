using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Inventory))]
public class InventoryEditor : Editor
{
    public override void OnInspectorGUI()
    {
        Inventory inventory = (Inventory)target;

        EditorGUILayout.LabelField("Unlocked Cards", EditorStyles.boldLabel);
        EditorGUI.indentLevel++;

        foreach (var card in inventory.unlockedCards)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField($"ID: {card.ID}");
            EditorGUILayout.LabelField($"Name: {card.CardName}");
            EditorGUILayout.EndHorizontal();
        }

        EditorGUI.indentLevel--;
        
        if (GUI.changed)
        {
            EditorUtility.SetDirty(target);
        }
    }
} 
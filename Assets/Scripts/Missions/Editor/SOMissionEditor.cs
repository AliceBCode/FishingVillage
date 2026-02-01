#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SOMission))]
public class SOMissionEditor : Editor
{
    private SerializedProperty nameProp;
    private SerializedProperty descriptionProp;
    private SerializedProperty iconProp;
    private SerializedProperty objectivesProp;
    private SerializedProperty actionsOnMissionStartedProp;
    private SerializedProperty actionsOnMissionCompletedProp;
    private SerializedProperty objectiveEventsProp;

    private void OnEnable()
    {
        nameProp = serializedObject.FindProperty("name");
        descriptionProp = serializedObject.FindProperty("description");
        iconProp = serializedObject.FindProperty("icon");
        objectivesProp = serializedObject.FindProperty("objectives");
        actionsOnMissionStartedProp = serializedObject.FindProperty("onStarted");
        actionsOnMissionCompletedProp = serializedObject.FindProperty("onCompleted");
        objectiveEventsProp = serializedObject.FindProperty("onObjectiveCompleted");
    }

public override void OnInspectorGUI()
{
    serializedObject.Update();
    
    EditorGUILayout.PropertyField(nameProp, new GUIContent("Name"));
    EditorGUILayout.PropertyField(descriptionProp, new GUIContent("Description"));
    EditorGUILayout.PropertyField(iconProp, new GUIContent("Icon"));
    EditorGUILayout.PropertyField(objectivesProp, new GUIContent("Objectives"));
    
    EditorGUILayout.PropertyField(actionsOnMissionStartedProp, new GUIContent("On Mission Started"));
    
    var mission = target as SOMission;
    if (mission)
    {
        var objectives = mission.CloneObjectives();
        
        // Filter out null objectives
        var validObjectives = new System.Collections.Generic.List<MissionObjective>();
        var validIndices = new System.Collections.Generic.List<int>();
        
        for (int i = 0; i < objectives.Length; i++)
        {
            if (objectives[i] != null)
            {
                validObjectives.Add(objectives[i]);
                validIndices.Add(i);
            }
        }
        
        // Resize array to match valid objectives count
        if (objectiveEventsProp.arraySize != validObjectives.Count)
        {
            objectiveEventsProp.arraySize = validObjectives.Count;
        }
    
        if (validObjectives.Count > 0)
        {
            for (int i = 0; i < validObjectives.Count; i++)
            {
                var objective = validObjectives[i];
                int originalIndex = validIndices[i];
                
                var entryProp = objectiveEventsProp.GetArrayElementAtIndex(i);
                var actionsOnCompletedProp = entryProp.FindPropertyRelative("onObjectiveCompleted");
                
                EditorGUILayout.Space(5);
                EditorGUILayout.PropertyField(actionsOnCompletedProp, new GUIContent($"On Objective {originalIndex}: {objective.Description}"));
                EditorGUILayout.Space(5);
            }
        }
        else
        {
            EditorGUILayout.HelpBox("Add objectives above to configure their completion actions", MessageType.Info);
        }
    }
    
    EditorGUILayout.PropertyField(actionsOnMissionCompletedProp, new GUIContent("On Mission Completed"));

    serializedObject.ApplyModifiedProperties();
}
}
#endif
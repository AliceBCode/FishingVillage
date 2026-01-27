#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MissionEvents))]
public class MissionEventsEditor : Editor
{
    private SerializedProperty missionProp;
    private SerializedProperty actionsOnMissionStartedProp;
    private SerializedProperty actionsOnMissionCompletedProp;
    private SerializedProperty objectiveEventsProp;

    private void OnEnable()
    {
        missionProp = serializedObject.FindProperty("mission");
        actionsOnMissionStartedProp = serializedObject.FindProperty("actionsOnMissionStarted");
        actionsOnMissionCompletedProp = serializedObject.FindProperty("actionsOnMissionCompleted");
        objectiveEventsProp = serializedObject.FindProperty("objectiveEvents");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // Draw mission field
        EditorGUILayout.PropertyField(missionProp);
        
        EditorGUILayout.Space();
        var mission = missionProp.objectReferenceValue as SOMission;
        
        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(actionsOnMissionStartedProp, new GUIContent("On Mission Started"));
        EditorGUILayout.PropertyField(actionsOnMissionCompletedProp, new GUIContent("On Mission Completed"));
        
        if (mission)
        {
            var objectives = mission.CloneObjectives();
            
            // Resize array to match objectives
            if (objectiveEventsProp.arraySize != objectives.Length)
            {
                objectiveEventsProp.arraySize = objectives.Length;
            }
            
            for (int i = 0; i < objectives.Length; i++)
            {
                var objective = objectives[i];
                var entryProp = objectiveEventsProp.GetArrayElementAtIndex(i);
                var objectiveNameProp = entryProp.FindPropertyRelative("objectiveName");
                var actionsOnCompletedProp = entryProp.FindPropertyRelative("actionsOnCompleted");
                
                // Update the stored name for display
                objectiveNameProp.stringValue = $"{i}: {objective.Name}";
                
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.LabelField($"Objective {i}: {objective.Name}", EditorStyles.boldLabel);
                EditorGUILayout.LabelField(objective.Description, EditorStyles.miniLabel);
                EditorGUILayout.PropertyField(actionsOnCompletedProp, new GUIContent("Actions on Completed"));
                EditorGUILayout.EndVertical();
                EditorGUILayout.Space(5);
            }
        }
        else
        {
            EditorGUILayout.HelpBox("Assign a mission to see objective actions", MessageType.Info);
        }

        serializedObject.ApplyModifiedProperties();
    }
}
#endif
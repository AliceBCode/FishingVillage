#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MissionEvents))]
public class MissionEventsEditor : Editor
{
    private SerializedProperty missionProp;
    private SerializedProperty onMissionStartedProp;
    private SerializedProperty onMissionCompletedProp;
    private SerializedProperty objectiveEventsProp;

    private void OnEnable()
    {
        missionProp = serializedObject.FindProperty("mission");
        onMissionStartedProp = serializedObject.FindProperty("onMissionStarted");
        onMissionCompletedProp = serializedObject.FindProperty("onMissionCompleted");
        objectiveEventsProp = serializedObject.FindProperty("objectiveEvents");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        
        EditorGUILayout.PropertyField(missionProp);
        
        var mission = missionProp.objectReferenceValue as SOMission;
        
        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(onMissionStartedProp);
        EditorGUILayout.PropertyField(onMissionCompletedProp);
        
        EditorGUILayout.Space();
        
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
                var onCompletedProp = entryProp.FindPropertyRelative("onObjectiveCompleted");
                
                // Update the stored name for display
                objectiveNameProp.stringValue = $"{i}: {objective.Name}";
                
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.LabelField($"Objective {i}: {objective.Name}", EditorStyles.boldLabel);
                EditorGUILayout.LabelField(objective.Description, EditorStyles.miniLabel);
                EditorGUILayout.PropertyField(onCompletedProp, new GUIContent("On Completed"));
                EditorGUILayout.EndVertical();
                EditorGUILayout.Space(5);
            }
        }
        else
        {
            EditorGUILayout.HelpBox("Assign a mission to see objective events", MessageType.Info);
        }

        serializedObject.ApplyModifiedProperties();
    }
}
#endif
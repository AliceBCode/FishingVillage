#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MissionEventsListener))]
public class MissionEventsListenerEditor : Editor
{
    private SerializedProperty missionProp;
    private SerializedProperty onMissionStartedProp;
    private SerializedProperty onMissionCompletedProp;
    private SerializedProperty objectiveEventsProp;

    private void OnEnable()
    {
        missionProp = serializedObject.FindProperty("mission");
        onMissionStartedProp = serializedObject.FindProperty("onMissionStarted");
        objectiveEventsProp = serializedObject.FindProperty("objectiveEvents");
        onMissionCompletedProp = serializedObject.FindProperty("onMissionCompleted");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.Space(15);
        EditorGUILayout.PropertyField(missionProp, new GUIContent("Mission"));
        EditorGUILayout.Space(15);
        
        var mission = missionProp.objectReferenceValue as SOMission;
        
        
        if (mission)
        {
            EditorGUILayout.PropertyField(onMissionStartedProp, new GUIContent("On Mission Started"));
            
            var objectives = mission.CloneObjectives();
            
            if (objectiveEventsProp.arraySize != objectives.Length)
            {
                objectiveEventsProp.arraySize = objectives.Length;
            }
            
            for (int i = 0; i < objectives.Length; i++)
            {
                var objective = objectives[i];
                var entryProp = objectiveEventsProp.GetArrayElementAtIndex(i);
                var objectiveNameProp = entryProp.FindPropertyRelative("objectiveName");
                var onCompletedProp = entryProp.FindPropertyRelative("onCompleted");
                
                objectiveNameProp.stringValue = $"{i}: {objective.Name}";
                EditorGUILayout.PropertyField(onCompletedProp, new GUIContent($"Objective {i}: {objective.Description}"));
            }
            
            EditorGUILayout.PropertyField(onMissionCompletedProp, new GUIContent("On Mission Completed"));
        }
        else
        {
            EditorGUILayout.HelpBox("Assign a mission to see objective events", MessageType.Info);
        }
        

        serializedObject.ApplyModifiedProperties();
    }
}
#endif
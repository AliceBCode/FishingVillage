#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MissionEventsListener))]
public class MissionEventsListenerEditor : Editor
{
    private SerializedProperty _missionProp;
    private SerializedProperty _onMissionStartedProp;
    private SerializedProperty _onMissionCompletedProp;
    private SerializedProperty _objectiveEventsProp;

    private void OnEnable()
    {
        _missionProp = serializedObject.FindProperty("mission");
        _onMissionStartedProp = serializedObject.FindProperty("onMissionStarted");
        _objectiveEventsProp = serializedObject.FindProperty("objectiveEvents");
        _onMissionCompletedProp = serializedObject.FindProperty("onMissionCompleted");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.Space(15);
        EditorGUILayout.PropertyField(_missionProp, new GUIContent("Mission"));
        EditorGUILayout.Space(15);
        
        var mission = _missionProp.objectReferenceValue as SOMission;
        
        
        if (mission)
        {
            EditorGUILayout.PropertyField(_onMissionStartedProp, new GUIContent("On Mission Started"));
            
            var objectives = mission.CloneObjectives();
            
            if (_objectiveEventsProp.arraySize != objectives.Length)
            {
                _objectiveEventsProp.arraySize = objectives.Length;
            }
            
            for (int i = 0; i < objectives.Length; i++)
            {
                var objective = objectives[i];
                var entryProp = _objectiveEventsProp.GetArrayElementAtIndex(i);
                var onCompletedProp = entryProp.FindPropertyRelative("onCompleted");
                
                EditorGUILayout.PropertyField(onCompletedProp, new GUIContent($"On Objective {i}: {objective.GetDescription()}"));
            }
            
            EditorGUILayout.PropertyField(_onMissionCompletedProp, new GUIContent("On Mission Completed"));
        }
        else
        {
            EditorGUILayout.HelpBox("Assign a mission to see objective events", MessageType.Info);
        }
        

        serializedObject.ApplyModifiedProperties();
    }
}
#endif
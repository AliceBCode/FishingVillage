#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SOMission))]
public class SoMissionEditor : Editor
{
    private SerializedProperty _nameProp;
    private SerializedProperty _descriptionProp;
    private SerializedProperty _iconProp;
    private SerializedProperty _objectivesProp;
    private SerializedProperty _actionsOnMissionStartedProp;
    private SerializedProperty _actionsOnMissionCompletedProp;
    private SerializedProperty _objectiveEventsProp;

    private void OnEnable()
    {
        _nameProp = serializedObject.FindProperty("name");
        _descriptionProp = serializedObject.FindProperty("description");
        _iconProp = serializedObject.FindProperty("icon");
        _objectivesProp = serializedObject.FindProperty("objectives");
        _actionsOnMissionStartedProp = serializedObject.FindProperty("onStarted");
        _actionsOnMissionCompletedProp = serializedObject.FindProperty("onCompleted");
        _objectiveEventsProp = serializedObject.FindProperty("onObjectiveCompleted");
    }

public override void OnInspectorGUI()
{
    serializedObject.Update();
    
    EditorGUILayout.PropertyField(_nameProp, new GUIContent("Name"));
    EditorGUILayout.PropertyField(_descriptionProp, new GUIContent("Description"));
    EditorGUILayout.PropertyField(_iconProp, new GUIContent("Icon"));
    EditorGUILayout.PropertyField(_objectivesProp, new GUIContent("Objectives"));
    
    EditorGUILayout.PropertyField(_actionsOnMissionStartedProp, new GUIContent("On Mission Started"));
    
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
        if (_objectiveEventsProp.arraySize != validObjectives.Count)
        {
            _objectiveEventsProp.arraySize = validObjectives.Count;
        }
    
        if (validObjectives.Count > 0)
        {
            for (int i = 0; i < validObjectives.Count; i++)
            {
                var objective = validObjectives[i];
                int originalIndex = validIndices[i];
                
                var entryProp = _objectiveEventsProp.GetArrayElementAtIndex(i);
                var actionsOnCompletedProp = entryProp.FindPropertyRelative("onObjectiveCompleted");
                
                EditorGUILayout.Space(5);
                EditorGUILayout.PropertyField(actionsOnCompletedProp, new GUIContent($"On Objective {originalIndex}: {objective.GetDescription()}"));
                EditorGUILayout.Space(5);
            }
        }
        else
        {
            EditorGUILayout.HelpBox("Add objectives above to configure their completion actions", MessageType.Info);
        }
    }
    
    EditorGUILayout.PropertyField(_actionsOnMissionCompletedProp, new GUIContent("On Mission Completed"));

    serializedObject.ApplyModifiedProperties();
}
}
#endif
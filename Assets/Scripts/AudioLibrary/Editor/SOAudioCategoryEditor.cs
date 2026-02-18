using UnityEditor;
using UnityEngine;

namespace DNExtensions.Systems.AudioSystem
{
    [CustomEditor(typeof(SOAudioCategory))]
    public class SOAudioCategoryEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("label"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("channel"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("audioMixerGroup"));

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Audio Mappings", EditorStyles.boldLabel);

            SerializedProperty list = serializedObject.FindProperty("audioMappings");

            for (int i = 0; i < list.arraySize; i++)
            {
                SerializedProperty element = list.GetArrayElementAtIndex(i);
                SerializedProperty id = element.FindPropertyRelative("id");
                SerializedProperty obj = element.FindPropertyRelative("audioObject");

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PropertyField(id, GUIContent.none, GUILayout.Width(120));
                
                // Perform the strict type check
                EditorGUI.BeginChangeCheck();
                Object newObj = EditorGUILayout.ObjectField(GUIContent.none, obj.objectReferenceValue, typeof(Object), false);
                
                if (EditorGUI.EndChangeCheck())
                {
                    // Only allow assignment if it's one of our two valid types
                    if (!newObj || newObj is AudioClip || newObj is SOAudioProfile)
                    {
                        obj.objectReferenceValue = newObj;
                    }
                    else
                    {
                        Debug.LogWarning("SOAudioCategory: Only AudioClips or SOAudioProfiles are allowed!");
                    }
                }

                if (GUILayout.Button("X", GUILayout.Width(20))) { list.DeleteArrayElementAtIndex(i); }
                EditorGUILayout.EndHorizontal();
            }

            if (GUILayout.Button("Add New Resource")) { list.InsertArrayElementAtIndex(list.arraySize); }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
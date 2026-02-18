#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace DNExtensions.Systems.AudioSystem
{
    [CustomEditor(typeof(SOAudioLibrary))]
    public class SOAudioLibraryEditor : Editor
    {
        private readonly Dictionary<string, bool> _foldouts = new Dictionary<string, bool>();

        public override void OnInspectorGUI()
        {
            EditorGUILayout.Space(10);
            base.OnInspectorGUI();
            var audioLibrary = (SOAudioLibrary)target;

            if (audioLibrary.AudioCategories == null || audioLibrary.AudioCategories.Length == 0) return;

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Library Management", EditorStyles.boldLabel);

            foreach (var category in audioLibrary.AudioCategories)
            {
                if (!category) continue;
                
                _foldouts.TryAdd(category.name, true);
                SerializedObject serializedCategory = new SerializedObject(category);
                
                // --- CATEGORY HEADER (Stand-alone Toolbar) ---
                EditorGUILayout.BeginHorizontal();
                
                // Foldout Toggle
                _foldouts[category.name] = EditorGUILayout.Foldout(_foldouts[category.name], category.Label, true, EditorStyles.foldoutHeader);
                
                GUILayout.FlexibleSpace();

                // Grouped Toolbar Settings
                DrawCategoryToolbar(serializedCategory);

                // Add Button
                if (GUILayout.Button("+ Sound", EditorStyles.miniButtonRight, GUILayout.Width(70)))
                {
                    AddNewResourceToCategory(category);
                    _foldouts[category.name] = true; 
                }
                
                EditorGUILayout.EndHorizontal();

                // --- CATEGORY CONTENT (Boxed Area) ---
                if (_foldouts[category.name])
                {
                    // Start the box ONLY inside the foldout
                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    EditorGUILayout.Space(2);
                    
                    EditorGUI.indentLevel++; 
                    DrawMappingList(serializedCategory);
                    EditorGUI.indentLevel--;
                    
                    EditorGUILayout.Space(2);
                    serializedCategory.ApplyModifiedProperties();
                    EditorGUILayout.EndVertical(); // Close the box here
                }
                
                EditorGUILayout.Space(5);
            }
        }

        private void DrawCategoryToolbar(SerializedObject serializedCategory)
        {
            SerializedProperty resMixer = serializedCategory.FindProperty("audioMixerGroup");
            if (resMixer != null) EditorGUILayout.PropertyField(resMixer, GUIContent.none, GUILayout.Width(100));
            
            SerializedProperty resChannel = serializedCategory.FindProperty("channel");
            if (resChannel != null) EditorGUILayout.PropertyField(resChannel, GUIContent.none, GUILayout.Width(70));
        }

        private void DrawMappingList(SerializedObject serializedCategory)
        {
            SerializedProperty resourcesProp = serializedCategory.FindProperty("audioMappings");

            if (resourcesProp == null || resourcesProp.arraySize == 0)
            {
                EditorGUILayout.LabelField("No sounds in this category.", EditorStyles.miniLabel);
                return;
            }
            
            for (int i = 0; i < resourcesProp.arraySize; i++)
            {
                SerializedProperty mapping = resourcesProp.GetArrayElementAtIndex(i);
                SerializedProperty idProp = mapping.FindPropertyRelative("id");
                SerializedProperty objProp = mapping.FindPropertyRelative("audioObject");

                EditorGUILayout.BeginHorizontal();
                
                EditorGUILayout.PropertyField(idProp, GUIContent.none, GUILayout.MinWidth(80));
                
                EditorGUI.BeginChangeCheck();
                Object newObj = EditorGUILayout.ObjectField(GUIContent.none, objProp.objectReferenceValue, typeof(Object), false);
                
                if (EditorGUI.EndChangeCheck())
                {
                    if (!newObj || newObj is AudioClip || newObj is SOAudioProfile)
                    {
                        objProp.objectReferenceValue = newObj;
                    }
                    else
                    {
                        Debug.LogWarning("SOAudioLibrary: Asset must be an AudioClip or SOAudioProfile!");
                    }
                }

                if (GUILayout.Button("X", EditorStyles.miniButton, GUILayout.Width(20)))
                {
                    resourcesProp.DeleteArrayElementAtIndex(i);
                }
                
                EditorGUILayout.EndHorizontal();
            }
        }

        private void AddNewResourceToCategory(SOAudioCategory category)
        {
            SerializedObject so = new SerializedObject(category);
            SerializedProperty prop = so.FindProperty("audioMappings");
            
            prop.InsertArrayElementAtIndex(prop.arraySize);
            
            SerializedProperty newElem = prop.GetArrayElementAtIndex(prop.arraySize - 1);
            newElem.FindPropertyRelative("id").stringValue = "New_ID";
            newElem.FindPropertyRelative("audioObject").objectReferenceValue = null;

            so.ApplyModifiedProperties();
        }
    }
}
#endif
#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;

namespace FishingVillage.AudioLibrary
{
    [CustomEditor(typeof(SOAudioLibrary))]
    public class SOAudioLibraryEditor : Editor
    {
        private readonly Dictionary<string, bool> _foldouts = new Dictionary<string, bool>();

        public override void OnInspectorGUI()
        {
            EditorGUILayout.Space(20);
            base.OnInspectorGUI();
            var audioLibrary = (SOAudioLibrary)target;

            if (audioLibrary.AudioCategories == null || audioLibrary.AudioCategories.Length == 0) return;

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Library", EditorStyles.boldLabel);

            foreach (var category in audioLibrary.AudioCategories)
            {
                if (!category) continue;
                
                _foldouts.TryAdd(category.name, true);
                
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                
                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(15);
                _foldouts[category.name] = EditorGUILayout.Foldout(_foldouts[category.name], category.Label, true);
                

                if (GUILayout.Button("+ Add New Sound", GUILayout.Width(120)))
                {
                    AddNewResourceToCategory(category);
                }
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.Space();

                if (_foldouts[category.name])
                {
                    SerializedObject serializedCategory = new SerializedObject(category);
                    SerializedProperty resourcesProp = serializedCategory.FindProperty("audioResources");

                    if (resourcesProp.arraySize == 0)
                    {
                        EditorGUILayout.LabelField("No audio resources in this category.");
                        GUILayout.Space(3);
                    }
                    
                    for (int i = 0; i < resourcesProp.arraySize; i++)
                    {
                        SerializedProperty mapping = resourcesProp.GetArrayElementAtIndex(i);
                        SerializedProperty idProp = mapping.FindPropertyRelative("id");
                        SerializedProperty resProp = mapping.FindPropertyRelative("audioResource");

                        EditorGUILayout.BeginHorizontal();
                        GUILayout.Space(3);
                        EditorGUILayout.PropertyField(idProp, GUIContent.none, GUILayout.Width(150));
                        EditorGUILayout.PropertyField(resProp, GUIContent.none);
                        
                        if (GUILayout.Button("X", GUILayout.Width(25)))
                        {
                            resourcesProp.DeleteArrayElementAtIndex(i);
                        }
                        
                        GUILayout.Space(3);
                        EditorGUILayout.EndHorizontal();
                        GUILayout.Space(3);
                    }
                    
                    serializedCategory.ApplyModifiedProperties();
                }
                
                EditorGUILayout.EndVertical();
                EditorGUILayout.Space();
            }
        }

        private void AddNewResourceToCategory(SOAudioCategory category)
        {
            SerializedObject so = new SerializedObject(category);
            SerializedProperty prop = so.FindProperty("audioResources");
            
            // Add a new element to the end of the array
            prop.InsertArrayElementAtIndex(prop.arraySize);
            
            // Clean the new element so it's not a duplicate of the last one
            SerializedProperty newElem = prop.GetArrayElementAtIndex(prop.arraySize - 1);
            newElem.FindPropertyRelative("id").stringValue = "New_ID";
            newElem.FindPropertyRelative("audioResource").objectReferenceValue = null;

            so.ApplyModifiedProperties(); // Saves the change to the asset
        }
    }
}
#endif
using System;
using DNExtensions.Utilities;
using DNExtensions.Utilities.Button;
using UnityEngine;

namespace FishingVillage.Interactable
{
    public interface IIdentifiable
    {
        string ID { get; }
    }

    public class IdentifiableInteractable : MonoBehaviour, IIdentifiable
    {
        [SerializeField, ReadOnly] private string id = "";
        
        public string ID => id;

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (string.IsNullOrEmpty(id))
            {
                id = Guid.NewGuid().ToString();
                UnityEditor.EditorUtility.SetDirty(this);
            }
        }
        
        [Button(ButtonPlayMode.OnlyWhenNotPlaying)]
        private void RegenerateID()
        {
            id = Guid.NewGuid().ToString();
            UnityEditor.EditorUtility.SetDirty(this);
        }
#endif
    }
}
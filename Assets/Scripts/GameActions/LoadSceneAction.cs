using System;
using DNExtensions.Utilities.CustomFields;
using DNExtensions.Utilities.SerializableSelector;
using UnityEngine;

namespace FishingVillage.GameActions
{
    [Serializable]
    [SerializableSelectorName("Load", "Scene")]
    public class LoadSceneAction : GameAction
    {
        [SerializeField] private SceneField scene;
        
        public override string ActionName => $"Load Scene {scene.SceneName}";

        public override void Execute()
        {
            if (!scene.IsSceneValid()) return;
            scene.LoadScene();
        }
    }
}
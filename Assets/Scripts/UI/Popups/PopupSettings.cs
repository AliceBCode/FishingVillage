using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PrimeTween;

namespace FishingVillage.UI.Popup
{
    
    [Serializable]
    public class PopupSettings
    {
        [SerializeField] private Color backgroundColor = Color.white;
        [SerializeField] private Sprite icon;

        public Color BackgroundColor => backgroundColor;
        public Sprite Icon => icon;
    }
    
}
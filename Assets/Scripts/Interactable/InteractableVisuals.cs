using DNExtensions.Utilities;
using FishingVillage.UI;
using UnityEngine;

namespace FishingVillage.Interactable
{
    [DisallowMultipleComponent]
    public class InteractableVisuals : MonoBehaviour
    {
        [SerializeField] private bool showPrompt = true;
        [SerializeField, EnableIf("showPrompt")] private Vector3 promptOffset = Vector3.up;
        [SerializeField] private bool useOutline = true;
        
        private Outline _outline;

        private void Awake()
        {
            if (useOutline && TryGetComponent(out _outline))
            {
                _outline.enabled = false;
                _outline.OutlineMode = Outline.Mode.OutlineVisible;
                _outline.OutlineColor = Color.cornflowerBlue;
                _outline.OutlineWidth = 3f;
            }
        }
        
        public void Hide()
        {
            if (showPrompt) InteractPrompt.Instance?.Hide(true);
            if (_outline) _outline.enabled = false;
        }

        public void Show()
        {
            if (showPrompt) InteractPrompt.Instance?.Show(transform.position + promptOffset);
            if (_outline) _outline.enabled = true;
        }
        
        public void Show(Vector3 position)
        {
            if (showPrompt) InteractPrompt.Instance?.Show(position + promptOffset);
            if (_outline) _outline.enabled = true;
        }
        
        public void UpdatePromptPosition(Vector3 position)
        {
            if (showPrompt) InteractPrompt.Instance?.UpdatePosition(position + promptOffset);
        }

        private void OnDrawGizmosSelected()
        {
            if (showPrompt) Gizmos.DrawWireSphere(transform.position + promptOffset, 0.25f);
        }
    }
}
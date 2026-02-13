using System;
using System.Collections.Generic;
using DNExtensions.Utilities.Button;
using DNExtensions.Utilities.Inline;
using FishingVillage.Dialogue;
using FishingVillage.Missions;
using FishingVillage.Player;
using FishingVillage.UI;
using UnityEngine;

namespace FishingVillage.Interactable
{
    [SelectionBase]
    [DisallowMultipleComponent]
    public class NPC : MonoBehaviour, IInteractable
    {
        [Header("Settings")]
        [SerializeField] private new string name = "NPC";
        [SerializeField] private NpcInteraction[] consumableInteractions = Array.Empty<NpcInteraction>();
        
        [Header("Proximity Dialogue")]
        [SerializeField] private bool playProximityDialogue = true;
        [SerializeField] private float proximityCooldown = 1.5f;
        [SerializeField, Inline] private SODialogueLines greetingDialogueLines;
        [SerializeField, Inline] private SODialogueLines farewellDialogueLines;
        
        private float _proximityCooldownTimer;
        private InteractableVisuals _visuals;
        private SpeechBubble _speechBubble;
        private DialogueSequence _activeDialogue;
        
        public string Name => name;

        private void Awake()
        {
            
            _visuals = GetComponent<InteractableVisuals>();
            _speechBubble = GetComponentInChildren<SpeechBubble>();
        }

        private void Update()
        {
            if (_proximityCooldownTimer > 0f)
            {
                _proximityCooldownTimer -= Time.deltaTime;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (_activeDialogue != null || !playProximityDialogue || _proximityCooldownTimer > 0) return;
            
            if (other.TryGetComponent(out PlayerController player))
            {
                ShowGreetBubble();
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (_activeDialogue != null ||!playProximityDialogue || _proximityCooldownTimer > 0) return;
            
            if (other.TryGetComponent(out PlayerController player))
            {
                ShowFarewellBubble();
            }
        }
        
        private void ReceiveItem(SOItem item)
        {
            if (PlayerInventory.Instance && PlayerInventory.Instance.TryRemoveItem(item))
            {
                GameEvents.ItemGivenToNpc(item, this);
            }
        }
        
        public bool CanInteract()
        {
            if (_activeDialogue != null)
            {
                return true;
            }
            
            if (MissionManager.Instance && MissionManager.Instance.HasMissionGiveItemFor(this, out var item))
            {
                if (PlayerInventory.Instance && PlayerInventory.Instance.HasItem(item))
                {
                    return true;
                }
            }

            if (HasConsumableInteraction())
            {
                return true;
            }

            return false;
        }

        public void Interact()
        {
            if (!CanInteract())
            {
                return;
            }
            
            
            GameEvents.InteractedWith(this);
            
            if (_activeDialogue != null)
            {
                if (_activeDialogue.AdvanceMode == DialogueAdvanceMode.Manual)
                {
                    ShowNextLine();
                }
                return;
            }
            
            
            foreach (var interaction in consumableInteractions)
            {
                if (interaction == null || interaction.IsConsumed) continue;
                interaction.Execute();
                return;
            }

            
            if (MissionManager.Instance && MissionManager.Instance.HasMissionGiveItemFor(this, out SOItem item))
            {
                ReceiveItem(item);
            }
        }

        public void ShowInteract()
        {
            if (_activeDialogue != null) return;
            if (!CanInteract()) return;
            
            _visuals.Show();
        }

        public void HideInteract()
        {
            _visuals.Hide();
        }

        #region Sequence Dialogue

        private void ShowNextLine()
        {
            if (_activeDialogue.IsComplete)
            {
                GameEvents.DialogueSequenceCompleted(this);
                _activeDialogue = null;
                _speechBubble.Hide(true);
                return;
            }
    
            string line = _activeDialogue.GetCurrentLine();
            _activeDialogue.Advance();
    
            _speechBubble?.Show(line, _activeDialogue.AdvanceMode == DialogueAdvanceMode.Manual);
            InteractPrompt.Instance?.Hide(false);
    
            if (_activeDialogue.AdvanceMode == DialogueAdvanceMode.Automatic)
            {
                Invoke(nameof(ShowNextLine), _activeDialogue.AutoAdvanceDelay);
            }
        }
        
        public void StartDialogueSequence(SODialogueSequence sequence)
        {
            if (!sequence) return;
            
            _activeDialogue = new DialogueSequence(sequence);
            _speechBubble?.Hide(false);
            ShowNextLine();
        }

        #endregion

        #region Proximity Dialogue

        [Button]
        private void ShowGreetBubble()
        {
            if (!greetingDialogueLines) return;
            
            _proximityCooldownTimer = proximityCooldown;
            _speechBubble?.Show(greetingDialogueLines.GetRandomLine,false, 3.5f);
        }
        
        [Button]
        private void ShowFarewellBubble()
        {
            if (!farewellDialogueLines) return;
            
            _proximityCooldownTimer = proximityCooldown;
            _speechBubble?.Show(farewellDialogueLines.GetRandomLine,false, 3.5f);
        }
        
        public void EnableProximityDialogue(bool enable)
        {
            playProximityDialogue = enable;
        }

        public void SetFarewellLines(SODialogueLines newLines)
        {
            if (!newLines) return;
            farewellDialogueLines = newLines;
        }

        public void SetGreetingLines(SODialogueLines newLines)
        {
            if (!newLines) return;
            greetingDialogueLines = newLines;
        }

        #endregion

        #region Consumable Interactions

        public void AddConsumableInteraction(NpcInteraction interaction)
        {
            if (interaction == null) return;
            
            var list = new List<NpcInteraction>(consumableInteractions);
            if (!list.Contains(interaction))
            {
                list.Add(interaction);
                consumableInteractions = list.ToArray();
            }
        }

        public void RemoveConsumableInteraction(NpcInteraction interaction)
        {
            if (interaction == null) return;
            
            var list = new List<NpcInteraction>(consumableInteractions);
            list.Remove(interaction);
            consumableInteractions = list.ToArray();
        }

        public void ResetConsumableInteraction(int index)
        {
            if (index >= 0 && index < consumableInteractions.Length)
            {
                consumableInteractions[index]?.Reset();
            }
        }
        
        private bool HasConsumableInteraction()
        {
            foreach (var interaction in consumableInteractions)
            {
                if (interaction is { IsConsumed: false })
                {
                    return true;
                }
            }
            
            return false;
        }

        #endregion
    }
}
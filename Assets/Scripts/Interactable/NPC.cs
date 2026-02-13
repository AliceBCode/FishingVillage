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
        
        private float _speechCooldownTimer;
        private SpeechBubble _speechBubble;
        private DialogueSequence _activeDialogue;
        
        public string Name => name;

        private void Awake()
        {
            _speechBubble = GetComponentInChildren<SpeechBubble>();
        }

        private void Update()
        {
            if (_speechCooldownTimer > 0f)
            {
                _speechCooldownTimer -= Time.deltaTime;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!playProximityDialogue || !greetingDialogueLines || _speechCooldownTimer > 0) 
                return;
            
            if (other.TryGetComponent(out PlayerController player))
            {
                ShowGreetBubble();
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (!playProximityDialogue || !farewellDialogueLines || _speechCooldownTimer > 0) 
                return;
            
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
            return _activeDialogue != null || MissionManager.Instance.HasMissionGiveItemFor(this, out var item) || HasConsumableInteraction();
        }

        public void Interact()
        {
            if (!CanInteract())
            {
                return;
            }
            
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
                interaction?.Execute();
            }
            
            
            GameEvents.NpcTalkedTo(this);

            if (MissionManager.Instance.HasMissionGiveItemFor(this, out SOItem item))
            {
                ReceiveItem(item);
            }
        }

        public void ShowInteract()
        {
            if (_activeDialogue != null) return;
            if (!CanInteract()) return;
            
            InteractPrompt.Instance?.Show(transform.position + Vector3.up);
        }

        public void HideInteract()
        {
            InteractPrompt.Instance?.Hide(true);
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
            
            string line = _activeDialogue.GetNextLine();
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
            
            playProximityDialogue = false;
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
            
            _speechCooldownTimer = proximityCooldown;
            _speechBubble?.Show(greetingDialogueLines.GetRandomLine,false, 3.5f);
        }
        
        [Button]
        private void ShowFarewellBubble()
        {
            if (!farewellDialogueLines) return;
            
            _speechCooldownTimer = proximityCooldown;
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
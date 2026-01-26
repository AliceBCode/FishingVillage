using System;
using DNExtensions;
using DNExtensions.Button;
using UI;
using UnityEngine;

public class NPC : Interactable
{
    [Header("NPC Settings")]
    [SerializeField] private new string name = "NPC";
    
    [Header("Proximity Dialogue")]
    [SerializeField] private bool proximityDialogue = true;
    [SerializeField] private SODialogueLines greetingDialogueLines;
    [SerializeField] private SODialogueLines farewellDialogueLines;
    [SerializeField] private float lineCooldown = 1.5f;
    [SerializeField, ReadOnly] private float lineCooldownTimer;
    

    private SpeechBubble _speechBubble;
    

    private void Awake()
    {
        _speechBubble = GetComponentInChildren<SpeechBubble>();
    }

    private void Update()
    {
        if (lineCooldownTimer > 0f)
        {
            lineCooldownTimer -= Time.deltaTime;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!proximityDialogue || !greetingDialogueLines || lineCooldownTimer > 0) return;
    
        if (other.TryGetComponent(out PlayerController player))
        {
            ShowGreetBubble();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!proximityDialogue || !farewellDialogueLines || lineCooldownTimer > 0) return;
    
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
    
    protected override void OnInteract()
    {
        GameEvents.NpcTalkedTo(this);

        if (MissionManager.Instance.HasMissionGiveItemFor(this, out SOItem item))
        {
            ReceiveItem(item);
        }
    }

    
    
    

    #region Proximity dialogue 

    public void ToggleProximityDialogue(bool toggle)
    {
        proximityDialogue = toggle;
    }

    public void SetFarewellLines(SODialogueLines newLines)
    {
        if (!newLines) return;
        
        farewellDialogueLines = newLines;
    }

    public void SetGreetingDialogueLines(SODialogueLines newLines)
    {
        if (!newLines) return;
        
        greetingDialogueLines = newLines;
    }
    
    
    [Button]
    private void ShowGreetBubble()
    {
        lineCooldownTimer = lineCooldown;
        
        _speechBubble?.Show(greetingDialogueLines?.GetRandomLine);
    }
    
    [Button]
    private void ShowFarewellBubble()
    {
        lineCooldownTimer = lineCooldown;
        
        _speechBubble?.Show(farewellDialogueLines?.GetRandomLine);
    }

    #endregion  Proximity Dialouge





    

    

}
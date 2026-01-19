using System;
using DNExtensions;
using DNExtensions.Button;
using UnityEngine;

public class NPC : Interactable
{
    [Header("NPC Settings")]
    [SerializeField] private new string name = "NPC";
    [SerializeField] private float lineCooldown = 2f;
    [SerializeField] private SODialogueLines greetingDialogueLines;
    [SerializeField] private SODialogueLines farewellDialogueLines;
    [SerializeField, ReadOnly] private float lineCooldownTimer;

    public string Name => name;

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
        if (!greetingDialogueLines || lineCooldownTimer > 0) return;
        
        if (other.TryGetComponent(out PlayerController player))
        {
            ShowGreetBubble();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!farewellDialogueLines || lineCooldownTimer > 0) return;
        
        if (other.TryGetComponent(out PlayerController player))
        {
            ShowFarewellBubble();
        }
    }
    
    private void ReceiveItem(SOItem item)
    {
        if (PlayerController.Instance && PlayerController.Instance.Inventory.TryRemoveItem(item))
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

    [Button]
    public void ShowGreetBubble()
    {
        lineCooldownTimer = lineCooldown;
        
        _speechBubble?.Show(greetingDialogueLines?.GetRandomLine);
    }
    
    [Button]
    public void ShowFarewellBubble()
    {
        lineCooldownTimer = lineCooldown;
        
        _speechBubble?.Show(farewellDialogueLines?.GetRandomLine);
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
    

    

}
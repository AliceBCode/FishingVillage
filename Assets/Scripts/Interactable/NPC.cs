using System;
using DNExtensions;
using DNExtensions.Button;
using UnityEngine;

public class NPC : Interactable
{
    [Header("NPC Settings")]
    [SerializeField] private new string name = "NPC";
    [SerializeField] private float lineCooldown = 2f;
    [SerializeField] private ChanceList<string> greetingDialogueLines;
    [SerializeField] private ChanceList<string> farewellDialogueLines;
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
        if (greetingDialogueLines.Count <= 0 || lineCooldownTimer > 0) return;
        
        if (other.TryGetComponent(out PlayerController player))
        {
            ShowGreetBubble();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (farewellDialogueLines.Count <= 0 || lineCooldownTimer > 0) return;
        
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

    [Button]
    public void ShowGreetBubble()
    {
        var line = greetingDialogueLines.GetRandomItem();
        var lineIndex = greetingDialogueLines.IndexOf(line);
        greetingDialogueLines.SetChance(lineIndex, 0);
        lineCooldownTimer = lineCooldown;
        
        _speechBubble?.Show(line);
    }
    
    [Button]
    public void ShowFarewellBubble()
    {
        var line = farewellDialogueLines.GetRandomItem();
        var lineIndex = farewellDialogueLines.IndexOf(line);
        farewellDialogueLines.SetChance(lineIndex, 0);
        lineCooldownTimer = lineCooldown;
        
        _speechBubble?.Show(line);
    }
    

    
    protected override void OnInteract()
    {
        GameEvents.NpcTalkedTo(this);
        
        // TODO: Check the held item of the player, if the npc needs it call receive item
    }
    

}
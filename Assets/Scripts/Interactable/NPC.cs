
using DNExtensions.Utilities;
using DNExtensions.Utilities.Button;
using UI;
using UnityEngine;

public class NPC : Interactable
{
    [Header("NPC Settings")]
    [SerializeField] private new string name = "NPC";
    
    [Header("Speech Bubble")]
    [SerializeField] private float speechCooldown = 1.5f;
    [SerializeField, ReadOnly] private float speechCooldownTimer;
    
    [Header("Proximity Dialogue")]
    [SerializeField] private bool playProximityDialogue = true;
    [SerializeField] private SODialogueLines greetingDialogueLines;
    [SerializeField] private SODialogueLines farewellDialogueLines;

    private SpeechBubble _speechBubble;
    private DialogueSequence _activeDialogue;
    
    public string Name => name;

    private void Awake()
    {
        _speechBubble = GetComponentInChildren<SpeechBubble>();
    }

    private void Update()
    {
        if (speechCooldownTimer > 0f)
        {
            speechCooldownTimer -= Time.deltaTime;
        }
    }
    

    private void OnTriggerEnter(Collider other)
    {
        if (!playProximityDialogue || !greetingDialogueLines || speechCooldownTimer > 0) 
            return;
        
        if (other.TryGetComponent(out PlayerController player))
        {
            ShowGreetBubble();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!playProximityDialogue || !farewellDialogueLines || speechCooldownTimer > 0) 
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
    
    protected override void OnInteract()
    {
        if (_activeDialogue != null)
        {
            if (_activeDialogue.AdvanceMode == DialogueAdvanceMode.Manual)
            {
                ShowNextLine();
            }
            return;
        }
        
        GameEvents.NpcTalkedTo(this);

        if (MissionManager.Instance.HasMissionGiveItemFor(this, out SOItem item))
        {
            ReceiveItem(item);
        }
    }

    public void GiveItemToPlayer(SOItem item)
    {
        if (PlayerInventory.Instance)
        {
            PlayerInventory.Instance.TryAddItem(item);
        }
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
        _speechBubble?.Show(line);
        speechCooldownTimer = speechCooldown;
        
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
        _speechBubble.Hide(false);
        ShowNextLine();
    }

    #endregion

    
    #region Proximity Dialogue

    [Button]
    private void ShowGreetBubble()
    {
        if (!greetingDialogueLines) return;
        
        speechCooldownTimer = speechCooldown;
        _speechBubble?.Show(greetingDialogueLines.GetRandomLine, 3.5f);
    }
    
    [Button]
    private void ShowFarewellBubble()
    {
        if (!farewellDialogueLines) return;
        
        speechCooldownTimer = speechCooldown;
        _speechBubble?.Show(farewellDialogueLines.GetRandomLine, 3.5f);
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

    public void SetGreetingDialogueLines(SODialogueLines newLines)
    {
        if (!newLines) return;
        greetingDialogueLines = newLines;
    }

    #endregion


}
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [SerializeField] private PlayerController player;
    [SerializeField] private TextMeshProUGUI inventoryText;
    [SerializeField] private TextMeshProUGUI activeMissionsText;
    [SerializeField] private TextMeshProUGUI completedMissionsText;
    
    private readonly List<SOMission> activeMissions = new List<SOMission>();
    private readonly List<SOMission> completedMissions = new List<SOMission>();
    private readonly Dictionary<SOMission, MissionObjective[]> missionConditions = new Dictionary<SOMission, MissionObjective[]>();
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        if (player)
        {
            player.Inventory.OnInventoryChanged += UpdateInventoryUI;
            UpdateInventoryUI(player.Inventory);
        }
        
        GameEvents.OnMissionStarted += OnMissionStarted;
        GameEvents.OnMissionCompleted += OnMissionCompleted;
        MissionObjective.OnObjectiveMet += UpdateActiveMissionsUI;
    }

    private void OnDestroy()
    {
        if (player)
        {
            player.Inventory.OnInventoryChanged -= UpdateInventoryUI;
        }
        
        GameEvents.OnMissionStarted -= OnMissionStarted;
        GameEvents.OnMissionCompleted -= OnMissionCompleted;
        MissionObjective.OnObjectiveMet -= UpdateActiveMissionsUI;
    }

    private void OnMissionStarted(SOMission mission)
    {
        activeMissions.Add(mission);
        
        if (MissionManager.Instance)
        {
            var conditions = MissionManager.Instance.GetMissionObjectives(mission, true);
            if (conditions != null)
            {
                missionConditions[mission] = conditions;
            }
        }
        
        UpdateActiveMissionsUI();
    }

    private void OnMissionCompleted(SOMission mission)
    {
        activeMissions.Remove(mission);
        completedMissions.Add(mission);
        missionConditions.Remove(mission);
        
        UpdateActiveMissionsUI();
        UpdateCompletedMissionsUI();
    }

    private void UpdateInventoryUI(Inventory inventory)
    {
        if (!inventoryText) return;
        
        inventoryText.text = "Inventory:\n";
        foreach (var item in inventory.Items)
        {
            inventoryText.text += item.Name + "\n";
        }
    }

    private void UpdateActiveMissionsUI()
    {
        if (!activeMissionsText) return;
        
        activeMissionsText.text = "Active Missions:\n\n";
        
        foreach (var mission in activeMissions)
        {
            activeMissionsText.text += $"{mission.Name}:"  + "\n";
            
            if (missionConditions.TryGetValue(mission, out var conditions))
            {
                foreach (var condition in conditions)
                {
                    string checkmark = condition.Met ? "[X]" : "[ ]";
                    activeMissionsText.text += $"  {checkmark} {condition.Description}\n";
                }
            }
            
            activeMissionsText.text += "\n";
        }
    }

    private void UpdateCompletedMissionsUI()
    {
        if (!completedMissionsText) return;
        
        completedMissionsText.text = "Completed Missions:\n\n";
        
        foreach (var mission in completedMissions)
        {
            completedMissionsText.text += mission.Name + "\n";
        }
    }
}
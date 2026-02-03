using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class MissionsPanel : MonoBehaviour
{
    public static MissionsPanel Instance;
    
    [SerializeField] private TextMeshProUGUI activeMissionsText;
    [SerializeField] private TextMeshProUGUI completedMissionsText;
    
    private readonly List<SOMission> _activeMissions = new List<SOMission>();
    private readonly List<SOMission> _completedMissions = new List<SOMission>();
    
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
        GameEvents.OnMissionStarted += OnMissionStarted;
        GameEvents.OnMissionCompleted += OnMissionCompleted;
        MissionObjective.OnObjectiveMet += OnObjectiveCompleted;
    }

    private void OnDestroy()
    {
        GameEvents.OnMissionStarted -= OnMissionStarted;
        GameEvents.OnMissionCompleted -= OnMissionCompleted;
        MissionObjective.OnObjectiveMet -= OnObjectiveCompleted;
    }

    private void OnMissionStarted(SOMission mission)
    {
        _activeMissions.Add(mission);
        UpdateActiveMissionsUI();
    }

    private void OnObjectiveCompleted(MissionObjective objective)
    {
        UpdateActiveMissionsUI();
    }

    private void OnMissionCompleted(SOMission mission)
    {
        _activeMissions.Remove(mission);
        _completedMissions.Add(mission);
        
        UpdateActiveMissionsUI();
        UpdateCompletedMissionsUI();
    }

    private void UpdateActiveMissionsUI()
    {
        if (!activeMissionsText) return;
    
        activeMissionsText.text = "Active Missions:\n\n";
    
        foreach (var mission in _activeMissions)
        {
            activeMissionsText.text += $"{mission.Name}:"  + "\n";
            
            if (MissionManager.Instance)
            {
                var objectives = MissionManager.Instance.GetMissionObjectives(mission, true);
                
                if (objectives != null && objectives.Length > 0)
                {
                    foreach (var objective in objectives)
                    {
                        string checkmark = objective.Met ? "[X]" : "[ ]";
                        activeMissionsText.text += $"  {checkmark} {objective.Description}\n";
                    }
                }
                else
                {
                    activeMissionsText.text += "  (No visible objectives)\n";
                }
            }
        
            activeMissionsText.text += "\n";
        }
    }

    private void UpdateCompletedMissionsUI()
    {
        if (!completedMissionsText) return;
        
        completedMissionsText.text = "Completed Missions:\n\n";
        
        foreach (var mission in _completedMissions)
        {
            completedMissionsText.text += mission.Name + "\n";
        }
    }
}
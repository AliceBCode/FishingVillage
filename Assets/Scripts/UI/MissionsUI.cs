using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class MissionsUI : MonoBehaviour
{
    public static MissionsUI Instance;
    
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
        
        GameEvents.OnMissionStarted += OnMissionStarted;
        GameEvents.OnMissionCompleted += OnMissionCompleted;
        MissionObjective.OnObjectiveMet += UpdateActiveMissionsUI;
    }

    private void OnDestroy()
    {
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
    
    

    private void UpdateActiveMissionsUI()
    {
        if (!activeMissionsText) return;
    
        activeMissionsText.text = "Active Missions:\n\n";
    
        foreach (var mission in activeMissions)
        {
            activeMissionsText.text += $"{mission.Name}:"  + "\n";
            
            if (MissionManager.Instance)
            {
                var conditions = MissionManager.Instance.GetMissionObjectives(mission, true);
                if (conditions != null)
                {
                    missionConditions[mission] = conditions;
                
                    foreach (var condition in conditions)
                    {
                        string checkmark = condition.Met ? "[X]" : "[ ]";
                        activeMissionsText.text += $"  {checkmark} {condition.Description}\n";
                    }
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
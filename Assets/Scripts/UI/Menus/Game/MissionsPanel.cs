using System.Collections.Generic;
using FishingVillage.Gameplay;
using FishingVillage.Missions;
using FishingVillage.Missions.Objectives;
using TMPro;
using UnityEngine;

namespace FishingVillage.UI.Menus
{
    

    public class MissionsPanel : MonoBehaviour
    {
        public static MissionsPanel Instance;

        
        [Header("Settings")]
        [SerializeField] private bool showNonActiveObjectives;
        
        
        [Header("UI References")]
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

            GameEvents.OnMissionStarted += OnMissionStarted;
            GameEvents.OnMissionCompleted += OnMissionCompleted;
            GameEvents.OnObjectiveMet += OnObjectiveUpdated;
            GameEvents.OnObjectiveProgressed += OnObjectiveUpdated;
        }


        private void OnDestroy()
        {
            GameEvents.OnMissionStarted -= OnMissionStarted;
            GameEvents.OnMissionCompleted -= OnMissionCompleted;
            GameEvents.OnObjectiveMet -= OnObjectiveUpdated;
            GameEvents.OnObjectiveProgressed -= OnObjectiveUpdated;
        }

        private void OnMissionStarted(SOMission mission)
        {
            _activeMissions.Add(mission);
            UpdateActiveMissionsUI();
        }
        

        private void OnMissionCompleted(SOMission mission)
        {
            _activeMissions.Remove(mission);
            _completedMissions.Add(mission);

            UpdateActiveMissionsUI();
            UpdateCompletedMissionsUI();
        }
        
        private void OnObjectiveUpdated(MissionObjective objective)
        {
            UpdateActiveMissionsUI();
        }

        private void UpdateActiveMissionsUI()
        {
            if (!activeMissionsText) return;

            activeMissionsText.text = "";

            foreach (var mission in _activeMissions)
            {
                activeMissionsText.text += $"{mission.Name}:" + "\n";
                    
                if (MissionManager.Instance)
                {
                    var objectives = MissionManager.Instance.GetMissionObjectives(mission);

                    if (objectives is { Length: > 0 })
                    {
                        foreach (var objective in objectives)
                        {
                            if (objective.IsHidden || (!objective.IsActive && !showNonActiveObjectives)) continue;
                            
                            string display = objective.Met ? $"○ <s>{objective.GetDescription()}</s>" : $"○ {objective.GetDescription()}";

                            activeMissionsText.text += display + "\n";
                        }
                    }
                    else
                    {
                        activeMissionsText.text += "???\n";
                    }
                }

                activeMissionsText.text += "\n";
            }
        }

        private void UpdateCompletedMissionsUI()
        {
            if (!completedMissionsText) return;

            completedMissionsText.text = "Completed:\n";

            foreach (var mission in _completedMissions)
            {
                completedMissionsText.text += $"○ <s>{mission.Name}</s>" + "\n";
            }
        }
    }
}
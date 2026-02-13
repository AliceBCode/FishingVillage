using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DNExtensions.Utilities;
using FishingVillage.Interactable;
using FishingVillage.Missions.Objectives;
using UnityEngine;

namespace FishingVillage.Missions
{
    public class MissionManager : MonoBehaviour
    {
        public static MissionManager Instance;
        
        [SerializeField, ReadOnly] private List<SOMission> activeMissions;
        [SerializeField, ReadOnly] private List<SOMission> completedMissions;
        
        private Dictionary<SOMission, MissionObjective[]> _missionObjectives;
        private Dictionary<SOMission, MissionObjectiveEvents[]> _missionObjectiveEvents;

        private void Awake()
        {
            if (Instance && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            
            activeMissions = new List<SOMission>();
            completedMissions = new List<SOMission>();
            _missionObjectives = new Dictionary<SOMission, MissionObjective[]>();
            _missionObjectiveEvents = new Dictionary<SOMission, MissionObjectiveEvents[]>();
            
            MissionObjective.OnObjectiveMet += OnObjectiveMet;
        }

        private void OnDestroy()
        {
            if (Instance != this) return;
            
            MissionObjective.OnObjectiveMet -= OnObjectiveMet;
                
            foreach (var kvp in _missionObjectives)
            {
                foreach (var objective in kvp.Value)
                {
                    objective.Cleanup();
                }
            }
        }
        
        private void OnObjectiveMet(MissionObjective completedObjective)
        {
            ExecuteObjectiveActions(completedObjective);
            ActivateNextObjective(completedObjective);
            StartCoroutine(CheckMissionCompletionNextFrame(completedObjective));
        }
        
        private IEnumerator CheckMissionCompletionNextFrame(MissionObjective completedObjective)
        {
            yield return null;
            CheckForMissionCompletion(completedObjective);
        }
        
        private void ExecuteObjectiveActions(MissionObjective completedObjective)
        {
            foreach (var kvp in _missionObjectives)
            {
                var mission = kvp.Key;
                var objectives = kvp.Value;
        
                for (int i = 0; i < objectives.Length; i++)
                {
                    if (objectives[i] == completedObjective)
                    {
                        if (_missionObjectiveEvents.TryGetValue(mission, out var events) && i < events.Length)
                        {
                            var eventEntry = events[i];
                            if (!eventEntry.hasTriggered)
                            {
                                eventEntry.hasTriggered = true;
                                foreach (var action in eventEntry.onObjectiveCompleted)
                                {
                                    action?.Execute();
                                }
                            }
                        }
                        return;
                    }
                }
            }
        }

        private void ActivateNextObjective(MissionObjective completedObjective)
        {
            foreach (var kvp in _missionObjectives)
            {
                var objectives = kvp.Value;
        
                for (int i = 0; i < objectives.Length - 1; i++)
                {
                    if (objectives[i] == completedObjective)
                    {
                        var nextObjective = objectives[i + 1];
                        if (nextObjective.RequiresPreviousObjective && !nextObjective.IsActive)
                        {
                            nextObjective.SetActive(true);
                        }
                        return;
                    }
                }
            }
        }

        private void CheckForMissionCompletion(MissionObjective completedObjective)
        {
            foreach (var mission in activeMissions.ToList())
            {
                if (!_missionObjectives.TryGetValue(mission, out var objectives)) continue;
        
                bool objectiveBelongsToMission = System.Array.Exists(objectives, obj => obj == completedObjective);
        
                if (objectiveBelongsToMission)
                {
                    CompleteMission(mission);
                }
            }
        }

        private void CompleteMission(SOMission mission)
        {
            if (!mission || !activeMissions.Contains(mission)) return;
            
            if (!_missionObjectives.TryGetValue(mission, out var objectives)) return;
            
            foreach (var objective in objectives)
            {
                if (!objective.Met && !objective.Evaluate())
                {
                    return;
                }
            }
            
            foreach (var objective in objectives)
            {
                objective.Cleanup();
            }
            
            activeMissions.Remove(mission);
            completedMissions.Add(mission);
            _missionObjectives.Remove(mission);
            _missionObjectiveEvents.Remove(mission);
            
            foreach (var action in mission.OnCompleted)
            {
                action?.Execute();
            }
            
            GameEvents.MissionCompleted(mission);
        }
        
        public void AddMission(SOMission mission)
        {
            if (!mission || completedMissions.Contains(mission) || activeMissions.Contains(mission)) return;
        
            var objectives = mission.CloneObjectives();
            var objectiveEvents = mission.CloneObjectiveEvents();
            
            _missionObjectives[mission] = objectives;
            _missionObjectiveEvents[mission] = objectiveEvents;
            
            for (int i = 0; i < objectives.Length; i++)
            {
                var objective = objectives[i];
                objective.Initialize();
                
                if (objective.RequiresPreviousObjective && i > 0)
                {
                    var previousObjective = objectives[i - 1];
                    objective.SetActive(previousObjective.Met);
                }
            }
        
            activeMissions.Add(mission);
            
            foreach (var action in mission.OnStarted)
            {
                action?.Execute();
            }
            
            GameEvents.MissionStarted(mission);
        }
        
        public MissionObjective[] GetMissionObjectives(SOMission mission, bool visibleOnly = false)
        {
            var objectives = _missionObjectives.GetValueOrDefault(mission);

            if (objectives == null)
                return null;

            if (visibleOnly)
            {
                return objectives.Where(obj => !obj.IsHidden && obj.IsActive).ToArray();
            }
        
            return objectives;
        }

        public bool HasMissionGiveItemFor(NPC npc, out SOItem item)
        {
            foreach (var missionObjectivesPair in _missionObjectives)
            {
                foreach (var objective in missionObjectivesPair.Value)
                {
                    if (objective is GiveItemToNpcObjective giveItemToNpcObjective)
                    {
                        if (giveItemToNpcObjective.IsNpc(npc) && !giveItemToNpcObjective.Met)
                        {
                            item = giveItemToNpcObjective.RequiredItem;
                            return true;
                        }
                    }
                }
            }

            item = null;
            return false;
        }
    }
}
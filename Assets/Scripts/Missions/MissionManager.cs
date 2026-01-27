using System;
using System.Collections.Generic;
using System.Linq;
using DNExtensions;
using UnityEngine;

public class MissionManager : MonoBehaviour
{
    public static MissionManager Instance;
    
    [SerializeField, ReadOnly] private List<SOMission> activeMissions;
    [SerializeField, ReadOnly] private List<SOMission> completedMissions;
    
    private Dictionary<SOMission, MissionObjective[]> missionObjectives;
    private Dictionary<SOMission, MissionObjectiveEvents[]> missionObjectiveEvents;

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
        missionObjectives = new Dictionary<SOMission, MissionObjective[]>();
        missionObjectiveEvents = new Dictionary<SOMission, MissionObjectiveEvents[]>();
        
        MissionObjective.OnObjectiveMet += OnObjectiveCompleted;
    }

    private void OnDestroy()
    {
        if (Instance != this) return;
        
        MissionObjective.OnObjectiveMet -= OnObjectiveCompleted;
            
        foreach (var kvp in missionObjectives)
        {
            foreach (var objective in kvp.Value)
            {
                objective.Cleanup();
            }
        }
    }

    private void OnObjectiveCompleted(MissionObjective completedObjective)
    {
        foreach (var kvp in missionObjectives)
        {
            var mission = kvp.Key;
            var objectives = kvp.Value;
        
            for (int i = 0; i < objectives.Length; i++)
            {
                if (objectives[i] == completedObjective)
                {
                    if (missionObjectiveEvents.TryGetValue(mission, out var events) && i < events.Length)
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
                    break;
                }
            }
        }
    
        CheckActiveMissionsForCompletion();
    }
    
    private void CheckActiveMissionsForCompletion()
    {
        foreach (var mission in activeMissions.ToList())
        {
            if (!missionObjectives.TryGetValue(mission, out var objectives)) continue;
            
            for (int i = 0; i < objectives.Length - 1; i++)
            {
                var currentObjective = objectives[i];
                var nextObjective = objectives[i + 1];
            
                if (currentObjective.Met && nextObjective.RequiresPreviousObjective && !nextObjective.IsActive)
                {
                    nextObjective.SetActive(true);
                }
            }
        
            CompleteMission(mission);
        }
    }

    private void CompleteMission(SOMission mission)
    {
        if (!mission || !activeMissions.Contains(mission)) return;
        
        if (!missionObjectives.TryGetValue(mission, out var objectives)) return;
        
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
        missionObjectives.Remove(mission);
        missionObjectiveEvents.Remove(mission);
        
        // Execute completion actions from SO
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
        
        missionObjectives[mission] = objectives;
        missionObjectiveEvents[mission] = objectiveEvents;
        
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
        
        // Execute start actions from SO
        foreach (var action in mission.OnStarted)
        {
            action?.Execute();
        }
        
        GameEvents.MissionStarted(mission);
    }
    
    public MissionObjective[] GetMissionObjectives(SOMission mission, bool visibleOnly = false)
    {
        var objectives = missionObjectives.GetValueOrDefault(mission);

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
        foreach (var missionObjectivesPair in missionObjectives)
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
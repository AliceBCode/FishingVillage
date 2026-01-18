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
        
        MissionObjective.OnObjectiveMet += CheckActiveMissionsForCompletion;
    }

    private void OnDestroy()
    {
        if (Instance != this) return;
        
        
        MissionObjective.OnObjectiveMet -= CheckActiveMissionsForCompletion;
            
        foreach (var kvp in missionObjectives)
        {
            foreach (var objective in kvp.Value)
            {
                objective.Cleanup();
            }
        }
    }
    
    private void CheckActiveMissionsForCompletion()
    {
        for (int i = activeMissions.Count - 1; i >= 0; i--)
        {
            CompleteMission(activeMissions[i]);
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
        
        GameEvents.MissionCompleted(mission);
    }
    
    public void AddMission(SOMission mission)
    {
        if (!mission || completedMissions.Contains(mission) || activeMissions.Contains(mission)) return;
        
        var objectives = mission.CloneObjectives();
        missionObjectives[mission] = objectives;
        
        foreach (var objective in objectives)
        {
            objective.Initialize();
        }
        
        activeMissions.Add(mission);
        GameEvents.MissionStarted(mission);
    }
    
    public MissionObjective[] GetMissionObjectives(SOMission mission, bool visibleOnly = false)
    {
        var objectives = missionObjectives.GetValueOrDefault(mission);
    
        if (objectives == null)
            return null;
    
        return visibleOnly 
            ? objectives.Where(obj => !obj.IsHidden).ToArray()
            : objectives;
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
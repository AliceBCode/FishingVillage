using System;
using System.Collections.Generic;
using DNExtensions;
using UnityEngine;

public class MissionManager : MonoBehaviour
{

    public static MissionManager Instance;
    public static event Action<SOMission> OnMissionStarted;
    public static event Action<SOMission> OnMissionCompleted;
    
    
    [SerializeField, ReadOnly] private List<SOMission> activeMissions;
    [SerializeField, ReadOnly] private List<SOMission> completedMissions;
    
    private Dictionary<SOMission, MissionCondition[]> missionConditions;

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
        missionConditions = new Dictionary<SOMission, MissionCondition[]>();
        
        MissionCondition.OnConditionMet += CheckActiveMissionsForCompletion;
    }

    private void OnDestroy()
    {
        if (Instance != this) return;
        
        
        MissionCondition.OnConditionMet -= CheckActiveMissionsForCompletion;
            
        foreach (var kvp in missionConditions)
        {
            foreach (var condition in kvp.Value)
            {
                condition.Cleanup();
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
        
        if (!missionConditions.TryGetValue(mission, out var conditions)) return;
        
        foreach (var condition in conditions)
        {
            if (!condition.Met && !condition.Evaluate())
            {
                return;
            }
        }
        
        foreach (var condition in conditions)
        {
            condition.Cleanup();
        }
        
        activeMissions.Remove(mission);
        completedMissions.Add(mission);
        missionConditions.Remove(mission);
        
        OnMissionCompleted?.Invoke(mission);
    }
    
    public void AddMission(SOMission mission)
    {
        if (!mission || completedMissions.Contains(mission) || activeMissions.Contains(mission)) return;
        
        var conditions = mission.CloneConditions();
        missionConditions[mission] = conditions;
        
        foreach (var condition in conditions)
        {
            condition.Initialize();
        }
        
        activeMissions.Add(mission);
        OnMissionStarted?.Invoke(mission);
    }
    
    public MissionCondition[] GetMissionConditions(SOMission mission)
    {
        return missionConditions.GetValueOrDefault(mission);
    }

    public bool HasMissionGiveItemFor(NPC npc, out SOItem item)
    {
        foreach (var missionConditionPair in missionConditions)
        {
            foreach (var condition in missionConditionPair.Value)
            {
                if (condition is GiveItemToNPCCondition giveItemCondition)
                {
                    if (giveItemCondition.IsNpc(npc) && !giveItemCondition.Met)
                    {
                        item = giveItemCondition.RequiredItem;
                        return true;
                    }
                }
            }
        }

        item = null;
        return false;
    }
}
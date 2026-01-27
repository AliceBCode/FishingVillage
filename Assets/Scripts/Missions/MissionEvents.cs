using System;
using UnityEngine;

[Serializable]
public class ObjectiveEventEntry
{
    [HideInInspector] public string objectiveName;
    [SerializeReference, SubclassSelector] 
    public GameAction[] actionsOnCompleted = Array.Empty<GameAction>();
    [HideInInspector] public bool hasTriggered;
}


public class MissionEvents : MonoBehaviour
{
    [SerializeField] private SOMission mission;
    
    [Header("Mission Events")]
    [SerializeReference, SubclassSelector] 
    private GameAction[] actionsOnMissionStarted = Array.Empty<GameAction>();
    [SerializeReference, SubclassSelector] 
    private GameAction[] actionsOnMissionCompleted = Array.Empty<GameAction>();
    [SerializeField] private ObjectiveEventEntry[] objectiveEvents;

    private void OnEnable()
    {
        GameEvents.OnMissionStarted += CheckMissionStarted;
        GameEvents.OnMissionCompleted += CheckMissionCompleted;
        MissionObjective.OnObjectiveMet += CheckObjectiveCompleted;
    }

    private void OnDisable()
    {
        GameEvents.OnMissionStarted -= CheckMissionStarted;
        GameEvents.OnMissionCompleted -= CheckMissionCompleted;
        MissionObjective.OnObjectiveMet -= CheckObjectiveCompleted;
    }

    private void CheckMissionStarted(SOMission startedMission)
    {
        if (startedMission == mission)
        {
            foreach (var action in actionsOnMissionStarted)
            {
                action?.Execute();
            }
        }
    }

    private void CheckMissionCompleted(SOMission completedMission)
    {
        if (completedMission == mission)
        {
            foreach (var action in actionsOnMissionCompleted)
            {
                action?.Execute();
            }
        }
    }

    private void CheckObjectiveCompleted(MissionObjective completedObjective)
    {
        if (!mission || !MissionManager.Instance) return;
        
        var objectives = MissionManager.Instance.GetMissionObjectives(mission);
        if (objectives == null) return;
        
        for (int i = 0; i < objectives.Length; i++)
        {
            if (objectives[i] == completedObjective && i < objectiveEvents.Length)
            {
                var entry = objectiveEvents[i];
                if (!entry.hasTriggered)
                {
                    entry.hasTriggered = true;
                    
                    foreach (var action in entry.actionsOnCompleted)
                    {
                        action?.Execute();
                    }
                }
                break;
            }
        }
    }
}
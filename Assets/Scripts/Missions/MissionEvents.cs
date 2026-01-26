using System;
using UnityEngine;
using UnityEngine.Events;

public class MissionEvents : MonoBehaviour
{
    [Serializable]
    public class ObjectiveEventEntry
    {
        [HideInInspector] public string objectiveName;
        public UnityEvent onObjectiveCompleted;
        [HideInInspector] public bool hasTriggered;
    }
    
    
    [SerializeField] private SOMission mission;
    [SerializeField] private UnityEvent onMissionStarted;
    [SerializeField] private UnityEvent onMissionCompleted;
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
            onMissionStarted?.Invoke();
        }
    }

    private void CheckMissionCompleted(SOMission completedMission)
    {
        if (completedMission == mission)
        {
            onMissionCompleted?.Invoke();
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
                    entry.onObjectiveCompleted?.Invoke();
                }
                break;
            }
        }
    }
    
}
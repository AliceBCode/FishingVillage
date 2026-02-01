using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class ObjectiveSceneEvent
{
    [HideInInspector] public string objectiveName;
    public UnityEvent onCompleted;
    [HideInInspector] public bool hasTriggered;
}

public class MissionEventsListener : MonoBehaviour
{
    [SerializeField] private SOMission mission;
    [SerializeField] private UnityEvent onMissionStarted;
    [SerializeField] private ObjectiveSceneEvent[] objectiveEvents;
    [SerializeField] private UnityEvent onMissionCompleted;

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
                    entry.onCompleted?.Invoke();
                }
                break;
            }
        }
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(256, 256, 256, 0.5f);
        Gizmos.DrawSphere(transform.position, 0.2f);

        var style = new GUIStyle()
        {
            fontSize = 8,
            normal = { textColor = Color.white },
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.MiddleCenter,
        };
        var state = mission ? mission.name : "No mission was set";
        Handles.Label(transform.position + new Vector3(0,0.5f), $"Mission Event Listener:\n{state}", style);
    }
}
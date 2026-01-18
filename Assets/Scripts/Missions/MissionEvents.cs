using UnityEngine;
using UnityEngine.Events;

public class MissionEvents : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("The mission to subscribe too")]
    [SerializeField] private SOMission mission;
    [SerializeField] private UnityEvent onMissionStarted;
    [SerializeField] private UnityEvent onMissionCompleted;

    private void OnEnable()
    {
        GameEvents.OnMissionStarted += CheckMissionStarted;
        GameEvents.OnMissionCompleted += CheckMissionCompleted;
    }

    private void OnDisable()
    {
        GameEvents.OnMissionStarted -= CheckMissionStarted;
        GameEvents.OnMissionCompleted -= CheckMissionCompleted;
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
}
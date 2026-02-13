using System;
using DNExtensions.Utilities;
using DNExtensions.Utilities.Inline;
using FishingVillage.Missions.Objectives;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace FishingVillage.Missions
{
    [Serializable]
    public class ObjectiveSceneEvent
    {
        public UnityEvent onCompleted;
        [HideInInspector] public bool hasTriggered;
    }

    public class MissionEventsListener : MonoBehaviour
    {
        [Space(15)]
        [SerializeField, Inline] private SOMission mission;
        [Separator]
        [Space(15)]
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
    
    }
}
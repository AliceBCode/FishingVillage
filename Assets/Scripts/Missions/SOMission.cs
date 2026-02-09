using System;
using DNExtensions.Utilities.SerializableSelector;
using FishingVillage.Missions.Objectives;
using UnityEngine;

namespace FishingVillage.Missions
{
    [Serializable]
    public class MissionObjectiveEvents
    {
        [SerializeReference, SerializableSelector]
        public GameActions.GameAction[] onObjectiveCompleted = Array.Empty<GameActions.GameAction>();
        [HideInInspector] public bool hasTriggered;
    }

    [CreateAssetMenu(fileName = "New Mission", menuName = "Scriptable Objects/Mission")]
    public class SOMission : ScriptableObject
{
    [Header("Settings")]
    [SerializeField] private new string name;
    [SerializeField, TextArea] private string description;
    [SerializeField] private Sprite icon;
    [SerializeReference, SerializableSelector] 
    private MissionObjective[] objectives = Array.Empty<MissionObjective>();
    
    [Header("Events")]
    [SerializeReference, SerializableSelector]
    private GameActions.GameAction[] onStarted = Array.Empty<GameActions.GameAction>();
    [SerializeField]
    private MissionObjectiveEvents[] onObjectiveCompleted = Array.Empty<MissionObjectiveEvents>();
    [SerializeReference, SerializableSelector]
    private GameActions.GameAction[] onCompleted = Array.Empty<GameActions.GameAction>();



    public string Name => name;
    public string Description => description;
    public Sprite Icon => icon;
    public GameActions.GameAction[] OnStarted => onStarted;
    public GameActions.GameAction[] OnCompleted => onCompleted;
    
    
    public MissionObjective[] CloneObjectives()
    {
        var clones = new MissionObjective[objectives.Length];
    
        for (int i = 0; i < objectives.Length; i++)
        {
            if (objectives[i] == null)
            {
                clones[i] = null;
                continue;
            }
        
            clones[i] = (MissionObjective)Activator.CreateInstance(objectives[i].GetType());
            JsonUtility.FromJsonOverwrite(JsonUtility.ToJson(objectives[i]), clones[i]);
        }
    
        return clones;
    }
    
    public MissionObjectiveEvents[] CloneObjectiveEvents()
    {
        var clones = new MissionObjectiveEvents[onObjectiveCompleted.Length];
    
        for (int i = 0; i < onObjectiveCompleted.Length; i++)
        {
            clones[i] = new MissionObjectiveEvents();
            JsonUtility.FromJsonOverwrite(JsonUtility.ToJson(onObjectiveCompleted[i]), clones[i]);
        }
    
        return clones;
    }
    
    public void StartMission()
    {
        if (MissionManager.Instance)
        {
            MissionManager.Instance.AddMission(this);
        }
    }
    }
}
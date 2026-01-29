using System;
using DNExtensions.Utilities.SerializableSelector;
using UnityEngine;

[Serializable]
public class MissionObjectiveEvents
{
    [HideInInspector] public string objectiveName;
    [SerializeReference, SerializableSelector] 
    public GameAction[] onObjectiveCompleted = Array.Empty<GameAction>();
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
    private GameAction[] onStarted = Array.Empty<GameAction>();
    [SerializeField] 
    private MissionObjectiveEvents[] onObjectiveCompleted = Array.Empty<MissionObjectiveEvents>();
    [SerializeReference, SerializableSelector] 
    private GameAction[] onCompleted = Array.Empty<GameAction>();
    

    
    public string Name => name;
    public string Description => description;
    public Sprite Icon => icon;
    public GameAction[] OnStarted => onStarted;
    public GameAction[] OnCompleted => onCompleted;
    
    
    public MissionObjective[] CloneObjectives()
    {
        var clones = new MissionObjective[objectives.Length];
        
        for (int i = 0; i < objectives.Length; i++)
        {
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
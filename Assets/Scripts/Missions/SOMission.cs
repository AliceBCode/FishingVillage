using System;
using UnityEngine;

[CreateAssetMenu(fileName = "New Mission", menuName = "Scriptable Objects/Mission")]
public class SOMission : ScriptableObject
{
    [Header("Settings")]
    [SerializeField] private new string name;
    [SerializeField, TextArea] private string description;
    [SerializeField] private Sprite icon;

    [SerializeReference, SubclassSelector] private MissionObjective[] objectives = Array.Empty<MissionObjective>();
    
    public string Name => name;
    public string Description => description;
    public Sprite Icon => icon;
    
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
    
    public void StartMission()
    {
        if (MissionManager.Instance)
        {
            MissionManager.Instance.AddMission(this);
        }
    }
}
using System;
using UnityEngine;

[CreateAssetMenu(fileName = "New Mission", menuName = "Scriptable Objects/Mission")]
public class SOMission : ScriptableObject
{
    [Header("Settings")]
    [SerializeField] private new string name;
    [SerializeField, TextArea] private string description;
    [SerializeField] private Sprite icon;

    [SerializeReference, SubclassSelector] private MissionCondition[] conditions = Array.Empty<MissionCondition>();
    
    public string Name => name;
    public string Description => description;
    public Sprite Icon => icon;
    
    public MissionCondition[] CloneConditions()
    {
        var clones = new MissionCondition[conditions.Length];
        
        for (int i = 0; i < conditions.Length; i++)
        {
            clones[i] = (MissionCondition)Activator.CreateInstance(conditions[i].GetType());
            JsonUtility.FromJsonOverwrite(JsonUtility.ToJson(conditions[i]), clones[i]);
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
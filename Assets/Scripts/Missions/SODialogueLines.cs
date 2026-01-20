using System;
using DNExtensions;
using UnityEngine;

[CreateAssetMenu(fileName = "New Dialogue Lines", menuName = "Scriptable Objects/Dialogue Lines")]
public class SODialogueLines : ScriptableObject
{

    [SerializeField] private ChanceList<string> dialogueLines;
    
    
    
    public int Count => dialogueLines.Count;

    public string GetRandomLine => dialogueLines.GetRandomItem();
}
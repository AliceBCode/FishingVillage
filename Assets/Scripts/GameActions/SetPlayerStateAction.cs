using System;
using DNExtensions.Utilities.SerializableSelector;
using UnityEngine;

[Serializable]
[SerializableSelectorName("Set State", "Player")]
public class SetPlayerStateAction : GameAction
{
    [SerializeField] private PlayerState state = PlayerState.Normal;
    
    public override string ActionName => $"Sets player state to: {state}";
    
    public override void Execute()
    {
        if (PlayerController.Instance)
        {
            PlayerController.Instance.SetState(state);
        }
    }
}